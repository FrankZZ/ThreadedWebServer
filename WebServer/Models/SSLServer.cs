using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace WebServer.Models
{
	public class SSLServer : Server
	{
		private X509Certificate certificate = new X509Certificate("Certificate\\Certificate.pfx", "KTYy77216");

		new public static string WEBROOT = Path.GetFullPath(Environment.CurrentDirectory + @"\WebRoot");

		public SSLServer(string host, int port): base(host, port) {}

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
				Console.WriteLine("[SSL] Listening on " + host + ":" + port + "...");

				while (listening)
				{
					this.handleClient(this.tcpListener.AcceptTcpClient());
				}

				this.tcpListener.Stop();
			}
		}

		private void handleClient(TcpClient client)
		{
			try 
			{
				SslStream sslStream = new SslStream(client.GetStream(), false);

				sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Ssl3, false);

				new ServerThread(sslStream).Run();

			}
			catch (Exception ex)
			{
				
			}
		}
	}
}