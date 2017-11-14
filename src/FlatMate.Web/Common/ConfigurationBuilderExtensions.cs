using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FlatMate.Web.Common
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// PRODUCTION AND STAGING:
        /// Reads user and password which are configuration separatly e.g. environment variables
        /// and build a new default connection string.
        /// 
        /// For environment variables use:
        /// [PREFIX]_db__user and [PREFIX]_db_password 
        /// </summary>
        public static IConfigurationBuilder AddProductionConnection(this IConfigurationBuilder builder, IHostingEnvironment env)
        {
            var config = builder.Build();

            if (env.IsStaging() || env.IsProduction())
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(config.GetConnectionString("Production"))
                {
                    UserID = config.GetValue<string>("db:user"),
                    Password = config.GetValue<string>("db:password")
                };

                builder.AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:DefaultConnection", connectionStringBuilder.ConnectionString } });
            }

            return builder;
        }
    }
}