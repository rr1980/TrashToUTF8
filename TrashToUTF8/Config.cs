using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrashToUTF8
{
    public static class Config
    {
        public static string SourcePath { get; set; } = @"D:\Projekte\TrashToUTF8\TrashToUTF8\SB\Dirty.sql";
        public static string TargetPath { get; set; } = @"D:\Projekte\TrashToUTF8\TrashToUTF8\SB\Results\Clean.sql";
        public static string LogPath { get; set; } = @"D:\Projekte\TrashToUTF8\TrashToUTF8\SB\Results\Log.txt";
        public static string LogFailPath { get; set; } = @"D:\Projekte\TrashToUTF8\TrashToUTF8\SB\Results\LogFail.txt";

        public static Encoding SourceEncoding { get; set; } = Encoding.GetEncoding("windows-1252");
        public static Encoding TargetEncoding { get; set; } = Encoding.UTF8;

        public static Regex Regex { get; set; } = new Regex("'([^0-9]+)'", RegexOptions.Multiline);

        public static List<string> SearchChars = new List<string> {
            "Å",
            "Ã",
            "©",
            "º",
            "Ã",
            "‡",
            "™",
            "…",
            "Å",
            "¾",
            "†",
            "»",
            "°",
            "Ñ",
        };

        public static List<string> BlackChars = new List<string> {
            "ￅ",
            "�",
            "¬",
            //"¾",
        };
    }
}
