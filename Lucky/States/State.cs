using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky {
	public class State
	{
		protected string Message;

		public string Key { get; private set; }

		protected List<State> list_states = new List<State>();

		public readonly Func<MetaInfo, string> Action;

		public State(string key, Func<MetaInfo, string> action)
		{
			Key = key;
			this.Action = action;
		}

		public virtual State GetNextState(string key)
		{
			if (list_states.Count == 1) return list_states[0];
			foreach (var state in list_states)
			{
				if (state.Key == key) return state;
			}
			return this;
		}

		public virtual void AddStates(State[] states)
		{
			list_states.AddRange(states);
		}

		public virtual List<string> GetKeys()
		{
			return list_states
				.Select(state => state.Key)
				.Where(key => key != null && key != "")
				.ToList();
		}
	} 
}