using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	public class User
	{
		public string UserName
		{
			get;
			set;
		}

		public bool IsAdmin
		{
			get;
			set;
		}

		public bool IsAuthorized
		{
			get;
			set;
		}

		public User()
		{
			this.IsAuthorized = false;
		}
	}
}
