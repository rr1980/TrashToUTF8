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

        public async Task Replace_K433()
        {

            _logger.LogInformation("AppTester1 Test...");

            var t2 = await _dataDbContext.Words.Where(w => w.Word.Contains(SearchStrings[0])).ToListAsync();

            Regex regex = new Regex("(.+)(\\+K433)");

            foreach (var item in t2)
            {
                var converted = Convert(item.Word);

                var newWord = regex.Replace(converted, ReplaceDelegate);

                _logger.LogInformation(string.Format("{0,-30} = {1,-30} = {2}", item.Word, converted, newWord));

                item.Word = newWord;
                _dataDbContext.Update(item);
            }

            await _dataDbContext.SaveChangesAsync();
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
