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
        //Task FindHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, string[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class;
        //Task FindHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, Expression<Func<T, object>>[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class;
        Task FindHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, string[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class;
        Task Replace<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, char[] searchChars, char[] blackChars, Expression<Func<T, bool>> searchParameter = null, bool save = false) where T : class;
    }

    public interface IDbInfoService : IRunnerBase
    {
        Task DbInfo();
        Task SearchWordsWithotConnection();
    }
}
