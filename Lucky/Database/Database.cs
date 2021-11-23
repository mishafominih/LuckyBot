using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
	public static class  Database
	{
        private static string Host = "localhost";
        private static string User = "postgres";
        private static string DBname = "Lucky";
        private static string Password = "1234qwe++wer";
        private static string Port = "5432";
        private static NpgsqlConnection connect = null;
        public static void Connect()
        {
            string connString =
                   String.Format(
                       "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                       Host,
                       User,
                       DBname,
                       Port,
                       Password);
            connect = new NpgsqlConnection(connString);
            connect.Open();
            Console.WriteLine("Подключение к бд установлено");
        }

        public static void Set(string query)
        {
            try
            {
                using (var command = new NpgsqlCommand(query, connect))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("При добавлении данных в бд произошла ошибка:");
                Console.WriteLine(ex.Message);
            }
        }

        public static List<T> Get<T>(string query) where T : IDataType, new()
        {
            var resultList = new List<T>();
            try
            {
                var command = new NpgsqlCommand(query, connect);
                var res = command.ExecuteReader();
                while (res.Read())
                {
                    var fields = new List<object>();
                    for (int i = 0; i < res.FieldCount; i++)
                    {
                        fields.Add(res.GetValue(i));
                    }
                    var data = new T();
                    data.Fill(fields);
                    resultList.Add(data);
                }
                res.Close();
                return resultList;
            }
            catch(Exception ex)
            {
                Connect();
                Console.WriteLine($"При работе с бд возникла ошибка: {ex.Message}");
                return resultList;
            }
        }

        public static void Close()
        { 
            connect.Close();
            Console.WriteLine("Соединение закрыто");
        }
    }
}
