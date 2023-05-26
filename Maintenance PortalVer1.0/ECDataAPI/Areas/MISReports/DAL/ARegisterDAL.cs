using Aregister.Models;
using CustomModels.Models.MISReports.ARegister;
using CustomModels.Security;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ARegisterDAL : IARegister
    {
        KaveriEntities dbContext = null;
        public ARegisterViewModel ARegisterView(ARegisterViewModel viewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();
                ARegisterViewModel aRegisterResultViewModel = new ARegisterViewModel();
                aRegisterResultViewModel.SROfficeList = new List<SelectListItem>();
                List<SROMaster> SROMasterList = new List<SROMaster>();

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == viewModel.OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == viewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                // For SR
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    string kaveriCode = Kaveri1Code.ToString();
                    SelectListItem select = new SelectListItem();
                    select.Text = SroName;
                    select.Value = kaveriCode;
                    aRegisterResultViewModel.SROfficeList.Add(select);
                }
                // For DR
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    aRegisterResultViewModel.SROfficeList.Add(objCommon.GetDefaultSelectListItem("Select", "0"));
                    SROMasterList = dbContext.SROMaster.Where(x => x.DistrictCode == Kaveri1Code).ToList();
                    if (SROMasterList != null)
                    {
                        SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.SRONameE;
                            select.Value = item.SROCode.ToString();
                            aRegisterResultViewModel.SROfficeList.Add(select);
                        }
                    }
                }

                // For others
                else
                {
                    aRegisterResultViewModel.SROfficeList.Add(objCommon.GetDefaultSelectListItem("Select", "0"));
                    SROMasterList = dbContext.SROMaster.ToList();
                    if (SROMasterList != null)
                    {
                        SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.SRONameE;
                            select.Value = item.SROCode.ToString();
                            aRegisterResultViewModel.SROfficeList.Add(select);
                        }
                    }
                }

                return aRegisterResultViewModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ARegisterResultModel GenerateReport(ARegisterViewModel aRegisterViewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                string Url = ConfigurationManager.AppSettings["AregisterByteArrayURL"];
                if (string.IsNullOrEmpty(Url))
                    throw new Exception("A register Byte array url key is missing");
                ARegisterResultModel aRegisterResultModel = new ARegisterResultModel();
                var SroDetails = dbContext.SROMaster.Where(m => m.SROCode == aRegisterViewModel.SROfficeID).FirstOrDefault();
                ARegisterGenerationDetails aRegisterGenerationDetails = dbContext.ARegisterGenerationDetails.
                                                                         Where(m => m.SROCode == aRegisterViewModel.SROfficeID &&
                                                                         m.ReceiptDate.Day == aRegisterViewModel.ForDate_DateTime.Day &&
                                                                         m.ReceiptDate.Month == aRegisterViewModel.ForDate_DateTime.Month &&
                                                                         m.ReceiptDate.Year == aRegisterViewModel.ForDate_DateTime.Year
                                                                         )
                                                                         .FirstOrDefault();
                if (aRegisterGenerationDetails == null)
                {
                    aRegisterResultModel.ResponseStatus = false;
                    aRegisterResultModel.ResponseMessage = "A Register details are not generated,Please try again after some time.";
                    return aRegisterResultModel;
                }
                if(!aRegisterGenerationDetails.IsReceiptsSynchronized && !aRegisterGenerationDetails.IsARegisterGenerated)
                {
                    aRegisterResultModel.ResponseStatus = false;
                    aRegisterResultModel.ResponseMessage = "A Register details are not synchronised,Please try again after some time.";
                    return aRegisterResultModel;
                }
                if (aRegisterGenerationDetails.IsARegisterGenerated)
                {
                    ARegisterFileDetails aRegisterFileDetails = dbContext.ARegisterFileDetails.Where(m =>
                                                                m.SROCode == aRegisterViewModel.SROfficeID &&
                                                                m.ID == aRegisterGenerationDetails.ID).FirstOrDefault();
                    //DateTime MaxDateReceived = Convert.ToDateTime(dbContext.USP_RPT_AREGISTER_GetReceiptMaxDate(aRegisterFileDetails.FileGenerationDate,
                    //                                                  new DateTime(), aRegisterViewModel.SROfficeID).FirstOrDefault());
                    //Changed by mayank on 21/Apr/2022 to get max receipt date of searched  date
                    DateTime MaxDateReceived = Convert.ToDateTime(dbContext.USP_RPT_AREGISTER_GetReceiptMaxDate(aRegisterFileDetails.SearchDate,
                                                                      new DateTime(), aRegisterViewModel.SROfficeID).FirstOrDefault());
                    if(MaxDateReceived>aRegisterFileDetails.FileGenerationDate)
                    {
                        aRegisterResultModel.MessageforGeneration="In Sro "+ SroDetails .SRONameE+ " for date "+ aRegisterViewModel.ForDate_DateTime.ToString("dd/MM/yyyy") + " receipt is generated after ARegister Report generation,Please contact System Admin.";
                        aRegisterResultModel.ResponseStatus = true;
                        return aRegisterResultModel;
                    }
                    else
                    {
                        aRegisterResultModel.MessageforGeneration = string.Empty;
                    }
                        
                    string EncryptedFileID = URLEncrypt.EncryptParameters(new String[] { "FileID=" + aRegisterFileDetails.FileID });
                    aRegisterResultModel.Url = Url + EncryptedFileID;
                    aRegisterResultModel.ResponseStatus = true;
                    return aRegisterResultModel;
                }
                else
                {
                    aRegisterResultModel.ResponseStatus = false;
                    aRegisterResultModel.ResponseMessage = "A Register report is not generated for Date " + aRegisterViewModel.ForDate;
                    return aRegisterResultModel;
                }

                #region Report Generation Code commented by mayank on 09/02/2022
                //if (aRegisterViewModel.LevelID == (int)ApiCommonEnum.LevelDetails.SR)//if SR
                //{
                //    //generate file and save details in database
                //    ARegisterPdfResultModel aRegisterPdfResultModel = new ARegisterPdfResultModel();
                //    var rPT_ARegisters = dbContext.RPT_ARegister(aRegisterViewModel.ForDate, aRegisterViewModel.SROfficeID).ToList();

                //    aRegisterPdfResultModel.ARegister_Result = rPT_ARegisters;
                //    aRegisterViewModel.SroName = SroDetails.SRONameE;
                //    aRegisterPdfResultModel.viewModel = aRegisterViewModel;
                //    //aRegisterPdfResultModel.ARegister_Result = GetDummyDetails();
                //    byte[] resultbytes = CreateAregisterPDF(aRegisterPdfResultModel);
                //    if (resultbytes != null)
                //    {
                //        ARegisterFileDetails aRegisterFileDetails = new ARegisterFileDetails();
                //        //long test = dbContext.ARegisterFileDetails.Max(m => m.FileID);
                //        if (dbContext.ARegisterFileDetails.Any())
                //        {
                //            aRegisterFileDetails.FileID = Convert.ToInt64(dbContext.ARegisterFileDetails.Max(m => m.FileID)) + 1;
                //        }
                //        else
                //        {
                //            aRegisterFileDetails.FileID = 1;
                //        }
                //        aRegisterFileDetails.DistrictCode = null;
                //        aRegisterFileDetails.SROCode = aRegisterViewModel.SROfficeID;
                //        aRegisterFileDetails.SearchSROCode = aRegisterViewModel.SROfficeID;
                //        aRegisterFileDetails.SearchDROCode = SroDetails.DistrictCode;
                //        aRegisterFileDetails.SearchDate = aRegisterViewModel.ForDate_DateTime;
                //        aRegisterFileDetails.FileGenerationDate = DateTime.Now;
                //        aRegisterFileDetails.ISDROffice = false;
                //        aRegisterFileDetails.UserID = aRegisterViewModel.UserID;
                //        string MaintaincePortalVirtualDirectoryPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualDirectoryPath"];
                //        if (string.IsNullOrEmpty(MaintaincePortalVirtualDirectoryPath))
                //            throw new Exception("Virtual path key is missing");
                //        //"/ ARegister / 2021 / Month / TRILetter / Report.PDF"
                //        string DirectoryVPath = "ARegister//" + DateTime.Now.Year + "//" + DateTime.Now.ToString("MMM") + "//" + SroDetails.ShortNameE;
                //        if (!Directory.Exists(MaintaincePortalVirtualDirectoryPath + "//" + DirectoryVPath))
                //            Directory.CreateDirectory(MaintaincePortalVirtualDirectoryPath + "//" + DirectoryVPath);
                //        string FileName = "File" + aRegisterFileDetails.FileID + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + ".pdf";
                //        string PhysicalFilePath = MaintaincePortalVirtualDirectoryPath + "//" + DirectoryVPath + "//" + FileName;
                //        try
                //        {
                //            File.WriteAllBytes(PhysicalFilePath, resultbytes);
                //        }
                //        catch (Exception)
                //        {

                //            throw;
                //        }
                //        if (!File.Exists(PhysicalFilePath))
                //            throw new Exception("File writing failed");

                //        aRegisterFileDetails.PFilePath = PhysicalFilePath;
                //        aRegisterFileDetails.VFilePath = "~/" + DirectoryVPath + "//" + FileName;
                //        aRegisterFileDetails.UserID = aRegisterViewModel.UserID;
                //        aRegisterFileDetails.ID = aRegisterGenerationDetails.ID;
                //        dbContext.ARegisterFileDetails.Add(aRegisterFileDetails);
                //        aRegisterGenerationDetails.IsARegisterGenerated = true;
                //        dbContext.Entry(aRegisterGenerationDetails).State = System.Data.Entity.EntityState.Modified;
                //        dbContext.SaveChanges();

                //        //pdf file view
                //        //aRegisterResultModel.Url = Url + aRegisterFileDetails.FileID;
                //        string EncryptedFileID = URLEncrypt.EncryptParameters(new String[] { "FileID=" + aRegisterFileDetails.FileID });
                //        aRegisterResultModel.Url = Url + EncryptedFileID;
                //    }
                //    aRegisterResultModel.ResponseStatus = true;
                //    return aRegisterResultModel;
                //}
                //else//for others aigrcomp,DR,techadmin
                //{
                //    aRegisterResultModel.ResponseStatus = false;
                //    aRegisterResultModel.ResponseMessage = "A Register report is not generated for Date " + aRegisterViewModel.ForDate + ",Please ask SR " + SroDetails.SRONameE + " to generate Report";
                //    return aRegisterResultModel;
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ARegisterResultModel ViewARegisterReport(string FileID)
        {
            try
            {
                ARegisterResultModel aRegisterResultModel = new ARegisterResultModel();
                dbContext = new KaveriEntities();
                long Lng_FileID = Convert.ToInt64(FileID);
                string filepath = dbContext.ARegisterFileDetails.Where(y => y.FileID == Lng_FileID).Select(z => z.VFilePath).FirstOrDefault().ToString();
                string MaintaincePortalVirtualSitePath = ConfigurationManager.AppSettings["MaintaincePortalVirtualSite"];
                filepath = filepath.Replace("~", "");
                //aRegisterResultModel.AregisterFileBytes = File.ReadAllBytes(MaintaincePortalVirtualDirectoryPath + "//" + filepath);

                WebClient webClient = new WebClient();
                Stream strm = webClient.OpenRead(new Uri(MaintaincePortalVirtualSitePath + "//" + filepath));
                using (MemoryStream ms = new MemoryStream())
                {
                    strm.CopyTo(ms);
                    aRegisterResultModel.AregisterFileBytes = ms.ToArray();
                }
                return aRegisterResultModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}