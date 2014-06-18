using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using WebServer.Exceptions;

namespace WebServer.Models
{
	public class Dispatcher
	{
		private Request request;
		private Response response;

		private const string ERROR_PATH = "Errors";

		public Dispatcher()
		{
			this.response = new Response();
		}

		public void Dispatch(Request request)
		{
			int status = -1;
			try
			{
				this.request = request;
				

				if (request.WebRoot == Constants.CONTROL_WEBROOT)
				{
					if (request.Method == "POST")
					{
						if (request.Path == "/index.html")
						{
							if (DispatchLogin())
							{
								return;
							}
							string token = Guid.NewGuid().ToString("N");
							GetSession().LastToken = token;
							Dictionary<string, string> dict = new Dictionary<string, string>();
							
							dict.Add("token", token);

							status = CheckStatus(true, dict);
						}
						else if (request.Path == "/index2.html") // Config
						{
							Session sess = GetSession();

							if (sess.User == null || !sess.User.IsAuthorized)
							{
								RedirectTo("/index.html");

								return;
							}

							if (sess.LastToken == request.Params.Get("token"))
							{
								if (!sess.User.IsAdmin)
								{
									status = 403;
								}
								else
								{
									Configuration.Entries.controlPort = request.Params.Get("controlPort");
									Configuration.Entries.defaultPage = request.Params.Get("defaultPage");
									Configuration.Entries.directoryBrowsing = request.Params.Get("directoryBrowsing");
									Configuration.Entries.webPort = request.Params.Get("webPort");
									Configuration.Entries.webRoot = request.Params.Get("webRoot");

									Configuration.Write();

									Dictionary<String, String> dict = Configuration.Entries.ToDictionary();
									string token = Guid.NewGuid().ToString("N");
									sess.LastToken = token;
									dict.Add("token", token);

									status = CheckStatus(true, dict);
								} 
								
							}
							else
								LoggerQueue.Put("Dispatcher: Detected form token mismatch.");

							
						}
					}
					else
					{
						if (request.Path == "/index.html")
						{
							Session sess = GetSession();

							if (sess.User != null && sess.User.IsAuthorized)
							{
								RedirectTo("/index2.html");

								return;
							}

							Dictionary<string, string> tokenDict = new Dictionary<string, string>();
							
							string token = Guid.NewGuid().ToString("N");

							sess.LastToken = token;
							tokenDict.Add("token", token);
							
							status = CheckStatus(true, tokenDict);
						}
						else if (request.Path == "/index2.html") // Config
						{
							Session sess = GetSession();

							if (sess.User == null || !sess.User.IsAuthorized)
							{
								RedirectTo("/index.html");

								return;
							}

							Dictionary<String, String> dict = Configuration.Entries.ToDictionary();
							string token = Guid.NewGuid().ToString("N");
							sess.LastToken = token;
							dict.Add("token", token);

							status = CheckStatus(true, dict);
						}
					}
				}
			}
			catch (Exception e)
			{
				status = 500;
				LoggerQueue.Put("Dispatcher: Got Exception: " + e.Message);
			}
			
			if (status == -1)
			{
				status = CheckStatus(false, null);
			}

			request.Response = response;

			

			if (status != 200 && status != 302)
			{
				LoggerQueue.Put("Dispatcher: Status is " + status + " " + request.Path);
				WriteError(status);
			}
		}

		private bool DispatchLogin()
		{
			string userName = request.Params.Get("username");
			string pass = request.Params.Get("password");
			string token = request.Params.Get("token");
			Session sess = null;
			sess = GetSession();

			if (sess.LastToken == request.Params.Get("token"))
			{
				if (sess.User != null && sess.User.IsAuthorized)
				{
					RedirectTo("/index2.html");
					return true;
				}

				Database db = new Database();

				User user = db.AuthenticateUser(userName, pass);
			
				if (user != null && user.IsAuthorized)
				{
					sess.User = user;
					RedirectTo("/index2.html");
					return true;
				}
			}
			else
				LoggerQueue.Put("Dispatcher: Detected form token mismatch.");

			return false;
		}

		private void RedirectTo(string to)
		{
			response.Status = 302;
			response.Headers.Add("Location", Constants.CONTROL_URL + to);

			byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
			request.Stream.Write(headers, 0, headers.Length);
		}

		private Session GetSession()
		{
			string sessionId = "";

			if (request.Headers.ContainsKey("Cookie"))
			{
				String cookie = request.Headers["Cookie"];
				sessionId = Cookie.GetSessionIdFromString(cookie);
				LoggerQueue.Put("Dispatcher: Got existing cookie " + sessionId);

			}

			Session sess = Session.Initialize(sessionId);
				
			if (!this.response.Headers.ContainsKey("Set-Cookie"))
			{
				this.response.Headers.Add("Set-Cookie", "SessId=" + sess.Id + "; Expires=" + sess.Expires.ToString("R") + "; secure");
				LoggerQueue.Put("Dispatcher: Sent new cookie");
			}

			return sess;
		}

		private void DispatchConfig()
		{
			// Empty
		}

		private int CheckStatus(bool parameterized, Dictionary<string, string> parameters)
		{
			string requestPath = request.Path;
			int index = requestPath.IndexOf("?");

			if (index > 0)
			{
				requestPath = requestPath.Substring(0, index);
			}

			string targetPath = request.WebRoot + requestPath;
			string absolutePath = Path.GetFullPath(targetPath);
			absolutePath = absolutePath.Replace("/", @"\");

			if (absolutePath.StartsWith(request.WebRoot))
			{
				if (!Path.HasExtension(requestPath))
				{
					if (!absolutePath.EndsWith("/")) absolutePath += "/";
					

					if (Directory.Exists(absolutePath))
					{
						if (File.Exists(absolutePath + Configuration.Entries.defaultPage))
						{
							absolutePath += Configuration.Entries.defaultPage;
						}
						else
						{
							if (Configuration.Entries.directoryBrowsing == "true")
							{
								String data = DirectoryListing.Generate(request.WebRoot, absolutePath);

								response.SetHeader("Content-Type", MimeTypes.List["html"]);

								byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
								byte[] body = Encoding.UTF8.GetBytes(data);

								if (request.Stream.CanWrite)
								{
									request.Stream.Write(headers, 0, headers.Length);
									request.Stream.Write(body, 0, body.Length);

									return 200;
								}
							}
							else
							{
								return 403;
							}
						}
					
					}

				}

				if (File.Exists(absolutePath))
				{
					String fileExtension = Path.GetExtension(absolutePath).Substring(1);

					if (MimeTypes.List.ContainsKey(fileExtension))
					{
						response.SetHeader("Content-Type", MimeTypes.List[fileExtension]);
					}

					if (parameterized && WriteFileParameterized(absolutePath, parameters))
					{
						return 200;
					}
					else if (WriteFile(absolutePath))
					{
						return 200;
					}
				}

				return 404;
			}

			return 500;
		}

		private void WriteError(int status)
		{

			
			response.SetHeader("Content-Type", MimeTypes.List["html"]);
			response.Status = status;

			string file = Application.StartupPath + @"\" + ERROR_PATH + @"\" + status + ".html";

			if (File.Exists(file))
			{
				LoggerQueue.Put("Dispatcher: Showing error page " + status);
				WriteFile(file);
			}
			else
			{
				LoggerQueue.Put("Dispatcher: Cannot find error page for status " + status);
				WriteFile(Application.StartupPath + @"\" + ERROR_PATH + @"\404_errorpage.html");
			}
				
		}
		
		private bool WriteFileParameterized(string file, Dictionary<string, string> data)
		{
			try
			{
				var buffer = new byte[128];
				var stream = request.Stream;

				using (StreamReader fs = new StreamReader(File.OpenRead(file)))
				{
					byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
					stream.Write(headers, 0, headers.Length);
					
					while (request.Stream.CanWrite && fs.Peek() > 0)
					{
						string line = fs.ReadLine();

						foreach (KeyValuePair<string, string> entry in data)
						{
							line = line.Replace("{{ " + entry.Key + " }}", entry.Value);
						}

						request.Stream.Write(Encoding.UTF8.GetBytes(line), 0, line.Length);
					}

					stream.Flush();
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private bool WriteFile(string file)
		{
			try
			{
				var buffer = new byte[128];
				var stream = request.Stream;

				using (FileStream fs = File.OpenRead(file))
				{
					byte[] headers = Encoding.UTF8.GetBytes(response.ToString());
					stream.Write(headers, 0, headers.Length);
					int hasRead;

					while (request.Stream.CanWrite && (hasRead = fs.Read(buffer, 0, buffer.Length)) > 0)
					{
						request.Stream.Write(buffer, 0, hasRead);
					}

					stream.Flush();
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}