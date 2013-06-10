using System;

namespace WebServer.Models
{
	public static class Init
	{
		private const string HOST = "0.0.0.0";
		private const int PORT = 3333;

		public static void Main(string[] args)
		{
			var config = new ConfigLoader("Config.xml").Read();

			var server = new Server(
				config.GetEntry("Host"), 
				Convert.ToInt32(config.GetEntry("Port"))
			);

			var sslServer = new SSLServer(
				config.GetEntry("Host"),
				443
			);

			var controlServer = new ControlServer(
				config.GetEntry("Host"),
				Convert.ToInt32(config.GetEntry("Port")) + 1
			);

			server.Run();
			sslServer.Run();
			controlServer.Run();
		}
	}
}