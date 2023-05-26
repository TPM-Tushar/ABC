#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   REMDaignosticsBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System.Collections.Generic;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class REMDaignosticsBAL : IREMDaignostics
    {
        IREMDaignostics misReportsDAL = new REMDaignosticsDAL();

        /// <summary>
        /// RemittanceDiagnosticsDetailsView
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public RemitanceDiagnosticsDetailsReqModel RemittanceDiagnosticsDetailsView(string EncryptedID)
        {
            return misReportsDAL.RemittanceDiagnosticsDetailsView(EncryptedID);
        }

        /// <summary>
        /// GetBankTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<BankTransactionDetailsResponseModel> GetBankTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetBankTransactionDetailsList(WrapperModel);
        }

        /// <summary>
        /// GetRemittanceDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<RemittanceDetailsResponseModel> GetRemittanceDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetRemittanceDetailsList(WrapperModel);
        }

        /// <summary>
        /// GetChallanDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ChallanDetailsResponseModel> GetChallanDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetChallanDetailsList(WrapperModel);
        }

        /// <summary>
        /// GetDoubleVerificationDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<DoubleVerificationDetailsResponseModel> GetDoubleVerificationDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetDoubleVerificationDetailsList(WrapperModel);
        }

        /// <summary>
        /// GetBankTransactionAmountDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<BankTransactionAmountDetailsResponseModel> GetBankTransactionAmountDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetBankTransactionAmountDetailsList(WrapperModel);
        }

        /// <summary>
        /// GetChallanMatrixTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ChallanMatrixTransactionDetailsResponseModel> GetChallanMatrixTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            return misReportsDAL.GetChallanMatrixTransactionDetailsList(WrapperModel);
        }
    }
}