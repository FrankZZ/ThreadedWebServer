using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServer.Exceptions;

namespace WebServer.Models
{
	class Cookie
	{
		private static Dictionary<string, HttpCookie> _cookies;

		public static HttpCookie GetCookie(string cookieId)
		{
			if (!_cookies.ContainsKey(cookieId))
				throw (new CookieNotFoundException());

			return _cookies[cookieId];
		}

		public static HttpCookie ParseCookie(string cookieString)
		{
			HttpCookie cookie = new HttpCookie();

			var variables = cookieString.Split(new char[] { ';' });

			foreach (string part in variables)
			{
				string[] keyval = part.Split(new char[] { ':' }, 2);
				cookie[keyval[0]] = keyval[1];
			}

			return cookie;
		}
	}
}
