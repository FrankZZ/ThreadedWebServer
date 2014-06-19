using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WebServer.Models
{
	public class Configuration
	{
		private static XmlSerializer serializer;

		private static string URI = Application.StartupPath + @"\Settings.xml";

		public static XMLSettingsModel Entries;

		public static bool ShutdownRequested = false;

		public static void Initialize()
		{
			Configuration.serializer = new XmlSerializer(typeof(XMLSettingsModel), new XmlRootAttribute() { ElementName = "Settings" });
			Configuration.Entries = new XMLSettingsModel();
		}

		public static void Read()
		{
			if (File.Exists(Configuration.URI))
			{
				using (FileStream fs = new FileStream(Configuration.URI, FileMode.Open, FileAccess.Read))
				{
					Entries = (XMLSettingsModel)serializer.Deserialize(fs);
				}
			}
		}

		public static void Write()
		{
			if (!File.Exists(Configuration.URI))
				File.Create(Configuration.URI).Close();

			using (FileStream fs = new FileStream(Configuration.URI, FileMode.Truncate, FileAccess.Write))
			{
				serializer.Serialize(fs, Entries);
			}
		}
	}
}
