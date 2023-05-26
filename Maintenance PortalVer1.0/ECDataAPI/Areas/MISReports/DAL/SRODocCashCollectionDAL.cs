#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SRODocCashCollectionDAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SRODocCashCollection;
using ECDataAPI.Areas.MISReports.Interface;
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
    public class SRODocCashCollectionDAL:IDisposable
    {
        KaveriEntities dbContext = null;
        /// <summary>
        /// Returns SRODocCashCollectionResponseModel to show SRODocCashCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SRODocCashCollectionResponseModel SRODocCashCollectionView(int OfficeID)
        {
            SRODocCashCollectionResponseModel resModel = new SRODocCashCollectionResponseModel();

            try
            {
                dbContext = new KaveriEntities();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";
                //  resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();
                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DROfficeList = new List<SelectListItem>();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

               

                    sroNameItem = GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
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
                    SROfficeList.Add(select);
                    resModel.SROfficeList = SROfficeList;
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");
                    // resModel.Stamp5Date = Convert.ToString(DateTime.Now);

                }

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

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
        /// Returns SelectListItem
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
        /// Returns List of SRODocCashDetailsModel required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SRODocCashDetailsModel> GetSRODocCashCollectionReportsDetails(SRODocCashCollectionResponseModel model)
        {

            SRODocCashDetailsModel ReportsDetails = null;
            List<SRODocCashDetailsModel> ReportsDetailsList = new List<SRODocCashDetailsModel>();
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                var TransactionList = dbContext.USP_RPT_SRO_CASH_COLLECTION(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                decimal TotalStampDuty = 0;
                decimal TotalReceiptFee = 0;
                decimal TotalSum = 0;
                long SrCount = 1;
                foreach (var item in TransactionList)
                {

                    ReportsDetails = new SRODocCashDetailsModel();
                    ReportsDetails.SrNo = SrCount++;


                    if (item.DocumentNumber.HasValue)
                        ReportsDetails.DocumentNumber = item.DocumentNumber.Value.ToString();
                    else
                        ReportsDetails.DocumentNumber = "null";



                    if (!string.IsNullOrEmpty(item.FinalRegistrationNumber))
                        ReportsDetails.FinalRegistrationNumber = item.FinalRegistrationNumber.ToString();
                    else
                        ReportsDetails.FinalRegistrationNumber = "null";


                    if (!string.IsNullOrEmpty(item.ReceiptNumber))
                        ReportsDetails.ReceiptNumber = item.ReceiptNumber.ToString();
                    else
                        ReportsDetails.ReceiptNumber = "null";

                    if (item.PresentDateTime.HasValue)
                        //ReportsDetails.PresentDatetime = item.PresentDateTime.Value.ToString();
                        ReportsDetails.PresentDatetime = Convert.ToDateTime(item.PresentDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    else
                        ReportsDetails.PresentDatetime = String.Empty;


                    //Commented By Raman on 28-06-2019
                    //ReportsDetails.StampDuty = item.STAMPDUTY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    //ReportsDetails.ReceiptFee = item.RECEIPTFEE.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    ReportsDetails.StampDuty = item.STAMPDUTY;
                    ReportsDetails.ReceiptFee = item.RegistrationFee;
                    //   ReportsDetails.StampDuty = item.stampduty;
                    //   ReportsDetails.ReceiptFee = item.ReceiptFee;

                    if (!string.IsNullOrEmpty(item.SRONAME))
                        ReportsDetails.SROName = item.SRONAME;
                    else
                        ReportsDetails.SROName = "null";

                    ReportsDetails.Total= item.STAMPDUTY + item.RegistrationFee;
                    TotalSum = ReportsDetails.Total + TotalSum;
                    TotalStampDuty = TotalStampDuty + item.STAMPDUTY;
                    TotalReceiptFee = TotalReceiptFee + item.RegistrationFee;
                    ReportsDetails.SROName = string.IsNullOrEmpty(item.SRONAME) ? "" : item.SRONAME; 
                    ReportsDetailsList.Add(ReportsDetails);

                }
                if (model.IsPdf == true || model.IsExcel == true)
                {

                    ReportsDetails = new SRODocCashDetailsModel();
                    ReportsDetails.SrNo = 0;
                    ReportsDetails.SROName = "";
                    ReportsDetails.DocumentNumber = "";
                    ReportsDetails.FinalRegistrationNumber = "";
                    ReportsDetails.ReceiptNumber = "";
                    ReportsDetails.PresentDatetime = "Total";
                    //Commented By Raman on 28-06-2019
                    //ReportsDetails.StampDuty = item.STAMPDUTY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    //ReportsDetails.ReceiptFee = item.RECEIPTFEE.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    ReportsDetails.StampDuty = TotalStampDuty;
                    ReportsDetails.ReceiptFee = TotalReceiptFee;
                    ReportsDetails.Total = TotalSum;
                    //   ReportsDetails.StampDuty = item.stampduty;
                    //   ReportsDetails.ReceiptFee = item.ReceiptFee;
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
         
        /// <summary>
        /// Returns totalCount of List of SRODocCashDetailsModel 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetSRODocCashCollectionReportsDetailsTotalCount(SRODocCashCollectionResponseModel model)
        {

            List<SRODocCashCollectionResponseModel> indexIIReportsDetailsList = new List<SRODocCashCollectionResponseModel>();
            KaveriEntities dbContext = null;
            //List<USP_INDEX2_DETAILS_Result> Result = null;
            // long Amount = Convert.ToInt64(model.Amount);
            int Count = 0;
            try
            {
                dbContext = new KaveriEntities();

                var TransactionList = dbContext.USP_RPT_SRO_CASH_COLLECTION(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();


                Count = TransactionList.Count;
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