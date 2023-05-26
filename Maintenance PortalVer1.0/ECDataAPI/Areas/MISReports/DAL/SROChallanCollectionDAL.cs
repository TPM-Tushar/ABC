using CustomModels.Models.MISReports.SROChallanCollection;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class SROChallanCollectionDAL : IDisposable
    {
        KaveriEntities dbContext = null;
        
        public SROChallanCollectionResponseModel SROChallanCollectionView(int OfficeID)
        {
            SROChallanCollectionResponseModel resModel = new SROChallanCollectionResponseModel();
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
                    dbContext.Dispose();
            }
            return resModel;

        }


        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }


        public List<SROChallanCollectionDetailsModel> GetSROChallanCollectionReportsDetails(SROChallanCollectionResponseModel model)
        {

            SROChallanCollectionDetailsModel ReportsDetails = null;
            List<SROChallanCollectionDetailsModel> ReportsDetailsList = new List<SROChallanCollectionDetailsModel>();
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                //var TransactionList = dbContext.USP_RPT_DDPO_COLLECTION(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                var TransactionList = dbContext.USP_RPT_CHALLAN_COLLECTION(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                decimal TotalStampDuty = 0;
                decimal TotalRegFee = 0;
                decimal TotalDdAmount = 0;
                long SrCount = 1;
                foreach (var item in TransactionList)
                {
                    ReportsDetails = new SROChallanCollectionDetailsModel();
                    ReportsDetails.SrNo = SrCount++;


                    ReportsDetails.DocumentNumber = string.IsNullOrEmpty(item.Documentnumber) ? "null" : item.Documentnumber;
                    ReportsDetails.ReceiptNumber = string.IsNullOrEmpty(item.ReceiptNumber) ? "null" : item.ReceiptNumber;


                    ReportsDetails.PresentDatetime = item.PresentDatetime != null ? Convert.ToDateTime(item.PresentDatetime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : String.Empty;
                    ReportsDetails.DDChalNumber = string.IsNullOrEmpty(item.DDChallanNumber) ? "" : item.DDChallanNumber;

                    ReportsDetails.StampDuty = item.stampduty;
                    ReportsDetails.RegistrationFee = item.RegistrationFee;

                    ReportsDetails.DDAmount = Convert.ToInt32(item.DDAmount);
                    TotalStampDuty = TotalStampDuty + item.stampduty;
                    TotalRegFee = TotalRegFee + item.RegistrationFee;
                    TotalDdAmount = Convert.ToDecimal(TotalDdAmount + item.DDAmount);
                    ReportsDetails.SroName = string.IsNullOrEmpty(item.SRONAMEE) ? "" : item.SRONAMEE;
                    ReportsDetailsList.Add(ReportsDetails);

                }
                if (model.IsExcel == true || model.IsPdf == true)
                {
                    ReportsDetails = new SROChallanCollectionDetailsModel();
                    ReportsDetails.SrNo = SrCount++;


                    ReportsDetails.DocumentNumber = "";
                    ReportsDetails.ReceiptNumber = "";
                    ReportsDetails.SroName = "";

                    ReportsDetails.PresentDatetime = "";
                    ReportsDetails.DDChalNumber = "Total";

                    ReportsDetails.StampDuty = TotalStampDuty;
                    ReportsDetails.RegistrationFee = TotalRegFee;

                    ReportsDetails.DDAmount = TotalDdAmount;


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
                    dbContext.Dispose();
            }

            return ReportsDetailsList;


        }
     

        public int GetSROChallanCollectionReportsDetailsTotalCount(SROChallanCollectionResponseModel model)
        {

            List<SROChallanCollectionResponseModel> indexIIReportsDetailsList = new List<SROChallanCollectionResponseModel>();
            KaveriEntities dbContext = null;
            //List<USP_INDEX2_DETAILS_Result> Result = null;
            // long Amount = Convert.ToInt64(model.Amount);
            int Count = 0;
            try
            {
                dbContext = new KaveriEntities();
                var Result = dbContext.USP_RPT_CHALLAN_COLLECTION(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                Count = Result.Count;
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

            //return Result.Count();

            return Count;


        }

        public string GetSroName(int SROfficeID)
        {
            // IndexIIReportsResponseModel resModel = new IndexIIReportsResponseModel();
            string SroName;
            try
            {
                dbContext = new KaveriEntities();
                SroName = dbContext.SROMaster.Where(x => x.SROCode == SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
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
            return SroName;

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}