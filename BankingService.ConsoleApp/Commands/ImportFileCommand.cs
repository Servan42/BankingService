using BankingService.Core.API.Interfaces;

namespace BankingService.ConsoleApp.Commands
{
    internal class ImportFileCommand : Command
    {
        private readonly IImportService importService;

        public ImportFileCommand(IImportService importService)
        {
            this.importService = importService;
        }

        public override string Name => "import";

        public override string ShortManual => "Imports a Bank CSV file ('-b <filePath>') or a Payapl CSV file ('-p <filePath>') to the database.";

        public override void Execute(string[] args)
        {
            var hint = "Use option '-b <filePath>' to import a Bank file, or '-p <filePath>' to import a paypal file";
            if (args.Length != 2)
            {
                EnhancedConsole.WriteWithForeGroundColor($"Input error (2 args expected): {hint}", ConsoleColor.Red, true);
                return;
            }

            if (UnkownOption(args[0]))
            {
                EnhancedConsole.WriteWithForeGroundColor($"Input error (option {args[0]} unknown): {hint}", ConsoleColor.Red, true);
                return;
            }

            if (!Path.Exists(args[1]))
            {
                EnhancedConsole.WriteWithForeGroundColor($"Input error (File specified does not exist): {hint}", ConsoleColor.Red, true);
                return;
            }

            if (args[0] == "-p")
            {
                this.importService.ImportPaypalFile(args[1]);
            }
            else
            {
                this.importService.ImportBankFile(args[1]);
            }

            Console.WriteLine("File imported.");
        }

        private bool UnkownOption(string option)
        {
            string[] validArgs = ["-p", "-b"];
            return !validArgs.Contains(option);
        }
    }
}
