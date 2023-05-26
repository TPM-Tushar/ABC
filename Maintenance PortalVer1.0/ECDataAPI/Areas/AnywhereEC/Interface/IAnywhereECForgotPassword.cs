using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.AnywhereEC.Interface
{
    interface IAnywhereECForgotPassword
    {
       ChangePasswordNewModel SaveNewPasswordView(int userid);
       ChangePasswordResponseModel SaveNewPassword(ChangePasswordNewModel model);
    }
}