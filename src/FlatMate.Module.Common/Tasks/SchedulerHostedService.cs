using FlatMate.Module.Common.Tasks.Cron;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlatMate.Module.Common.Tasks
{
    public class SchedulerHostedService : HostedService
    {
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<ScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                var wrapper = new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime + scheduledTask.InitialDelay
                };

                if (!scheduledTask.RunOnStartup)
                {
                    wrapper.Increment();
                }

                _scheduledTasks.Add(wrapper);
            }
        }

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            foreach (var task in _scheduledTasks)
            {
                if (!task.ShouldRun(referenceTime))
                {
                    continue;
                }

                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            public DateTime LastRunTime { get; set; }

            public DateTime NextRunTime { get; set; }

            public CrontabSchedule Schedule { get; set; }

            public ScheduledTask Task { get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}
