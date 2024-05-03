namespace BankingService.ConsoleApp.Model
{
    internal class TableLine
    {
        public bool IsHeader { get; }
        public bool IsSeparator { get; }
        public bool IsClassicLine => !IsHeader && !IsSeparator;

        public List<string> Data { get; }

        public TableLine()
        {
            IsSeparator = true;
        }

        public TableLine(string headerText)
        {
            IsHeader = true;
            Data = [headerText];
        }

        public TableLine(List<string> columnsData)
        {
            Data = new List<string>(columnsData);
        }
    }
}