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
		public int Count;

		public User()
		{

		}

		public void Fill(List<object> data)
        {
			int.TryParse(data[0].ToString(), out Id);
			machine = new Machine(Machine.GetState(data[1].ToString()));
			int.TryParse(data[2].ToString(), out Count);
		}

		public string GetAnswer(MetaInfo metaInfo)
        {
			return machine.Update(metaInfo);
        }

		public static User Get(long? id)
		{
			var query = $"SELECT * FROM \"Users\" WHERE id='{id}'";
			var users = Database.Get<User>(query);
			return users.FirstOrDefault();
		}

		public void Update()
		{
			var updateQuery = $"UPDATE public.\"Users\" " +
							  $"SET \"currentState\"='{machine.currentState.Key}'" +
							  $", \"count\"='{Count}' " +
							  $"WHERE id='{Id}';";
			Database.Set(updateQuery);
		}
	}
}
