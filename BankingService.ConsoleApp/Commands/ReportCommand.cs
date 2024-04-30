using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class ReportCommand : Command
    {
        private readonly IReportService reportService;

        public ReportCommand(IReportService reportService)
        {
            this.reportService = reportService;
        }

        public override string Name => "report";

        public override string ShortManual => "Displays an operations report. Takes a number between 1 and 12 representing the month.";

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
            DisplayReport(reportService.GetOperationsReport(startDate, endDate), startDate, endDate);
        }
        private (DateTime startDate, DateTime endDate) GetStartAndEndDateFromMonthNumber(int monthNumber)
        {
            var currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, monthNumber, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }
        
        private void DisplayReport(OperationsReportDto operationsReportDto, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"Report for {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            Console.WriteLine("  Sum per Category:");
            var catPadding = operationsReportDto.SumPerCategory.Keys.Max(c => c.Length);
            var valuePadding = operationsReportDto.SumPerCategory.Values.Max(v => v.ToString().Length);
            foreach (var spc in operationsReportDto.SumPerCategory)
            {
                Console.WriteLine($"    {spc.Key.PadRight(catPadding)}  {spc.Value.ToString().PadLeft(valuePadding)} euro");
            }
        }
    }
}
