using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FlatMate.Web.Common
{
    public static class ConfigurationBuilderExtensions
    {
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