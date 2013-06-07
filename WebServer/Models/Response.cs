using System;
using System.Collections.Generic;

namespace WebServer.Models
{
	public class Response
	{
		private static Dictionary<int, string> statuses = new Dictionary<int, string>()
		{
			{ 200, "OK" },
			{ 400, "Bad Request"},
			{ 403, "Forbidden" },
			{ 404, "Not Found" }
		};

		private string protocol = "HTTP/1.1";

		private int status = 200;
		public int Status
		{
			get { return status; }
			set { status = value; }
		}

		private Dictionary<string, string> headers;
		public Dictionary<string, string> Headers
		{
			get { return headers; }
		}

		private string body;
		public string Body
		{
			set { body = value; }
			get { return body; }
		}

		public Response()
		{
			headers = new Dictionary<string, string>()
			{
				{ "Content-Type", "text/html; charset=utf-8" }
			};
		}

		public override string ToString()
		{
			var lines = new List<string>()
			{
				protocol + " "  + status + " " + statuses[status]
			};

			foreach (var pair in headers)
			{
				lines.Add(pair.Key + ": " + pair.Value);
			}

			lines.Add(null);
			lines.Add(body);

			string result = String.Join("\n", lines);

			Console.WriteLine(result);

			return result;
		}
	}
}