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

		static void Main(string[] args)
		{
			var server = new Server(HOST, PORT);
			server.listen();
		}
	}
}