using System;
using System.IO;
using System.Text;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class Dispatcher
	{
		private Request request;
		private Response response;

		public Dispatcher()
		{
			this.response = new Response();
		}

		public void Dispatch(Request request)
		{
			this.request = request;
			request.Response = response;

			int status = CheckStatus();

			if (status != 200)
			{
				Console.WriteLine("Error");
				WriteError(status);
			}
		}

		private int CheckStatus()
		{
			string requestPath = request.Path;
			int index = requestPath.IndexOf("?");

			if (index > 0)
			{
				requestPath = requestPath.Substring(0, index);
			}

			string targetPath = request.WebRoot + requestPath;
			string absolutePath = Path.GetFullPath(targetPath);

			if (absolutePath.StartsWith(request.WebRoot))
			{
				if (!Path.HasExtension(requestPath))
				{
					if (!absolutePath.EndsWith("/")) absolutePath += "/";
					absolutePath += "index.html";
				}

				if (File.Exists(absolutePath))
				{
					String fileExtension = Path.GetExtension(absolutePath).Substring(1);

					if (MimeTypes.List.ContainsKey(fileExtension))
					{
						response.SetHeader("Content-Type", MimeTypes.List[fileExtension]);
					}

					if (WriteFile(absolutePath))
					{
						return 200;
					}
				}

				return 404;
			}

			return 500;
		}

		private void WriteError(int status)
		{
			response.SetHeader("Content-Type", MimeTypes.List["html"]);

			string data = "<h2>Error " + status + "</h2>";

			byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
			byte[] body = Encoding.UTF8.GetBytes(data);

			if (request.Stream.CanWrite)
			{
				request.Stream.Write(headers, 0, headers.Length);
				request.Stream.Write(body, 0, body.Length);
			}
		}

		private bool WriteFile(string file)
		{
			try
			{
				var buffer = new byte[1024];
				var stream = request.Stream;

				using (FileStream fs = File.OpenRead(file))
				{
					byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
					stream.Write(headers, 0, headers.Length);

					while (request.Stream.CanWrite && fs.Read(buffer, 0, buffer.Length) > 0)
					{
						request.Stream.Write(buffer, 0, buffer.Length);
					}

					stream.Flush();
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}