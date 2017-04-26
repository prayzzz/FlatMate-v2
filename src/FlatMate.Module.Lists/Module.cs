﻿using FlatMate.Module.Lists.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Module.Lists
{
    public class Module
    {
        public static void ConfigureServices(IServiceCollection service, IConfigurationRoot configuration)
        {
            service.AddDbContext<ListsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}