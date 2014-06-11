using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class Session
	{
		public static Session Initialize()
		{
			return new Session();
		}

		private Dictionary<string, string> values;

		public string Id
		{
			get { return this.values["SessionId"]; }
		}

		public Session()
		{
			this.values = new Dictionary<string, string>();
			
			this.values["SessionId"] = Guid.NewGuid().ToString("N");
		}

		public void SetValue(string key, string value)
		{
			if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value) || values.ContainsKey(key))
				throw new ArgumentException();

			values.Add(key, value);
		}

		public string GetValue(string key)
		{
			if (String.IsNullOrEmpty(key))
				throw new ArgumentException();

			if (!values.ContainsKey(key))
				throw new ArgumentOutOfRangeException();

			return values[key];
		}
	}
}
