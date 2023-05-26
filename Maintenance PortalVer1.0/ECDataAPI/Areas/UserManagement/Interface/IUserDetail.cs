


#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   IUserDetail.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for BAL nad DAL classes.
*/
#endregion



using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.UserManagement.Interface
{
   public interface IUserDetail
    {


        //  List<UserDetail> GetAllUsers();

        UserActivationModel RegisterUser(UserModel Obj);
        bool CheckUserNameAvailability(string encryptedID);

        UserActivationModel AccountActivation(UserActivationModel Obj);

        string SendEmail(EmailModel model);

        string AccountActivationByLink(string Id);

        string ForgotPasswordByLink(ChangePasswordModel obj);
        
        UserActivationModel ForgotPassword(UserActivationModel Obj);

     
        LoginResponseModel UserLogin(LoginViewModel Obj);
    }
}
