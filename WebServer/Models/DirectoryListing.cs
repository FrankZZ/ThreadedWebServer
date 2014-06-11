using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class DirectoryListing
	{
		public static String Generate(String pathString)
		{
			String html = "<ul>";

			String[] dirList = Directory.GetDirectories(pathString);

			for(int i = 0; i<dirList.Length; i++)
			{
				String dir = dirList[i].Substring(pathString.Length);

				html += "<li><a href=\"./" + dir + "\">" + dir + "</a></li>";
			}

			String[] fileList = Directory.GetFiles(pathString);

			for (int i = 0; i < fileList.Length; i++)
			{
				String file = fileList[i].Substring(pathString.Length);

				html += "<li><a href=\"./" + file + "\">" + file + "</a></li>";
			}

			html += "</ul>";

			return html;
		}
	}
}
