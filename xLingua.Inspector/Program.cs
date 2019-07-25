using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using xLingua.Entities;
using xLingua.Inspector.Core;
using xLingua.Inspector.Resolver;

namespace xLingua.Inspector
{
    class Program
    {
        static char[] searchChars = new char[] {

                    '�',
                    //'Â','Ã','«','‘','¹','“','Ã','Ð','Å','©','º','‡','™','…','Å','¾','†','»','°','Ñ',
                    //'â',
                };

        static char[] blackChars = new char[] { 'ￅ', '�', '¬', '±', '?' };

        static string logPath = @"D:\Projekte\TrashToUTF8\xLingua.Inspector\Logs\";

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;

            Run();

            Console.ReadLine();
        }

        private static void Run()
        {
            try
            {
                var rlySave = false;

                //Start<Basewordexamples>(x => x.Id, x => x.Text, null, rlySave);

                //Start<Feedback>(x => x.Id, x => x.Text, null, rlySave);
                //Start<Feedback>(x => x.Id, x => x.Comment, null, rlySave);

                ////Start<Universal>(x => x.Id, x => x.Word, null, rlySave);

                //Start<Statistic>(x => x.Id, x => x.Keyword, null, rlySave);

                //Start<Functions>(x => x.Id, x => x.Function, null, rlySave);

                //Start<Grammar>(x => x.Id, x => x.Title, null, rlySave);
                //Start<Grammar>(x => x.Id, x => x.Text, null, rlySave);

                //Start<LanguageTranslations>(x => x.Id, x => x.Language, null, rlySave);
                //Start<LanguageTranslations>(x => x.Id, x => x.Name, null, rlySave);
                //Start<LanguageTranslations>(x => x.Id, x => x.Fromlanguage, null, rlySave);
                //Start<LanguageTranslations>(x => x.Id, x => x.Tolanguage, null, rlySave);
                //Start<LanguageTranslations>(x => x.Id, x => x.Fromurl, null, rlySave);
                //Start<LanguageTranslations>(x => x.Id, x => x.Tourl, null, rlySave);

                //Start<Languages>(x => x.Id, x => x.Code, null, rlySave);
                //Start<Languages>(x => x.Id, x => x.NativeName, null, rlySave);
                //Start<Languages>(x => x.Id, x => x.EnglishName, null, rlySave);
                //Start<Languages>(x => x.Id, x => x.CultureName, null, rlySave);

                ////Start<Characters>(x => x.Id, x => x.Name, null, rlySave);
                ////Start<Characters>(x => x.Id, x => x.Tolerant, null, rlySave);

                Start<Words>(x => x.Id, x => x.Word, rlySave);

                Start<BaseWords>(x => x.Id, x => x.Word, rlySave);

                Console.WriteLine("Alles FERTIG!!!");

                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Debug.WriteLine("ERROR: " + ex.Message + Environment.NewLine + JsonConvert.SerializeObject(ex, Formatting.Indented));
            }
        }

        private static List<T> Start<T>(Expression<Func<T, long>> idProp, Expression<Func<T, string>> columnProp, bool rlySave = false) where T : class, IEntity
        {
            return new Parser<T>(idProp, columnProp, searchChars, new Utf8Resolver<T>(columnProp, searchChars, blackChars, logPath),  rlySave).Parse();
        }
    }
}