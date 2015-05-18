using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;
using NSemVersion;

namespace GitTagVersion.Core.Format
{
    public class SemVer1Formatter : IVersionFormatter
    {
        public const string FormatPrefix = "SemVer1";

        public const string VersionWithPreRelease = "VersionWithPreRelease";

        public string Prefix
        {
            get { return FormatPrefix; }
        }

        public IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo)
        {
            string preRelease = null;

            if (versionInfo.SemVersion.IsPreRelease)
            {
                // append revision to prerelease
                preRelease = String.Format("{0}-{1:D4}", versionInfo.SemVersion.PreRelease, versionInfo.SemVersionRevision);
            }

            var fullSemVersion = new SemVersion(
                            versionInfo.SemVersion.Major,
                            versionInfo.SemVersion.Minor,
                            versionInfo.SemVersion.Patch,
                            preRelease != null ? new PreReleasePart(preRelease) : null);

            yield return new KeyValuePair<string, string>(VersionWithPreRelease, fullSemVersion.ToString());
        }
    }
}
