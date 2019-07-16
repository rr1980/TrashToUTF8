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

    public interface IAppTesterService : IRunnerBase
    {
        Task Replace_K433();
        //void Test();
    }
}
