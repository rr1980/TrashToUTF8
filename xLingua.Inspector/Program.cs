using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using xLingua.Entities;
using xLingua.Inspector.Core;

namespace xLingua.Inspector
{
    class Program
    {
        static char[] searchChars = new char[] {
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

        static char[] blackChars = new char[] { 'ￅ', '�', '¬', '±', };

        static void Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.UTF8;



                //var _results = Parser.Parse<Statistic>(x=>x.Id, x => x.Keyword, searchChars, new Resolver(searchChars, blackChars));
                var _results = Parser.Parse<Words>(x=>x.Id, x => x.Word, searchChars, new Resolver(searchChars, blackChars));

                Console.ReadLine();

                foreach (var item in _results)
                {
                    Console.WriteLine(string.Format("{0,-10} {1,50}", item.Id, item.Word));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Debug.WriteLine("ERROR: " + ex.Message + Environment.NewLine + JsonConvert.SerializeObject(ex, Formatting.Indented));
            }

            Console.ReadLine();
        }


    }
}