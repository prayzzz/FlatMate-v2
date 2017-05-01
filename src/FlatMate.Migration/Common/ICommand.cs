using System.Collections.Generic;
using prayzzz.Common.Results;

namespace FlatMate.Migration.Common
{
    public interface ICommand
    {
        IEnumerable<string> CommandNames { get; }

        Result Execute(IEnumerable<string> arguments);
    }
}