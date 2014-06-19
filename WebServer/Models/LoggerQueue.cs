using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer.Models
{


	class LoggerQueue
	{
		private const int BUFLEN = 3;

		private static Semaphore put = new Semaphore(BUFLEN, BUFLEN); //Er kan maar BUFLEN in de Queue;
		private static Semaphore get = new Semaphore(0, BUFLEN); //Er kan niet meer uit dan in

		private static string[] buffer = new string[BUFLEN];

		private Thread thread;

		private static int getpos, putpos;
		private static int count;

		public LoggerQueue()
		{
			this.thread = new Thread(new ThreadStart(Work));
			this.thread.Start();
		}

		public void Work()
		{
			while (true)
			{
				string msg = LoggerQueue.Get();

				using (StreamWriter sw = File.AppendText("./log.txt"))
				{
					sw.WriteLine(msg);
				}

			}
		}

		private static string Get()
		{
			get.WaitOne(); // Staat er iets in de Queue?
			lock (buffer)
			{
				string msg = buffer[getpos];
				getpos = (getpos + 1) % BUFLEN;
				count--;

				put.Release(); //Er is iets uit de Queue gehaald dus er kan weer iets in
				return msg;
			}
		}

		public static void Put(string msg)
		{
			put.WaitOne(); // Is er nog plek in de Queue?
			lock (buffer)
			{
				buffer[putpos] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss - ") + msg;
				
				Console.WriteLine(buffer[putpos]);

				putpos = (putpos + 1) % BUFLEN;
				count++;

				get.Release(); // Er is een item toegevoegd, er kan er dus 1 weer uit worden gehaald
			}

		}
	}
}
