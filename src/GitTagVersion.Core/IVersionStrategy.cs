using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core
{
    public interface IVersionStrategy
    {
       VersionInfo DetermineVersion(string commitish = null, IProgress<string> progress = null);
    }
}
