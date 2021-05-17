using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public class Administrator : RegisteredUser
    {
        public Administrator(string password) : base(password) {    }
    }
}
