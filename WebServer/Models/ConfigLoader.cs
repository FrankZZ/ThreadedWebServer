using System;
using System.Xml;

namespace WebServer.Models
{
	public class ConfigLoader
	{
		private XmlReader xmlReader;

		public ConfigLoader(string URI)
		{
			xmlReader = new XmlTextReader(URI);
		}

		public void read()
		{
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