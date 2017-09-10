using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlatMate.Module.Common.Tasks
{
    public abstract class ScheduledTask
    {
        /// <summary>
        ///     Format: [minutes] [hours] [days] [months] [days of week]
        /// </summary>
        public abstract string Schedule { get; }

        public virtual TimeSpan InitialDelay => TimeSpan.FromMinutes(1);

        public virtual bool RunOnStartup => false;

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
