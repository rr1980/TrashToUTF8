using System;
using System.Collections.Generic;
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
        public static List<string> SearchChars = new List<string> {
            "Ð",
            "Å",
            "Ã",
            "©",
            "º",
            "Ã",
            "‡",
            "™",
            "…",
            "Å",
            "¾",
            "†",
            "»",
            "°",
            "Ñ",
        };

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

            //var t1 = _dataDbContext.BaseWords.FirstOrDefault();

            var t2 = _dataDbContext.BaseWords.Where(w => SearchChars.Contains(w.Word)).ToList();
        }
    }
}
