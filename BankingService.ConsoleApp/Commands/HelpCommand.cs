namespace BankingService.ConsoleApp.Commands
{
    internal class HelpCommand : Command
    {
        private readonly CommandInvoker invoker;

        public HelpCommand(CommandInvoker invoker)
        {
            this.invoker = invoker;
        }

        public override string Name => "help";

        public override string ShortManual => "Display help.";

        public override void Execute(string[] args)
        {
            Console.WriteLine("This is the BankingService CLI, used to call useful banking functions.");
            var manuals = invoker.GetCommandsManual();
            int maxCommandLength = manuals.Keys.Max(k => k.Length);
            foreach (var kvp in invoker.GetCommandsManual().OrderBy(m => m.Key))
            {
                Console.Write(" ");
                EnhancedConsole.WriteWithForeGroundColor(kvp.Key.PadRight(maxCommandLength), ConsoleColor.Cyan);
                Console.WriteLine($"  {kvp.Value}");
            }
        }
    }
}
