#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ARegisterAnalysisReportDAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 7 Sep 2022
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for ARegister Analysis Report.

*/
#endregion
using CustomModels.Models.Remittance.ARegisterAnalysisReport;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.PreRegApplicationDetailsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ARegisterAnalysisReportDAL : IARegisterAnalysisReport
    {
        KaveriEntities dbContext = new KaveriEntities();
     
		        
		 public ARegisterAnalysisReportModel ARegisterAnalysisReportView(int officeID)
        {
            ARegisterAnalysisReportModel resModel = new ARegisterAnalysisReportModel();
            try
            {
               
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.SROfficeList = new List<SelectListItem>();
                SelectListItem selectListFirst = new SelectListItem();
                selectListFirst.Text = "Select";
                selectListFirst.Value = "";
                resModel.SROfficeList.Insert(0, selectListFirst);
                //string FirstRecord = "Select";
                //resModel.SROfficeList.Add(FirstRecord);
                //
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            //selectListOBJ.Text = "Select";
                            //selectListOBJ.Value = "0";
                            selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
                            selectListOBJ.Value = item.SROCode.ToString();
                            //resModel.SROfficeList.Add("Select");
                            resModel.SROfficeList.Add(selectListOBJ);
                        }
                    }
                }
                //
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

        public ARegisterResultModel GetARegisterAnalysisReportDetails(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {
            ARegisterResultModel aRegisterResultModel = new ARegisterResultModel();
            try
            {
                long SrCount = 1;

                if (aRegisterAnalysisReportModel.ARegister)
                {
                    var Result = dbContext.RPT_ARegister(aRegisterAnalysisReportModel.DateTime_Date.ToString("dd-MM-yyyy"), aRegisterAnalysisReportModel.SROfficeID).ToList();
                    RPTARegisterResult rPTARegisterResult = null;
                    // rPTARegisterResult.DocumentID = 
                    aRegisterResultModel.ARegister_Result = new List<RPTARegisterResult>();

                foreach ( var item in Result)
                    {
                        // rPTARegisterResult.DocumentID = (long)item.DocumentID;
                        //rPTARegisterResult.SROCode = (int)item.SROCode;
                        rPTARegisterResult = new RPTARegisterResult();
                        rPTARegisterResult.srNo = SrCount++;
                        if (item.PresentDateTime != null)
                            rPTARegisterResult.PresentDateTime = Convert.ToDateTime(item.PresentDateTime).ToString("dd/MM/yyyy");
                        else
                            rPTARegisterResult.PresentDateTime = "--";
                        rPTARegisterResult.PresenterName = item.PresenterName ?? "--";
                        rPTARegisterResult.StampArticleName = item.StampArticleName ?? "--";
                        rPTARegisterResult.Consideration = (item.Consideration ?? 0);
                        rPTARegisterResult.StampDuty_Cash = (item.StampDuty_Cash ?? 0);
                        rPTARegisterResult.StampDuty_Others = (item.StampDuty_Others ?? 0);
                        rPTARegisterResult.GovtDuty = (item.GovtDuty ?? 0);
                        rPTARegisterResult.Infrastructure = (item.Infrastructure ?? 0);
                        rPTARegisterResult.Muncipal = (item.Muncipal ?? 0);
                        rPTARegisterResult.Corporation = (item.Corporation ?? 0);
                        rPTARegisterResult.TalukBoard = (item.TalukBoard ?? 0);
                        rPTARegisterResult.DocumentNumber = item.DocumentNumber ?? "--";
                        if (item.ReturnDate != null)
                            rPTARegisterResult.ReturnDate = Convert.ToDateTime(item.ReturnDate).ToString("dd/MM/yyyy");
                        else
                            rPTARegisterResult.ReturnDate = "--";
                        rPTARegisterResult.BookID = (item.BookID ?? "--");
                        rPTARegisterResult.VolumeName = (item.VolumeName ?? "--");
                        if (item.CompletionDate != null)
                            rPTARegisterResult.CompletionDate = Convert.ToDateTime(item.CompletionDate).ToString("dd/MM/yyyy");
                        else
                            rPTARegisterResult.CompletionDate = "--";
                        rPTARegisterResult.RegistrationFees = (item.RegistrationFees ?? 0);
                        rPTARegisterResult.Deficient_RegistrationFees = (item.Deficient_RegistrationFees ?? 0);
                        rPTARegisterResult.CopyFees = (item.CopyFees ?? 0);
                        rPTARegisterResult.MutationFee = (item.MutationFee ?? 0);
                        rPTARegisterResult.OtherFees = (item.OtherFees ?? 0);
                        rPTARegisterResult.HinduMarriageFee = (item.HinduMarriageFee ?? 0);
                        rPTARegisterResult.Remarks = item.Remarks ?? "--";





                        aRegisterResultModel.ARegister_Result.Add(rPTARegisterResult);
                    }
                    

                }
                else if(aRegisterAnalysisReportModel.AnyWhereECARegister)
                {
                    RPTARegisterAnywhereECResult rPTARegisterAnywhereECResult = null;
                    DateTime ToDate = aRegisterAnalysisReportModel.DateTime_Date.AddHours(20).AddMinutes(00);
                    DateTime FromDate = ToDate.AddDays(-1);
                    List<RPT_ARegister_AnywhereEC_Result> AnyWhereECList = dbContext.RPT_ARegister_AnywhereEC
                                                                        (FromDate, ToDate, aRegisterAnalysisReportModel.SROfficeID).ToList();
                    aRegisterResultModel.AnyWhereEC_ARegisterDetailList = new List<RPTARegisterAnywhereECResult>();
                    
                    foreach ( var item in AnyWhereECList)
                    {
                        rPTARegisterAnywhereECResult = new RPTARegisterAnywhereECResult();
                        rPTARegisterAnywhereECResult.srNo = SrCount++;

                        rPTARegisterAnywhereECResult.DocumentID = item.DocumentID;
                        //rPTARegisterAnywhereECResult.SROCode = (int)item.SROCode;
                       // rPTARegisterAnywhereECResult.SRONAME = item.SRONAME;
                        rPTARegisterAnywhereECResult.ReceiptDateTime = (DateTime)item.ReceiptDateTime;
                        rPTARegisterAnywhereECResult.PresenterName = item.PresenterName ?? "--";
                        rPTARegisterAnywhereECResult.DocumentNumber = item.DocumentNumber ?? "--";
                        if (item.OtherFees != null)
                            rPTARegisterAnywhereECResult.OtherFees = (decimal)item.OtherFees;
                        else
                            rPTARegisterAnywhereECResult.OtherFees = 0;
                        rPTARegisterAnywhereECResult.ReceiptID = item.ReceiptID;
                        rPTARegisterAnywhereECResult.SROApplicationNumber = item.SROApplicationNumber ?? "--";
                        rPTARegisterAnywhereECResult.ExemptionDescription = item.ExemptionDescription ?? "--";
                       // rPTARegisterAnywhereECResult.ReceiptID = item.ReceiptID ?? "--" ;
                        rPTARegisterAnywhereECResult.ReceiptNumber = item.ReceiptNumber;
                        aRegisterResultModel.AnyWhereEC_ARegisterDetailList.Add(rPTARegisterAnywhereECResult);
                    }

                    

                }
                else if(aRegisterAnalysisReportModel.KOSARegister)
                {
                    //
                    #region  KaveriOnline Service
                    using (PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService())
                    {
                        AregisterRequestModel aregisterRequestModel = new AregisterRequestModel();
                        //ARegisterAnalysisReportModel aRegisterAnalysisReportModel = new ARegisterAnalysisReportModel();
                        int DistrictCode = Convert.ToInt32(dbContext.SROMaster.Where(m => m.SROCode == aRegisterAnalysisReportModel.SROfficeID).
                                                        Select(m => m.DistrictCode).FirstOrDefault());
                        aregisterRequestModel.DistrictCode = DistrictCode;
                        aregisterRequestModel.SroCode = aRegisterAnalysisReportModel.SROfficeID;
                        aregisterRequestModel.ForDate = aRegisterAnalysisReportModel.DateTime_Date;
                        ARegisterResponseModel RespModel = objService.GetARegisterDetails(aregisterRequestModel);
                        if (RespModel != null)
                        {
                            if (RespModel.ResponseStatus == "000")
                            {
                                if (RespModel.AregiterDetailList != null)
                                {
                                    aRegisterResultModel.KOS_ARegisterDetailList = new List<AregisterKOSDetailModel>();
                                   
                                    AregisterKOSDetailModel aregisterKOSDetailModel = null;
                                    foreach (var item in RespModel.AregiterDetailList)
                                    {
                                        
                                        aregisterKOSDetailModel = new AregisterKOSDetailModel();
                                        aregisterKOSDetailModel.srNo = SrCount++;
                                        if (item.TransactionDateTime != null)
                                            aregisterKOSDetailModel.TransactionDateTime = Convert.ToDateTime(item.TransactionDateTime).ToString("dd/MM/yyyy");
                                        else
                                            aregisterKOSDetailModel.TransactionDateTime = "--";
                                        aregisterKOSDetailModel.PartyName = item.PartyName ?? "--";
                                        if (item.ApplicationType == "OnlineCCApplication")
                                            aregisterKOSDetailModel.CCStampDuty = item.CCStampDuty != 0 ? item.CCStampDuty.ToString() : "0";
                                        else
                                            aregisterKOSDetailModel.CCStampDuty = "0";
                                        string DocumentNo = item.ApplicationNumber ?? "--";
                                        string RecieptNo = item.ChallanRefNumber ?? "--";
                                        if (RecieptNo == "--")
                                        {
                                            aregisterKOSDetailModel.ApplicationNumber = (item.ApplicationNumber ?? "--");
                                        }
                                        else
                                        {
                                            string DocNo = DocumentNo + @" (" + System.Environment.NewLine + ("Challan No : " + RecieptNo) + ")";
                                            aregisterKOSDetailModel.ApplicationNumber = DocNo;
                                        }
                                        aregisterKOSDetailModel.TotalAmt = item.TotalAmt;

                                        aRegisterResultModel.KOS_ARegisterDetailList.Add(aregisterKOSDetailModel);
                                    }
                                }
                               
                            }
                            else
                                throw new Exception("KOS_" + aRegisterAnalysisReportModel.SROfficeID + "_" + aRegisterAnalysisReportModel.DateTime_Date.ToString("dd/MM/yyyy") + "::Error Occured in service");
                        }
                        else
                            throw new Exception("KOS_" + aRegisterAnalysisReportModel.SROfficeID + "_" + aRegisterAnalysisReportModel.DateTime_Date.ToString("dd/MM/yyyy") + "::Null object received from Service");
                    }
                    #endregion
                    //

                }
                //return aRegisterResultModel;
                return aRegisterResultModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ARegisterSynchcheckResultModel GetSynchronizationCheckResult(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {

            try
            {
                ARegisterSynchcheckResultModel aRegisterSynchcheckResultModel = new ARegisterSynchcheckResultModel();

                ARegisterGenerationDetails aRegisterGenerationDetails = dbContext.ARegisterGenerationDetails.
                                                                        Where(m => m.SROCode == aRegisterAnalysisReportModel.SROfficeID &&
                                                                        m.ReceiptDate.Day == aRegisterAnalysisReportModel.DateTime_Date.Day &&
                                                                        m.ReceiptDate.Month == aRegisterAnalysisReportModel.DateTime_Date.Month &&
                                                                        m.ReceiptDate.Year == aRegisterAnalysisReportModel.DateTime_Date.Year
                                                                        )
                                                                        .FirstOrDefault();
                if (aRegisterGenerationDetails == null)
                {
                    aRegisterSynchcheckResultModel.ResponseStatus = false;
                    aRegisterSynchcheckResultModel.ResponseMessage = "A Register details are not synchronised.";
                    // aRegisterSynchcheckResultModel.ReceiptsSynchronized = "0";
                    //aRegisterSynchcheckResultModel.ARegisterGenerated = "0";
                    aRegisterSynchcheckResultModel.ARegisterGenerated = false;
                    aRegisterSynchcheckResultModel.ReceiptsSynchronized = false;
                    return aRegisterSynchcheckResultModel;
                }
           if (aRegisterGenerationDetails != null)
                {
                    //aRegisterSynchcheckResultModel.ResponseStatus = false;
                    if(!aRegisterGenerationDetails.IsReceiptsSynchronized)
                    {
                        aRegisterSynchcheckResultModel.ResponseMessage = "A Register details are not synchronised.";
                    }
                    aRegisterSynchcheckResultModel.ARegisterGenerated = aRegisterGenerationDetails.IsARegisterGenerated;
                    aRegisterSynchcheckResultModel.ReceiptsSynchronized = aRegisterGenerationDetails.IsReceiptsSynchronized;
                }
 
                //if (aRegisterGenerationDetails.IsARegisterGenerated == true)
                //{
                //     aRegisterSynchcheckResultModel.ResponseStatus = false;
                //    // aRegisterSynchcheckResultModel.ResponseMessage = "A Register already Generated.";
                //    aRegisterSynchcheckResultModel.ReceiptsSynchronized = "1";
                //    aRegisterSynchcheckResultModel.ARegisterGenerated = "1";
                //    return aRegisterSynchcheckResultModel;
                //}
                //else
                //{
                //    aRegisterSynchcheckResultModel.ARegisterGenerated = "0";
                //    aRegisterSynchcheckResultModel.ReceiptsSynchronized = "1";
                //}

                return aRegisterSynchcheckResultModel;
            }
            catch (Exception ex)
            {
                throw ;
            }
            
        }
    }
    
}