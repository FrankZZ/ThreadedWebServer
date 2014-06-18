using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class Cookie
	{
		public static string GetSessionIdFromString(string cookieString)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();

			string[] variables = cookieString.Split(new char[] { ';' });

			foreach (string part in variables)
			{
				
				string[] keyval = part.Trim().Split(new char[] { '=' }, 2);
				
				if (keyval.Length == 2)
				{
					if (keyval[0] == "SessId")
						return keyval[1];
				}
				else
					throw(new ArgumentException("Invalid cookie"));
			}

			return "error";
		}
	}
}
