using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using CustomModels.Models.Remittance.ARegisterAnalysisReport;
using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using CustomModels.Models.Remittance.MasterData;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KAIGR_ONLINE;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ChallanDataEntryCorrectionDetailsDAL
    {
        public ChallanDataEntryCorrectionDetailsReportModel ChallanDataEntryCorrectionDetailsView()
        {
            KaveriEntities dbContext = new KaveriEntities();

            List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
            SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
            ChallanDataEntryCorrectionDetailsReportModel resModel = new ChallanDataEntryCorrectionDetailsReportModel();

            try
            {

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.SROfficeList = new List<SelectListItem>();
                SelectListItem selectListFirst = new SelectListItem();

                SelectListItem selectListSecond = new SelectListItem();
                selectListFirst.Text = "All";
                selectListFirst.Value = "0";
                //selectListSecond.Text = "All";
                //selectListSecond.Value = "0";
                resModel.SROfficeList.Insert(0, selectListFirst);










                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            //selectListOBJ.Text = "Select";
                            //selectListOBJ.Value = "0";
                            selectListOBJ.Text = item.SRONameE;
                            selectListOBJ.Value = item.SROCode.ToString();
                            //resModel.SROfficeList.Add("Select");
                            resModel.SROfficeList.Add(selectListOBJ);
                        }
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
                {
                    dbContext.Dispose();
                }
            }
            return resModel;





        }
        public ChallanDataEntryCorrectionDetailsResultModel GetCDECorrectionDetailsData(ChallanDataEntryCorrectionDetailsReportModel masterDataReportModel)

        {
            ChallanDataEntryCorrectionDetailsReportTableModel resultModel = new ChallanDataEntryCorrectionDetailsReportTableModel();


            ChallanDataEntryCorrectionDetailsReportModel challanDataEntryCorrectionDetailsReportModel = new ChallanDataEntryCorrectionDetailsReportModel();

            ChallanDataEntryCorrectionDetailsResultModel ResultModel = new ChallanDataEntryCorrectionDetailsResultModel();
            ResultModel.CDECDDataTableList = new List<ChallanDataEntryCorrectionDetailsReportTableModel>();
            bool IsLcoallyUpdated = Convert.ToBoolean(masterDataReportModel.LocallyUpdated);
            bool IsCentrallyUpdated = Convert.ToBoolean(masterDataReportModel.CentrallyUpdated);


            try

            {
                KaveriEntities dbContext = new KaveriEntities();

                dbContext = new KaveriEntities();


                var Result = dbContext.ChallanNumberDataEntryCorrection.ToList().Take(1);


                if (IsCentrallyUpdated && IsLcoallyUpdated)
                {
                    if (masterDataReportModel.SROId == 0)
                    {
                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.IsLocallyUpdated == IsLcoallyUpdated && x.IsCentrallyupdated == IsCentrallyUpdated) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();

                    }
                    else
                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.SROCode == masterDataReportModel.SROId) && (x.IsLocallyUpdated == IsLcoallyUpdated && x.IsCentrallyupdated == IsCentrallyUpdated) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();

                }
                /* Added by Rushikesh 1 March 2023 */
                if(!IsCentrallyUpdated && !IsLcoallyUpdated)
                {
                    if (masterDataReportModel.SROId == 0)
                    {
                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => 
                        (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) 
                        && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();
                    }
                    else
                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.SROCode == masterDataReportModel.SROId)
                        && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate))
                        && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();
                }
                // End by Rushikesh 1 March 2023
                else
                {

                    if (IsLcoallyUpdated)
                    {
                        if(masterDataReportModel.SROId==0)
                        {
                            Result = dbContext.ChallanNumberDataEntryCorrection.Where((x =>(x.IsLocallyUpdated == IsLcoallyUpdated) && (x.IsCentrallyupdated == false || x.IsCentrallyupdated == null) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();

                        }
                        else
                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.SROCode == masterDataReportModel.SROId) && (x.IsLocallyUpdated == IsLcoallyUpdated) && (x.IsCentrallyupdated == false || x.IsCentrallyupdated == null) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();
                    }
                    else
                    {
                        if (masterDataReportModel.SROId == 0)
                        {
                            Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.IsCentrallyupdated == IsCentrallyUpdated) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();

                        }
                        else

                        Result = dbContext.ChallanNumberDataEntryCorrection.Where((x => (x.SROCode == masterDataReportModel.SROId) && (x.IsCentrallyupdated == IsCentrallyUpdated) && (DbFunctions.TruncateTime(x.ApplicationDate) >= DbFunctions.TruncateTime(masterDataReportModel.DateTime_FromDate)) && (DbFunctions.TruncateTime(x.ApplicationDate) <= DbFunctions.TruncateTime(masterDataReportModel.DateTime_ToDate)))).ToList();

                    }
                }

                foreach (var item in Result)
                {
                     resultModel = new ChallanDataEntryCorrectionDetailsReportTableModel();
                    resultModel.SROCode = item.SROCode;
                    var SROName = dbContext.SROMaster.Where((x => x.SROCode == resultModel.SROCode)).Distinct();

                    if (masterDataReportModel.SROId != 0)
                    {
                        SROName = dbContext.SROMaster.Where((x => x.SROCode == masterDataReportModel.SROId)).Distinct();
                    }
                    
                    foreach (var item2 in SROName)
                    {
                        resultModel.SroName = item2.SRONameE;
                    }
                    // item.ApplicationDate = Convert.ToDateTime(resultModel.ApplicationDate);
                    string input = item.ApplicationDate.ToString();
                    int index = input.LastIndexOf(" ");

                    resultModel.ApplicationDate = input.Substring(0, index);

                    // String.Format("{DD/MM/yy}", resultModel.ApplicationDate);

                    resultModel.Old_BIND_InstrumentNumber = item.Old_BIND_InstrumentNumber;

                    string olddate = item.Old_BIND_InstrumentDate.ToString();
                    string NewDate = item.Old_BIND_InstrumentDate.ToString();

                    resultModel.Old_BIND_InstrumentDate = olddate.Substring(0, index);
                    resultModel.ChallanNumber = item.ChallanNumber;
                    if (item.ChallanDate == null)
                    {
                        resultModel.ChallanDate = "NA";

                    }
                    else
                    {
                        resultModel.ChallanDate = olddate.Substring(0, index);

                    }
                    resultModel.Reason = item.Reason;
                    if (item.IsLocallyUpdated == true)
                    {
                        resultModel.IsLocallyUpdated = "YES";
                    }
                    else
                    {
                        resultModel.IsLocallyUpdated = "NO";

                    }
                    if (item.IsCentrallyupdated == true)
                    {
                        resultModel.IsCentrallyupdated = "YES";
                    }
                    else
                    {
                        resultModel.IsCentrallyupdated = "NO";

                    }

                    ResultModel.CDECDDataTableList.Add(resultModel);
                  



                }



            }
            catch (Exception e)
            {

                throw e;
            }
            return ResultModel;




        }



    }





}


