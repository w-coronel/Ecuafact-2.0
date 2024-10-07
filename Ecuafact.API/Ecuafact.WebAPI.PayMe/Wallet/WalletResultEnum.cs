using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.PayMe
{
    public enum WalletResultEnum
    {
        None = -1,
        Authorized = 0,
        Denied = 1,
        Rejected = 5,
        Duplicated = 6
    } 
}
