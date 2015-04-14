using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core;
using System.Text.RegularExpressions;
using NSemVersion;

namespace GitTagVersion.Core.Git
{
    /// <summary>
    /// Helper class to discover anchoring tags for semantic version.
    /// </summary>
    public class VersionTagFinder
    {
        // v1.0.0 or 1.0.0 format
        private static readonly Regex DefaultVersionRegex
            = new Regex(@"^v?(?<version>.*)$", RegexOptions.Singleline);

        public VersionTagFinder(Repository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.Repository = repository;
        }

        protected Repository Repository { get; set; }

        /// <summary>
        /// Get repository tags matching v1.0.0 version formats
        /// </summary>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public SortedList<SemVersion, Tag> GetVersionTags(SemVersion maxVersion = null)
        {
            var versionTags = new SortedList<SemVersion, Tag>();
            foreach (var tag in Repository.Tags)
            {
                var match = DefaultVersionRegex.Match(tag.Name);
                if (!match.Success)
                    continue;

                // fist capture shouuld be version number
                var versionName = match.Groups["version"].Value;

                SemVersion semVersion;
                if (SemVersion.TryParse(versionName, out semVersion))
                {
                    if (maxVersion != null && semVersion > maxVersion)
                        continue;

                    versionTags.Add(semVersion, tag);
                }
            }
            return versionTags;
        }
    }
}
