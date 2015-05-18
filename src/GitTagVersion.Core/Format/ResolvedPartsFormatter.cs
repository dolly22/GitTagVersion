using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;

namespace GitTagVersion.Core.Format
{
    public class ResolvedPartsFormatter : IVersionFormatter
    {
        public const string FormatPrefix = "Resolved";

        public const string Major = "Major";
        public const string Minor = "Minor";
        public const string Patch = "Patch";
        public const string Revision = "Revision";
        public const string VersionOnly = "VersionOnly";
        public const string VersionWithPreRelease = "VersionWithPreRelease";

        public string Prefix
        {
            get { return FormatPrefix; }
        }

        public IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo)
        {
            return new Dictionary<string, string>()
            {
                { Major, versionInfo.SemVersion.Major.ToString() },
                { Minor, versionInfo.SemVersion.Minor.ToString() },
                { Patch, versionInfo.SemVersion.Patch.ToString() },
                { Revision, versionInfo.SemVersionRevision.ToString() },
                { VersionOnly, versionInfo.SemVersion.ToString("V") },
                { VersionWithPreRelease, versionInfo.SemVersion.ToString("P") }
            };
        }
    }
}
