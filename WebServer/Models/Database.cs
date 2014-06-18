using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebServer.Models
{
	public class Database
	{
		private const string USERNAME = "sehac";
		private const string PASSWORD = "sehac2014";
		private const string SERVER = "frankwammes.nl";
		private const string DATABASE = "sehac";
		private const string USERS_DB = "users";
		private MySqlConnection conn;

		public Database()
		{
			this.Initialize();

		}

		private void Initialize()
		{
			this.conn = new MySqlConnection("SERVER=" + SERVER + ";" + "DATABASE=" + DATABASE + ";" + "UID=" + USERNAME + ";" + "PASSWORD=" + PASSWORD + ";");
			this.OpenConnection();
		}

		private bool OpenConnection()
		{
			if (!this.conn.Ping())
			{
				LoggerQueue.Put("Connecting to database...");
				try
				{
					this.conn.Open();
					LoggerQueue.Put("Database connected.");
					return true;
				}
				catch (MySqlException ex)
				{
					LoggerQueue.Put(ex.Message);

					switch (ex.Number)
					{
						case 0:
							LoggerQueue.Put("Cannot connect to database: connection error");
							MessageBox.Show("Cannot connect to server.");
							break;

						case 1045:
							LoggerQueue.Put("Cannot connect to database: invalid username/password");
							MessageBox.Show("Invalid username/password.");
							break;
					}

					return false;
				}
			}

			return true;
		}

		public string GetUserSalt(string user)
		{
			MySqlCommand q = new MySqlCommand("SELECT `salt` FROM users WHERE `username` = @username", this.conn);
			q.Prepare();

			q.Parameters.AddWithValue("@username", user);

			using (IDataReader reader = q.ExecuteReader())
			{
				if (reader.Read())
				{
					LoggerQueue.Put("Database: Got salt for user: " + user + ". Salt: " + reader[0]);
					return reader[0].ToString();
				}

			}
			LoggerQueue.Put("Database: No salt found for user: " + user);
			return null;
		}

		public User AuthenticateUser(string userName, string password)
		{
			string salt = GetUserSalt(userName);
			
			if (salt == null)
				return null;

			string hashedPassword = Hash(password, salt);
			bool isAdmin = false;
			User user = new User();


			MySqlCommand q = new MySqlCommand("SELECT `username`, `admin`, `password` FROM users WHERE LOWER(`username`) = LOWER(@username)", this.conn);
			
			q.Prepare();

			q.Parameters.AddWithValue("@username", userName);

			using (MySqlDataReader reader = q.ExecuteReader())
			{
				if (reader.Read())
				{
					string passwordHashFromDb = reader.GetString(reader.GetOrdinal("password"));

					if (passwordHashFromDb.Equals(hashedPassword))
					{
						isAdmin = (reader.GetInt32(reader.GetOrdinal("admin")) == 1);
						user.IsAuthorized = true;
						user.UserName = reader.GetString(reader.GetOrdinal("username"));
						user.IsAdmin = isAdmin;
						
						LoggerQueue.Put("Database: Authenticated " + userName + " successfully as " + (isAdmin == true ? "Admin" : "User"));
					}
					else
						LoggerQueue.Put("Database: Wrong password for user: " + userName);
				}

			}
			
			return user;
		}

		public bool RegisterUser(string user, string password)
		{
			string salt = Guid.NewGuid().ToString("N");

			try
			{
				MySqlCommand q = new MySqlCommand("INSERT INTO users (`username`, `salt`, `password`, `admin`) VALUES (@username, @salt, @password, 0)", this.conn);

				q.Prepare();

				q.Parameters.AddWithValue("@username", user);
				q.Parameters.AddWithValue("@salt", salt);
				q.Parameters.AddWithValue("@password", Hash(password, salt));

				if (q.ExecuteNonQuery() == 1)
					LoggerQueue.Put("Database: Registered user: \"" + user + "\" successfully");
				else
					LoggerQueue.Put("Database: Cannot register user: \"" + user + "\".");
			}			
			catch (Exception e)
			{
				LoggerQueue.Put("Database: Error occured while registering user: " + user);
				LoggerQueue.Put(e.Message);
			}
			return false;
		}

		private string Hash(string password, string salt)
		{
			HashAlgorithm algorithm = new SHA1Managed();

			var plainTextBytes = Encoding.ASCII.GetBytes(password);
			var saltBytes = Encoding.ASCII.GetBytes(salt);

			var plainTextWithSaltBytes = AppendByteArray(plainTextBytes, saltBytes);
			var saltedSHA1Bytes = algorithm.ComputeHash(plainTextWithSaltBytes);
			var saltedSHA1WithAppendedSaltBytes = AppendByteArray(saltedSHA1Bytes, saltBytes);

			return Convert.ToBase64String(saltedSHA1WithAppendedSaltBytes);
		}

		private static byte[] AppendByteArray(byte[] byteArray1, byte[] byteArray2)
		{
			var byteArrayResult =
					new byte[byteArray1.Length + byteArray2.Length];

			for (var i = 0; i < byteArray1.Length; i++)
				byteArrayResult[i] = byteArray1[i];
			for (var i = 0; i < byteArray2.Length; i++)
				byteArrayResult[byteArray1.Length + i] = byteArray2[i];

			return byteArrayResult;
		}
	}
}

