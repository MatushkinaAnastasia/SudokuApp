using System;
using System.Configuration;

namespace UtilsLibrary
{
	public static class PathWorker
	{
		public static string GetPath (string key)
		{
			var pathTo = ConfigurationManager.AppSettings.Get(key);
			Console.WriteLine(pathTo);

			return pathTo;
		}
	}
}
