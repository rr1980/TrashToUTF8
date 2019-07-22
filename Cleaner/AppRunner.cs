using System;
using Cleaner.Core;
using Cleaner.Core.DB.Entities;
using Cleaner.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner
{

    public class AppRunner : IRunner
    {
        public static char[] SearchChars = new char[] {
            'Â',
            'Ã',
            '«',
            '‘',
            '¹',
            '“',
            'Ã',
            'Ð',
            'Å',
            '©',
            'º',
            '‡',
            '™',
            '…',
            'Å',
            '¾',
            '†',
            '»',
            '°',
            'Ñ',
            //'â',
        };

        public static char[] BlackChars = new char[] {
            'ￅ',
            '�',
            '¬',
            '±',
        };

        private readonly ILogger<AppRunner> _logger;
        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbReplacerService _dbReplacerService;
        private readonly IDbInfoService _dbInfoService;

        public AppRunner(ILogger<AppRunner> logger, IOptions<AppSettings> appSettings, IServiceProvider serviceProvider, 
            IDbReplacerService dbReplacerService,
            IDbInfoService dbInfoService
            )
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _serviceProvider = serviceProvider;
            _dbReplacerService = dbReplacerService;
            _dbInfoService = dbInfoService;

            _logger.LogDebug("AppRunner init...");
        }

        public void Execute()
        {
            _logger.LogDebug("AppRunner execute...");
            _logger.LogInformation("Start...");


            //_dbInfoService.SearchWordsWithotConnection();
            _dbReplacerService.Replace<Characters>(x => x.Id, x => x.Name, SearchChars, BlackChars
                //x=>x.Id == 1930
                , null
                , 
                //true
                false
                ).Wait();

            do
            {


            } while (!string.IsNullOrEmpty(Console.ReadLine()));

            Stop();
        }

        public void Stop()
        {
            foreach (var item in _serviceProvider.GetServices<IDbReplacerService>())
            {
                item.Stop();
            }

            _logger.LogDebug("AppRunner stop...");
        }
    }
}
