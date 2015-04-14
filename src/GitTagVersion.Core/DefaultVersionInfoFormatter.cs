using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core
{
	public class DefaultVersionInfoFormatter
	{
		public SemVersion Format(VersionInfo versionInfo)
		{
			if (versionInfo == null)
				throw new ArgumentNullException("versionInfo");

			string preRelease = null;
			var metadata = new List<string>();

			// add metadata
			if (!String.IsNullOrEmpty(versionInfo.AdditionalMetadata))
				metadata.Add(versionInfo.AdditionalMetadata);

			if (versionInfo.SemVersionRevision > 0)
			{
				var revisionInfo = "rev."+ (versionInfo.SemVersionRevision + 1);

				if (versionInfo.SemVersion.IsPreRelease)
				{
					// apeend revision to prerelease
					preRelease = versionInfo.SemVersion.PreRelease.ToString() + "-" + revisionInfo;
				}
				else
				{
					metadata.Add(revisionInfo);
				}
			}

			// add commit info to metadata
			metadata.Add(versionInfo.Commit.Committer.When.ToString("yyyyMMdd"));
			metadata.Add(versionInfo.Commit.Sha.Substring(0, 7));

			return new SemVersion(
				versionInfo.SemVersion.Major, 
				versionInfo.SemVersion.Minor, 
				versionInfo.SemVersion.Patch,
				preRelease != null ? new PreReleasePart(preRelease) : null, 
				new BuildMetadataPart(metadata));
		}
	}
}
