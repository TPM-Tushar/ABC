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
using ECDataAPI.Areas.PhotoThumb.Interface;
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

namespace ECDataAPI.Areas.PhotoThumb.DAL
{
    public class PhotoThumbDAL : IPhotoThumb
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public PhotoThumbViewModel PhotoThumbView(int officeID)
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            PhotoThumbViewModel ViewModel = new PhotoThumbViewModel();

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

        public PhotoThumbTableModel PhotoThumbAvailaibility(int SROCode, long DocumentNumber, int BookTypeID, string fyear)
        {
            PhotoThumbTableModel resModel;

            try
            {
                int sno = 0;
                List<PartyDetailModel> PDM = new List<PartyDetailModel>();
                dbContext = new KaveriEntities();
                string bookID = dbContext.BookTypeMaster.Where(x => x.BookID == BookTypeID).Select(y => y.PrintableName).FirstOrDefault().ToString();
                string TriLetter = dbContext.SROMaster.Where(x => x.SROCode == SROCode).Select(y => y.ShortNameE).FirstOrDefault().ToString();
                string FRN = TriLetter + "-" + bookID + "-" + DocumentNumber.ToString("D5") + "-" + fyear;
                Dictionary<long, int> DocSroList = new Dictionary<long, int>();
                DocSroList = dbContext.DocumentMaster.Where(x => x.FinalRegistrationNumber == FRN).Select(y => new { y.DocumentID, y.SROCode }).ToDictionary(p => p.DocumentID, p => p.SROCode);

                if (DocSroList != null)
                {
                    foreach (var a in DocSroList)
                    {
                        List<PartyDetailModel> PartyInfoList = dbContext.PartyInfo.Where(x => x.DocumentID == (long)a.Key && x.SROCode == (int)a.Value && x.DocumentMaster.Stamp5DateTime != null).Select(y => new PartyDetailModel { SROCode = y.SROCode, DocumentID = y.DocumentID, PartyID = y.PartyID, Fname = y.FirstName, Lname = y.LastName }).ToList<PartyDetailModel>();
                        PDM.AddRange(PartyInfoList);
                    }

                    foreach (PartyDetailModel pdm in PDM)
                    {
                        sno++;
                        UploadDet photoPath = dbContext.PhotoThumbUploadDetails.Where(x => x.DocumentID == pdm.DocumentID && x.PartyID == pdm.PartyID && x.SROCode == pdm.SROCode && x.IsPhoto == true).Select(y => new UploadDet { UploadPath = y.UploadPath, Date = y.UploadDateTime }).FirstOrDefault();
                        UploadDet thumbPath = dbContext.PhotoThumbUploadDetails.Where(x => x.DocumentID == pdm.DocumentID && x.PartyID == pdm.PartyID && x.SROCode == pdm.SROCode && x.IsThumb == true).Select(y => new UploadDet { UploadPath = y.UploadPath, Date = y.UploadDateTime }).FirstOrDefault();


                        if (photoPath!=null && photoPath.UploadPath != "")
                        {
                            pdm.PhotoPath = photoPath.UploadPath;
                        }
                        else
                        {
                            pdm.PhotoPath = string.Empty;
                        }
                        if (thumbPath!=null && thumbPath.UploadPath != "")
                        {
                            pdm.ThumbPath = thumbPath.UploadPath;
                        }
                        else
                        {
                            pdm.ThumbPath = string.Empty;
                        }
                        if (photoPath != null && photoPath.Date.ToString() != "")
                        {
                            pdm.UploadDatePhoto = photoPath.Date.ToString();
                        }
                        else
                        {
                            pdm.UploadDatePhoto = string.Empty;
                        }
                        if (thumbPath != null && thumbPath.Date.ToString() != "")
                        {
                            pdm.UploadDateThumb = thumbPath.Date.ToString();
                        }
                        else
                        {
                            pdm.UploadDateThumb = string.Empty;
                        }
                        pdm.SrNo = sno;
                    }
                    resModel = new PhotoThumbTableModel();
                    resModel.SROName = dbContext.SROMaster.Where(x => x.SROCode == SROCode).Select(y => y.SRONameE).First();
                    resModel.PDM = PDM;
                    resModel.IsError = false;
                    return resModel;


                }
                else
                {
                    resModel = new PhotoThumbTableModel();
                    resModel.IsError = true;
                    resModel.ErrorMessage = "No data found for entered Final Registration Number.";
                    return resModel;
                }
            }
            catch (Exception ex)
            {
                resModel = new PhotoThumbTableModel();
                resModel.IsError = true;
                resModel.ErrorMessage = ex.Message;
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