namespace BankingService.ConsoleApp
{
    internal abstract class Command
    {
        public abstract string Name { get; }
        public abstract string ShortManual { get; }

        public abstract void Execute(string[] args);
    }
}
