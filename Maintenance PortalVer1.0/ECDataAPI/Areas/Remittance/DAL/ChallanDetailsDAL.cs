using CustomModels.Models.Remittance.ChallanDetailsReport;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ChallanDetailsDAL : IChallanDetails
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion


        public ChallanDetailsModel ChallanDetailsReportView()
        {
            ChallanDetailsModel model = new ChallanDetailsModel();
            try
            {
                model.StampType = new List<SelectListItem>();
                model.Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime date = DateTime.Now.Date;
                //List<StampTypeMaster> stampTypeMasters = dbContext.StampTypeMaster.ToList();
                model.StampType.Add(new SelectListItem() { Text = "Select", Value = "0" });
                //foreach (var item in stampTypeMasters)
                //{
                //    model.StampType.Add(new SelectListItem() { Text = item.NameInEnglish, Value = item.StampTypeID.ToString() });
                //}

            }
            catch (Exception)
            {
                throw;
            }
            return model;
        }

        public ChallanDetailsResModel GetChallanReportDetails(ChallanDetailsModel model)
        {
            ChallanDetailsResModel resModel = new ChallanDetailsResModel();
            List<ChallanDetailsDataTableModel> ChallanDetailsDataTableModelList = new List<ChallanDetailsDataTableModel>();

            DateTime Date = DateTime.Parse(model.Date);
            int SrNo = 1;
            try
            {
                var result = dbContext.RPT_GetInstrumentDetails(model.InstrumentNumber, model.Date, 0);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        ChallanDetailsDataTableModel challanDetailObj = new ChallanDetailsDataTableModel();
                        challanDetailObj.SrNo = SrNo++;
                        challanDetailObj.SROName = string.IsNullOrEmpty(item.SRONameE) ? "-" : item.SRONameE;
                        challanDetailObj.IsPayDoneAtDROffice = item.IsDRO ? "Yes" : "No";
                        challanDetailObj.DistrictName = item.IsDRO ? item.DistrictNameE : "-";
                        challanDetailObj.ChallanNumber = item.InstrumentNumber;
                        challanDetailObj.ChallanDate = item.InstrumentDate == null ? "" : ((DateTime)item.InstrumentDate).ToString("dd-MM-yyyy");
                        challanDetailObj.Amount = item.Amount;
                        challanDetailObj.IsStampPayment = item.StampTypeID == 0 ? "No" : "Yes";
                        challanDetailObj.IsReceiptPayment = item.ReceiptID > 0 ? "Yes" : "No";
                        challanDetailObj.ReceiptNumber = item.ReceiptNumber == null ? "-" : Convert.ToString(item.ReceiptNumber);
                        challanDetailObj.Receipt_StampPayDate = item.Receipt_StampDate == null ? "" : item.Receipt_StampDate.ToString();
                        challanDetailObj.InsertDateTime = item.InsertDateTime == null ? "" : item.InsertDateTime.ToString();
                        challanDetailObj.ServiceName = item.ServiceName;
                        challanDetailObj.DocumentPendingNumber = item.DocumentPendingNumber;
                        challanDetailObj.FinalRegistrationNumber = item.FRN;
                        ChallanDetailsDataTableModelList.Add(challanDetailObj);

                    }

                }
                resModel.challanDetailsDataTableList = ChallanDetailsDataTableModelList;
            }
            catch (Exception)
            {
                throw;
            }

            return resModel;
        }
    }
}