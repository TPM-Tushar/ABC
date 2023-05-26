using CustomModels.Models.MISReports.ARegisterGenerationDetails;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ARegisterGenerationDetailsDAL
    {
        KaveriEntities dbContext = null;
        public ARegisterGenerationDetailsModel ARegisterGenerationDetailsView(int OfficeID)
        {
            ARegisterGenerationDetailsModel resModel = new ARegisterGenerationDetailsModel();
            try
            {
                dbContext = new KaveriEntities();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";
                //  resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                    ;
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
                    select.Text = "All";
                    select.Value = "0";
                    resModel.SROfficeList.Add(select);
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");

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

        public List<ARegisterGenerationDetailsTableModel> GetARegisterGenerationReportsDetails(ARegisterGenerationDetailsModel model)
        {
            ARegisterGenerationDetailsTableModel ReportsDetails = null;
            List<ARegisterGenerationDetailsTableModel> ReportsDetailsList = new List<ARegisterGenerationDetailsTableModel>();

            try
            {
              
                long SrCount = 1;
                dbContext = new KaveriEntities();

                var ARegisterReportList = dbContext.USP_AREGISTER_ReportGenerationDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.SROfficeID, model.DROfficeID, model.NGFile).ToList();
                foreach (var item in ARegisterReportList)
                {
                    ReportsDetails = new ARegisterGenerationDetailsTableModel();

                    ReportsDetails.srNo = SrCount++;
           
                    ReportsDetails.DistrictName = String.IsNullOrEmpty(item.DistrictName) ? "-" : item.DistrictName;
                    ReportsDetails.SroName =String.IsNullOrEmpty( item.SRONameE)?"-" : item.SRONameE;
                    ReportsDetails.File_Path = String.IsNullOrEmpty(item.PFilePath)?"-": item.PFilePath;
              
                    ReportsDetails.Receipt_Date = String.IsNullOrEmpty(item.ReceiptDate.ToString()) ? "-" : item.ReceiptDate.ToString();
                    ReportsDetails.IsReceiptsSynchronized =String.IsNullOrEmpty( item.IsReceiptsSynchronized.ToString())? "-" : item.IsReceiptsSynchronized.ToString();
                    ReportsDetails.File_Gen_Date =String.IsNullOrEmpty( item.FileGenerationDate.ToString())?"-": item.FileGenerationDate.ToString();
                    ReportsDetailsList.Add(ReportsDetails);
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
            return ReportsDetailsList;
        }
    }
}