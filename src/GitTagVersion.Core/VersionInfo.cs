using LibGit2Sharp;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core
{
	public class VersionInfo
	{
		/// <summary>
		/// Commit
		/// </summary>
		public Commit Commit { get; set; }

		/// <summary>
		/// Semantic version with prerelease part (as parsed from git tag)
		/// </summary>
		public SemVersion SemVersion { get; set; }

		/// <summary>
		/// Version revision. Differentiate between different builds of same semversion
		/// </summary>
		public int SemVersionRevision { get; set; }

		/// <summary>
		/// Additional version metadata
		/// </summary>
		public string AdditionalMetadata { get; set; }
	}
}
