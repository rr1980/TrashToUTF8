using System;
using System.Linq;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner
{
    public class AppTester1 : IAppTesterService
    {
        private readonly ILogger<AppRunner> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;

        public AppTester1(ILogger<AppRunner> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _dataDbContext = dataDbContext;

            _logger.LogDebug("AppTester1 init...");
        }


        public void Stop()
        {
            _logger.LogDebug("AppTester1 stop...");
        }

        public void Test()
        {
            _logger.LogInformation("AppTester1 Test...");
            var t = _dataDbContext.BaseWords.FirstOrDefault();
        }
    }
}
