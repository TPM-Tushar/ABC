#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IntegrationCallExceptionsDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.IntegrationCallExceptions;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;
//using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class IntegrationCallExceptionsDAL : IIntegrationCallExceptions, IDisposable
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        #region Method

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public IntegrationCallExceptionsModel GetOfficeList(String OfficeType)
        {
            IntegrationCallExceptionsModel model = new IntegrationCallExceptionsModel();
            model.OfficeTypeList = new List<SelectListItem>();
            SelectListItem selectListItem = new SelectListItem();
            selectListItem.Text = "All";
            selectListItem.Value = "0";
            model.OfficeTypeList.Add(selectListItem);

            if (OfficeType.ToLower().Equals("sro"))
            {
                // COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 24-05-2019
                // var SROMasterList = dbContext.SROMaster.ToList();
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
                            selectListOBJ.Value = item.SROCode.ToString();
                            model.OfficeTypeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            if (OfficeType.ToLower().Equals("dro"))
            {
                // COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 24-05-2019
                //var DistrictMasterList = dbContext.DistrictMaster.ToList();
                List<DistrictMaster> DistrictMasterList = dbContext.DistrictMaster.ToList();
                DistrictMasterList = DistrictMasterList.OrderBy(x => x.DistrictNameE).ToList();
                if (DistrictMasterList != null)
                {
                    if (DistrictMasterList.Count() > 0)
                    {
                        foreach (var item in DistrictMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            selectListOBJ.Text = item.DistrictNameE + " (" + item.DistrictCode.ToString() + ")";
                            selectListOBJ.Value = item.DistrictCode.ToString();
                            model.OfficeTypeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// GetExceptionsDetails
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <param name="OfficeTypeID"></param>
        /// <returns></returns>
        public IEnumerable<IntegrationCallExceptionsModel> GetExceptionsDetails(String OfficeType, String OfficeTypeID)
        {
            List<IntegrationCallExceptionsModel> exceptionsDetailsList = new List<IntegrationCallExceptionsModel>();
            try
            {
                int OfficeTypeIDINT = Convert.ToInt32(OfficeTypeID);
                List<AMS_REG_KhajaneExceptionLog> AMS_REG_KhajaneExceptionLogList = null;
                List<AMS_REG_KhajaneExceptionLog> OrderByLogDateList = null;

                // CASE 1: TO GET BOTH DRO AND SRO ALL OFFICES EXCEPTION LIST 
                if (String.IsNullOrEmpty(OfficeType))
                {
                    AMS_REG_KhajaneExceptionLogList = dbContext.AMS_REG_KhajaneExceptionLog.ToList();
                    if (AMS_REG_KhajaneExceptionLogList != null)
                    {
                        if (AMS_REG_KhajaneExceptionLogList.Count() > 0)
                        {
                            OrderByLogDateList = AMS_REG_KhajaneExceptionLogList.OrderByDescending(x => x.LogDate).ToList();
                        }
                    }
                }

                // CASE 2: FOR DRO 
                else if (OfficeType.ToLower().Equals("dro"))
                {
                    // CASE 2.1: TO GET ALL DRO OFFICES EXCEPTION LIST 
                    if (OfficeTypeIDINT == 0)
                    {
                        AMS_REG_KhajaneExceptionLogList = dbContext.AMS_REG_KhajaneExceptionLog.Where(x => x.IsDRO == true).ToList();
                        if (AMS_REG_KhajaneExceptionLogList != null)
                        {
                            if (AMS_REG_KhajaneExceptionLogList.Count() > 0)
                            {
                                OrderByLogDateList = AMS_REG_KhajaneExceptionLogList.OrderByDescending(x => x.LogDate).ToList();
                            }
                        }
                    }
                    // CASE 2.2: TO GET DRO OFFICE EXCEPTION LIST OF A PARTICULAR DRO OFFICE
                    else
                    {
                        AMS_REG_KhajaneExceptionLogList = dbContext.AMS_REG_KhajaneExceptionLog.Where(x => x.IsDRO == true && x.DRO == OfficeTypeIDINT).ToList();
                        if (AMS_REG_KhajaneExceptionLogList != null)
                        {
                            if (AMS_REG_KhajaneExceptionLogList.Count() > 0)
                            {
                                OrderByLogDateList = AMS_REG_KhajaneExceptionLogList.OrderByDescending(x => x.LogDate).ToList();
                            }
                        }
                    }

                }
                // CASE 3: FOR SRO 
                else if (OfficeType.ToLower().Equals("sro"))
                {
                    // CASE 3.1: TO GET ALL SRO OFFICES EXCEPTION LIST 
                    if (OfficeTypeIDINT == 0)
                    {
                        AMS_REG_KhajaneExceptionLogList = dbContext.AMS_REG_KhajaneExceptionLog.Where(x => (x.IsDRO == false || x.IsDRO == null)).ToList();
                        if (AMS_REG_KhajaneExceptionLogList != null)
                        {
                            if (AMS_REG_KhajaneExceptionLogList.Count() > 0)
                            {
                                OrderByLogDateList = AMS_REG_KhajaneExceptionLogList.OrderByDescending(x => x.LogDate).ToList();
                            }
                        }
                    }
                    // CASE 3.2: TO GET SRO OFFICE EXCEPTION LIST OF A PARTICULAR SRO OFFICE
                    else
                    {
                        AMS_REG_KhajaneExceptionLogList = dbContext.AMS_REG_KhajaneExceptionLog.Where(x => (x.IsDRO == false || x.IsDRO == null) && x.SROCode == OfficeTypeIDINT).ToList();
                        if (AMS_REG_KhajaneExceptionLogList != null)
                        {
                            if (AMS_REG_KhajaneExceptionLogList.Count() > 0)
                            {
                                OrderByLogDateList = AMS_REG_KhajaneExceptionLogList.OrderByDescending(x => x.LogDate).ToList();
                            }
                        }
                    }
                }

                if (OrderByLogDateList != null)
                {
                    if (OrderByLogDateList.Count() > 0)
                    {
                        foreach (var item in OrderByLogDateList)
                        {
                            IntegrationCallExceptionsModel model = new IntegrationCallExceptionsModel();
                            model.Logid = item.Logid;
                            model.SROCode = (item.SROCode == null) ? "-" : item.SROCode.ToString();
                            model.ExceptionType = (String.IsNullOrEmpty(item.ExceptionType)) ? "-" : item.ExceptionType;
                            model.InnerExceptionMsg = (String.IsNullOrEmpty(item.InnerExceptionMsg)) ? "-" : item.InnerExceptionMsg;
                            model.ExceptionMsg = (String.IsNullOrEmpty(item.ExceptionMsg)) ? "-" : item.ExceptionMsg;
                            model.ExceptionStackTrace = (String.IsNullOrEmpty(item.ExceptionStackTrace)) ? "-" : item.ExceptionStackTrace;
                            model.ExceptionMethodName = (String.IsNullOrEmpty(item.ExceptionMethodName)) ? "-" : item.ExceptionMethodName;
                            model.LogDate = (item.LogDate == null) ? "-" : item.LogDate.ToString();
                            model.IsDRO = (item.IsDRO == null) ? "-" : item.IsDRO.ToString();
                            model.DRO = (item.DRO == null) ? "-" : item.DRO.ToString();
                            exceptionsDetailsList.Add(model);
                        }
                    }
                }
                return exceptionsDetailsList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}