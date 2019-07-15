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

                //var tmp1 = p.Convert("Ð²Ñ‹Ñ€Ð°ÑÑ‚Ð°ÑŽÑ‰Ðµï¿½");
                //var tmp2 = p.Convert(tmp1);

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
