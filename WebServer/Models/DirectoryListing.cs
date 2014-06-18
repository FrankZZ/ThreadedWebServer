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
		public static String Generate(String rootPathString, String pathString)
		{
			pathString = pathString.TrimEnd('/');

			String html = "<html><head><title>Directory Listing</title></head><body><h1>Directory Listing</h1><ul>";

			String[] dirList = Directory.GetDirectories(pathString);

			for(int i = 0; i<dirList.Length; i++)
			{
				String url = dirList[i].Substring(rootPathString.Length + 1);
				String dir = dirList[i].Substring(pathString.Length);

				html += "<li><a href=\"/" + url + "\">" + dir + "</a></li>";
			}

			String[] fileList = Directory.GetFiles(pathString);

			for (int i = 0; i < fileList.Length; i++)
			{
				String url = fileList[i].Substring(rootPathString.Length + 1);
				String file = fileList[i].Substring(pathString.Length);

				html += "<li><a href=\"/" + url + "\">" + file + "</a></li>";
			}

			html += "</ul></body></html>";

			return html;
		}
	}
}
