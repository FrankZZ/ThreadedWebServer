using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class LoggerQueue
	{
		private static EventWaitHandle wh = new AutoResetEvent(false);

		private Thread worker;
		
		private static String[] logQueue = new String[3];

		private static SemaphoreSlim sem = new SemaphoreSlim(logQueue.Length);

		private static int logIdx = -1;

		public LoggerQueue()
		{
			worker = new Thread(Work);
			worker.Start();
		}

		public static void Add(String line)
		{
			sem.Wait();

			lock (logQueue)
			{
				
				logIdx++;
				logQueue[logIdx] = line;
			}

			wh.Set();

		}

		public void Work()
		{
			while (true)
			{
				String line = null;

				lock (logQueue)
				{
					line = logQueue[0];

					if (line != null)
					{
						// Alles 1 naar links opschuiven
						var newArray = new String[logQueue.Length];

						Array.Copy(logQueue, 1, newArray, 0, logQueue.Length - 1);
						
						logQueue = newArray;

						logIdx--;
						sem.Release();
					}
				}

				if (line != null)
				{
					//Console.WriteLine("Writing to file: " + line);

					// Write the string to a file.
					
					using (StreamWriter sw = File.AppendText("./log.txt"))
					{
						sw.WriteLine(line);
					}
					//Console.WriteLine("Done: " + line);
				}
				else
				{
					//Console.WriteLine("Nothing to write, sleeping...");
					wh.WaitOne();
				}

			}
		}
	}
}
