using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lucky
{
	public class Program
	{
		public static void CheckerEvents()
        {
			var minitesCheck = 3;//������ ��������!!!!
			while (true)
			{
				Console.WriteLine("�������� �������");
				var query = $"SELECT * FROM \"Events\"";
				var events = Database.Get<Event>(query);
				foreach(var e in events)
                {
					if(Math.Abs((e.Start - DateTime.Now).TotalMinutes) <= minitesCheck / 2 )
                    {
						query = $"SELECT * " +
								$"FROM \"Users\"" +
								$"WHERE \"id\" = (" +
								$"	SELECT \"user\" " +
								$"	FROM \"UsersEvents\"" +
								$"	WHERE \"event\"='{e.Id}' AND \"user\"=\"id\")";
						var users = Database.Get<User>(query);
						//��������� ������������
						if (users.Count == 0) continue;
						var rand = new Random();
						var lucky = rand.Next(0, users.Count);
						var luckyUser = users[lucky];
						var winCost = users.Count * e.Cost;
						luckyUser.Count += winCost;
						luckyUser.Update();
						Bot.SendMessage(luckyUser.Id, $"����������!!! �� ������� � ������� {e.Name}!!! ���� ������� �������� {winCost}�");
						//������ ���� �������� ������������� �� ��������� ��������
						foreach(var user in users)
                        {
							if(user.Count < e.Cost)//���� � ������������ ���� �����, ���������� ��� �� �������
                            {
								query = $"DELETE FROM public.\"UsersEvents\" WHERE \"user\"='{user.Id}' AND \"event\"='{e.Id}'; ";
								Database.Set(query);
							}
                            else
                            {
								user.Count -= e.Cost;
								user.Update();
                            }
							if(user != luckyUser)
                            {
								Bot.SendMessage(user.Id, $"����������� ������� {e.Name}. ���������� - {"https://vk.com/id" + luckyUser.Id}!");
							}
                        }
					}
                }
				Thread.Sleep(minitesCheck * 60 * 1000);
            }
        }

		public static void Main(string[] args)
		{
			Database.Connect();
			var thread = new Thread(CheckerEvents);
			thread.Start();
			Bot.Authorize();
			Database.Close();
			Console.ReadLine();
			//CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
