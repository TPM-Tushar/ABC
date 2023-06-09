﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace ECDataAPI.AnywhereCCService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PreRegCCServiceSoap", Namespace="http://tempuri.org/")]
    public partial class PreRegCCService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetCCFileNameOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPageCountIsReadableOperationCompleted;
        
        private System.Threading.SendOrPostCallback TestOperationCompleted;
        
        private System.Threading.SendOrPostCallback TestWebserviceOperationCompleted;
        
        private System.Threading.SendOrPostCallback DownloadCCChunkOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetFileSizeOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetFilesListOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeleteFileOperationCompleted;
        
        private System.Threading.SendOrPostCallback CheckIfCCFileExistOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public PreRegCCService() {
            this.Url = global::ECDataAPI.Properties.Settings.Default.ECDataAPI_AnywhereCCService_PreRegCCService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetCCFileNameCompletedEventHandler GetCCFileNameCompleted;
        
        /// <remarks/>
        public event GetPageCountIsReadableCompletedEventHandler GetPageCountIsReadableCompleted;
        
        /// <remarks/>
        public event TestCompletedEventHandler TestCompleted;
        
        /// <remarks/>
        public event TestWebserviceCompletedEventHandler TestWebserviceCompleted;
        
        /// <remarks/>
        public event DownloadCCChunkCompletedEventHandler DownloadCCChunkCompleted;
        
        /// <remarks/>
        public event GetFileSizeCompletedEventHandler GetFileSizeCompleted;
        
        /// <remarks/>
        public event GetFilesListCompletedEventHandler GetFilesListCompleted;
        
        /// <remarks/>
        public event DeleteFileCompletedEventHandler DeleteFileCompleted;
        
        /// <remarks/>
        public event CheckIfCCFileExistCompletedEventHandler CheckIfCCFileExistCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCCFileName", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public CCFileDetailsModel GetCCFileName(string finalRegistratioNumber, int sroCode, int documentType, bool isSignedCopy, string name, string address, ref string errorMessage) {
            object[] results = this.Invoke("GetCCFileName", new object[] {
                        finalRegistratioNumber,
                        sroCode,
                        documentType,
                        isSignedCopy,
                        name,
                        address,
                        errorMessage});
            errorMessage = ((string)(results[1]));
            return ((CCFileDetailsModel)(results[0]));
        }
        
        /// <remarks/>
        public void GetCCFileNameAsync(string finalRegistratioNumber, int sroCode, int documentType, bool isSignedCopy, string name, string address, string errorMessage) {
            this.GetCCFileNameAsync(finalRegistratioNumber, sroCode, documentType, isSignedCopy, name, address, errorMessage, null);
        }
        
        /// <remarks/>
        public void GetCCFileNameAsync(string finalRegistratioNumber, int sroCode, int documentType, bool isSignedCopy, string name, string address, string errorMessage, object userState) {
            if ((this.GetCCFileNameOperationCompleted == null)) {
                this.GetCCFileNameOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCCFileNameOperationCompleted);
            }
            this.InvokeAsync("GetCCFileName", new object[] {
                        finalRegistratioNumber,
                        sroCode,
                        documentType,
                        isSignedCopy,
                        name,
                        address,
                        errorMessage}, this.GetCCFileNameOperationCompleted, userState);
        }
        
        private void OnGetCCFileNameOperationCompleted(object arg) {
            if ((this.GetCCFileNameCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCCFileNameCompleted(this, new GetCCFileNameCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetPageCountIsReadable", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetPageCountIsReadable(string finalRegistratioNumber, int sroCode, int documentType) {
            object[] results = this.Invoke("GetPageCountIsReadable", new object[] {
                        finalRegistratioNumber,
                        sroCode,
                        documentType});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetPageCountIsReadableAsync(string finalRegistratioNumber, int sroCode, int documentType) {
            this.GetPageCountIsReadableAsync(finalRegistratioNumber, sroCode, documentType, null);
        }
        
        /// <remarks/>
        public void GetPageCountIsReadableAsync(string finalRegistratioNumber, int sroCode, int documentType, object userState) {
            if ((this.GetPageCountIsReadableOperationCompleted == null)) {
                this.GetPageCountIsReadableOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPageCountIsReadableOperationCompleted);
            }
            this.InvokeAsync("GetPageCountIsReadable", new object[] {
                        finalRegistratioNumber,
                        sroCode,
                        documentType}, this.GetPageCountIsReadableOperationCompleted, userState);
        }
        
        private void OnGetPageCountIsReadableOperationCompleted(object arg) {
            if ((this.GetPageCountIsReadableCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPageCountIsReadableCompleted(this, new GetPageCountIsReadableCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Test", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Test() {
            this.Invoke("Test", new object[0]);
        }
        
        /// <remarks/>
        public void TestAsync() {
            this.TestAsync(null);
        }
        
        /// <remarks/>
        public void TestAsync(object userState) {
            if ((this.TestOperationCompleted == null)) {
                this.TestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestOperationCompleted);
            }
            this.InvokeAsync("Test", new object[0], this.TestOperationCompleted, userState);
        }
        
        private void OnTestOperationCompleted(object arg) {
            if ((this.TestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TestWebservice", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool TestWebservice() {
            object[] results = this.Invoke("TestWebservice", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void TestWebserviceAsync() {
            this.TestWebserviceAsync(null);
        }
        
        /// <remarks/>
        public void TestWebserviceAsync(object userState) {
            if ((this.TestWebserviceOperationCompleted == null)) {
                this.TestWebserviceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestWebserviceOperationCompleted);
            }
            this.InvokeAsync("TestWebservice", new object[0], this.TestWebserviceOperationCompleted, userState);
        }
        
        private void OnTestWebserviceOperationCompleted(object arg) {
            if ((this.TestWebserviceCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestWebserviceCompleted(this, new TestWebserviceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DownloadCCChunk", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] DownloadCCChunk(string FileName, long Offset, int BufferSize, ref string errorMesage) {
            object[] results = this.Invoke("DownloadCCChunk", new object[] {
                        FileName,
                        Offset,
                        BufferSize,
                        errorMesage});
            errorMesage = ((string)(results[1]));
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void DownloadCCChunkAsync(string FileName, long Offset, int BufferSize, string errorMesage) {
            this.DownloadCCChunkAsync(FileName, Offset, BufferSize, errorMesage, null);
        }
        
        /// <remarks/>
        public void DownloadCCChunkAsync(string FileName, long Offset, int BufferSize, string errorMesage, object userState) {
            if ((this.DownloadCCChunkOperationCompleted == null)) {
                this.DownloadCCChunkOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDownloadCCChunkOperationCompleted);
            }
            this.InvokeAsync("DownloadCCChunk", new object[] {
                        FileName,
                        Offset,
                        BufferSize,
                        errorMesage}, this.DownloadCCChunkOperationCompleted, userState);
        }
        
        private void OnDownloadCCChunkOperationCompleted(object arg) {
            if ((this.DownloadCCChunkCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DownloadCCChunkCompleted(this, new DownloadCCChunkCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFileSize", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long GetFileSize(string FileName) {
            object[] results = this.Invoke("GetFileSize", new object[] {
                        FileName});
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public void GetFileSizeAsync(string FileName) {
            this.GetFileSizeAsync(FileName, null);
        }
        
        /// <remarks/>
        public void GetFileSizeAsync(string FileName, object userState) {
            if ((this.GetFileSizeOperationCompleted == null)) {
                this.GetFileSizeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileSizeOperationCompleted);
            }
            this.InvokeAsync("GetFileSize", new object[] {
                        FileName}, this.GetFileSizeOperationCompleted, userState);
        }
        
        private void OnGetFileSizeOperationCompleted(object arg) {
            if ((this.GetFileSizeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileSizeCompleted(this, new GetFileSizeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFilesList", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetFilesList() {
            object[] results = this.Invoke("GetFilesList", new object[0]);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetFilesListAsync() {
            this.GetFilesListAsync(null);
        }
        
        /// <remarks/>
        public void GetFilesListAsync(object userState) {
            if ((this.GetFilesListOperationCompleted == null)) {
                this.GetFilesListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFilesListOperationCompleted);
            }
            this.InvokeAsync("GetFilesList", new object[0], this.GetFilesListOperationCompleted, userState);
        }
        
        private void OnGetFilesListOperationCompleted(object arg) {
            if ((this.GetFilesListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFilesListCompleted(this, new GetFilesListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DeleteFile", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DeleteFile(string fileName) {
            object[] results = this.Invoke("DeleteFile", new object[] {
                        fileName});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void DeleteFileAsync(string fileName) {
            this.DeleteFileAsync(fileName, null);
        }
        
        /// <remarks/>
        public void DeleteFileAsync(string fileName, object userState) {
            if ((this.DeleteFileOperationCompleted == null)) {
                this.DeleteFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteFileOperationCompleted);
            }
            this.InvokeAsync("DeleteFile", new object[] {
                        fileName}, this.DeleteFileOperationCompleted, userState);
        }
        
        private void OnDeleteFileOperationCompleted(object arg) {
            if ((this.DeleteFileCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteFileCompleted(this, new DeleteFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckIfCCFileExist", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public CCFileExistResModel CheckIfCCFileExist(CCFileExistReqModel cCFileExistReqModel) {
            object[] results = this.Invoke("CheckIfCCFileExist", new object[] {
                        cCFileExistReqModel});
            return ((CCFileExistResModel)(results[0]));
        }
        
        /// <remarks/>
        public void CheckIfCCFileExistAsync(CCFileExistReqModel cCFileExistReqModel) {
            this.CheckIfCCFileExistAsync(cCFileExistReqModel, null);
        }
        
        /// <remarks/>
        public void CheckIfCCFileExistAsync(CCFileExistReqModel cCFileExistReqModel, object userState) {
            if ((this.CheckIfCCFileExistOperationCompleted == null)) {
                this.CheckIfCCFileExistOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckIfCCFileExistOperationCompleted);
            }
            this.InvokeAsync("CheckIfCCFileExist", new object[] {
                        cCFileExistReqModel}, this.CheckIfCCFileExistOperationCompleted, userState);
        }
        
        private void OnCheckIfCCFileExistOperationCompleted(object arg) {
            if ((this.CheckIfCCFileExistCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckIfCCFileExistCompleted(this, new CheckIfCCFileExistCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CCFileDetailsModel {
        
        private string cCFileNameField;
        
        private bool isTiffCurruptedField;
        
        private long documentIdField;
        
        private string waterMarkField;
        
        private bool isFruitsCCField;
        
        /// <remarks/>
        public string CCFileName {
            get {
                return this.cCFileNameField;
            }
            set {
                this.cCFileNameField = value;
            }
        }
        
        /// <remarks/>
        public bool IsTiffCurrupted {
            get {
                return this.isTiffCurruptedField;
            }
            set {
                this.isTiffCurruptedField = value;
            }
        }
        
        /// <remarks/>
        public long DocumentId {
            get {
                return this.documentIdField;
            }
            set {
                this.documentIdField = value;
            }
        }
        
        /// <remarks/>
        public string WaterMark {
            get {
                return this.waterMarkField;
            }
            set {
                this.waterMarkField = value;
            }
        }
        
        /// <remarks/>
        public bool IsFruitsCC {
            get {
                return this.isFruitsCCField;
            }
            set {
                this.isFruitsCCField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CCFileExistResDetailModel {
        
        private long documentIDField;
        
        private int sroCodeField;
        
        private string fileNameField;
        
        private bool isFileExistField;
        
        private bool isFileReadableField;
        
        private int pageCountField;
        
        private bool dataExistField;
        
        private string filePathField;
        
        /// <remarks/>
        public long DocumentID {
            get {
                return this.documentIDField;
            }
            set {
                this.documentIDField = value;
            }
        }
        
        /// <remarks/>
        public int SroCode {
            get {
                return this.sroCodeField;
            }
            set {
                this.sroCodeField = value;
            }
        }
        
        /// <remarks/>
        public string FileName {
            get {
                return this.fileNameField;
            }
            set {
                this.fileNameField = value;
            }
        }
        
        /// <remarks/>
        public bool IsFileExist {
            get {
                return this.isFileExistField;
            }
            set {
                this.isFileExistField = value;
            }
        }
        
        /// <remarks/>
        public bool IsFileReadable {
            get {
                return this.isFileReadableField;
            }
            set {
                this.isFileReadableField = value;
            }
        }
        
        /// <remarks/>
        public int PageCount {
            get {
                return this.pageCountField;
            }
            set {
                this.pageCountField = value;
            }
        }
        
        /// <remarks/>
        public bool DataExist {
            get {
                return this.dataExistField;
            }
            set {
                this.dataExistField = value;
            }
        }
        
        /// <remarks/>
        public string FilePath {
            get {
                return this.filePathField;
            }
            set {
                this.filePathField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CCFileExistResModel {
        
        private CCFileExistResDetailModel[] cCFileDetailListField;
        
        private string responseMsgField;
        
        private string responseStatusField;
        
        /// <remarks/>
        public CCFileExistResDetailModel[] CCFileDetailList {
            get {
                return this.cCFileDetailListField;
            }
            set {
                this.cCFileDetailListField = value;
            }
        }
        
        /// <remarks/>
        public string ResponseMsg {
            get {
                return this.responseMsgField;
            }
            set {
                this.responseMsgField = value;
            }
        }
        
        /// <remarks/>
        public string ResponseStatus {
            get {
                return this.responseStatusField;
            }
            set {
                this.responseStatusField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CCFileExistReqDetailModel {
        
        private long documentIDField;
        
        private int sroCodeField;
        
        private string finalRegistrationNoField;
        
        private int documentTypeIDField;
        
        private CCFileDetailsBy cCFileDetailsByField;
        
        /// <remarks/>
        public long DocumentID {
            get {
                return this.documentIDField;
            }
            set {
                this.documentIDField = value;
            }
        }
        
        /// <remarks/>
        public int SroCode {
            get {
                return this.sroCodeField;
            }
            set {
                this.sroCodeField = value;
            }
        }
        
        /// <remarks/>
        public string FinalRegistrationNo {
            get {
                return this.finalRegistrationNoField;
            }
            set {
                this.finalRegistrationNoField = value;
            }
        }
        
        /// <remarks/>
        public int DocumentTypeID {
            get {
                return this.documentTypeIDField;
            }
            set {
                this.documentTypeIDField = value;
            }
        }
        
        /// <remarks/>
        public CCFileDetailsBy CCFileDetailsBy {
            get {
                return this.cCFileDetailsByField;
            }
            set {
                this.cCFileDetailsByField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum CCFileDetailsBy {
        
        /// <remarks/>
        FileExist,
        
        /// <remarks/>
        FileReadable,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CCFileExistReqModel {
        
        private CCFileExistReqDetailModel[] cCFileExistReqDetailListField;
        
        /// <remarks/>
        public CCFileExistReqDetailModel[] CCFileExistReqDetailList {
            get {
                return this.cCFileExistReqDetailListField;
            }
            set {
                this.cCFileExistReqDetailListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void GetCCFileNameCompletedEventHandler(object sender, GetCCFileNameCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCCFileNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetCCFileNameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public CCFileDetailsModel Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((CCFileDetailsModel)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string errorMessage {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void GetPageCountIsReadableCompletedEventHandler(object sender, GetPageCountIsReadableCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPageCountIsReadableCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPageCountIsReadableCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void TestCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void TestWebserviceCompletedEventHandler(object sender, TestWebserviceCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestWebserviceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TestWebserviceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void DownloadCCChunkCompletedEventHandler(object sender, DownloadCCChunkCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DownloadCCChunkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DownloadCCChunkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string errorMesage {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void GetFileSizeCompletedEventHandler(object sender, GetFileSizeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFileSizeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFileSizeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void GetFilesListCompletedEventHandler(object sender, GetFilesListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFilesListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFilesListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void DeleteFileCompletedEventHandler(object sender, DeleteFileCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeleteFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeleteFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void CheckIfCCFileExistCompletedEventHandler(object sender, CheckIfCCFileExistCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckIfCCFileExistCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckIfCCFileExistCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public CCFileExistResModel Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((CCFileExistResModel)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591