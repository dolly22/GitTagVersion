using GitTagVersion.Core;
using GitTagVersion.Core.Format;
using GitTagVersion.Core.Git;
using GitTagVersion.Core.Resolver;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Console
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				var version = GetVersion(args);

				System.Console.WriteLine("version={0}", version);
				return 0;
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine("Error: {0}", ex.Message);
				return -1;
			}
		}

		public static string GetVersion(string[] args = null)
		{
			var discoverPath = ".";
			if (args.Length > 0)
				discoverPath = args[0];

			var repoPath = Repository.Discover(discoverPath);
			System.Console.WriteLine("Using repository: {0}", repoPath);

            var invoker = new GitTagVersionInvoker();
            var versions = invoker.GetVersion(repoPath, new Progress<string>(System.Console.WriteLine));

            return versions[invoker.FormatKey(SemVer2Formatter.FormatPrefix, SemVer2Formatter.FullVersion)];
		}
	}
}
