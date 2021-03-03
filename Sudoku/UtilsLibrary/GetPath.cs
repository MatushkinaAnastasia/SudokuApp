using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace UtilsLibrary
{
	public static class GetPath
	{
		public static string MakePath (string key)
		{
			var pathTo = ConfigurationManager.AppSettings.Get(key);
			Console.WriteLine(pathTo);

			return pathTo;
		}
	}
}
