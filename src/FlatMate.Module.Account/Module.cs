using App.Metrics;
using App.Metrics.Counter;
using FlatMate.Module.Account.DataAccess;
using FlatMate.Module.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Account
{
    public class Module : FlatMateModule
    {
        public static CounterOptions FailedLoginAttempts => new CounterOptions
        {
            Name = "Failed Login Attemps",
            MeasurementUnit = Unit.Calls,
        };

        public override void ConfigureServices(IServiceCollection service, IConfiguration configuration)
        {
            base.ConfigureServices(service, configuration);
            service.AddDbContext<AccountDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
