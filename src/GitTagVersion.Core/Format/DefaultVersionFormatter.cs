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
		public VersionInfo FormatVersion(ResolvedVersionInfo versionInfo)
		{
			if (versionInfo == null)
				throw new ArgumentNullException("versionInfo");

			string preRelease = null;
			var metadata = new List<string>();

			// add version revision
			if (versionInfo.SemVersionRevision > 0)
			{
				var revision = (versionInfo.SemVersionRevision + 1);

				if (versionInfo.SemVersion.IsPreRelease)
				{
					// apeend revision to prerelease
					preRelease = versionInfo.SemVersion.PreRelease.ToString() + "-rev." + (versionInfo.SemVersionRevision + 1);
				}
				else
				{
					metadata.Add(revision.ToString());
				}
			}

			// add metadata
			if (!String.IsNullOrEmpty(versionInfo.AdditionalMetadata))
				metadata.Add(versionInfo.AdditionalMetadata);

			// add commit info to metadata
			metadata.Add(versionInfo.Commit.Committer.When.ToString("yyyyMMdd"));
			metadata.Add(versionInfo.Commit.Sha.Substring(0, 7));

			var fullSemVersion = new SemVersion(
				versionInfo.SemVersion.Major,
				versionInfo.SemVersion.Minor,
				versionInfo.SemVersion.Patch,
				preRelease != null ? new PreReleasePart(preRelease) : null,
				new BuildMetadataPart(metadata));

			var assemblyVersion = new Version(versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor);

			var fileVersion = new Version(
				versionInfo.SemVersion.Major, 
				versionInfo.SemVersion.Minor,
				versionInfo.SemVersionRevision,
				versionInfo.SemVersion.Patch
			);

			return new VersionInfo()
			{
				SemVersion = fullSemVersion,
				AssemblyVersion = assemblyVersion,
				FileVersion = fileVersion
			};
		}
	}
}
