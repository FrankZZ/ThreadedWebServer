using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class Constants
	{
		public static string CONTROL_WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\ControlRoot");
		public static string WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\WebRoot");
		public static string CONTROL_URL = "https://localhost:3334";
		public static string CONFIG_URI = "Config.xml";
	}
}
