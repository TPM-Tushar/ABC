#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DailyReceiptDetailsDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion


using CustomModels.Models.MISReports.DailyReceiptDetails;
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
    public class DailyReceiptDetailsDAL : IDailyReceiptDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        /// <summary>
        /// Returns DailyReceiptDetailsViewModel Required to show DailyReceiptDetails
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public DailyReceiptDetailsViewModel DailyReceiptDetails(int OfficeID)
        {
            DailyReceiptDetailsViewModel ViewModel = new DailyReceiptDetailsViewModel();

            try
            {
                dbContext = new KaveriEntities();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string FirstRecord = "All";
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();

                ViewModel.SROfficeList = new List<SelectListItem>();
                ViewModel.DROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                ViewModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ViewModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //ViewModel.DROfficeList.Add(droNameItem);
                    //ViewModel.SROfficeList.Add(sroNameItem);

                    ViewModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    ViewModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = mas_OfficeMaster.Kaveri1Code.ToString() });
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //ViewModel.DROfficeList.Add(droNameItem);
                    //ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, "All");

                    ViewModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(mas_OfficeMaster.Kaveri1Code) });
                    ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, "All");

                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem ItemAll = new SelectListItem();
                    //ItemAll.Text = "All";
                    //ItemAll.Value = "0";

                    ViewModel.SROfficeList.Add(new SelectListItem { Text = "All", Value = "0" });
                    ViewModel.DROfficeList = objCommon.GetDROfficesList("All");
                }
                ViewModel.FeeTypeList = objCommon.GetFeeType("All");
                ViewModel.ModuleNameList = GetModuleNameList();
                ViewModel.ModuleID = Convert.ToInt32(ApiCommonEnum.ModuleNames.DocumentReg);
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

        //public DailyReceiptDetailsViewModel DailyReceiptDetails(int OfficeID)
        //{
        //    DailyReceiptDetailsViewModel ViewModel = new DailyReceiptDetailsViewModel();

        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        string FirstRecord = "All";

        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        ViewModel.SROfficeList = new List<SelectListItem>();
        //        ViewModel.DROfficeList = new List<SelectListItem>();
        //        var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
        //                                select new
        //                                {
        //                                    OfficeMaster.LevelID,
        //                                    OfficeMaster.Kaveri1Code
        //                                }).FirstOrDefault();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        ViewModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        ViewModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //        if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);
        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            ViewModel.DROfficeList.Add(droNameItem);
        //            ViewModel.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            ViewModel.DROfficeList.Add(droNameItem);
        //            ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, FirstRecord);
        //        }
        //        else
        //        {
        //            SelectListItem ItemAll = new SelectListItem();
        //            ItemAll.Text = "All";
        //            ItemAll.Value = "0";
        //            ViewModel.SROfficeList.Add(ItemAll);
        //            ViewModel.DROfficeList = objCommon.GetDROfficesList("All");
        //        }
        //        ViewModel.FeeTypeList = objCommon.GetFeeType("All");
        //        ViewModel.ModuleNameList = GetModuleNameList();
        //        ViewModel.ModuleID= Convert.ToInt32(ApiCommonEnum.ModuleNames.DocumentReg);
        //        //ViewModel.FeeTypeList = new List<SelectListItem>();
        //        //SelectListItem Select = new SelectListItem();
        //        //Select = objCommon.GetDefaultSelectListItem("Select","1");
        //        //ViewModel.FeeTypeList.Add(Select);

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }
        //    return ViewModel;

        //}

        /// <summary>
        /// Returns Module Name List
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        List<SelectListItem> GetModuleNameList()
        {
            List<SelectListItem> ModuleNameList = new List<SelectListItem>();

            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("All", (Convert.ToInt32(ApiCommonEnum.ModuleNames.All)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Document Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.DocumentReg)).ToString()));
            //ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Marriage Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.MarrriageReg)).ToString()));
            //ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Marriage Notice", (Convert.ToInt32(ApiCommonEnum.ModuleNames.MarriageNotice)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Stamp Duty", (Convert.ToInt32(ApiCommonEnum.ModuleNames.StampDuty)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Others", (Convert.ToInt32(ApiCommonEnum.ModuleNames.Others)).ToString()));

            return ModuleNameList;
        }

        /// <summary>
        /// Returns Total Count of DailyReceiptDetailsList
        /// </summary>
        /// <param name="DailyReceiptDetailsViewModel"></param>
        /// <returns></returns>
        public int GetDailyReceiptsTotalCount(DailyReceiptDetailsViewModel model)
        {

            KaveriEntities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new KaveriEntities();
                RecordCnt=dbContext.USP_RPT_DailyReceiptDetails(model.DROfficeID,model.SROfficeID, model.ModuleID, model.FeeTypeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();
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

        /// <summary>
        /// returns DailyReceiptDetailsResModel to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DailyReceiptDetailsResModel GetDailyReceiptTableData(DailyReceiptDetailsViewModel model)
        {

            DailyReceiptDetailsResModel DailyReceiptsResModel = new DailyReceiptDetailsResModel();
            DailyReceiptDetailsModel ReportsDetails = null;
            List<DailyReceiptDetailsModel> ReportsDetailsList = new List<DailyReceiptDetailsModel>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                var ReceiptDetailsList = dbContext.USP_RPT_DailyReceiptDetails(model.DROfficeID,model.SROfficeID, model.ModuleID, model.FeeTypeID,model.DateTime_FromDate,model.DateTime_ToDate).ToList();
                int counter = (model.startLen + 1); //To start Serial Number 
                Decimal TotalAmount = 0;
                Decimal TotalAmountPaid = 0;

                DailyReceiptsResModel.TotalRecords = ReceiptDetailsList.Count;
                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        ReceiptDetailsList = ReceiptDetailsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                DailyReceiptsResModel.DailyReceiptsDetailsList = new List<DailyReceiptDetailsModel>();
                foreach (var item in ReceiptDetailsList)
                {

                    ReportsDetails = new DailyReceiptDetailsModel();
                    ReportsDetails.SerialNo = counter++;
                    ReportsDetails.DocumentNo = item.DocumentNumber == null ? "" : Convert.ToString(item.DocumentNumber);
                    ReportsDetails.ArticleName = string.IsNullOrEmpty(item.ArticleNameE) ? string.Empty : item.ArticleNameE;
                    ReportsDetails.FRN = string.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                    ReportsDetails.ReceiptNo = item.ReceiptNumber == null ? 0 : Convert.ToInt32(item.ReceiptNumber);
                    ReportsDetails.DateOfReceipt = string.IsNullOrEmpty(item.DateOfReceipt) ? string.Empty : item.DateOfReceipt;
                    ReportsDetails.Description = string.IsNullOrEmpty(item.DescriptionE) ? string.Empty : item.DescriptionE;
                    ReportsDetails.AmountPaid = item.AmountPaid == null ? 0 :Convert.ToDecimal(item.AmountPaid);
                    ReportsDetails.MarriageCaseNumber = string.IsNullOrEmpty(item.MarriageCaseNo) ? string.Empty : item.MarriageCaseNo;
                    ReportsDetails.DescriptionEnglish = string.IsNullOrEmpty(item.DescriptionEnglish) ? string.Empty : item.DescriptionEnglish;
                    ReportsDetails.PresentDateTime = string.IsNullOrEmpty(item.PresentDateTime) ? string.Empty : item.PresentDateTime;
                    //ReportsDetails.DDChallanNo = item.DDChalNumber == null ? 0 : Convert.ToInt32(item.DDChalNumber);
					//Changed By ShivamB for view this column in the DailyRecieptDetails Grid Table on 07-09-2022
                    ReportsDetails.ChallanNo = string.IsNullOrEmpty(item.ChallanNo) ? string.Empty : item.ChallanNo;
					//Changed By ShivamB for view this column in the DailyRecieptDetails Grid Table on 07-09-2022
                    ReportsDetails.Amount = item.Amount == null ? 0 : Convert.ToDecimal(item.Amount);
                    ReportsDetails.StampType = item.StampType == null ? "" : item.StampType;
                    ReportsDetails.SroName = string.IsNullOrEmpty(item.SROName) ? " " : item.SROName;
                    DailyReceiptsResModel.DailyReceiptsDetailsList.Add(ReportsDetails);
                    TotalAmount = TotalAmount + ReportsDetails.Amount;
                    TotalAmountPaid = TotalAmountPaid + ReportsDetails.AmountPaid;
                }
                DailyReceiptsResModel.TotalAmount = TotalAmount;

                DailyReceiptDetailsModel ReportsDetailLastRec = new DailyReceiptDetailsModel();
                ReportsDetailLastRec.SerialNo = 0;
                ReportsDetailLastRec.DocumentNo = "";
                ReportsDetailLastRec.ArticleName = "";
                ReportsDetailLastRec.FRN = "";
                ReportsDetailLastRec.ReceiptNo = 0;
                ReportsDetailLastRec.DateOfReceipt = string.Empty;
                ReportsDetailLastRec.Description = string.Empty;
                ReportsDetailLastRec.AmountPaid = TotalAmountPaid;
                ReportsDetailLastRec.MarriageCaseNumber = string.Empty;
                ReportsDetailLastRec.DescriptionEnglish = string.Empty;
                ReportsDetailLastRec.PresentDateTime = string.Empty;
                //ReportsDetails.DDChallanNo = item.DDChalNumber == null ? 0 : Convert.ToInt32(item.DDChalNumber);
                ReportsDetailLastRec.ChallanNo = string.Empty;
                ReportsDetailLastRec.Amount = TotalAmount;
                ReportsDetailLastRec.StampType = string.Empty;
                ReportsDetailLastRec.SroName = string.Empty;
                DailyReceiptsResModel.DailyReceiptsDetailsList.Add(ReportsDetailLastRec);

                return DailyReceiptsResModel;


            }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            if (dbContext != null)
                dbContext.Dispose();
        }

        //try
        //{
        //    DailyReceiptDetailsResModel resModel = new DailyReceiptDetailsResModel();
        //    resModel.DailyReceiptsDetailsList = new List<DailyReceiptDetailsModel>();
        //    DailyReceiptDetailsModel item = null;
        //    for (int i = 0; i < 50; i++)
        //    {
        //        item= new DailyReceiptDetailsModel();

        //        item.DocumentNo = "677";
        //        item.ArticleName = "isjdhf";
        //        item.FRN = "s";
        //        item.ReceiptNo = "sajgfd";
        //        item.DateOfReceipt = "hkdsa";
        //        item.Description = "ffsg";
        //        item.Amount = 23;
        //        resModel.DailyReceiptsDetailsList.Add(item);
        //    }


        //    return resModel;




        //}
        //catch (Exception ex)
        //{
        //    throw;
        //}
        //finally
        //{
        //    if (dbContext != null)
        //        dbContext.Dispose();
        //}

        //return SaleDeedRevCollectionOuterModel;


    }


}
}