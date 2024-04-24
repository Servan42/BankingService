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
            if (args.Length != 2
                || UnkownOption(args[0])
                || !Path.Exists(args[1]))
            {
                EnhancedConsole.WriteWithForeGroundColor("Input error: Use option '-b <filePath>' to import a Bank file, or '-p <filePath>' to import a paypal file",
                    ConsoleColor.Red, true);
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

        //importService.ImportBankFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\***REMOVED***_Jan.csv");
        //importService.ImportPaypalFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\paypa_janvier.CSV");
    }
}
