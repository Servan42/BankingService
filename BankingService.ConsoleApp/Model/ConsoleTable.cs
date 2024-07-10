namespace BankingService.ConsoleApp.Model
{
    internal class ConsoleTable
    {
        private List<TableLine> tableLines = new();
        private List<int> columnsMaxLenghs;
        private readonly int nbColumns;
        private readonly bool[] columnsRightPadded;
        private readonly int tableLeftPadding;
        private readonly string columnSeparator;

        private int tableWidth;

        public ConsoleTable(int nbColumns, bool[] columnsRightPadded, int tableLeftPadding = 0, string columnSeparator = "  ")
        {
            this.nbColumns = nbColumns;
            this.columnsRightPadded = columnsRightPadded;
            this.tableLeftPadding = tableLeftPadding;
            this.columnSeparator = columnSeparator;
        }

        internal void AddHeaderLine(string headerText)
        {
            tableLines.Add(new TableLine(headerText));
        }

        internal void AddLine(List<string> columnValues)
        {
            tableLines.Add(new TableLine(columnValues));
        }

        internal void AddSeparatorLine()
        {
            tableLines.Add(new TableLine());
        }

        internal void Display()
        {
            CalculateTableAndColumnWidth();
            foreach (var line in tableLines)
            {
                Console.Write(new string(' ', tableLeftPadding));
                if (line.IsSeparator)
                {
                    DisplaySeparatorLine();
                }
                else if (line.IsHeader)
                {
                    DisplayHeaderLine(line);
                }
                else
                {
                    DisplayClassicLine(line);
                }
            }
        }

        private void DisplaySeparatorLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private void DisplayHeaderLine(TableLine line)
        {
            int nbSpacesToAdd = tableWidth - 4 - line.Data[0].Length;
            Console.WriteLine("| " + new string(' ', nbSpacesToAdd / 2) + line.Data[0] + new string(' ', (int)Math.Ceiling(nbSpacesToAdd / 2.0)) + " |");
        }

        private void DisplayClassicLine(TableLine line)
        {
            var paddedColumnData = new List<string>();
            for (int i = 0; i < nbColumns; i++)
            {
                if (columnsRightPadded[i])
                {
                    paddedColumnData.Add(line.Data[i].PadRight(columnsMaxLenghs[i]));
                }
                else
                {
                    paddedColumnData.Add(line.Data[i].PadLeft(columnsMaxLenghs[i]));
                }
            }
            Console.WriteLine($"| {string.Join(columnSeparator, paddedColumnData)} |");
        }

        private void CalculateTableAndColumnWidth()
        {
            tableWidth = 0;
            tableWidth += 2; // Pipe and one padding space
            var headerLines = tableLines.Where(l => l.IsHeader);
            int maxHeaderWidth = headerLines.Any() ? headerLines.Max(h => h.Data[0].Length) : 0;

            columnsMaxLenghs = new();
            for (int i = 0; i < nbColumns; i++)
            {
                columnsMaxLenghs.Add(tableLines.Where(c => c.IsClassicLine).Max(c => c.Data[i].Length));
            }
            int separatorsWidth = (nbColumns - 1) * columnSeparator.Length;
            int maxLineWidth = columnsMaxLenghs.Sum() + separatorsWidth;

            tableWidth += Math.Max(maxLineWidth, maxHeaderWidth);

            tableWidth += 2; // one padding space and pipe
        }
    }
}
