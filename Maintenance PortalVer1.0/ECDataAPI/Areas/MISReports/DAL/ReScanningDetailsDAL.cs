using CustomModels.Models.MISReports.ReScanningDetails;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ReScanningDetailsDAL : IReScanningDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        public ReScanningDetailsResModel GetReScanningTableData(ReScanningDetailsViewModel model)
        {

            ReScanningDetailsResModel RescanningDetailsResModel = new ReScanningDetailsResModel();
            ReScanningDetailsModel ReportsDetails = null;
            //List<ReScanningDetailsModel> ReportsDetailsList = new List<ReScanningDetailsModel>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                var ReceiptDetailsList = dbContext.USP_RPT_RescanningDetails(model.SROfficeID, model.ModuleID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                int counter = 1;

                RescanningDetailsResModel.ReScanningDetailsList = new List<ReScanningDetailsModel>();
                foreach (var item in ReceiptDetailsList)
                {
                    ReportsDetails = new ReScanningDetailsModel();
                    ReportsDetails.SerialNo = counter++;
                    ReportsDetails.DocumentNo = item.DocumentNumber == null ? " " : Convert.ToString(item.DocumentNumber);
                    ReportsDetails.FinalRegistrationNumber = string.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                    ReportsDetails.SROOffice = string.IsNullOrEmpty(item.SROOffice) ? string.Empty : item.SROOffice;
                    ReportsDetails.RescanEnableDateTime = string.IsNullOrEmpty(item.RescanEnableDateTime.ToString()) ? "NA" : item.RescanEnableDateTime.ToString();
                    ReportsDetails.IsFileUploaded = item.IsFileUploaded;
                    ReportsDetails.MarriageCaseNo = item.MarriageCaseNo;

                    if (model.ModuleID == 2)
                        ReportsDetails.FinalRegistrationNumber = ReportsDetails.MarriageCaseNo; 

                    RescanningDetailsResModel.ReScanningDetailsList.Add(ReportsDetails);

                }
                return RescanningDetailsResModel;
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

        public int GetReScanningTotalCount(ReScanningDetailsViewModel model)
        {

            KaveriEntities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new KaveriEntities();
                RecordCnt = dbContext.USP_RPT_RescanningDetails(model.SROfficeID, model.ModuleID, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();
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

            return RecordCnt;
        }

        public ReScanningDetailsViewModel ReScanningDetails(int OfficeID)
        {
            ReScanningDetailsViewModel ViewModel = new ReScanningDetailsViewModel();

            try
            {
                dbContext = new KaveriEntities();
                string FirstRecord = "Select";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                ViewModel.SROfficeList = new List<SelectListItem>();
                ViewModel.DROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);
                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList.Add(sroNameItem);
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, FirstRecord);
                }
                else
                {
                    SelectListItem ItemAll = new SelectListItem();
                    ItemAll.Text = "All";
                    ItemAll.Value = "0";
                    ViewModel.SROfficeList.Add(ItemAll);
                    ViewModel.DROfficeList = objCommon.GetDROfficesList("All");
                }
                ViewModel.ModuleNameList = GetModuleNameList();
                //ViewModel.FeeTypeList = new List<SelectListItem>();
                //SelectListItem Select = new SelectListItem();
                //Select = objCommon.GetDefaultSelectListItem("Select","1");
                //ViewModel.FeeTypeList.Add(Select);

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
            return ViewModel;
        }


        /// <summary>
        /// Returns Module Name List
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        List<SelectListItem> GetModuleNameList()
        {
            List<SelectListItem> ModuleNameList = new List<SelectListItem>();
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Select", (Convert.ToInt32(0)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Document Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.DocumentReg)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Marriage Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.MarrriageReg)).ToString()));
            return ModuleNameList;
        }
    }


}