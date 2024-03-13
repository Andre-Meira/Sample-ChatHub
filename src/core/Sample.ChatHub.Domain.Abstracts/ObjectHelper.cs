using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ChatHub.Domain.Abstracts;

public static class ObjectHelper
{
    public static bool IsAny(this object o, params Type[] types)
    {
        return types.Contains(o.GetType());
    }
}
