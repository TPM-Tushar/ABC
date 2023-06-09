﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.PaymentGatewayReference {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PaymentGatewayReference.serviceSoap")]
    public interface serviceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/generateEchallanPDF", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet generateEchallanPDF(string demand, string encrypt_XML_data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/generateEchallanPDF", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> generateEchallanPDFAsync(string demand, string encrypt_XML_data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/generateEchallanPDFNew", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet generateEchallanPDFNew(string demand, string encrypt_XML_data, string serviceCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/generateEchallanPDFNew", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> generateEchallanPDFNewAsync(string demand, string encrypt_XML_data, string serviceCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanPaymentStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string eChallanPaymentStatus(string demandno, string encryptedEchallanno);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanPaymentStatus", ReplyAction="*")]
        System.Threading.Tasks.Task<string> eChallanPaymentStatusAsync(string demandno, string encryptedEchallanno);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanSuccessPayments", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string eChallanSuccessPayments(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanSuccessPayments", ReplyAction="*")]
        System.Threading.Tasks.Task<string> eChallanSuccessPaymentsAsync(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanPaymentData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string eChallanPaymentData(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/eChallanPaymentData", ReplyAction="*")]
        System.Threading.Tasks.Task<string> eChallanPaymentDataAsync(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/treasuryReceipts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string treasuryReceipts(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/treasuryReceipts", ReplyAction="*")]
        System.Threading.Tasks.Task<string> treasuryReceiptsAsync(string demandno, string encryptedXMLDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ereceipt", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet ereceipt(string demandno, string enc_echallanno);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ereceipt", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> ereceiptAsync(string demandno, string enc_echallanno);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/echallancount", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet echallancount(string transactiondate, string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/echallancount", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> echallancountAsync(string transactiondate, string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/getGPFName", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string getGPFName(string encryptedGPFnoDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/getGPFName", ReplyAction="*")]
        System.Threading.Tasks.Task<string> getGPFNameAsync(string encryptedGPFnoDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/getPpanName", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string getPpanName(string encryptedPpannoDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/getPpanName", ReplyAction="*")]
        System.Threading.Tasks.Task<string> getPpanNameAsync(string encryptedPpannoDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ValidateRefund", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string ValidateRefund(string encryptedRefundDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ValidateRefund", ReplyAction="*")]
        System.Threading.Tasks.Task<string> ValidateRefundAsync(string encryptedRefundDetails);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface serviceSoapChannel : ECDataAPI.PaymentGatewayReference.serviceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class serviceSoapClient : System.ServiceModel.ClientBase<ECDataAPI.PaymentGatewayReference.serviceSoap>, ECDataAPI.PaymentGatewayReference.serviceSoap {
        
        public serviceSoapClient() {
        }
        
        public serviceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public serviceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public serviceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public serviceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet generateEchallanPDF(string demand, string encrypt_XML_data) {
            return base.Channel.generateEchallanPDF(demand, encrypt_XML_data);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> generateEchallanPDFAsync(string demand, string encrypt_XML_data) {
            return base.Channel.generateEchallanPDFAsync(demand, encrypt_XML_data);
        }
        
        public System.Data.DataSet generateEchallanPDFNew(string demand, string encrypt_XML_data, string serviceCode) {
            return base.Channel.generateEchallanPDFNew(demand, encrypt_XML_data, serviceCode);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> generateEchallanPDFNewAsync(string demand, string encrypt_XML_data, string serviceCode) {
            return base.Channel.generateEchallanPDFNewAsync(demand, encrypt_XML_data, serviceCode);
        }
        
        public string eChallanPaymentStatus(string demandno, string encryptedEchallanno) {
            return base.Channel.eChallanPaymentStatus(demandno, encryptedEchallanno);
        }
        
        public System.Threading.Tasks.Task<string> eChallanPaymentStatusAsync(string demandno, string encryptedEchallanno) {
            return base.Channel.eChallanPaymentStatusAsync(demandno, encryptedEchallanno);
        }
        
        public string eChallanSuccessPayments(string demandno, string encryptedXMLDates) {
            return base.Channel.eChallanSuccessPayments(demandno, encryptedXMLDates);
        }
        
        public System.Threading.Tasks.Task<string> eChallanSuccessPaymentsAsync(string demandno, string encryptedXMLDates) {
            return base.Channel.eChallanSuccessPaymentsAsync(demandno, encryptedXMLDates);
        }
        
        public string eChallanPaymentData(string demandno, string encryptedXMLDates) {
            return base.Channel.eChallanPaymentData(demandno, encryptedXMLDates);
        }
        
        public System.Threading.Tasks.Task<string> eChallanPaymentDataAsync(string demandno, string encryptedXMLDates) {
            return base.Channel.eChallanPaymentDataAsync(demandno, encryptedXMLDates);
        }
        
        public string treasuryReceipts(string demandno, string encryptedXMLDates) {
            return base.Channel.treasuryReceipts(demandno, encryptedXMLDates);
        }
        
        public System.Threading.Tasks.Task<string> treasuryReceiptsAsync(string demandno, string encryptedXMLDates) {
            return base.Channel.treasuryReceiptsAsync(demandno, encryptedXMLDates);
        }
        
        public System.Data.DataSet ereceipt(string demandno, string enc_echallanno) {
            return base.Channel.ereceipt(demandno, enc_echallanno);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> ereceiptAsync(string demandno, string enc_echallanno) {
            return base.Channel.ereceiptAsync(demandno, enc_echallanno);
        }
        
        public System.Data.DataSet echallancount(string transactiondate, string username, string password) {
            return base.Channel.echallancount(transactiondate, username, password);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> echallancountAsync(string transactiondate, string username, string password) {
            return base.Channel.echallancountAsync(transactiondate, username, password);
        }
        
        public string getGPFName(string encryptedGPFnoDetails) {
            return base.Channel.getGPFName(encryptedGPFnoDetails);
        }
        
        public System.Threading.Tasks.Task<string> getGPFNameAsync(string encryptedGPFnoDetails) {
            return base.Channel.getGPFNameAsync(encryptedGPFnoDetails);
        }
        
        public string getPpanName(string encryptedPpannoDetails) {
            return base.Channel.getPpanName(encryptedPpannoDetails);
        }
        
        public System.Threading.Tasks.Task<string> getPpanNameAsync(string encryptedPpannoDetails) {
            return base.Channel.getPpanNameAsync(encryptedPpannoDetails);
        }
        
        public string ValidateRefund(string encryptedRefundDetails) {
            return base.Channel.ValidateRefund(encryptedRefundDetails);
        }
        
        public System.Threading.Tasks.Task<string> ValidateRefundAsync(string encryptedRefundDetails) {
            return base.Channel.ValidateRefundAsync(encryptedRefundDetails);
        }
    }
}
