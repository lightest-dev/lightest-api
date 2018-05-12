using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    [Flags]
    public enum AccessRights: int
    {
        Owner = 1,
        Read = 2,
        Write = 4,
        AssignAdmin = 8
    }
}
