using System.Collections.Generic;
using OpenBots.DependencyModel.ProjectSystem;

namespace OpenBots.DependencyModel.Compilation
{
    public interface ICompilationReferenceReader
    {
        IEnumerable<CompilationReference> Read(ProjectFileInfo projectFile);
    }
}