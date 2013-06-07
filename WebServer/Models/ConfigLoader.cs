using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Collections.Generic;

namespace WebServer.Models
{
	public class ConfigLoader
	{
		private XmlReader xmlReader;
		private Dictionary<String, String> entries = new Dictionary<string, string>();

		public ConfigLoader(string URI)
		{
			URI = Application.StartupPath + @"\" + URI;

			if (File.Exists(URI))
			{
				xmlReader = new XmlTextReader(URI);
			}
		}

		public ConfigLoader Read()
		{
			if (xmlReader == null) return null;

			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					if (xmlReader.Name == "Entry")
					{
						entries.Add(xmlReader.GetAttribute("name"), 
							xmlReader.GetAttribute("value"));
					}
				}
			}

			return this;
		}

		public string GetEntry(string key)
		{
			if (entries.ContainsKey(key))
			{
				return entries[key];
			}

			return null;
		}
	}
}