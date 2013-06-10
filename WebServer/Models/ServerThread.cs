﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace WebServer.Models
{
	public class ServerThread : IDisposable
	{
		private const int BUFFER_LENGTH = 4096;
		private const int MAX_HEADER_LENGTH = 256;

		private TcpClient tcpClient;
		private Thread thread;

		private Stream stream;

		public ServerThread(Stream stream)
		{
			this.stream = stream;
			this.thread = new Thread(new ThreadStart(Work));
		}

		public void Run()
		{
			if (!thread.IsAlive)
			{
				thread.Start();
			}
		}

		private void Work()
		{
			if (this.stream.CanRead)
			{
				try
				{
					var request = new Request(stream);

					using (StreamReader sr = new StreamReader(stream))
					{
						if (sr.Peek() > 0)
						{
							var buffer = new char[BUFFER_LENGTH];
							sr.Read(buffer, 0, buffer.Length);

							var lines = new String(buffer).Trim().Split('\n');
							bool isFirst = true;

							foreach (string value in lines)
							{
								string line = value.Trim();

								if (!String.IsNullOrEmpty(line) && line.Length < MAX_HEADER_LENGTH)
								{
									Console.WriteLine(line);

									if (isFirst)
									{
										request.ParseRequest(line);
										isFirst = false;

										continue;
									}

									request.ParseHeader(line);
								}
							}

							request.dispatch(this.stream);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		public void Dispose()
		{
			// Empty
		}
	}
}