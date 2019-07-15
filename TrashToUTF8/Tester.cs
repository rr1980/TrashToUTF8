using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TrashToUTF8
{
    public static class Tester
    {
        public static string Convert(string sourceText)
        {
            byte[] asciiBytes = Config.SourceEncoding.GetBytes(sourceText);
            char[] asciiChars = Config.TargetEncoding.GetChars(asciiBytes);

            return new string(asciiChars);
        }

        public static MatchCollection Find(string regex, string text)
        {
            return new Regex(regex).Matches(text);
        }

        public static string Load(string sourcePath)
        {
            return File.ReadAllText(sourcePath, Encoding.UTF8);
        }
    }
}
