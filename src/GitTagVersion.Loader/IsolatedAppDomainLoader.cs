using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitTagVersion.Loader
{
	public class IsolatedAppDomainLoader
	{
		const string AssemblyName = "GitTagVersion.Core";
		const string GitTagVersionMarshallType = "GitTagVersion.Core.GitTagVersionInvoker";

		public static IDictionary<string, string> Run(bool verbose)
		{
			var loadDirectory = new Uri(typeof(IsolatedAppDomainLoader).Assembly.CodeBase).LocalPath;

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
				var domain = AppDomain.CreateDomain("GitTagVersion", AppDomain.CurrentDomain.Evidence, domainSetup);
				try
				{
					var invoker = (IGitTagVersionInvoker)domain.CreateInstanceAndUnwrap(AssemblyName, GitTagVersionMarshallType);
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
