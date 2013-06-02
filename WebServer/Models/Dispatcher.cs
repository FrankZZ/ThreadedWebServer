using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			if (true)
			{
				request.Response = response;

				response.Body = "<h1>Het werkt!</h1>";
			}
		}
	}
}