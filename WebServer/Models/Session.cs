using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class Session
	{
		private static Dictionary<string, Session> sessions = new Dictionary<string, Session>();

		public static Session Initialize(string SessionId)
		{
			if (sessions.ContainsKey(SessionId))
			{
				Session session = sessions[SessionId];
				
				if (session.EnforceExpiry())
				{
					sessions.Remove(SessionId);
					LoggerQueue.Put("Session: Session expired: " + SessionId);
				}
				else
					return session;
			}
			else
			{
				LoggerQueue.Put("Session: Session \"" + SessionId + "\" unknown.");
			}

			Session sess = new Session();
			return sess;
		}

		public static Session Initialize()
		{
			return new Session();
		}

		public static void AddSession(Session sess)
		{
			Session.sessions.Add(sess.Id, sess);
		}

		public string Id;
		public User User;
		public string LastToken;

		public DateTime Expires
		{
			get;
			private set;
		}

		public Session()
			: this(Guid.NewGuid().ToString("N"))
		{
			
		}

		public Session(string SessionId)
		{
			Id = SessionId;
			Expires = DateTime.Now.AddMinutes(10);
			Session.sessions.Add(SessionId, this);
		}

		private bool EnforceExpiry()
		{
			return (this.Expires < DateTime.Now);
		}

		
	}
}
