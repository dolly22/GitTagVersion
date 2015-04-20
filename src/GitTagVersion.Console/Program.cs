using GitTagVersion.Core;
using GitTagVersion.Core.Format;
using GitTagVersion.Core.Git;
using GitTagVersion.Core.Resolver;
using GitTagVersion.Interfaces;
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


			using (var repo = new Repository(repoPath))
			{
				var resolverStrategy = new DefaultResolverStrategy(repo);
				var progress = new Progress<string>(System.Console.WriteLine);

				var resolvedVersion = resolverStrategy.DetermineVersion(progress: progress);
				var version = new DefaultVersionFormatter().FormatVersion(resolvedVersion);

				return version.ToString();
			}
		}
	}
}
