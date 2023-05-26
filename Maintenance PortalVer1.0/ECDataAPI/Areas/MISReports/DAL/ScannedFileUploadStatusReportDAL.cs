#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ScannedFileUploadStatusReportDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ScannedFileUploadStatusReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ScannedFileUploadStatusReportDAL : IScannedFileUploadStatusReport
    {
        KaveriEntities dbContext = null;

        public ScannedFileUploadStatusRptReqModel GetScannedFileUploadStatusDetails(int OfficeID)
        {
            try
            {
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string FirstRecord = "All";
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                ScannedFileUploadStatusRptReqModel model = new ScannedFileUploadStatusRptReqModel();

                model.SROfficeList = new List<SelectListItem>();
                model.DROfficeList = new List<SelectListItem>();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                //string kaveriCode = Kaveri1Code.ToString();


                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))//For SR Login
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //model.SROfficeList = new List<SelectListItem>();
                    //model.DROfficeList = new List<SelectListItem>();
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DROfficeList.Add(droNameItem);
                    //model.SROfficeList.Add(sroNameItem);
                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                    model.IsDrLogin = false;
                    model.IsSrLogin = true;
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))//For DR Login
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DROfficeList.Add(droNameItem);
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                    model.IsDrLogin = true;
                    model.IsSrLogin = false;
                }
                else//For Login other than SR and DR Login
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //SROfficeList.Add(select);
                    //model.SROfficeList = SROfficeList;
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    model.DROfficeList = objCommon.GetDROfficesList("All");
                    model.IsDrLogin = false;
                    model.IsSrLogin = false;
                }
                //model.OfficeType = "DR";//By default DR Passing to View

                //Added by Madhusoodan on 28-08-2020
                model.DocumentType = objCommon.GetDocumentType();
                model.DocumentTypeID = 1; //For by default Document

                return model;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //public ScannedFileUploadStatusRptReqModel GetScannedFileUploadStatusDetails(int OfficeID)
        //{
        //    try
        //    {
        //        string FirstRecord = "All";
        //        dbContext = new KaveriEntities();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        List<SelectListItem> SROfficeList = new List<SelectListItem>();
        //        ScannedFileUploadStatusRptReqModel model = new ScannedFileUploadStatusRptReqModel();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DROfficeList = new List<SelectListItem>();
        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        string kaveriCode = Kaveri1Code.ToString();

        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))//For SR Login
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);
        //            model.SROfficeList = new List<SelectListItem>();
        //            model.DROfficeList = new List<SelectListItem>();
        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DROfficeList.Add(droNameItem);
        //            model.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))//For DR Login
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DROfficeList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
        //        }
        //        else//For Login other than SR and DR Login
        //        {
        //            SelectListItem select = new SelectListItem();
        //            select.Text = "All";
        //            select.Value = "0";
        //            SROfficeList.Add(select);
        //            model.SROfficeList = SROfficeList;
        //            model.DROfficeList = objCommon.GetDROfficesList("All");
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
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        public ScannedFileUploadStatusRptResModel LoadScannedFileUploadStatusTable(ScannedFileUploadStatusRptReqModel ReqModel)
        {
            try
            {
                int Counter = 1;
                dbContext = new KaveriEntities();
                ScannedFileUploadStatusRptResModel ScannedFilesResmodel = new ScannedFileUploadStatusRptResModel();
                ScannedFileUploadStatusDetailsModel ScannedFilesRptModel = null;
                List<ScannedFileUploadStatusDetailsModel> ScannedFilesDetailsList = new List<ScannedFileUploadStatusDetailsModel>();
                List<USP_RPT_ScannedFileUploadStatusReport_Result> todaysDocRegList = new List<USP_RPT_ScannedFileUploadStatusReport_Result>();
                
                //Shubham bhagat 23-4-2020
                todaysDocRegList = dbContext.USP_RPT_ScannedFileUploadStatusReport(ReqModel.DROfficeID, ReqModel.SROfficeID, ReqModel.OfficeType, ReqModel.DocumentTypeID).OrderBy(x=>x.SRONAMEE).ToList();

                ScannedFilesResmodel.ScannedFileList = new List<ScannedFileUploadStatusDetailsModel>();
                foreach (var item in todaysDocRegList)
                {
                    ScannedFilesRptModel = new ScannedFileUploadStatusDetailsModel();
                    ScannedFilesRptModel.SrNo = Counter++;
                    ScannedFilesRptModel.SubRegistrarOffice = string.IsNullOrEmpty(item.SRONAMEE) ? "" : item.SRONAMEE;
                    ScannedFilesRptModel.LastUploadDateTime = (item.LastUploadDateTime == null) ? "-" : ((DateTime)item.LastUploadDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    //Added by Madhusoodan on 29-04-2020
                    //To show the other columns from DB
                    ScannedFilesRptModel.RegistrationModule = string.IsNullOrEmpty(item.REGISTRATION_MODULE) ? "" : item.REGISTRATION_MODULE;
                    ScannedFilesRptModel.NoOfFiles = item.No_of_Files == null? "0" : item.No_of_Files.ToString();

                    //ScannedFilesRptModel.FileSize = (item.FILE_SIZE == null) ? "0.000" : item.FILE_SIZE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    ScannedFilesRptModel.FileSize = (item.FILE_SIZE == null) ? "0.000" : (item.FILE_SIZE / 1024).Value.ToString("N3", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                   

                    //Commented by Mahusoodan on 28-04-2020
                    //ScannedFilesRptModel.FilePendingForDays = (item.FilePendingForDays == null ) ? "" : Convert.ToString(item.FilePendingForDays);
                    ScannedFilesDetailsList.Add(ScannedFilesRptModel);
                }
                ScannedFilesResmodel.ScannedFileList = ScannedFilesDetailsList;
                ScannedFilesResmodel.GenerationDateTime = DateTime.Now.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
                return ScannedFilesResmodel;

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

        //public string NullableDateTimeToString(DateTime? dt, string format)
        //{
        //    string stingformatteddatetime = (dt == null )? "" : ((DateTime)dt).ToString(format, CultureInfo.InvariantCulture);
        //    return stingformatteddatetime;
        //}
    }
}