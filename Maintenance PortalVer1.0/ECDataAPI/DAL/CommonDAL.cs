using CustomModels.Models.Common;
using ECDataAPI.Interface;
using ECDataAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using iTextSharp.text.pdf;
//using Security;
using System.IO;
using System.Security;
using System.Net;
using iTextSharp.text;
using System.Configuration;
using ECDataAPI.Common;
using System.Data.Entity;
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using CustomModels.Security;
using CustomModels.Common;

namespace ECDataAPI.DAL
{
    public class CommonDAL : ICommonInterface, IDisposable
    {
        KaveriEntities dbContext = null;
        String errorMessage = String.Empty;
        // private String[] encryptedParameters = null;
        //  private Dictionary<String, String> decryptedParameters = null;



        /// <summary>
        /// This method returns TRUE if ApiConsumer i.e. Client is authorized.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsAuthorizedClient(ClientAuthenticationModel model)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    return dbContext.UMG_API_Users.Where(X => X.User_Name == model.ApiConsumerUserName && X.Password == model.ApiConsumerPassword).Any();
                }
            }
            catch (Exception)
            {
                //throw ex;
                //log exception here
                return false;
            }

        }



        public MasterDropDownModel FillMasterDropDownModel()
        {
            MasterDropDownModel model = null;

            try
            {
                using (var scope = new TransactionScope())
                {
                    model = new MasterDropDownModel();
                    model.CountryDropDown = new List<SelectListItem>() {
            new SelectListItem
                {
                    Text = "Select",
                    Value = "0",
                },
                new SelectListItem
                {
                    Text = "India",
                    Value = "1",
                },
                new SelectListItem
                {
                    Text = "USA",
                    Value = "2",
                }
            };

                    model.StatesDropDown = new List<SelectListItem>(){
            new SelectListItem{
              Text = "Select",
                    Value = "0"
            }, new SelectListItem{
              Text = "Goa",
                    Value = "1"
            }, new SelectListItem{
              Text = "Bengal",
                    Value = "2"
            },

            };

                    model.DistrictsDropDown = new List<SelectListItem>(){
            new SelectListItem{
              Text = "Select",
                    Value = "0"
            },
             new SelectListItem{
              Text = "Ahmednagar",
                    Value = "1"
            }, new SelectListItem{
              Text = "Aurangabad",
                    Value = "2"
            },

            };


                    model.TitleDropDown = new List<SelectListItem>(){
            new SelectListItem{
              Text = "Select",
                    Value = "0"
            },
             new SelectListItem{
              Text = "MR.",
                    Value = "1"
            }, new SelectListItem{
              Text = "Mrs.",
                    Value = "2"
            },
             new SelectListItem{
              Text = "Miss.",
                    Value = "3"
            },
            };


                    model.ProfessionDropDown = new List<SelectListItem>(){
            new SelectListItem{
              Text = "Select",
                    Value = "0"
            },
             new SelectListItem{
              Text = "Engineer",
                    Value = "1"
            }, new SelectListItem{
              Text = "Doctor",
                    Value = "2"
            },

            };



                    model.GenderDropDown = new List<SelectListItem>() {
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = "Male",
                    Value = "1"
                },new SelectListItem
                {
                    Text = "Female",
                    Value = "2"
                }

                };




                    model.MaritalStatusDropDown = new List<SelectListItem>()

            {
            new SelectListItem
            {
                Text = "Select",
                Value = "0"
            },new SelectListItem
            {
                Text = "Married",
                Value = "1"
            },new SelectListItem
            {
                Text = "Single",
                Value = "2"
            }};

                    scope.Complete();
                    return model;

                }

            }
            catch (Exception)
            {
                throw;
            }


        }


        public bool CheckUserPermissionForRole(short roleID, string area, string controller, string action)
        {
            try
            {
                dbContext = new KaveriEntities();
                var result = (from controllerActionDetails in dbContext.UMG_ControllerActionDetails
                              join actionMappings in dbContext.UMG_RoleActionAuth
                              on controllerActionDetails.CAID equals actionMappings.CAID
                              where actionMappings.RoleID == roleID
                              select new { controllerActionDetails.AreaName, controllerActionDetails.ControllerName, controllerActionDetails.ActionName }).ToList();
                return result.Contains(new { AreaName = area, ControllerName = controller, ActionName = action });
            }
            catch
            {
                throw;
            }
            finally
            {
                if (null != dbContext)
                    dbContext.Dispose();
            }
        }



        ///// <summary>
        ///// This method adds the "Metadata" to the file bytes.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public FileDisplayModel AddMetaDataToDocument(MetaDataFileDetailsModel model, MetaDataInfoModel infoModel)
        //{
        //    PdfReader reader = null;
        //    PdfReader.unethicalreading = true;
        //    byte[] OutputArray = null;

        //    FileDisplayModel objFileDisplayModel = null;
        //    try
        //    {
        //        encryptedParameters = model.EncryptedID.Split('/');
        //        if (!(encryptedParameters.Length == 3))
        //            throw new SecurityException("Url Tempered");
        //        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

        //        objFileDisplayModel = new FileDisplayModel();

        //        String encryptedID = string.Empty;

        //        // Int16 ServiceID = Convert.ToInt16(decryptedParameters["ServiceID"].ToString().Trim());
        //        Int16 officeID = Convert.ToInt16(decryptedParameters["OfficeID"].ToString().Trim());



        //        switch (model.ServiceID)
        //        {
        //            case (short)ECDataAPI.Common.ApiCommonEnum.ServicesProcess.FirmRegistration:
        //                Int64 firmID = Convert.ToInt64(decryptedParameters["FirmID"].ToString().Trim());

        //                objFileDisplayModel.fileBytes = model.FileBytes;
        //                reader = new PdfReader(objFileDisplayModel.fileBytes);
        //                objFileDisplayModel.NumberOfPages = reader.NumberOfPages;
        //                encryptedID = URLEncrypt.EncryptParameters(new String[] { "FirmID=" + firmID, "NumberOfPages=" + objFileDisplayModel.NumberOfPages, "OfficeID=" + officeID, "ServiceID=" + (short)ApiCommonEnum.ServicesProcess.FirmRegistration });
        //                break;

        //            case (short)ECDataAPI.Common.ApiCommonEnum.ServicesProcess.CertifiedCopies:
        //                Int64 ApplicationID = Convert.ToInt64(decryptedParameters["ApplicationID"].ToString().Trim());
        //                reader = new PdfReader(model.FileBytes);
        //                objFileDisplayModel.NumberOfPages = reader.NumberOfPages;
        //                encryptedID = URLEncrypt.EncryptParameters(new String[] { "ApplicationID=" + ApplicationID, "NumberOfPages=" + objFileDisplayModel.NumberOfPages, "OfficeID=" + officeID, "ServiceID=" + (short)ApiCommonEnum.ServicesProcess.CertifiedCopies });
        //                break;
        //        }

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (PdfStamper stamper = new PdfStamper(reader, ms))
        //            {
        //                Dictionary<String, String> info = reader.Info;

        //                if (info.ContainsKey("Title"))
        //                    info["Title"] = infoModel.Author;
        //                else
        //                    info.Add("Title", infoModel.Author);



        //                if (info.ContainsKey("DocumentKey"))
        //                    info["DocumentKey"] = encryptedID;
        //                else
        //                    info.Add("DocumentKey", encryptedID);

        //                if (info.ContainsKey("Creator"))
        //                    info["Creator"] = infoModel.Author;
        //                else
        //                    info.Add("Creator", infoModel.Author);

        //                if (info.ContainsKey("Author"))
        //                    info["Author"] = infoModel.Author;
        //                else
        //                    info.Add("Author", infoModel.Author);

        //                stamper.MoreInfo = info;
        //                stamper.Close();
        //            }
        //            OutputArray = ms.ToArray();

        //        }
        //        if (OutputArray != null)
        //            objFileDisplayModel.fileBytes = OutputArray;


        //        return objFileDisplayModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}



        ///// <summary>
        ///// This method adds the "Metadata" to the file bytes.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public FileDisplayModel AddMetaDataToDocument(MetaDataFileDetailsModel model, MetaDataInfoModel infoModel)
        //{
        //    PdfReader reader = null;
        //    PdfReader.unethicalreading = true;
        //    byte[] OutputArray = null;

        //    FileDisplayModel objFileDisplayModel = null;
        //    try
        //    {
        //        encryptedParameters = model.EncryptedID.Split('/');
        //        if (!(encryptedParameters.Length == 3))
        //            throw new SecurityException("Url Tempered");
        //        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

        //        objFileDisplayModel = new FileDisplayModel();

        //        String encryptedID = string.Empty;

        //        // Int16 ServiceID = Convert.ToInt16(decryptedParameters["ServiceID"].ToString().Trim());
        //        Int16 officeID = Convert.ToInt16(decryptedParameters["OfficeID"].ToString().Trim());



        //        switch (model.ServiceID)
        //        {
        //            case (short)ECDataAPI.Common.ApiCommonEnum.ServicesProcess.FirmRegistration:
        //                Int64 firmID = Convert.ToInt64(decryptedParameters["FirmID"].ToString().Trim());

        //                objFileDisplayModel.fileBytes = model.FileBytes;
        //                reader = new PdfReader(objFileDisplayModel.fileBytes);
        //                objFileDisplayModel.NumberOfPages = reader.NumberOfPages;
        //                encryptedID = URLEncrypt.EncryptParameters(new String[] { "FirmID=" + firmID, "NumberOfPages=" + objFileDisplayModel.NumberOfPages, "OfficeID=" + officeID, "ServiceID=" + (short)ApiCommonEnum.ServicesProcess.FirmRegistration, "FirmApplicationType=" + (short)ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling });
        //                break;



        //            case (short)ECDataAPI.Common.ApiCommonEnum.ServicesProcess.CertifiedCopies:
        //                Int64 ApplicationID = Convert.ToInt64(decryptedParameters["ApplicationID"].ToString().Trim());
        //                reader = new PdfReader(model.FileBytes);
        //                objFileDisplayModel.NumberOfPages = reader.NumberOfPages;
        //                encryptedID = URLEncrypt.EncryptParameters(new String[] { "ApplicationID=" + ApplicationID, "NumberOfPages=" + objFileDisplayModel.NumberOfPages, "OfficeID=" + officeID, "ServiceID=" + (short)ApiCommonEnum.ServicesProcess.CertifiedCopies });
        //                break;
        //        }

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (PdfStamper stamper = new PdfStamper(reader, ms))
        //            {
        //                Dictionary<String, String> info = reader.Info;

        //                if (info.ContainsKey("Title"))
        //                    info["Title"] = infoModel.Author;
        //                else
        //                    info.Add("Title", infoModel.Author);



        //                if (info.ContainsKey("DocumentKey"))
        //                    info["DocumentKey"] = encryptedID;
        //                else
        //                    info.Add("DocumentKey", encryptedID);

        //                if (info.ContainsKey("Creator"))
        //                    info["Creator"] = infoModel.Author;
        //                else
        //                    info.Add("Creator", infoModel.Author);

        //                if (info.ContainsKey("Author"))
        //                    info["Author"] = infoModel.Author;
        //                else
        //                    info.Add("Author", infoModel.Author);

        //                stamper.MoreInfo = info;
        //                stamper.Close();
        //            }
        //            OutputArray = ms.ToArray();

        //        }
        //        if (OutputArray != null)
        //            objFileDisplayModel.fileBytes = OutputArray;


        //        return objFileDisplayModel;
        //    }
        //    catch (Exception )
        //    {
        //        throw ;
        //    }

        //}


        ///// <summary>
        ///// This method returns "FileDisplayModel" obj. which contains byte[] of SSRS Report with added MetaData.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public FileDisplayModel GetSSRReportWithMetaData(SSRReportToBytesModel model)
        //{
        //    try
        //    {
        //        FileDisplayModel responseModel = new FileDisplayModel();


        //        ApiCommonFunctions commonFun = new ApiCommonFunctions();
        //        MetaDataFileDetailsModel metadataObj = new MetaDataFileDetailsModel();

        //        //******************** To convert SSRS report into byte[]. *****************************
        //        metadataObj.FileBytes = commonFun.ConvertSSRReportToBytes(model);
        //        //  model.FileBytes = filrbytes;
        //        metadataObj.EncryptedID = model.EncryptedID;
        //        metadataObj.ServiceID = model.ServiceID;
        //        //******************* To specify which Metadata need to be added  ************************
        //        MetaDataInfoModel infoModel = new MetaDataInfoModel();

        //        infoModel.Author = "Goa Govt.";
        //        infoModel.Creator = "GAURI Online Service";
        //        infoModel.Title = "Firm Certificate";


        //        //************************ To add metadata to file. ****************************
        //        responseModel = new CommonDAL().AddMetaDataToDocument(metadataObj, infoModel);


        //        return responseModel;
        //    }
        //    catch (Exception )
        //    {
        //        throw ;
        //    }
        //}



        ///// <summary>
        ///// This method returns empty String if document(Digitally sign) is uploaded successfully.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public string UploadSignedDocument(FileDisplayModel model)
        //{


        //    bool IsUploaded = false;
        //    Int16 NoOfPages = 0;
        //    string UploadPath = ConfigurationManager.AppSettings["FirmCertificatesUploadPath"];
        //    string responseMessage = null;

        //    PdfReader reader = new PdfReader(model.fileBytes);
        //    // ApiCommonFunctions.FlattenSignature(model.fileBytes);
        ////    string retuHash = ApiCommonFunctions.GetHashOfDocument(model.fileBytes);
        //    //ApiCommonFunctions.WriteTofile("HashOfDocWhileUpld:::" + retuHash);

        //    String[] encryptedParameters = null;
        //    Dictionary<String, String> decryptedParameters = null;

        //    string FileServerPath = string.Empty;

        //    try
        //    {
        //        encryptedParameters = model.EncryptedID.Split('/');
        //        if (!(encryptedParameters.Length == 3))
        //            throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");

        //        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

        //        // string DocumentReportNumber = Convert.ToString(decryptedParameters["EncryptedDocumentKey"].ToString().Trim());
        //        Int16 OfficeID = Convert.ToInt16(decryptedParameters["OfficeID"].ToString().Trim());
        //        Int64 FirmID = Convert.ToInt64(decryptedParameters["FirmID"].ToString().Trim());
        //        Int64 UserID = Convert.ToInt64(decryptedParameters["UserID"].ToString().Trim());
        //        Int64 ResourceID = Convert.ToInt64(decryptedParameters["ResourceID"].ToString().Trim());
        //        Int64 TokenID = Convert.ToInt64(decryptedParameters["TokenID"].ToString().Trim());
        //        Int64 ApplicationID = Convert.ToInt64(decryptedParameters["ApplicationID"].ToString().Trim());
        //        Int16 ServiceType = Convert.ToInt16(decryptedParameters["ServiceType"].ToString().Trim());
        //        Int16 ProcessType = Convert.ToInt16(decryptedParameters["ProcessType"].ToString().Trim());
        //        Int16 ScanProcessId = Convert.ToInt16(decryptedParameters["ScanProcessId"].ToString().Trim());
        //        Int64 AmendmentID = Convert.ToInt64(decryptedParameters["AmendmentID"].ToString().Trim());
        //        Int64 DissolutionID = Convert.ToInt64(decryptedParameters["DissolutionID"].ToString().Trim());

        //        //  Int16 ScanProcessId = Convert.ToInt16(decryptedParameters["ScanProcessId"].ToString().Trim());


        //        dbContext = new KaveriEntities();
        //        Dictionary<String, String> info = reader.Info;

        //        long filesize = reader.FileLength;
        //        string encryptedDocumentKey = string.Empty;

        //        if (info.ContainsKey("DocumentKey"))
        //        {
        //            encryptedDocumentKey = reader.Info["DocumentKey"];
        //            try
        //            {
        //                encryptedParameters = encryptedDocumentKey.Split('/');
        //                if (!(encryptedParameters.Length == 3))
        //                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");

        //                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
        //                Int16 officeID = Convert.ToInt16(decryptedParameters["OfficeID"].ToString().Trim());
        //                Int64 firmID = Convert.ToInt64(decryptedParameters["FirmID"].ToString().Trim());
        //                Int16 NumberOfPages = Convert.ToInt16(decryptedParameters["NumberOfPages"].ToString().Trim());
        //                NoOfPages = NumberOfPages;

        //                AcroFields af = reader.AcroFields;

        //                List<string> names = af.GetSignatureNames();
        //                if (names.Count < 1)
        //                {
        //                    return "This document is not digitally signed.";
        //                }
        //                else
        //                {
        //                    FRM_FirmDetails firm = dbContext.FRM_FirmDetails.Where(m => m.FirmID == firmID).FirstOrDefault();// (byte)ApiCommonEnum.DocumentStatusTypes.DigitallySigned).Count()// calculate if frm db after function calls
        //                    firm.DocumentStatusID = (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms;
        //                    dbContext.Entry(firm).State = EntityState.Modified;
        //                    dbContext.SaveChanges();
        //                }
        //                string name = (string)names[0];
        //                iTextSharp.text.pdf.security.PdfPKCS7 pk = af.VerifySignature(name);

        //                //Getting SignerName
        //                string SignerName = pk.SignName;

        //                //****************  Uncomment for "Signer Name" validation ************************

        //                UMG_UserProfile userModel = dbContext.UMG_UserProfile.Where(x => x.UserID == UserID).FirstOrDefault();
        //                string SRName = userModel.FirstName + " " + userModel.LastName;

        //                if (!(SignerName.ToLower()).Equals(SRName.ToLower()))
        //                {
        //                    return "Firm certificate expected to be signed by <b>"+"' " + SRName + " '"+"</b>";
        //                }
        //                reader.Close();
        //            }
        //            catch (Exception )
        //            {
        //                throw ;
        //            }

        //            //------------- To get directory structure to store uploaded documents -----------------
        //            ApiCommonFunctions common = new ApiCommonFunctions();
        //            string directoryStructure = common.GetDirectoryStructure(FirmID, ApplicationID, OfficeID, ServiceType, ProcessType, ScanProcessId);
        //            directoryStructure += "\\Certificates\\";


        //            String FileName = string.Empty;

        //            if (ApplicationID == (int)ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling)
        //                FileName = "FirmCert-" + DateTime.Now.ToString("dd-MM-yyyy");
        //            else if (ApplicationID == (int)ApiCommonEnum.FirmApplicationType.FirmAmendmentFilling)
        //                FileName = "AmendCrt-" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss");
        //           else if (ApplicationID == (int)ApiCommonEnum.FirmApplicationType.FirmDissolutionFilling)
        //                FileName = "DissolutionCert-" + DateTime.Now.ToString("dd-MM-yyyy");



        //            string virtualFilePath = string.Empty;
        //            DownloadUploadFiles downloadUploadFiles = new DownloadUploadFiles();
        //            try
        //            {
        //                 FileServerPath = downloadUploadFiles.UploadByteArrayFileVirtual(model.fileBytes, FileName, directoryStructure, out virtualFilePath);
        //            }
        //            catch (Exception)
        //            {
        //                return "Error occured while uploading document , please contact help desk";
        //            }

        //            // call to insertscsn details
        //            ScanDetails scanModel = new ScanDetails();

        //            scanModel.FileName = FileName;

        //            scanModel.FileSize = filesize; ///********* Need to check *****
        //            //scanModel.DocumentID = 
        //            //scanModel.FileServerPath = IsUploaded;
        //            scanModel.FirmID = FirmID;
        //            scanModel.AmendmentID = FirmID;
        //            scanModel.DissolutionID = FirmID;

        //            scanModel.FirmApplicationTypeID = (int)ApplicationID;
        //            scanModel.IsDigitalSign = true;
        //            scanModel.IsUploaded = IsUploaded;
        //            scanModel.DocumentStatusID = (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms;
        //            scanModel.IsUploaded = IsUploaded;
        //            scanModel.KioskTokenID = TokenID;
        //            scanModel.OfficeID = OfficeID;
        //            scanModel.Pages = NoOfPages;
        //            scanModel.ServiceID = ServiceType;
        //            scanModel.UploadDateTime = DateTime.Now;
        //            scanModel.ScannedByUserID = UserID;
        //            scanModel.UploadedByUserID = UserID;
        //            scanModel.UploadedByResourceID = (int)ResourceID;
        //            scanModel.ScannedByResourceID = (int)ResourceID;
        //            scanModel.VirtualServerPath = virtualFilePath;
        //            scanModel.FileServerPath = FileServerPath;
        //            scanModel.ProcessID = ProcessType;
        //            scanModel.ScanProcessID = ScanProcessId;
        //            scanModel.Checksum = model.Checksum;
        //            scanModel.FirmApplicationTypeID =(int) ApplicationID;
        //            scanModel.AmendmentID = AmendmentID;
        //            scanModel.DissolutionID = DissolutionID;


        //            string result = InsertOnlineScanDetails(scanModel);
        //            if (!string.IsNullOrEmpty(result))
        //            {
        //                return result;
        //            }

        //            return responseMessage;

        //        }
        //        else
        //        {
        //            return "Invalid document,Please upload valid document.";
        //        }
        //    }
        //    catch (Exception )
        //    {
        //        throw ;
        //    }
        //}




        ///// <summary>
        ///// Save scan details for online user
        ///// </summary>
        ///// <param name="model">InsertScanDetails</param>
        ///// <returns>string message as success of exception occurrs</returns>
        //public string InsertOnlineScanDetails(ScanDetails model)
        //{
        //    dbContext = new KaveriEntities();
        //    try
        //    {
        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //           // Int64 maxScanID = 0;
        //            short pages = 0;
        //            var userDetails = dbContext.UMG_UserDetails.Where(m => m.UserID == model.ScannedByUserID).FirstOrDefault();

        //            if (model.ServiceID == Convert.ToInt16(ApiCommonEnum.enumServiceTypes.FirmRegistration))  //for Firm Registration Service
        //            {
        //                //short processMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == model.ScanProcessID).Select(m => m.ProcessMappingID).FirstOrDefault();
        //                //SCN_FRM_ScanDetails firmScanDetails = null;
        //                if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmRegistration)
        //                {
        //                    try
        //                    {
        //                        //if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID && c.ProcessMappingID == processMappingID).Count() == 1)
        //                        //{
        //                        //    throw new Exception("Document Already Exists");
        //                        //}
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ApiExceptionLogs.LogError(e);
        //                        return "Document already exists";
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(model.VirtualServerPath) && model.IsDigitalSign && userDetails.DefaultRoleID == Convert.ToInt16(ApiCommonEnum.RoleDetails.SR))
        //                {
        //                    if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmRegistration)
        //                    {
        //                        var firmDetails = dbContext.FRM_FirmDetails.Where(m => m.FirmID == model.FirmID).FirstOrDefault();

        //                        //firmScanDetails = dbContext.SCN_FRM_ScanDetails.Where(m => m.ProcessMappingID == processMappingID && m.FirmID == model.FirmID).FirstOrDefault();

        //                        FRM_FinalUploadedDocs oFinalUploadedDocs = new FRM_FinalUploadedDocs();
        //                        oFinalUploadedDocs.FinalUploadedDocID = dbContext.FRM_FinalUploadedDocs.Count() == 0 ? 1 : dbContext.FRM_FinalUploadedDocs.Max(x => x.FinalUploadedDocID) + 1;
        //                        oFinalUploadedDocs.ProcessMappingID = 0;//processMappingID;
        //                        oFinalUploadedDocs.FirmID = model.FirmID.Value;
        //                        oFinalUploadedDocs.AmendmentID = model.AmendmentID == 0 ? (long?)null : model.AmendmentID;
        //                        oFinalUploadedDocs.DissolutionID = model.DissolutionID == 0 ? (long?)null : model.DissolutionID;
        //                        oFinalUploadedDocs.Pages = model.Pages;
        //                        oFinalUploadedDocs.FileUploadDate = DateTime.Now;
        //                        oFinalUploadedDocs.FileName = model.FileName;
        //                        oFinalUploadedDocs.FileSize = model.FileSize;
        //                        oFinalUploadedDocs.FileServerPath = model.FileServerPath;
        //                        oFinalUploadedDocs.Checksum = model.Checksum;
        //                        oFinalUploadedDocs.UploadedByUserID = model.UploadedByUserID;
        //                        oFinalUploadedDocs.UploadedByResourceID = model.UploadedByResourceID;
        //                        oFinalUploadedDocs.CrossRefID = model.CrossRefID == 0 ? (long?)null : model.CrossRefID;
        //                        oFinalUploadedDocs.VirtualServerPath = model.VirtualServerPath;
        //                        oFinalUploadedDocs.IsDigitalSign = model.IsDigitalSign;

        //                        if (model.FirmApplicationTypeID == (int)ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling)
        //                        {
        //                            oFinalUploadedDocs.UploadDocTypeID = (int)ApiCommonEnum.UploadedDocumnetType.FirmCertificate;
        //                        }
        //                        else if (model.FirmApplicationTypeID == (int)ApiCommonEnum.FirmApplicationType.FirmAmendmentFilling)
        //                        {
        //                            oFinalUploadedDocs.UploadDocTypeID = (int)ApiCommonEnum.UploadedDocumnetType.FirmAmendmentCertificate;
        //                            oFinalUploadedDocs.AmendmentID = model.AmendmentID;

        //                            var AmendmentfirmDetails = dbContext.FRM_AmendmentDetails.Where(x => x.AmendmentID == model.AmendmentID).FirstOrDefault();
        //                            AmendmentfirmDetails.DocumentStatusID = Convert.ToByte(ApiCommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued);
        //                            AmendmentfirmDetails.ApplicationUpdateDateTime = DateTime.Now;


        //                             dbContext.Entry(AmendmentfirmDetails).State = System.Data.Entity.EntityState.Modified;

        //                    }
        //                        else if (model.FirmApplicationTypeID == (int)ApiCommonEnum.FirmApplicationType.FirmDissolutionFilling)
        //                        {
        //                            oFinalUploadedDocs.UploadDocTypeID = (int)ApiCommonEnum.UploadedDocumnetType.FirmDissolutionCertificate;
        //                            oFinalUploadedDocs.DissolutionID = model.DissolutionID;

        //                            var DissolutionfirmDetails = dbContext.FRM_FirmDissolutionDetails.Where(x => x.DissolutionID == model.DissolutionID).FirstOrDefault();
        //                              DissolutionfirmDetails.DocumentStatusID = Convert.ToByte(ApiCommonEnum.DocumentStatusTypes.FirmDissolutionAccepted);
        //                            DissolutionfirmDetails.ApplicationUpdateDateTime = DateTime.Now;
        //                            dbContext.Entry(DissolutionfirmDetails).State = System.Data.Entity.EntityState.Modified;



        //                        }

        //                        dbContext.FRM_FinalUploadedDocs.Add(oFinalUploadedDocs);


        //                    }
        //                }

        //                if (model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmRegistration))
        //                {
        //                    //SCN_FRM_ScanDetails scanDetailsModel = new SCN_FRM_ScanDetails();

        //                    //if (dbContext.SCN_FRM_ScanDetails.Any())
        //                    //    maxScanID = dbContext.SCN_FRM_ScanDetails.Select(c => c.ScanID).Max() + 1;

        //                    //scanDetailsModel.ScanID = maxScanID;
        //                    //scanDetailsModel.OfficeID = model.OfficeID;
        //                    //scanDetailsModel.ProcessMappingID = processMappingID;
        //                    //scanDetailsModel.FirmID = model.FirmID;
        //                    //scanDetailsModel.AmendmentID = model.AmendmentID == 0 ? (long?)null : model.AmendmentID;
        //                    //scanDetailsModel.DissolutionID = model.DissolutionID == 0 ? (long?)null : model.DissolutionID;
        //                    //scanDetailsModel.ScanForwardDate = DateTime.Now;
        //                    //scanDetailsModel.FileName = model.FileName;
        //                    //scanDetailsModel.FileSize = model.FileSize;
        //                    //scanDetailsModel.LocalPath = null;
        //                    //scanDetailsModel.FileServerPath = model.FileServerPath;

        //                    //scanDetailsModel.VirtualServerPath = model.VirtualServerPath;
        //                    //scanDetailsModel.ScanDate = DateTime.Now;
        //                    //scanDetailsModel.StartTime = DateTime.Now;
        //                    //scanDetailsModel.EndTime = DateTime.Now;
        //                    //scanDetailsModel.IsUploaded = true;
        //                    //scanDetailsModel.Checksum = model.Checksum;
        //                    //scanDetailsModel.ScannedByUserID = model.ScannedByUserID;
        //                    //scanDetailsModel.ScannedByResourceID = model.ScannedByResourceID;
        //                    //scanDetailsModel.UploadedByUserID = model.UploadedByUserID;
        //                    //scanDetailsModel.UploadedByResourceID = model.UploadedByResourceID;
        //                    //scanDetailsModel.UploadDateTime = DateTime.Now;
        //                    //scanDetailsModel.IsApproved = true;
        //                    //scanDetailsModel.ApprovedDate = DateTime.Now;

        //                    List<SupportDocuments> supportDocuments = null;
        //                    short applicationTypeID = 0;
        //                    if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmRegistration && model.ScanProcessID == (short)ApiCommonEnum.ScaningServicesProcess.RegularDocument)
        //                    {
        //                        var firmDetails = dbContext.FRM_FirmDetails.Where(x => x.FirmID == model.FirmID).FirstOrDefault();
        //                        if (null != firmDetails)
        //                        {
        //                            if (model.ScanProcessID == (short)ApiCommonEnum.ScaningServicesProcess.RegularDocument)
        //                            {
        //                                var kioskTokenDetails = dbContext.KSK_KioskTokenDetails.Where(m => m.KioskTokenID == firmDetails.KioskTokenID).FirstOrDefault();

        //                                //if (kioskTokenDetails.IsOnline)
        //                                //{
        //                                //    firmDetails.DocumentStatusID = Convert.ToByte(CommonEnum.DocumentStatusTypes.DigitallySigned);
        //                                //}
        //                                firmDetails.DocumentStatusID = Convert.ToByte(ApiCommonEnum.DocumentStatusTypes.ApprovedFirms);

        //                                firmDetails.IsScanned = true;
        //                            }
        //                            pages = firmDetails.Pages.HasValue ? Convert.ToInt16(firmDetails.Pages.Value) : pages;
        //                            dbContext.Entry(firmDetails).State = System.Data.Entity.EntityState.Modified;
        //                        }

        //                        applicationTypeID = Convert.ToInt16(ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling);
        //                        //Get Supporting document list
        //                        supportDocuments = (from SD in dbContext.MAS_FRM_SupportDocuments
        //                                            join TSD in dbContext.FRM_SupportedDocuments on SD.SupportDocumentID equals TSD.SupportDocumentID
        //                                            where SD.IsActive == true && SD.ApplicationTypeID == applicationTypeID && TSD.FirmID == model.FirmID
        //                                            select new SupportDocuments
        //                                            {
        //                                                SupportDocumentID = SD.SupportDocumentID,
        //                                                ApplicationTypeID = SD.ApplicationTypeID,
        //                                                Description = SD.Description,
        //                                                DescriptionR = SD.DescriptionR,
        //                                                FileServerPath = TSD.FileServerPath,
        //                                                VirtualServerPath = TSD.VirtualServerPath
        //                                            }).Distinct().ToList();
        //                    }


        //                    //scanDetailsModel.IsDigitalSign = model.IsDigitalSign == true ? true : (bool?)null;
        //                    //scanDetailsModel.Pages = pages;
        //                    //dbContext.SCN_FRM_ScanDetails.Add(scanDetailsModel);
        //                    dbContext.SaveChanges();

        //                    #region Supporting document merged into one and save enclosure details into scanning table

        //                    ////Check if it supporting document is uploaded then merge and insert into scanning table
        //                    //if (null != supportDocuments && supportDocuments.Count > 0)
        //                    //{
        //                    //    List<string> supportingDocFileDetailsList = new List<string>();
        //                    //    List<string> supportingDocDescFileDetailsList = new List<string>();
        //                    //    foreach (var item in supportDocuments)
        //                    //    {
        //                    //        if (string.IsNullOrEmpty(item.VirtualServerPath))
        //                    //            continue;
        //                    //        string supportDocumentDetails = item.SupportDocumentID + "#" + item.VirtualServerPath + "#" + item.FileServerPath;
        //                    //        supportingDocFileDetailsList.Add(supportDocumentDetails);
        //                    //        supportDocumentDetails = item.Description + "#" + item.SupportDocumentID;
        //                    //        supportingDocDescFileDetailsList.Add(supportDocumentDetails);
        //                    //    }
        //                    //    if ((null != supportingDocFileDetailsList && supportingDocFileDetailsList.Count > 0) && (null != supportingDocDescFileDetailsList && supportingDocDescFileDetailsList.Count > 0))
        //                    //    {
        //                    //           if (applicationTypeID == Convert.ToInt16(ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling))
        //                    //            model.FileName = "FRM_" + model.FirmID + "_SD_ENC_MERGED_" + applicationTypeID + ".pdf";

        //                    //        model.ScanProcessID = Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.Enclosures);

        //                    //        //   Common.ICommonRepository commonRepository = new Common.ApiCommonFunctions();

        //                    //        ApiCommonFunctions common = new ApiCommonFunctions();

        //                    //        string directoryName = common.GetDirectoryStructure(model.FirmID.Value, model.OfficeID, (short)model.ServiceID, model.ProcessID, model.ScanProcessID.Value);
        //                    //        string virtualFilePath = string.Empty;


        //                    //      //  akashpatil on(18 - 05 - 18)
        //                    //        Common.DownloadUploadFiles downloadUploadFiles = new Common.DownloadUploadFiles();

        //                    //        string physicalPath = downloadUploadFiles.UploadFirmSupportingDocumentWithVirtualPath(supportingDocFileDetailsList, supportingDocDescFileDetailsList.ToArray(), model.FileName, directoryName, out virtualFilePath);
        //                    //        string sFileDetails = downloadUploadFiles.GetFileLenghtChecksumWithVirtual(virtualFilePath);

        //                    //        processMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == model.ScanProcessID).Select(m => m.ProcessMappingID).FirstOrDefault();

        //                    //        model.FileServerPath = physicalPath;
        //                    //        model.VirtualServerPath = virtualFilePath;
        //                    //        model.FileSize = Convert.ToInt64(sFileDetails.Split('#')[0]);
        //                    //        model.Checksum = sFileDetails.Split('#')[1];

        //                    //        scanDetailsModel = new SCN_FRM_ScanDetails();
        //                    //        scanDetailsModel.ScanID = maxScanID + 1;
        //                    //        scanDetailsModel.OfficeID = model.OfficeID;
        //                    //        scanDetailsModel.ProcessMappingID = processMappingID;
        //                    //        scanDetailsModel.FirmID = model.FirmID;
        //                    //        scanDetailsModel.AmendmentID = model.AmendmentID == 0 ? (long?)null : model.AmendmentID;
        //                    //        scanDetailsModel.DissolutionID = model.DissolutionID == 0 ? (long?)null : model.DissolutionID;
        //                    //        scanDetailsModel.ScanForwardDate = DateTime.Now;
        //                    //        scanDetailsModel.FileName = model.FileName;
        //                    //        scanDetailsModel.FileSize = model.FileSize;
        //                    //        scanDetailsModel.LocalPath = null;
        //                    //        scanDetailsModel.FileServerPath = model.FileServerPath;
        //                    //        scanDetailsModel.VirtualServerPath = model.VirtualServerPath;
        //                    //        scanDetailsModel.ScanDate = DateTime.Now;
        //                    //        scanDetailsModel.StartTime = DateTime.Now;
        //                    //        scanDetailsModel.EndTime = DateTime.Now;
        //                    //        scanDetailsModel.IsUploaded = true;
        //                    //        scanDetailsModel.Checksum = model.Checksum;
        //                    //        scanDetailsModel.ScannedByUserID = model.ScannedByUserID;
        //                    //        scanDetailsModel.ScannedByResourceID = model.ScannedByResourceID;
        //                    //        scanDetailsModel.UploadedByUserID = model.UploadedByUserID;
        //                    //        scanDetailsModel.UploadedByResourceID = model.UploadedByResourceID;
        //                    //        scanDetailsModel.UploadDateTime = DateTime.Now;
        //                    //        scanDetailsModel.IsApproved = true;
        //                    //        scanDetailsModel.ApprovedDate = DateTime.Now;
        //                    //        dbContext.SCN_FRM_ScanDetails.Add(scanDetailsModel);
        //                    //        dbContext.SaveChanges();
        //                    //    }
        //                    //}

        //                    #endregion
        //                }
        //            }
        //            dbContext.SaveChanges();
        //            scope.Complete();
        //        }
        //        return string.Empty;
        //    }
        //    catch (Exception )
        //    {
        //        //throw new BusinessException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.AddScanningDetailsException);
        //        throw ;
        //    }
        //    finally
        //    {
        //        if (null != dbContext)
        //            dbContext.Dispose();
        //    }
        //}




        // Otp generation method -- Shrinivas

        public string GenerateOTP()
        {
            try
            {
                dbContext = new KaveriEntities();

                string OTP = "";

                int i = 0, randomDigit = 0;

                bool[] IsDigitAlreadyGenerated = new bool[10]; // No random digit from 0 to 9 is generated yet

                for (i = 0; i < IsDigitAlreadyGenerated.Length; i++)
                {
                    IsDigitAlreadyGenerated[i] = false;
                }

                for (i = 0; i < 6; i++) // Generating 6 random unique digits
                {
                    randomDigit = new Random().Next(0, 9);

                    while (IsDigitAlreadyGenerated[randomDigit]) // if random generated digit is already generated
                    {
                        randomDigit = new Random().Next(0, 9); // generate new random digit
                    }

                    OTP = OTP + randomDigit; // Build OTP by concatenating generated random digit 
                    IsDigitAlreadyGenerated[randomDigit] = true;  // mark the random digit as generated 
                }

                string encryptedOTP = SHA512Checksum.CalculateSHA512Hash(OTP);

                //checking if generated OTP is already present in database
                UMG_OTP_VerificationDetails OTPVerificationDBObject = dbContext.UMG_OTP_VerificationDetails.FirstOrDefault(x => x.OTPToSend == encryptedOTP);
                if (OTPVerificationDBObject != null)
                {
                    //checking if the pre-generated OTP is already verified 
                    if (OTPVerificationDBObject.IsOTPVerified == true)
                        return OTP;
                    else
                        return GenerateOTP();
                }
                else
                {
                    return OTP;
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
        }

        // Otp generation method -- Shrinivas



        // Added by Shubham Bhagat on 19-11-2018
        /// <summary>
        /// This method returns "FileDisplayModel" obj. which contains byte[] of FirmCertificate Report with added MetaData.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>



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

        public List<SelectListItem> GetSROfficesListByDisrictID(long DistrictCode)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                //if (isAllSelected)
                OfficeList.Insert(0, objCommon.GetDefaultSelectListItem("All", "0"));
                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
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

            return OfficeList;


        }


        public List<SelectListItem> GetSROOfficeListByDistrictIDWithFirstRecord(long DistrictCode, string FirstRecord)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                //if (isAllSelected)
                OfficeList.Insert(0, objCommon.GetDefaultSelectListItem(FirstRecord, "0"));
                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
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

            return OfficeList;


        }












        //Added By Raman Kalegaonkar on 20-05-2019
        public string GetSroName(int SROfficeID)
        {
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

        //Added By Raman Kalegaonkar on 20-05-2019
        public string GetDroName(int DistrictID)
        {
            string DroName;
            try
            {
                dbContext = new KaveriEntities();
                DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DistrictID).Select(x => x.DistrictNameE).FirstOrDefault();
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
            return DroName;

        }

        //Added By Raman Kalegaonkar on 20-05-2019
        public short GetLevelIdByOfficeId(short OfficeID)
        {
            short LevelID;
            try
            {
                dbContext = new KaveriEntities();
                LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
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
            return LevelID;

        }

        //Added by Raman Kalegaonkar on 03-08-2019  for Registration Office
        public Dictionary<int, int> GetSROOfficeListDictionary(long DistrictCode)
        {
            int CounterSroDict = 1;
            Dictionary<int, int> SROOfficeDict = new Dictionary<int, int>();
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
                foreach (var SROListObj in OfficeList)
                {
                    SROOfficeDict.Add(Convert.ToInt32(SROListObj.Value), CounterSroDict++);
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

            return SROOfficeDict;


        }

        //Added by Raman Kalegaonkar on 03-08-2019 for Registration Statistics
        public List<SelectListItem> GetSROfficesListByDisrictID(bool IsFirstRecord, long DistrictCode)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();
                if (IsFirstRecord)
                {
                    OfficeList.Insert(0, objCommon.GetDefaultSelectListItem("ALL", "0"));
                }
                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());

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

            return OfficeList;


        }


        //Added by Raman Kalegaonkar on 04-08-2019
        public Dictionary<int, int> GetJurisdictionWiseDictionary(long DistrictCode)
        {
            int CounterSroDict = 1;
            Dictionary<int, int> SROOfficeDict = new Dictionary<int, int>();
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                List<SelectListItem> SROMasterList = dbContext.USP_RPT_JURISDICTIONAL_SRO().Select(i => new SelectListItem()
                {
                    Text = i.SRONameE,
                    Value = i.SROCode.ToString()
                }).ToList();

                foreach (var SROListObj in SROMasterList)
                {
                    SROOfficeDict.Add(Convert.ToInt32(SROListObj.Value), CounterSroDict++);
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

            return SROOfficeDict;


        }

        //Added by Raman Kalegaonkar on 05/12/2019 for CDWrittenReport
        public List<SelectListItem> GetCDNumberList(int SROCode, String FirstRecord)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();
                OfficeList.Insert(0, objCommon.GetDefaultSelectListItem(FirstRecord, "0"));
                OfficeList.AddRange(dbContext.CDMaster.OrderByDescending(c => c.VolumeName).Where(m => m.SROCode == SROCode).Select(m => new SelectListItem { Value = m.CDID.ToString(), Text = m.VolumeName }).ToList());
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

            return OfficeList;


        }



        public MenuHighlightResponseModel GetMenuDetailsToHighlight(MenuHighlightReqModel reqModel)
        {


            MenuHighlightResponseModel responseModel = new MenuHighlightResponseModel();


            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var USP_MENU_TO_HIGHLIGHT_Result = dbContext.USP_MENU_TO_HIGHLIGHT(reqModel.ControllerName, reqModel.ActionName).ToList();


                if (USP_MENU_TO_HIGHLIGHT_Result != null && USP_MENU_TO_HIGHLIGHT_Result.Count > 0)
                    responseModel.MenuName = USP_MENU_TO_HIGHLIGHT_Result[0].MenuName;
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

            return responseModel;



        }


    }
}