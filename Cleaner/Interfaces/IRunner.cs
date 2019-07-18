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
        Task Test();
        Task Test_BaseWords();
        Task Test_Words();
        Task Replace_K433();
    }

    public interface IDbInfoService : IRunnerBase
    {
        Task DbInfo();
    }
}
