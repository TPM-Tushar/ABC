using CustomModels.Models.XELFiles;
using ECDataAPI.Areas.XELFiles.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.ECDATADOCS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.XELFiles.DAL
{
    public class XELFilesDetailsDAL : IXELFilesDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        public XELFilesViewModel GetAuditSpecificationDetails(int OfficeID)
        {
            XELFilesViewModel ViewModel = new XELFilesViewModel();

            try
            {
                dbContext = new KaveriEntities();
                //string FirstRecord = "Select";

                ViewModel.SROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    ViewModel.SROfficeList = getSROfficesList();
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    ViewModel.SROfficeList = getSROfficesList();
                }
                else
                {
                    ViewModel.SROfficeList = getSROfficesList();
                }
                //ViewModel.Year = GetYearList();
                //ViewModel.Month = GetMonthList();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                ViewModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ViewModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

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

        public XELFilesViewModel GetJobsDetails(int OfficeID)
        {
            XELFilesViewModel ViewModel = new XELFilesViewModel();

            try
            {
                dbContext = new KaveriEntities();
                //string FirstRecord = "Select";

                ViewModel.SROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    ViewModel.SROfficeList = objCommon.GetSROfficesList();
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    ViewModel.SROfficeList = objCommon.GetSROfficesList();
                }
                else
                {
                    ViewModel.SROfficeList = objCommon.GetSROfficesList();
                }
                ViewModel.Year = GetYearList();
                ViewModel.Month = GetMonthList();

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

        public List<SelectListItem> GetModuleNameList()
        {
            List<SelectListItem> ModuleNameList = new List<SelectListItem>();
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Select", (Convert.ToInt32(0)).ToString()));
            ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Document Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.DocumentReg)).ToString()));
            //ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Marriage Registration", (Convert.ToInt32(ApiCommonEnum.ModuleNames.MarrriageReg)).ToString()));
            return ModuleNameList;
        }

        public List<SelectListItem> GetYearList()
        {
            List<SelectListItem> ModuleNameList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Select", (Convert.ToInt32(0)).ToString()));
                List<string> years = new List<string>();
                int startYear = 2019;
                int currentYear = DateTime.Today.Year;
                int cnt = currentYear - startYear;

                for (int i = 0; i <= cnt; i++)
                {
                    years.Add(startYear.ToString());
                    startYear = startYear + 1;
                }
                foreach (var item in years)
                {
                    ModuleNameList.Add(objCommon.GetDefaultSelectListItem(item, item));
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

            return ModuleNameList;
        }

        public List<SelectListItem> GetMonthList()
        {
            List<SelectListItem> ModuleNameList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                ModuleNameList.Add(objCommon.GetDefaultSelectListItem("Select", (Convert.ToInt32(0)).ToString()));
                for (int i = 0; i < months.Length - 1; i++)
                {
                    //ddl.Items.Add(new ListItem(months[i], i.ToString()));
                    ModuleNameList.Add(objCommon.GetDefaultSelectListItem(months[i], Convert.ToInt32(i + 1).ToString()));
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

            return ModuleNameList;

        }

        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        public int GetRegisteredJobsTotalCount(XELFilesViewModel model)
        {
            ECDATA_DOCS_Entities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new ECDATA_DOCS_Entities();
                RecordCnt = dbContext.XELReadJobDetails.ToList().Count();
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

        public RegisteredJobsListModel GetRegisteredJobsTableData(XELFilesViewModel model)
        {
            RegisteredJobsListModel RegisteredJobsListModel = new RegisteredJobsListModel();
            RegisteredJobsModel RegisteredJobsModel = null;
            //List<ReScanningDetailsModel> ReportsDetailsList = new List<ReScanningDetailsModel>();
            ECDATA_DOCS_Entities dbContext = null;
            try
            {
                dbContext = new ECDATA_DOCS_Entities();
                var ReceiptDetailsList = dbContext.XELReadJobDetails.OrderByDescending(c => c.JobID).ToList();


                RegisteredJobsListModel.RegisteredJobsModelLst = new List<RegisteredJobsModel>();
                foreach (var item in ReceiptDetailsList)
                {
                    RegisteredJobsModel = new RegisteredJobsModel();
                    RegisteredJobsModel.JobID = item.JobID;
                    if (item.isdro)
                    {
                        RegisteredJobsModel.SROCode = item.DROCode.Value;
                        RegisteredJobsModel.SROOfficeName = GetSROOfficeName(item.DROCode.Value, item.isdro);
                    }
                    else
                    {

                        RegisteredJobsModel.SROCode = item.SROCode.Value;
                        RegisteredJobsModel.SROOfficeName = GetSROOfficeName(item.SROCode.Value);

                    }
                    RegisteredJobsModel.OfficeType = item.isdro ? "DRO" : "SRO";
                    RegisteredJobsModel.FromYear = item.FromYear;
                    RegisteredJobsModel.ToYear = item.ToYear;
                    RegisteredJobsModel.FromMonth = GetMonth(item.FromMonth);
                    RegisteredJobsModel.ToMonth = GetMonth(item.ToMonth);
                    RegisteredJobsModel.RegisteredDateTime = string.IsNullOrEmpty(item.JobRegisterDateTime.ToString()) ? "-" : item.JobRegisterDateTime.ToString();
                    RegisteredJobsModel.CompletedDateTime = string.IsNullOrEmpty(item.JobCompletionDateTime.ToString()) ? "-" : item.JobCompletionDateTime.ToString();
                    RegisteredJobsModel.IsJobCompleted = item.IsJobCompleted;

                    RegisteredJobsModel.Description = string.IsNullOrEmpty(item.ErrorMessage) ? "-" : item.ErrorMessage;

                    RegisteredJobsListModel.RegisteredJobsModelLst.Add(RegisteredJobsModel);

                }
                return RegisteredJobsListModel;
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

        private string GetSROOfficeName(int SROCOde, bool isdro = false)
        {
            try
            {
                if (SROCOde == 0)
                {
                    return string.Empty;
                }
                KaveriEntities dbContext;
                dbContext = new KaveriEntities();
                if (isdro)
                    return dbContext.DistrictMaster.Where(x => x.DistrictCode == SROCOde).Select(x => x.DistrictNameE).FirstOrDefault();
                else return dbContext.SROMaster.Where(x => x.SROCode == SROCOde).Select(x => x.SRONameE).FirstOrDefault();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetMonth(int iMonthNo)
        {
            try
            {
                DateTime dtDate = new DateTime(2000, iMonthNo, 1);
                string sMonthName = dtDate.ToString("MMM");
                return dtDate.ToString("MMMM");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public XELFilesViewModel RegisterJobsDetails(XELFilesViewModel model)
        {
            XELFilesViewModel resModel = new XELFilesViewModel();
            ECDATA_DOCS_Entities dbContext = null;
            try
            {
                dbContext = new ECDATA_DOCS_Entities();

                // commented by akash on 8--11-19
                //if (dbContext.XELReadJobDetails.Where(x => x.SROCode == model.SROfficeID && x.FromYear == model.FromYearID && x.ToYear == model.ToYearID && x.FromMonth == model.FromMonthID
                //                                                         && x.ToMonth == model.ToMonthID && x.IsJobCompleted == false && x.IsErrorOccured == false).Any())
                //{
                //    resModel.IsInserted = false;
                //    resModel.ErrorMessage = "Same data fetch request already exists";
                //    return resModel;
                //}


                using (TransactionScope scope = new TransactionScope())
                {
                    XELReadJobDetails dbModel = new XELReadJobDetails();




                    if (model.OfficeType == "DRO")
                    {
                        dbModel.DROCode = model.SROfficeID;
                        dbModel.isdro = true;

                    }

                    if (model.OfficeType == "SRO")
                    {
                        dbModel.SROCode = model.SROfficeID;

                    }
                    dbModel.FromYear = model.FromYearID;
                    dbModel.ToYear = model.ToYearID;
                    dbModel.ToYear = model.ToYearID;
                    dbModel.ToMonth = model.ToMonthID;
                    dbModel.FromMonth = model.FromMonthID;
                    dbModel.JobRegisterDateTime = DateTime.Now;
                    dbContext.XELReadJobDetails.Add(dbModel);
                    dbContext.SaveChanges();
                    scope.Complete();
                }

                resModel.IsInserted = true;
                resModel.ErrorMessage = "";
                resModel.ResponseMessage = "Jobs Registered Successfully";
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext = null;
                }
            }
        }

        public int GetAuditSpecificationDetailsTotalCount(XELFilesViewModel model)
        {
            ECDATA_DOCS_Entities dbContext = null;
            int RecordCnt = 0;
            try
            {
                dbContext = new ECDATA_DOCS_Entities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                if (model.OfficeType == "DRO")
                {
                    if (model.SROfficeID == 999)
                        RecordCnt = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == true && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).ToList().Count();
                    else
                        RecordCnt = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == true && x.DROCode == model.SROfficeID && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).ToList().Count();
                }
                else
                {
                    if (model.SROfficeID == 999)
                        RecordCnt = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == false && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).ToList().Count();
                    else
                        RecordCnt = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == false && x.SROCode == model.SROfficeID && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).ToList().Count();

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

            return RecordCnt;
        }

        public XELFilesResModel GetAuditSpecificationDetailsTableData(XELFilesViewModel model)
        {
            XELFilesResModel resModel = new XELFilesResModel();
            XELFilesModel resDatabaleModel = null;
            //List<ReScanningDetailsModel> ReportsDetailsList = new List<ReScanningDetailsModel>();
            ECDATA_DOCS_Entities dbContext = null;
            try
            {
                dbContext = new ECDATA_DOCS_Entities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);

                List<XELAuditSpecificationDetail> dbAuditSpecificationDetailObj = new List<XELAuditSpecificationDetail>();

                if (model.OfficeType == "DRO")
                {
                    if (model.SROfficeID == 999)
                        dbAuditSpecificationDetailObj = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == true && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).OrderByDescending(x => x.IDetailID).Skip(model.startLen).Take(model.totalNum).ToList();
                    else
                        dbAuditSpecificationDetailObj = dbContext.XELAuditSpecificationDetail.Where(x => x.DROCode == model.SROfficeID && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).OrderByDescending(x => x.IDetailID).Skip(model.startLen).Take(model.totalNum).ToList();

                }
                else
                {
                    if (model.SROfficeID == 999)
                        dbAuditSpecificationDetailObj = dbContext.XELAuditSpecificationDetail.Where(x => x.isdro == false && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).OrderByDescending(x => x.IDetailID).Skip(model.startLen).Take(model.totalNum).ToList();
                    else
                        dbAuditSpecificationDetailObj = dbContext.XELAuditSpecificationDetail.Where(x => x.SROCode == model.SROfficeID && x.EVENT_TIME >= FromDate && x.EVENT_TIME <= ToDate).OrderByDescending(x => x.IDetailID).Skip(model.startLen).Take(model.totalNum).ToList();

                }

                int counter = model.startLen + 1;



                resModel.xelFilesModellST = new List<XELFilesModel>();
                foreach (var item in dbAuditSpecificationDetailObj)
                {
                    resDatabaleModel = new XELFilesModel();
                    resDatabaleModel.SrNo = counter++;
                    // resDatabaleModel.SROCode = item.SROCode;
                    if (item.isdro)
                    {
                        resDatabaleModel.SROCode = item.DROCode.Value;
                        resDatabaleModel.OfficeName = GetSROOfficeName(item.DROCode.Value, true);

                    }
                    else
                    {
                        resDatabaleModel.SROCode = item.SROCode.Value;
                        resDatabaleModel.OfficeName = GetSROOfficeName(item.SROCode.Value);

                    }
                    resDatabaleModel.OfficeType = item.isdro ? "DRO" : "SRO";
                    resDatabaleModel.EventTime = (item.EVENT_TIME == null) ? "-" : item.EVENT_TIME.ToString();
                    resDatabaleModel.LoginName = string.IsNullOrEmpty(item.LOGIN_NAME) ? "-" : item.LOGIN_NAME;
                    resDatabaleModel.ServerName = string.IsNullOrEmpty(item.SERVER_NAME) ? "-" : item.SERVER_NAME;
                    resDatabaleModel.DatabaseName = string.IsNullOrEmpty(item.DATABASE_NAME) ? "-" : item.DATABASE_NAME;
                    resDatabaleModel.ApplicationName = string.IsNullOrEmpty(item.APPLICATION_NAME) ? "-" : item.APPLICATION_NAME;
                    resDatabaleModel.Statement = string.IsNullOrEmpty(item.STATEMENT) ? "-" : (item.STATEMENT.ToLower().Equals("null") ? "-" : item.STATEMENT);
                    resDatabaleModel.HostName = string.IsNullOrEmpty(item.HOST_NAME) ? "-" : item.HOST_NAME;

                    resModel.xelFilesModellST.Add(resDatabaleModel);

                }
                return resModel;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        private List<SelectListItem> getSROfficesList()
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                OfficeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
                OfficeList.Insert(1, GetDefaultSelectListItem("All", "999"));
                OfficeList.AddRange(dbContext.SROMaster.OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
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

            return OfficeList;


        }



        public XELLogViewModel GetXELLogView()
        {

            XELLogViewModel ViewModel = new XELLogViewModel();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            ViewModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            ViewModel.ToDate = now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            ViewModel.SROfficeList = objCommon.getSROfficesList(true);
            ViewModel.TableNameList = new List<SelectListItem>() {

                new SelectListItem(){ Text="XEL Transmission Log",Value="1"},
                new SelectListItem(){ Text="XEL Schedulers Exception Log",Value="2"}
            };



            return ViewModel;
        }


        public XELLogViewModel LoadXELLogDetails(XELLogViewModel model)
        {

            XELLogViewModel responseModel = new XELLogViewModel();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            ECDATA_DOCS_Entities ecDataEntities = new ECDATA_DOCS_Entities();

            responseModel.XELLogDetailsModelList = new List<XELLogDetailsModel>();

            int SrNo = 0;
            bool isDRO = model.OfficeType == "DRO";
            if (model.TableID == 1)  //  [XELTransmissionLog]
            {

                var xelTransmissionLogList = ecDataEntities.USP_XEL_TRANSMISSION_LOG(model.SROfficeID, model.dtFromDate, model.dtToDate, isDRO).ToList();


                foreach (var item in xelTransmissionLogList)
                {
                    XELLogDetailsModel obj = new XELLogDetailsModel();


                    obj.SrNo = ++SrNo;

                    obj.sroName = item.SroName;
                    obj.OfficeType = item.isdro ? "DRO" : "SRO";
                    obj.AbsolutePath = item.AbsolutePath != null ? item.AbsolutePath : "-";
                    obj.fileName = item.FileName != null ? item.FileName : "-";
                    obj.IsSuccessfullUpload = item.isSuccessfullUpload ? "<i aria-hidden='true' style='color:GREEN;'>Yes</i>" : "<i  aria-hidden='true' style='color:red;'>No</i>";
                    obj.TransmissionInitateDateTime = item.TransmissionInitiateDateTime.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); ;


                    obj.TransmissionCompleteDateTime = item.TransmissionCompleteDateTime.HasValue ? item.TransmissionCompleteDateTime.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : "";
                    obj.Year = item.Year.ToString();
                    obj.Month = item.Month.ToString();
                    obj.FileSize = item.FileSize.HasValue ? ((item.FileSize.Value) / 1000000) + " MB" : "-";

                    obj.IsFileReadSuccessful = item.IsFileReadSuccessful ? "<i aria-hidden='true' style='color:GREEN;'>Yes</i>" : "<i  aria-hidden='true' style='color:red;'>No</i>";

                    obj.FileReadDateTime = item.FileReadDateTime.HasValue ? item.FileReadDateTime.Value.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) : "";
                    obj.EventStartDate = item.EventStartDate.HasValue ? item.EventStartDate.Value.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) : "";
                    obj.EventEndDate = item.EventEndDate.HasValue ? item.EventEndDate.Value.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) : "";



                    obj.sExceptionType = "";
                    obj.InnerExceptionMsg = "";
                    obj.ExceptionMsg = "";
                    obj.ExceptionStackTrace = "";
                    obj.ExceptionMethodName = "";
                    obj.LogDate = "";
                    obj.SchedulerName = "";

                    responseModel.XELLogDetailsModelList.Add(obj);
                }

            }

            else if (model.TableID == 2)  //[XELSchedulersExceptionLog]
            {
                var xelExceptionLogList = ecDataEntities.USP_XEL_EXCEPTION_LOG(model.SROfficeID, model.dtFromDate, model.dtToDate, isDRO).ToList();


                foreach (var item in xelExceptionLogList)
                {
                    XELLogDetailsModel obj = new XELLogDetailsModel();

                    obj.sroName = item.SroName;

                    obj.SrNo = ++SrNo;

                    obj.AbsolutePath = "";
                    obj.fileName = "";
                    obj.IsSuccessfullUpload = "";
                    obj.IsFileReadSuccessful = "";
                    obj.TransmissionInitateDateTime = "";
                    obj.TransmissionCompleteDateTime = "";
                    obj.Year = "";
                    obj.Month = "";
                    obj.FileSize = "";
                    obj.FileReadDateTime = "";
                    obj.EventStartDate = "";
                    obj.EventEndDate = "";
                    obj.OfficeType = item.isdro ? "DRO" : "SRO";
                    
                    obj.sExceptionType = item.ExceptionType != null ? item.ExceptionType : "-";
                    obj.InnerExceptionMsg = item.InnerExceptionMsg != null ? item.InnerExceptionMsg : "-";
                    obj.ExceptionMsg = item.ExceptionMsg != null ? item.ExceptionMsg : "-";
                    obj.ExceptionStackTrace = item.ExceptionStackTrace != null ? item.ExceptionStackTrace : "-";
                    obj.ExceptionMethodName = item.ExceptionMethodName != null ? item.ExceptionMethodName : "-";
                    obj.LogDate = item.LogDate.HasValue ? item.LogDate.Value.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) : "";
                    obj.SchedulerName = item.Scheduler_Name != null ? item.Scheduler_Name : "-";

                    responseModel.XELLogDetailsModelList.Add(obj);
                }


            }

            return responseModel;
        }

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public XELLogViewModel GetOfficeList(String OfficeType)
        {
            XELLogViewModel model = new XELLogViewModel();
            model.SROfficeList = new List<SelectListItem>();
            model.SROfficeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
           // model.SROfficeList.Insert(1, GetDefaultSelectListItem("All", "999"));
            dbContext = new KaveriEntities();

            if (OfficeType.ToLower().Equals("sro"))
            {
                //var SROMasterList = dbContext.SROMaster.ToList();
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                           // selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
                            selectListOBJ.Text = item.SRONameE  ;
                            selectListOBJ.Value = item.SROCode.ToString();
                            model.SROfficeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            if (OfficeType.ToLower().Equals("dro"))
            {
                //var DistrictMasterList = dbContext.DistrictMaster.ToList();
                List<DistrictMaster> DistrictMasterList = dbContext.DistrictMaster.ToList();
                DistrictMasterList = DistrictMasterList.OrderBy(x => x.DistrictNameE).ToList();
                if (DistrictMasterList != null)
                {
                    if (DistrictMasterList.Count() > 0)
                    {
                        foreach (var item in DistrictMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                           // selectListOBJ.Text = item.DistrictNameE + " (" + item.DistrictCode.ToString() + ")";
                            selectListOBJ.Text = item.DistrictNameE ;
                            selectListOBJ.Value = item.DistrictCode.ToString();
                            model.SROfficeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            return model;
        }

    }
}