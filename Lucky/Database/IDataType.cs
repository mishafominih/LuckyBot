using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucky
{
    public interface IDataType
    {
        void Fill(List<object> data);
    }
}
