using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.UserManagement
{
    public  class LoginResponseModel
    {
        public long UserID { get; set; }
        public short DefaultRoleId { get; set; }
        public short LevelId { get; set; }
        public short OfficeId { get; set; }
        public string ResponseMessage { get; set; }
        public string Password { get; set; }
        public long ResourceID { get; set; }

        public string UserFullName { get; set; }

        //#region On 9-4-2019 by Shubham Bhagat for password change on first login
        //public bool IsFirstLogin { get; set; }
        //#endregion
    }
}
