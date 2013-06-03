using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Models.Exceptions;

namespace WebServer.Models.Models
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

			if (true)
			{
				request.Response = response;

				FileReader fr = new FileReader(request.Path);

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
					response.Status = 404; // Not found
				}

				response.Body = fr.getContents();
			}
		}
	}
}