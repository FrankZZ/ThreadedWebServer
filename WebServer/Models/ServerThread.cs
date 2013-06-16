using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace WebServer.Models
{
	public class ServerThread : IDisposable
	{
		private const int BUFFER_LENGTH = 256;
		private const int MAX_HEADER_LENGTH = 256;

		private Thread thread;

		private string webRoot;

		private Socket listener;

		private Stream stream;

		public ServerThread(Stream stream, Socket listener, string WebRoot)
		{
			this.webRoot = WebRoot;
			this.listener = listener;
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
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			if (this.stream.CanRead)
			{
				try
				{
					var request = new Request(stream, this.webRoot);

					using (StreamReader sr = new StreamReader(stream))
					{
						if (sr.Peek() < 0)
						{
							return;
						}

						bool isFirst = true;
						bool isBody = false;

		
						String line = null;

						while (sr.Peek() >= 0)
						{

							line = sr.ReadLine();

							if (!String.IsNullOrEmpty(line))
							{
								Console.WriteLine("Line: " + line);

								if (isBody)
								{
									Console.WriteLine("Body!!!");
								}
								else
								{
									if (line.Length < MAX_HEADER_LENGTH)
									{
										if (isFirst)
										{
											request.ParseRequest(line);
											isFirst = false;

											continue;
										}

										request.ParseHeader(line);
									}
								}
							}
							else
							{
								break; //Body komt eraan
							}

						}

						if (request.Method == "POST")
						{
							int length = Convert.ToInt32(request.Headers["Content-Length"]);
							char[] buffer = new char[length];

							sr.Read(buffer, 0, buffer.Length);

							String body = new String(buffer);

							request.Params = HttpUtility.ParseQueryString(body);

							Console.WriteLine(request.Params.Get("username"));
						}

						request.dispatch();
						
						String ip = ((IPEndPoint)listener.RemoteEndPoint).Address.ToString();
						
						listener.Close();
						stream.Dispose();
						
						this.Log(stopwatch, request, ip);
					}

					Console.WriteLine("-");
					
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}

		private void Log(Stopwatch stopwatch, Request request, String ip)
		{
			stopwatch.Stop();

			LoggerQueue.Add(ip + " - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - "
				+ stopwatch.ElapsedMilliseconds + "ms" + ": " + request.Method + " " + request.Path);
		}

		public void Dispose()
		{
			// Empty
		}
	}
}