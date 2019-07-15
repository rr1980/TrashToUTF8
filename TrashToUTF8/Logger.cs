using System;
using System.Diagnostics;
using System.IO;

namespace TrashToUTF8
{
    public static class Logger
    {
        private static StreamWriter logWriter;
        private static StreamWriter logFailWriter;


        private static int Pos_X { get; set; }
        private static int Pos_Y { get; set; }
        private static int LastCount { get; set; }
        private static int CounterSteps { get; set; } = 1000;
        private static bool FirstPosChange { get; set; }
        private static readonly object LockObj = new object();

        static Logger()
        {
            if (File.Exists(Config.LogPath))
            {
                File.Delete(Config.LogPath);
            }

            if (File.Exists(Config.LogFailPath))
            {
                File.Delete(Config.LogFailPath);
            }

            logWriter = File.AppendText(Config.LogPath);
            logFailWriter = File.AppendText(Config.LogFailPath);

            Console.WriteLine(Environment.NewLine);
        }

        public static void LogPrint(string msg, ConsoleColor? col = null)
        {
            Print(msg, col);
            Log(msg);
        }

        public static void LogError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print(e.Message);
            Console.ResetColor();
            Log(e.Message);
        }

        public static void Print(string msg, ConsoleColor? col = null)
        {
            if(col.HasValue)
            {
                Console.ForegroundColor = col.Value;
            }

            Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + msg + Environment.NewLine);

            Console.ResetColor();
        }

        public static void PrintCounter(int current, int from, bool last = false)
        {
            lock (LockObj)
            {
                if (!FirstPosChange)
                {
                    FirstPosChange = true;
                    Pos_X = Console.CursorLeft;
                    Pos_Y = Console.CursorTop;
                }

                if (current >= LastCount + CounterSteps || last == true)
                {
                    LastCount = current;

                    Console.SetCursorPosition(Pos_X, Pos_Y);

                    Console.WriteLine(current + " von möglichen: " + from);

                    if (last)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void Log(string msg)
        {
            logWriter.WriteLine(msg);
        }

        public static void LogFail(string msg)
        {
            logFailWriter.WriteLine(msg);
        }

        internal static void Stop()
        {
            logWriter.Close();
            logFailWriter.Close();
        }
    }
}
