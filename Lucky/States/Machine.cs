using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
	public class Machine
	{
		private static List<State> states = new List<State>();
		private static State start;

		public State currentState { get; private set; }
		static Machine()
		{
			var start = new State("В начало", (meta) => "Привет!");
			var balance = new State("Баланс", (meta) => $"Ваш баланс равен: {meta.User.Count}");
			var fillCount = new State("Пополнить баланс", (meta) =>
			{
				var updateQuery = $"UPDATE public.\"Users\" SET \"count\"='{meta.User.Count + 10}' WHERE id='{meta.User.Id}';";
				Database.Set(updateQuery);
				return "Баланс увеличен";
			});
			var events = new State("События", (meta) =>
			{
				var query = $"SELECT * FROM \"Events\"";
				var events = Database.Get<Event>(query);
				if(events.Count == 0)
					return "На данный момент событий нет(";
				Bot.SendMessage(meta.User.Id, "Доступны следующие события:");
				foreach (var e in events){
					Bot.SendMessage(meta.User.Id, e.ToString());
				}
				return "На этом все)";
			});
			var calendar = new State("Расписание", (meta) => "В данное время расписание недоступно");
			var allEvents = new State("Мои события", (meta) => "В данное время никаких событий нет");
			start.AddStates(new State[] { balance, fillCount, events });
			events.AddStates(new State[] { calendar, allEvents });

			states.AddRange(new State[] { balance, fillCount, events, calendar, allEvents });

			foreach (var state in states)
			{
				state.AddStates(new State[] { start });
			}

			states.Add(start);
			Machine.start = start;
		}

		public Machine(State start = null)
		{
			currentState = start != null ? start : Machine.start;
		}

		public string Update(MetaInfo metaInfo)
        {
			currentState = currentState.GetNextState(metaInfo.Message);
			var answer = currentState.Action(metaInfo);
			return answer;
		}

		public List<string> GetKeys()
		{
			return currentState.GetKeys();
		}

		public static State GetState(string key)
		{
			foreach (var st in states)
			{
				if (st.Key == key) return st;
			}
			return null;
		}
	}
}