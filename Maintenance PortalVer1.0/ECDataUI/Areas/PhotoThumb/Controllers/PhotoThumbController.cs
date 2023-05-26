using Aspose.Words;
using CustomModels.Models.PhotoThumb;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.PhotoThumb.Controllers
{
    public class PhotoThumbController : Controller
    {
        private ServiceCaller caller;

        public ActionResult PhotoThumbView()
        {
            try
            {
                caller = new ServiceCaller("PhotoThumbAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                PhotoThumbViewModel reqModel = caller.GetCall<PhotoThumbViewModel>("PhotoThumbView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });
            }

        }
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult PhotoThumbAvailaibilityTable(FormCollection formCollection)
        {
            PhotoThumbTableModel ResModel = new PhotoThumbTableModel();
            try
            {
                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("PhotoThumbAPIController");
                PhotoThumbReqModel ReqModel = new PhotoThumbReqModel();
                ReqModel.SROCode = Convert.ToInt32(formCollection["SROfficeID"]);
                ReqModel.DocumentNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
                ReqModel.BookTypeID = Convert.ToInt32(formCollection["BookTypeID"]);
                ReqModel.FinancialYearStr = formCollection["FinancialYear"].ToString();

                ResModel = caller.GetCall<PhotoThumbTableModel>("PhotoThumbAvailaibility", new { SROCode = Convert.ToInt32(formCollection["SROfficeID"]), DocumentNumber = Convert.ToInt64(formCollection["DocumentNumber"]), BookTypeID = Convert.ToInt32(formCollection["BookTypeID"]), fyear = formCollection["FinancialYear"].ToString() }, out errorMessage);

                if (ResModel.IsError == true || ResModel == null)
                {
                    if (ResModel == null)
                    {
                        errorMessage = "Some error occured while loading table.";
                    }
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = ResModel.ErrorMessage
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (!Directory.Exists(Server.MapPath("~/Content/TempPhotoThumbFiles")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/TempPhotoThumbFiles"));
                }


                DeleteReport("abc");

                //----------------------- CONVERT ENC TO TIFF-------------------------
                ////-------------------------------------------------------------------------------------------- 
                foreach (PartyDetailModel pdm in ResModel.PDM)
                {
                    string decryptFilePath = HttpContext.Server.MapPath("~/Content/TempPhotoThumbFiles");
                    
                    FileUploaderPhotoThumb.FileUploaderSoapClient uploaderSoapClient = new FileUploaderPhotoThumb.FileUploaderSoapClient();


                    if(pdm.PhotoPath == "" || pdm.ThumbPath == "")
                    {
                        pdm.ConvertedPhoto = "<div style='padding:1rem;width:100%;'>Unavailable</div>";
                        pdm.ConvertedThumb = "<div style='padding: 1rem;width:100%; '>Unavailable</div>";
                        continue;
                    }


                    //Photo
                    try
                    {
                        Decrypt decryptorPhoto = new Decrypt();
                        string fileNameToProcessPhotoTiff = "TiffPhoto" + pdm.PartyID.ToString() + pdm.SROCode.ToString();
                        string fileNameToProcessPhoto = "Photo" + pdm.PartyID.ToString() + pdm.SROCode.ToString();
                        byte[] Photo = uploaderSoapClient.DownloadScannedPhotoThumb(pdm.PhotoPath);
                        string tempENCPhoto = Server.MapPath("~/Content/TempPhotoThumbFiles/tempENCPhoto.enc");
                        if (System.IO.File.Exists(tempENCPhoto))
                        {
                            System.IO.File.Delete(tempENCPhoto);
                        }
                        using (FileStream fs = new FileStream(tempENCPhoto, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.Write(Photo, 0, Photo.Length);
                        }
                        decryptorPhoto.encFilePath = tempENCPhoto;
                        decryptorPhoto.decFilePath = @decryptFilePath + @"\" + fileNameToProcessPhotoTiff + ".tiff";
                        string TiffFileNamePhoto = fileNameToProcessPhotoTiff + ".tiff";
                        string JPGFileNamePhoto = fileNameToProcessPhoto + ".jpg";
                        string TiffFilePathPhoto = @decryptFilePath + @"\" + fileNameToProcessPhotoTiff + ".tiff";
                        string JPGFilePathPhoto = @decryptFilePath + @"\" + fileNameToProcessPhoto + ".jpg";



                        if (System.IO.File.Exists(TiffFilePathPhoto))
                        {
                            System.IO.File.Delete(TiffFilePathPhoto);
                        }


                        if (decryptorPhoto.DecryptFile("fecdba9876543210", ref errorMessage) == false)
                        //if (DecryptAndSave(decryptorPhoto.encFilePath, "fecdba9876543210", decryptorPhoto.decFilePath) == false)
                        {
                            pdm.ConvertedPhoto = "<div style='padding:75% 0;width:100%;'>Unavailable</div>";
                        }
                        else
                        {
                            if (System.IO.File.Exists(JPGFilePathPhoto))
                            {
                                System.IO.File.Delete(JPGFilePathPhoto);
                            }
                            var doc = new Document();
                            var builder = new DocumentBuilder(doc);

                            var shape = builder.InsertImage(decryptorPhoto.decFilePath);
                            shape.ImageData.Save(JPGFilePathPhoto);
                            pdm.ConvertedPhoto = @"<img src='\Content\TempPhotoThumbFiles\" + JPGFileNamePhoto + "' style='max-width:100%;position: absolute;max-height: 100%;left: 0;margin-right: auto;margin-left: auto;right: 0;top: 0;margin-bottom: auto;bottom: 0;margin-top: auto;'><div style='width: 100%;'><b style=\"text-decoration: underline; text-underline-offset: 5px; \">Uploaded Date </b><br>" + pdm.UploadDatePhoto + "</div>";

                        }
                    }
                    catch(Exception ex)
                    {
                        ExceptionLogs.LogException(ex);
                        pdm.ConvertedPhoto = "<div style='padding: 1rem; width: 100%; '>Unavailable</div>";
                    }

                    //For Thumb
                    try
                    {
                        Decrypt decryptorThumb = new Decrypt();
                        string fileNameToProcessThumbTiff = "TiffThumb" + pdm.PartyID.ToString() + pdm.SROCode.ToString();
                        string fileNameToProcessThumb = "Thumb" + pdm.PartyID.ToString() + pdm.SROCode.ToString();
                        byte[] Thumb = uploaderSoapClient.DownloadScannedPhotoThumb(pdm.ThumbPath);
                        string tempENCThumb = Server.MapPath("~/Content/TempPhotoThumbFiles/tempENCThumb.enc");
                        if (System.IO.File.Exists(tempENCThumb))
                        {
                            System.IO.File.Delete(tempENCThumb);
                        }
                        using (FileStream fs = new FileStream(tempENCThumb, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.Write(Thumb, 0, Thumb.Length);
                        }
                        decryptorThumb.encFilePath = tempENCThumb;
                        decryptorThumb.decFilePath = @decryptFilePath + @"\" + fileNameToProcessThumbTiff + ".tiff";
                        string TiffFileNameThumb = fileNameToProcessThumbTiff + ".tiff";
                        string JPGFileNameThumb = fileNameToProcessThumb + ".jpg";
                        string TiffFilePathThumb = @decryptFilePath + @"\" + fileNameToProcessThumbTiff + ".tiff";
                        string JPGFilePathThumb = @decryptFilePath + @"\" + fileNameToProcessThumb + ".jpg";



                        if (System.IO.File.Exists(TiffFilePathThumb))
                        {
                            System.IO.File.Delete(TiffFilePathThumb);
                        }



                        if (decryptorThumb.DecryptFile("fecdba9876543210", ref errorMessage) == false)
                        //if (DecryptAndSave(decryptorThumb.encFilePath, "fecdba9876543210", decryptorThumb.decFilePath) == false)
                        {
                            pdm.ConvertedThumb = "<div style='padding:75% 0;width:100%; '>Unavailable</div>";

                        }
                        else
                        {
                            if (System.IO.File.Exists(JPGFilePathThumb))
                            {
                                System.IO.File.Delete(JPGFilePathThumb);
                            }
                            var doc = new Document();
                            var builder = new DocumentBuilder(doc);

                            var shape = builder.InsertImage(decryptorThumb.decFilePath);
                            shape.ImageData.Save(JPGFilePathThumb);
                            pdm.ConvertedThumb = @"<img src='\Content\TempPhotoThumbFiles\" + JPGFileNameThumb + "' style='max-width:100%;position: absolute;max-height: 65%;left: 0;margin-right: auto;margin-left: auto;right: 0;top: 0;margin-bottom: auto;bottom: 0;margin-top: auto;'><div style='width: 100%;'><b style=\"text-decoration: underline; text-underline-offset: 5px; \">Uploaded Date</b> <br>" + pdm.UploadDateThumb + "</div>";
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(ex);
                        pdm.ConvertedThumb = "<div style='padding: 1rem; width: 100%; '>Unavailable</div>";
                        continue;
                    }


                    ////--------------------------------------------------------------------------------------------//////

                }

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {

                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }

                    }
                    else
                    {
                        ResModel.PDM = ResModel.PDM.Where(m => m.Fname.ToString().ToLower().Contains(searchValue.ToLower()) || m.Lname.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                    }
                }



                var gridData = ResModel.PDM.Select(PhotoThumbTableModel => new
                {
                    SNo = PhotoThumbTableModel.SrNo,
                    PartyID = PhotoThumbTableModel.PartyID,
                    DocumentID = PhotoThumbTableModel.DocumentID,
                    SROName = ResModel.SROName,
                    Fname = PhotoThumbTableModel.Fname,
                    Lname = PhotoThumbTableModel.Lname,
                    Photo = PhotoThumbTableModel.ConvertedPhoto,
                    Thumb = PhotoThumbTableModel.ConvertedThumb
                });

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum;
                int skip = startLen;
                int totalCount = ResModel.PDM.Count;
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = totalCount,
                    status = "1",
                    recordsFiltered = totalCount
                });

                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = ResModel.ErrorMessage
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }



        public string DeleteReport(string FileName)
        {
            try
            {
                DateTime CurrentDateTime = DateTime.Now;
                string fullName = string.Empty;
                long CurrentTimeStamp = (CurrentDateTime.Ticks - 621355968000000000) / 10000000;
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Server.MapPath("~/Content/TempPhotoThumbFiles/"));
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles();
                foreach (FileInfo foundFile in filesInDir)
                {
                    fullName = foundFile.FullName;
                    DateTime time = foundFile.CreationTime;
                    long timeStamp = (time.Ticks - 621355968000000000) / 10000000;

                    long total = CurrentTimeStamp - timeStamp;

                    if (total > 1799)
                    {
                        System.IO.File.Delete(fullName);
                        Console.WriteLine(fullName + "Deleted");
                    }
                }

                return "Successful";
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        //public bool DecryptAndSave(string encFilePath, string Key, string decFilePath)
        //{
        //    try
        //    {

        //        FileInfo fInfo = new FileInfo(encFilePath); //string to get the encrypted file info
        //        long numBytes = fInfo.Length; //string to store length of the file

        //        FileStream fStream = new FileStream(encFilePath, FileMode.Open, FileAccess.Read); //file stream for encrypte file
        //        BinaryReader br = new BinaryReader(fStream); //Binary reader to read the file stream

        //        byte[] key = Convert.FromBase64String(Key);

        //        byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
        //        byte[] iv = new byte[16];
        //        Array.Copy(ivArr, iv, 16);
        //        // convert the file to a byte array 
        //        Byte[] cipherText = br.ReadBytes((int)numBytes);
        //        br.Close();
        //        fStream.Close();
        //        fStream.Dispose();
        //        // Check arguments.
        //        if (cipherText == null || cipherText.Length <= 0)
        //            throw new ArgumentNullException("cipherText");
        //        if (Key == null || Key.Length <= 0)
        //            throw new ArgumentNullException("Key");

        //        // Declare the string used to hold
        //        // the decrypted text.
        //        //string plaintext = null;
        //        byte[] decrypted;
        //        // Create an Aes object
        //        // with the specified key and IV.
        //        using (Aes aesAlg = Aes.Create())
        //        {
        //            aesAlg.KeySize = 128;
        //            aesAlg.BlockSize = 128;
        //            aesAlg.Key = key;
        //            aesAlg.IV = iv;
        //            aesAlg.Padding = PaddingMode.PKCS7;
        //            aesAlg.Mode = CipherMode.CBC;
        //            // Create a decryptor to perform the stream transform.
        //            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //            // Create the streams used for decryption.
        //            using (MemoryStream msDecrypt = new MemoryStream())
        //            {
        //                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
        //                {
        //                    //using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                    //{

        //                    // Read the decrypted bytes from the decrypting stream
        //                    // and place them in a string.
        //                    csDecrypt.Write(cipherText, 0, cipherText.Length);
        //                    csDecrypt.FlushFinalBlock();
        //                    //plaintext = srDecrypt.ReadToEnd();
        //                    decrypted = msDecrypt.ToArray();

        //                    //}
        //                }
        //            }

        //        }

        //        using (FileStream fs = new FileStream(decFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
        //        {
        //            fs.Seek(0, SeekOrigin.Begin);
        //            fs.Write(decrypted, 0, decrypted.Length);
        //        }
        //        //using (BinaryWriter binWriter = new BinaryWriter(System.IO.File.Open(decFilePath, FileMode.Create)))
        //        //{

        //        //    binWriter.Write(plaintext);
        //        //}
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}