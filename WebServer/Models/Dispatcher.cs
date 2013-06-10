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

			bool isDirectory = false;

			string absolutePath = Path.GetFullPath(request.WebRoot + request.Path);
			
			// Check for webroot jail breakout
			if (absolutePath.StartsWith(request.WebRoot))
			{
				if (!Path.HasExtension(request.Path))
				{
					if (!absolutePath.EndsWith("/"))
					{
						absolutePath += "/";
					}

					absolutePath += "index.html";
					Console.WriteLine(absolutePath);
					isDirectory = true;
				}

				try
				{
					String fileExtension = Path.GetExtension(absolutePath).Substring(1);
					
					if (MimeTypes.List.ContainsKey(fileExtension))
					{
						response.SetHeader("Content-Type", MimeTypes.List[fileExtension]);
					}

					var buffer = new byte[1024];
					var stream = request.Stream;

					using (FileStream fs = File.OpenRead(absolutePath))
					{

						byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
						
						stream.Write(headers, 0, headers.Length);
						
						while (fs.Read(buffer, 0, buffer.Length) > 0)
						{
							stream.Write(buffer, 0, buffer.Length);
						}

						stream.Flush();
					}

				}
				catch (AccessDeniedException ex)
				{
					response.Status = 404; // Not found
				}
				catch (FileNotFoundException ex)
				{
					if (isDirectory)
					{
						// index.html not found in the directory

						response.Status = 403; // Forbidden
					}
					else
					{
						response.Status = 404; // Not found
					}
				}
			}
		}
	}
}