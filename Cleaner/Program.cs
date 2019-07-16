using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cleaner
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TrashToUTF8";

            var configurationBuilder = new ConfigurationBuilder();
            //.AddInMemoryCollection(congig);

            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = configurationBuilder.Build();

            var serviceProvider = RunContainer.Build<AppRunner>(serviceCollection =>
            {
                serviceCollection.AddSingleton<IConfiguration>(configuration);
                serviceCollection.Configure<AppSettings>(configuration);

                serviceCollection.AddLogging(loggingBuilder =>
                {
                    //loggingBuilder.ClearProviders();

                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                    loggingBuilder.AddConfiguration(configuration.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug();

                });

                serviceCollection.AddDbContext<DataDbContext>(options =>
                {
                    options.UseMySQL(configuration.GetConnectionString("DefaultConnection"));
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                });

                serviceCollection.AddSingleton<IAppTesterService, AppTester1>();

            });

            var runner = serviceProvider.GetRequiredService<IRunner>();

            Task.Run(() => runner.Execute()).Wait();

            Console.ReadKey();
        }
    }
}
