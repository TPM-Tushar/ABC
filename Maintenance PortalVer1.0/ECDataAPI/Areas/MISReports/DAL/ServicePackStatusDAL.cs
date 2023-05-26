#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ServicePackStatusDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ServicePackStatus;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
// using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ServicePackStatusDAL : IServicePackStatus
    {
        KaveriEntities dbContext = null;

        /// <summary>
        /// Service Pack Status View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public ServicePackStatusModel ServicePackStatusView(int OfficeID)
        {
            ServicePackStatusModel resModel = new ServicePackStatusModel();
            resModel.DROfficeList = new List<SelectListItem>();
            resModel.SROfficeList = new List<SelectListItem>();

            List<SROMaster> SROMasterList = new List<SROMaster>();
            List<DistrictMaster> DROfficeList = new List<DistrictMaster>();
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                // For SR
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DistrictCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DistrictNameE = dbContext.DistrictMaster.Where(x => x.DistrictCode == DistrictCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    SelectListItem sroNameItem = new SelectListItem();
                    sroNameItem.Text = SroName;
                    sroNameItem.Value = Kaveri1Code.ToString();
                    resModel.SROfficeList.Add(sroNameItem);

                    SelectListItem droNameItem = new SelectListItem();
                    droNameItem.Text = DistrictNameE;
                    droNameItem.Value = DistrictCode.ToString();
                    resModel.DROfficeList.Add(droNameItem);
                }
                // For DR
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DistrictNameE = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    SelectListItem select = new SelectListItem();
                    select.Text = DistrictNameE;
                    select.Value = Kaveri1Code.ToString();
                    resModel.DROfficeList.Add(select);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
                }
                // For others
                else
                {
                    resModel.DROfficeList.Add(GetDefaultSelectListItem("All", "0"));
                    DROfficeList = dbContext.DistrictMaster.ToList();
                    if (DROfficeList != null)
                    {
                        DROfficeList = DROfficeList.OrderBy(x => x.DistrictNameE).ToList();
                        foreach (var item in DROfficeList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.DistrictNameE;
                            select.Value = item.DistrictCode.ToString();
                            resModel.DROfficeList.Add(select);
                        }
                    }

                    resModel.SROfficeList.Add(GetDefaultSelectListItem("All", "0"));
                    //SROMasterList = dbContext.SROMaster.ToList();
                    //if (SROMasterList != null)
                    //{
                    //    SROMasterList = SROMasterList.OrderBy(x => x.ShortNameE).ToList();
                    //    foreach (var item in SROMasterList)
                    //    {
                    //        SelectListItem select = new SelectListItem();
                    //        select.Text = item.SRONameE;
                    //        select.Value = item.SROCode.ToString();
                    //        resModel.SROfficeList.Add(select);
                    //    }
                    //}

                }

                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 07-09-2020

                //resModel.ServicePackList = new List<SelectListItem>();
                //resModel.ServicePackList.Add(GetDefaultSelectListItem("All", "0"));
                //List<ServicePackMaster> ServicePackListList = dbContext.ServicePackMaster.ToList();
                //if (ServicePackListList != null)
                //{
                //    foreach (var item in ServicePackListList)
                //    {
                //        SelectListItem select = new SelectListItem();
                //        select.Text = item.Name + " " + item.Version;
                //        select.Value = item.PackID.ToString();
                //        resModel.ServicePackList.Add(select);
                //    }
                //}

                //resModel.StatusList = new List<SelectListItem>();
                //resModel.StatusList.Add(new SelectListItem
                //{
                //    Text = "All",
                //    Value = "0"
                //});
                //resModel.StatusList.Add(new SelectListItem
                //{
                //    Text = "Installed",
                //    Value = "1"
                //});
                //resModel.StatusList.Add(new SelectListItem
                //{
                //    Text = "Not Installed",
                //    Value = "2"
                //});

                resModel.SoftwareReleaseTypeList = new List<SelectListItem>();
                resModel.SoftwareReleaseTypeList.Add(new SelectListItem() { Value = "0", Text = "All" });
                resModel.SoftwareReleaseTypeList.Add(new SelectListItem() { Value = "1", Text = "EXE/DLL" });
                resModel.SoftwareReleaseTypeList.Add(new SelectListItem() { Value = "2", Text = "Service Pack" });

                resModel.ServicePackChangeTypeList = new List<SelectListItem>();
                resModel.ServicePackChangeTypeList.Add(new SelectListItem() { Value = "0", Text = "All" });
                resModel.ServicePackChangeTypeList.Add(new SelectListItem() { Value = "1", Text = "Bugs" });
                resModel.ServicePackChangeTypeList.Add(new SelectListItem() { Value = "2", Text = "Enhancements" });

                resModel.ReleasedStatusList = new List<SelectListItem>();
                resModel.ReleasedStatusList.Add(new SelectListItem() { Value = "0", Text = "All" });
                resModel.ReleasedStatusList.Add(new SelectListItem() { Value = "1", Text = "Released" });
                resModel.ReleasedStatusList.Add(new SelectListItem() { Value = "2", Text = "In Progress" });

                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 07-09-2020

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
            return resModel;
        }

        /// <summary>
        /// Get Default Select List Item
        /// </summary>
        /// <param name="sTextValue"></param>
        /// <param name="sOptionValue"></param>
        /// <returns></returns>
        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        /// <summary>
        /// Service Pack Status Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ServicePackStatusDetails> ServicePackStatusDetails(ServicePackStatusModel model)
        {
            ServicePackStatusDetails ReportsDetails = null;
            List<ServicePackStatusDetails> ReportsDetailsList = new List<ServicePackStatusDetails>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                //var TransactionList = dbContext.USP_RPT_SERVICEPACK_DETAILS(model.SROfficeID, model.ServicePackID, model.StatusID).Skip(model.startLen).Take(model.totalNum).ToList();
                var TransactionList = dbContext.USP_RPT_SERVICEPACK_STATUS_DETAILS(model.IsSRDRFlag, model.DROfficeID, model.SROfficeID, model.SoftwareReleaseTypeID, model.ServicePackChangeTypeID, model.ReleasedStatusID).Skip(model.startLen).Take(model.totalNum).ToList();
                if (TransactionList != null)
                {
                    //int counter = 1;
                    int counter = model.startLen;
                    foreach (var item in TransactionList)
                    {
                        ReportsDetails = new ServicePackStatusDetails();

                        ReportsDetails.SerialNo = ++counter;
                        ReportsDetails.DistrictName = string.IsNullOrEmpty(item.DR_OFFICE_NAME) ? "null" : item.DR_OFFICE_NAME;
                        ReportsDetails.SROName = string.IsNullOrEmpty(item.SR_OFFICE_NAME) ? "null" : item.SR_OFFICE_NAME;
                        ReportsDetails.SoftwareReleaseType = string.IsNullOrEmpty(item.SOFTWARE_RELEASE_TYPE) ? "null" : item.SOFTWARE_RELEASE_TYPE;
                        ReportsDetails.ReleaseMode = (bool)item.RELEASE_MODE ? "Test Release" : "Final Release";
                        ReportsDetails.Major = item.Major == null ? String.Empty : item.Major.ToString();
                        ReportsDetails.Minor = item.Minor == null ? String.Empty : item.Minor.ToString();
                        ReportsDetails.Description = string.IsNullOrEmpty(item.Description) ? String.Empty : item.Description;
                        ReportsDetails.InstallationProcedure = string.IsNullOrEmpty(item.InstallationProcedure) ? String.Empty : item.InstallationProcedure;
                        ReportsDetails.ChangeType = item.ChangeTypeDesc == null ? String.Empty : item.ChangeTypeDesc;
                        ReportsDetails.Status = item.STATUS == true ? "Released" : "In Process";
                        ReportsDetails.ReleaseInstruction = string.IsNullOrEmpty(item.RELEASE_INSTRUCTION) ? String.Empty : item.RELEASE_INSTRUCTION;
                        ReportsDetails.AddedDate = item.ServicePackAddedDateTime == null ? String.Empty : ((DateTime)item.ServicePackAddedDateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 4-12-2020
                        // TO REMOVE TIME PART BECAUSE TIME IS NOT CAPTURING FOR ServicePackReleaseDateTime
                        //ReportsDetails.ReleaseDate = item.ServicePackReleaseDateTime == null ? String.Empty : ((DateTime)item.ServicePackReleaseDateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        ReportsDetails.ReleaseDate = item.ServicePackReleaseDateTime == null ? String.Empty : ((DateTime)item.ServicePackReleaseDateTime).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 4-12-2020

                        ReportsDetailsList.Add(ReportsDetails);
                    }
                }
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
            // dummy data to be removed
            //if (model.IsSRDRFlag=="D")
            //{
            //    ReportsDetails = new ServicePackStatusDetails();

            //    ReportsDetails.SerialNo = 1;
            //    ReportsDetails.DistrictName = "Dummy Data";
            //    ReportsDetails.SROName = "";
            //    ReportsDetails.SoftwareReleaseType = "Dummy release type";
            //    ReportsDetails.ReleaseMode = "Dummy Test Release";
            //    ReportsDetails.Major = "1";
            //    ReportsDetails.Minor = "2";
            //    ReportsDetails.Description = "Dummy dewschnj gfbtft ft";
            //    ReportsDetails.InstallationProcedure = "Dummy install procedure";
            //    ReportsDetails.ChangeType = "Dummy Change type";
            //    ReportsDetails.Status = "Dummy Released" ;
            //    ReportsDetails.ReleaseInstruction = "Dummy Release intr";
            //    ReportsDetails.AddedDate = "Dummy date";
            //    ReportsDetails.ReleaseDate = "Dummy date";

            //    ReportsDetailsList.Add(ReportsDetails);
            //}

            return ReportsDetailsList;


        }

        /// <summary>
        /// Service Pack Status Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ServicePackStatusTotalCount(ServicePackStatusModel model)
        {
            int Count = 0;
            try
            {
                dbContext = new KaveriEntities();
                var TransactionList = dbContext.USP_RPT_SERVICEPACK_STATUS_DETAILS(model.IsSRDRFlag, model.DROfficeID, model.SROfficeID, model.SoftwareReleaseTypeID, model.ServicePackChangeTypeID, model.ReleasedStatusID).ToList();
                Count = TransactionList.Count;
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
            return Count;
        }
    }
}