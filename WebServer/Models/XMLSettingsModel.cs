using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebServer.Models
{
	[Serializable]
	public class XMLSettingsModel
	{
		[XmlAttribute]
		public String Host
		{
			get; 
			set;
		}

		[XmlAttribute]
		public String webPort
		{
			get;
			set;
		}

		[XmlAttribute]
		public String controlPort
		{
			get;
			set;
		}
		
		[XmlAttribute]
		public String webRoot
		{
			get;
			set;
		}

		[XmlAttribute]
		public String defaultPage
		{
			get;
			set;
		}

		[XmlAttribute]
		public String directoryBrowsing
		{
			get;
			set;
		}

		public Dictionary<string, string> ToDictionary()
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			
			Type type = this.GetType();
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo item in properties)
			{
				if (item.PropertyType.FullName == "System.String")
					dict.Add(item.Name, (string)item.GetValue(this));
			}
			
			return dict;
		}
	}
}
