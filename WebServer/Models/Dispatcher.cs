using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class Dispatcher
	{
		private Request request;
		private Response response;

		private const string ERROR_PATH = "Errors";

		public Dispatcher()
		{
			this.response = new Response();
		}

		public void Dispatch(Request request)
		{
			this.request = request;

			if (request.Method == "POST")
			{
				if (request.Path == "/index.html")
				{
					DispatchLogin();
				}
				else if (request.Path == "/index2.html") // Config
				{
					DispatchConfig();
				}
			}
			else
			{
				if (request.Path == "/index2.html") // Config
				{

/*					if (request.Headers.ContainsKey("Cookie"))
					{
						String cookie = request.Headers["Cookie"];
						var values = Cookie.Parse(cookie);
					}
*/				}
			}

			request.Response = response;

			int status = CheckStatus();

			if (status != 200)
			{
				Console.WriteLine("Error");
				WriteError(status);
			}
		}

		private void DispatchLogin()
		{
			var user = request.Params.Get("username");
			var pass = request.Params.Get("password");
			var token = request.Params.Get("token");

/*			Session sess = null;

			if (request.Headers.ContainsKey("Cookie"))
			{
				String cookie = request.Headers["Cookie"];
				var values = Cookie.Parse(cookie);
			}
			else
			{
				sess = Session.New();

				this.response.Headers.Add("Set-Cookie",
					"SessID=" + sess.Id + "; Expires=Wed, 09 Jun 2021 10:18:14 GMT; secure");
			}
*/		}

		private void DispatchConfig()
		{
			// Empty
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
					

					if (Directory.Exists(absolutePath))
					{
						if (File.Exists(absolutePath + "index.html"))
						{
							absolutePath += "index.html";
						}
						else
						{
							String data = DirectoryListing.Generate(absolutePath);

							response.SetHeader("Content-Type", MimeTypes.List["html"]);

							byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
							byte[] body = Encoding.UTF8.GetBytes(data);

							if (request.Stream.CanWrite)
							{
								request.Stream.Write(headers, 0, headers.Length);
								request.Stream.Write(body, 0, body.Length);

								return 200;
							}
						}
					
					}

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
			response.Status = status;

			string file = Application.StartupPath + @"\" + ERROR_PATH + @"\" + status + ".html";

			if (File.Exists(file))
			{
				string data = null;

				using (var sr = new StreamReader(file))
				{
					data = sr.ReadToEnd();
				}

				byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
				byte[] body = Encoding.UTF8.GetBytes(data);

				if (request.Stream.CanWrite)
				{
					request.Stream.Write(headers, 0, headers.Length);
					request.Stream.Write(body, 0, body.Length);
				}
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