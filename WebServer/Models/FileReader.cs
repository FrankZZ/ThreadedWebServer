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

		public string MimeType
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
				_fileExtension = Path.GetExtension(URI).Substring(1);
				if (MimeTypes.List.ContainsKey(_fileExtension))
				{
					_mimeType = MimeTypes.List[_fileExtension];
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("FileReader: Unknown error while parsing URI " + URI);
				//Console.WriteLine(ex.ToString());
			}

		}

		public void sendContents(Stream stream)
		{

			var buffer = new byte[CONTENT_BUFFER_SIZE];
			
			


			using (FileStream fs = File.OpenRead(this.URI))
			{
				using (BinaryWriter bw = new BinaryWriter(stream))
				{
					int read;
					while (fs.Read(buffer, 0, buffer.Length) > 0)
					{
						bw.Write(buffer);
					}
				}
			}
		
		}
	}
}
