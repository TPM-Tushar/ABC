#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   KaveriSupportApiController.cs
    * Author Name       :   - Akash Patil
    * Creation Date     :   - 02-05-2019
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for kaveri support module.
*/
#endregion

#region References
using CustomModels.Models.Common;
using CustomModels.Models.KaveriSupport;
using ECDataAPI.Areas.KavariSupport.Interface;
using ECDataAPI.Common;
using ECDataAPI.EcDataService;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
#endregion

namespace ECDataAPI.Areas.KavariSupport.BAL
{
    public class KaveriSupportBAL : IKaveriSupport, IDisposable
    {
        #region Properties

        ECDataAPI.EcDataService.ECDataService service = new ECDataAPI.EcDataService.ECDataService();
        #endregion

        #region Methods

        /// <summary>
        /// RegisterTicketDetailsAndGenerateKeyPair() returns AppDeveloperViewModel object with ResponseMessage if details saved successfully
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public AppDeveloperViewModel RegisterTicketDetailsAndGenerateKeyPair(AppDeveloperViewModel viewModel)
        {

            try
            {
                EcDataService.TicketDetailsViewModel servicemodel = new TicketDetailsViewModel();


                servicemodel.Description = viewModel.TicketDescription;
                servicemodel.ModuleID = viewModel.ModuleID;
                servicemodel.SROCode = viewModel.SRONameID;
                servicemodel.TicketNumber = viewModel.TicketNumber;

                string response = string.Empty;
                string tempDate = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

                ApiCommonFunctions.Tuple<string, string> tuple = ApiCommonFunctions.CreateKeyPair();
                servicemodel.PublicKey = tuple.PublicKey;
                servicemodel.PrivateKey = tuple.PrivateKey;

                if (string.IsNullOrEmpty(servicemodel.PublicKey))
                {
                    viewModel.ErrorMessage = "Invalid public key";
                    return viewModel;
                }

                servicemodel = service.SaveTicketDetails(servicemodel);
                if (!string.IsNullOrEmpty(servicemodel.ErrorMessage))
                {
                    viewModel.ErrorMessage = servicemodel.ErrorMessage;
                    return viewModel;
                }

                //In case of success i.e a.Ticket saved , b.Public key details being saved successfully
                viewModel.TicketID = servicemodel.TicketID;
                viewModel.IsUploadedSuccessfully = true;
                viewModel.ResponseMessage = "Ticket details saved successfully ";

                return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method returns True if public key details saved successfully
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private bool UploadPublicKeyforEnclosureEncryption(AppDeveloperViewModel viewModel, ref string responseMessage)
        {
            try
            {

                if (string.IsNullOrEmpty(viewModel.PublicKeyStr))
                {
                    responseMessage = "Invalid Public Key";
                    return false;
                }
                else
                {
                    responseMessage = service.SavePublicKey(viewModel.PublicKeyStr, viewModel.PrivateKeyStr, viewModel.TicketID);
                    if (responseMessage.Contains("success"))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// this method decrypt file using parameters i.e zipFilePath and TicketNumber , returns decrypted file path in out parameter DecryptFilepath
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="TicketNumber"></param>
        /// <param name="DecryptFilepath"></param>
        /// <returns></returns>
        public DecryptEnclosureModel NewDecryptAndSave(string zipFilePath, string TicketNumber, out string DecryptFilepath)
        {
            try
            {
                ApiCommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");
                ApiCommonFunctions.WriteErrorLog("In NewDecryptAndSave");

                DecryptEnclosureModel responseModel = new DecryptEnclosureModel();

                string keyToEncryptPrivateKey = string.Empty;
                string fileName = string.Empty;
                string iFilePath = string.Empty;
                string response = string.Empty;
                StringBuilder decryptFileBuilder = new StringBuilder();
                string rootPath = ConfigurationManager.AppSettings["KaveriSupportDecryptFilesPath"];
                string saveDecryptFilepath = string.Empty;
                string saveDecryptFilepathURL = string.Empty;

                ApiCommonFunctions.WriteErrorLog("RootPath :: " + rootPath);

                #region UNZIP
                ZipFile zip = ZipFile.Read(zipFilePath);
                if (Directory.Exists(rootPath))
                    Directory.Delete(rootPath, true);
                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);
                int cnt = 0;
                ApiCommonFunctions.WriteErrorLog("zip Count :: " + zip.Count);
                foreach (ZipEntry e in zip)
                {
                    e.Extract(rootPath, ExtractExistingFileAction.OverwriteSilently);
                    cnt++;
                }
                #endregion

                #region Get All Txt Files
                int fileCountInFolder = Directory.GetFiles(rootPath).Length;
                ApiCommonFunctions.WriteErrorLog("fileCountInFolder :: " + fileCountInFolder);
                #endregion

                #region Get Private Key
                PrivateKeyDetailsViewModel result = service.GetPrivateKeyForEnclosureDecryptionUsingTicketNumber(TicketNumber);

                ApiCommonFunctions.WriteErrorLog("result.ResponseMessage :: " + result.ResponseMessage);

                if (!result.ResponseStatus)
                {
                    responseModel.ResponseStatus = false;
                    responseModel.ErrorMessage = result.ResponseMessage;

                    DecryptFilepath = "";
                    return responseModel;
                }

                keyToEncryptPrivateKey = result.PrivateKey;

                ApiCommonFunctions.WriteErrorLog("keyToEncryptPrivateKey :: " + keyToEncryptPrivateKey);

                #endregion
                try
                {
                    for (int i = 1; i <= fileCountInFolder; i++)
                    {
                        response = string.Empty;
                        string tempfilePath = rootPath + "\\" + i + ".txt";
                        byte[] encryptedBytes = System.IO.File.ReadAllBytes(tempfilePath);
                        response = ApiCommonFunctions.Decrypt(keyToEncryptPrivateKey, encryptedBytes);
                        decryptFileBuilder.Append(response);
                    }
                }
                catch (Exception e)
                {
                    ApiExceptionLogs.LogError(e);

                    DecryptFilepath = "";
                    responseModel.ResponseStatus = false;
                    responseModel.ErrorMessage = "Failed to decrypt because of incorrect ZIP file or ticket number, Please select valid combination ZIP file and ticket number.";

                    return responseModel;
                }

                ApiCommonFunctions.WriteErrorLog("decryptFileBuilder :: " + decryptFileBuilder.ToString());

                if (decryptFileBuilder.ToString() != "")
                {

                    #region Write Decrypted text to file
                    saveDecryptFilepath = System.Web.Configuration.WebConfigurationManager.AppSettings["KaveriSupportPath"];

                    ApiCommonFunctions.WriteErrorLog("saveDecryptFilepath :: " + saveDecryptFilepath);

                    if (saveDecryptFilepath != "")
                    {

                        if (!Directory.Exists(saveDecryptFilepath))
                            Directory.CreateDirectory(saveDecryptFilepath);


                        fileName = "DecryptedFile_" + TicketNumber + ".sql";
                        iFilePath = System.IO.Path.Combine(saveDecryptFilepath, fileName);

                        ApiCommonFunctions.WriteErrorLog("iFilePath :: " + iFilePath);

                        //Commented temp by Akash
                        #region iFilePath Delete if Exists
                        //if (System.IO.File.Exists(iFilePath))
                        //    System.IO.File.Delete(iFilePath);
                        #endregion

                        byte[] bytesData = Encoding.UTF8.GetBytes(decryptFileBuilder.ToString());


                        #region Check if Hard Drive is empty or not
                        double DriveLimit = 0;
                        double CurrentFileSize = bytesData.Length;

                        string HardDrive = saveDecryptFilepath.FirstOrDefault().ToString();
                        HardDrive = HardDrive + ":/";

                        DriveInfo Ddrive = new DriveInfo(HardDrive);
                        if (Ddrive.IsReady)  //Drive is Ready
                        {
                            //Byte Reduction
                            double AvailableFreeSpaceInKB = Ddrive.AvailableFreeSpace;   // 295 GB ==>316938629120.0 in bytes       
                            double OneGBToByte = (1024 * 1024 * 1024);   //1 GB-->1073741824.0 Bytes
                            DriveLimit = AvailableFreeSpaceInKB - OneGBToByte;
                            double d = (DriveLimit / (1024 * 1024 * 1024));  //DriveLimit in GB : 294.17210006713867
                            double DriveSizeReducedByOneGB1 = d - 1;  //293.17210006713867
                        }
                        #endregion

                        if (DriveLimit > CurrentFileSize)   //Space Available 
                        {
                            ApiCommonFunctions.WriteErrorLog("DriveLimit > CurrentFileSize");

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(rootPath);
                            using (FileStream stream = new FileStream(iFilePath, FileMode.Create))
                            {
                                stream.Write(bytesData, 0, bytesData.Length);
                                stream.Close();
                            }
                        }
                        else
                        {
                            ApiCommonFunctions.WriteErrorLog("Failed to decrypt and save file because of insufficient disk space , Please contact admin");
                            DecryptFilepath = "";
                            responseModel.ResponseStatus = false;
                            responseModel.ErrorMessage = "Failed to decrypt and save file because of insufficient disk space , Please contact admin";

                            return responseModel;
                        }

                        DecryptFilepath = iFilePath;
                        responseModel.ResponseStatus = true;

                        //if (responseModel.ResponseStatus)
                        //{
                        //  //  string responseMsg = service.DeactivateKeyUsingTicketNumber(TicketNumber);
                        //    if (!string.IsNullOrEmpty(responseMsg))
                        //    {
                        //        DecryptFilepath = "";
                        //        responseModel.ResponseStatus = false;
                        //        responseModel.ErrorMessage = responseMsg;// "Failed to Decrypte and Save Zip file.";
                        //        return responseModel;
                        //    }

                        //}


                        ApiCommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");

                        return responseModel;
                    }
                    else
                    {
                        DecryptFilepath = "";
                        responseModel.ResponseStatus = false;
                        responseModel.ErrorMessage = "Failed to Decrypte and Save Zip file.";


                        ApiCommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");


                        return responseModel;
                    }
                    #endregion
                }
                else
                {
                    ApiCommonFunctions.WriteErrorLog("decryptFileBuilder == null");
                    ApiCommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");
                    DecryptFilepath = "";

                    responseModel.ResponseStatus = false;
                    responseModel.ErrorMessage = "Error Occured while getting Enclosure content.";

                    return responseModel;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// This method returns private key in string
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            try
            {
                return service.GetPrivateKeyForEnclosureDecryption();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// This method returns true if ticket details exists
        /// </summary>
        /// <param name="TicketNumber"></param>
        /// <returns></returns>
        public string IsTicketExists(string TicketNumber)
        {
            try
            {
                string result = service.IsTicketExists(TicketNumber);
                return result;

            }
            catch (Exception e)
            {
                ApiExceptionLogs.LogError(e);
                return "Something went wrong while connecting to service , Please contact admin.";
            }
        }

        /// <summary>
        /// This method used to Encrypt SQL patch file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AppDeveloperViewModel EncryptSQLPatchFile(AppDeveloperViewModel model)
        {

            try
            {
                #region Local Prop
                string iFilePath = string.Empty;
                int fileCnt = 1;
                int newfilecount = 0;
                string finalString = string.Empty;
                string keyToEncrypt = string.Empty;
                var regex = @"^[0-9]*$";
                var match = Regex.Match(model.TicketNumber, regex, RegexOptions.IgnoreCase);
                #endregion

                #region De-activate key using ticket ticket number(i.e in TicketEncryptionKeyStore table)
                //string responseMsg = service.DeactivateKeyUsingTicketNumber(model.TicketNumber);
                //if (!string.IsNullOrEmpty(responseMsg))
                //{
                //    model.ErrorMessage = "Error occured while deactivating key , Please contact help desk";
                //    return model;
                //}
                #endregion


                finalString = System.IO.File.ReadAllText(model.Filepath);

                #region Delete filepath if exist
                string txtFilePath = ConfigurationManager.AppSettings["KaveriSupportEncryptFilesPath"];
                if (Directory.Exists(txtFilePath))
                    Directory.Delete(txtFilePath, true);

                #endregion

                #region GET Public Key for Encryption
                ApiCommonFunctions.Tuple<string, string> tuple = ApiCommonFunctions.CreateKeyPair();

                keyToEncrypt = tuple.PublicKey;
                #endregion

                #region Check if Hard Drive is empty or not
                string MinimumDiskSpaceReqForKaveriSupport = ConfigurationManager.AppSettings["MinimumDiskSpaceReqForKaveriSupport"];
                double DriveLimit = 0;
                double CurrentFileSize = Convert.ToDouble(MinimumDiskSpaceReqForKaveriSupport);

                string HardDrive = txtFilePath.FirstOrDefault().ToString();
                HardDrive = HardDrive + ":/";

                DriveInfo Ddrive = new DriveInfo(HardDrive);
                if (Ddrive.IsReady)  //Drive is Ready
                {
                    //Byte Reduction
                    double AvailableFreeSpaceInKB = Ddrive.AvailableFreeSpace;   // 295 GB ==>316938629120.0 in bytes       
                    double OneGBToByte = (1024 * 1024 * 1024);   //1 GB-->1073741824.0 Bytes
                    DriveLimit = AvailableFreeSpaceInKB - OneGBToByte;
                    double d = (DriveLimit / (1024 * 1024 * 1024));  //DriveLimit in GB : 294.17210006713867
                    double DriveSizeReducedByOneGB1 = d - 1;  //293.17210006713867
                }
                if (DriveLimit <= CurrentFileSize)   //Space Available 
                {
                    model.ErrorMessage = "Unable to encrypt and save file due to insufficient disk space ";
                    return model;

                }
                #endregion

                #region Divide and Encrypt
                IEnumerable<string> str = ChunksUpto(finalString, 100);
                List<string> strList = str.ToList();
                int encCnt = 0;

                foreach (var item in strList)
                {
                    string tempString = item;
                    //  byte[] tempEncryt = NewEncryptAndSave(tempString, fileCnt, keyToEncrypt, out newfilecount);
                    FileModel fileModel = NewEncryptAndSave(tempString, fileCnt, keyToEncrypt, out newfilecount);

                    if (!string.IsNullOrEmpty(fileModel.ErrorMessage))
                    {
                        model.ErrorMessage = fileModel.ErrorMessage;
                        return model;
                    }

                    fileCnt = newfilecount;
                    encCnt++;
                }
                #endregion

                #region Create Zip File
                string fileName = "SQLPatch_" + model.TicketNumber + ".zip";

                string zipPath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["KaveriSupportPath"], model.EcryptedPatchFilePath);

                string zipFilePath = System.IO.Path.Combine(zipPath, fileName);
                if (!Directory.Exists(zipPath))
                    Directory.CreateDirectory(zipPath);
                if (System.IO.File.Exists(zipFilePath))
                    System.IO.File.Delete(zipFilePath);

                string[] filePaths = Directory.GetFiles(txtFilePath);

                encCnt = 0;

                using (ZipFile zip = new ZipFile())
                {
                    foreach (var filePath in filePaths)
                    {
                        try
                        {
                            zip.AddFile(filePath.ToString());
                            encCnt++;

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    zip.UseZip64WhenSaving = Zip64Option.Always;
                    zip.Save(zipFilePath);
                }

                #endregion

                #region Save Private Key


                TicketDetailsViewModel response = service.GetTicketIDandDeActivateKey(model.TicketNumber);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    model.ErrorMessage = response.ErrorMessage;
                    return model;
                }
                if (response.TicketID == 0)
                {
                    model.ErrorMessage = "Entered Ticket Number is deactivated or doesn't exists , Please try another ticket number";
                    return model;
                }

                EcDataService.PrivateKeyDetailsViewModel privateKeyDetailsModel = new EcDataService.PrivateKeyDetailsViewModel();
                privateKeyDetailsModel.TicketID = response.TicketID;
                privateKeyDetailsModel.PrivateKey = tuple.PrivateKey;
                privateKeyDetailsModel.ScriptFilePhysicalPath = zipFilePath;
                string VirtualPath = zipFilePath.Replace(System.Configuration.ConfigurationManager.AppSettings["KaveriSupportPath"], "~/");
                VirtualPath = VirtualPath.Replace(@"\", "//");
                privateKeyDetailsModel.ScriptFileVirtualPath = VirtualPath;


                bool result = service.SavePrivateKey(privateKeyDetailsModel);
                if (!result)
                {
                    model.ErrorMessage = "Failed to encrypt and save file.";
                    return model;
                }
                #endregion

                model.ResponseMessage = "File encrypted and saved successfully ";

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }


        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));

        }

        /// <summary>
        /// this method encrypt file using parameters i.e data , fileCnt & keyToEncryptPublicKey , returns new file Count in out parameter newfileCnt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileCnt"></param>
        /// <param name="keyToEncryptPublicKey"></param>
        /// <param name="newfileCnt"></param>
        /// <returns></returns>
        public FileModel NewEncryptAndSave(string data, int fileCnt, string keyToEncryptPublicKey, out int newfileCnt)
        {
            try
            {
                FileModel ResponseModel = new FileModel();

                string rootPath = ConfigurationManager.AppSettings["KaveriSupportEncryptFilesPath"];
                //string keyToEncryptPublicKey = string.Empty;
                string fileName = string.Empty;
                string iFilePath = string.Empty;

                byte[] bytesData = ApiCommonFunctions.Encrypt(keyToEncryptPublicKey, data);
                //fileName = "EncryptFile_" + fileCnt + ".txt";
                fileName = fileCnt + ".txt";
                iFilePath = System.IO.Path.Combine(rootPath, fileName);

                #region iFilePath  if Exists

                if (System.IO.File.Exists(iFilePath))
                {
                    fileCnt++;
                    fileName = fileCnt + ".txt";
                    iFilePath = System.IO.Path.Combine(rootPath, fileName);
                }

                #endregion

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);
                using (FileStream stream = new FileStream(iFilePath, FileMode.Create))
                {
                    stream.Write(bytesData, 0, bytesData.Length);
                    stream.Close();
                }

                newfileCnt = fileCnt;
                ResponseModel.FileBytes = bytesData;
                return ResponseModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// This method returns AppDeveloperViewModel object to populate SRO and Module list on view
        /// </summary>
        /// <returns></returns>
        public AppDeveloperViewModel GetTicketRegistrationDetails()
        {
            AppDeveloperViewModel returnmodel = new AppDeveloperViewModel();
            try
            {
                #region Populate Module List
                ModuleListViewModel[] moduleList = null;
                try
                {
                    moduleList = service.PopulateModuleList();
                }
                catch (Exception e)
                {
                    returnmodel.ErrorMessage = "Something went wrong while connecting to service , Please contact admin.";
                    ApiExceptionLogs.LogError(e);
                    return returnmodel;
                }
                returnmodel.ModuleNameDropDown = new List<SelectListItem>();

                SelectListItem selectItemDefault = new SelectListItem();
                selectItemDefault.Text = "select";
                selectItemDefault.Value = "0";

                returnmodel.ModuleNameDropDown.Add(selectItemDefault);

                foreach (var item in moduleList)
                {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Text = item.moduleName;
                    selectItem.Value = item.moduleID.ToString();

                    returnmodel.ModuleNameDropDown.Add(selectItem);
                }

                #endregion

                #region Populate SRO List
                var srolist = service.PopulateSROList();
                returnmodel.SRONameDropDown = new List<SelectListItem>();

                SelectListItem selectItemDefault2 = new SelectListItem();
                selectItemDefault2.Text = "select";
                selectItemDefault2.Value = "0";
                returnmodel.SRONameDropDown.Add(selectItemDefault2);
                foreach (var item in srolist)
                {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Text = item.SROName;
                    selectItem.Value = item.SROCode.ToString();

                    returnmodel.SRONameDropDown.Add(selectItem);
                }
                #endregion
                return returnmodel;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Listing of Ticket details

        /// <summary>
        /// This method return ticket details list
        /// </summary>
        /// <returns></returns>
        public TicketDetailsListModel LoadTicketDetailsList()
        {
            try
            {
                TicketDetailsListModel response = new TicketDetailsListModel();

                EcDataService.TicketDetailsListServiceModel responseModel = null;

                try
                {
                    responseModel = service.LoadTicketDetailsList();
                }
                catch (Exception e)
                {
                    response.ErrorMessage = "Something went wrong while connecting to service , Please contact admin.";
                    ApiExceptionLogs.LogError(e);
                    return response;
                }

                response.TicketDetailsList = new List<TicketDetailsModel>();

                TicketDetailsModel model = null;

                foreach (var item in responseModel.TicketDetailsList)
                {
                    model = new TicketDetailsModel();

                    model.TicketNumber = item.TicketNumber;
                    model.TicketDescription = item.TicketDescription;
                    model.ModuleName = item.ModuleName;
                    model.Office = item.Office;
                    model.RegistrationDateTime = item.RegistrationDateTime;
                    model.IsActive = item.IsActive;
                    response.TicketDetailsList.Add(model);
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method returns private key details list
        /// </summary>
        /// <returns></returns>
        public TicketDetailsListModel LoadPrivateKeyDetailsList()
        {
            try
            {
                TicketDetailsListModel response = new TicketDetailsListModel();
                EcDataService.TicketDetailsListServiceModel responseModel = null;

                try
                {
                    responseModel = service.LoadPrivateKeyDetailsList();
                }
                catch (Exception e)
                {
                    response.ErrorMessage = "Something went wrong while connecting to service , Please contact admin.";
                    ApiExceptionLogs.LogError(e);
                    return response;
                }

                response.PrivateKeyDetailsList = new List<PrivateKeyDetailsModel>();
                PrivateKeyDetailsModel model = null;
                string Temp = string.Empty;
                // int idx = 0;
                foreach (var item in responseModel.PrivateKeyDetailsList)
                {
                    model = new PrivateKeyDetailsModel();

                    model.TicketNumber = item.TicketNumber;
                    //idx = item.PhysicalFilePath.LastIndexOf('\\');
                    //if (idx != -1)
                    //{
                    //    model.FileName = item.PhysicalFilePath.Substring(idx + 1);
                    //}
                    //else
                    //{
                    //    model.FileName = " -- ";
                    //}

                    model.FileName = item.FileName;

                    model.UploadDateTime = item.UploadDateTime;
                    model.IsActive = item.IsActive;

                    response.PrivateKeyDetailsList.Add(model);
                }
                return response;
            }
            catch (Exception)
            {
                throw;
                //return null;
            }

        }
        #endregion



        public void Dispose()
        {
            if (service != null)
                service.Dispose();
        }
        #endregion
    }
}