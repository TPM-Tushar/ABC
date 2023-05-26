using CustomModels.Models.MISReports.ScanningStatistics;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ScanningStatisticsDAL 
    {
        KaveriEntities dbContext = null;
        public ScanningStatisticsReqModel ScanningStatisticsView(int OfficeID)
        {
            ScanningStatisticsReqModel resModel = new ScanningStatisticsReqModel();

            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "Select";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
          

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                string kaveriCode = Kaveri1Code.ToString();

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DROfficeList = new List<SelectListItem>();

                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList.Add(sroNameItem);

                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
                }
                else
                {

                    SelectListItem select = new SelectListItem();
                    select.Text = "Select";
                    select.Value = "0";
                    resModel.SROfficeList.Add(select);
                    resModel.DROfficeList = objCommon.GetDROfficesList("Select");

                }

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
            return resModel;
        }

       public ScanningStatisticsResModel GetScanningStatisticsDetails(ScanningStatisticsReqModel scanningStatisticsResModel)
        {
            ScanningStatisticsResModel resultModel = new ScanningStatisticsResModel();
            resultModel.scanningStatisticsTableModelsList = new List<ScanningStatisticsTableModel>();
            ScanningStatisticsTableModel resModel = null;
            long SrCount = 1;

            int SROfficeListID = Convert.ToInt32(scanningStatisticsResModel.SROfficeID);
            int DROfficeListID = Convert.ToInt32(scanningStatisticsResModel.DROfficeID);
            var FromDate = Convert.ToDateTime(scanningStatisticsResModel.DateTime_FromDate).ToString("dd/MM/yyyy");
            var ToDate = Convert.ToDateTime(scanningStatisticsResModel.DateTime_ToDate).ToString("dd/MM/yyyy");
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                var Result = dbContext.ECMIS_ScannedPageCountDetails(FromDate,ToDate, DROfficeListID, SROfficeListID).ToList();

                if (Result != null)
                {
                    foreach (var item in Result)
                    {

                        resModel = new ScanningStatisticsTableModel();
                        resModel.srNo = SrCount++;
                        resModel.DistrictName = item.DistrictName == "" ? "--" : item.DistrictName;
                        resModel.SROName = item.SROName == "" ? "--" : item.SROName;
                        resModel.RegistrationNumber = item.RegistrationNumber == "" ? "--" : item.RegistrationNumber;
                        //resModel.DateOfRegistration = item.DateOfRegistration.Value.ToString("dd/MM/yyyy");
                        resModel.DateOfRegistration = String.IsNullOrEmpty(item.DateOfRegistration.ToString()) ? "--" :Convert.ToDateTime(item.DateOfRegistration).ToString("dd/MM/yyyy");
                        resModel.ScannedPagecount = item.ScannedPagecount.ToString();
                        //resModel.ScanDate = item.ScanDate.Value.ToString("dd/MM/yyyy");
                        resModel.ScanDate = String.IsNullOrEmpty(item.ScanDate.ToString()) ? "--" : Convert.ToDateTime(item.ScanDate).ToString("dd/MM/yyyy");
                        resModel.DocType = item.DocType == "" ? "--" : item.DocType;



                        resultModel.scanningStatisticsTableModelsList.Add(resModel);


                    }

                }
                //}
          
            }
            catch (Exception ex)
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
            return resultModel;
        }
    }
}