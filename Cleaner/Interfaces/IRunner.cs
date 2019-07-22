using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cleaner.Interfaces
{
    public interface IRunnerBase
    {
        void Stop();
    }

    public interface IRunner : IRunnerBase
    {
        void Execute();
    }

    public interface IDbReplacerService : IRunnerBase
    {
        Task Replace<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, char[] searchChars, char[] blackChars, bool save = false) where T : class;
    }

    public interface IDbInfoService : IRunnerBase
    {
        Task DbInfo();
        Task SearchWordsWithotConnection();
    }
}
