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
        readonly IVersionFormatter[] formatters = new IVersionFormatter[]
        {
            new ResolvedPartsFormatter(),
            new BuildNumberFormatter(),
            new CodeVersionFormatter(),
            new SemVer1Formatter(),
            new SemVer2Formatter()
        };

		public IDictionary<string, string> GetVersion(string discoverPath = ".", IProgress<string> progress = null)
		{
			var repoPath = Repository.Discover(discoverPath);

			using (var repo = new Repository(repoPath))
			{
				var versionResolver = new DefaultResolverStrategy(repo);				
				var resolvedVersion = versionResolver.DetermineVersion(progress: progress);

                var results = new Dictionary<string, string>();
                foreach (var format in formatters)
                {
                    var prefix = format.Prefix;
                    foreach (var result in format.Format(resolvedVersion))
                    {
                        results.Add(FormatKey(prefix, result.Key), result.Value);
                    }   
                }

                return results;
            }
		}

        public string FormatKey(string prefix, string key)
        {
            return String.Format("{0}.{1}", prefix, key); ;
        }
	}  
}
