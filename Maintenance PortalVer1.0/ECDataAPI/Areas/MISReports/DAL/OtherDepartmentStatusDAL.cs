using CustomModels.Models.MISReports.OtherDepartmentStatus;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
//using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class OtherDepartmentStatusDAL: IOtherDepartmentStatus
    {
        KaveriEntities dbContext = null;


        public OtherDepartmentStatusModel OtherDepartmentStatusView(int OfficeID)
        {
            OtherDepartmentStatusModel resModel = new OtherDepartmentStatusModel();
            resModel.SROfficeList = new List<SelectListItem>();
            resModel.SROfficeList.Add(GetDefaultSelectListItem("Select", "0"));
            List<SROMaster> SROMasterList = new List<SROMaster>();
            try
            {
                dbContext = new KaveriEntities();              
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();                

                // For SR
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    string kaveriCode = Kaveri1Code.ToString();
                    SelectListItem select = new SelectListItem();
                    select.Text = SroName;
                    select.Value = kaveriCode;
                    resModel.SROfficeList.Add(select);          
                }
                // For DR
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    SROMasterList = dbContext.SROMaster.Where(x=>x.DistrictCode==Kaveri1Code).ToList();                    
                    if (SROMasterList != null)
                    {
                        SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.SRONameE;
                            select.Value = item.SROCode.ToString();
                            resModel.SROfficeList.Add(select);
                        }
                    }
                }

                // For others
                else
                {
                    SROMasterList = dbContext.SROMaster.ToList();
                    if (SROMasterList != null)
                    {
                        SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.SRONameE;
                            select.Value = item.SROCode.ToString();
                            resModel.SROfficeList.Add(select);
                        }
                    }
                }

                resModel.IntegrationtypeList = new List<SelectListItem>();
                resModel.IntegrationtypeList.Add(GetDefaultSelectListItem("Select", "0"));
                List<IntegrationDepartment> IntegrationDepartmentList= dbContext.IntegrationDepartment.ToList();
                if (IntegrationDepartmentList!=null)
                {
                    foreach (var item in IntegrationDepartmentList)
                    {
                        SelectListItem select = new SelectListItem();
                        select.Text = item.DepartmentName;
                        select.Value = item.UDeptID.ToString();
                        resModel.IntegrationtypeList.Add(select);
                    }
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
            return resModel;

        }
        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        public List<OtherDepartmentStatusDetailsModel> OtherDepartmentStatusDetails(OtherDepartmentStatusModel model)
        {

            OtherDepartmentStatusDetailsModel ReportsDetails = null;
            List<OtherDepartmentStatusDetailsModel> ReportsDetailsList = new List<OtherDepartmentStatusDetailsModel>();
            List<OtherDepartmentStatusDetailsModel> TransactionList = new List<OtherDepartmentStatusDetailsModel>();
            KaveriEntities dbContext = null;
            // long Amount = Convert.ToInt64(model.Amount);
            try
            {
                dbContext = new KaveriEntities();
                //int financialYearID = model.FinacialYearID == null ? 0 : Convert.ToInt32(model.FinacialYearID);
                //TransactionList = dbContext.USP_RPT_SALEDEED_REGISTERED(model.DROfficeID, model.SROfficeID, financialYearID, null).Skip(model.startLen).Take(model.totalNum).ToList();
                int counter = 1;
                //foreach (var item in TransactionList)
                //{
                ReportsDetails = new OtherDepartmentStatusDetailsModel();
                ReportsDetails.SerialNo = counter++;
                ReportsDetails.Column1 = "Column 1 data";
                ReportsDetails.Column2 = "Column 1 data";
                ReportsDetails.Column3 = "Column 1 data";
                ReportsDetails.Column4 = "Column 1 data";
                ReportsDetails.Column5 = "Column 1 data";
                ReportsDetails.Column6 = "Column 1 data";
                ReportsDetails.Column7 = "Column 1 data";
                //ReportsDetails.DistrictName = string.IsNullOrEmpty(item.DISTRICTNAME) ? "null" : item.DISTRICTNAME;
                //ReportsDetails.SROName = string.IsNullOrEmpty(item.SRONAMEE) ? "null" : item.SRONAMEE;
                //ReportsDetails.DocumentsRegistered = item.NO_OF_DOCUMENTS;                  
                //ReportsDetails.StampDuty = item.STAMPDUTY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //ReportsDetails.RegistrationFee = item.REGISTRATIONFEE.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //decimal total = item.STAMPDUTY + item.REGISTRATIONFEE;
                //ReportsDetails.Total = total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                ReportsDetailsList.Add(ReportsDetails);
                //}
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

            return ReportsDetailsList;


        }

        public int OtherDepartmentStatusDetailsTotalCount(OtherDepartmentStatusModel model)
        {
            int Count = 0;
            try
            {
                dbContext = new KaveriEntities();
                //var TransactionList = dbContext.USP_RPT_SALEDEED_REGISTERED(model.DROfficeID, model.SROfficeID, financialYearID, null).ToList();
                //Count = TransactionList.Count;
                Count = 1;
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
            return Count;
        }
    }
}