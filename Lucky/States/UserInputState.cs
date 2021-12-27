using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky
{
    public class UserInputState : State
    {
        public UserInputState(string key, Func<MetaInfo, string> action) : base(key, action)
        {

        }

        public override State GetNextState(string key)
        {
            return base.GetNextState(key);
        }
    }
}
