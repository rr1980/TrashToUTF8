using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Cleaner
{
    public class AppTester1 : IAppTesterService
    {
        public static string[] SearchStrings = new string[] {
            "+K433"
        };

        public static char[] SearchChars = new char[] {
            'Ð',
            'Å',
            'Ã',
            '©',
            'º',
            'Ã',
            '‡',
            '™',
            '…',
            'Å',
            '¾',
            '†',
            '»',
            '°',
            'Ñ',
        };

        private readonly ILogger<AppTester1> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;

        public AppTester1(ILogger<AppTester1> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext)
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

        public async Task Test()
        {

            _logger.LogInformation("AppTester1 Test...");

            //var t1 = _dataDbContext.BaseWords.FirstOrDefault();
            //  kÃ²lÃ¡yá»+K433
            //var t2 = await _dataDbContext.Words.Where(w => w.Word.IndexOfAny(SearchChars) != -1).ToListAsync();
            var t2 = await _dataDbContext.Words.Where(w => w.Word.Contains(SearchStrings[0])).ToListAsync();

            //var words = string.Join(Environment.NewLine, t2.Select(x => string.Format("{0,-60} = {1}", x.Word, Convert(x.Word))));

            //_logger.LogInformation(words);

            //Regex regex = new Regex(SearchStrings[0]);
            Regex regex = new Regex("(.+)(\\+K433)");

            foreach (var item in t2)
            {
                var converted = Convert(item.Word);

                var newWord = regex.Replace(converted, ReplaceDelegate);

                //var newWord = converted.Replace(SearchStrings[0], "a");


                _logger.LogInformation(string.Format("{0,-30} = {1,-30} = {2}", item.Word, converted, newWord));
                item.Word = newWord;
            }

            //var t2 = await _dataDbContext.BaseWords.Where(w => w.Word.Contains('Ã')).ToListAsync();
        }

        private string ReplaceDelegate(Match match)
        {
            return match.Groups[1].Value;
        }

        public string Convert(string sourceText)
        {
            byte[] asciiBytes = Encoding.GetEncoding("windows-1252").GetBytes(sourceText);
            char[] asciiChars = Encoding.UTF8.GetChars(asciiBytes);

            return new string(asciiChars);
        }
    }
}
