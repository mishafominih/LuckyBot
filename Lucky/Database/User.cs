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


    }
}
