using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Git
{
    public class VersionBranchParser
    {
        private static readonly Regex DefaultVersionRegex 
            = new Regex(@"^v?(?<version>\d+\.\d+(?:\.\d+)?)\-master$", RegexOptions.Singleline);

        public VersionBranchParser(Repository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.Repository = repository;
        }

        protected Repository Repository { get; set; }

        public BranchInfo GetBranchInfo(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException("branch");

            var version = ParseVersionBranch(branch);
            if (version != null)
            {
                // found version branch
                return new BranchInfo
                {
                    Version = version
                };
            }

            var logicalBranchName = ParseNamedBranch(branch);
            if (!String.IsNullOrWhiteSpace(logicalBranchName))
            {
                // found named branch
                return new BranchInfo
                {
                    LogicalName = logicalBranchName
                };
            }

            return null;
        }

        protected System.Version ParseVersionBranch(Branch branch)
        {
            System.Version version;

            if (branch == null)
                throw new ArgumentNullException("branch");

            var versionMatch = DefaultVersionRegex.Match(branch.Name);
            if (versionMatch.Success)
            {
                // prefix successfully parsed from info
                var parsedVersion = versionMatch.Groups["version"].Value;

                if (!System.Version.TryParse(parsedVersion, out version))
                    throw new InvalidOperationException(String.Format("Unable to parse version from string: {0}", parsedVersion));

                return version;
            }
            return null;
        }

        protected string ParseNamedBranch(Branch branch)
        {
            return branch.Name;
        }

        public class BranchInfo
        {
            public bool IsVersionBranch
            {
                get { return Version != null; }
            }

            public System.Version Version { get; set; }

            public bool IsNamedBranch
            {
                get { return !String.IsNullOrWhiteSpace(LogicalName); }
            }

            public string LogicalName { get; set; }
        }
    }
}
