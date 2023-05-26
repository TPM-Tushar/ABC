#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   REMDaignosticsSummaryDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using CustomModels.Security;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class REMDaignosticsSummaryDAL : IREMDaignosticsSummary, IDisposable
    {
        #region Properties
        //private Dictionary<String, String> decryptedParameters = null;
        //private String[] encryptedParameters = null;
        private KaveriEntities dbContext = null;
        #endregion

        /// <summary>
        /// GetOfficeListSummary
        /// </summary>
        /// <returns></returns>
        public RemittanceOfficeListSummaryModel GetOfficeListSummary()
        {
            try
            {
                dbContext = new KaveriEntities();
                RemittanceOfficeListSummaryModel model = new RemittanceOfficeListSummaryModel(); // Return model

                model.SROfficeDetailList = new List<RemittanceOfficeDetailModel>(); // list initialize
                model.DROfficeDetailList = new List<RemittanceOfficeDetailModel>(); // list initialize

                RemittanceOfficeDetailModel sROfficeDetailModel = null;
                RemittanceOfficeDetailModel dROfficeDetailModel = null;

                var SROfficeLIST = dbContext.SROMaster.ToList();

                var SROOfficeWiseList = dbContext.USP_REM_RemittanceDetailsSROOfficeWiseSummary().ToList();
                var DROOfficeWiseList = dbContext.USP_REM_RemittanceDetialsDROOfficeWiseSummary().ToList();

                RemittanceOfficeListSummaryModel remittanceOfficeListSummaryModel = new RemittanceOfficeListSummaryModel();
                remittanceOfficeListSummaryModel.SROfficeDetailList = new List<RemittanceOfficeDetailModel>();
                foreach (var item in SROOfficeWiseList)
                {

                    sROfficeDetailModel = new RemittanceOfficeDetailModel();
                    sROfficeDetailModel.EncryptedID = URLEncrypt.EncryptParameters(new String[] {
                                                                    "OfficeID="+item.srocode , "IsDro="+item.IsDRO
                                                                                });
                    sROfficeDetailModel.EncryptedIDForReceiptsNotRemitted = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotRemitted,"IsDro="+item.IsDRO,"OfficeID="+item.srocode
                                                                                });
                    sROfficeDetailModel.EncryptedIDChallanNotGenerated = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ChallanNotGenerated , "IsDro="+item.IsDRO,"OfficeID="+item.srocode
                                                                                });
                    sROfficeDetailModel.EncryptedIDForBankReconcilationPending = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.BankReconcealationPending, "IsDro="+item.IsDRO,"OfficeID="+item.srocode
                                                                                });
                    sROfficeDetailModel.EncryptedIDForLinkForReceiptsNotSubmittedForRemittance = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotSubmittedForRemittance, "IsDro="+item.IsDRO,"OfficeID="+item.srocode
                                                                                });

                    sROfficeDetailModel.TotalReceiptsGenerated = Convert.ToInt32(item.BankTranCount);
                    sROfficeDetailModel.TotalChallanGenerated = Convert.ToInt32(item.ChallanTranCount);
                    sROfficeDetailModel.TotalPaymentsReconciled = Convert.ToInt32(item.VerificationCount);
                    sROfficeDetailModel.TotalReceiptsRemitted = Convert.ToInt32(item.RemitCount);
                    sROfficeDetailModel.SROCode = item.srocode;
                    sROfficeDetailModel.SROfficeName = item.sronamee;
                    sROfficeDetailModel.SubmittedForRemittance = Convert.ToInt32(item.SubmittedCount);
                    sROfficeDetailModel.ReceiptsNotSubmittedForRemittance = sROfficeDetailModel.TotalReceiptsGenerated - sROfficeDetailModel.SubmittedForRemittance;
                   
                    //if Difference is greater than zero then only link will be shown...
                    sROfficeDetailModel.LinkForChallanNotGenerated = (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalChallanGenerated) > 0 ? "<a href='#' onclick=RedirectToOfficeDetailsPage('" + sROfficeDetailModel.EncryptedIDChallanNotGenerated + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalChallanGenerated) + "</a>" : (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalChallanGenerated).ToString();
                    sROfficeDetailModel.LinkForReceiptsNotRemitted = (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalReceiptsRemitted) > 0 ? "<a href='#' onclick=RedirectToOfficeDetailsPage('" + sROfficeDetailModel.EncryptedIDForReceiptsNotRemitted + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalReceiptsRemitted) + "</a>" : (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalReceiptsRemitted).ToString();
                    sROfficeDetailModel.LinkForBankReconcilationPending = (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalPaymentsReconciled) > 0?"<a href='#' onclick=RedirectToOfficeDetailsPage('" + sROfficeDetailModel.EncryptedIDForBankReconcilationPending + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalPaymentsReconciled) + "</a>": (sROfficeDetailModel.SubmittedForRemittance - sROfficeDetailModel.TotalPaymentsReconciled).ToString();
                    sROfficeDetailModel.LinkForReceiptsNotSubmittedForRemittance= (sROfficeDetailModel.TotalReceiptsGenerated - sROfficeDetailModel.SubmittedForRemittance) > 0 ? ("<a href='#' onclick=RedirectToOfficeDetailsPage('" + sROfficeDetailModel.EncryptedIDForLinkForReceiptsNotSubmittedForRemittance + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (sROfficeDetailModel.TotalReceiptsGenerated - sROfficeDetailModel.SubmittedForRemittance) + "</a>") : (sROfficeDetailModel.TotalReceiptsGenerated - sROfficeDetailModel.SubmittedForRemittance).ToString();

                    //For SRO Name Link
                    sROfficeDetailModel.BtnToRedirectToDetails = "<a href='#' onclick=RedirectToOfficeDetailsPage('" + sROfficeDetailModel.EncryptedIDForBankReconcilationPending + "');style='color: #449070; font-size: 14px;font-weight: bold;'>" + sROfficeDetailModel.SROfficeName + "</a>";

                    model.SROfficeDetailList.Add(sROfficeDetailModel);


                }


                foreach (var item in DROOfficeWiseList)
                {
                    dROfficeDetailModel = new RemittanceOfficeDetailModel();

                    dROfficeDetailModel.EncryptedIDForReceiptsNotRemitted = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotRemitted,"IsDro="+true,"OfficeID="+item.DistrictCode
                                                                                });
                    dROfficeDetailModel.EncryptedIDChallanNotGenerated = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ChallanNotGenerated , "IsDro="+true,"OfficeID="+item.DistrictCode
                                                                                });
                    dROfficeDetailModel.EncryptedIDForBankReconcilationPending = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.BankReconcealationPending, "IsDro="+true,"OfficeID="+item.DistrictCode
                                                                                });


                    dROfficeDetailModel.EncryptedIDForLinkForReceiptsNotSubmittedForRemittance = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionStatus="+ (short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotSubmittedForRemittance, "IsDro="+true,"OfficeID="+item.DistrictCode
                                                                                });
                    dROfficeDetailModel.DROfficeName = item.DistrictNameE;
                    dROfficeDetailModel.TotalReceiptsRemitted = Convert.ToInt32(item.RemitCount);
                    dROfficeDetailModel.TotalReceiptsGenerated = Convert.ToInt32(item.BankTranCount);
                    dROfficeDetailModel.TotalChallanGenerated = Convert.ToInt32(item.ChallanTranCount);
                    dROfficeDetailModel.TotalPaymentsReconciled = Convert.ToInt32(item.VerificationCount);
                    dROfficeDetailModel.SubmittedForRemittance = Convert.ToInt32(item.SubmittedCount);
                    dROfficeDetailModel.ReceiptsNotSubmittedForRemittance = dROfficeDetailModel.TotalReceiptsGenerated - dROfficeDetailModel.SubmittedForRemittance;
                    dROfficeDetailModel.DROCode = item.DistrictCode;
                    dROfficeDetailModel.DROfficeName = item.DistrictNameE;
                    //if Difference is greater than zero then only link will be shown...
                    dROfficeDetailModel.LinkForChallanNotGenerated = (dROfficeDetailModel.SubmittedForRemittance-dROfficeDetailModel.TotalChallanGenerated ) > 0 ? "<a href='#' onclick=RedirectToOfficeDetailsPage('" + dROfficeDetailModel.EncryptedIDChallanNotGenerated + "'); style='color: #14673a; font-size: 14px;font-weight: bold;' data-toggle='tooltip' title='Challan not generated'>" + (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalChallanGenerated) + "</a>" : (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalChallanGenerated).ToString();
                    dROfficeDetailModel.LinkForReceiptsNotRemitted = (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalReceiptsRemitted) > 0 ? "<a href='#' title='Challan not generated' onclick=RedirectToOfficeDetailsPage('" + dROfficeDetailModel.EncryptedIDForReceiptsNotRemitted + "'); style='color: #14673; font-size: 14px;font-weight: bold;'>" + (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalReceiptsRemitted) + "</a>" : (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalReceiptsRemitted).ToString();
                    dROfficeDetailModel.LinkForBankReconcilationPending = (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalPaymentsReconciled) > 0 ? "<a href='#' onclick=RedirectToOfficeDetailsPage('" + dROfficeDetailModel.EncryptedIDForBankReconcilationPending + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalPaymentsReconciled) + "</a>" : (dROfficeDetailModel.SubmittedForRemittance - dROfficeDetailModel.TotalPaymentsReconciled).ToString();
                    dROfficeDetailModel.LinkForReceiptsNotSubmittedForRemittance = (dROfficeDetailModel.TotalReceiptsGenerated - dROfficeDetailModel.SubmittedForRemittance) > 0 ? ("<a href='#' onclick=RedirectToOfficeDetailsPage('" + dROfficeDetailModel.EncryptedIDForLinkForReceiptsNotSubmittedForRemittance + "'); style='color: #14673a; font-size: 14px;font-weight: bold;'>" + (dROfficeDetailModel.TotalReceiptsGenerated - dROfficeDetailModel.SubmittedForRemittance) + "</a>") : (dROfficeDetailModel.TotalReceiptsGenerated - dROfficeDetailModel.SubmittedForRemittance).ToString();
                    model.DROfficeDetailList.Add(dROfficeDetailModel);

                }

                return model;
            }
            catch (Exception ) { throw ; }
            finally { if (dbContext != null) dbContext.Dispose(); }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}