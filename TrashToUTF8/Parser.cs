using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TrashToUTF8
{
    public class Parser
    {
        Encoding SourceEncoding;
        Encoding TargetEncoding;
        string SourcePath;
        string TargetPath;

        StreamWriter targetWriter;

        int AllRowsCount;

        Logger logger;

        List<string> searchChars = new List<string> {
            "Å",
            "Ã",
            "‡",
            "™",
            "…",
            "Å",
        };

        List<string> blackChars = new List<string> {
            "ￅ",
            "�",
            "¬",
            "¾",
        };

        public Parser(Encoding sourceEncoding, Encoding targetEncoding, string sourcePath, string targetPath)
        {
            logger = new Logger(@"D:\Projekte\TrashToUTF8\TrashToUTF8\Log.txt");

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

            targetWriter = File.AppendText(TargetPath);
        }

        public void Start()
        { 
            var allRowArray = File.ReadAllLines(SourcePath);    //10221287
            //var allRowArray = new List<string> { "(1099649,'Ã°Â¿Ã°Â¾Ã°Â·Ã°Â°Ã°Â²Ã°Â¸Ã°Â´Ã°Â¾Ã°Â²Ã°Â°Ã±Â‚Ã±ÂŒ',1,4,1,1,1,'2014-04-30 11:14:45',0)," }.ToArray();
            //var allRowArray = new List<string> { "(1034097,'przyjï¿„ï¾™te',1,2,1,1,1,'2013-08-27 10:03:58',0)," }.ToArray();

            AllRowsCount = allRowArray.Count();

            Parse(allRowArray);
        }


        void Parse(string[] allRowArray)
        {
            var rowCounter = 0;
            var dirtyRowCounter = 0;
            for (int i = 0; i < allRowArray.Length; i++)
            {
                rowCounter++;

                var row = allRowArray[i];

                var exit = false;

                List<string> results = new List<string>();
                results.Add(allRowArray[i]);

                while (CheckSearchChars(row) && !exit)
                {
                    var newRow = Convert(row, SourceEncoding, TargetEncoding);

                    if (CheckBlckChars(newRow))
                    {
                        exit = true;
                        results = new List<string> { allRowArray[i] };
                    }
                    else
                    {
                        row = newRow;
                        results.Add(newRow);
                    }
                }

                if(results.Count() > 1)
                {
                    var counter = 0;
                    dirtyRowCounter++;

                    for (int u = 0; u < results.Count; u++)
                    {
                        if (u == results.Count-1)
                        {
                            logger.Log("==\t" + results[u]);
                            logger.Log("-------------");
                        }
                        else
                        {
                            logger.Log(counter + "\t" + results[u]);
                        }

                        counter++;

                    }
                }
                else
                {
                    var tmp = 0;
                }


                Write(results.Last());


                if (rowCounter >= 100000)
                {
                    rowCounter = 0;
                    logger.Print("rdy: " + i.ToString() + " / " + AllRowsCount);
                }
            }

            logger.LogPrint(Environment.NewLine + "Betroffene Zeilen: " + dirtyRowCounter + " / " + AllRowsCount);

            targetWriter.Close();
            logger.Stop();
        }

        void Write(string row)
        {
            targetWriter.WriteLine(row);
        }

        string Convert(string sourceText, Encoding s, Encoding t)
        {
            byte[] asciiBytes = s.GetBytes(sourceText);
            char[] asciiChars = t.GetChars(asciiBytes);
            return new string(asciiChars);
        }

        bool CheckSearchChars(string row)
        {
            foreach (var item in searchChars)
            {
                if (row.Contains(item))
                {
                    return true;
                }
            }

            return false;
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
