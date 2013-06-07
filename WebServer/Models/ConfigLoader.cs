using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace WebServer.Models
{
	public class ConfigLoader
	{
		private XmlReader xmlReader;

		public ConfigLoader(string URI)
		{
			URI = Application.StartupPath + @"\" + URI;

			if (File.Exists(URI))
			{
				xmlReader = new XmlTextReader(URI);
			}
		}

		public void read()
		{
			if (xmlReader == null) return;

			while (xmlReader.Read())
			{
				while (xmlReader.MoveToNextAttribute())
				{
					Console.WriteLine(xmlReader.Name);
				}
			}
		}
	}
}