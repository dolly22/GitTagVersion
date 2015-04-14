using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Interfaces
{
	public interface IGitTagVersion
	{
		IDictionary<string, string> GetVersion(string discoverPath = ".");
	}
}
