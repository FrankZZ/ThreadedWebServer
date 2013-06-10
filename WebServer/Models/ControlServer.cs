using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace WebServer.Models
{
	public class ControlServer : Server
	{
		new public static string WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\ControlWebRoot");

		public ControlServer(string host, int port): base(host, port) {}

		/* Override */
		override protected void doListen()
		{
			bool listening = true;

			try
			{
				this.tcpListener.Start();
			}
			catch (Exception ex)
			{
				listening = false;
				Console.WriteLine(ex.Message);
			}

			if (listening)
			{
				Console.WriteLine("[CTRL] Listening on " + host + ":" + port + "...");

				while (listening)
				{
					new ServerThread(this.tcpListener.AcceptTcpClient().GetStream()).Run();
				}

				this.tcpListener.Stop();
			}
		}
	}
}