using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SinglState : State
{
	public SinglState(string key, string message, State[] states = null) :
		base(key, message, states)
	{ }

	public override State GetNextState(string key)
	{
		return list_states[0];
	}
}