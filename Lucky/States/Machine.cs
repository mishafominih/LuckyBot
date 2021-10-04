using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Machine
{
	private static State start;

	private State currentState;
	public static void Initialize()
	{
		var start = new State("Начать заного", "Привет!");
		var balance = new ActionState("Баланс", "Ваш баланс равен: ", (prev) => "0р");
		var fillCount = new State("Пополнить баланс", "Извините, в данное время это невозможно(");
		var events = new State("События", "В данное время никаких событий нет");
		var calendar = new State("Расписание", "В данное время расписание недоступно");
		var allEvents = new State("Все события", "В данное время никаких событий нет");
		start.AddStates(new State[] { balance, fillCount, events });
		events.AddStates(new State[] { calendar, allEvents });
		Machine.start = start;
	}

	public Machine(State start = null)
	{
		currentState = start != null ? start : Machine.start;
	}

	public bool SetNextState(string answer)
	{
		var newState = currentState.GetNextState(answer);
		if (currentState == newState) return false;
		currentState = newState;
		return true;
	}

	public string GetMessage()
	{
		return currentState.GetMessage();
	}

	public List<string> GetKeys()
	{
		return currentState.GetKeys();
	}
}