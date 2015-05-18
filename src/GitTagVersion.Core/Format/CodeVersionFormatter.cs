using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;
using NSemVersion;

namespace GitTagVersion.Core.Format
{
    public class CodeVersionFormatter : IVersionFormatter
    {
        public const string FormatPrefix = "Code";

        public const string AssemblyVersion = "AssemblyVersion";
        public const string FileVersion = "FileVersion";

        public string Prefix
        {
            get { return FormatPrefix; }
        }

        public IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo)
        {
            var assemblyVersion = new Version(versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, 0, 0);

            yield return new KeyValuePair<string, string>(AssemblyVersion, assemblyVersion.ToString());

            var fileVersion = new Version(
                versionInfo.SemVersion.Major,
                versionInfo.SemVersion.Minor,
                versionInfo.SemVersionRevision,
                versionInfo.SemVersion.Patch
            );

            yield return new KeyValuePair<string, string>(FileVersion, fileVersion.ToString());
        }
    }
}
