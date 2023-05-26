#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   AnywhereECLogDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion
using CustomModels.Models.MISReports.AnywhereECLog;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class AnywhereECLogDAL : IAnywhereECLog
    {
        KaveriEntities dbContext = null;
        public AnywhereECLogView AnywhereECLogView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();

                AnywhereECLogView model = new AnywhereECLogView();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.

                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string kaveriCode = ofcDetailsObj.Kaveri1Code.ToString();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });

                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(ofcDetailsObj.Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);

                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "Select";
                    //select.Value = "0";
                    model.SROfficeList.Add(new SelectListItem() { Text = "Select", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("Select");

                }
                model.LogTypeList = objCommon.GetLogTypes("All");
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //public AnywhereECLogView AnywhereECLogView(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();

        //        AnywhereECLogView model = new AnywhereECLogView();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DistrictList = new List<SelectListItem>();
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

        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {

        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
        //        }
        //        else
        //        {

        //            SelectListItem select = new SelectListItem();
        //            select.Text = "Select";
        //            select.Value = "0";
        //            model.SROfficeList.Add(select);
        //            model.DistrictList = objCommon.GetDROfficesList("Select");

        //        }
        //        model.LogTypeList = objCommon.GetLogTypes("All");
        //        return model;



        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        public AnywhereECLogResModel GetAnywhereECLogDetails(AnywhereECLogView model)
        {

            AnywhereECLogResModel AnywhereECResModel = new AnywhereECLogResModel();
            AnywhereECLogDetailsModel ReportsDetails = null;
            List<AnywhereECLogDetailsModel> ReportsDetailsList = new List<AnywhereECLogDetailsModel>();
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                Byte LogTypeId = Convert.ToByte(model.LogTypeID);
                var ReceiptDetailsList = dbContext.GetECUserLog(model.SROfficeID,LogTypeId, model.FromDate, model.ToDate).ToList();
                AnywhereECResModel.TotalRecords = ReceiptDetailsList.Count;
                int counter = (model.startLen + 1); //To start Serial Number 
                AnywhereECResModel.AnywhereECDetailsList = new List<AnywhereECLogDetailsModel>();
                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        ReceiptDetailsList = ReceiptDetailsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                foreach (var item in ReceiptDetailsList)
                {
                    ReportsDetails = new AnywhereECLogDetailsModel();
                    ReportsDetails.SerialNo = counter++;
                    ReportsDetails.ApplicationNo = string.IsNullOrEmpty(item.ApplicationNumber) ? "" : item.ApplicationNumber;
                    ReportsDetails.SROfficeAppNo = string.IsNullOrEmpty(item.SROApplicationNumber) ? "": item.SROApplicationNumber;
                    ReportsDetails.UserName = string.IsNullOrEmpty(item.LoginName) ? "": item.LoginName;
                    ReportsDetails.Desc = string.IsNullOrEmpty(item.Description) ? "": item.Description;
                    if (item.LogDateTime == null)
                        ReportsDetails.LogDateTime = "";
                    else
                        //ReportsDetails.LogDateTime = item.LogDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ReportsDetails.LogDateTime = item.LogDateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    ReportsDetails.ApplicationFilingDate = item.ApplicationDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
                    AnywhereECResModel.AnywhereECDetailsList.Add(ReportsDetails);
                }
              
                return AnywhereECResModel;

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
        public int GetAnywhereECLogTotalCount(AnywhereECLogView model)
        {
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                Byte LogTypeId = Convert.ToByte(model.LogTypeID);
                var ReceiptDetailsList = dbContext.GetECUserLog(model.SROfficeID, LogTypeId, model.FromDate, model.ToDate).ToList();
                return ReceiptDetailsList.Count();
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