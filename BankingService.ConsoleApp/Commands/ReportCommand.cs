using BankingService.ConsoleApp.Model;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;

namespace BankingService.ConsoleApp.Commands
{
    internal class ReportCommand : Command
    {
        private readonly IReportService reportService;
        private TransactionsReportDto report;

        public ReportCommand(IReportService reportService)
        {
            this.reportService = reportService;
        }

        public override string Name => "report";

        public override string ShortManual => "Displays an transactions report. Takes a number between 1 and 12 representing the month.";

        public override void Execute(string[] args)
        {
            if(args.Length != 1)
            {
                EnhancedConsole.WriteWithForeGroundColor("This command takes one argument, a number between 1 and 12, representing the month.", ConsoleColor.Red, true);
                return;
            }

            if (!int.TryParse(args[0], out int monthNumber))
            {
                EnhancedConsole.WriteWithForeGroundColor($"The argument '{args[0]}' could not be recognized as a number", ConsoleColor.Red, true);
                return;
            }

            if (monthNumber > 12 || monthNumber < 1)
            {
                EnhancedConsole.WriteWithForeGroundColor($"The month number '{args[0]}' is not between 1 and 12.", ConsoleColor.Red, true);
                return;
            }

            (var startDate, var endDate) = GetStartAndEndDateFromMonthNumber(monthNumber);
            report = reportService.GetTransactionsReport(startDate, endDate);
            DisplayReport();
        }
        private (DateTime startDate, DateTime endDate) GetStartAndEndDateFromMonthNumber(int monthNumber)
        {
            var currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, monthNumber, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }
        
        private void DisplayReport()
        {
            Console.WriteLine($"Report for {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
            Console.WriteLine();
            DisplaySumPerCategoryTable();
            Console.WriteLine();
            DisplayBalanceDataTable();
            Console.WriteLine();
            DisplayHighestTransactions();
        }

        private void DisplayHighestTransactions()
        {
            Console.WriteLine("  Highest transactions:");
            var table = new ConsoleTable(6, [true, true, true, true, true, true], 4, " | ");
            table.AddSeparatorLine();
            table.AddLine(["Date", "Flow", "Type", "Category", "AutoComment", "Comment"]);
            table.AddSeparatorLine();
            foreach (var line in report.HighestTransactions)
            {
                table.AddLine(
                [
                    line.Date.ToString("yyyy-MM-dd"),
                    line.Flow.ToString(),
                    line.Type,
                    line.Category,
                    line.AutoComment,
                    line.Comment
                ]);
            }
            table.AddSeparatorLine();
            table.Display();
        }

        private void DisplayBalanceDataTable()
        {
            Console.WriteLine("  Balance Data:");

            var table = new ConsoleTable(2, [true, false], 4);
            table.AddSeparatorLine();
            table.AddHeaderLine("Without savings");
            table.AddSeparatorLine();
            table.AddLine(["Positive Sum", report.PositiveSumWithoutSavings.ToString()]);
            table.AddLine(["Negative Sum", report.NegativeSumWithoutSavings.ToString()]);
            table.AddLine(["Balance", report.BalanceWithoutSavings.ToString()]);
            table.AddSeparatorLine();
            table.AddHeaderLine("With savings");
            table.AddSeparatorLine();
            table.AddLine(["Positive Sum", report.PositiveSum.ToString()]);
            table.AddLine(["Negative Sum", report.NegativeSum.ToString()]);
            table.AddLine(["Balance", report.Balance.ToString()]);
            table.AddSeparatorLine();
            table.Display();
        }

        private void DisplaySumPerCategoryTable()
        {
            Console.WriteLine("  Sum per Category:");

            var incomeLines = report.SumPerCategory.Where(x => x.Key == "Income" || x.Key == "Epargne");
            var expensesLines = report.SumPerCategory.Where(x => x.Key != "Income" && x.Key != "Epargne");
            var expensesLinesWithoutCosts = expensesLines.Where(x => !x.Key.Contains("Charges"));

            var table = new ConsoleTable(3, [true, false, false], 4);
            table.AddSeparatorLine();
            table.AddHeaderLine("Income and Savings");
            table.AddSeparatorLine();

            foreach (var spc in incomeLines.OrderBy(spc => spc.Value))
            {
                table.AddLine([spc.Key, "", spc.Value.ToString()]);
            }

            table.AddSeparatorLine();
            table.AddHeaderLine("Expenses Categories");
            table.AddSeparatorLine();

            var total = expensesLines.Sum(x => x.Value);
            foreach (var spc in expensesLines.OrderBy(spc => spc.Value))
            {
                table.AddLine([spc.Key, ((spc.Value / total) * 100).ToString(".0") + "%", spc.Value.ToString()]);
            }

            table.AddSeparatorLine();
            table.AddHeaderLine("Expenses Categories without Costs");
            table.AddSeparatorLine();

            total = expensesLinesWithoutCosts.Sum(x => x.Value);
            foreach (var spc in expensesLinesWithoutCosts.OrderBy(spc => spc.Value))
            {
                table.AddLine([spc.Key, ((spc.Value / total) * 100).ToString(".0") + "%", spc.Value.ToString()]);
            }

            table.AddSeparatorLine();
            table.Display();
        }
    }
}
