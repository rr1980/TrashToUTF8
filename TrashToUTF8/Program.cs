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


                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
