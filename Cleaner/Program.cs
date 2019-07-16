using System;
using System.Collections.Generic;
using System.IO;
using Cleaner.Core;
using Cleaner.Interfaces;
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
                    loggingBuilder.ClearProviders();

                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                    loggingBuilder.AddConfiguration(configuration.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug();

                });

                serviceCollection.AddSingleton<IRunnerService, AppTester1>();
                serviceCollection.AddSingleton<IRunnerService, AppTester2>();

            });

            serviceProvider.GetRequiredService<IRunner>().Execute();

            Console.ReadKey();
        }
    }
}
