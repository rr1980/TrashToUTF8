namespace Cleaner.Interfaces
{
    public interface IRunner
    {
        void Execute();
        void Stop();
    }

    public interface IRunnerService : IRunner
    {

    }
}
