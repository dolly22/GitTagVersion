using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;
using NSemVersion;

namespace GitTagVersion.Core.Format
{
    public class SemVer2Formatter : IVersionFormatter
    {
        public const string FormatPrefix = "SemVer2";

        public const string FullVersion = "FullVersion";
        public const string VersionOnly = "VersionOnly";
        public const string VersionWithPreRelease = "VersionWithPreRelease";

        public string Prefix
        {
            get { return FormatPrefix; }
        }

        public IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo)
        {
            string preRelease = null;
            var metadata = new List<string>();

            // add version revision
            if (versionInfo.SemVersion.IsPreRelease)
            {
                // append revision to prerelease
                preRelease = versionInfo.SemVersion.PreRelease.ToString() + "." + versionInfo.SemVersionRevision;
            }
            else
            {
                metadata.Add(versionInfo.SemVersionRevision.ToString());
            }

            // add metadata
            if (!String.IsNullOrEmpty(versionInfo.AdditionalMetadata))
                metadata.Add(versionInfo.AdditionalMetadata);

            // add commit info to metadata
            metadata.Add(versionInfo.Commit.Committer.When.ToString("yyyyMMdd"));
            metadata.Add(versionInfo.Commit.Sha.Substring(0, 7));

            var version = new SemVersion(
                versionInfo.SemVersion.Major,
                versionInfo.SemVersion.Minor,
                versionInfo.SemVersion.Patch,
                preRelease != null ? new PreReleasePart(preRelease) : null,
                new BuildMetadataPart(metadata));

            // add formatted versions
            yield return new KeyValuePair<string, string>(FullVersion, version.ToString("F"));
            yield return new KeyValuePair<string, string>(VersionOnly, version.ToString("V"));
            yield return new KeyValuePair<string, string>(VersionWithPreRelease, version.ToString("P"));
        }
    }
}
