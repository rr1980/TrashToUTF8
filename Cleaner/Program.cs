using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Cleaner.Services;
using Cleaner.Services.Replace;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Cleaner
{

    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;

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

                serviceCollection.AddDbContext<DataDbContext>(options =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseMySql(configuration.GetConnectionString("DefaultConnection"), b=> {
                        b.UnicodeCharSet(CharSet.Utf8mb4);
                    });

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                });

                serviceCollection.AddSingleton<IDbReplacerService, DbReplacerService>();
                serviceCollection.AddSingleton<IDbInfoService, DbInfoService>();

            });

            var runner = serviceProvider.GetRequiredService<IRunner>();

            try
            {
                Run(runner).Wait();
            }
            catch (Exception ex)
            {
                throw;
            }

            Console.ReadKey();
        }

        static async Task Run(IRunner runner)
        {
            try
            {
                await Task.Run(() => runner.Execute()).ContinueWith((t) =>
                {
                    if (t.IsFaulted) throw t.Exception;
                });
            }

            catch (Exception e)
            {
                throw;
            }
        }
    }
}
