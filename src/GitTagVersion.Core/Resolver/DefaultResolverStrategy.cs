using GitTagVersion.Core.Git;
using GitTagVersion.Core.Resolver;
using LibGit2Sharp;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Resolver
{
	public class DefaultResolverStrategy : IResolverStrategy
	{
		readonly Repository repository;

		public DefaultResolverStrategy(Repository repository)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");

			this.repository = repository;
		}

		public ResolvedVersionInfo DetermineVersion(string commitish = null, IProgress<string> progress = null)
		{
			var commit = repository.Head.Tip;
			if (!String.IsNullOrWhiteSpace(commitish))
			{
				commit = repository.Lookup<Commit>(commitish);
				ReportProgress(progress, String.Format("Using explicit commit: {0}", commit.Sha));
			}

			return GetVersionInfo(commit, progress);
		}

		private ResolvedVersionInfo GetVersionInfo(Commit commit, IProgress<string> progress)
		{
			var versionInfo = new ResolvedVersionInfo()
			{
				Commit = commit
			};

			SemVersion maxVersion = null;
			VersionBranchParser.BranchInfo branchVersionInfo = null;

			// get local branch from commit
			var branch = repository.Branches
				.FirstOrDefault(b => !b.IsRemote && b.IsCurrentRepositoryHead && b.Tip == commit);

			if (branch != null)
			{
				ReportProgress(progress, String.Format("Parsing local branch name: {0}", branch.Name));

				// parse commit version
				branchVersionInfo = new VersionBranchParser(repository).GetBranchInfo(branch);

				// fix highest possible version from branch?
				if (branchVersionInfo.IsVersionBranch)
				{
					ReportProgress(progress, String.Format("Found version branch: {0}", branchVersionInfo.Version));
					var branchVersion = branchVersionInfo.Version;

					// 1.1 -> 1.1.x
					maxVersion = new SemVersion(branchVersion.Major, branchVersion.Minor, branchVersion.Build == -1 ? int.MaxValue : branchVersion.Build);
				}
			}
			else
			{
				ReportProgress(progress, String.Format("Local branch not resolved for commit: {0}", commit.Sha));
			}

			// parse repository tags
			var tags = new VersionTagFinder(repository).GetVersionTags(maxVersion);
			if (tags.Any())
			{
				var tagsInfo = String.Join(", ", tags.Select(t => t.Key));
				ReportProgress(progress, String.Format("Version tags: {0}", tagsInfo));

				// count tag commit distances
				var tagDistance = new VersionTagDistanceResolver(repository).GetMostRecentDistance(commit, tags);
				if (tagDistance != null)
				{
					ReportProgress(progress, String.Format("Most recent tag distance resolved - tag: {0}, distance: {1}", tagDistance.TagVersion, tagDistance.Distance));

					// set versions
					versionInfo.SemVersion = tagDistance.TagVersion;
					versionInfo.SemVersionRevision = tagDistance.Distance;
				}
			}
			else
			{
				// no tags to anchor
				ReportProgress(progress, "No anchoring version tags were found");
			}

			// update version name
			if (branchVersionInfo != null && branchVersionInfo.IsNamedBranch)
			{
				ReportProgress(progress, String.Format("Named branch: {0}", branchVersionInfo.LogicalName));
				versionInfo.AdditionalMetadata = branchVersionInfo.LogicalName;
			}

			// use default version if no tagged version is available
			if (versionInfo.SemVersion == null)
			{
				// count commits from start
				var distanceToFirstRepoCommit = repository.Commits.QueryBy(new CommitFilter
				{
					Since = commit,
					SortBy = CommitSortStrategies.Topological
				}).Count();
				
				versionInfo.SemVersion = new SemVersion(0, 1, 0);
				versionInfo.SemVersionRevision = distanceToFirstRepoCommit;
			}

			return versionInfo;
		}

		private void ReportProgress(IProgress<string> progress, string message)
		{
			if (progress != null)
			{
				try
				{
					progress.Report(message);
				}
				catch (Exception)
				{
					// consume any
				}
			}
		}
	}
}
