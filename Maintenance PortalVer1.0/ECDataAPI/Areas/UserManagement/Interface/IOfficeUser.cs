using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IOfficeUser
    {
        UserActivationModel RegisterUser(OfficeUserDetailsModel Obj);
        bool CheckUserNameAvailability(string encryptedID);
        UserActivationModel UpdateOfficeUser(OfficeUserDetailsModel model);
        //bool UpdateOfficeUser(OfficeUserDetailsModel model);
        bool DeleteOfficeUser(String EncryptedId);
        OfficeUserDetailsModel GetUserDetails(string EncryptedId);
        //Commented by mayank on 06-08-2021
        //UserGridWrapperModel LoadOfficeUserDetailsGridData();
        UserGridWrapperModel LoadOfficeUserDetailsGridData(int officeID, int LevelID);
        OfficeUserDetailsModel GetOfficeUserDetailsList();

        // Added by Shubham Bhagat on 5-1-2019
        //OfficeUserDetailsModel GetOfficeDetailsInfo(int officeDetailId);
        OfficeUserDetailsModel GetOfficeDetailsInfo(String officeDetailId);

        // Added by Shubham Bhagat on 8-4-2019
        OfficeUserDetailsModel GetRoleListByLevel(String LevelID, int roleID = 0, String roleEncryptId = null, int officeID = 0, String officeEncryptId = null);

    }
}
