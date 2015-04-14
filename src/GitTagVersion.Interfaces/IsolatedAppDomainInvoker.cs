using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Interfaces
{
	public class IsolatedAppDomainInvoker
	{
		const string AssemblyName = "GitTagVersion.Core";
		const string GitTagVersionMarshallType = "GitTagVersion.Core.GitTagVersionInvoker";

		public static IDictionary<string, string> RunGitTagVersion(bool verbose)
		{
			var loadDirectory = new Uri(typeof(IsolatedAppDomainInvoker).Assembly.CodeBase).LocalPath;

			// get base path
			var appBasePath = Path.GetDirectoryName(loadDirectory);

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			try
			{
				var domainSetup = new AppDomainSetup()
				{
					ApplicationBase = appBasePath,
					LoaderOptimization = LoaderOptimization.MultiDomainHost
				};

				// create new app domain
				var domain = AppDomain.CreateDomain("GitTagVersionInvoker", AppDomain.CurrentDomain.Evidence, domainSetup);
				try
				{
					var invoker = (IGitTagVersion)domain.CreateInstanceAndUnwrap(AssemblyName, GitTagVersionMarshallType);
					return invoker.GetVersion();
				}
				finally
				{
					// unload app domain
					AppDomain.Unload(domain);
				}
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			}
		}

		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			return loadedAssemblies.FirstOrDefault(a => a.FullName == args.Name);
		}
	}
}
