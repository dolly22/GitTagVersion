using LibGit2Sharp;
using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Git
{
    public class VersionTagDistanceResolver
    {
        public VersionTagDistanceResolver(Repository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.Repository = repository;
        }

        protected Repository Repository { get; set; }

        public TagDistance GetMostRecentDistance(object startReference, SortedList<SemVersion, Tag> tagVersions, int maxIterations = int.MaxValue)
        {
            int iteration = 0;
            var unwalkedParents = new Dictionary<Commit, CommitDistanceNode>();
            var tagDistances = new Dictionary<Tag, int>();

            var commits = Repository.Commits.QueryBy(new CommitFilter()
            {
                Since = startReference,
                SortBy = CommitSortStrategies.Topological
            });

            // walk commits children before parents
            foreach (var commit in commits)
            {
                CommitDistanceNode commitNode;
                iteration++;

                // limit by iterations
                if (iteration >= maxIterations)
                    break;

                if (unwalkedParents.TryGetValue(commit, out commitNode))
                {
                    // remove unwalked and // update commit countupdate commit count
                    unwalkedParents.Remove(commit);                    
                    commitNode.UpdateCommitsFromStart(iteration);
                }
                else
                {
                    if (iteration > 1)
                        throw new InvalidOperationException("Commit not found in unwalked nodes");
                    commitNode = new CommitDistanceNode();
                }

                // get tag with highest version
                var tag = tagVersions.Where(t => t.Value.Target == commit)
                    .OrderByDescending(t => t.Key)
                    .FirstOrDefault();
                
                if (tag.Key != null)
                {
                    // remove lower tag versions
                    foreach (var key in tagVersions.Keys.Where(k => k < tag.Key).ToList())
                        tagVersions.Remove(key);

                    tagDistances[tag.Value] = commitNode.CommitsFromStart;

                    // we have deterministrically come to an end
                    if (tagVersions.Count == 1)
                        break;
                }

                // link parent commits child references
                foreach (var parent in commit.Parents)
                {
                    CommitDistanceNode parentCommitNode;

                    // add unknown parents to unwalked
                    if (!unwalkedParents.TryGetValue(parent, out parentCommitNode))
                    {
                        // if not already stored, create new node cotnainer
                        parentCommitNode = new CommitDistanceNode();
                        unwalkedParents.Add(parent, parentCommitNode);
                    }
                    parentCommitNode.Children.Add(commitNode);
                }
            }

            // break or deterministic end
            foreach (var tag in tagVersions.Reverse())
            {
                int commitDistance;
                if (tagDistances.TryGetValue(tag.Value, out commitDistance))
                {
                    return new TagDistance
                    {
                        Tag = tag.Value,
                        TagVersion = tag.Key,
                        Distance = commitDistance
                    };
                }
            }
            return null;
        }

        public class TagDistance
        {
            public Tag Tag { get; set; }

            /// <summary>
            /// Original version tag version
            /// </summary>
            public SemVersion TagVersion { get; set; }

            /// <summary>
            /// Distance (in commits) from resolved commit
            /// </summary>
            public int Distance { get; set; }
        }

        private class CommitDistanceNode
        {
            readonly IList<CommitDistanceNode> children = new List<CommitDistanceNode>();

            public IList<CommitDistanceNode> Children
            {
                get { return children; }
            }

            public int CommitsFromStart { get; private set; }

            private int uniqueCountTag;

            private int CountChildCommits(int uniqueCountTag)
            {
                var treeUniqueChildren = 0;

                // already walked here
                if (this.uniqueCountTag == uniqueCountTag)
                    return treeUniqueChildren;
                this.uniqueCountTag = uniqueCountTag;

                // walk to children
                foreach (var child in Children)
                {
                    treeUniqueChildren += child.CountChildCommits(uniqueCountTag);
                }

                // add self
                return treeUniqueChildren + 1;
            }

            public void UpdateCommitsFromStart(int uniqueCountTag)
            {
                if (Children.Count > 1)
                {
                    this.CommitsFromStart = CountChildCommits(uniqueCountTag);
                }
                else if (Children.Count == 1)
                {
                    CommitsFromStart = this.Children.First().CommitsFromStart + 1;
                }
            }
        }
    }
}
