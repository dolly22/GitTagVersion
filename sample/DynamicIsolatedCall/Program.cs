using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var assemblyFile = "..\\..\\..\\..\\src\\GitTagVersion.Core\\bin\\Debug\\GitTagVersion.Interfaces.dll";

			//var assembly = typeof(GitTagCaller).Assembly;
			var assembly = Assembly.LoadFrom(assemblyFile);
			var type = assembly.GetType("GitTagVersion.Interfaces.IsolatedAppDomainInvoker");
			var method = type.GetMethod("RunGitTagVersion", BindingFlags.Static | BindingFlags.Public);
			var instance = Activator.CreateInstance(type);

			var result = method.Invoke(null, new object[] { false }); 
		}
	}
}
