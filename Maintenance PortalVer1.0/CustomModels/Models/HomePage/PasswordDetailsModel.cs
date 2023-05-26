using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.HomePage
{
   public class PasswordDetailsModel
    {

        public long UserID { get; set; }
        public short RoleID { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsPasswordExpired { get; set; }

        #region On 9-4-2019 by Shubham Bhagat for password change on first login
        public bool IsFirstLogin { get; set; }
        #endregion
    }
}
