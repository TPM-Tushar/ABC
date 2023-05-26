#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   AnywhereRegistrationStatisticsDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion


using CustomModels.Models.MISReports.AnywhereECLog;
using CustomModels.Models.MISReports.AnywhereRegistrationStatistics;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.DAL;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class AnywhereRegistrationStatisticsDAL : IAnywhereRegistrationStatistics
    {
        public AnywhereRegStatViewModel AnywhereRegistrationStatisticsView(int OfficeID)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                AnywhereRegStatViewModel model = new AnywhereRegStatViewModel();
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
                //string kaveriCode = Kaveri1Code.ToString();
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

                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(ofcDetailsObj.Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DistrictList.Add(droNameItem);
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
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
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public AnywhereRegStatViewModel AnywhereRegistrationStatisticsView(int OfficeID)
        //{
        //    KaveriEntities dbContext = null;
        //    try
        //    {
        //        dbContext = new KaveriEntities();

        //        AnywhereRegStatViewModel model = new AnywhereRegStatViewModel();
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
        //        return model;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public AnywhereRegStatResModel GetAnywhereRegStatDetails(AnywhereRegStatViewModel model)
        {

            Dictionary<int, string> SRODictionary = new Dictionary<int, string>();
           AnywhereRegStatResModel anywhereRegStatModel = new AnywhereRegStatResModel();
            AnywhereRegStatDetailsModel RegStatDetailsVar = null;
            anywhereRegStatModel.AnywhereRegStatList = new List<AnywhereRegStatDetailsModel>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                var AnywhereRegStatList = dbContext.GetDocumentRegistrationStatistics(model.DistrictID, model.FromDate, model.ToDate);             

                foreach (var item in AnywhereRegStatList)
                {
                    RegStatDetailsVar = new AnywhereRegStatDetailsModel();
                    RegStatDetailsVar.SROCode = Convert.ToString(item.SROCode);
                    RegStatDetailsVar.RegSROCode = Convert.ToString(item.RegSROCode);
                    RegStatDetailsVar.RegOffice = string.IsNullOrEmpty(item.RegOffice) ? string.Empty : item.RegOffice;
                    RegStatDetailsVar.Jurisdiction = string.IsNullOrEmpty(item.Jurisdiction) ? string.Empty : item.Jurisdiction;
                    RegStatDetailsVar.DocCount = item.DocCount == null ? "" : Convert.ToString(item.DocCount);
                    anywhereRegStatModel.AnywhereRegStatList.Add(RegStatDetailsVar);
                }

                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                Dictionary<int, int> SROOfficeDict = new Dictionary<int, int>();
                Dictionary<int, int> JurisdictionWiseDict = new Dictionary<int, int>();
                CommonDAL objCommonDAL = new CommonDAL();

                sroOfficeList = objCommonDAL.GetSROfficesListByDisrictID(false, model.DistrictID);
                SROOfficeDict = objCommonDAL.GetSROOfficeListDictionary(model.DistrictID);
                //Remove This Comment
                //JurisdictionWiseDict = objCommonDAL.GetJurisdictionWiseDictionary(model.DistrictID);
                //This is Alternative
                JurisdictionWiseDict = SROOfficeDict;
                int[,] AnywhereRegStatArray = new int[(SROOfficeDict.Count+1), (JurisdictionWiseDict.Count + 2)];
                int RowIndex, ColIndex;

                foreach (var AnywhereRegStatVar in anywhereRegStatModel.AnywhereRegStatList)
                {
                    RowIndex = JurisdictionWiseDict[Convert.ToInt32(AnywhereRegStatVar.SROCode)];
                    ColIndex = SROOfficeDict[Convert.ToInt32(AnywhereRegStatVar.RegSROCode)];
                    AnywhereRegStatArray[RowIndex, ColIndex] = Convert.ToInt32(AnywhereRegStatVar.DocCount);
                }

                List<int> TotalJurisdictionList = new List<int>();
                int TotalJursdnCount = 0;
                for (RowIndex = 1; RowIndex <= SROOfficeDict.Count; RowIndex++)
                {
                    for (ColIndex = 1; ColIndex <= SROOfficeDict.Count; ColIndex++)
                    {
                        //if (AnywhereRegStatArray[RowIndex, ColIndex] == null)
                        //{
                        //    AnywhereRegStatArray[RowIndex, ColIndex] = 0;
                        //}
                        TotalJursdnCount = TotalJursdnCount + AnywhereRegStatArray[RowIndex, ColIndex];
                    }
                    TotalJurisdictionList.Add(TotalJursdnCount);
                    TotalJursdnCount = 0;
                }
                int RowIndexForTotal = 1;
                foreach (int JursdnCount in TotalJurisdictionList)
                {
                    AnywhereRegStatArray[RowIndexForTotal, (SROOfficeDict.Count + 1)] = JursdnCount;
                    RowIndexForTotal++;
                }

               
                //ColIndex = 1;
                //RowIndex = 1;
                //foreach (var itemSRO in sroOfficeList)
                //{

                //    AnywhereRegStatArray[0, ColIndex] = itemSRO.Text;  
                //    AnywhereRegStatArray[RowIndex, 0] = itemSRO.Text;//Comment This
                //    ColIndex++;
                //    RowIndex++;
                //}

                //foreach (var itemSRO in JurisdictionWiseList)
                //{

                //    AnywhereRegStatArray[RowIndex, 0] = itemSRO.Text;
                //    ColIndex++;
                //    RowIndex++;
                //}
                anywhereRegStatModel.AnywhereRegStatArray = AnywhereRegStatArray;
                anywhereRegStatModel.RowCount = SROOfficeDict.Count;
                anywhereRegStatModel.ColumnCount = SROOfficeDict.Count;
                anywhereRegStatModel.SROList = sroOfficeList;
                anywhereRegStatModel.SRODictionary = GetSROOfficeListDictionary(sroOfficeList);

                anywhereRegStatModel.TDWidth = Convert.ToInt32(100 / (anywhereRegStatModel.SROList.Count + 1));
                return anywhereRegStatModel;
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

        //Added by Raman Kalegaonkar on 16-09-2019 for iterating First Column of SRO
        public Dictionary<int, string> GetSROOfficeListDictionary(List<SelectListItem> SROList)
        {
            int CounterSroDict = 1;
            Dictionary<int, string> SROOfficeDict = new Dictionary<int, string>();
            try
            {
                foreach (var SROListObj in SROList)
                {
                    SROOfficeDict.Add(CounterSroDict++, SROListObj.Text);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return SROOfficeDict;
        }

    }
}





