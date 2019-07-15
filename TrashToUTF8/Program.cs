using System;
using System.Text;

namespace TrashToUTF8
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.UTF8;

                Parser p = new Parser();
                p.Start();

                //Test_Find();
                //Test_Convert();

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        private static void Test_Convert()
        {
            var tmp1 = Tester.Convert("Ð²Ñ‹Ñ€Ð°ÑÑ‚Ð°ÑŽÑ‰Ðµï¿½");
            var tmp2 = Tester.Convert(tmp1);
        }

        private static void Test_Find()
        {
            var text = Tester.Load(Config.TargetPath);
            var matches = Tester.Find("'([^0-9a-zA-Z,]+)'", text);

            var count = matches.Count;

            foreach (var item in matches)
            {
                Console.WriteLine(item);
            }
        }
    }
}
