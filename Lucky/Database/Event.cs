using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
    public class Event : IDataType
    {
        public int Id;
        public DateTime Start;
        public int Cost;
        public string Name;

        public Event() { }

        public void Fill(List<object> data)
        {
            int.TryParse(data[0].ToString(), out Id);
            DateTime.TryParse(data[1].ToString(), out Start);
            int.TryParse(data[2].ToString(), out Cost);
            Name = data[3].ToString();
        }

        public override string ToString()
        {
            return $"Событие \"{Name}\". Розыгрыш в {Start} по екб. Цена за вход: {Cost}р.";
        }
    }
}
