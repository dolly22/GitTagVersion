using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTagVersion.Core.Resolver;

namespace GitTagVersion.Core.Format
{
    public interface IVersionFormatter
    {
        string Prefix { get; }

        IEnumerable<KeyValuePair<string, string>> Format(ResolvedVersionInfo versionInfo);
    }
}
