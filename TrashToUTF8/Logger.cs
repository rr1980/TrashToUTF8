using System;
using System.Diagnostics;
using System.IO;

namespace TrashToUTF8
{
    public class Logger
    {
        StreamWriter logWriter;

        public Logger(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            logWriter = File.AppendText(path);
        }

        public void LogPrint(string msg)
        {
            Print(msg);
            Log(msg);
        }

        public void Print(string msg)
        {
            Console.WriteLine(msg);
            //Debug.WriteLine(msg);
        }

        public void Log(string msg)
        {
            //Console.WriteLine(msg);
            logWriter.WriteLine(msg);
        }

        internal void Stop()
        {
            logWriter.Close();
        }
    }
}
