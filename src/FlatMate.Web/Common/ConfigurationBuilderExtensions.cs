using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FlatMate.Web.Common
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Reads user and password which are configuration separatly e.g. environment variables
        /// and build a new default connection string.
        ///
        /// For environment variables use:
        /// User: [PREFIX]_connectionstrings__flatmate__user
        /// Password: [PREFIX]_connectionstrings__flatmate_password
        /// </summary>
        public static IConfigurationBuilder BuildConnectionString(this IConfigurationBuilder builder)
        {
            var config = builder.Build();

            var connections = config.GetSection("ConnectionStrings");
            var connectionStringBuilder = new SqlConnectionStringBuilder(connections["Flatmate:Url"])
            {
                UserID = connections["Flatmate:User"],
                Password = connections["Flatmate:Password"]
            };

            builder.AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:DefaultConnection", connectionStringBuilder.ConnectionString } });

            return builder;
        }
    }
}