using System;
using Cleaner.Core;
using Cleaner.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner
{
    public class AppTester1 : IRunnerService
    {
        private readonly ILogger<AppRunner> _logger;
        private readonly AppSettings _appSettings;

        public AppTester1(ILogger<AppRunner> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            _logger.LogDebug("AppTester1 init...");
        }

        public void Execute()
        {
            _logger.LogDebug("AppTester1 execute...");

            //do
            //{


            //} while (!string.IsNullOrEmpty(Console.ReadLine()));

            Stop();
        }

        public void Stop()
        {
            _logger.LogDebug("AppTester1 stop...");
        }
    }

    public class AppTester2 : IRunnerService
    {
        private readonly ILogger<AppRunner> _logger;
        private readonly AppSettings _appSettings;

        public AppTester2(ILogger<AppRunner> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            _logger.LogDebug("AppTester2 init...");
        }

        public void Execute()
        {
            _logger.LogDebug("AppTester2 execute...");

            //do
            //{


            //} while (!string.IsNullOrEmpty(Console.ReadLine()));

            Stop();
        }

        public void Stop()
        {
            _logger.LogDebug("AppTester2 stop...");
        }
    }
}
