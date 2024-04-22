using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.ConsoleStuff
{
    internal static class EnhancedConsole
    {
        internal static void DisplayStringsOnXColumns(int columnNumbers, int leftPadding, List<string> strings)
        {
            var queue = new Queue<string>(strings);
            var initialLine = Console.GetCursorPosition().Top;
            var linesPerColumn = (int)Math.Ceiling(strings.Count / (decimal)columnNumbers);

            var currentLine = initialLine;
            var colIndent = leftPadding;
            var biggestStringLength = 0;

            while (queue.Count > 0)
            {
                if (currentLine - initialLine >= linesPerColumn)
                {
                    currentLine = initialLine;
                    colIndent += biggestStringLength + 4;
                    biggestStringLength = 0;
                }
                var data = queue.Dequeue();
                biggestStringLength = Math.Max(biggestStringLength, data.Length);
                Console.SetCursorPosition(colIndent, currentLine);
                Console.Write(data);
                currentLine++;
            }

            Console.SetCursorPosition(0, initialLine + linesPerColumn + 1);
        }

        internal static void WriteWithForeGroundColor(string data, ConsoleColor color, bool newLine)
        {
            var backup = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(data);
            Console.ForegroundColor = backup;
            if (newLine) Console.WriteLine();
        }
    }
}
