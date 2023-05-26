///*----------------------------------------------------------------------------------------
// * Project Id: 
// * Project Name: Kaveri Maintainance Portal
// * File Name: SROModificationDetailsDAL.cs
// * Author : Harshit Gupta
// * Creation Date :14/Jan/2019
// * Desc : Provides methods for database interaction
// * ECR No : 
// * ---------------------------------------------------------------------------------------*/
//#region References
//using CustomModels.Models.Reports.DiagnosticReports;
//using ECDataAPI.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//#endregion

//namespace ECDataAuditDetails.DAL
//{
//    public class SROModificationDetailsDAL 
//    {
//        ApiCommonFunctions objCommon = new ApiCommonFunctions();

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public SROModificationDetailsRequestModel GetSROModificationDetailsRequestModel()
//        {
//            SROModificationDetailsRequestModel model = new SROModificationDetailsRequestModel();
//            model.OfficeList = objCommon.getSROfficesList(true);
//            model.LogsProgramNameList = GetAuditLogModificationProgramNameList(false);
//            model.ProgramID = model.LogsProgramNameList.Select(X => Convert.ToInt32(X.Value)).ToArray();
//            return model;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        public List<SROModificationDetailsViewModel> GetModificationDetailsList(ECDataAuditDetailsRequestModel model)
//        {
//            List<SROModificationDetailsViewModel> List = new List<SROModificationDetailsViewModel>();
//            SROModificationDetailsViewModel obj = null;
//            AuditLogEntities dbContext = null;
//            try
//            {
//                dbContext = new AuditLogEntities();
//                List<USP_REG_MODIFICATION_INFO_Result> Result = dbContext.USP_REG_MODIFICATION_INFO(model.Datetime_FromDate, model.Datetime_ToDate, model.OfficeID, model.programs).ToList();
//                foreach (var item in Result)
//                {
//                    obj = new SROModificationDetailsViewModel();
//                    obj.EncryptedMasterID = item.IMasterID;
//                    obj.EncryptedDetailsID =item.IDetailID;
//                    obj.SroName = item.SRONAME;
//                    obj.DateOfEvent = item.DATEOFEVENT;
//                    obj.ModificationType = item.MODIFICATION_TYPE;
//                    obj.Loginname = item.LOGIN_NAME;
//                    obj.IPAddress = item.IPADRESS;
//                    obj.HostName = item.HOST_NAME;
//                    obj.ApplicationName = item.APPLICATION_NAME;
//                    obj.PhysicalAddress = objCommon.ConvertStringToMACAddress(item.Physicaladdress);
//                    List.Add(obj);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//            finally
//            {

//                if (dbContext != null)
//                    dbContext.Dispose();
//            }
//            return List;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="MasterId"></param>
//        /// <param name="DetailsID"></param>
//        /// <returns></returns>
//        public List<SROModificationDetailsViewModel> GetModificationTypeDetails(long MasterId, long DetailsID)
//        {
//            AuditLogEntities dbContext = new AuditLogEntities();
//            List<SROModificationDetailsViewModel> listSROModificationDetailsViewModel = new List<SROModificationDetailsViewModel>();
//            try
//            {
//                List<USP_REG_MODIFICATION_DETAILS_Result> Result = dbContext.USP_REG_MODIFICATION_DETAILS(MasterId, DetailsID).ToList();
//                SROModificationDetailsViewModel obj = null;
//                    foreach (var item in Result)
//                    {
//                        obj = new SROModificationDetailsViewModel();
//                        obj.SroName = item.SRONAME;
//                        obj.DateOfEvent = item.DATEOFEVENT;
//                        obj.ModificationType = item.MODIFICATION_TYPE;
//                        obj.Statement = item.STATEMENT;
//                        listSROModificationDetailsViewModel.Add(obj);
//                    }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            finally
//            {
//                if (dbContext != null)
//                    dbContext.Dispose();
//            }
//            return listSROModificationDetailsViewModel;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="isAllSelected"></param>
//        /// <returns></returns>
//        //public List<SelectListItem> GetAuditLogModificationProgramNameList(bool isAllSelected = false)
//        //{
//        //    List<SelectListItem> ModificationsProgramNameList = new List<SelectListItem>();
//        //    AuditLogEntities dbContext = null;
//        //    try
//        //    {
//        //        dbContext = new AuditLogEntities();
//        //        if (isAllSelected)
//        //            ModificationsProgramNameList.Insert(0, objCommon.GetDefaultSelectListItem("All", "0"));
//        //        List<USP_MODIFICATION_PROGRAM_NAMES_Result> result = dbContext.USP_MODIFICATION_PROGRAM_NAMES("0").ToList();
//        //        foreach (var item in result)
//        //        {

//        //            if (item != null)
//        //                ModificationsProgramNameList.Add(new SelectListItem { Value = item.ID.ToString(), Text = (string.IsNullOrEmpty(item.ProgramName) ? "-" : item.ProgramName.Trim()) });
//        //        }
//        //    }
//        //    catch (Exception)
//        //    {

//        //        throw;
//        //    }
//        //    finally
//        //    {

//        //        if (dbContext != null)
//        //            dbContext.Dispose();
//        //    }
//        //    return ModificationsProgramNameList;
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        //public List<SROModificationDetailsViewModel> GetModificationDetailsAllList(ECDataAuditDetailsRequestModel model)
//        //{
//        //    List<SROModificationDetailsViewModel> listSROModificationDetails = new List<SROModificationDetailsViewModel>();
//        //    SROModificationDetailsViewModel obj = null;
//        //    AuditLogEntities dbContext = null;
//        //    try
//        //    {
//        //        dbContext = new AuditLogEntities();
//        //        List<USP_REG_MODIFICATION_DETAILS_ALL_Result> Result = dbContext.USP_REG_MODIFICATION_DETAILS_ALL(model.Datetime_FromDate, model.Datetime_ToDate, model.OfficeID, model.programs).ToList();
//        //        foreach (var item in Result)
//        //        {
//        //            obj = new SROModificationDetailsViewModel();
//        //            obj.EncryptedMasterID = item.IMasterID;
//        //            obj.EncryptedDetailsID = item.IDetailID;
//        //            obj.SroName = item.SRONAME;
//        //            obj.DateOfEvent = item.DATEOFEVENT;
//        //            obj.ModificationType = item.MODIFICATION_TYPE;
//        //            obj.Loginname = item.LOGIN_NAME;
//        //            obj.IPAddress = item.IPADRESS;
//        //            obj.HostName = item.HOST_NAME;
//        //            obj.ApplicationName = item.APPLICATION_NAME;
//        //            obj.Statement = item.STATEMENT;
//        //            obj.PhysicalAddress = objCommon.ConvertStringToMACAddress(item.Physicaladdress);
//        //            listSROModificationDetails.Add(obj);
//        //        }

//        //    }
//        //    catch (Exception)
//        //    {

//        //        throw;
//        //    }
//        //    finally
//        //    {

//        //        if (dbContext != null)
//        //            dbContext.Dispose();
//        //    }
//        //    return listSROModificationDetails.ToList();
//        //}

//    }
//}