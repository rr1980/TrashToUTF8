using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrashToUTF8
{
    public partial class Parser
    {
        int DirtyWordsCounter = 0;
        int DirtyFailWordsCounter = 0;
        int AllWordsCounter = 0;

        public Parser()
        {

        }

        public void Start()
        {
            if (Config.CanWrite)
            {
                ClearOldFiles();
            }

            Parse();
            Logger.Stop();
        }

        public string Convert(string sourceText)
        {
            byte[] asciiBytes = Config.SourceEncoding.GetBytes(sourceText);
            char[] asciiChars = Config.TargetEncoding.GetChars(asciiBytes);

            return new string(asciiChars);
        }

        private void ClearOldFiles()
        {
            if (!File.Exists(Config.SourcePath))
            {
                throw new FileNotFoundException("Datei nicht gefunden!", Config.SourcePath);
            }

            if (File.Exists(Config.TargetPath))
            {
                File.Delete(Config.TargetPath);
                Logger.Print("Lösche alte Resultate: " + Config.TargetPath, ConsoleColor.Yellow);
            }

            var dir = Path.GetDirectoryName(Config.TargetPath);
            Directory.CreateDirectory(dir);
        }


        private void Parse()
        {
            string allTextFromSource = ReadSource();

            allTextFromSource = CustomReplace(allTextFromSource);

            Logger.Print("Parsen gestartet...", ConsoleColor.Yellow);

            AllWordsCounter = Config.Regex.Matches(allTextFromSource).Count;

            var allTextForTarget = Config.Regex.Replace(allTextFromSource, ReplaceDelegate);

            Task.Run(() => Logger.PrintCounter(DirtyWordsCounter, AllWordsCounter, true)).Wait();

            Logger.Print("Parsen beendet...", ConsoleColor.Yellow);

            WriteTarget(allTextForTarget);

            Logger.Print("Fertig!", ConsoleColor.Green);
            Logger.LogPrint("Betroffene Wörter: " + DirtyWordsCounter + " / " + AllWordsCounter, ConsoleColor.Green);
            Logger.LogPrint("Ungelöste Wörter: " + DirtyFailWordsCounter, ConsoleColor.Red);
        }

        private string CustomReplace(string allTextFromSource)
        {
            foreach (var item in Config.CustomReplace)
            {
                allTextFromSource = allTextFromSource.Replace(item.Key, item.Value);
            }

            return allTextFromSource;
        }

        private static void WriteTarget(string allTextForTarget)
        {
            if (Config.CanWrite)
            {
                Logger.Print("Schreibe Resultat... " + Config.TargetPath, ConsoleColor.Blue);
                File.WriteAllText(Config.TargetPath, allTextForTarget, Config.TargetEncoding);
            }
        }

        private static string ReadSource()
        {
            Logger.Print("Lese Quelle... " + Config.SourcePath, ConsoleColor.Blue);
            return File.ReadAllText(Config.SourcePath, Encoding.UTF8);
        }

        private string ReplaceDelegate(Match match)
        {
            var matchWord = match.Groups[1];

            CheckResult current = CheckSearchChars(matchWord.Value);

            if (current.Found)
            {
                DirtyWordsCounter++;

                Task.Run(() => Logger.PrintCounter(DirtyWordsCounter, AllWordsCounter));

                return Clear(matchWord.Value, current.FoundChar);
            }

            return "'" + matchWord.Value + "'";
        }

        private string Clear(string dirtyWord, string foundChar)
        {
            var oldWord = dirtyWord;

            while (CheckSearchChars(dirtyWord).Found)
            {
                var newWord = Convert(dirtyWord);

                if (CheckBlackChars(newWord))
                {
                    DirtyFailWordsCounter++;
                    Logger.LogFail(foundChar + "\t" + oldWord + Environment.NewLine + "=\t" + newWord + Environment.NewLine);
                    return "'" + oldWord + "'";
                }
                else
                {
                    dirtyWord = newWord;
                }
            }

            Logger.Log(foundChar + "\t" + oldWord + Environment.NewLine + "=\t" + dirtyWord + Environment.NewLine);

            return "'" + dirtyWord + "'";
        }

        private CheckResult CheckSearchChars(string row)
        {
            foreach (var item in Config.SearchChars)
            {
                if (row.Contains(item))
                {
                    return new CheckResult
                    {
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

        private bool CheckBlackChars(string row)
        {
            foreach (var item in Config.BlackChars)
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