using System.IO;
using System.Reflection;

namespace OpenBots.Core.Templates
{
    public static class TemplateLoader
    {
        public static string ReadTemplate(string name)
        {
            var resourceStream = typeof(TemplateLoader).GetTypeInfo().Assembly.GetManifestResourceStream($"OpenBots.ScriptEngine.Core.Templates.{name}");
            using (var streamReader = new StreamReader(resourceStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}