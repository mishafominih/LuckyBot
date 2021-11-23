using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace Lucky
{
	public static class Bot
	{
		static VkApi bot = null;

		public static void Authorize()
		{
			bot = new VkApi();
			var token = Settings.GetToken();
			bot.Authorize(new ApiAuthParams { AccessToken = token
			});
			while (true) // Бесконечный цикл, получение обновлений
			{
				var s = bot.Groups.GetLongPollServer(205462182);
				var poll = bot.Groups.GetBotsLongPollHistory(
				   new BotsLongPollHistoryParams()
				   { Server = s.Server, Ts = s.Ts, Key = s.Key, Wait = 25 });
				if (poll?.Updates == null) continue; // Проверка на новые события
				foreach (var new_event in poll.Updates)
				{
					if (new_event.Type == GroupUpdateType.MessageNew)
					{
						var id = new_event.Message.FromId;
						var query = $"SELECT * FROM \"Users\" WHERE id='{id}'";
						var users = Database.Get<User>(query);
						var user = users.FirstOrDefault();
                        if (user == null)
                        {
							var insertQuery = $"INSERT INTO public.\"Users\"(id, \"currentState\") VALUES ('{id}', 'Начать заного');";
							Database.Set(insertQuery);
							user = Database.Get<User>(query)[0];
                        }
                        user.machine.SetNextState(new_event.Message.Text);
                        var updateQuery = $"UPDATE public.\"Users\" SET \"currentState\"='{user.machine.currentState.Key}' WHERE id='{id}';";
						Database.Set(updateQuery);
                        var answer = user.machine.GetMessage();
                        SendMessage(user.Id, answer, user.machine.GetKeys());
                    }
				}
			}
		}

		public static void SendMessage(int userId, string message, List<string> buttons = null)
		{
			var rand = new Random();
			var parameters = new MessagesSendParams();

			parameters.UserId = userId;
			parameters.RandomId = rand.Next();
			parameters.Message = message;
			parameters.Keyboard = getKeyboard(buttons);
			try
			{
				bot.Messages.Send(parameters);
			}
			catch(AccessTokenInvalidException)
			{
				Authorize();
				bot.Messages.Send(parameters);
			}
		}

		private static MessageKeyboard getKeyboard(List<string> buttons = null)
		{
			if (buttons == null || buttons.Count == 0) return new KeyboardBuilder().Build();
			var keyboard = new KeyboardBuilder();
			foreach(var btn in buttons)
			{
				keyboard.AddButton(btn, btn);
			}
			return keyboard.SetInline(false)
				.SetOneTime()
				.Build();

			//.AddButton("Подтвердить", "btnValue", KeyboardButtonColor.Primary)
			//.SetInline(false)
			//.SetOneTime()
			//.AddLine()
			//.AddButton("Отменить", "btnValue", KeyboardButtonColor.Primary)
			//.Build();
		}
	}
}
