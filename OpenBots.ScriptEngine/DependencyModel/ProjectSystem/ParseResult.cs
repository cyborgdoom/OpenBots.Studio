using System.Collections.Generic;

namespace OpenBots.DependencyModel.ProjectSystem
{
    public class ParseResult
    {
        public ParseResult(IReadOnlyCollection<PackageReference> packageReferences)
        {
            PackageReferences = packageReferences;
        }

        public IReadOnlyCollection<PackageReference> PackageReferences { get; }
    }
}