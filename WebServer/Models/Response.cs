﻿using System;
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

			string result = String.Join("\r\n", lines);
			// 2 newlines voor seperator: headers - body
			result += "\r\n\r\n";

			Console.WriteLine(result);

			return result;
		}

		public void SetHeader(string key, string value)
		{
			if (headers.ContainsKey(key))
			{
				headers.Remove(key);
			}

			headers.Add(key, value);
		}
	}
}