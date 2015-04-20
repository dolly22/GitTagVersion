using GitTagVersion.Core.Format;
using GitTagVersion.Core.Resolver;
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
				var versionResolver = new DefaultResolverStrategy(repo);
				//var progress = new Progress<string>(System.Console.WriteLine);

				var resolvedVersion = versionResolver.DetermineVersion(progress: null);
				var versionInfo = new DefaultVersionFormatter().FormatVersion(resolvedVersion);

				//TODO: allow semversion formatting
				var shortVersion = String.Format("{0}.{1}.{2}", versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, versionInfo.SemVersion.Patch);

				var semVersionPreRelease = String.Format("{0}.{1}.{2}{3}",
					versionInfo.SemVersion.Major, versionInfo.SemVersion.Minor, versionInfo.SemVersion.Patch, versionInfo.SemVersion.PreRelease.FormatPart());

				var result = new Dictionary<string, string>() {
					{ "ShortVersion", shortVersion },
					{ "ShortSemVersion", semVersionPreRelease },
					{ "FullSemVersion", versionInfo.SemVersion.ToString() }
				};

				return result;
			}
		}
	}  
}
