using System;

namespace OpenBots.Core
{
    public class ScriptRuntimeException : Exception
    {
        public ScriptRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
