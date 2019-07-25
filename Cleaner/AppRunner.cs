using System;
using System.Linq;
using Cleaner.Core;
using Cleaner.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using xLingua.Entities;

namespace Cleaner
{

    public class AppRunner : IRunner
    {
        public static char[] SearchChars = new char[] {
            '�',
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

            //_dbReplacerService.Replace<Characters>(x => x.Id, x => x.Name, SearchChars, BlackChars, null, false).Wait();

            //_dbReplacerService.FindHugos<BaseWords>(x => x.Id, x => x.Word, x => x.Language.EnglishName, new char[] { '�' }, null).Wait();

            //_dbReplacerService.FindHugos<Words>(x => x.Id, x => x.Word, x => x.BaseWordLinks.First().BaseWord.Language.EnglishName, new char[] { '�' },
            //    new string[] { "Word", "BaseWord.Language" },
            //    //x => x.Id > 10 && x.Id < 1000, 
            //    null,
            //    "Words"
            //    ).Wait();


            _dbReplacerService.ReplaceHugos<BaseWords>(x => x.Id, x => x.Word, x => x.Language.EnglishName, new char[] { '�' },
            //_dbReplacerService.ReplaceHugos<Words>(x => x.Id, x => x.Word, x => x.BaseWordLinks.First().BaseWord.Language.EnglishName, new char[] { '�' },
                new string[] { "Word", "BaseWord.Language" },
                //x => x.Id > 100000 && x.Id < 1000000, 
                null,
                null
                ).Wait();

            //_dbReplacerService.FindHugos<Connections>(x => x.Word.Id, x => x.Word.Word, x => x.BaseWord.Language.EnglishName, new char[] { '�' }, 
            //    new Expression<Func<Connections, object>>[] {
            //        x =>x.BaseWord.Language,
            //        x =>x.Word
            //    }
            //    , x=> x.Id > 10 && x.Id < 1000
            //    , "Words").Wait();

            do
            {


            } while (!string.IsNullOrEmpty(Console.ReadLine()));

            Stop();
        }

        public void FindHugos()
        {
            
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
