using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace WebServer.Models
{
	public class Server
	{
		protected IPEndPoint serverEP;
		protected Socket listener;

		virtual protected string WEBROOT 
		{
			get { return Constants.WEBROOT; }
		}

		public Server(string host, int port)
		{
			this.serverEP = new IPEndPoint(IPAddress.Parse(host), port);

			this.listener = new Socket(serverEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
				listener.Bind(serverEP);
				listener.Listen(10);
			}
			catch (Exception ex)
			{
				listening = false;
				LoggerQueue.Put(ex.Message);
			}

			if (listening)
			{
				LoggerQueue.Put("Listening on " + serverEP.ToString() + "...");

				while (listening)
				{
					Socket so = listener.Accept();
					NetworkStream ns = new NetworkStream(so);
					this.handleClient(ns, so);
				}

				//tcpListener.Stop();
			}
		}

		virtual protected void handleClient(NetworkStream ns, Socket so)
		{
			new ServerThread(ns, so, WEBROOT).Run();
		}
	}
}