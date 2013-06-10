using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace WebServer.Models
{
	public class Server
	{
		protected string host;
		protected int port;
		protected TcpListener tcpListener;

		public static string WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\WebRoot");

		public Server(string host, int port)
		{
			this.host = host;
			this.port = port;

			var addr = IPAddress.Parse(host);
			this.tcpListener = new TcpListener(addr, port);

		}

		public void Run()
		{
			Thread thread = new Thread(new ThreadStart(doListen));
			thread.Start();
			
		}

		virtual protected void doListen()
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
				Console.WriteLine("[WEB] Listening on " + host + ":" + port + "...");

				while (listening)
				{
					new ServerThread(tcpListener.AcceptTcpClient().GetStream()).Run();
				}

				tcpListener.Stop();
			}
		}
	}
}