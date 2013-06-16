using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace WebServer.Models
{
	public class ControlServer : Server
	{
		private X509Certificate certificate = new X509Certificate("Certificate\\Certificate.pfx", "KTYy77216");

		override protected string WEBROOT
		{
			get { return Path.GetFullPath(Environment.CurrentDirectory + @"\ControlRoot"); }
		}

		public ControlServer(string host, int port) : base(host, port) { }

		override protected void handleClient(NetworkStream ns, Socket so)
		{
			try
			{
				SslStream sslStream = new SslStream(ns, false);

				sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Ssl3, false);

				new ServerThread(sslStream, so, WEBROOT).Run();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return;
		}
	}
}