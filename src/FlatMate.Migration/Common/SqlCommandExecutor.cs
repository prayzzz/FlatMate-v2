using System.Data;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Common
{
    public class SqlCommandExecutor
    {
        private readonly ILogger _logger;

        public SqlCommandExecutor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public int ExecuteNonQuery(IDbCommand sqlCommand)
        {
            _logger.LogDebug($"Executing command '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(IDbCommand sqlCommand)
        {
            _logger.LogDebug($"Executing command '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteReader();
        }

        public object ExecuteScalar(IDbCommand sqlCommand)
        {
            _logger.LogDebug($"Executing command '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteScalar();
        }
    }
}