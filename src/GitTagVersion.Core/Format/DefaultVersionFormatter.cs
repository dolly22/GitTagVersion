using GitTagVersion.Core.Resolver;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Format
{
	public class DefaultVersionFormatter
	{
		public enum FormatKey
		{
			FullVersion,

			VersionOnly,
			VersionWithPreRelease,

			VersionWithPreReleaseV1,

			AssemblyVersion,
			FileVersion
		}

		public IDictionary<string, string> GetFormattedVersion(ResolvedVersionInfo versionInfo)
		{
			if (versionInfo == null)
				throw new ArgumentNullException("versionInfo");

			var dict = new Dictionary<string, string>();

			// add classic versions
			AddClassicVersion(versionInfo, dict);

			// add semversion1 versions
			AddSemVersionV1(versionInfo, dict);		

			// add semversion2 versions
			AddSemVersionV2(versionInfo, dict);			

			return dict;
		}

		private void AddSemVersionV1(ResolvedVersionInfo versionInfo, IDictionary<string, string> dict)
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

			AddFormattedVersion(dict, FormatKey.VersionWithPreReleaseV1, fullSemVersion.ToString());
		}

		private void AddSemVersionV2(ResolvedVersionInfo versionInfo, IDictionary<string, string> dict)
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
			AddFormattedVersion(dict, FormatKey.FullVersion, version.ToString("F"));
			AddFormattedVersion(dict, FormatKey.VersionOnly, version.ToString("V"));
			AddFormattedVersion(dict, FormatKey.VersionWithPreRelease, version.ToString("P"));
		}

		private void AddClassicVersion(ResolvedVersionInfo versionInfo, IDictionary<string, string> dict)
		{
			var assemblyVersion = new Version(versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, 0, 0);

			var fileVersion = new Version(
				versionInfo.SemVersion.Major,
				versionInfo.SemVersion.Minor,
				versionInfo.SemVersionRevision,
				versionInfo.SemVersion.Patch
			);

			AddFormattedVersion(dict, FormatKey.AssemblyVersion, assemblyVersion.ToString());
			AddFormattedVersion(dict, FormatKey.FileVersion, fileVersion.ToString());
		}

		private void AddFormattedVersion(IDictionary<string, string> dict, FormatKey key, string value)
		{
			dict[key.ToString()] = value;
		}
	}
}
