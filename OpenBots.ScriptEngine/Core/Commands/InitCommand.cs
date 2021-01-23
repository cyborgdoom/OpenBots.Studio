using OpenBots.DependencyModel.Logging;

namespace OpenBots.Core.Commands
{
    public class InitCommand
    {
        private readonly LogFactory _logFactory;

        public InitCommand(LogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public void Execute(InitCommandOptions options)
        {
            var scaffolder = new Scaffolder(_logFactory);
            scaffolder.InitializerFolder(options.FileName, options.WorkingDirectory);
        }
    }
}