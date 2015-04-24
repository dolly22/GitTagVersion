using GitTagVersion.Core.Format;
using GitTagVersion.Core.Resolver;
using GitTagVersion.Loader;
using LibGit2Sharp;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core
{
	public class GitTagVersionInvoker : MarshalByRefObject, IGitTagVersionInvoker
	{
		public IDictionary<string, string> GetVersion(string discoverPath = ".")
		{
			var repoPath = Repository.Discover(discoverPath);

			using (var repo = new Repository(repoPath))
			{
				var versionResolver = new DefaultResolverStrategy(repo);
				//var progress = new Progress<string>(System.Console.WriteLine);

				var resolvedVersion = versionResolver.DetermineVersion(progress: null);
				var formatter = new DefaultVersionFormatter();

				return formatter.GetFormattedVersion(resolvedVersion);
			}
		}
	}  
}
