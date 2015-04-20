using NSemVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Core.Format
{
	public class VersionInfo
	{
		public System.Version AssemblyVersion { get; set; }

		public System.Version FileVersion { get; set; }

		public SemVersion SemVersion { get; set; }
	}
}
