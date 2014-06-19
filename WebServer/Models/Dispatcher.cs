using System;
using System.Collections.Generic;
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
		protected Request request;
		protected Response response;

		private const string ERROR_PATH = "Errors";

		public Dispatcher()
		{
			this.response = new Response();
		}

		public virtual void Dispatch(Request request)
		{
			this.request = request;
			int status = CheckStatus(false, null);

			request.Response = response;

			if (status != 200 && status != 302)
			{
				LoggerQueue.Put("Dispatcher: Status is " + status + " " + request.Path);
				WriteError(status);
			}
		}

		protected int CheckStatus(bool parameterized, Dictionary<string, string> parameters)
		{
			string requestPath = request.Path;
			int index = requestPath.IndexOf("?");

			if (index > 0)
			{
				requestPath = requestPath.Substring(0, index);
			}

			string targetPath = request.WebRoot + requestPath;
			string absolutePath = Path.GetFullPath(targetPath);
			absolutePath = absolutePath.Replace("/", @"\");

			if (absolutePath.StartsWith(request.WebRoot))
			{
				if (!Path.HasExtension(requestPath))
				{
					if (!absolutePath.EndsWith("/")) absolutePath += "/";
					

					if (Directory.Exists(absolutePath))
					{
						if (File.Exists(absolutePath + Configuration.Entries.defaultPage))
						{
							absolutePath += Configuration.Entries.defaultPage;
						}
						else
						{
							if (Configuration.Entries.directoryBrowsing == "true")
							{
								String data = DirectoryListing.Generate(request.WebRoot, absolutePath);

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
							else
							{
								return 403;
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

					if (parameterized && WriteFileParameterized(absolutePath, parameters))
					{
						return 200;
					}
					else if (WriteFile(absolutePath))
					{
						return 200;
					}
				}

				return 404;
			}

			return 403;
		}

		protected void WriteError(int status)
		{

			
			response.SetHeader("Content-Type", MimeTypes.List["html"]);
			response.Status = status;

			string file = Application.StartupPath + @"\" + ERROR_PATH + @"\" + status + ".html";

			if (File.Exists(file))
			{
				LoggerQueue.Put("Dispatcher: Showing error page " + status);
				WriteFile(file);
			}
			else
			{
				LoggerQueue.Put("Dispatcher: Cannot find error page for status " + status);
				WriteFile(Application.StartupPath + @"\" + ERROR_PATH + @"\404_errorpage.html");
			}
				
		}
		
		protected bool WriteFileParameterized(string file, Dictionary<string, string> data)
		{
			try
			{
				var buffer = new byte[128];
				var stream = request.Stream;

				using (StreamReader fs = new StreamReader(File.OpenRead(file)))
				{
					byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
					stream.Write(headers, 0, headers.Length);
					
					while (request.Stream.CanWrite && fs.Peek() > 0)
					{
						string line = fs.ReadLine();

						foreach (KeyValuePair<string, string> entry in data)
						{
							line = line.Replace("{{ " + entry.Key + " }}", entry.Value);
						}

						request.Stream.Write(Encoding.UTF8.GetBytes(line), 0, line.Length);
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

		protected bool WriteFile(string file)
		{
			try
			{
				var buffer = new byte[128];
				var stream = request.Stream;

				using (FileStream fs = File.OpenRead(file))
				{
					byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
					stream.Write(headers, 0, headers.Length);
					int hasRead;

					while (request.Stream.CanWrite && (hasRead = fs.Read(buffer, 0, buffer.Length)) > 0)
					{
						request.Stream.Write(buffer, 0, hasRead);
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