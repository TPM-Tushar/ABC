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

using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SupportEnclosure.DAL
{
    public class SupportEnclosureDetailsDAL : ISupportEnclosureDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

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

        public SupportEnclosureDetailsViewModel SupportEnclosureDetails(int OfficeID)
        {
            SupportEnclosureDetailsViewModel ViewModel = new SupportEnclosureDetailsViewModel();

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

        public int GetSupportDocumentEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            KaveriEntities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new KaveriEntities();
                RecordCnt = dbContext.USP_RPT_GetSupportEnclosureDetails(model.SROfficeID, model.DocumentNumber, model.BookTypeID, model.FinancialYearStr, Convert.ToInt32(EnclosureType.DocumentEnclosure)).ToList().Count();
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

            return RecordCnt;
        }

        public SupportEnclosureDetailsResModel GetSupportDocumentEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            SupportEnclosureDetailsResModel resonseModel = new SupportEnclosureDetailsResModel();
            SupportEnclosureDetailsModel SupportEnclosureDetails = null;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var SupportEnclosureList = dbContext.USP_RPT_GetSupportEnclosureDetails(model.SROfficeID, model.DocumentNumber, model.BookTypeID, model.FinancialYearStr, Convert.ToInt32(EnclosureType.DocumentEnclosure)).Skip(model.startLen).Take(model.totalNum).ToList();
                int counter = 1;

                resonseModel.SupportEnclosureDetailsList = new List<SupportEnclosureDetailsModel>();
                foreach (var item in SupportEnclosureList)
                {
                    SupportEnclosureDetails = new SupportEnclosureDetailsModel();
                    SupportEnclosureDetails.SerialNo = counter++;
                    SupportEnclosureDetails.DocumentID = Convert.ToInt64(item.DocumentID);
                    SupportEnclosureDetails.DocumentNo = item.DocumentNumber == null ? " " : Convert.ToString(item.DocumentNumber);
                    SupportEnclosureDetails.FinalRegistrationNumber = string.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                    SupportEnclosureDetails.SupportDocumentTypeID = Convert.ToInt32(item.SupportDocumentTypeID);
                    SupportEnclosureDetails.SupportDocumentType = item.DocumentNumber == null ? "NA" : Convert.ToString(item.SupportDocumentType);
                    SupportEnclosureDetails.SROOffice = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                    SupportEnclosureDetails.SROCode = model.SROfficeID;

                    SupportEnclosureDetails.UploadDateTime = string.IsNullOrEmpty(item.UploadDateTime.ToString()) ? "NA" : item.UploadDateTime.ToString();
                    SupportEnclosureDetails.FilePath = item.FilePath;
                    //SupportEnclosureDetails.PartyID = Convert.ToInt64(item.PartyID);
                    //SupportEnclosureDetails.PartyName = item.PartyName;
                    int idx = item.FilePath.LastIndexOf('\\');
                    if (idx != -1)
                    {
                        SupportEnclosureDetails.FileName = item.FilePath.Substring(idx + 1);
                    }
                    else
                    {
                        SupportEnclosureDetails.FileName = "NA";
                    }
                    resonseModel.SupportEnclosureDetailsList.Add(SupportEnclosureDetails);

                }
                return resonseModel;
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

        public int GetSupportPartyEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            KaveriEntities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new KaveriEntities();
                RecordCnt = dbContext.USP_RPT_GetSupportEnclosureDetails(model.SROfficeID, model.DocumentNumber, model.BookTypeID, model.FinancialYearStr, Convert.ToInt32(EnclosureType.PartyEnclosure)).ToList().Count();
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

            return RecordCnt;
        }

        public SupportEnclosureDetailsResModel GetSupportPartyEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            SupportEnclosureDetailsResModel resonseModel = new SupportEnclosureDetailsResModel();
            SupportEnclosureDetailsModel SupportEnclosureDetails = null;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var SupportEnclosureList = dbContext.USP_RPT_GetSupportEnclosureDetails(model.SROfficeID, model.DocumentNumber, model.BookTypeID, model.FinancialYearStr, Convert.ToInt32(EnclosureType.PartyEnclosure)).Skip(model.startLen).Take(model.totalNum).ToList();
                int counter = 1;

                resonseModel.SupportEnclosureDetailsList = new List<SupportEnclosureDetailsModel>();
                foreach (var item in SupportEnclosureList)
                {
                    SupportEnclosureDetails = new SupportEnclosureDetailsModel();
                    SupportEnclosureDetails.SerialNo = counter++;
                    SupportEnclosureDetails.DocumentID = Convert.ToInt64(item.DocumentID);
                    SupportEnclosureDetails.DocumentNo = item.DocumentNumber == null ? " " : Convert.ToString(item.DocumentNumber);
                    SupportEnclosureDetails.FinalRegistrationNumber = string.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                    SupportEnclosureDetails.SupportDocumentTypeID = Convert.ToInt32(item.SupportDocumentTypeID);
                    SupportEnclosureDetails.SupportDocumentType = item.DocumentNumber == null ? "NA" : Convert.ToString(item.SupportDocumentType);
                    SupportEnclosureDetails.SROOffice = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                    SupportEnclosureDetails.SROCode = model.SROfficeID;
                    SupportEnclosureDetails.UploadDateTime = string.IsNullOrEmpty(item.UploadDateTime.ToString()) ? "NA" : item.UploadDateTime.ToString();
                    SupportEnclosureDetails.FilePath = item.FilePath;
                    SupportEnclosureDetails.PartyID = Convert.ToInt64(item.PartyID);
                    SupportEnclosureDetails.PartyName = item.PartyName;
                    int idx = item.FilePath.LastIndexOf('\\');
                    if (idx != -1)
                    {
                        SupportEnclosureDetails.FileName = item.FilePath.Substring(idx + 1);
                    }
                    else
                    {
                        SupportEnclosureDetails.FileName = "NA";
                    }

                    resonseModel.SupportEnclosureDetailsList.Add(SupportEnclosureDetails);

                }
                return resonseModel;
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

        public SupportEnclosureDetailsResModel GetSupportDocumentEnclosureBytes(SupportEnclosureDetailsModel model)
        {
            throw new NotImplementedException();
        }

        public enum EnclosureType
        {
            DocumentEnclosure = 1,
            PartyEnclosure = 2
        }
    }
}