using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class State
{
	protected string Message;

	public string Key { get; private set; }

	protected List<State> list_states;

	public State(string key, string message, State[] states = null)
	{
		Message = message;
		Key = key;
		list_states = states == null ? new List<State>() : states.ToList();
	}

	public virtual State GetNextState(string key)
	{
		foreach (var state in list_states)
		{
			if (state.Key == key) return state;
		}
		return this;
	}

	public virtual string GetMessage()
	{
		return Message;
	}

	public virtual void AddStates(State[] states)
	{
		list_states.AddRange(states);
	}

	public virtual List<string> GetKeys()
	{
		return list_states
			.Select(state => state.Key)
			.ToList();
	}
}