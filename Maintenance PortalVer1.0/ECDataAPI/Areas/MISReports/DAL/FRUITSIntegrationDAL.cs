using CustomModels.Models.MISReports.FRUITSIntegration;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.PreRegApplicationDetailsService;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static ECDataAPI.Common.ApiCommonEnum;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class FRUITSIntegrationDAL : IFRUITSIntegration
    {
        public KaveriFruitsIntegrationResultModel DownloadForm3(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = new KaveriFruitsIntegrationResultModel();
            try
            {
                using (ApplicationDetailsService objService = new ApplicationDetailsService())
                {
                    FRUITSAppPendingRequestModel reqModel = new FRUITSAppPendingRequestModel();
                    reqModel.RequestForFormIII = "DownloadForm3";
                    reqModel.ReferenceNo = kaveriFruitsIntegrationViewModel.ReferenceNo;
                    reqModel.SroCode = kaveriFruitsIntegrationViewModel.SROCode;
                    FRUITSAppPendingResponseModel resModel = objService.GetForm3DetailsfromReferenceNo(reqModel);
                    if (resModel != null)
                    {
                        if (resModel.ResponseCode == "000")
                        {
                            kaveriFruitsIntegrationResultModel.Form3 = resModel.Form3Details;
                        }
                        else
                        {
                            kaveriFruitsIntegrationResultModel.ResponseMsg = "Error Occured while fetching XML";
                        }
                    }
                    else
                    {
                        kaveriFruitsIntegrationResultModel.ResponseMsg = "Error Occured while fetching XML";
                    }
                }
                return kaveriFruitsIntegrationResultModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationResultModel DownloadTransXML(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = new KaveriFruitsIntegrationResultModel();
            try
            {
                using (ApplicationDetailsService objService = new ApplicationDetailsService())
                {
                    FRUITSAppPendingRequestModel reqModel = new FRUITSAppPendingRequestModel();
                    reqModel.RequestForFormIII = "DownloadTransXML";
                    reqModel.ReferenceNo = kaveriFruitsIntegrationViewModel.ReferenceNo;
                    reqModel.SroCode = kaveriFruitsIntegrationViewModel.SROCode;
                    FRUITSAppPendingResponseModel resModel = objService.GetForm3DetailsfromReferenceNo(reqModel);
                    if (resModel != null)
                    {
                        if (resModel.ResponseCode == "000")
                        {
                            kaveriFruitsIntegrationResultModel.Form3 = resModel.Form3Details;
                        }
                        else
                        {
                            kaveriFruitsIntegrationResultModel.ResponseMsg = "Error Occured while fetching XML";
                        }
                    }
                    else
                    {
                        kaveriFruitsIntegrationResultModel.ResponseMsg = "Error Occured while fetching XML";
                    }
                }
                return kaveriFruitsIntegrationResultModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationResultModel GetFRUITSRecvDetails(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            KaveriEntities dbContext = null;
            KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = new KaveriFruitsIntegrationResultModel();
            kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList = new List<KaveriFruitsIntegrationDetailModel>();
            try
            {
                dbContext = new KaveriEntities();
                ApplicationDetailsService objService = new ApplicationDetailsService();
                FRUITSAppPendingRequestModel fRUITSAppPendingRequestModel = new FRUITSAppPendingRequestModel();
                List<string> ProcessedReferenceNoList = new List<string>();
                fRUITSAppPendingRequestModel.SroCode = kaveriFruitsIntegrationViewModel.SROCode;
                fRUITSAppPendingRequestModel.DistrictCode = kaveriFruitsIntegrationViewModel.DistrictID;
                fRUITSAppPendingRequestModel.MonthCode = kaveriFruitsIntegrationViewModel.MonthCode;
                if (kaveriFruitsIntegrationViewModel.MonthCode < 4)
                    kaveriFruitsIntegrationViewModel.FinancialyearCode = kaveriFruitsIntegrationViewModel.FinancialyearCode + 1;

                fRUITSAppPendingRequestModel.FinancialYearCode = kaveriFruitsIntegrationViewModel.FinancialyearCode;


                int SnoCount = 1;
                //if (kaveriFruitsIntegrationViewModel.SROCode != 0 && kaveriFruitsIntegrationViewModel.DistrictID != 0)
                //    ProcessedReferenceNoList = dbContext.FRUITS_DATA_RECV_DETAILS.Where(m => m.SROCode == kaveriFruitsIntegrationViewModel.SROCode).Select(m => m.ReferenceNo).ToList();
                //else if (kaveriFruitsIntegrationViewModel.SROCode == 0 && kaveriFruitsIntegrationViewModel.DistrictID != 0)
                //{
                //    List<int> SrocodeList = dbContext.SROMaster.Where(m => m.DistrictCode == kaveriFruitsIntegrationViewModel.DistrictID).Select(t => t.SROCode).ToList();
                //    ProcessedReferenceNoList = dbContext.FRUITS_DATA_RECV_DETAILS.Where(m => SrocodeList.Contains(m.SROCode)).Select(m => m.ReferenceNo).ToList();
                //}
                //else
                //{
                //    ProcessedReferenceNoList = dbContext.FRUITS_DATA_RECV_DETAILS.Select(m => m.ReferenceNo).ToList();
                //}
                //string SroName = dbContext.SROMaster.Where(m => m.SROCode == kaveriFruitsIntegrationViewModel.SROCode).Select(m => m.SRONameE).FirstOrDefault();
                ProcessedReferenceNoList = dbContext.USP_GET_FRUITS_PROCESSED_DOCUMENT(kaveriFruitsIntegrationViewModel.DistrictID, kaveriFruitsIntegrationViewModel.SROCode, kaveriFruitsIntegrationViewModel.FinancialyearCode, kaveriFruitsIntegrationViewModel.MonthCode).ToList();
                if (ProcessedReferenceNoList != null)
                {
                    fRUITSAppPendingRequestModel.CentralizedRefernceNoList = ProcessedReferenceNoList.ToArray();
                }
                else
                {
                    fRUITSAppPendingRequestModel.CentralizedRefernceNoList = new string[] { };
                }
                objService.Timeout = 180000;
                FRUITSAppPendingResponseModel ResModel = objService.GetAllPendingFRUITSApp(fRUITSAppPendingRequestModel);
                if (ResModel != null)
                {
                    if (ResModel.ResponseCode == "000")
                    {
                        foreach (var item in ResModel.PendingFRUITSAppList)
                        {
                            KaveriFruitsIntegrationDetailModel kaveriFruitsIntegrationDetailModel = new KaveriFruitsIntegrationDetailModel();
                            kaveriFruitsIntegrationDetailModel.Sno = SnoCount++;
                            kaveriFruitsIntegrationDetailModel.ReferenceNo = item.ReferenceNo;
                            if (string.IsNullOrEmpty(item.FormIIIData))
                                kaveriFruitsIntegrationDetailModel.Form3 = "--";
                            else
                                kaveriFruitsIntegrationDetailModel.Form3 = "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=DownloadForm3('" + item.ReferenceNo + "','" + item.SROCode + "');><i>Form 3</i></a>";
                            kaveriFruitsIntegrationDetailModel.OfficeName = dbContext.SROMaster.Where(m => m.SROCode == item.SROCode).Select(m => m.SRONameE).FirstOrDefault();
                            kaveriFruitsIntegrationDetailModel.TranXML = "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=DownloadTransXML('" + item.ReferenceNo + "','" + item.SROCode + "');><i>Trans XML</i></a>"; ;
                            kaveriFruitsIntegrationDetailModel.AcknowledgementNo = item.AknowledgementNo;
                            kaveriFruitsIntegrationDetailModel.DataReceivedDate = item.DataReceivedDate.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                            kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList.Add(kaveriFruitsIntegrationDetailModel);
                        }
                        kaveriFruitsIntegrationResultModel.ResponseCode = "000";
                        kaveriFruitsIntegrationResultModel.ResponseMsg = "";
                    }
                    else
                    {
                        kaveriFruitsIntegrationResultModel.ResponseCode = ResModel.ResponseCode;
                        kaveriFruitsIntegrationResultModel.ResponseMsg = ResModel.ResponseMsg;
                    }
                }
                return kaveriFruitsIntegrationResultModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationViewModel KAVERIFRUITSIntegration(int OfficeID)
        {
            KaveriFruitsIntegrationViewModel model = new KaveriFruitsIntegrationViewModel();
            KaveriEntities dbContext = null;

            model.DistrictList = new List<SelectListItem>();
            model.SROfficeList = new List<SelectListItem>();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();


                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });

                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                }
                else
                {
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("All");
                }
                model.MonthList = getMonthList();
                model.FinancialYearList = getFinancialYearList();
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return model;
        }

        public List<SelectListItem> getMonthList()
        {
            try
            {
                List<SelectListItem> Monthlist = new List<SelectListItem>();
                Monthlist.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                Monthlist.Insert(1, new SelectListItem { Value = "4", Text = "April" });
                Monthlist.Insert(2, new SelectListItem { Value = "5", Text = "May" });
                Monthlist.Insert(3, new SelectListItem { Value = "6", Text = "June" });
                Monthlist.Insert(4, new SelectListItem { Value = "7", Text = "July" });
                Monthlist.Insert(5, new SelectListItem { Value = "8", Text = "August" });
                Monthlist.Insert(6, new SelectListItem { Value = "9", Text = "September" });
                Monthlist.Insert(7, new SelectListItem { Value = "10", Text = "October" });
                Monthlist.Insert(8, new SelectListItem { Value = "11", Text = "November" });
                Monthlist.Insert(9, new SelectListItem { Value = "12", Text = "December" });
                Monthlist.Insert(10, new SelectListItem { Value = "1", Text = "January" });
                Monthlist.Insert(11, new SelectListItem { Value = "2", Text = "February" });
                Monthlist.Insert(12, new SelectListItem { Value = "3", Text = "March" });

                return Monthlist;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SelectListItem> getFinancialYearList()
        {
            try
            {
                List<SelectListItem> Financialyearlist = new List<SelectListItem>();
                Financialyearlist.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                KaveriEntities dbcontext = null;
                using (dbcontext = new KaveriEntities())
                {
                    var financialyearlist = dbcontext.USP_FINANCIAL_YEAR().ToList();
                    if (financialyearlist != null)
                    {
                        //if (financialyearlist.Count() > 4)
                        //{
                        //    financialyearlist.RemoveRange(2, 4);
                        //}
                        foreach (var item in financialyearlist)
                        {
                            //Financialyearlist.Add(new SelectListItem { Value = item.YEAR.ToString(), Text = item.FYEAR });
                            Financialyearlist.Add(new SelectListItem { Value = item.YEAR.ToString(), Text = item.FYEAR ?? string.Empty });
                        }
                    }
                    return Financialyearlist;
                }
            }
            catch (Exception)
            {
              throw;
            }
        }
    }
}