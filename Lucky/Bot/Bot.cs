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
						var user = User.Get(id);
                        if (user == null)
                        {
							var insertQuery = $"INSERT INTO public.\"Users\"(id, \"currentState\") VALUES ('{id}', 'Начать заного');";
							Database.Set(insertQuery);
							user = User.Get(id);
						}
						var answer = user.GetAnswer(new MetaInfo(user, new_event.Message.Text));
						user.Update();
                        SendMessage(user.Id, answer, user.machine.GetKeys());
                    }
				}
			}
		}

		public static void SendMessage(int userId, string message, List<string> buttons = null)
		{
			if (message == null || message == "") return;
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
			var buttonsCount = 2;
			var counter = 0;
			foreach(var btn in buttons)
			{
				if(counter == buttonsCount)
                {
					keyboard.AddLine();
					counter = 0;
                }
				keyboard.AddButton(btn, btn);
				counter++;
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
