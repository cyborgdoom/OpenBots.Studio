using System.IO;

namespace OpenBots.Core.Commands
{
    public class InitCommandOptions
    {
        public InitCommandOptions(string fileName, string workingDirectory)
        {
            FileName = fileName;
            WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
        }

        public string FileName { get; }
        public string WorkingDirectory { get; }
    }
}