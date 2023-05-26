using CustomModels.Models.MISReports.SevaSidhuApplicationDetails;
using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
using CustomModels.Models.Remittance.MasterData;
using ECDataAPI.Common;
using ECDataAPI.EcDataService;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Common;
using ECDataUI.Session;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace ECDataAPI.Areas.MISReports.DAL
{
    //added by vijay on 01-03-2023
    public class SevaSindhuApplicationDetailsDAL
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;
        int Office_ID;
        SevaSindhuApplicationDetailsResultModel ResultModel = new SevaSindhuApplicationDetailsResultModel();
        public SevaSindhuApplicationDetailsReportModel SevaSindhuApplicationDetailsReportview(int OfficeID)
        {
            KaveriEntities dbContext = null;
            Office_ID = OfficeID;
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                SevaSindhuApplicationDetailsReportModel model = new SevaSindhuApplicationDetailsReportModel();

                model.SROfficeList = new List<SelectListItem>();
                model.DROfficeList = new List<SelectListItem>();


                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();



                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });


                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

             

                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");

                }
                else
                {
              
                    model.DROfficeList = objCommon.GetDROfficesList("All");
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                }

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);

                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
         



              

                return model;
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
                if (searchDBContext != null)
                {
                    searchDBContext.Dispose();
                }
            }
        }

        public SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails(SevaSindhuApplicationDetailsReportModel reportModel)
        {
            SevaSindhuApplicationDetailsReportTableModel model = new SevaSindhuApplicationDetailsReportTableModel();
            KaveriEntities dbContext = null;
            ResultModel.SevaSindhuApplicationDetailsReportTableList = new List<SevaSindhuApplicationDetailsReportTableModel>();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            try
            {
                //if(Convert.ToInt32(OfficeID)==18)
                //{
                //    Console.WriteLine("hello");
                //}
               
                SevaSindhuApplicationDetailsReportTableModel model2 = new SevaSindhuApplicationDetailsReportTableModel();
                KaveriEntities dbContext2 = null;
                ResultModel.SevaSindhuApplicationDetailsReportTableList = new List<SevaSindhuApplicationDetailsReportTableModel>();

                dbContext2 = new KaveriEntities();
                int count = 0;
                // var Result = dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS.ToList();
                //var Result = from item in dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                //             where (item.SROCode == reportModel.SROfficeID && (DbFunctions.TruncateTime( item.ApplicationRegistrationDateTime) <= DbFunctions.TruncateTime(reportModel.ToDate) &&
                //             DbFunctions.TruncateTime(item.ApplicationRegistrationDateTime) >= DbFunctions.TruncateTime(reportModel.FromDate)))
                //             select item;
                var Result = dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS.Take(1).ToList();
                if (reportModel.SROfficeID == 0 && reportModel.DROfficeID!=0)
                {

                   Result = dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                       .Where(x => dbContext2.SROMaster
                                    .Where(s => s.DistrictCode == reportModel.DROfficeID)
                                    .Select(s => s.SROCode)
                                    .Contains(x.SROCode)
                             && DbFunctions.TruncateTime(x.DataReceivedDate) >= DbFunctions.TruncateTime(reportModel.FromDate)
                             && DbFunctions.TruncateTime(x.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate))
                       .ToList();

                }
             else   if (reportModel.SROfficeID != 0 && reportModel.DROfficeID != 0)
                {

                    Result = dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                        .Where(x => dbContext2.SROMaster
                                     .Where(s => s.DistrictCode == reportModel.DROfficeID && s.SROCode == reportModel.SROfficeID)
                                     .Select(s => s.SROCode)
                                     .Contains(x.SROCode)
                              && DbFunctions.TruncateTime(x.DataReceivedDate) >= DbFunctions.TruncateTime(reportModel.FromDate)
                              && DbFunctions.TruncateTime(x.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate))
                        .ToList();

                }

                else
                {
                     Result = dbContext2.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS.Where((x => 
                   (DbFunctions.TruncateTime(x.DataReceivedDate) >=
                 DbFunctions.TruncateTime(reportModel.FromDate)) &&
                 (DbFunctions.TruncateTime(x.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate)))).ToList();
                }
            

                foreach (var item in Result)
                {
                    model2 = new SevaSindhuApplicationDetailsReportTableModel();

                    count++;
                    model2.SRNO = count;
                    model2.ReferenceNo = item.ReferenceNo;
                    model2.AknowledgementNo = item.AknowledgementNo;
                    model2.ApplicationRecivedDateTime = item.DataReceivedDate.ToString();
                    model2.AppointmentDateTime=item.AppointmentDateTime.ToString();
                    model2.TransXML = item.TransXML;
                    model2.MarrigecaseNo= item.FinalRegistrationNumber;
                    if (item.ApplicationStatusCode==1)
                    {
                        model2.ApplicationStatus = "Accepted";
                        model2.ApplicationAcceptDateTime = item.ApplicationAcceptDateTime.ToString();
                        model2.ApplicationRejectDateTime = "--";
                        model2.RejectionReason = "--";
                    }
                    else if(item.ApplicationStatusCode==2)
                    {
                        model2.ApplicationStatus = "Rejected";
                        model2.RejectionReason = item.RejectionReason;
                        model2.ApplicationAcceptDateTime = "--";
                        model2.ApplicationRejectDateTime = item.DataReceivedDate.ToString();    

                    }
                    else
                    {
                        model2.ApplicationStatus = "--";
                        model2.RejectionReason ="--";
                        model2.ApplicationAcceptDateTime = "--";
                        model2.ApplicationRejectDateTime = "--";




                    }
                    if (item.FinalRegistrationNumber==null)
                    {
                        model2.MarrigecaseNo = "--";
                    }
                    if(item.ApplicationRegistrationDateTime==null)
                    {
                        model2.MarriageRegistrationDate = "--";
                    }

                    else
                    model2.MarriageRegistrationDate=item.ApplicationRegistrationDateTime.ToString();

                    var OfficeName = dbContext2.SROMaster
                    .Where(x => x.SROCode == item.SROCode)
                    .Select(x => x.SRONameE)
                    .FirstOrDefault();

                        model2.OfficeName = OfficeName;
                    

                        XDocument doc = XDocument.Parse(model2.TransXML);



                    foreach (XElement item2 in doc.Descendants("MarriageRegistrationDetails"))
                    {
                        string MarriageID = item2.Element("MarriageTypeID")?.Value;
                        int MarriageTypeID = Convert.ToInt32(MarriageID);

                        model2.MarriageTypeID = MarriageID;
                        switch (MarriageTypeID)
                        {
                            case 1:
                           
                                model2.MarriageTypeID = "Hindu Marriage";
                                break;
                            case 2:
                                model2.MarriageTypeID = "Special Marriage";

                                break;
                                case 3:
                                model2.MarriageTypeID = "Special Other Marriage";
                                break;

                        }
                    }

                    ResultModel.SevaSindhuApplicationDetailsReportTableList.Add(model2);

                }
                  return ResultModel;

            }
            catch (Exception e)
            {
                throw;
            }







        }

        public SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails_For_TA(SevaSindhuApplicationDetailsReportModel reportModel)
        {

            try
            {
                SevaSindhuApplicationDetailsReportTableModel model = new SevaSindhuApplicationDetailsReportTableModel();
                KaveriEntities dbContext = null;
                dbContext=new KaveriEntities();
                ResultModel.SevaSindhuApplicationDetailsReportTableList = new List<SevaSindhuApplicationDetailsReportTableModel>();
                var result = dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS.ToList();
                foreach (var item in result)
                {
                    model = new SevaSindhuApplicationDetailsReportTableModel();

                    model.TransXML =item.TransXML;
                    model.ReferenceNo= item.ReferenceNo;
                    model.RequestID=item.RequestID;
                    model.AknowledgementNo= item.AknowledgementNo;
                    model.DataReceivedDate=item.DataReceivedDate;
                    model.AppointmentDate_Time = item.AppointmentDateTime;
                    model.AppointmentSlot = item.AppointmentSlot;
                    model.IsApplicationRegistered = item.IsApplicationRegistered;
                    model.ApplicationRegistrationDateTime=item.ApplicationRegistrationDateTime;
                    model.FinalRegistrationNumber = item.FinalRegistrationNumber;
                    model.RegistrationID= item.RegistrationID;
                    model.NoticeID=item.NoticeID;
                    model.ApplicationAcceptDateTime = item.ApplicationAcceptDateTime.ToString();
                    model.ApplicationRejectDateTime = item.ApplicationRejectDateTime.ToString();
                    model.ApplicationStatusCode = item.ApplicationStatusCode;
                    model.RejectionReason = item.RejectionReason;
                    model.SROCode=item.SROCode;


                    ResultModel.SevaSindhuApplicationDetailsReportTableList.Add(model);


                }

                return ResultModel;

            }


            catch (Exception ex)
            {
                throw;
            }


        }
    }
}