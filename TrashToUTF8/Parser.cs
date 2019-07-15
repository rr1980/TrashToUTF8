using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrashToUTF8
{
    public class Parser
    {
        int DirtyRowCounter = 0;

        Encoding SourceEncoding;
        Encoding TargetEncoding;
        string SourcePath;
        string TargetPath;

        //StreamWriter targetWriter;

        Logger logger;

        List<string> searchChars = new List<string> {
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

        List<string> blackChars = new List<string> {
            "ￅ",
            "�",
            "¬",
            //"¾",
        };

        public Parser(Encoding sourceEncoding, Encoding targetEncoding, string sourcePath, string targetPath)
        {

            var dir = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(dir);

            logger = new Logger(@"D:\Projekte\TrashToUTF8\TrashToUTF8\SB\Results\Log.txt");

            SourceEncoding = sourceEncoding;
            TargetEncoding = targetEncoding;

            SourcePath = sourcePath;
            TargetPath = targetPath;

            if (!File.Exists(SourcePath))
            {
                throw new FileNotFoundException("Datei nicht gefunden!", SourcePath);
            }

            if (File.Exists(TargetPath))
            {
                File.Delete(TargetPath);
                logger.Print("Delete: " + TargetPath);
            }
        }

        public void Start()
        { 
            Parse(SourcePath);

            logger.Stop();
        }
        void Parse(string SourcePath)
        {
            var all = File.ReadAllText(SourcePath, Encoding.UTF8);
            //var all = "ÃÂºÃÂ¾ÃÂ½Ã‘Å’ÃÂºÃÂ¾ÃÂ±ÃÂµÃÂ¶ÃÂ¸Ã‘â€ ÃÂ°";

            //Regex regex = new Regex("'(.*?)'", RegexOptions.Multiline);
            Regex regex = new Regex("'([^0-9]+)'", RegexOptions.Multiline);
            var v = regex.Replace(all, replace);

            var allWords = regex.Matches(all).Count;

            File.WriteAllText(TargetPath, v, TargetEncoding);

            logger.LogPrint(Environment.NewLine + "Betroffene Zeilen: " + DirtyRowCounter + " / " + allWords);
        }


        private string replace(Match match)
        {
            var w = match.Groups[1];
            CheckResult current = CheckSearchChars(w.Value);
            if (current.Found)
            {
                DirtyRowCounter++;
                return Clear(w.Value, current.FoundChar);
            }

            return "'" + w.Value + "'";
        }

        string Clear(string w, string foundChar)
        {
            var oldW = w;

            while (CheckSearchChars(w).Found)
            {
                var newW = Convert(w, SourceEncoding, TargetEncoding);

                if (CheckBlckChars(newW))
                {
                    return "'" + oldW + "'";
                }
                else
                {
                    w = newW;
                }
            }


            logger.Log(foundChar + "\t" + oldW + Environment.NewLine + "=\t" + w + Environment.NewLine);

            return "'" + w + "'";
        }

        string Convert(string sourceText, Encoding s, Encoding t)
        {
            byte[] asciiBytes = s.GetBytes(sourceText);
            char[] asciiChars = t.GetChars(asciiBytes);

            var result = new string(asciiChars);

            //Console.WriteLine(sourceText + " -> " + result);

            return result;
        }

        class CheckResult
        {
            public bool Found { get; set; }
            public string FoundChar { get; set; }
        }

        CheckResult CheckSearchChars(string row)
        {
            foreach (var item in searchChars)
            {
                if (row.Contains(item))
                {
                    return new CheckResult {
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

        private bool CheckBlckChars(string row)
        {
            foreach (var item in blackChars)
            {
                if (row.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}



//void Parse(string[] allRowArray)
//{
//    var rowCounter = 0;
//    var dirtyRowCounter = 0;
//    for (int i = 0; i < allRowArray.Length; i++)
//    {
//        rowCounter++;

//        var row = allRowArray[i];

//        var exit = false;

//        List<string> results = new List<string>();
//        results.Add(allRowArray[i]);

//        while (CheckSearchChars(row) && !exit)
//        {
//            var newRow = Convert(row, SourceEncoding, TargetEncoding);

//            if (CheckBlckChars(newRow))
//            {
//                exit = true;
//                results = new List<string> { allRowArray[i] };
//            }
//            else
//            {
//                row = newRow;
//                results.Add(newRow);
//            }
//        }

//        if(results.Count() > 1)
//        {
//            var counter = 0;
//            dirtyRowCounter++;

//            for (int u = 0; u < results.Count; u++)
//            {
//                if (u == results.Count-1)
//                {
//                    logger.Log("==\t" + results[u]);
//                    logger.Log("-------------");
//                }
//                else
//                {
//                    logger.Log(counter + "\t" + results[u]);
//                }

//                counter++;

//            }
//        }
//        else
//        {
//            var tmp = 0;
//        }


//        Write(results.Last());


//        if (rowCounter >= 100000)
//        {
//            rowCounter = 0;
//            logger.Print("rdy: " + i.ToString() + " / " + AllRowsCount);
//        }
//    }

//    logger.LogPrint(Environment.NewLine + "Betroffene Zeilen: " + dirtyRowCounter + " / " + AllRowsCount);

//    targetWriter.Close();
//    logger.Stop();
//}