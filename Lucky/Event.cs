using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
	public class Event
	{
		public string name;
		public DateTime Start;
		public DateTime End;
		public DateTime StartTimeSignUp;
		public DateTime EndTimeSingUp;
		public DateTime TimeLottery;
		public int minCash;
		public int maxCash;
		public List<User> users;

		public Event()
		{

		}

		public User DefineWinner()
		{
			var rand = new Random();
			var id = rand.Next(0, users.Count);
			return users[id];
		}
	}
}
