﻿using System;
using System.Globalization;
using System.Text;

namespace TrashToUTF8
{

    class Program
    {
        static Encoding SourceEncoding;
        static Encoding TargetEncoding = Encoding.UTF8;
        static string Source = @"D:\Projekte\TrashToUTF8\TrashToUTF8\Dump_09072019_192105.sql";
        static string Target = @"D:\Projekte\TrashToUTF8\TrashToUTF8\ReneDump_clear.sql";

        static void Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                SourceEncoding = Encoding.GetEncoding("windows-1252");

                Console.OutputEncoding = Encoding.UTF8;

                Parser p = new Parser(SourceEncoding, TargetEncoding, Source, Target);

                p.Start();



                Console.WriteLine("Ready");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
