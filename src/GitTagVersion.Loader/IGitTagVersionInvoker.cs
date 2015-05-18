using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Loader
{
	public interface IGitTagVersionInvoker
	{
		IDictionary<string, string> GetVersion(string discoverPath = ".", IProgress<string> progress = null);
	}
}
