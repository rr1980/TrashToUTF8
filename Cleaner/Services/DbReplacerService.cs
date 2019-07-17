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
using System.Collections.Generic;
using Cleaner.Core.DB.Entities;

namespace Cleaner.Services
{
    class ClearResult
    {
        public bool Ok { get; set; }
        public string Text { get; set; }
    }

    class CheckResult
    {
        public bool Found { get; set; }
        public char FoundChar { get; set; }
    }

    public class DbReplacerService : IDbReplacerService
    {
        public static string[] SearchStrings = new string[] {
            "ini",
            //"od",
        };

        public static char[] SearchChars = new char[] {
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
            //'Ñ',
        };

        public static char[] BlackChars = new char[] {
            'ￅ',
            '�',
            '¬',
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

        public async Task Test_Words()
        {
            Regex regex = new Regex("(.+)(ini)(.+)");


            var entities = await _dataDbContext.Words
                //.Where(x=>x.Language.Id == 9)
                .ToListAsync(); ;



            List<Words> results = new List<Words>();
            foreach (var item in SearchChars)
            {
                results.AddRange(entities.Where(x => x.Word.Contains(item)));
            }


            var count = results.Count();


            try
            {
                foreach (var item in results)
                {
                    var result = Clear(item.Id, item.Word, item.BaseWordLinks.FirstOrDefault()?.BaseWord?.Language?.EnglishName);
                    if (result.Ok)
                    {
                        item.Word = result.Text;
                        //_dataDbContext.Update(item);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            //_dataDbContext.SaveChanges();

            Console.WriteLine("FERTIG!");

            await Task.CompletedTask;
        }

        public async Task Test_BaseWords()
        {
            Regex regex = new Regex("(.+)(ini)(.+)");


            var entities = await _dataDbContext.BaseWords
                .Include(x=>x.Language)
                //.Where(x=>x.Language.Id == 9)
                .ToListAsync(); ;



            List<BaseWords> results = new List<BaseWords>();
            foreach (var item in SearchChars)
            {
                results.AddRange(entities.Where(x => x.Word.Contains(item)));
            }


            var count = results.Count();



            foreach (var item in results)
            {
                var result = Clear(item.Id, item.Word, item.Language.EnglishName);
                if (result.Ok)
                {
                    item.Word = result.Text;
                    //_dataDbContext.Update(item);
                }
            }

            //_dataDbContext.SaveChanges();

            Console.WriteLine("FERTIG!");

            await Task.CompletedTask;
        }

        private ClearResult Clear(long id, string dirtyWord, string language = null)
        {
            var oldWord = dirtyWord;

            while (CheckSearchChars(dirtyWord).Found)
            {
                var newWord = Convert(dirtyWord);

                if (CheckBlackChars(newWord))
                {
                    Console.WriteLine(string.Format("NO  {0,-10} {1,50} = {2,-30} {3,-20}",id, oldWord, newWord, language));
                    return new ClearResult
                    {
                        Text = "'" + oldWord + "'",
                        Ok = false
                    };
                }
                else
                {
                    dirtyWord = newWord;
                }
            }

            Console.WriteLine(string.Format("NO  {0,-10} {1,50} = {2, -30} {3,-20} FIXED", id, oldWord, dirtyWord, language));

            return new ClearResult
            {
                Text = "'" + dirtyWord + "'",
                Ok = true
            };
        }

        private CheckResult CheckSearchChars(string row)
        {
            foreach (var item in SearchChars)
            {
                if (row.Contains(item))
                {
                    return new CheckResult
                    {
                        Found = true,
                        FoundChar = item
                    };
                }
            }

            return new CheckResult
            {
                Found = false,
            };
        }

        private bool CheckBlackChars(string row)
        {
            foreach (var item in BlackChars)
            {
                if (row.Contains(item))
                {
                    return true;
                }
            }

            return false;
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
