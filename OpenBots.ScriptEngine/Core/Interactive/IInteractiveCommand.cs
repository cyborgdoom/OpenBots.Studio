namespace OpenBots.Core
{
    public interface IInteractiveCommand
    {
        string Name { get; }
        void Execute(CommandContext commandContext);
    }
}
