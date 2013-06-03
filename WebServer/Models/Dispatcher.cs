using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			string absolutePath = Path.GetFullPath(Server.WEBROOT + request.Path);
			
			// Check for webroot jail breakout
			if (absolutePath.StartsWith(Server.WEBROOT))
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

				FileReader fr = new FileReader(absolutePath);

				try
				{
					fr.Parse();

					response.Body = fr.getContents();
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