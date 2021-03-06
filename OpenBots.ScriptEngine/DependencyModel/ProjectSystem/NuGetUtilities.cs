using NuGet.Configuration;
using System.Linq;

namespace OpenBots.DependencyModel.ProjectSystem
{
    internal static class NuGetUtilities
    {
        public static string GetNearestConfigPath(string pathToEvaluate)
        {
            var settings = Settings.LoadDefaultSettings(pathToEvaluate);
            return settings.GetConfigFilePaths().FirstOrDefault();
        }
    }
}
