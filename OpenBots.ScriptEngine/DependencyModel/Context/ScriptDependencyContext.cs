namespace OpenBots.DependencyModel.Context
{
    public class ScriptDependencyContext
    {
        public ScriptDependencyContext(ScriptDependency[] dependencies)
        {
            Dependencies = dependencies;
        }

        public ScriptDependency[] Dependencies { get; }
    }
}