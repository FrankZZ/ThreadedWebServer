using System;

namespace WebServer.Models
{
	public static class Init
	{
		public static void Main(string[] args)
		{
			new LoggerQueue();
			
			var config = new ConfigLoader("Config.xml").Read();

			var server = new Server(
				config.GetEntry("Host"), 
				Convert.ToInt32(config.GetEntry("Port"))
			);

			var controlServer = new ControlServer(
				config.GetEntry("Host"),
				Convert.ToInt32(config.GetEntry("SSLPort"))
			);

			server.Run();
			controlServer.Run();

		}
	}
}