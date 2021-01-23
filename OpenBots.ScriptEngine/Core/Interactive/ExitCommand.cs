using System;

namespace OpenBots.Core
{
    public class ExitCommand : IInteractiveCommand
    {
        public string Name => "exit";

        public void Execute(CommandContext commandContext)
        {
            commandContext.Runner.Exit();
        }
    }
}
