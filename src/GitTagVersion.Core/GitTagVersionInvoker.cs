using GitTagVersion.Interfaces;
using LibGit2Sharp;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core
{
	public class GitTagVersionInvoker : MarshalByRefObject, IGitTagVersion
	{
		public IDictionary<string, string> GetVersion(string discoverPath = ".")
		{
			var repoPath = Repository.Discover(discoverPath);

			using (var repo = new Repository(repoPath))
			{
				var tagFinder = new DefaultVersionStrategy(repo);
				//var progress = new Progress<string>(System.Console.WriteLine);

				var versionInfo = tagFinder.DetermineVersion(progress: null);
				var formattedVersion = new DefaultVersionInfoFormatter().Format(versionInfo);

				var shortVersion = String.Format("{0}.{1}.{2}", versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, versionInfo.SemVersion.Patch);

				var semVersionPreRelease = String.Format("{0}.{1}.{2}{3}",
					versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, versionInfo.SemVersion.Patch, versionInfo.SemVersion.PreRelease.FormatPart());

				var result = new Dictionary<string, string>() {
					{ "ShortVersion", shortVersion },
					{ "ShortSemVersion", semVersionPreRelease },
					{ "FullSemVersion", formattedVersion.ToString() }
				};

				return result;
			}
		}
	}  
}
