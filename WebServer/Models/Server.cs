using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace WebServer.Models
{
	public class Server
	{
		private string host;
		private int port;
		private TcpListener tcpListener;

		public static string WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\WebRoot");

		public Server(string host, int port)
		{
			this.host = host;
			this.port = port;

			var addr = IPAddress.Parse(host);
			this.tcpListener = new TcpListener(addr, port);
		}

		public void listen()
		{
			bool listening = true;

			try
			{
				tcpListener.Start();
			}
			catch (Exception ex)
			{
				listening = false;
				Console.WriteLine(ex.Message);
			}

			if (listening)
			{
				Console.WriteLine("Listening on " + host + ":" + port + "...");

				while (listening)
				{
					new ServerThread(tcpListener.AcceptTcpClient()).Run();
				}

				tcpListener.Stop();
			}
		}
	}
}