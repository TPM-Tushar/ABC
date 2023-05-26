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

using CustomModels.Models.PhotoThumb;
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
using ECDataAPI.Areas.PhotoThumb.Interface;

namespace ECDataAPI.Areas.PhotoThumb.DAL
{
    public class PhotoThumbFailedDAL : IPhotoThumbFailed
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public PhotoThumbFailedViewModel PhotoThumbFailedView(int officeID)
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            PhotoThumbFailedViewModel ViewModel = new PhotoThumbFailedViewModel();

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

        public PhotoThumbFailedTableModel PhotoThumbFailed(int SROCode)
        {
            try
            {
                int sno = 1;
                int sno1 = 1;
                dbContext = new KaveriEntities();

                List<PhotoThumbUploadLog> FailedLogList = new List<PhotoThumbUploadLog>();
                List<long> successList = dbContext.PhotoThumbCD_Success.Where(y=> y.SROCode == SROCode).Select(x=> (long)x.PartyID).ToList(); 
                FailedLogList = dbContext.PhotoThumbUploadLog.Where(x => x.PhotoCount != x.PhotosUploaded || x.ThumbCount != x.ThumbUploaded).ToList();

                List<PhotoThumbFailedTableModel> FailedList = new List<PhotoThumbFailedTableModel>();
                List<PhotoThumbFailedListModel> failedList;
                PhotoThumbFailedTableModel FinalList= new PhotoThumbFailedTableModel();
                FinalList.FailedList = new List<PhotoThumbFailedListModel>();
                FinalList.NonDistinctFailedList = new List<PhotoThumbFailedListModel>();
                foreach (PhotoThumbUploadLog entry in FailedLogList)
                {
                    failedList = new List<PhotoThumbFailedListModel>();
                    failedList= dbContext.PhotoThumbCD_Failed.Where(x=> x.LogID == entry.LogID && !successList.Contains((long)x.PartyID) && x.SROCode == SROCode).Select(z=> new PhotoThumbFailedListModel{SNo= sno ,FRN= z.PartyInfo.DocumentMaster.FinalRegistrationNumber ,ID=z.ID, LogID= z.LogID, PartyID=(long)z.PartyID, IsPhoto=(bool)z.IsPhoto, IsThumb = (bool)z.IsThumb , SROCode = (int)z.SROCode, ErrorMessage = z.Error, Date = entry.EndDateTime.ToString(), CDNumber= entry.CDNumber, PartyName = dbContext.PartyInfo.Where(d=> d.PartyID == z.PartyID && d.SROCode==z.SROCode).Select(c=>c.FirstName).FirstOrDefault()}).ToList<PhotoThumbFailedListModel>();
                    if(failedList.Count>0)
                    FinalList.NonDistinctFailedList.AddRange(failedList);
                }
                FinalList.FailedList = FinalList.NonDistinctFailedList.GroupBy(x => new { x.PartyID,x.IsPhoto,x.IsThumb }).Select(a => a.First()).ToList();
                FinalList.FailedList.ForEach(x => x.SNo = sno++);
                FinalList.NonDistinctFailedList.ForEach(x => x.SNo = sno1++);
                return FinalList;
            }
            catch (Exception ex)
            {
                PhotoThumbFailedTableModel resModel = new PhotoThumbFailedTableModel();
                resModel.IsError = true;
                resModel.ExError = ex.Message;
                return resModel;
            }

            
        }

        public PhotoThumbFailedTableModel PhotoThumbFailedDetail(long PartyID, int SROCode, bool isPhoto, bool IsThumb)
        {
            try
            {
                int sno = 1;
                dbContext = new KaveriEntities();

                List<PhotoThumbUploadLog> FailedLogList = new List<PhotoThumbUploadLog>();
                List<long> successList = dbContext.PhotoThumbCD_Success.Where(y => y.SROCode == SROCode).Select(x => (long)x.PartyID).ToList();
                FailedLogList = dbContext.PhotoThumbUploadLog.Where(x => x.PhotoCount != x.PhotosUploaded || x.ThumbCount != x.ThumbUploaded).ToList();

                List<PhotoThumbFailedTableModel> FailedList = new List<PhotoThumbFailedTableModel>();
                List<PhotoThumbFailedListModel> failedList;
                PhotoThumbFailedTableModel FinalList = new PhotoThumbFailedTableModel();
                FinalList.FailedList = new List<PhotoThumbFailedListModel>();
                foreach (PhotoThumbUploadLog entry in FailedLogList)
                {
                    failedList = new List<PhotoThumbFailedListModel>();
                    failedList = dbContext.PhotoThumbCD_Failed.Where(x => x.LogID == entry.LogID && !successList.Contains((long)x.PartyID) && x.SROCode == SROCode && x.IsPhoto == isPhoto && x.IsThumb == IsThumb).Select(z => new PhotoThumbFailedListModel { SNo = sno, CDNumber=entry.CDNumber ,Date = entry.EndDateTime.ToString(), ErrorMessage = z.Error }).ToList<PhotoThumbFailedListModel>();
                    if (failedList.Count > 0)
                        FinalList.FailedList.AddRange(failedList);
                }
                //FinalList.FailedList = FinalList.FailedList.GroupBy(x => new { x.PartyID, x.IsPhoto, x.IsThumb }).Select(a => a.First()).ToList();
                FinalList.FailedList.ForEach(x => x.SNo = sno++);
                return FinalList;
            }
            catch (Exception ex)
            {
                PhotoThumbFailedTableModel resModel = new PhotoThumbFailedTableModel();
                resModel.IsError = true;
                resModel.ExError = ex.Message;
                return resModel;
            }


        }

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

                bookTypesList.RemoveRange(1, 3);
                bookTypesList.RemoveRange(7, 2);

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

    }
}