using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.AnywhereEC.DAL;
using ECDataAPI.Areas.AnywhereEC.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.AnywhereEC.BAL
{
    public class AnywhereECForgotPasswordBAL : IAnywhereECForgotPassword
    {
        IAnywhereECForgotPassword dalObj = new AnywhereECForgotPasswordDAL();

        public ChangePasswordNewModel SaveNewPasswordView(int userid)
        {
            try
            {
                return dalObj.SaveNewPasswordView(userid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ChangePasswordResponseModel SaveNewPassword(ChangePasswordNewModel model)
        {
            try
            {
                return dalObj.SaveNewPassword(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}