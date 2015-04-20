using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Resolver
{
    public interface IResolverStrategy
    {
       ResolvedVersionInfo DetermineVersion(string commitish = null, IProgress<string> progress = null);
    }
}
