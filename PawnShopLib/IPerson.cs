using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public interface IPerson
    {
        string GetFullName();
        DateTime BirthDay { get; }
    }
}
