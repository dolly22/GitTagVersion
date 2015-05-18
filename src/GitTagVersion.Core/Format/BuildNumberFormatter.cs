using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;
using NSemVersion;

namespace GitTagVersion.Core.Format
{
    public class BuildNumberFormatter : IVersionFormatter
    {
        public const string FormatPrefix = "Build";

        public const string Number = "Number";

        public string Prefix
        {
            get { return FormatPrefix; }
        }

        public IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo)
        {
            string preRelease = null;

            var semVersion = versionInfo.SemVersion;
            if (semVersion.IsPreRelease)
                preRelease = versionInfo.SemVersion.PreRelease.ToString() + "." + versionInfo.SemVersionRevision;
            else
                preRelease = versionInfo.SemVersionRevision.ToString();

            var version = new SemVersion(
                versionInfo.SemVersion.Major,
                versionInfo.SemVersion.Minor,
                versionInfo.SemVersion.Patch,
                preRelease != null ? new PreReleasePart(preRelease) : null);

            yield return new KeyValuePair<string, string>(Number, version.ToString("P"));
        }
    }
}
