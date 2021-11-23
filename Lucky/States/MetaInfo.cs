using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky
{
    public class MetaInfo
    {
        public User User;
        public string Message;
        public MetaInfo(User user, string message)
        {
            User = user;
            Message = message;
        }
    }
}
