#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ScannedFileDownloadDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -22-10-2019
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion


using CustomModels.Models.Utilities.ScannedfileDownload;
using ECDataAPI.Areas.Utilities.Controllers;
using ECDataAPI.Common;
using ECDataAPI.EcDataService;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using ECDataAPI.Entity.KaveriEntities;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Utilities.DAL
{
    public class ScannedFileDownloadDAL : IScannedFileDownload
    {
        KaveriEntities dbContext = null;

        /// <summary>
        /// returns Model which contains log table 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        public List<SelectListItem> GetFinancialYearList()
        {
            List<SelectListItem> financialYearList = new List<SelectListItem>();
            DateTime startYear = Convert.ToDateTime(new DateTime(2003, 1, 1)); //("1/1/2000");
            DateTime today = DateTime.Now.AddYears(1);
            string financialYear = null;
            SelectListItem item = new SelectListItem();
            item.Value = "";
            item.Text = "Select";
            financialYearList.Add(item);

            while (today.Year >= startYear.Year)
            {
                SelectListItem listItem = new SelectListItem();
                financialYear = string.Format("{0}-{1}", startYear.AddYears(-1).Year.ToString(), startYear.Year.ToString().Substring(2, 2));
                listItem.Value = financialYear;
                listItem.Text = financialYear;
                financialYearList.Add(listItem);
                startYear = startYear.AddYears(1);
            }
            return financialYearList;
        }

        public List<SelectListItem> GetAllBookTypes()
        {
            dbContext = new KaveriEntities();
            List<SelectListItem> bookTypes = null;

            try
            {
                List<BookTypeMaster> bookTypesList = dbContext.BookTypeMaster.ToList();
                //Added By Tushar for Download Enclosure on 23 march 2022
                bookTypesList.RemoveRange(1, 1);
                //bookTypesList.RemoveRange(1, 3);
                bookTypesList.RemoveRange(9, 2);
                //End By Tushar for Download Enclosure on 23 march 2022

                bookTypes = bookTypesList.Select(x => new SelectListItem
                {
                    Text = x.BookNameEnglish,
                    Value = x.BookID.ToString()
                }).ToList();

                bookTypes.Insert(0, new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

                return bookTypes;
            }
            catch
            {
                return new List<SelectListItem>();
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        public ScannedFileDownloadView ScannedFileDownloadView(int OfficeID)
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            ScannedFileDownloadView ViewModel = new ScannedFileDownloadView();

            try
            {
                dbContext = new KaveriEntities();
                string FirstRecord = "Select";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                ViewModel.SROfficeList = new List<SelectListItem>();
                ViewModel.DROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);
                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList.Add(sroNameItem);
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, FirstRecord);
                }
                else
                {
                    SelectListItem ItemAll = new SelectListItem();
                    ItemAll.Text = "Select";
                    ItemAll.Value = "0";
                    ViewModel.SROfficeList.Add(ItemAll);
                    ViewModel.DROfficeList = objCommon.GetDROfficesList("Select");
                }
                ViewModel.FinancialYear = GetFinancialYearList();
                ViewModel.BookType = GetAllBookTypes();
                //Added By Tushar on 23 March 2022
                ViewModel.ReasonDetaills = GetAllReasonDetaills();
                var GetDocumentList = objCommon.GetDocumentType();
                GetDocumentList.RemoveRange(4, 1);
                ViewModel.DocumentType = GetDocumentList;

                //End By Tushar on 23 March 2022
                #region Added by mayank on 24/Mar/2022
                ViewModel.MarriageType = objCommon.GetMarriageType();

                #endregion
                //Added By tushar on 1 April 2022
                ViewModel.NoticeType = objCommon.GetNoticeType();
                //End By Tushar on 1 April 2022
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
            return ViewModel;
        }
        public ScannedFileDownloadView LoadScannedFileDownloadLogTable(long UserID)
        {
            ScannedFileDownloadView ScannedFileDownloadModel = new ScannedFileDownloadView();
            ScannedFileLogTableModel TableRow = null;
            String UserName = string.Empty;
            List<ScannedFileLogTableModel> ScannedFileDownloadList = new List<ScannedFileLogTableModel>();
            int Counter = 0;
            try
            {
                dbContext = new KaveriEntities();

                var ScannedFileDownloadLogTable = (from ScannedFileDownloadLogTab in dbContext.ScannedFileDownloadLog
                                                   select new
                                                   {
                                                       ScannedFileDownloadLogTab.FRN,
                                                       ScannedFileDownloadLogTab.FileName,
                                                       ScannedFileDownloadLogTab.Filepath,
                                                       //ScannedFileDownloadLogTab.DownloadedBY,
                                                       ScannedFileDownloadLogTab.DownloadReason,
                                                       ScannedFileDownloadLogTab.DownloadDateTime,
                                                       ScannedFileDownloadLogTab.SROName,
                                                       ScannedFileDownloadLogTab.UserID,
                                                       ScannedFileDownloadLogTab.ReasonID
                                                   }).ToList();

                // by shubham 05-05-2021 add condition to filter records of a sr and dr accourding to userid 


                if (ScannedFileDownloadLogTable != null)
                {
                    ScannedFileDownloadLogTable = ScannedFileDownloadLogTable.OrderByDescending(x => x.DownloadDateTime).ToList();
                    if (UserID == 6)//UserId of AigrComp
                    {
                        ScannedFileDownloadLogTable = ScannedFileDownloadLogTable.Where(x => x.UserID == UserID).ToList();
                    }
                }

                ScannedFileDownloadModel.ScannedFileDownloadList = new List<ScannedFileLogTableModel>();
                if (ScannedFileDownloadLogTable != null)
                {
                    foreach (var item in ScannedFileDownloadLogTable)
                    {
                        UserName = dbContext.UMG_UserProfile.Where(x => x.UserID == item.UserID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                        TableRow = new ScannedFileLogTableModel();
                        TableRow.SrNo = Counter++;
                        TableRow.FRN = Convert.ToString(item.FRN);
                        TableRow.SroName = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                        TableRow.FileName = string.IsNullOrEmpty(item.FileName) ? string.Empty : item.FileName;
                        TableRow.Filepath = string.IsNullOrEmpty(item.Filepath) ? string.Empty : item.Filepath;
                        //TableRow.DownloadedBY = string.IsNullOrEmpty(item.DownloadedBY) ? string.Empty : item.DownloadedBY;
                        TableRow.DownloadedBY = string.IsNullOrEmpty(UserName) ? string.Empty : UserName;
                        if (item.ReasonID == null || item.ReasonID == 0)
                        {
                            TableRow.DownloadReason = string.IsNullOrEmpty(item.DownloadReason) ? string.Empty : item.DownloadReason;
                        }
                        else
                        {
                            TableRow.DownloadReason = dbContext.MAS_EnclosureDownloadReasons.
                                                      Where(m => m.ReasonID == item.ReasonID).
                                                      Select(m => m.ReasonDetaills).FirstOrDefault();
                        }
                        if (item.DownloadDateTime != null)
                        {
                            TableRow.DownloadDateTime = Convert.ToDateTime(item.DownloadDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            TableRow.DownloadDateTime = "";
                        }
                        ScannedFileDownloadModel.ScannedFileDownloadList.Add(TableRow);
                    }
                }
                return ScannedFileDownloadModel;
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

        /// <summary>
        /// returns Byte Array From ScannedFileDownloadDAL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScannedFileDownloadResModel GetScannedFileByteArray(ScannedFileDownloadView ReqModel)
        {
            ScannedFileDownloadResModel ResponseModel = new ScannedFileDownloadResModel();
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                dbContext = new KaveriEntities();
                ECDataService service = new ECDataService();
                //FileContentRequestModel reqModel = new FileContentRequestModel();
                ScannedFileContentRequestModel reqModel = new ScannedFileContentRequestModel();
                //FileContentResponseModel fileContentResponseModel = null;
                ScannedFileContentResponseModel ServiceResponseModel = null;
                string ScannedFileName = string.Empty;
                string JustName = string.Empty;
                ResponseModel.UserName = dbContext.UMG_UserDetails.Where(x => x.UserID == ReqModel.UserID).Select(x => x.UserName).FirstOrDefault();

                var SrName = dbContext.SROMaster.Where(x => x.SROCode == ReqModel.SROfficeID && x.DistrictCode == ReqModel.DROfficeID).Select(n => n.ShortNameE).FirstOrDefault();
                string dc_num = null;
                //Added By Tushar on 14 Dec 2022
                string PrintableBookName = string.Empty;
                //End By Tushar on 14 Dec 2022
                if (ReqModel.DocumentTypeID == 1)
                {
                    if (ReqModel.DocumentNumber < 10)
                    {
                        dc_num = "0000" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 100)
                    {
                        dc_num = "000" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 1000)
                    {
                        dc_num = "00" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 10000)
                    {
                        dc_num = "0" + ReqModel.DocumentNumber.ToString();
                    }
                    else if (ReqModel.DocumentNumber >= 10000)
                    {
                        dc_num = ReqModel.DocumentNumber.ToString();
                    }
                    //Commented and Added By Tushar on 14 Dec 2022 
                    //string PrintableBookName = dbContext.BookTypeMaster.Where(m => m.BookID == ReqModel.BookTypeID).
                    //                           Select(t => t.PrintableName).FirstOrDefault();
                    PrintableBookName = dbContext.BookTypeMaster.Where(m => m.BookID == ReqModel.BookTypeID).
                                               Select(t => t.PrintableName).FirstOrDefault();
                    ScannedFileName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr;
                }
                else if (ReqModel.DocumentTypeID == 2)
                {
                    string[] marriageTypes = { "HM", "S", "SO", "O" };
                    ScannedFileName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr;
                }
                //Added By Tushar on 1April 2022 for add Document Type Notice
                else if(ReqModel.DocumentTypeID == 3)
                {
                    //string[] noticeType = { "", "", "" };
                    //string[] noticeType = { "HN" , "SN", "ON" , "O"  };
                    string[] noticeType = { "SN", "ON", "HN", "O" };

                    ScannedFileName = SrName + "-" + noticeType[ReqModel.NoticeTypeListID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + noticeType[ReqModel.NoticeTypeListID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr;
                
            }
                //End By Tushar on 1 April 2022


                var ScannedFileUploadDetails = dbContext.ScannedFileUploadDetails.Where(x => x.ScannedFileName == ScannedFileName).Select(n => new
                {
                    n.ScannedFileName,
                    n.UploadPath,
                    n.RootDirectory,
                    n.DocumentID,
                    n.SROCode
                }).FirstOrDefault();
                //Added By Tushar on 14 Dec 2022
                if (ScannedFileUploadDetails == null && ReqModel.DocumentTypeID == 1)
                {
                    SrName = dbContext.SROBifurcationTriLetterDetails.Where(x => x.Srocode == ReqModel.SROfficeID).Select(n => n.PreviousTriLetter).FirstOrDefault();

                    if (SrName != null)
                    {
                        ScannedFileName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr + ".enc";
                        JustName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr;

                        ScannedFileUploadDetails = dbContext.ScannedFileUploadDetails.Where(x => x.ScannedFileName == ScannedFileName).Select(n => new
                        {
                            n.ScannedFileName,
                            n.UploadPath,
                            n.RootDirectory,
                            n.DocumentID,
                            n.SROCode
                        }).FirstOrDefault();
                    }
                }
                // End BY Tushar on 14 Dec 2022
                if (ScannedFileUploadDetails != null)
                {
                    string DownloadPath = string.Empty;
                    DownloadPath = Path.Combine(ScannedFileUploadDetails.RootDirectory, ScannedFileUploadDetails.UploadPath, ScannedFileUploadDetails.ScannedFileName);
                    if (ScannedFileUploadDetails.UploadPath.Length > 4)
                    {
                        if (ScannedFileUploadDetails.UploadPath.Substring(ScannedFileUploadDetails.UploadPath.Length - 4, 4) != ".enc")
                            DownloadPath = Path.Combine(ScannedFileUploadDetails.RootDirectory, ScannedFileUploadDetails.UploadPath, ScannedFileUploadDetails.ScannedFileName);
                        else
                            DownloadPath = Path.Combine(ScannedFileUploadDetails.RootDirectory, ScannedFileUploadDetails.UploadPath);
                    }

                    reqModel.FilePath = DownloadPath.Replace("|", " ");
                    ResponseModel.FileNameWithExt = ScannedFileName;
                    ResponseModel.FileNameWithoutExt = JustName;

                }
                else
                {
                    ResponseModel.IsError = true;
                    if (ReqModel.DocumentTypeID == 1)
                        ResponseModel.ErrorMessage = "No data found for entered Document Number";
                    else if (ReqModel.DocumentTypeID == 2)
                        ResponseModel.ErrorMessage = "No data found for entered Marriage Case Number";
                    else if(ReqModel.DocumentTypeID == 3)
                    {
                        ResponseModel.ErrorMessage = "No data found for entered Notice Number";
                    }
                    return ResponseModel;
                }

                //fileContentResponseModel = service.GetFileContent(reqModel);
                ServiceResponseModel = service.GetScannedFileContent(reqModel);

                //Stream stream = new MemoryStream(fileContentResponseModel.FileContent);
                //string Checksum = CalculateMD5(stream);


                if (ServiceResponseModel != null)
                {
                    //To Create a temporary file to write contents of byte array
                    #region Temporary File
                    //if (!Directory.Exists(RootPath))
                    //    Directory.CreateDirectory(RootPath);

                    //using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    //{
                    //    stream.Write(fileContentResponseModel.FileContent, 0, fileContentResponseModel.FileContent.Length);
                    //    stream.Close();
                    //}
                    //long lngCheckSum = objTestCheckSum.GetCRCFromFile(FilePath);

                    //if (CheckSum == lngCheckSum)
                    //{
                    if (ServiceResponseModel.FileExists)
                    {
                        ResponseModel.ScannedFileByteArray = ServiceResponseModel.FileContent;
                        List<string> RefMasterList = new List<string>();
                        if (ReqModel.DocumentTypeID == 1)
                            RefMasterList = dbContext.ReferenceMaster.
                                                       Where(m => m.DocumentID == ScannedFileUploadDetails.DocumentID &&
                                                       m.SROCode == ScannedFileUploadDetails.SROCode).
                                                       Select(refe => refe.ReferenceText).ToList();

                        else if (ReqModel.DocumentTypeID == 2)
                            RefMasterList = dbContext.ReferenceMaster.
                                                       Where(m => m.RegistrationID == ScannedFileUploadDetails.DocumentID &&
                                                       m.SROCode == ScannedFileUploadDetails.SROCode).
                                                       Select(refe => refe.ReferenceText).ToList();
                        else if (ReqModel.DocumentTypeID == 3)
                            RefMasterList = dbContext.ReferenceMaster.
                                                       Where(m => m.NoticeID == ScannedFileUploadDetails.DocumentID &&
                                                       m.SROCode == ScannedFileUploadDetails.SROCode).
                                                       Select(refe => refe.ReferenceText).ToList();

                        if (RefMasterList != null && RefMasterList.Count > 0)
                        {
                            ResponseModel.ReferenceString = string.Empty;
                            ResponseModel.IsReferenceStringExist = true;
                            RefMasterList.ForEach(m =>
                            { ResponseModel.ReferenceString += m; });
                            ResponseModel.ReferenceString= ResponseModel.ReferenceString.Replace("\n", " ");
                        }
                    }
                    else
                    {
                        ResponseModel.IsError = true;
                        ResponseModel.ErrorMessage = ServiceResponseModel.ResponseMessage;
                        return ResponseModel;
                    }


                    //}
                    //else
                    //{
                    //    ResponseModel.ScannedFileByteArray = null;//Handeled in Controller
                    //}
                    #endregion

                    //Delete temporary file
                    // objCommon.DeleteFileFromTemporaryFolder(FilePath);

                }
                else
                {
                    ResponseModel.IsError = true;
                    ResponseModel.ErrorMessage = "Internal Service Error Occured,Please contact administrator.";
                    return ResponseModel;
                }

                //}
                //else
                //{
                //    ResponseModel.IsError = true;
                //    ResponseModel.ErrorMessage = "No data found for entered Final Registration Number.";
                //    return ResponseModel;
                //}
                return ResponseModel;
            }
            catch (Exception ex)
            {
                 throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            //return ResponseModel;
        }

        /// <summary>
        /// returns Status (whether Scanned File id downloaded)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveScannedFileDownloadDetails(ScannedFileDownloadView ReqModel)
        {
            bool SaveStatus = false;
            try
            {
                dbContext = new KaveriEntities();
                ECDataService service = new ECDataService();
                string ScannedFileName = string.Empty;
                string JustName = string.Empty;
                //var DocMaster = dbContext.DocumentMaster.Where(x => x.FinalRegistrationNumber == ReqModel.FRN).Select(n => new
                //{
                //    n.DocumentID,
                //    n.SROCode
                //}).FirstOrDefault();

                String UserName = dbContext.UMG_UserProfile.Where(x => x.UserID == ReqModel.UserID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();



                //var ScannedFileUploadDetails = dbContext.ScannedFileUploadDetails.Where(x => x.SROCode == DocMaster.SROCode && x.DocumentID == DocMaster.DocumentID && x.DocumentTypeID == 1).Select(n => new
                //{
                //    n.ScannedFileName,
                //    n.UploadPath,
                //    n.RootDirectory
                //}).FirstOrDefault();
                var SrName = dbContext.SROMaster.Where(x => x.SROCode == ReqModel.SROfficeID && x.DistrictCode == ReqModel.DROfficeID).Select(n => n.ShortNameE).FirstOrDefault();
                string dc_num = null;
                //Added By Tushar on 14 Dec 2022
                string PrintableBookName = string.Empty;
                //End By Tushar on 14 Dec 2022
                if (ReqModel.DocumentTypeID == 1)
                {
                    if (ReqModel.DocumentNumber < 10)
                    {
                        dc_num = "0000" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 100)
                    {
                        dc_num = "000" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 1000)
                    {
                        dc_num = "00" + ReqModel.DocumentNumber.ToString();
                    }

                    else if (ReqModel.DocumentNumber < 10000)
                    {
                        dc_num = "0" + ReqModel.DocumentNumber.ToString();
                    }
                    else if (ReqModel.DocumentNumber >= 10000)
                    {
                        dc_num = ReqModel.DocumentNumber.ToString();
                    }
                    //Commented and Added By Tushar on 14 Dec 2022 
                    //string PrintableBookName = dbContext.BookTypeMaster.Where(m => m.BookID == ReqModel.BookTypeID).
                    //                           Select(t => t.PrintableName).FirstOrDefault();
                    PrintableBookName = dbContext.BookTypeMaster.Where(m => m.BookID == ReqModel.BookTypeID).
                                              Select(t => t.PrintableName).FirstOrDefault();
                    //ScannedFileName = SrName + "-" + ReqModel.BookTypeID + "-" + dc_num + "-" + ReqModel.FinancialYearStr + ".enc";
                    ScannedFileName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr;
                    //JustName = SrName + "-" + ReqModel.BookTypeID + "-" + dc_num + "-" + ReqModel.FinancialYearStr;
                }
                //Commented and Added By Tushar on 1 April 2022 for add Document Type Notice 
                //else
                //{
                //    string[] marriageTypes = { "HM", "S", "SO", "O" };
                //    ScannedFileName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr + ".enc";
                //    JustName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr;
                //}
                 else if(ReqModel.DocumentTypeID == 2)
                {
                    string[] marriageTypes = { "HM", "S", "SO", "O" };
                    ScannedFileName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + marriageTypes[ReqModel.MarriageTypeID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr;
                }
                else if (ReqModel.DocumentTypeID == 3)
                {
                    //string[] noticeType = { "", "", "" };
                    //string[] noticeType = { "HN", "SN", "ON", "O" };
                    string[] noticeType = { "SN", "ON", "HN", "O" };
                    ScannedFileName = SrName + "-" + noticeType[ReqModel.NoticeTypeListID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr + ".enc";
                    JustName = SrName + "-" + noticeType[ReqModel.NoticeTypeListID - 1] + ReqModel.DocumentNumber + "-" + ReqModel.FinancialYearStr;

                }
                //End BY Tushar on 1 April 2022
                var ScannedFileUploadDetails = dbContext.ScannedFileUploadDetails.Where(x => x.ScannedFileName == ScannedFileName).Select(n => new
                {
                    n.ScannedFileName,
                    n.UploadPath,
                    n.RootDirectory,
                    n.SROCode
                }).FirstOrDefault();
                //Added By Tushar on 14 Dec 2022
                if (ScannedFileUploadDetails == null && ReqModel.DocumentTypeID == 1)
                {
                    SrName = dbContext.SROBifurcationTriLetterDetails.Where(x => x.Srocode == ReqModel.SROfficeID).Select(n => n.PreviousTriLetter).FirstOrDefault();

                    if (SrName != null)
                    {
                        ScannedFileName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr + ".enc";
                        JustName = SrName + "-" + PrintableBookName + "-" + dc_num + "-" + ReqModel.FinancialYearStr;

                        ScannedFileUploadDetails = dbContext.ScannedFileUploadDetails.Where(x => x.ScannedFileName == ScannedFileName).Select(n => new
                        {
                            n.ScannedFileName,
                            n.UploadPath,
                            n.RootDirectory,
                            n.SROCode
                        }).FirstOrDefault();
                    }
                }
                //End By Tushar on 14 Dec 2022
                if (ScannedFileUploadDetails != null)
                {
                    string DownloadPath = Path.Combine(ScannedFileUploadDetails.RootDirectory, ScannedFileUploadDetails.UploadPath);
                    //string SROName = dbContext.SROMaster.Where(x => x.SROCode == DocMaster.SROCode).Select(x => x.SRONameE).FirstOrDefault();

                    string SROName = dbContext.SROMaster.Where(x => x.SROCode == ScannedFileUploadDetails.SROCode).Select(x => x.SRONameE).FirstOrDefault();

                    int LogID;
                    if (dbContext.ScannedFileDownloadLog.Any())
                    {
                        LogID = dbContext.ScannedFileDownloadLog.Max(x => x.LogID) + 1;
                    }
                    else
                    {
                        LogID = 1;
                    }
                    ScannedFileDownloadLog ScannedFileDownloadLog = new ScannedFileDownloadLog();
                    ScannedFileDownloadLog.LogID = LogID;
                    //ScannedFileDownloadLog.DocumentID = DocMaster.DocumentID;
                    ScannedFileDownloadLog.SROCode = ScannedFileUploadDetails.SROCode;
                    ScannedFileDownloadLog.FRN = JustName;
                    ScannedFileDownloadLog.FileName = ScannedFileUploadDetails.ScannedFileName;
                    ScannedFileDownloadLog.Filepath = DownloadPath;
                    //ScannedFileDownloadLog.DownloadedBY = string.IsNullOrEmpty(UserName) ? "" : UserName;
                    ScannedFileDownloadLog.UserID = ReqModel.UserID;
                    //ScannedFileDownloadLog.DownloadReason = ReqModel.DownloadReason;
                    ScannedFileDownloadLog.DownloadDateTime = DateTime.Now;
                    ScannedFileDownloadLog.SROName = SROName;
                    //Added By Tushar on 23 March 2022
                    ScannedFileDownloadLog.ReasonID = Convert.ToByte(ReqModel.ReasonID);
                    ScannedFileDownloadLog.DocumentTypeID = Convert.ToByte(ReqModel.DocumentTypeID);
                    // End BY Tushar on 23 March 2022
                    dbContext.ScannedFileDownloadLog.Add(ScannedFileDownloadLog);
                    dbContext.SaveChanges();
                    SaveStatus = true;
                }
                return SaveStatus;
            }
            catch (Exception EX)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            //return SaveStatus;
        }


        static string CalculateMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                //using (var stream = File.OpenRead(filename))
                //{
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                // }
            }
        }

        //Added By Tushar on 23 March 2022
        public List<SelectListItem> GetAllReasonDetaills()
        {
            dbContext = new KaveriEntities();
            List<SelectListItem> ReasonDetaills = null;

            try
            {
                List<MAS_EnclosureDownloadReasons> ReasonDetaillsList = dbContext.MAS_EnclosureDownloadReasons.ToList();

                ReasonDetaills = ReasonDetaillsList.Select(x => new SelectListItem
                {
                    Text = x.ReasonDetaills,
                    Value = x.ReasonID.ToString()
                }).ToList();

                ReasonDetaills.Insert(0, new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

                return ReasonDetaills;
            }
            catch
            {
                return new List<SelectListItem>();
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        //End By Tushar on 23 March 2022

    }
}