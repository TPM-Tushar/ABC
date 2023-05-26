using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Common
{
    public static class PreventMultipleLogin
    {
        static PreventMultipleLogin()
        {
            IsUserLoggedIn = new Dictionary<long, string>();
            IsLoggedSession = new Dictionary<long, string>();
        }

        public static Dictionary<Int64, string> IsUserLoggedIn { get; set; }
        public static Dictionary<long, string> IsLoggedSession { get; set; }
    }
}
