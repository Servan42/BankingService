namespace BankingService.ConsoleApp
{
    internal class CommandInvoker
    {
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();
        
        public void Register(Command command)
        {
            commands.Add(command.Name, command);
        }

        public void Execute(string commandName, string[] args)
        {
            if(!commands.ContainsKey(commandName))
            {
                EnhancedConsole.WriteWithForeGroundColor(
                    $"'{commandName}': command not found. (type 'help' for more info)",
                    ConsoleColor.Red,
                    true);
                return;
            }

            commands[commandName].Execute(args);
        }

        public Dictionary<string, string> GetCommandsManual()
        {
            var result = new Dictionary<string, string>();
            foreach(var kvp in commands)
            {
                result.Add(kvp.Key, kvp.Value.ShortManual);
            }
            return result;
        }
    }
}
