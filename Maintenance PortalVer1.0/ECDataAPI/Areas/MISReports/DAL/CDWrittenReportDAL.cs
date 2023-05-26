using CustomModels.Models.CDWrittenReport;
using CustomModels.Security;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class CDWrittenReportDAL : ICDWrittenReport
    {

        KaveriEntities dbContext = null;
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;


        /// <summary>
        /// Returns CD Written Report View Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public CDWrittenReportViewModel CDWrittenReportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                CDWrittenReportViewModel model = new CDWrittenReportViewModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();
                model.CDNumberList = new List<SelectListItem>();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                //SelectListItem select = new SelectListItem();

                //select.Text = "Select";
                //select.Value = "0";
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                //string kaveriCode = Kaveri1Code.ToString();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, ofcDetailsObj.kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DistrictList.Add(droNameItem);
                    //model.SROfficeList.Add(sroNameItem);

                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                    model.CDNumberList = objCommon.CDNumberList(ofcDetailsObj.Kaveri1Code, "Select");

                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    //string DroCode_string = Convert.ToString(ofcDetailsObj.Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DistrictList.Add(droNameItem);
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                    model.CDNumberList.Add(new SelectListItem() { Text = "Select", Value = "0" });

                }
                else
                {
                    model.SROfficeList.Add(new SelectListItem() { Text = "Select", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("Select");
                    model.CDNumberList.Add(new SelectListItem() { Text = "Select", Value = "0" });
                }
                return model;

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

        //public CDWrittenReportViewModel CDWrittenReportView(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        CDWrittenReportViewModel model = new CDWrittenReportViewModel();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        SelectListItem select = new SelectListItem();

        //        select.Text = "Select";
        //        select.Value = "0";
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DistrictList = new List<SelectListItem>();
        //        model.CDNumberList = new List<SelectListItem>();
        //        string kaveriCode = Kaveri1Code.ToString();
        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);

        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList.Add(sroNameItem);

        //            model.CDNumberList = objCommon.CDNumberList(Kaveri1Code,"Select");

        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
        //            model.CDNumberList.Add(select);

        //        }
        //        else
        //        {

        //        model.SROfficeList.Add(select);
        //        model.DistrictList = objCommon.GetDROfficesList("Select");
        //        model.CDNumberList.Add(select); 
        //        }
        //        return model;

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
        //}


        public CDWrittenReportResModel LoadCDWrittenReportDataTable(CDWrittenReportViewModel model)
        {
            CDWrittenReportResModel CDWrittenResModel = new CDWrittenReportResModel();
            CDWrittenReportRecordModel CDWrittenRec = null;
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<USP_RPT_CDWrittenReport_Result> CDWrittenListWhenSearch = new List<USP_RPT_CDWrittenReport_Result>();
                List<USP_RPT_CDWrittenReport_Result> CDWrittenList = new List<USP_RPT_CDWrittenReport_Result>();
                CDWrittenListWhenSearch = dbContext.USP_RPT_CDWrittenReport(model.DistrictID,model.SROfficeID,model.CDNumber).ToList();
                int counter = (model.startLen + 1); //To start Serial Number 
                CDWrittenResModel.ICDWrittenReportRecordList = new List<CDWrittenReportRecordModel>();
                List<CDWrittenReportRecordModel> CDWrittenRecList = new List<CDWrittenReportRecordModel>();

                CDWrittenResModel.TotalCount = CDWrittenListWhenSearch.Count();
                if (string.IsNullOrEmpty(model.SearchValue))
                {
                    if (model.IsExcel)
                    {
                        CDWrittenList = CDWrittenListWhenSearch;
                    }
                    else
                    {
                        CDWrittenList = CDWrittenListWhenSearch.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                else
                {
                    CDWrittenList = CDWrittenListWhenSearch;
                }

                foreach (var item in CDWrittenList)
                {
                    CDWrittenRec = new CDWrittenReportRecordModel();
                    CDWrittenRec.SerialNo = counter++;
                    CDWrittenRec.DocType = string.IsNullOrEmpty(item.Doctype) ? string.Empty : item.Doctype;
                    CDWrittenRec.RegistrationNumber = string.IsNullOrEmpty(item.FRN) ? string.Empty : Convert.ToString(item.FRN);
                    CDWrittenRec.LocalServerStoragePath = (string.IsNullOrEmpty(item.LocalServerPath) && string.IsNullOrEmpty(item.FileName)) ? string.Empty : item.LocalServerPath + item.FileName;
                    CDWrittenRec.FileUploadedToCentralServer = string.IsNullOrEmpty(item.FileUploadedToCentralServer) ? string.Empty: item.FileUploadedToCentralServer;
                    CDWrittenRec.SizeOfFile = (item.FileSize==null) ? 0 : Convert.ToDouble(item.FileSize/10240);
                    CDWrittenRec.DateOfScan = (item.ScanDate==null) ? string.Empty : Convert.ToDateTime(item.ScanDate).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture); 
                    CDWrittenRec.DateOfUpload = (item.UploadDateTime ==null) ? string.Empty : Convert.ToDateTime(item.UploadDateTime).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
                    CDWrittenRec.DocDeliveryDate = (item.DateOfReturn==null) ? string.Empty : Convert.ToDateTime(item.DateOfReturn).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    CDWrittenRec.DateOfRegistration = (item.Stamp5DateTime == null) ? String.Empty : Convert.ToDateTime(item.Stamp5DateTime).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    CDWrittenRec.OfficeName = String.IsNullOrEmpty(item.SRONameE) ? string.Empty : item.SRONameE;
                    if (item.RootDirectory != null && item.UploadPath != null && item.ScannedFileName != null)
                        CDWrittenRec.SDCStoragePath = item.RootDirectory + item.UploadPath + item.ScannedFileName;
                    else
                        CDWrittenRec.SDCStoragePath = string.Empty;

                    CDWrittenRecList.Add(CDWrittenRec);
                }
                CDWrittenResModel.ICDWrittenReportRecordList = CDWrittenRecList;

                //For Searching
                if (!string.IsNullOrEmpty(model.SearchValue))
                {
                    CDWrittenResModel.ICDWrittenReportRecordList = CDWrittenResModel.ICDWrittenReportRecordList.Where(m => m.RegistrationNumber.ToString().ToLower().Contains(model.SearchValue.ToLower())
                    ).ToList();
                    CDWrittenResModel.FilteredRecCount = CDWrittenResModel.ICDWrittenReportRecordList.Count();
                    CDWrittenResModel.ICDWrittenReportRecordList = CDWrittenResModel.ICDWrittenReportRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                }
                return CDWrittenResModel;
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


    }
}