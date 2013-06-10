using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace WebServer.Models
{
	public class ControlServer : Server
	{
		override protected string WEBROOT
		{
			get { return Path.GetFullPath(Environment.CurrentDirectory + @"\ControlWebRoot"); }
		}

		public ControlServer(string host, int port): base(host, port) {}
	}
}