using System;

namespace WebServer.Models
{
	/*
	 * Goed doorlezen: http://bit.ly/1153Eqd
	 * Na het inloggen nog een keer vernieuwen
	 * */

	public static class Init
	{
		private const string HOST = "0.0.0.0";
		private const int PORT = 3333;

		public static void Main(string[] args)
		{
			new ConfigLoader("Config.xml").read();

			var server = new Server(HOST, PORT);
			server.listen();
		}
	}
}