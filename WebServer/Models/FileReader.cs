using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class FileReader
	{
		private string URI;

		private string _mimeType;

		public string mimeType
		{
			get { return _mimeType; }
		}

		private string _fileExtension;

		public string FileExtension
		{
			get { return _fileExtension; }
		}



		public FileReader(string URI)
		{
			this.URI = URI;
		}


		public void Parse()
		{
			try 
			{
				string absolutePath = Path.GetFullPath(Server.WEBROOT + URI);
			
				// Check for webroot jail breakout
				if (absolutePath.StartsWith(Server.WEBROOT))
				{
					_fileExtension = Path.GetExtension(absolutePath);
				}
				else
				{
					throw new AccessDeniedException();
				}
			}
			catch (FileNotFoundException ex)
			{
				// Re-throw for Dispatcher
				throw ex;
			}
			catch (AccessDeniedException ex)
			{
				// Re-throw for Dispatcher
				throw ex;
			}
			catch (Exception ex)
			{
				Console.WriteLine("FileReader: Unknown error while parsing URI");
				Console.WriteLine(ex.ToString());
			}

		}

		public string getContents()
		{
			StringBuilder result = new StringBuilder();

			var buffer = new char[2048];

			try 
			{
				using (StreamReader sr = new StreamReader(this.URI))
				{
				
					while (sr.Read(buffer, 0, buffer.Length) != 0)
					{
						result.Append(buffer);
					}
				}
			}
			catch (Exception ex)
			{
				
			}

			return result.ToString();
		}
	}
}
