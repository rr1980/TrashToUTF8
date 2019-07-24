using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using xLingua.Inspector.Core;

namespace xLingua.Inspector.Resolver
{
    #region helper
    enum ClearResultType
    {
        Fixed,
        Impossible,
        Unnecessary
    }

    class ClearResult
    {
        public ClearResultType Type { get; set; }
        public string Text { get; set; }
    }

    class CheckResult
    {
        public bool Found { get; set; }
        public char FoundChar { get; set; }
    }
    #endregion

    public class Utf8Resolver<T> : BaseResolver<T> where T : class
    {

        private char[] _searchChars;
        private char[] _blackChars;

        private readonly ConsoleColor defaultConsoleColor;

        public Utf8Resolver(Expression<Func<T, string>> columnProp, char[] searchChars, char[] blackChars, string logPath): base(logPath, columnProp)
        {
            defaultConsoleColor = Console.ForegroundColor;
            _searchChars = searchChars;
            _blackChars = blackChars;

        }

        public override string Resolve(long id, string dirtyValue)
        {
            var result = Clear(dirtyValue, _searchChars, _blackChars);

            if(result.Type == ClearResultType.Unnecessary)
            {
                return dirtyValue;
            }
            else if (result.Type == ClearResultType.Fixed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                LogGood(string.Format("Fixed     ID: {0}", id));
                Console.ForegroundColor = defaultConsoleColor;

                LogGood(string.Format("Old           {0}", dirtyValue));
                LogGood(string.Format("New           {0}", result.Text));
                LogGood();

                return result.Text;
            }
            else if (result.Type == ClearResultType.Impossible)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                LogBad(string.Format("NOT FIXED ID: {0}", id));
                Console.ForegroundColor = defaultConsoleColor;

                LogBad(string.Format("Old           {0}", dirtyValue));
                LogBad(string.Format("Wrong         {0}", result.Text));
                LogBad();


                return result.Text;
            }

            return dirtyValue;
        }

        private ClearResult Clear(string dirtyWord, char[] searchChars, char[] blackChars)
        {
            var oldWord = dirtyWord.Trim();

            while (CheckSearchChars(searchChars, dirtyWord.Trim()).Found)
            {
                var newWord = Convert(dirtyWord.Trim()).Trim();

                var cbc = CheckBlackChars(blackChars, newWord.Trim());
                if (cbc.Found)
                {
                    return new ClearResult
                    {
                        Text = newWord,
                        Type = ClearResultType.Impossible
                    };
                }
                else
                {
                    dirtyWord = newWord;
                }
            }

            if (oldWord.Trim() != dirtyWord.Trim())
            {
                return new ClearResult
                {
                    Text = dirtyWord,
                    Type = ClearResultType.Fixed
                };
            }

            return new ClearResult
            {
                Text = dirtyWord,
                Type = ClearResultType.Unnecessary
            };

        }

        private CheckResult CheckSearchChars(char[] searchChars, string row)
        {
            foreach (var item in searchChars)
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

        private CheckResult CheckBlackChars(char[] blackChars, string row)
        {
            foreach (var item in blackChars)
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

        private string Convert(string sourceText)
        {
            byte[] asciiBytes = Encoding.GetEncoding("windows-1252").GetBytes(sourceText);
            char[] asciiChars = Encoding.UTF8.GetChars(asciiBytes);

            return new string(asciiChars);
        }

        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}