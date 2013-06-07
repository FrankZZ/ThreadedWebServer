using System;
using System.IO;
using System.Text;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class FileReader
	{
		private const int CONTENT_BUFFER_SIZE = 512;

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
				_fileExtension = Path.GetExtension(URI);
			}
			catch (Exception ex)
			{
				Console.WriteLine("FileReader: Unknown error while parsing URI " + URI);
				//Console.WriteLine(ex.ToString());
			}

		}

		public string getContents()
		{
			StringBuilder result = new StringBuilder();

			var buffer = new char[CONTENT_BUFFER_SIZE];


			using (StreamReader sr = new StreamReader(this.URI))
			{
				
				while (sr.Read(buffer, 0, buffer.Length) != 0)
				{
					result.Append(buffer);
				}
			}

			return result.ToString();
		}
	}
}
