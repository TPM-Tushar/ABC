#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SroDD_POCollectionDAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SroDD_POCollection;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class SroDD_POCollectionDAL: IDisposable
    {
        KaveriEntities dbContext = null;
        /// <summary>
        /// Returns SroDD_POCollectionResponseModel required to show SroDD_POCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SroDD_POCollectionResponseModel SroDD_POCollectionView(int OfficeID)
        {
            SroDD_POCollectionResponseModel resModel = new SroDD_POCollectionResponseModel();

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
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code,FirstRecord);
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
        /// <summary>
        /// retruns SelectListItem
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
        /// Returns List of SroDD_POCollectionDetailsModel Required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        public List<SroDD_POCollectionDetailsModel> GetSroDD_POCollectionReportsDetails(SroDD_POCollectionResponseModel model)
        {

            SroDD_POCollectionDetailsModel ReportsDetails = null;
            List<SroDD_POCollectionDetailsModel> ReportsDetailsList = new List<SroDD_POCollectionDetailsModel>();
            KaveriEntities dbContext = null;
            // long Amount = Convert.ToInt64(model.Amount);
            try
            {

                dbContext = new KaveriEntities();
                var TransactionList = dbContext.USP_RPT_DDPO_COLLECTION( model.DROfficeID,model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                decimal TotalStampDuty = 0;
                decimal TotalRegFee = 0;
                decimal TotalDdAmount = 0;
                long SrCount = 1;
                foreach (var item in TransactionList)
                {
                    ReportsDetails = new SroDD_POCollectionDetailsModel();
                    ReportsDetails.SrNo = SrCount++;

                    
                    ReportsDetails.DocumentNumber = string.IsNullOrEmpty(item.Documentnumber)?"null": item.Documentnumber;
                    ReportsDetails.ReceiptNumber = string.IsNullOrEmpty(item.ReceiptNumber) ? "null": item.ReceiptNumber;


                    ReportsDetails.PresentDatetime = item.PresentDatetime!=null ? Convert.ToDateTime(item.PresentDatetime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)  : String.Empty;
                    ReportsDetails.DDChalNumber = string.IsNullOrEmpty(item.DDChallanNumber) ? "": item.DDChallanNumber;

                    ReportsDetails.StampDuty = item.stampduty;
                    ReportsDetails.RegistrationFee = item.RegistrationFee;

                    ReportsDetails.DDAmount = item.DDAmount;
                    TotalStampDuty = TotalStampDuty + item.stampduty;
                    TotalRegFee = TotalRegFee + item.RegistrationFee;
                    TotalDdAmount = TotalDdAmount + item.DDAmount;
                    ReportsDetails.SroName = string.IsNullOrEmpty(item.SRONAMEE) ? "": item.SRONAMEE;
                    ReportsDetailsList.Add(ReportsDetails);

                }
                if (model.IsExcel == true || model.IsPdf == true)
                {
                    ReportsDetails = new SroDD_POCollectionDetailsModel();
                    ReportsDetails.SrNo = SrCount++;


                    ReportsDetails.DocumentNumber = "";
                    ReportsDetails.ReceiptNumber = "";
                    ReportsDetails.SroName = "";

                    ReportsDetails.PresentDatetime = "";
                    ReportsDetails.DDChalNumber= "Total";

                    ReportsDetails.StampDuty = TotalStampDuty;
                    ReportsDetails.RegistrationFee = TotalRegFee;

                    ReportsDetails.DDAmount = TotalDdAmount;


                    ReportsDetailsList.Add(ReportsDetails);


                }

            }
            catch (Exception )
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
        /// <summary>
        /// Returns Total Count GetSroDD_POCollectionReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public int GetSroDD_POCollectionReportsDetailsTotalCount(SroDD_POCollectionResponseModel model)
        {

            List<SroDD_POCollectionResponseModel> indexIIReportsDetailsList = new List<SroDD_POCollectionResponseModel>();
            KaveriEntities dbContext = null;
            //List<USP_INDEX2_DETAILS_Result> Result = null;
            // long Amount = Convert.ToInt64(model.Amount);
            int Count = 0;
            try
            {
                dbContext = new KaveriEntities();
              var  Result = dbContext.USP_RPT_DDPO_COLLECTION(model.DROfficeID,model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
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

        /// <summary>
        /// Returns SRoName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>
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