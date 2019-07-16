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
        void Test();
    }
}
