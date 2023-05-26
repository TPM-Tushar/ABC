using CustomModels.Models.MISReports.DiskUtilization;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DiskUtilizationDAL : IDiskUtilization
    {
        KaveriEntities dbContext = null;

        public DiskUtilizationREQModel DiskUtilizationView(int OfficeID)
        {
            DiskUtilizationREQModel model = new DiskUtilizationREQModel();
            model.ApplicationList = new List<ApplicationReqModel>();
            ApplicationReqModel inner_model = null;
            try
            {
                dbContext = new KaveriEntities();
                var vAppList = dbContext.STORAGE_SERVERMASTER.Where(x => x.IS_CONSIDER == true).ToList();
                if (vAppList != null)
                {
                    if (vAppList.Count > 0)
                    {
                        foreach (var item in vAppList)
                        {
                            inner_model = new ApplicationReqModel();
                            inner_model.ServerID = item.SERVERID;
                            inner_model.ServerType = item.SERVERTYPE;
                            inner_model.IPAddress = item.IPADDRESS;
                            inner_model.Description = item.DESCRIPTION;
                            inner_model.Action = @"<span class='fa fa-eye' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= LoadServerDriveDetails('" + (item.SERVERID) + "')></span>";
                            model.ApplicationList.Add(inner_model);
                        }
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return model;
        }

        public DiskUtilizationWrapper DiskUtilizationDetails(DiskUtilizationREQModel model)
        {
            try
            {
                DiskUtilizationWrapper resModel = new DiskUtilizationWrapper();
                resModel.DiskUtilizationDetailList = new List<DiskUtilizationDetail>();
                if (model.ServerId == 1)   //.97
                {
                    #region 10.10.20.97
                    EcDataService.ECDataService service = new EcDataService.ECDataService();
                    ECDataAPI.EcDataService.ErrorDetailsResponseModel responseModel = service.GetErrorFileDetails("root");

                    if (responseModel.isError)
                    {
                        resModel.isError = responseModel.isError;
                        resModel.sErrorMessage = responseModel.sErrorMsg;
                        resModel.DiskUtilizationDetailList = new List<DiskUtilizationDetail>();
                        return resModel;
                    }
                    else
                    {
                        resModel.TotalRecords = responseModel.DriveInfoModelList.Count();
                        DiskUtilizationDetail objModel;
                        int cnt = 1;
                        foreach (var item in responseModel.DriveInfoModelList)
                        {
                            objModel = new DiskUtilizationDetail();
                            objModel.SerialNumber = cnt;
                            objModel.DriveName = item.DriveName;
                            objModel.FreeSpace = item.FreeSpace;
                            objModel.TotalSpace = item.TotalSpace;
                            string TotalSpaceStr = objModel.TotalSpace.Substring(0, objModel.TotalSpace.Length - 2).Trim();
                            string FreeSpaceStr = objModel.FreeSpace.Substring(0, objModel.FreeSpace.Length - 2).Trim();
                            decimal usedspace = Convert.ToDecimal(TotalSpaceStr) - Convert.ToDecimal(FreeSpaceStr);
                            objModel.UsedSpace = usedspace.ToString();
                            objModel.FileSystem = item.FileSystem;
                            objModel.FreeSpacePercentage = item.FreeSpacePercentage;
                            objModel.DriveType = item.DriveType;
                            resModel.DiskUtilizationDetailList.Add(objModel);
                            cnt++;
                        }

                        if (resModel.DiskUtilizationDetailList.Count > 0)
                        {
                            if (LogDriveInfo(model.ServerId, resModel))
                                return resModel;
                            else
                                throw new Exception("Error Occured While Saving/ Updating Drive Info for Selected Server.");
                        }
                    }
                    return resModel;
                    #endregion
                }
                else if (model.ServerId == 2)   //.132
                {
                    #region 10.10.20.132
                    ErrorDetailsDAL objDAL = new ErrorDetailsDAL();
                    CustomModels.Models.Remittance.ErrorDetails.ErrorDetailsResponseModel responseModel = objDAL.GetErrorFileDetails("root");


                    if (responseModel.isError)
                    {
                        resModel.isError = responseModel.isError;
                        resModel.sErrorMessage = responseModel.sErrorMsg;
                        resModel.DiskUtilizationDetailList = new List<DiskUtilizationDetail>();
                        return resModel;
                    }
                    else
                    {
                        resModel.TotalRecords = responseModel.DriveInfoModelList.Count();
                        DiskUtilizationDetail objModel;
                        int cnt = 1;
                        foreach (var item in responseModel.DriveInfoModelList)
                        {
                            objModel = new DiskUtilizationDetail();
                            objModel.SerialNumber = cnt;
                            objModel.DriveName = item.DriveName;
                            objModel.FreeSpace = item.FreeSpace;
                            objModel.TotalSpace = item.TotalSpace;
                            string TotalSpaceStr = objModel.TotalSpace.Substring(0, objModel.TotalSpace.Length - 2).Trim();
                            string FreeSpaceStr = objModel.FreeSpace.Substring(0, objModel.FreeSpace.Length - 2).Trim();
                            decimal usedspace = Convert.ToDecimal(TotalSpaceStr) - Convert.ToDecimal(FreeSpaceStr);
                            objModel.UsedSpace = usedspace.ToString();
                            objModel.FileSystem = item.FileSystem;
                            objModel.FreeSpacePercentage = item.FreeSpacePercentage;
                            objModel.DriveType = item.DriveType;
                            resModel.DiskUtilizationDetailList.Add(objModel); cnt++;
                        }

                        if (resModel.DiskUtilizationDetailList.Count > 0)
                        {
                            if (LogDriveInfo(model.ServerId, resModel))
                                return resModel;
                            else
                                throw new Exception("Error Occured While Saving/ Updating Drive Info for Selected Server.");
                        }
                    }
                    return resModel;
                    #endregion
                }
                else if (model.ServerId == 3)  //.117
                {
                    #region 10.10.28.117
                    PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();
                    ECDataAPI.PreRegApplicationDetailsService.ErrorDetailsResponseModel responseModel = service.GetErrorFileDetails("root");

                    if (responseModel.isError)
                    {
                        resModel.isError = responseModel.isError;
                        resModel.sErrorMessage = responseModel.sErrorMsg;
                        resModel.DiskUtilizationDetailList = new List<DiskUtilizationDetail>();
                        return resModel;
                    }
                    else
                    {
                        resModel.TotalRecords = responseModel.DriveInfoModelList.Count();
                        DiskUtilizationDetail objModel;
                        int cnt = 1;
                        foreach (var item in responseModel.DriveInfoModelList)
                        {
                            objModel = new DiskUtilizationDetail();
                            objModel.SerialNumber = cnt;
                            objModel.DriveName = item.DriveName;
                            objModel.FreeSpace = item.FreeSpace;
                            objModel.TotalSpace = item.TotalSpace;
                            string TotalSpaceStr = objModel.TotalSpace.Substring(0, objModel.TotalSpace.Length - 2).Trim();
                            string FreeSpaceStr = objModel.FreeSpace.Substring(0, objModel.FreeSpace.Length - 2).Trim();
                            decimal usedspace = Convert.ToDecimal(TotalSpaceStr) - Convert.ToDecimal(FreeSpaceStr);
                            objModel.UsedSpace = usedspace.ToString();
                            objModel.FileSystem = item.FileSystem;
                            objModel.FreeSpacePercentage = item.FreeSpacePercentage;
                            objModel.DriveType = item.DriveType;
                            resModel.DiskUtilizationDetailList.Add(objModel); cnt++;
                        }

                        if (resModel.DiskUtilizationDetailList.Count > 0)
                        {
                            if (LogDriveInfo(model.ServerId, resModel))
                                return resModel;
                            else
                                throw new Exception("Error Occured While Saving/ Updating Drive Info for Selected Server.");
                        }
                    }
                    return resModel;
                    #endregion
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool LogDriveInfo(int ServerId, DiskUtilizationWrapper resModel)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    using (TransactionScope tScope = new TransactionScope())
                    {
                        DateTime _captureDate = DateTime.Now.Date;

                        STORAGE_SERVERDETAILS dbDriveInfo = new STORAGE_SERVERDETAILS();

                        foreach (var item in resModel.DiskUtilizationDetailList)
                        {
                            dbDriveInfo = dbContext.STORAGE_SERVERDETAILS.Where(x => x.SERVERID == ServerId && x.CAPTURE_DATE == _captureDate && x.DRIVENAME == item.DriveName).FirstOrDefault();

                            if (dbDriveInfo != null)
                            {
                                dbDriveInfo.SERVERID = ServerId;
                                dbDriveInfo.DRIVENAME = item.DriveName;
                                dbDriveInfo.DRIVETYPE = item.DriveType;
                                dbDriveInfo.FILE_SYSTEM = item.FileSystem;
                                dbDriveInfo.TOTAL_SPACE = item.TotalSpace;
                                dbDriveInfo.FREE_SPACE = item.FreeSpace;
                                dbDriveInfo.CAPTURE_DATE = _captureDate;
                                dbContext.Entry(dbDriveInfo).State = EntityState.Modified;
                            }
                            else
                            {
                                dbDriveInfo = new STORAGE_SERVERDETAILS();

                                dbDriveInfo.SERVERID = ServerId;
                                dbDriveInfo.DRIVENAME = item.DriveName;
                                dbDriveInfo.DRIVETYPE = item.DriveType;
                                dbDriveInfo.FILE_SYSTEM = item.FileSystem;
                                dbDriveInfo.TOTAL_SPACE = item.TotalSpace;
                                dbDriveInfo.FREE_SPACE = item.FreeSpace;
                                dbDriveInfo.CAPTURE_DATE = _captureDate;
                                dbContext.STORAGE_SERVERDETAILS.Add(dbDriveInfo);
                            }
                        }
                        dbContext.SaveChanges();
                        tScope.Complete();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}