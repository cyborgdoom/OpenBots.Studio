using OpenBots.Core.Commands;
using OpenBots.DependencyModel.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Text;

namespace OpenBots.Core.ScriptEngine
{
    public static class ScriptEngineCompiler
    {
        public static Func<string, bool, LogFactory> CreateLogFactory
         = (verbosity, debugMode) => LogHelper.CreateLogFactory(debugMode ? "debug" : verbosity);

        public static object Compile(string code)
        {
            var logFactory = CreateLogFactory(null, false);
            var strErrors = new StringBuilder();
            var options = new ExecuteCodeCommandOptions(code, null, null
                ,
                OptimizationLevel.Debug,
                false, null);
            try
            {
               return new ExecuteCodeCommand(ScriptConsole.Default, 
                    logFactory).Execute<int>(options).Result;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggregateEx)
                {
                    ex = aggregateEx.Flatten().InnerException;
                }
                if (ex is CompilationErrorException || ex is ScriptRuntimeException)
                {
                    var errors = ex as CompilationErrorException;
                    foreach (var error in errors.Diagnostics)
                    {
                        strErrors.AppendLine(error.ToString());
                    }
                }
            }
            return strErrors;
        }

    }
}
