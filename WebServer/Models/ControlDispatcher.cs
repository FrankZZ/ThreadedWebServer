using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class ControlDispatcher : Dispatcher
	{
		override public void Dispatch(Request request)
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

							if (sess.LastToken == request.Params["token"])
							{
								if (!sess.User.IsAdmin)
								{
									status = 403;
								}
								else
								{
									if (request.Params["directoryBrowsing"] != "true" && request.Params["directoryBrowsing"] != "false")
										throw new ArgumentException("Invalid");
									else
									{
										if (Configuration.Entries.controlPort != request.Params["controlPort"])
											Configuration.ShutdownRequested = true;

										if (Configuration.Entries.webPort != request.Params["webPort"])
											Configuration.ShutdownRequested = true;

										Configuration.Entries.controlPort = request.Params["controlPort"];
										Configuration.Entries.defaultPage = request.Params["defaultPage"];
										Configuration.Entries.directoryBrowsing = request.Params["directoryBrowsing"];
										Configuration.Entries.webPort = request.Params["webPort"];
										Configuration.Entries.webRoot = request.Params["webRoot"];

										Configuration.Write();
									}
									
								} 
								
							}
							else
								LoggerQueue.Put("Dispatcher: Detected form token mismatch.");
							
							if (status == -1)
							{
								Dictionary<String, String> dict = Configuration.Entries.ToDictionary();
								string token = Guid.NewGuid().ToString("N");
								sess.LastToken = token;
								dict.Add("token", token);

								status = CheckStatus(true, dict);
							}
							
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
		}

		private bool DispatchLogin()
		{
			string userName = request.Params["username"];
			string pass = request.Params["password"];
			string token = request.Params["token"];
			Session sess = null;
			sess = GetSession();

			if (sess.LastToken == request.Params["token"])
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

			if (sess.Id != sessionId && !this.response.Headers.ContainsKey("Set-Cookie"))
			{
				this.response.Headers.Add("Set-Cookie", "SessId=" + sess.Id + "; Expires=" + sess.Expires.ToString("R") + "; secure");
				LoggerQueue.Put("Dispatcher: Cookie invalid. Sent new cookie");
			} else LoggerQueue.Put("Dispatcher: Cookie validated");

			return sess;
		}
	}
}
