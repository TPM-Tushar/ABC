#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsDAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   DAL layer for Support Enclosure
*/
#endregion

using CustomModels.Models.DataEntryCorrection;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Security;
using System.Text;
using System.IO;
using ECDataUI.Session;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Transactions;
using System.Data;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace ECDataAPI.Areas.DataEntryCorrection.DAL
{
    public class ReScanningApplicationDAL : IReScanningApplication
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();




        //***********************************************************************************************************************************************************************************


        public ReScanningApplicationViewModel ReScanningApplicationView(int officeID)
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            ReScanningApplicationViewModel ViewModel = new ReScanningApplicationViewModel();

            try
            {
                dbContext = new KaveriEntities();
                string FirstRecord = "Select";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                ViewModel.SROfficeList = new List<SelectListItem>();
                ViewModel.DROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                //if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                //{
                //    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                //    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                //    string DroCode_string = Convert.ToString(DroCode);
                //    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    ViewModel.DROfficeList.Add(droNameItem);
                //    ViewModel.SROfficeList.Add(sroNameItem);
                //}
                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
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


        //***********************************************************************************************************************************************************************************


        public string PCount(int SROCode, string DocNo, string Fyear, string BType)
        {
            try
            {
                dbContext = new KaveriEntities();
                int? PgCount = -1;


                if (BType == "0" && Fyear == "0")
                {
                    PgCount = dbContext.MarriageRegistration.Where(m => m.MarriageCaseNo == DocNo && m.SROCode == SROCode).Select(y => y.Pages).FirstOrDefault();
                }
                else
                {
                    string TriLetter = dbContext.SROMaster.Where(x => x.SROCode == SROCode).Select(y => y.ShortNameE).FirstOrDefault();
                    string FRN = TriLetter + "-" + BType + "-" + Convert.ToInt64(DocNo).ToString("D5") + "-" + Fyear;
                    PgCount = dbContext.DocumentMaster.Where(x => x.FinalRegistrationNumber == FRN).Select(y => y.PageCount).FirstOrDefault();
                }


                if (PgCount == null || PgCount == -1)
                {
                    return "Not Available";
                }

                return PgCount.ToString();
            }
            catch (Exception ex)
            {
                return "Not Available";
            }
        }


        //***********************************************************************************************************************************************************************************


        public DetailModel btnShowClick(int DRO, int SRO, string OrderNo, string Date, string DocNo, string DType, string BType, string FYear)
        {
            try
            {
                dbContext = new KaveriEntities();
                int DocumentTypeID = (DType == "MARR") ? 2 : 1;
                DetailModel res = new DetailModel();

                res.TriLetter = dbContext.SROMaster.Where(x => x.SROCode == SRO).Select(y => y.ShortNameE).FirstOrDefault();




                if (DocumentTypeID == 2)
                {
                    res.DocID = dbContext.MarriageRegistration.Where(y => y.MarriageCaseNo == DocNo && y.SROCode == SRO).Select(x => x.RegistrationID).FirstOrDefault();
                    //res.FRN = dbContext.DocumentMaster.Where(y => y.DocumentID == res.DocID && y.SROCode == SRO).Select(x => x.FinalRegistrationNumber).FirstOrDefault();

                }
                else
                {
                    long docNum = Convert.ToInt64(DocNo);
                    string FRN = GetFRN(SRO, BType, FYear, DocNo);
                    res.DocID = dbContext.DocumentMaster.Where(y => y.FinalRegistrationNumber == FRN).Select(x => x.DocumentID).FirstOrDefault();
                    string FruitsCheck = (dbContext.DocumentMaster.Where(y => y.FinalRegistrationNumber == FRN).Select(x => x.RemarksByUser).FirstOrDefault() != null) ? dbContext.DocumentMaster.Where(y => y.FinalRegistrationNumber == FRN).Select(x => x.RemarksByUser).FirstOrDefault() : "---";
                    if (FruitsCheck.Contains(@"filed automatically by system") && FruitsCheck.Contains("KAVERI-FRUITS"))
                    {
                        res.DocID = -1111;
                    }
                    //res.FRN = dbContext.DocumentMaster.Where(y => y.DocumentID == res.DocID && y.SROCode == SRO).Select(x => x.FinalRegistrationNumber).FirstOrDefault();

                }



                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //***********************************************************************************************************************************************************************************


        public List<ReScanningApplicationOrderTableModel> LoadDocDetailsTable(int DROCode)
        {
            int SNo = 0;
            try
            {
                dbContext = new KaveriEntities();

                List<ReScanningApplicationOrderTableModel> OTL = new List<ReScanningApplicationOrderTableModel>();
                List<USP_DEC_RescanApp_OrderList_Result> ODList = dbContext.USP_DEC_RescanApp_OrderList(DROCode, 0).ToList();
                ODList = ODList.OrderByDescending(x => x.InsertDateTime).ToList<USP_DEC_RescanApp_OrderList_Result>();
                if (ODList != null)
                {

                }
                foreach (var a in ODList)
                {
                    SNo++;
                    string OD = (a.OrderDate != null) ? Convert.ToDateTime(a.OrderDate).ToString("dd-MM-yyyy") : "-";
                    string InsD = (a.InsertDateTime != null) ? Convert.ToDateTime(a.InsertDateTime).ToString("dd-MM-yyyy HH:mm") : "-";
                    ReScanningApplicationOrderTableModel RSAModel = new ReScanningApplicationOrderTableModel();
                    RSAModel.SNo = SNo.ToString();
                    RSAModel.SROName = a.SROName;
                    RSAModel.DROrderNumber = a.DROrderNumber != null ? a.DROrderNumber : "-";
                    RSAModel.OrderDate = OD;
                    RSAModel.DocTypeID = a.DocTypeID;
                    RSAModel.EntryDate = InsD;
                    RSAModel.RegistrationNumber = a.RegistrationNumber;
                    string pth = (a.AbsoluteFilePath == null || a.AbsoluteFilePath == "") ? "" : a.AbsoluteFilePath.Replace(@"\", "/");
                    RSAModel.ViewBtn = "<a href='javascript:void(0)' onClick=\"ViewBtnClickOrderTable('" + @pth + "')\" download='" + a.DocumentID + "_" + a.SROCode + @".pdf'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>";
                    RSAModel.DistrictName = a.DRONAME != null ? a.DRONAME : "-";
                    RSAModel.EnteredBY = a.ENTERED_BY;
                    RSAModel.IsActive = (a.IsActive != null) ? a.IsActive : false;
                    RSAModel.isReScanCompleted = a.IsFileUploaded;

                    OTL.Add(RSAModel);
                }

                //OTL = OTL.OrderByDescending(x => x.EntryDate).ToList<ReScanningApplicationOrderTableModel>();

                return OTL;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //***********************************************************************************************************************************************************************************


        public DROrderFilePathResultModel ViewBtnClickOrderTable(string path)
        {
            try
            {
                DROrderFilePathResultModel dROrderFilePathResultModel = new DROrderFilePathResultModel();
                //using (dbContext = new KaveriEntities())
                //{
                //    long Lng_FileID = Convert.ToInt64(DocumentID);
                //    string filepath = "";
                //    if (DocTypeID == 1)
                //        filepath = dbContext.DocumentRescanDetails.Where(y => y.DocumentId == DocumentID && y.SROCode == SROCode).Select(z => z.AbsoluteFilePath).FirstOrDefault().ToString();
                //    else if (DocTypeID == 2)
                //        filepath = dbContext.MarriageRescanDetails.Where(y => y.MarriageId == DocumentID && y.SROCode == SROCode).Select(z => z.AbsoluteFilePath).FirstOrDefault().ToString();

                //    string MaintaincePortalVirtualSitePath = ConfigurationManager.AppSettings["MaintaincePortalVirtualSiteOrders"];

                //    MaintaincePortalVirtualSitePath = MaintaincePortalVirtualSitePath + "//Section68";

                //    WebClient webClient = new WebClient();
                //    Stream strm = webClient.OpenRead(new Uri(MaintaincePortalVirtualSitePath + "//" + filepath));
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        strm.CopyTo(ms);
                //        dROrderFilePathResultModel.DataEntryCorrectionFileBytes = ms.ToArray();
                //    }
                return dROrderFilePathResultModel;
                //}
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        //***********************************************************************************************************************************************************************************


        public int Upload(ReScanningApplicationReqModel req)
        {
            int isSuccess = 0;

            ReScanningApplicationResModel res = new ReScanningApplicationResModel();
            try
            {
                int DocumentTypeID = (req.DType == "MARR") ? 2 : 1;
                dbContext = new KaveriEntities();
                if (DocumentTypeID == 2)
                {
                    MarriageRescanDetails drd = new MarriageRescanDetails();
                    drd.MarriageId = req.DocID;
                    drd.SROCode = req.SROCode;
                    drd.RescanEnableDateTime = DateTime.Now;////////////
                    drd.IsFileUploaded = false;
                    drd.DistrictCode = req.DROCode;
                    drd.OrderNumber = req.OrderNo;
                    drd.OrderDate = Convert.ToDateTime(req.Date);
                    drd.UserID = req.UserID;
                    drd.IPAddress = req.IPAddress;
                    drd.InsertDateTime = DateTime.Now;
                    drd.AbsoluteFilePath = req.FilePath;
                    drd.FileName = req.FileName;
                    drd.IsActive = true;
                    drd.IsPageCountModified = (req.NPC == -1) ? false : true;
                    drd.PageCount = (req.NPC == -1) ? (int?)null : req.NPC;
                    
                    
                    //Added by Madhur on 24-08-2022
                    drd.isMissingDocument = (req.isMissingDocument == null) ? false : (bool)req.isMissingDocument;
                    //End


                    dbContext.MarriageRescanDetails.Add(drd);

                }
                else
                {
                    DocumentRescanDetails drd = new DocumentRescanDetails();
                    drd.DocumentId = req.DocID;
                    drd.SROCode = req.SROCode;
                    drd.RescanEnableDateTime = DateTime.Now;///////
                    drd.IsFileUploaded = false;
                    drd.DistrictCode = req.DROCode;
                    drd.OrderNumber = req.OrderNo;
                    drd.OrderDate = Convert.ToDateTime(req.Date);
                    drd.UserID = req.UserID;
                    drd.IPAddress = req.IPAddress;
                    drd.InsertDateTime = DateTime.Now;
                    drd.AbsoluteFilePath = req.FilePath;
                    drd.FileName = req.FileName;
                    drd.IsActive = true;
                    drd.IsPageCountModified = (req.NPC == -1) ? false : true;
                    drd.PageCount = (req.NPC == -1) ? (int?)null : req.NPC;


                    //Added by Madhur on 24-08-2022
                    drd.isMissingDocument = (req.isMissingDocument == null) ? false : (bool)req.isMissingDocument;
                    //End


                    dbContext.DocumentRescanDetails.Add(drd);

                }
                dbContext.SaveChanges();

            }
            catch (Exception Ex)
            {
                isSuccess = 0;
            }
            isSuccess = 1;
            return isSuccess;
        }


        //***********************************************************************************************************************************************************************************


        public List<SelectListItem> GetFinancialYearList()
        {
            List<SelectListItem> financialYearList = new List<SelectListItem>();
            DateTime startYear = Convert.ToDateTime(new DateTime(2003, 1, 1)); //("1/1/2000");
            DateTime today = DateTime.Now.AddYears(1);
            string financialYear = null;
            SelectListItem item = new SelectListItem();
            item.Value = "0";
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


        //***********************************************************************************************************************************************************************************


        public List<SelectListItem> GetAllBookTypes()
        {
            dbContext = new KaveriEntities();
            List<SelectListItem> bookTypes = null;

            try
            {
                List<BookTypeMaster> bookTypesList = dbContext.BookTypeMaster.ToList();

                //bookTypesList.RemoveRange(1, 3);
                //bookTypesList.RemoveRange(7, 2);

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



        //***********************************************************************************************************************************************************************************


        public string GetFRN(int SROCode, string BType, string FYear, string DocNo)
        {
            try
            {
                dbContext = new KaveriEntities();
                string TriLetter = dbContext.SROMaster.Where(x => x.SROCode == SROCode).Select(y => y.ShortNameE).FirstOrDefault();
                int btype = Convert.ToInt32(BType);
                string FRN = TriLetter + "-" + dbContext.BookTypeMaster.Where(x => x.BookID == btype).Select(y => y.PrintableName).FirstOrDefault() + "-" + Convert.ToInt64(DocNo).ToString("D5") + "-" + FYear;


                return FRN;
            }
            catch (Exception ex)
            {
                return "";
            }
        }



        //public PhotoThumbFailedTableModel PhotoThumbFailed(int SROCode)
        //{
        //    try
        //    {
        //        int sno = 1;
        //        int sno1 = 1;
        //        dbContext = new KaveriEntities();

        //        List<PhotoThumbUploadLog> FailedLogList = new List<PhotoThumbUploadLog>();
        //        List<long> successList = dbContext.PhotoThumbCD_Success.Where(y=> y.SROCode == SROCode).Select(x=> (long)x.PartyID).ToList(); 
        //        FailedLogList = dbContext.PhotoThumbUploadLog.Where(x => x.PhotoCount != x.PhotosUploaded || x.ThumbCount != x.ThumbUploaded).ToList();

        //        List<PhotoThumbFailedTableModel> FailedList = new List<PhotoThumbFailedTableModel>();
        //        List<PhotoThumbFailedListModel> failedList;
        //        PhotoThumbFailedTableModel FinalList= new PhotoThumbFailedTableModel();
        //        FinalList.FailedList = new List<PhotoThumbFailedListModel>();
        //        FinalList.NonDistinctFailedList = new List<PhotoThumbFailedListModel>();
        //        foreach (PhotoThumbUploadLog entry in FailedLogList)
        //        {
        //            failedList = new List<PhotoThumbFailedListModel>();
        //            failedList= dbContext.PhotoThumbCD_Failed.Where(x=> x.LogID == entry.LogID && !successList.Contains((long)x.PartyID) && x.SROCode == SROCode).Select(z=> new PhotoThumbFailedListModel{SNo= sno ,FRN= z.PartyInfo.DocumentMaster.FinalRegistrationNumber ,ID=z.ID, LogID= z.LogID, PartyID=(long)z.PartyID, IsPhoto=(bool)z.IsPhoto, IsThumb = (bool)z.IsThumb , SROCode = (int)z.SROCode, ErrorMessage = z.Error, Date = entry.EndDateTime.ToString(), CDNumber= entry.CDNumber, PartyName = dbContext.PartyInfo.Where(d=> d.PartyID == z.PartyID && d.SROCode==z.SROCode).Select(c=>c.FirstName).FirstOrDefault()}).ToList<PhotoThumbFailedListModel>();
        //            if(failedList.Count>0)
        //            FinalList.NonDistinctFailedList.AddRange(failedList);
        //        }
        //        FinalList.FailedList = FinalList.NonDistinctFailedList.GroupBy(x => new { x.PartyID,x.IsPhoto,x.IsThumb }).Select(a => a.First()).ToList();
        //        FinalList.FailedList.ForEach(x => x.SNo = sno++);
        //        FinalList.NonDistinctFailedList.ForEach(x => x.SNo = sno1++);
        //        return FinalList;
        //    }
        //    catch (Exception ex)
        //    {
        //        PhotoThumbFailedTableModel resModel = new PhotoThumbFailedTableModel();
        //        resModel.IsError = true;

        //        //Changed by Madhur on 17-06-2022
        //        //resModel.ExError = ex.Message;
        //        resModel.ExError = ex.Message + ",Inner Exception: " + ex.InnerException;

        //        return resModel;
        //    }


        //}

        //public PhotoThumbFailedTableModel PhotoThumbFailedDetail(long PartyID, int SROCode, bool isPhoto, bool IsThumb)
        //{
        //    try
        //    {
        //        int sno = 1;
        //        dbContext = new KaveriEntities();

        //        List<PhotoThumbUploadLog> FailedLogList = new List<PhotoThumbUploadLog>();
        //        List<long> successList = dbContext.PhotoThumbCD_Success.Where(y => y.SROCode == SROCode).Select(x => (long)x.PartyID).ToList();
        //        FailedLogList = dbContext.PhotoThumbUploadLog.Where(x => x.PhotoCount != x.PhotosUploaded || x.ThumbCount != x.ThumbUploaded).ToList();

        //        List<PhotoThumbFailedTableModel> FailedList = new List<PhotoThumbFailedTableModel>();
        //        List<PhotoThumbFailedListModel> failedList;
        //        PhotoThumbFailedTableModel FinalList = new PhotoThumbFailedTableModel();
        //        FinalList.FailedList = new List<PhotoThumbFailedListModel>();
        //        foreach (PhotoThumbUploadLog entry in FailedLogList)
        //        {
        //            failedList = new List<PhotoThumbFailedListModel>();
        //            failedList = dbContext.PhotoThumbCD_Failed.Where(x => x.LogID == entry.LogID && !successList.Contains((long)x.PartyID) && x.SROCode == SROCode && x.IsPhoto == isPhoto && x.IsThumb == IsThumb).Select(z => new PhotoThumbFailedListModel { SNo = sno, CDNumber=entry.CDNumber ,Date = entry.EndDateTime.ToString(), ErrorMessage = z.Error }).ToList<PhotoThumbFailedListModel>();
        //            if (failedList.Count > 0)
        //                FinalList.FailedList.AddRange(failedList);
        //        }
        //        //FinalList.FailedList = FinalList.FailedList.GroupBy(x => new { x.PartyID, x.IsPhoto, x.IsThumb }).Select(a => a.First()).ToList();
        //        FinalList.FailedList.ForEach(x => x.SNo = sno++);
        //        return FinalList;
        //    }
        //    catch (Exception ex)
        //    {
        //        PhotoThumbFailedTableModel resModel = new PhotoThumbFailedTableModel();
        //        resModel.IsError = true;
        //        //Changed by Madhur on 17-06-2022
        //        //resModel.ExError = ex.Message;
        //        resModel.ExError = ex.Message + ",Inner Exception: " + ex.InnerException;
        //        return resModel;
        //    }


        //}
    }
}