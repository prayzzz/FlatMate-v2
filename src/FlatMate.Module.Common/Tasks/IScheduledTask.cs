using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlatMate.Module.Common.Tasks
{
    public interface IScheduledTask
    {
        /// <summary>
        ///     Format: [minutes] [hours] [days] [months] [days of week]
        /// </summary>
        string Schedule { get; }

        TimeSpan InitialDelay { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
