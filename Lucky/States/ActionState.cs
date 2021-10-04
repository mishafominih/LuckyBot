using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActionState : State
{
	private Func<object, string> function;

	public ActionState(string key, string message, Func<object, string> function,  State[] states = null) : 
		base(key, message, states)
	{
		
		this.function = function;
	}

	public override string GetMessage()
	{
		return base.GetMessage() + function == null ? "" : function(this);
	}
}