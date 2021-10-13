﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
	public static class DataManager
	{
		private static List<User> users = new List<User>();
		private static List<Event> events = new List<Event>();

		public static User GetUser(int id)
		{
			return users.Where(x => x.Id == id).FirstOrDefault();
		}

		public static void AddUser(User user)
		{
			users.Add(user);
		}

		public static List<Event> GetEvents()
		{
			return events;
		}
	}
}
