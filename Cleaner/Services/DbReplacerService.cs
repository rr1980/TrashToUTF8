using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace Cleaner.Services
{
    public class DbReplacerService : IDbReplacerService
    {
        public static string[] SearchStrings = new string[] {
            "ini",
            //"od",
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

        private readonly ILogger<DbReplacerService> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;

        public DbReplacerService(ILogger<DbReplacerService> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _dataDbContext = dataDbContext;

            _logger.LogDebug("DbReplacerService init...");
        }


        public void Stop()
        {
            _logger.LogDebug("DbReplacerService stop...");
        }

        public async Task Test()
        {
            Regex regex = new Regex("(.+)(ini)(.+)");

            var timer = new Stopwatch();
            timer.Start();

            var words = _dataDbContext.Words.AsNoTracking();
            foreach (var item in SearchStrings)
            {
                words = words.Where(x => x.Word.Contains(item));
            }

            var t3 = await words.ToListAsync();

            var count = t3.Count();

            timer.Stop();
            var ms = timer.ElapsedMilliseconds;

            _logger.LogCritical("Time: {0} / Count: {1}", ms, count);

            //foreach (var item in t3)
            //{
            //    Console.WriteLine(item.Word);
            //}


            await Task.CompletedTask;
        }

        #region Replace
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
        #endregion
    }
}
