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
				meta.User.Count += 10;
				meta.User.Update();
				return "Баланс увеличен";
			});
			var events = new State("Все события", (meta) =>
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
			var subscription = new State("Записаться на событие", (meta) => "Введите название события");
			var trySubscription = new State("", (meta) =>
			{
				var query = $"SELECT * " +
							$"FROM \"Events\"" +
							$"WHERE name='{meta.Message}'";
				var events = Database.Get<Event>(query);
				if (events.Count == 0) return "Не удалось найти такое событие";
				if (meta.User.Count < events[0].Cost) return "Упс:( Не достаточно средств на вашем счете для записи на событие";
				query = $"INSERT INTO public.\"UsersEvents\"(\"user\", \"event\") VALUES ('{meta.User.Id}', '{events[0].Id}');";
				Database.Set(query);
				meta.User.Count -= events.Count;
				meta.User.Update();
				return "Вы успешно записались на событие";
			});
			var unsubscription = new State("Отписаться от события", (meta) => "Введите название события");
			var tryUnsubscription = new State("", (meta) =>
			{
				var query = $"SELECT * " +
							$"FROM \"Events\"" +
							$"WHERE name='{meta.Message}'";
				var events = Database.Get<Event>(query);
				if (events.Count == 0) return "Не удалось найти такое событие";
				query = $"DELETE FROM public.\"UsersEvents\" WHERE \"user\"='{meta.User.Id}' AND \"event\"='{events[0].Id}'; ";
				Database.Set(query);
				meta.User.Count += events.Count;
				meta.User.Update();
				return "Вы успешно отписались от события";
			});
			var myEvents = new State("Мои события", (meta) =>
			{
				var query = $"SELECT * " +
							$"FROM \"Events\"" +
							$"WHERE \"id\"=(" +
							$"	SELECT event" +
							$"	FROM \"UsersEvents\"" +
							$"	WHERE \"user\"='{meta.User.Id}' AND \"event\"=id)";
				var events = Database.Get<Event>(query);
				foreach(var e in events)
                {
					Bot.SendMessage(meta.User.Id, e.ToString());
				}
				return "На этом все)";
			});
			start.AddStates(new State[] { balance, fillCount, events });
			events.AddStates(new State[] { myEvents, subscription });
			subscription.AddStates(new State[] { trySubscription });
			trySubscription.AddStates(new State[] { start });
			states.AddRange(new State[] { balance, fillCount, events, myEvents });
			myEvents.AddStates(new State[] { unsubscription });
			unsubscription.AddStates(new State[] { tryUnsubscription });
			tryUnsubscription.AddStates(new State[] { start });
			foreach (var state in states)
			{
				state.AddStates(new State[] { start });
			}

			states.Add(start);
			states.Add(subscription);
			states.Add(trySubscription);
			states.Add(unsubscription);
			states.Add(tryUnsubscription);
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