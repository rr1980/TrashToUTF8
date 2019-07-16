using System;
using Cleaner.Core;
using Cleaner.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner
{

    public class AppRunner : IRunner
    {
        private readonly ILogger<AppRunner> _logger;
        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppTesterService _appTesterService;

        public AppRunner(ILogger<AppRunner> logger, IOptions<AppSettings> appSettings, IServiceProvider serviceProvider, IAppTesterService appTesterService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _serviceProvider = serviceProvider;
            _appTesterService = appTesterService;

            _logger.LogDebug("AppRunner init...");
        }

        public void Execute()
        {
            _logger.LogDebug("AppRunner execute...");
            _logger.LogInformation("Start...");



            _appTesterService.Test();

            do
            {


            } while (!string.IsNullOrEmpty(Console.ReadLine()));

            Stop();
        }

        public void Stop()
        {
            foreach (var item in _serviceProvider.GetServices<IAppTesterService>())
            {
                item.Stop();
            }

            _logger.LogDebug("AppRunner stop...");
        }
    }
}
