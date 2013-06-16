using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebServer.Models
{
	public class Request
	{
		private string[] validMethods = {"GET"/*, "POST"*/};
		private string[] validProtocols = {"HTTP/1.0", "HTTP/1.1"};
		
		private String body;
		public String Body
		{
			get { return body; }
		}

		private Stream stream;
		public Stream Stream
		{
			get { return stream; }
		}

		private string method;
		public string Method
		{
			get { return method; }
		}

		private string webRoot;
		public string WebRoot
		{
			get { return webRoot; }
		}

		private string path;
		public string Path
		{
			get { return path; }
		}

		private string protocol;
		public string Protocol
		{
			get { return protocol; }
		}

		private Dictionary<string, string> headers;
		public Dictionary<string, string> Headers
		{
			get { return headers; }
		}

		private Dispatcher dispatcher;

		private Response response;
		public Response Response
		{
			get { return response; }
			set { response = value; }
		}

		public Request(Stream stream, string webRoot)
		{
			headers = new Dictionary<string, string>();
			dispatcher = new Dispatcher();
			this.webRoot = webRoot;
			this.stream = stream;

			//this.getBody();
		}

		public void SetHeader(string key, string value)
		{
			if (headers.ContainsKey(key))
			{
				headers.Remove(key);
			}

			headers.Add(key, value);
		}

		public void ParseRequest(string line)
		{
			Console.WriteLine("Request: " + line);

			var parts = line.Split(' ');

			if (parts.Length != 3) return;

			if (validMethods.Contains(parts[0]))
			{
				method = parts[0];
			}

			path = parts[1];

			if (validProtocols.Contains(parts[2]))
			{
				protocol = parts[2];
			}
		}

		public void ParseHeader(string line)
		{
			var parts = line.Split(':');

			if (parts.Length == 2)
			{
				SetHeader(parts[0], parts[1]);
			}
		}

		public void getBody()
		{
			using (StreamReader sr = new StreamReader(this.stream))
			{
				this.body = sr.ReadToEnd();
			}

			Console.WriteLine(this.body);
		}

		public void dispatch()
		{
			dispatcher.Dispatch(this);
		}
	}
}