using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky
{
    public class User : IDataType
	{
		public int Id;
		public Machine machine;

		public User(int id, State currentState = null)
		{
			Id = id;
			machine = new Machine(currentState);
		}

		public User()
		{

		}

		public void Fill(List<object> data)
        {
			int.TryParse(data[0].ToString(), out Id);
			machine = new Machine(Machine.GetState(data[1].ToString()));
		}

		public static bool operator ==(User user1, User user2)
		{
			if (user1 is null && user2 is null) return true;
			if (user1 is null || user2 is null) return false;
			return user1.Id == user2.Id;
		}

		public static bool operator !=(User user1, User user2)
		{
			return !(user1.Id == user2.Id);
		}

		public override bool Equals(object obj)
		{
			if (obj is User user)
			{
				return user == this;
			}
			return false;
		}
    }
}
