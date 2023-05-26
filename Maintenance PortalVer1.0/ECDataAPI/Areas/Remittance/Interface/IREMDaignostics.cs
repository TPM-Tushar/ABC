#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IREMDaignostics.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using System.Collections.Generic;
namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IREMDaignostics
    {
        RemitanceDiagnosticsDetailsReqModel RemittanceDiagnosticsDetailsView(string EncryptedID);

        //List<BankTransactionDetailsResponseModel> BankTransactionDetailsList(String EncryptedID);
        List<BankTransactionDetailsResponseModel> GetBankTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);
        List<RemittanceDetailsResponseModel> GetRemittanceDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);
        List<ChallanDetailsResponseModel> GetChallanDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);
        List<DoubleVerificationDetailsResponseModel> GetDoubleVerificationDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);
        List<BankTransactionAmountDetailsResponseModel> GetBankTransactionAmountDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);
        List<ChallanMatrixTransactionDetailsResponseModel> GetChallanMatrixTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel);


    }
}