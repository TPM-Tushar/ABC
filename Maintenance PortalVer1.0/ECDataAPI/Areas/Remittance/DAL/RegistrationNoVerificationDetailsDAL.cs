#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationDetailsDAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for Registration No Verification Details.

*/
#endregion
using CustomModels.Models.Remittance.RegistrationNoVerificationDetails;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class RegistrationNoVerificationDetailsDAL
    {
        KaveriEntities dbContext = null;
        public RegistrationNoVerificationDetailsModel RegistrationNoVerificationDetailsView(int officeID)
        {
            RegistrationNoVerificationDetailsModel resModel = new RegistrationNoVerificationDetailsModel();

            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("01/01/2000", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("31/01/2023", System.Globalization.CultureInfo.InvariantCulture);
                resModel.SROfficeList = new List<SelectListItem>();
                SelectListItem selectListFirst = new SelectListItem();
                SelectListItem selectListSecond = new SelectListItem();
                selectListFirst.Text = "All";
                selectListFirst.Value = "0";
                //selectListSecond.Text = "All";
                //selectListSecond.Value = "0";
                resModel.SROfficeList.Insert(0, selectListFirst);
               // resModel.SROfficeList.Insert(1, selectListSecond);
                //string FirstRecord = "Select";
                //resModel.SROfficeList.Add(FirstRecord);
                //
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            //selectListOBJ.Text = "Select";
                            //selectListOBJ.Value = "0";
                            selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
                            selectListOBJ.Value = item.SROCode.ToString();
                            //resModel.SROfficeList.Add("Select");
                            resModel.SROfficeList.Add(selectListOBJ);
                        }
                    }
                }
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var GetDocumentList = objCommon.GetDocumentType();
                GetDocumentList.RemoveRange(3, 2);
                resModel.DocumentType = GetDocumentList;
                //

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


        public List<RegistrationNoVerificationDetailsTableModel> GetRegistrationNoVerificationDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            RegistrationNoVerificationDetailsTableModel resModel = null;
            List<RegistrationNoVerificationDetailsTableModel> RegistrationNoVerificationDetailsList = new List<RegistrationNoVerificationDetailsTableModel>();
            try
            {
                long SrCount = 1;
                dbContext = new KaveriEntities();
                resModel = new RegistrationNoVerificationDetailsTableModel();
                List<RegistrationNoVerificationDetailsTableModel> Result = new List<RegistrationNoVerificationDetailsTableModel>();
                var DocumentIDFinal = 15;

                //
                int DocumentTypeID = Convert.ToInt32(registrationNoVerificationDetailsModel.DocumentTypeId);
                int DocumentTypeIDFinal = 0;
                switch (DocumentTypeID)
                {
                    case 1:
                        {
                            DocumentTypeIDFinal = 1;
                            break;
                        }
                    case 2:
                        {
                            DocumentTypeIDFinal = 2;
                            break;
                        }
                }
                //
                var ErrorTypetxt ="";
                switch(registrationNoVerificationDetailsModel.ErrorCode)
                {
                    case 1:
                        //ErrorTypetxt = "1";
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.MisMatch).ToString();
                        break;
                    case 2:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Deleted).ToString();
                        break;
                    case 3:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Added).ToString();
                        break;
                    case 4:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralScanDocumentNotPresentLocalPresent).ToString();
                        break;
                    case 5:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralandLocalScanDocumentNotPresent).ToString();
                        break;
                    case 6:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralScanDocumentPresentLocalNotPresent).ToString();
                        break;
                    case 7:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.ScanFileMismatch).ToString();
                        break;

                    //Added by Rushikesh 6 Feb 2023
                    case 9:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp5Date).ToString();
                        break;
                    case 10:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp1Date).ToString();
                        break;
                    case 11:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp2Date).ToString();
                        break;
                    case 12:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp3Date).ToString();
                        break;
                    case 14:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp4Date).ToString();
                        break;
                    case 15:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.ExecutionDate).ToString();
                        break;
                    case 16:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.PresentationDate).ToString();
                        break;
                    case 17:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.StampDate).ToString();
                        break;
                    case 18:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.WithdrawlDate).ToString();
                        break;
                    case 19:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.RefusalDate).ToString();
                        break;
                    //Added by Rushikesh on 6 Feb 2023
                    //Update by Rushikesh on 14 Feb 2023
                    case 20:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.Stamp5Date1).ToString();
                        break;

                    default:
                        ErrorTypetxt = "0";
                        break;
                }
                //
            
          
                if (registrationNoVerificationDetailsModel.SROfficeID != 0)
                {
				   //Added By Tushar on 3 Jan 2023
                    if (registrationNoVerificationDetailsModel.IsPropertyAreaDetailsErrorType && registrationNoVerificationDetailsModel.ErrorCode != 0)
                    {
                
                        Result = dbContext.RPT_PropertyAreaDetails.Where(s => s.SROCode == registrationNoVerificationDetailsModel.SROfficeID && s.ErrorType.Contains(ErrorTypetxt)).Select(x => new RegistrationNoVerificationDetailsTableModel
                        {
                            PropertyID = x.PropertyID,
                            SROCode = x.SROCode,
                            VillageCode = x.VillageCode,
                            TotalArea = x.TotalArea,
                            MeasurementUnit = x.MeasurementUnit,
                            DocumentID = x.DocumentID,
                            BatchID = x.BatchID,
                            C_PropertyID = x.C_PropertyID,
                            C_SROCode= x.C_SROCode,
                            C_VillageCode= x.C_VillageCode,
                            C_TotalArea = x.C_TotalArea,
                            C_DocumentID= x.C_DocumentID,
                            C_MeasurementUnit = x.C_MeasurementUnit,
                            ErrorType = x.ErrorType,

                        }).ToList();
                
                    }
                    else
                    {
					//End By Tushar on 3 Jan 2023
                    //
                    #region Refresh 
                    if (registrationNoVerificationDetailsModel.IsRefresh)
                    {
                            //Added BY Tushar on 5 jan 2023
                            resModel = new RegistrationNoVerificationDetailsTableModel();

                            //Commented By ShivamB on 13-02-2023 for Disable deleting record
                            //var QueryRPT_PropertyAreaDetails = @"DELETE FROM RPT_PropertyAreaDetails
                            //              WHERE SROCode=" + registrationNoVerificationDetailsModel.SROfficeID;
                            //Ended By ShivamB on 13-02-2023 for Disable deleting record

                            //End By Tushar on 5 Jan 2023
                            var QueryRPT_DocReg_NoCLDetailsResult = @"DELETE FROM RPT_DocReg_NoCLDetails
                                          WHERE SROCode=" + registrationNoVerificationDetailsModel.SROfficeID + " " + @"AND DocumentTypeID=" + DocumentTypeIDFinal;

                        var QueryRPT_DocReg_NoCLBatchDetailsResult = @"Delete from RPT_DocReg_NoCLBatchDetails
                                          WHERE SROCode=" + registrationNoVerificationDetailsModel.SROfficeID + " "+ @"AND DocumentTypeID="+ DocumentTypeIDFinal;


                        try
                        {
                            using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                            {
                                using (SqlCommand command = new SqlCommand(QueryRPT_DocReg_NoCLDetailsResult, connection))
                                {
                                    //Aded By Tushar on 5 jan 2023
                                    command.CommandTimeout = 120;
                                    //End By Tushar on 5 Jan 2023
                                    //1
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                   

                                   //Commented By ShivamB on 13-02-2023 for Disable deleting record
                                   ////2
                                   ////Added By Tushar on 5 jan 2023
                                   // command.CommandText = QueryRPT_PropertyAreaDetails;
                                   // command.ExecuteNonQuery();
                                   // //End By Tushar on 5 Jan 2023
                                   //Ended By ShivamB on 13-02-2023 for Disable deleting record

                                    //3
                                    command.CommandText = QueryRPT_DocReg_NoCLBatchDetailsResult;
                                    command.ExecuteNonQuery();
                                    command.Dispose();

                                }
                                connection.Close();
                            }
                                //Added By Tushar on 5 jan 2023
                                resModel.RefreshMessage = "Refresh Completed with Success for Registration No Verification Details.";
                                //End By Tushar on 5 Jan 2023
                        }
                        catch (Exception ex)
                        {

                                //Added By Tushar on 5 jan 2023
                                resModel.RefreshMessage = "Error occured while Refreshing Details.";
                                //End By Tushar on 5 Jan 2023
                                //Added By ShivamB on 13-02-2023 for logging exception log
                                ApiExceptionLogs.LogError(ex, "GetRegistrationNoVerificationDetails");
                                //Added By ShivamB on 13-02-2023 for logging exception log
                                throw;
                        }
                             //Added By Tushar on 5 jan 2023
                            RegistrationNoVerificationDetailsList.Add(resModel);
                             //End By Tushar on 5 Jan 2023
                        return RegistrationNoVerificationDetailsList;
                    }
                    //
                    #endregion

                    //
                    #region FileNA
                    if (registrationNoVerificationDetailsModel.IsFileNA && registrationNoVerificationDetailsModel.IsCNull== false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {

                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID


                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName == null || DN.L_ScannedFileName == ""))
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                  
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022


                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                    //
                    #region C_NA && L_A
                    if (registrationNoVerificationDetailsModel.IsCNull && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
					var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                            /*
                            Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                      join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                      on DN.BatchID equals DNB.BatchID


                                      where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                      && ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName != null) && (DN.L_ScannedFileName != ""))
                                      && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                      && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                      && DN.DocumentTypeID == DocumentTypeIDFinal
                                      && DN.DocumentID >= DocumentIDFinal

                                      select new RegistrationNoVerificationDetailsTableModel
                                      {
                                          DocumentID = DN.DocumentID,
                                          SROCode = DN.SROCode,
                                          C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                          C_FRN = DN.C_FRN,
                                          C_ScannedFileName = DN.C_ScannedFileName,
                                          L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                          L_FRN = DN.L_FRN,
                                          L_ScannedFileName = DN.L_ScannedFileName,
                                          BatchID = DN.BatchID,
                                          C_CDNumber = DN.C_CDNumber,
                                          L_CDNumber = DN.L_CDNumber,
                                          ErrorType = DN.ErrorType,
                                          DocumentTypeID = (int)DN.DocumentTypeID,
                                          BatchDateTime = DNB.BatchDateTime.ToString(),

                                          C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                          L_ScanDate = DN.L_ScanDate.ToString(),
                                             //Added By Tushar on 29 Nov 2022
                                          L_StartTime = DN.L_StartTime.ToString(),
                                          L_EndTime = DN.L_EndTime.ToString(),
                                          L_Filesize = DN.L_Filesize.ToString(),
                                          L_Pages = (long)DN.L_Pages,
                                          L_Checksum = (long)DN.L_Checksum,
                                          IsDuplicate = DN.IsDuplicate
                                          //End By tushar on 29 Nov 2022

                                      }
                                                             ).Distinct().ToList(); */

                            //Added by rushikesh 17 feb 2023
                            /*
                            var ExceptResult = dbContext.RPT_DocReg_NoCLDetails.Where(x => x.DocumentID >= DocumentIDFinal
                               && x.DocumentTypeID == DocumentTypeIDFinal
                               && x.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                               && (x.L_ScannedFileName != null && x.L_ScannedFileName != "")
                               && (x.C_ScannedFileName == null || x.C_ScannedFileName == "")
                               && ((x.C_Stamp5DateTime == null ? x.L_Stamp5DateTime : x.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                               && (x.C_Stamp5DateTime == null ? x.L_Stamp5DateTime : x.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date)))                           
                               .Select(s => s.DocumentID)
                               .Except(dbContext.ScannedFileUploadDetails.Select(s => s.DocumentID))
                               .ToList();

                            Result = (from t1 in dbContext.RPT_DocReg_NoCLDetails
                                      join t3 in dbContext.RPT_DocReg_NoCLBatchDetails on t1.BatchID equals t3.BatchID
                                      where ExceptResult.Contains(t1.DocumentID)

                                      select new RegistrationNoVerificationDetailsTableModel
                                      {
                                          DocumentID = t1.DocumentID,
                                          SROCode = t1.SROCode,
                                          C_Stamp5DateTime = t1.C_Stamp5DateTime.ToString(),
                                          C_FRN = t1.C_FRN,
                                          C_ScannedFileName = t1.C_ScannedFileName,
                                          L_Stamp5DateTime = t1.L_Stamp5DateTime.ToString(),
                                          L_FRN = t1.L_FRN,
                                          L_ScannedFileName = t1.L_ScannedFileName,
                                          BatchID = t1.BatchID,
                                          C_CDNumber = t1.C_CDNumber,
                                          L_CDNumber = t1.L_CDNumber,
                                          ErrorType = t1.ErrorType,
                                          DocumentTypeID = (int)t1.DocumentTypeID,
                                          BatchDateTime = t3.BatchDateTime.ToString(),
                                          C_ScanFileUploadDateTime = t1.C_ScanFileUploadDateTime.ToString(),
                                          L_ScanDate = t1.L_ScanDate.ToString(),
                                          //Added By Tushar on 29 Nov 2022
                                          L_StartTime = t1.L_StartTime.ToString(),
                                          L_EndTime = t1.L_EndTime.ToString(),
                                          L_Filesize = t1.L_Filesize.ToString(),
                                          L_Pages = (long)t1.L_Pages,
                                          L_Checksum = (long)t1.L_Checksum,
                                          IsDuplicate = t1.IsDuplicate
                                          //End By tushar on 29 Nov 2022
                                      }).Distinct().ToList();
                            */
                            //Commented and Added By Tushar on 10 May 2023
                            Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                      join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                      join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                      from SF in SFJoin.DefaultIfEmpty()
                                      where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails
                                      && DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                      && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude

                                       && ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName != null) && (DN.L_ScannedFileName != ""))
                                      && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                      && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                      && DN.DocumentTypeID == DocumentTypeIDFinal
                                      && DN.DocumentID >= DocumentIDFinal
                                      //
                                      select new RegistrationNoVerificationDetailsTableModel
                                      {
                                          DocumentID = DN.DocumentID,
                                          SROCode = DN.SROCode,
                                          C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                          C_FRN = DN.C_FRN,
                                          C_ScannedFileName = DN.C_ScannedFileName,
                                          L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                          L_FRN = DN.L_FRN,
                                          L_ScannedFileName = DN.L_ScannedFileName,
                                          BatchID = DN.BatchID,
                                          C_CDNumber = DN.C_CDNumber,
                                          L_CDNumber = DN.L_CDNumber,
                                          ErrorType = DN.ErrorType,
                                          DocumentTypeID = (int)DN.DocumentTypeID,
                                          BatchDateTime = DNB.BatchDateTime.ToString(),
                                          C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                          L_ScanDate = DN.L_ScanDate.ToString(),
                                          L_StartTime = DN.L_StartTime.ToString(),
                                          L_EndTime = DN.L_EndTime.ToString(),
                                          L_Filesize = DN.L_Filesize.ToString(),
                                          L_Pages = (long)DN.L_Pages,
                                          L_Checksum = (long)DN.L_Checksum,
                                          IsDuplicate = DN.IsDuplicate
                                      }).Distinct().ToList();
                            //End byTushar on 10 May 2023
                        }
                    #endregion
                    
                    #region C_A && L_NA
                    if (registrationNoVerificationDetailsModel.IsLNull && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {

                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID


                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && ((DN.C_ScannedFileName != null) && (DN.C_ScannedFileName != "") && (DN.L_ScannedFileName == null || DN.L_ScannedFileName == ""))
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),

                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                         //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022

                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                    //

                    #region IsDuplicate
                    if(registrationNoVerificationDetailsModel.IsDuplicate)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID into z
                                  from y in z.DefaultIfEmpty()


                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && DN.IsDuplicate == true
                                  //Commented By Tushar on 10 May 2023
                                  //&& ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  //&& (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      //BatchDateTime = DNB.BatchDateTime.ToString(),
                                      BatchDateTime = y.BatchDateTime.ToString(),

                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022

                                  }
                                  ).Distinct().OrderBy(a => a.L_FRN).ThenBy(a => a.SROCode).ToList();
                    }
                    #endregion
                    //Added By Tushar on 2 Nov 2022
                    #region Errortype
                    if (registrationNoVerificationDetailsModel.IsErrorTypecheck && registrationNoVerificationDetailsModel.ErrorCode > 0)
                    {
                            //updated by rushikesh 14 feb 2023
                            if (!registrationNoVerificationDetailsModel.IsDateErrorType_DateDetails)
                            {
                                //Result = (from DN in dbContext.RPT_DocReg_NoCLDetails.AsEnumerable()
                                //          join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                //          on DN.BatchID equals DNB.BatchID

                                //          where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID 
                                //          && DN.ErrorType.Split(',').Contains(ErrorTypetxt) 
                                //          && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                //          && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                //          && DN.DocumentTypeID == DocumentTypeIDFinal
                                //          && DN.DocumentID >= DocumentIDFinal

                                //          select new RegistrationNoVerificationDetailsTableModel
                                //          {
                                //              DocumentID = DN.DocumentID,
                                //              SROCode = DN.SROCode,
                                //              C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                //              C_FRN = DN.C_FRN,
                                //              C_ScannedFileName = DN.C_ScannedFileName,
                                //              L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                //              L_FRN = DN.L_FRN,
                                //              L_ScannedFileName = DN.L_ScannedFileName,
                                //              BatchID = DN.BatchID,
                                //              C_CDNumber = DN.C_CDNumber,
                                //              L_CDNumber = DN.L_CDNumber,
                                //              ErrorType = DN.ErrorType,
                                //              DocumentTypeID = (int)DN.DocumentTypeID,
                                //              BatchDateTime = DNB.BatchDateTime.ToString(),

                                //              C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                //              L_ScanDate = DN.L_ScanDate.ToString(),
                                //              //Added By Tushar on 29 Nov 2022
                                //              L_StartTime = DN.L_StartTime.ToString(),
                                //              L_EndTime = DN.L_EndTime.ToString(),
                                //              L_Filesize = DN.L_Filesize.ToString(),
                                //              L_Pages = (long)DN.L_Pages,
                                //              L_Checksum = (long)DN.L_Checksum,
                                //              IsDuplicate = DN.IsDuplicate,
                                //              //End By tushar on 29 Nov 2022

                                //              //Added By Rushikesh on 6 Feb 2023

                                //              //Local
                                //              L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                //              L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                //              L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                //              L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                //              L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                //              L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                //              L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                //              L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                //              L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                //              L_RefusalDate = DN.L_RefusalDate.ToString(),

                                //              //Central
                                //              C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                //              C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                //              C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                //              C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                //              C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                //              C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                //              C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                //              C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                //              C_RefusalDate = DN.C_RefusalDate.ToString(),
                                //              //End By Rushikesh on 6 Feb 2023

                                //          }).Distinct().ToList(); 
                                //Commented and Added By Tushar on 14 March 2023 because query needs more time to execute
                                //Added By Tushar on 10 May 2023
                                if (ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.CentralandLocalScanDocumentNotPresent).ToString() || ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.CentralScanDocumentNotPresentLocalPresent).ToString())
                                {
                                    var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                                    var QueryResultForSpecificErrorType = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                                           join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                                                           join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                                                           from SF in SFJoin.DefaultIfEmpty()
                                                                           where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails
                                                                           && DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                                           && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude
                                                                           && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                                           && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                           && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                           && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                           && DN.DocumentID >= DocumentIDFinal
                                                                           //
                                                                           select new


                                                                           {
                                                                               DocumentID = DN.DocumentID,
                                                                               SROCode = DN.SROCode,
                                                                               C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                                               C_FRN = DN.C_FRN,

                                                                               C_ScannedFileName = DN.C_ScannedFileName,
                                                                               L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                                               L_FRN = DN.L_FRN,
                                                                               L_ScannedFileName = DN.L_ScannedFileName,
                                                                               BatchID = DN.BatchID,
                                                                               C_CDNumber = DN.C_CDNumber,
                                                                               L_CDNumber = DN.L_CDNumber,
                                                                               ErrorType = DN.ErrorType,
                                                                               DocumentTypeID = (int)DN.DocumentTypeID,
                                                                               BatchDateTime = DNB.BatchDateTime,

                                                                               C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                                               L_ScanDate = DN.L_ScanDate.ToString(),
                                                                               //Added By Tushar on 29 Nov 2022
                                                                               L_StartTime = DN.L_StartTime,
                                                                               L_EndTime = DN.L_EndTime,
                                                                               L_Filesize = DN.L_Filesize,
                                                                               L_Pages = (long)DN.L_Pages,
                                                                               L_Checksum = (long)DN.L_Checksum,
                                                                               IsDuplicate = DN.IsDuplicate,
                                                                               //End By tushar on 29 Nov 2022

                                                                               //Added By Rushikesh on 6 Feb 2023

                                                                               //Local
                                                                               L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                                               L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                                               L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                                               L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                                               L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                                               L_PresentDateTime = DN.L_PresentDateTime,
                                                                               L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                                               L_DateOfStamp = DN.L_DateOfStamp,
                                                                               L_WithdrawalDate = DN.L_WithdrawalDate,
                                                                               L_RefusalDate = DN.L_RefusalDate,

                                                                               //Central
                                                                               C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                                               C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                                               C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                                               C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                                               C_PresentDateTime = DN.C_PresentDateTime,
                                                                               C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                                               C_DateOfStamp = DN.C_DateOfStamp,
                                                                               C_WithdrawalDate = DN.C_WithdrawalDate,
                                                                               C_RefusalDate = DN.C_RefusalDate,
                                                                           }).Distinct().ToList();
                                    Result = QueryResultForSpecificErrorType.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                }
                                else if (ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.MisMatch).ToString())
                                {
                                    //
                                    var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                                    var QueryResultForMisMatch = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                                                  on DN.BatchID equals DNB.BatchID
                                                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                                   && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                                   && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",")  // Exclude records with ErrorType containing ErrorTypetxtToExclude

                                                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                      && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                  && DN.DocumentID >= DocumentIDFinal
                                                                  select new


                                                                  {
                                                                      DocumentID = DN.DocumentID,
                                                                      SROCode = DN.SROCode,
                                                                      C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                                      C_FRN = DN.C_FRN,

                                                                      C_ScannedFileName = DN.C_ScannedFileName,
                                                                      L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                                      L_FRN = DN.L_FRN,
                                                                      L_ScannedFileName = DN.L_ScannedFileName,
                                                                      BatchID = DN.BatchID,
                                                                      C_CDNumber = DN.C_CDNumber,
                                                                      L_CDNumber = DN.L_CDNumber,
                                                                      ErrorType = DN.ErrorType,
                                                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                                                      BatchDateTime = DNB.BatchDateTime,

                                                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                                                      //Added By Tushar on 29 Nov 2022
                                                                      L_StartTime = DN.L_StartTime,
                                                                      L_EndTime = DN.L_EndTime,
                                                                      L_Filesize = DN.L_Filesize,
                                                                      L_Pages = (long)DN.L_Pages,
                                                                      L_Checksum = (long)DN.L_Checksum,
                                                                      IsDuplicate = DN.IsDuplicate,
                                                                      //End By tushar on 29 Nov 2022

                                                                      //Added By Rushikesh on 6 Feb 2023

                                                                      //Local
                                                                      L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                                      L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                                      L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                                      L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                                      L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                                      L_PresentDateTime = DN.L_PresentDateTime,
                                                                      L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                                      L_DateOfStamp = DN.L_DateOfStamp,
                                                                      L_WithdrawalDate = DN.L_WithdrawalDate,
                                                                      L_RefusalDate = DN.L_RefusalDate,

                                                                      //Central
                                                                      C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                                      C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                                      C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                                      C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                                      C_PresentDateTime = DN.C_PresentDateTime,
                                                                      C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                                      C_DateOfStamp = DN.C_DateOfStamp,
                                                                      C_WithdrawalDate = DN.C_WithdrawalDate,
                                                                      C_RefusalDate = DN.C_RefusalDate,
                                                                  }).Distinct().ToList();
                                    Result = QueryResultForMisMatch.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                }
                                else
                                {
                                    //End By Tushar on 10 May 2023
                                    var QueryResult = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                       join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                                       on DN.BatchID equals DNB.BatchID
                                                       where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                        && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                       && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                           && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                       && DN.DocumentTypeID == DocumentTypeIDFinal
                                                       && DN.DocumentID >= DocumentIDFinal
                                                       select new


                                                       {
                                                           DocumentID = DN.DocumentID,
                                                           SROCode = DN.SROCode,
                                                           C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                           C_FRN = DN.C_FRN,

                                                           C_ScannedFileName = DN.C_ScannedFileName,
                                                           L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                           L_FRN = DN.L_FRN,
                                                           L_ScannedFileName = DN.L_ScannedFileName,
                                                           BatchID = DN.BatchID,
                                                           C_CDNumber = DN.C_CDNumber,
                                                           L_CDNumber = DN.L_CDNumber,
                                                           ErrorType = DN.ErrorType,
                                                           DocumentTypeID = (int)DN.DocumentTypeID,
                                                           BatchDateTime = DNB.BatchDateTime,

                                                           C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                           L_ScanDate = DN.L_ScanDate.ToString(),
                                                           //Added By Tushar on 29 Nov 2022
                                                           L_StartTime = DN.L_StartTime,
                                                           L_EndTime = DN.L_EndTime,
                                                           L_Filesize = DN.L_Filesize,
                                                           L_Pages = (long)DN.L_Pages,
                                                           L_Checksum = (long)DN.L_Checksum,
                                                           IsDuplicate = DN.IsDuplicate,
                                                           //End By tushar on 29 Nov 2022

                                                           //Added By Rushikesh on 6 Feb 2023

                                                           //Local
                                                           L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                           L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                           L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                           L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                           L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                           L_PresentDateTime = DN.L_PresentDateTime,
                                                           L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                           L_DateOfStamp = DN.L_DateOfStamp,
                                                           L_WithdrawalDate = DN.L_WithdrawalDate,
                                                           L_RefusalDate = DN.L_RefusalDate,

                                                           //Central
                                                           C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                           C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                           C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                           C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                           C_PresentDateTime = DN.C_PresentDateTime,
                                                           C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                           C_DateOfStamp = DN.C_DateOfStamp,
                                                           C_WithdrawalDate = DN.C_WithdrawalDate,
                                                           C_RefusalDate = DN.C_RefusalDate,
                                                       }).Distinct().ToList();
                                    Result = QueryResult.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                    //End By Tushar on 14 March 2023
                                }
                            }
                            //added by rushikesh 14 feb 2023
                            else
                            {
                                //Result= dbContext.RPT_DocReg_NoCLDetails.Join(dbContext.RPT_DocReg_DateDetails,s=>s.DocumentID,d=>d.DocumentID,(m,n)=> new { } )

                                //Result = dbContext.RPT_DocReg_DateDetails.Where(x => x.ErrorType.Contains(ErrorTypetxt) && x.SROCode == registrationNoVerificationDetailsModel.SROfficeID).Select(s => new RegistrationNoVerificationDetailsTableModel
                                //
                                Result = dbContext.RPT_DocReg_NoCLDetails.Join(dbContext.RPT_DocReg_DateDetails,
                                t1 => t1.DocumentID, RPT_DocReg_NoCLDetailsResult => RPT_DocReg_NoCLDetailsResult.DocumentID,
                                (m, n) => new { t1 = m, RPT_DocReg_NoCLDetailsResult = n })
                                    .Where(x => x.t1.DocumentTypeID == registrationNoVerificationDetailsModel.DocumentTypeId && x.RPT_DocReg_NoCLDetailsResult.ErrorType.Contains(ErrorTypetxt) && x.RPT_DocReg_NoCLDetailsResult.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                                              && ((x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                              && (x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                              && x.t1.DocumentTypeID == DocumentTypeIDFinal
                                                                              && x.t1.DocumentID >= DocumentIDFinal)

                   .Select(x => new RegistrationNoVerificationDetailsTableModel
                   {
                       DocumentID = x.RPT_DocReg_NoCLDetailsResult.DocumentID,
                       SROCode = x.RPT_DocReg_NoCLDetailsResult.SROCode,
                       TableName = x.RPT_DocReg_NoCLDetailsResult.TableName,
                       ReceiptID = x.RPT_DocReg_NoCLDetailsResult.ReceiptID,
                       L_DateOfReceipt = x.RPT_DocReg_NoCLDetailsResult.L_DateOfReceipt.ToString(),
                       C_DateOfReceipt = x.RPT_DocReg_NoCLDetailsResult.C_DateOfReceipt.ToString(),
                       StampDetailsID = x.RPT_DocReg_NoCLDetailsResult.StampDetailsID,
                       L_DateOfStamp = x.RPT_DocReg_NoCLDetailsResult.L_DateOfStamp.ToString(),
                       C_DateOfStamp = x.RPT_DocReg_NoCLDetailsResult.C_DateOfStamp.ToString(),
                       L_DDChalDate = x.RPT_DocReg_NoCLDetailsResult.L_DDChalDate.ToString(),
                       C_DDChalDate = x.RPT_DocReg_NoCLDetailsResult.C_DDChalDate.ToString(),
                       L_StampPaymentDate = x.RPT_DocReg_NoCLDetailsResult.L_StampPaymentDate.ToString(),
                       C_StampPaymentDate = x.RPT_DocReg_NoCLDetailsResult.C_StampPaymentDate.ToString(),
                       L_DateOfReturn = x.RPT_DocReg_NoCLDetailsResult.L_DateOfReturn.ToString(),
                       C_DateOfReturn = x.RPT_DocReg_NoCLDetailsResult.C_DateOfReturn.ToString(),
                       PartyID = x.RPT_DocReg_NoCLDetailsResult.PartyID,
                       L_AdmissionDate = x.RPT_DocReg_NoCLDetailsResult.L_AdmissionDate.ToString(),
                       C_AdmissionDate = x.RPT_DocReg_NoCLDetailsResult.C_AdmissionDate.ToString(),
                       BatchID = x.RPT_DocReg_NoCLDetailsResult.BatchID,
                       ErrorType = x.RPT_DocReg_NoCLDetailsResult.ErrorType
                   })
                  .ToList();
                                /*
                                Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                    join DNB in dbContext.RPT_DocReg_DateDetails on new { p1=DN.DocumentID,p2=DN.SROCode}  equals {p1= DNB. }
                                    where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                                              && DNB.ErrorType.Contains(ErrorTypetxt)
                                                                              && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                              && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                              && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                              && DN.DocumentID >= DocumentIDFinal
                                    select new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DNB.DocumentID,
                                        SROCode = DNB.SROCode,
                                        TableName = DNB.TableName,
                                        ReceiptID = DNB.ReceiptID,
                                        L_DateOfReceipt = DNB.L_DateOfReceipt.ToString(),
                                        C_DateOfReceipt = DNB.C_DateOfReceipt.ToString(),
                                        StampDetailsID = DNB.StampDetailsID,
                                        L_DateOfStamp = DNB.L_DateOfStamp.ToString(),
                                        C_DateOfStamp = DNB.C_DateOfStamp.ToString(),
                                        L_DDChalDate = DNB.L_DDChalDate.ToString(),
                                        C_DDChalDate = DNB.C_DDChalDate.ToString(),
                                        L_StampPaymentDate = DNB.L_StampPaymentDate.ToString(),
                                        C_StampPaymentDate = DNB.C_StampPaymentDate.ToString(),
                                        L_DateOfReturn = DNB.L_DateOfReturn.ToString(),
                                        C_DateOfReturn = DNB.C_DateOfReturn.ToString(),
                                        PartyID = DNB.PartyID,
                                        L_AdmissionDate = DNB.L_AdmissionDate.ToString(),
                                        C_AdmissionDate = DNB.C_AdmissionDate.ToString(),
                                        BatchID = DNB.BatchID,
                                        ErrorType = DNB.ErrorType

                                    }
                                    ).ToList();
                                        */
                            }
                            //end by rushikesh 14 feb 2023
                    }
                    #endregion
                    //End By Tushar on 2 Nov 2022
                    #region FRN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == true && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                      
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                            
                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && (DN.C_FRN != DN.L_FRN)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  //|| (DN.C_FRN == null)
                                  //|| (DN.L_FRN == null))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion

                    #region SFN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == true && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID


                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && (DN.C_ScannedFileName != DN.L_ScannedFileName)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                   && DN.DocumentTypeID == DocumentTypeIDFinal
                                  //|| (DN.C_FRN == null)
                                  //|| (DN.L_FRN == null))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion

                    #region FRN and SFN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == true && registrationNoVerificationDetailsModel.IsSFNCheck == true && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID


                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && (DN.C_ScannedFileName != DN.L_ScannedFileName)
                                  && (DN.C_FRN != DN.L_FRN)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  //|| (DN.C_FRN == null)
                                  //|| (DN.L_FRN == null))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion

                    #region SROCODE != 0 && DocumentType == Document
                    if ((registrationNoVerificationDetailsModel.DocumentTypeId == Convert.ToInt64(Common.ApiCommonEnum.DocumentType.Document)) && registrationNoVerificationDetailsModel.IsDateNull == false && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  //  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  //  && (DN.C_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.C_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //// && (DN.L_Stamp5DateTime == DBNull.Value ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime)
                                  //  //  && (Convert.ToDateTime(DN.C_Stamp5DateTime).Date > registrationNoVerificationDetailsModel.DateTime_Date.Date && Convert.ToDateTime(DN.C_Stamp5DateTime).Date < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //  where DN.L_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.L_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date
                                  //where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID && ( (DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) > registrationNoVerificationDetailsModel.DateTime_Date.Date &&
                                  //(DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) < (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  //
                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && DN.DocumentTypeID == 1
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022

                                  }
                                                         ).Distinct().ToList(); 
                    }
                    #endregion

                    #region SROCODE != 0 && DocumentType == Marriage
                    if ((registrationNoVerificationDetailsModel.DocumentTypeId == Convert.ToInt64(Common.ApiCommonEnum.DocumentType.Marriage)) && registrationNoVerificationDetailsModel.IsDateNull == false && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  //  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  //  && (DN.C_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.C_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //// && (DN.L_Stamp5DateTime == DBNull.Value ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime)
                                  //  //  && (Convert.ToDateTime(DN.C_Stamp5DateTime).Date > registrationNoVerificationDetailsModel.DateTime_Date.Date && Convert.ToDateTime(DN.C_Stamp5DateTime).Date < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //  where DN.L_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.L_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date
                                  //where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID && ( (DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) > registrationNoVerificationDetailsModel.DateTime_Date.Date &&
                                  //(DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) < (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  //
                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  && DN.DocumentTypeID == 2
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                        ).Distinct().ToList();  
                    }
                    #endregion

                    #region Both Date NULL
                    if (registrationNoVerificationDetailsModel.IsDateNull && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  //  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                  //  && (DN.C_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.C_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //// && (DN.L_Stamp5DateTime == DBNull.Value ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime)
                                  //  //  && (Convert.ToDateTime(DN.C_Stamp5DateTime).Date > registrationNoVerificationDetailsModel.DateTime_Date.Date && Convert.ToDateTime(DN.C_Stamp5DateTime).Date < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //  where DN.L_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.L_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date
                                  //where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID && ( (DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) > registrationNoVerificationDetailsModel.DateTime_Date.Date &&
                                  //(DN.L_Stamp5DateTime.Equals(null) ? DN.C_Stamp5DateTime : DN.L_Stamp5DateTime) < (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  //
                                  where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID

                                  && (DN.C_Stamp5DateTime == null)
                                  && (DN.L_Stamp5DateTime == null)

                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion

                    }
                }
                else
                {
                    //Added by rushikesh 9 feb 2023
                    //For all records
                    if (registrationNoVerificationDetailsModel.IsDateErrorType_DateDetails && registrationNoVerificationDetailsModel.ErrorCode != 0)
                    {
                        Result = dbContext.RPT_DocReg_NoCLDetails.Join(dbContext.RPT_DocReg_DateDetails,
                                 t1 => t1.DocumentID, RPT_DocReg_NoCLDetailsResult => RPT_DocReg_NoCLDetailsResult.DocumentID,
                                 (m, n) => new { t1 = m, RPT_DocReg_NoCLDetailsResult = n })
                                    .Where(x=>x.t1.DocumentTypeID == registrationNoVerificationDetailsModel.DocumentTypeId && x.RPT_DocReg_NoCLDetailsResult.ErrorType.Contains(ErrorTypetxt)
                                                                              && ((x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                              && (x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                              && x.t1.DocumentTypeID == DocumentTypeIDFinal
                                                                              && x.t1.DocumentID >= DocumentIDFinal)
           
                   .Select(x => new RegistrationNoVerificationDetailsTableModel
                   {
                       DocumentID = x.RPT_DocReg_NoCLDetailsResult.DocumentID,
                       SROCode = x.RPT_DocReg_NoCLDetailsResult.SROCode,
                       TableName = x.RPT_DocReg_NoCLDetailsResult.TableName,
                       ReceiptID = x.RPT_DocReg_NoCLDetailsResult.ReceiptID,
                       L_DateOfReceipt = x.RPT_DocReg_NoCLDetailsResult.L_DateOfReceipt.ToString(),
                       C_DateOfReceipt = x.RPT_DocReg_NoCLDetailsResult.C_DateOfReceipt.ToString(),
                       StampDetailsID = x.RPT_DocReg_NoCLDetailsResult.StampDetailsID,
                       L_DateOfStamp = x.RPT_DocReg_NoCLDetailsResult.L_DateOfStamp.ToString(),
                       C_DateOfStamp = x.RPT_DocReg_NoCLDetailsResult.C_DateOfStamp.ToString(),
                       L_DDChalDate = x.RPT_DocReg_NoCLDetailsResult.L_DDChalDate.ToString(),
                       C_DDChalDate = x.RPT_DocReg_NoCLDetailsResult.C_DDChalDate.ToString(),
                       L_StampPaymentDate = x.RPT_DocReg_NoCLDetailsResult.L_StampPaymentDate.ToString(),
                       C_StampPaymentDate = x.RPT_DocReg_NoCLDetailsResult.C_StampPaymentDate.ToString(),
                       L_DateOfReturn = x.RPT_DocReg_NoCLDetailsResult.L_DateOfReturn.ToString(),
                       C_DateOfReturn = x.RPT_DocReg_NoCLDetailsResult.C_DateOfReturn.ToString(),
                       PartyID = x.RPT_DocReg_NoCLDetailsResult.PartyID,
                       L_AdmissionDate = x.RPT_DocReg_NoCLDetailsResult.L_AdmissionDate.ToString(),
                       C_AdmissionDate = x.RPT_DocReg_NoCLDetailsResult.C_AdmissionDate.ToString(),
                       BatchID = x.RPT_DocReg_NoCLDetailsResult.BatchID,
                       ErrorType = x.RPT_DocReg_NoCLDetailsResult.ErrorType
                   })
                  .ToList();
                        /*
                        Result = dbContext.RPT_DocReg_DateDetails.Where(x => x.ErrorType.Contains(ErrorTypetxt)).Select(s => new RegistrationNoVerificationDetailsTableModel

                        {
                            DocumentID = s.DocumentID,
                            SROCode = s.SROCode,
                            TableName = s.TableName,
                            ReceiptID = s.ReceiptID,
                            L_DateOfReceipt = s.L_DateOfReceipt.ToString(),
                            C_DateOfReceipt = s.C_DateOfReceipt.ToString(),
                            StampDetailsID = s.StampDetailsID,
                            L_DateOfStamp = s.L_DateOfStamp.ToString(),
                            C_DateOfStamp = s.C_DateOfStamp.ToString(),
                            L_DDChalDate = s.L_DDChalDate.ToString(),
                            C_DDChalDate = s.C_DDChalDate.ToString(),
                            L_StampPaymentDate = s.L_StampPaymentDate.ToString(),
                            C_StampPaymentDate = s.C_StampPaymentDate.ToString(),
                            L_DateOfReturn = s.L_DateOfReturn.ToString(),
                            C_DateOfReturn = s.C_DateOfReturn.ToString(),
                            PartyID = s.PartyID,
                            L_AdmissionDate = s.L_AdmissionDate.ToString(),
                            C_AdmissionDate = s.C_AdmissionDate.ToString(),
                            BatchID = s.BatchID,
                            ErrorType = s.ErrorType,

                        }).ToList();
                        */
                    }
                    //End by rushikesh 9 feb 2023

                    //Added By Tushar on 3 Jan 2023
                    if (registrationNoVerificationDetailsModel.IsPropertyAreaDetailsErrorType && registrationNoVerificationDetailsModel.ErrorCode != 0)
                    {
            
                        Result = dbContext.RPT_PropertyAreaDetails.Where(s=>s.ErrorType.Contains(ErrorTypetxt)).Select(x => new RegistrationNoVerificationDetailsTableModel
                        {
                            PropertyID = x.PropertyID,
                            SROCode = x.SROCode,
                            VillageCode = x.VillageCode,
                            TotalArea = x.TotalArea,
                            MeasurementUnit = x.MeasurementUnit,
                            DocumentID = x.DocumentID,
                            BatchID = x.BatchID,
                            C_PropertyID = x.C_PropertyID,
                            C_SROCode = x.C_SROCode,
                            C_VillageCode = x.C_VillageCode,
                            C_TotalArea = x.C_TotalArea,
                            C_DocumentID = x.C_DocumentID,
                            C_MeasurementUnit = x.C_MeasurementUnit,
                            ErrorType = x.ErrorType,

                        }).ToList();
              
                    }
                    else { 
					//End By Tushar on 3 Jan 2023
                    #region Refresh
                    if (registrationNoVerificationDetailsModel.IsRefresh)
                    {

                        var QueryRPT_DocReg_NoCLDetailsResult = @"DELETE FROM RPT_DocReg_NoCLDetails
                                          WHERE DocumentTypeID=" + DocumentTypeIDFinal;
                        var QueryRPT_DocReg_NoCLBatchDetailsResult = @"Delete from RPT_DocReg_NoCLBatchDetails
                                          WHERE DocumentTypeID=" + DocumentTypeIDFinal;


                        try
                        {
                            using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                            {
                                using (SqlCommand command = new SqlCommand(QueryRPT_DocReg_NoCLDetailsResult, connection))
                                {
                                    //1
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    //2
                                    command.CommandText = QueryRPT_DocReg_NoCLBatchDetailsResult;
                                    command.ExecuteNonQuery();
                                    command.Dispose();

                                }
                                connection.Close();
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                     
                        

                    return RegistrationNoVerificationDetailsList;
                    }
                    #endregion
                    //
                    #region FileNA
                    if (registrationNoVerificationDetailsModel.IsFileNA && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {

                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  where
                              
                                  ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName == null || DN.L_ScannedFileName == ""))
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();
                    }
                    #endregion
                    //
                    #region C_NA && L_A
                    if (registrationNoVerificationDetailsModel.IsCNull && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                            //Updated By rushikesh 21 Feb 2023

                            //using Except [For Non SRO and if SRO: s.SROCode == registrationNoVerificationDetailsModel.SROfficeID]
                            /*
                            var ExceptResult = dbContext.RPT_DocReg_NoCLDetails.Where(x => x.DocumentID >= DocumentIDFinal 
                            && x.DocumentTypeID == DocumentTypeIDFinal
                            && (x.L_ScannedFileName != null && x.L_ScannedFileName != "") 
                            && (x.C_ScannedFileName == null || x.C_ScannedFileName == "")
                            && ((x.C_Stamp5DateTime == null ? x.L_Stamp5DateTime : x.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                            && (x.C_Stamp5DateTime == null ? x.L_Stamp5DateTime : x.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date)))
                            .Select(s => s.DocumentID)
                            .Except(dbContext.ScannedFileUploadDetails.Select(s => s.DocumentID)).ToList();

                            Result = (from t1 in dbContext.RPT_DocReg_NoCLDetails
                                      join t3 in dbContext.RPT_DocReg_NoCLBatchDetails on t1.BatchID equals t3.BatchID
                                      where ExceptResult.Contains(t1.DocumentID)
                                      select new RegistrationNoVerificationDetailsTableModel
                                      {
                                          DocumentID = t1.DocumentID,
                                          SROCode = t1.SROCode,
                                          C_Stamp5DateTime = t1.C_Stamp5DateTime.ToString(),
                                          C_FRN = t1.C_FRN,
                                          C_ScannedFileName = t1.C_ScannedFileName,
                                          L_Stamp5DateTime = t1.L_Stamp5DateTime.ToString(),
                                          L_FRN = t1.L_FRN,
                                          L_ScannedFileName = t1.L_ScannedFileName,
                                          BatchID = t1.BatchID,
                                          C_CDNumber = t1.C_CDNumber,
                                          L_CDNumber = t1.L_CDNumber,
                                          ErrorType = t1.ErrorType,
                                          DocumentTypeID = (int)t1.DocumentTypeID,
                                          BatchDateTime = t3.BatchDateTime.ToString(),
                                          C_ScanFileUploadDateTime = t1.C_ScanFileUploadDateTime.ToString(),
                                          L_ScanDate = t1.L_ScanDate.ToString(),
                                          L_StartTime = t1.L_StartTime.ToString(),
                                          L_EndTime = t1.L_EndTime.ToString(),
                                          L_Filesize = t1.L_Filesize.ToString(),
                                          L_Pages = (long)t1.L_Pages,
                                          L_Checksum = (long)t1.L_Checksum,
                                          IsDuplicate = t1.IsDuplicate
                                      }).Distinct().ToList();
                            */
                            /*
                           Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                     join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                     on DN.BatchID equals DNB.BatchID

                                     where

                                     ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName != null) && (DN.L_ScannedFileName != ""))
                                     && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                     && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                     && DN.DocumentTypeID == DocumentTypeIDFinal
                                     && DN.DocumentID >= DocumentIDFinal

                              select new RegistrationNoVerificationDetailsTableModel
                              {
                                           DocumentID = DN.DocumentID,
                                           SROCode = DN.SROCode,
                                           C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                           C_FRN = DN.C_FRN,
                                           C_ScannedFileName = DN.C_ScannedFileName,
                                           L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                           L_FRN = DN.L_FRN,
                                           L_ScannedFileName = DN.L_ScannedFileName,
                                           BatchID = DN.BatchID,
                                           C_CDNumber = DN.C_CDNumber,
                                           L_CDNumber = DN.L_CDNumber,
                                           ErrorType = DN.ErrorType,
                                           DocumentTypeID = (int)DN.DocumentTypeID,
                                           BatchDateTime = DN.BatchDateTime.ToString(),
                                           C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                           L_ScanDate = DN.L_ScanDate.ToString(),
                                           //Added By Tushar on 29 Nov 2022
                                           L_StartTime = DN.L_StartTime.ToString(),
                                           L_EndTime = DN.L_EndTime.ToString(),
                                           L_Filesize = DN.L_Filesize.ToString(),
                                           L_Pages = (long)DN.L_Pages,
                                           L_Checksum = (long)DN.L_Checksum,
                                           IsDuplicate = DN.IsDuplicate
                                       }).Distinct().ToList();
                               */
                               //Commented and added by Tushar on 10 May 2023
                            var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                            Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                      join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                      join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                      from SF in SFJoin.DefaultIfEmpty()
                                      where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails

                                      && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude

                                       && ((DN.C_ScannedFileName == null || DN.C_ScannedFileName == "") && (DN.L_ScannedFileName != null) && (DN.L_ScannedFileName != ""))
                                      && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                      && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                      && DN.DocumentTypeID == DocumentTypeIDFinal
                                      && DN.DocumentID >= DocumentIDFinal
                                      //
                                      select new RegistrationNoVerificationDetailsTableModel
                                      {
                                          DocumentID = DN.DocumentID,
                                          SROCode = DN.SROCode,
                                          C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                          C_FRN = DN.C_FRN,
                                          C_ScannedFileName = DN.C_ScannedFileName,
                                          L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                          L_FRN = DN.L_FRN,
                                          L_ScannedFileName = DN.L_ScannedFileName,
                                          BatchID = DN.BatchID,
                                          C_CDNumber = DN.C_CDNumber,
                                          L_CDNumber = DN.L_CDNumber,
                                          ErrorType = DN.ErrorType,
                                          DocumentTypeID = (int)DN.DocumentTypeID,
                                          BatchDateTime = DNB.BatchDateTime.ToString(),
                                          C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                          L_ScanDate = DN.L_ScanDate.ToString(),
                                          L_StartTime = DN.L_StartTime.ToString(),
                                          L_EndTime = DN.L_EndTime.ToString(),
                                          L_Filesize = DN.L_Filesize.ToString(),
                                          L_Pages = (long)DN.L_Pages,
                                          L_Checksum = (long)DN.L_Checksum,
                                          IsDuplicate = DN.IsDuplicate
                                      }).Distinct().ToList();
                        }
                        #endregion
                        //
                        #region  C_A && L_NA
                        if (registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {

                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  where

                                  ((DN.C_ScannedFileName != null) && (DN.C_ScannedFileName != "") && (DN.L_ScannedFileName == null || DN.L_ScannedFileName == ""))
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                   && DN.DocumentTypeID == DocumentTypeIDFinal
                                   && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();
                    }
                    #endregion
                    //
                    #region IsDuplicate
                    if (registrationNoVerificationDetailsModel.IsDuplicate)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID into z
                                  from y in z.DefaultIfEmpty()


                                  where
                                   DN.IsDuplicate == true
                                   //Commented by tushar on 10 May 2023
                                  //&& ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  //&& (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      //BatchDateTime = DNB.BatchDateTime.ToString(),
                                      BatchDateTime = y.BatchDateTime.ToString(),

                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022

                                  }
                                  ).Distinct().OrderBy(a=>a.L_FRN).ThenBy(a=>a.SROCode).ToList();
                    }
                    #endregion
                    //Added By Tushar on 2 Nov 2022
                    #region Errortype
                    if (registrationNoVerificationDetailsModel.IsErrorTypecheck && registrationNoVerificationDetailsModel.ErrorCode > 0)
                    {

                            if (!registrationNoVerificationDetailsModel.IsDateErrorType_DateDetails)
                            {

                                //           Result = (from DN in dbContext.RPT_DocReg_NoCLDetails.AsEnumerable()
                                //                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                //                  on DN.BatchID equals DNB.BatchID


                                //           where DN.ErrorType.Split(',').Contains(ErrorTypetxt)
                                //          && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                //          && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                //          && DN.DocumentTypeID == DocumentTypeIDFinal
                                //          && DN.DocumentID >= DocumentIDFinal

                                //          select new RegistrationNoVerificationDetailsTableModel
                                //          {
                                //              DocumentID = DN.DocumentID,
                                //              SROCode = DN.SROCode,
                                //              C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                //              C_FRN = DN.C_FRN,
                                //              C_ScannedFileName = DN.C_ScannedFileName,
                                //              L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                //              L_FRN = DN.L_FRN,
                                //              L_ScannedFileName = DN.L_ScannedFileName,
                                //              BatchID = DN.BatchID,
                                //              C_CDNumber = DN.C_CDNumber,
                                //              L_CDNumber = DN.L_CDNumber,
                                //              ErrorType = DN.ErrorType,
                                //              DocumentTypeID = (int)DN.DocumentTypeID,
                                //              BatchDateTime = DNB.BatchDateTime.ToString(),

                                //              C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                //              L_ScanDate = DN.L_ScanDate.ToString(),
                                //                 //Added By Tushar on 29 Nov 2022
                                //              L_StartTime = DN.L_StartTime.ToString(),
                                //              L_EndTime = DN.L_EndTime.ToString(),
                                //              L_Filesize = DN.L_Filesize.ToString(),
                                //              L_Pages = (long)DN.L_Pages,
                                //              L_Checksum = (long)DN.L_Checksum,
                                //              IsDuplicate = DN.IsDuplicate,
                                //              //End By tushar on 29 Nov 2022

                                //              //Added By Rushikesh on 6 Feb 2023
                                //              //Local
                                //              L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                //              L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                //              L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                //              L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                //              L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                //              L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                //              L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                //              L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                //              L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                //              L_RefusalDate = DN.L_RefusalDate.ToString(),

                                //              //Central
                                //              C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                //              C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                //              C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                //              C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                //              C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                //              C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                //              C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                //              C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                //              C_RefusalDate = DN.C_RefusalDate.ToString(),
                                //              //End By Rushikesh on 6 Feb 2023

                                //}
                                //                                 ).Distinct().ToList();
                                //Commented and Added By Tushar on 14 March 2023 because query needs more time to execute
                                //Added by Tushar on 10 May 2023
                                if (ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.CentralandLocalScanDocumentNotPresent).ToString() || ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.CentralScanDocumentNotPresentLocalPresent).ToString())
                                {
                                    var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                                    var QueryResultForSpecificErrorType = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                                           join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                                                           join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                                                           from SF in SFJoin.DefaultIfEmpty()
                                                                           where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails
                                                                           && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude
                                                                           && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                                           && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                           && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                           && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                           && DN.DocumentID >= DocumentIDFinal
                                                                           select new


                                                                           {
                                                                               DocumentID = DN.DocumentID,
                                                                               SROCode = DN.SROCode,
                                                                               C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                                               C_FRN = DN.C_FRN,

                                                                               C_ScannedFileName = DN.C_ScannedFileName,
                                                                               L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                                               L_FRN = DN.L_FRN,
                                                                               L_ScannedFileName = DN.L_ScannedFileName,
                                                                               BatchID = DN.BatchID,
                                                                               C_CDNumber = DN.C_CDNumber,
                                                                               L_CDNumber = DN.L_CDNumber,
                                                                               ErrorType = DN.ErrorType,
                                                                               DocumentTypeID = (int)DN.DocumentTypeID,
                                                                               BatchDateTime = DNB.BatchDateTime,

                                                                               C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                                               L_ScanDate = DN.L_ScanDate.ToString(),
                                                                               //Added By Tushar on 29 Nov 2022
                                                                               L_StartTime = DN.L_StartTime,
                                                                               L_EndTime = DN.L_EndTime,
                                                                               L_Filesize = DN.L_Filesize,
                                                                               L_Pages = (long)DN.L_Pages,
                                                                               L_Checksum = (long)DN.L_Checksum,
                                                                               IsDuplicate = DN.IsDuplicate,
                                                                               //End By tushar on 29 Nov 2022

                                                                               //Added By Rushikesh on 6 Feb 2023

                                                                               //Local
                                                                               L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                                               L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                                               L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                                               L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                                               L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                                               L_PresentDateTime = DN.L_PresentDateTime,
                                                                               L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                                               L_DateOfStamp = DN.L_DateOfStamp,
                                                                               L_WithdrawalDate = DN.L_WithdrawalDate,
                                                                               L_RefusalDate = DN.L_RefusalDate,

                                                                               //Central
                                                                               C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                                               C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                                               C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                                               C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                                               C_PresentDateTime = DN.C_PresentDateTime,
                                                                               C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                                               C_DateOfStamp = DN.C_DateOfStamp,
                                                                               C_WithdrawalDate = DN.C_WithdrawalDate,
                                                                               C_RefusalDate = DN.C_RefusalDate,
                                                                           }).Distinct().ToList();


                                    //
                                    Result = QueryResultForSpecificErrorType.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                }
                                else if (ErrorTypetxt == Convert.ToInt16(DocumentVerificationErrorcode.MisMatch).ToString())
                                {
                                    //
                                    var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();

                                    var QueryResultForMisMatchErrorType = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                                           join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                                                           on DN.BatchID equals DNB.BatchID
                                                                           where
                                                                           ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                                           && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",")  // Exclude records with ErrorType containing ErrorTypetxtToExclude

                                                                           && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                           && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                           && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                           && DN.DocumentID >= DocumentIDFinal
                                                                           select new


                                                                           {
                                                                               DocumentID = DN.DocumentID,
                                                                               SROCode = DN.SROCode,
                                                                               C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                                               C_FRN = DN.C_FRN,

                                                                               C_ScannedFileName = DN.C_ScannedFileName,
                                                                               L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                                               L_FRN = DN.L_FRN,
                                                                               L_ScannedFileName = DN.L_ScannedFileName,
                                                                               BatchID = DN.BatchID,
                                                                               C_CDNumber = DN.C_CDNumber,
                                                                               L_CDNumber = DN.L_CDNumber,
                                                                               ErrorType = DN.ErrorType,
                                                                               DocumentTypeID = (int)DN.DocumentTypeID,
                                                                               BatchDateTime = DNB.BatchDateTime,

                                                                               C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                                               L_ScanDate = DN.L_ScanDate.ToString(),
                                                                               //Added By Tushar on 29 Nov 2022
                                                                               L_StartTime = DN.L_StartTime,
                                                                               L_EndTime = DN.L_EndTime,
                                                                               L_Filesize = DN.L_Filesize,
                                                                               L_Pages = (long)DN.L_Pages,
                                                                               L_Checksum = (long)DN.L_Checksum,
                                                                               IsDuplicate = DN.IsDuplicate,
                                                                               //End By tushar on 29 Nov 2022

                                                                               //Added By Rushikesh on 6 Feb 2023

                                                                               //Local
                                                                               L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                                               L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                                               L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                                               L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                                               L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                                               L_PresentDateTime = DN.L_PresentDateTime,
                                                                               L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                                               L_DateOfStamp = DN.L_DateOfStamp,
                                                                               L_WithdrawalDate = DN.L_WithdrawalDate,
                                                                               L_RefusalDate = DN.L_RefusalDate,

                                                                               //Central
                                                                               C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                                               C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                                               C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                                               C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                                               C_PresentDateTime = DN.C_PresentDateTime,
                                                                               C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                                               C_DateOfStamp = DN.C_DateOfStamp,
                                                                               C_WithdrawalDate = DN.C_WithdrawalDate,
                                                                               C_RefusalDate = DN.C_RefusalDate,
                                                                               //}).Distinct().ToList();
                                                                           });
                                    Result = QueryResultForMisMatchErrorType.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                    //
                                }
                                else
                                {
                                    //End By Tushar on 10 May 2023 
                                    var QueryResult = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                                       join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                                       on DN.BatchID equals DNB.BatchID
                                                       where
                                                        ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                                       && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                           && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                       && DN.DocumentTypeID == DocumentTypeIDFinal
                                                       && DN.DocumentID >= DocumentIDFinal
                                                       select new


                                                       {
                                                           DocumentID = DN.DocumentID,
                                                           SROCode = DN.SROCode,
                                                           C_Stamp5DateTime = DN.C_Stamp5DateTime,
                                                           C_FRN = DN.C_FRN,

                                                           C_ScannedFileName = DN.C_ScannedFileName,
                                                           L_Stamp5DateTime = DN.L_Stamp5DateTime,
                                                           L_FRN = DN.L_FRN,
                                                           L_ScannedFileName = DN.L_ScannedFileName,
                                                           BatchID = DN.BatchID,
                                                           C_CDNumber = DN.C_CDNumber,
                                                           L_CDNumber = DN.L_CDNumber,
                                                           ErrorType = DN.ErrorType,
                                                           DocumentTypeID = (int)DN.DocumentTypeID,
                                                           BatchDateTime = DNB.BatchDateTime,

                                                           C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime,
                                                           L_ScanDate = DN.L_ScanDate.ToString(),
                                                           //Added By Tushar on 29 Nov 2022
                                                           L_StartTime = DN.L_StartTime,
                                                           L_EndTime = DN.L_EndTime,
                                                           L_Filesize = DN.L_Filesize,
                                                           L_Pages = (long)DN.L_Pages,
                                                           L_Checksum = (long)DN.L_Checksum,
                                                           IsDuplicate = DN.IsDuplicate,
                                                           //End By tushar on 29 Nov 2022

                                                           //Added By Rushikesh on 6 Feb 2023

                                                           //Local
                                                           L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1,
                                                           L_Stamp1DateTime = DN.L_Stamp1DateTime,
                                                           L_Stamp2DateTime = DN.L_Stamp2DateTime,
                                                           L_Stamp3DateTime = DN.L_Stamp3DateTime,
                                                           L_Stamp4DateTime = DN.L_Stamp4DateTime,
                                                           L_PresentDateTime = DN.L_PresentDateTime,
                                                           L_ExecutionDateTime = DN.L_ExecutionDateTime,
                                                           L_DateOfStamp = DN.L_DateOfStamp,
                                                           L_WithdrawalDate = DN.L_WithdrawalDate,
                                                           L_RefusalDate = DN.L_RefusalDate,

                                                           //Central
                                                           C_Stamp1DateTime = DN.C_Stamp1DateTime,
                                                           C_Stamp2DateTime = DN.C_Stamp2DateTime,
                                                           C_Stamp3DateTime = DN.C_Stamp3DateTime,
                                                           C_Stamp4DateTime = DN.C_Stamp4DateTime,
                                                           C_PresentDateTime = DN.C_PresentDateTime,
                                                           C_ExecutionDateTime = DN.C_ExecutionDateTime,
                                                           C_DateOfStamp = DN.C_DateOfStamp,
                                                           C_WithdrawalDate = DN.C_WithdrawalDate,
                                                           C_RefusalDate = DN.C_RefusalDate,
                                                       }).Distinct().ToList();
                                    Result = QueryResult.Select(DN => new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DN.DocumentID,
                                        SROCode = DN.SROCode,
                                        C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                        C_FRN = DN.C_FRN,

                                        C_ScannedFileName = DN.C_ScannedFileName,
                                        L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                        L_FRN = DN.L_FRN,
                                        L_ScannedFileName = DN.L_ScannedFileName,
                                        BatchID = DN.BatchID,
                                        C_CDNumber = DN.C_CDNumber,
                                        L_CDNumber = DN.L_CDNumber,
                                        ErrorType = DN.ErrorType,
                                        DocumentTypeID = (int)DN.DocumentTypeID,
                                        BatchDateTime = DN.BatchDateTime.ToString(),

                                        C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                        L_ScanDate = DN.L_ScanDate.ToString(),
                                        //Added By Tushar on 29 Nov 2022
                                        L_StartTime = DN.L_StartTime.ToString(),
                                        L_EndTime = DN.L_EndTime.ToString(),
                                        L_Filesize = DN.L_Filesize.ToString(),
                                        L_Pages = (long)DN.L_Pages,
                                        L_Checksum = (long)DN.L_Checksum,
                                        IsDuplicate = DN.IsDuplicate,
                                        //End By tushar on 29 Nov 2022

                                        //Added By Rushikesh on 6 Feb 2023

                                        //Local
                                        L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                        L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                        L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                        L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                        L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                        L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                        L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                        L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                        L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                        L_RefusalDate = DN.L_RefusalDate.ToString(),

                                        //Central
                                        C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                        C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                        C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                        C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                        C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                        C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                        C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                        C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                        C_RefusalDate = DN.C_RefusalDate.ToString(),
                                    }).ToList();
                                    //End By Tushar on 14 March 2023
                                }
                            }
                            else
                            {
                                //Result= dbContext.RPT_DocReg_NoCLDetails.Join(dbContext.RPT_DocReg_DateDetails,s=>s.DocumentID,d=>d.DocumentID,(m,n)=> new { } )

                                //Result = dbContext.RPT_DocReg_DateDetails.Where(x => x.ErrorType.Contains(ErrorTypetxt) && x.SROCode == registrationNoVerificationDetailsModel.SROfficeID).Select(s => new RegistrationNoVerificationDetailsTableModel
                                //
                                Result = dbContext.RPT_DocReg_NoCLDetails.Join(dbContext.RPT_DocReg_DateDetails,
                                  t1 => t1.DocumentID, t2 => t2.DocumentID,
                                  (m, n) => new { t1 = m, t2 = n })
                                     .Where(x => x.t1.DocumentTypeID == registrationNoVerificationDetailsModel.DocumentTypeId && x.t2.ErrorType.Contains(ErrorTypetxt)
                                                                               && ((x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                               && (x.t1.C_Stamp5DateTime == null ? x.t1.L_Stamp5DateTime : x.t1.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                               && x.t1.DocumentTypeID == DocumentTypeIDFinal
                                                                               && x.t1.DocumentID >= DocumentIDFinal)

                    .Select(x => new RegistrationNoVerificationDetailsTableModel
                    {
                        DocumentID = x.t2.DocumentID,
                        SROCode = x.t2.SROCode,
                        TableName = x.t2.TableName,
                        ReceiptID = x.t2.ReceiptID,
                        L_DateOfReceipt = x.t2.L_DateOfReceipt.ToString(),
                        C_DateOfReceipt = x.t2.C_DateOfReceipt.ToString(),
                        StampDetailsID = x.t2.StampDetailsID,
                        L_DateOfStamp = x.t2.L_DateOfStamp.ToString(),
                        C_DateOfStamp = x.t2.C_DateOfStamp.ToString(),
                        L_DDChalDate = x.t2.L_DDChalDate.ToString(),
                        C_DDChalDate = x.t2.C_DDChalDate.ToString(),
                        L_StampPaymentDate = x.t2.L_StampPaymentDate.ToString(),
                        C_StampPaymentDate = x.t2.C_StampPaymentDate.ToString(),
                        L_DateOfReturn = x.t2.L_DateOfReturn.ToString(),
                        C_DateOfReturn = x.t2.C_DateOfReturn.ToString(),
                        PartyID = x.t2.PartyID,
                        L_AdmissionDate = x.t2.L_AdmissionDate.ToString(),
                        C_AdmissionDate = x.t2.C_AdmissionDate.ToString(),
                        BatchID = x.t2.BatchID,
                        ErrorType = x.t2.ErrorType
                    })
                   .ToList();
                                /*
                                Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                    join DNB in dbContext.RPT_DocReg_DateDetails on new { p1=DN.DocumentID,p2=DN.SROCode}  equals {p1= DNB. }
                                    where DN.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                                                                              && DNB.ErrorType.Contains(ErrorTypetxt)
                                                                              && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                                                              && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                                                              && DN.DocumentTypeID == DocumentTypeIDFinal
                                                                              && DN.DocumentID >= DocumentIDFinal
                                    select new RegistrationNoVerificationDetailsTableModel
                                    {
                                        DocumentID = DNB.DocumentID,
                                        SROCode = DNB.SROCode,
                                        TableName = DNB.TableName,
                                        ReceiptID = DNB.ReceiptID,
                                        L_DateOfReceipt = DNB.L_DateOfReceipt.ToString(),
                                        C_DateOfReceipt = DNB.C_DateOfReceipt.ToString(),
                                        StampDetailsID = DNB.StampDetailsID,
                                        L_DateOfStamp = DNB.L_DateOfStamp.ToString(),
                                        C_DateOfStamp = DNB.C_DateOfStamp.ToString(),
                                        L_DDChalDate = DNB.L_DDChalDate.ToString(),
                                        C_DDChalDate = DNB.C_DDChalDate.ToString(),
                                        L_StampPaymentDate = DNB.L_StampPaymentDate.ToString(),
                                        C_StampPaymentDate = DNB.C_StampPaymentDate.ToString(),
                                        L_DateOfReturn = DNB.L_DateOfReturn.ToString(),
                                        C_DateOfReturn = DNB.C_DateOfReturn.ToString(),
                                        PartyID = DNB.PartyID,
                                        L_AdmissionDate = DNB.L_AdmissionDate.ToString(),
                                        C_AdmissionDate = DNB.C_AdmissionDate.ToString(),
                                        BatchID = DNB.BatchID,
                                        ErrorType = DNB.ErrorType

                                    }
                                    ).ToList();
                                        */
                            }

                    }
                    #endregion
                    //End By Tushar on 2 Nov 2022
                    #region FRN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == true && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  where
                                  (DN.C_FRN != DN.L_FRN)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                   && DN.DocumentTypeID == DocumentTypeIDFinal
                                   && DN.DocumentID >= DocumentIDFinal

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                    #region SFN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == true && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  where
                                  (DN.C_ScannedFileName != DN.L_ScannedFileName)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                   && DN.DocumentTypeID == DocumentTypeIDFinal
                                  //|| (DN.C_FRN == null)
                                  //|| (DN.L_FRN == null)
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                    #region FRN and SFN Mismatch
                    if (registrationNoVerificationDetailsModel.IsFRNCheck == true && registrationNoVerificationDetailsModel.IsSFNCheck == true && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  where
                                  (DN.C_FRN != DN.L_FRN)
                                  && (DN.C_ScannedFileName != DN.L_ScannedFileName)
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                   && DN.DocumentTypeID == DocumentTypeIDFinal
                                  //|| (DN.C_FRN == null)
                                  //|| (DN.L_FRN == null)
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                    #region SROCODE == 0 && DocumentType == Document (SROCODE == 0 for All)
                    if ((registrationNoVerificationDetailsModel.DocumentTypeId == Convert.ToInt64(Common.ApiCommonEnum.DocumentType.Document)) && registrationNoVerificationDetailsModel.IsDateNull == false && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false && registrationNoVerificationDetailsModel.IsDateErrorType_DateDetails == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  //where  (DN.C_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.C_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //  && (Convert.ToDateTime(DN.C_Stamp5DateTime).Date > registrationNoVerificationDetailsModel.DateTime_Date.Date && Convert.ToDateTime(DN.C_Stamp5DateTime).Date < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)


                                  //
                                  where DN.DocumentTypeID == 1 &&
                                  ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date &&
                                  (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate,
                                      //End By tushar on 29 Nov 2022

                                      //Added by Rushikesh 9 Feb 2023
                                      //Local
                                      L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                      L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                      L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                      L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                      L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                      L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                      L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                      L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                      L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                      L_RefusalDate = DN.L_RefusalDate.ToString(),

                                      //Central
                                      C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                      C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                      C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                      C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                      C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                      C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                      C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                      C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                      C_RefusalDate = DN.C_RefusalDate.ToString(),
                                      
                                      //End by Rushikesh 9 Feb 2023

                                  }
                                                        ).Distinct().ToList(); 
                    }
                    #endregion

                    #region SROCODE == 0 && DocumentType == Marriage (SROCODE == 0 for All)
                    if ((registrationNoVerificationDetailsModel.DocumentTypeId == Convert.ToInt64(Common.ApiCommonEnum.DocumentType.Marriage)) && registrationNoVerificationDetailsModel.IsDateNull == false && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                                  //where  (DN.C_Stamp5DateTime > registrationNoVerificationDetailsModel.DateTime_Date.Date && DN.C_Stamp5DateTime < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)
                                  //  && (Convert.ToDateTime(DN.C_Stamp5DateTime).Date > registrationNoVerificationDetailsModel.DateTime_Date.Date && Convert.ToDateTime(DN.C_Stamp5DateTime).Date < registrationNoVerificationDetailsModel.DateTime_ToDate.Date)


                                  //
                                  where DN.DocumentTypeID == 2 &&
                                  ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= registrationNoVerificationDetailsModel.DateTime_Date.Date &&
                                  (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                                  && DN.DocumentID >= DocumentIDFinal
                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate,
                                      //End By tushar on 29 Nov 2022

                                      //Added by Rushikesh 9 Feb 2023
                                      //Local
                                      L_Stamp5DateTime_1 = DN.L_Stamp5DateTime_1.ToString(),
                                      L_Stamp1DateTime = DN.L_Stamp1DateTime.ToString(),
                                      L_Stamp2DateTime = DN.L_Stamp2DateTime.ToString(),
                                      L_Stamp3DateTime = DN.L_Stamp3DateTime.ToString(),
                                      L_Stamp4DateTime = DN.L_Stamp4DateTime.ToString(),
                                      L_PresentDateTime = DN.L_PresentDateTime.ToString(),
                                      L_ExecutionDateTime = DN.L_ExecutionDateTime.ToString(),
                                      L_DateOfStamp = DN.L_DateOfStamp.ToString(),
                                      L_WithdrawalDate = DN.L_WithdrawalDate.ToString(),
                                      L_RefusalDate = DN.L_RefusalDate.ToString(),

                                      //Central
                                      C_Stamp1DateTime = DN.C_Stamp1DateTime.ToString(),
                                      C_Stamp2DateTime = DN.C_Stamp2DateTime.ToString(),
                                      C_Stamp3DateTime = DN.C_Stamp3DateTime.ToString(),
                                      C_Stamp4DateTime = DN.C_Stamp4DateTime.ToString(),
                                      C_PresentDateTime = DN.C_PresentDateTime.ToString(),
                                      C_ExecutionDateTime = DN.C_ExecutionDateTime.ToString(),
                                      C_DateOfStamp = DN.C_DateOfStamp.ToString(),
                                      C_WithdrawalDate = DN.C_WithdrawalDate.ToString(),
                                      C_RefusalDate = DN.C_RefusalDate.ToString(),

                                      //End by Rushikesh 9 Feb 2023
                                  }
                                                                   ).Distinct().ToList(); 
                    }
                    #endregion

                    #region Both Date NULL
                    if (registrationNoVerificationDetailsModel.IsDateNull && registrationNoVerificationDetailsModel.IsFRNCheck == false && registrationNoVerificationDetailsModel.IsSFNCheck == false && registrationNoVerificationDetailsModel.IsFileNA == false && registrationNoVerificationDetailsModel.IsCNull == false && registrationNoVerificationDetailsModel.IsLNull == false && registrationNoVerificationDetailsModel.IsErrorTypecheck == false && registrationNoVerificationDetailsModel.IsDuplicate == false)
                    {
                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails
                                  on DN.BatchID equals DNB.BatchID

                               
                                  where 
                                  (DN.C_Stamp5DateTime == null)
                                  && (DN.L_Stamp5DateTime == null)

                                  //

                                  select new RegistrationNoVerificationDetailsTableModel
                                  {
                                      DocumentID = DN.DocumentID,
                                      SROCode = DN.SROCode,
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                                      BatchID = DN.BatchID,
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      ErrorType = DN.ErrorType,
                                      DocumentTypeID = (int)DN.DocumentTypeID,
                                      BatchDateTime = DNB.BatchDateTime.ToString(),
                                      //Added By Tushar on 14 Oct 2022
                                      C_ScanFileUploadDateTime = DN.C_ScanFileUploadDateTime.ToString(),
                                      L_ScanDate = DN.L_ScanDate.ToString(),
                                      //End By Tushar on 14 Oct 2022
                                      //Added By Tushar on 29 Nov 2022
                                      L_StartTime = DN.L_StartTime.ToString(),
                                      L_EndTime = DN.L_EndTime.ToString(),
                                      L_Filesize = DN.L_Filesize.ToString(),
                                      L_Pages = (long)DN.L_Pages,
                                      L_Checksum = (long)DN.L_Checksum,
                                      IsDuplicate = DN.IsDuplicate
                                      //End By tushar on 29 Nov 2022
                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                }
             }

				//Added By Tushar on 3 Jan 2023
                if (registrationNoVerificationDetailsModel.IsPropertyAreaDetailsErrorType && registrationNoVerificationDetailsModel.ErrorCode != 0)
                {
                    foreach (var item in Result)
                    {
                        resModel = new RegistrationNoVerificationDetailsTableModel();

                        resModel.srNo = SrCount++;
                        resModel.PropertyID = item.PropertyID;
                        resModel.DocumentID = item.DocumentID == null ? 0 : item.DocumentID;
                        resModel.SROCode = item.SROCode;
                        resModel.VillageCode = item.VillageCode == null ? 0 : item.VillageCode;
                        resModel.TotalArea = item.TotalArea == null ? 0 : item.TotalArea;
                        resModel.MeasurementUnit = item.MeasurementUnit == null ? 0 : item.MeasurementUnit;
                        resModel.C_PropertyID = item.C_PropertyID == null ? 0 : item.C_PropertyID;
                        resModel.C_SROCode = item.C_SROCode == null ? 0 : item.C_SROCode;
                        resModel.C_VillageCode = item.C_VillageCode == null ? 0 : item.C_VillageCode;
                        resModel.C_TotalArea = item.C_TotalArea == null ? 0 : item.C_TotalArea;
                        resModel.C_DocumentID = item.C_DocumentID == null ? 0 : item.C_DocumentID;
                        resModel.C_MeasurementUnit = item.C_MeasurementUnit == null ? 0 : item.C_MeasurementUnit;
                        resModel.BatchID = item.BatchID == null ? 0 : item.BatchID;
                        resModel.ErrorType = item.ErrorType;

                        RegistrationNoVerificationDetailsList.Add(resModel);
                    }
                }

                //Added by Rushikesh 6 Feb 2023
                else if (registrationNoVerificationDetailsModel.IsDateDetailsErrorType && registrationNoVerificationDetailsModel.ErrorCode != 0)
                {
                   
                    foreach (var item in Result)
                    {
                        resModel = new RegistrationNoVerificationDetailsTableModel();
                        resModel.srNo = SrCount++;
                        resModel.PropertyID = item.PropertyID;
                        resModel.DocumentID = item.DocumentID == null ? 0 : item.DocumentID;
                        resModel.SROCode = item.SROCode;
                        resModel.VillageCode = item.VillageCode == null ? 0 : item.VillageCode;
                        resModel.TotalArea = item.TotalArea == null ? 0 : item.TotalArea;
                        resModel.MeasurementUnit = item.MeasurementUnit == null ? 0 : item.MeasurementUnit;
                        resModel.C_PropertyID = item.C_PropertyID == null ? 0 : item.C_PropertyID;
                        resModel.C_SROCode = item.C_SROCode == null ? 0 : item.C_SROCode;
                        resModel.C_VillageCode = item.C_VillageCode == null ? 0 : item.C_VillageCode;
                        resModel.C_TotalArea = item.C_TotalArea == null ? 0 : item.C_TotalArea;
                        resModel.C_DocumentID = item.C_DocumentID == null ? 0 : item.C_DocumentID;
                        resModel.C_MeasurementUnit = item.C_MeasurementUnit == null ? 0 : item.C_MeasurementUnit;
                        resModel.BatchID = item.BatchID == null ? 0 : item.BatchID;
                        resModel.ErrorType = item.ErrorType;

                        resModel.L_Stamp5DateTime_1 = String.IsNullOrEmpty(item.L_Stamp5DateTime_1) ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime_1).ToString("dd/MM/yyyy  HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                        resModel.L_Stamp1DateTime = String.IsNullOrEmpty(item.L_Stamp1DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp1DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp2DateTime = String.IsNullOrEmpty(item.L_Stamp2DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp2DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp3DateTime = String.IsNullOrEmpty(item.L_Stamp3DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp3DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp4DateTime = String.IsNullOrEmpty(item.L_Stamp4DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp4DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_PresentDateTime = String.IsNullOrEmpty(item.L_PresentDateTime) ? "--" : Convert.ToDateTime(item.L_PresentDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_ExecutionDateTime = String.IsNullOrEmpty(item.L_ExecutionDateTime) ? "--" : Convert.ToDateTime(item.L_ExecutionDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_DateOfStamp = String.IsNullOrEmpty(item.L_DateOfStamp) ? "--" : Convert.ToDateTime(item.L_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_WithdrawalDate = String.IsNullOrEmpty(item.L_WithdrawalDate) ? "--" : Convert.ToDateTime(item.L_WithdrawalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_RefusalDate = String.IsNullOrEmpty(item.L_RefusalDate) ? "--" : Convert.ToDateTime(item.L_RefusalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp1DateTime = String.IsNullOrEmpty(item.C_Stamp1DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp1DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp2DateTime = String.IsNullOrEmpty(item.C_Stamp2DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp2DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp3DateTime = String.IsNullOrEmpty(item.C_Stamp3DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp3DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp4DateTime = String.IsNullOrEmpty(item.C_Stamp4DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp4DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_PresentDateTime = String.IsNullOrEmpty(item.C_PresentDateTime) ? "--" : Convert.ToDateTime(item.C_PresentDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_ExecutionDateTime = String.IsNullOrEmpty(item.C_ExecutionDateTime) ? "--" : Convert.ToDateTime(item.C_ExecutionDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DateOfStamp = String.IsNullOrEmpty(item.C_DateOfStamp) ? "--" : Convert.ToDateTime(item.C_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_WithdrawalDate = String.IsNullOrEmpty(item.C_WithdrawalDate) ? "--" : Convert.ToDateTime(item.C_WithdrawalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_RefusalDate = String.IsNullOrEmpty(item.C_RefusalDate) ? "--" : Convert.ToDateTime(item.C_RefusalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        //extras default fields from Js
                        resModel.C_Stamp5DateTime = String.IsNullOrEmpty(item.C_Stamp5DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp5DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_FRN = String.IsNullOrEmpty(item.C_FRN) ? "--" : item.C_FRN;
                        resModel.C_ScannedFileName = String.IsNullOrEmpty(item.C_ScannedFileName) ? "--" : item.C_ScannedFileName;
                        resModel.C_CDNumber = String.IsNullOrEmpty(item.C_CDNumber) ? "--" : item.C_CDNumber;
                        resModel.C_ScanFileUploadDateTime = String.IsNullOrEmpty(item.C_ScanFileUploadDateTime) ? "--" : Convert.ToDateTime(item.C_ScanFileUploadDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp5DateTime = String.IsNullOrEmpty(item.L_Stamp5DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.DocumentTypeID = item.DocumentTypeID;
                        resModel.BatchDateTime = String.IsNullOrEmpty(item.BatchDateTime) ? "--" : Convert.ToDateTime(item.BatchDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_ScannedFileName = String.IsNullOrEmpty(item.C_ScannedFileName) ? "--" : item.C_ScannedFileName;
                        resModel.L_FRN = String.IsNullOrEmpty(item.L_FRN) ? "--" : item.L_FRN;
                        resModel.L_ScannedFileName = String.IsNullOrEmpty(item.L_ScannedFileName) ? "--" : item.L_ScannedFileName;
                        resModel.L_CDNumber = String.IsNullOrEmpty(item.L_CDNumber) ? "--" : item.L_CDNumber;
                        resModel.L_ScanDate = String.IsNullOrEmpty(item.L_ScanDate) ? "--" : Convert.ToDateTime(item.L_ScanDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_StartTime = String.IsNullOrEmpty(item.L_StartTime) ? "--" : Convert.ToDateTime(item.L_StartTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_EndTime = String.IsNullOrEmpty(item.L_EndTime) ? "--" :  Convert.ToDateTime(item.L_EndTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Filesize = String.IsNullOrEmpty(item.L_Filesize) ? "--" : item.L_Filesize;
                        resModel.L_Pages = item.L_Pages == null ? 0 : item.L_Pages;
                        resModel.L_Checksum = item.L_Checksum == null ? 0 : item.L_Checksum;
                        //resModel.L_Stamp5DateTime = String.IsNullOrEmpty(item.L_Stamp5DateTime) ? "--" : item.L_Stamp5DateTime; Convert.ToDateTime(item.L_Stamp1DateTime).ToString("dd/MM/yyyy  HH:mm:ss");

                        RegistrationNoVerificationDetailsList.Add(resModel);
                    }
                }
                // End by Rushikesh 6 Feb 2023

                //Added by Rushikesh 9 Feb 2023
                else if (registrationNoVerificationDetailsModel.IsDateErrorType_DateDetails && registrationNoVerificationDetailsModel.ErrorCode != 0)
                {

                    foreach (var item in Result)
                    {
                        resModel = new RegistrationNoVerificationDetailsTableModel();
                        resModel.srNo = SrCount++;
                        resModel.DocumentID = item.DocumentID == null ? 0 : item.DocumentID;
                        resModel.SROCode = item.SROCode;
                        resModel.TableName = String.IsNullOrEmpty(item.TableName) ? "--" : item.TableName;
                        resModel.ReceiptID = item.ReceiptID == null ? 0 : item.ReceiptID;
                        resModel.L_DateOfReceipt = String.IsNullOrEmpty(item.L_DateOfReceipt)? "--" : Convert.ToDateTime(item.L_DateOfReceipt).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DateOfReceipt = String.IsNullOrEmpty(item.C_DateOfReceipt)? "--" : Convert.ToDateTime(item.C_DateOfReceipt).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.StampDetailsID = item.StampDetailsID == null ? 0 : item.StampDetailsID;
                        resModel.L_DateOfStamp = String.IsNullOrEmpty(item.L_DateOfStamp) ? "--" : Convert.ToDateTime(item.L_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DateOfStamp = String.IsNullOrEmpty(item.C_DateOfStamp) ? "--" : Convert.ToDateTime(item.C_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_DDChalDate = String.IsNullOrEmpty(item.L_DDChalDate) ? "--" : Convert.ToDateTime(item.L_DDChalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DDChalDate = String.IsNullOrEmpty(item.C_DDChalDate) ? "--" : Convert.ToDateTime(item.C_DDChalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_StampPaymentDate = String.IsNullOrEmpty(item.L_StampPaymentDate) ? "--" : Convert.ToDateTime(item.L_StampPaymentDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_StampPaymentDate = String.IsNullOrEmpty(item.C_StampPaymentDate) ? "--" : Convert.ToDateTime(item.C_StampPaymentDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_DateOfReturn = String.IsNullOrEmpty(item.L_DateOfReturn) ? "--" : Convert.ToDateTime(item.L_DateOfReturn).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DateOfReturn = String.IsNullOrEmpty(item.C_DateOfReturn) ? "--" : Convert.ToDateTime(item.C_DateOfReturn).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.PartyID = item.PartyID == null ? 0 : item.PartyID; 
                        resModel.L_AdmissionDate = String.IsNullOrEmpty(item.L_AdmissionDate) ? "--" : Convert.ToDateTime(item.L_AdmissionDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_AdmissionDate = String.IsNullOrEmpty(item.C_AdmissionDate) ? "--" : Convert.ToDateTime(item.C_AdmissionDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.BatchID = item.BatchID == null ? 0 : item.BatchID;
                        resModel.ErrorType = String.IsNullOrEmpty(item.ErrorType) ? "--" : item.ErrorType;

                        RegistrationNoVerificationDetailsList.Add(resModel);
                    }
                }
                // End by Rushikesh 9 Feb 2023
                else
                {
				//End By Tushar on 3 Jan 2023
				   foreach (var item in Result)
                    {
                    resModel = new RegistrationNoVerificationDetailsTableModel();

                    resModel.srNo = SrCount++;
                    resModel.DocumentID = item.DocumentID;
                    resModel.SROCode = item.SROCode;
        
                    resModel.C_Stamp5DateTime = item.C_Stamp5DateTime == "" ? "--" : Convert.ToDateTime(item.C_Stamp5DateTime).ToString("dd/MM/yyyy");
                    resModel.C_FRN = item.C_FRN ?? "--";
                    resModel.C_ScannedFileName = item.C_ScannedFileName ?? "--";
           
                    resModel.L_Stamp5DateTime = item.L_Stamp5DateTime == "" ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime).ToString("dd/MM/yyyy");
         
                    resModel.L_FRN = item.L_FRN ?? "--"; 
                    resModel.L_ScannedFileName = item.L_ScannedFileName ?? "--";
                    //resModel.BatchID = item.BatchID;
                    resModel.BatchID = item.BatchID == null ? 0 :item.BatchID;
                    resModel.C_CDNumber = item.C_CDNumber ?? "--";
                    resModel.L_CDNumber = item.L_CDNumber ?? "--";
                    resModel.ErrorType = item.ErrorType ?? "--";
                    resModel.DocumentTypeID =Convert.ToInt32(item.DocumentTypeID);
                    resModel.BatchDateTime = item.BatchDateTime == "" ? "--" : Convert.ToDateTime(item.BatchDateTime).ToString("dd/MM/yyyy");
                    //Added By Tushar on 14 Oct 2022
                    resModel.C_ScanFileUploadDateTime = item.C_ScanFileUploadDateTime == "" ? "--" : Convert.ToDateTime(item.C_ScanFileUploadDateTime).ToString("dd/MM/yyyy");
                    resModel.L_ScanDate = item.L_ScanDate == "" ? "--" : Convert.ToDateTime(item.L_ScanDate).ToString("dd/MM/yyyy");
                    //End By Tushar on 14 Oct 2022
                    //Added By Tushar on 29 Nov 2022
                    resModel.L_StartTime =string.IsNullOrEmpty(item.L_StartTime) ? "--" : Convert.ToDateTime(item.L_StartTime).ToString();
                    resModel.L_EndTime =string.IsNullOrEmpty(item.L_EndTime) ? "--" : Convert.ToDateTime(item.L_EndTime).ToString();
                    resModel.L_Filesize =string.IsNullOrEmpty(item.L_Filesize) ? "--" :item.L_Filesize;
                    resModel.L_Pages =item.L_Pages == null ? 0 :item.L_Pages;
                    resModel.L_Checksum = item.L_Checksum;
                    resModel.IsDuplicate = item.IsDuplicate == null ? false :Convert.ToBoolean(item.IsDuplicate);
                        resModel.L_DateOfStamp = String.IsNullOrEmpty(item.L_DateOfStamp) ? "--" : item.L_DateOfStamp;

                        //End By Tushar on 29 Nov 2022

                        //Added by Rushikesh 9 feb 2023
                        resModel.L_Stamp5DateTime_1 = String.IsNullOrEmpty(item.L_Stamp5DateTime_1) ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime_1).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp1DateTime = String.IsNullOrEmpty(item.L_Stamp1DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp1DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp2DateTime = String.IsNullOrEmpty(item.L_Stamp2DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp2DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp3DateTime = String.IsNullOrEmpty(item.L_Stamp3DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp3DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp4DateTime = String.IsNullOrEmpty(item.L_Stamp4DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp4DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_PresentDateTime = String.IsNullOrEmpty(item.L_PresentDateTime) ? "--" : Convert.ToDateTime(item.L_PresentDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_ExecutionDateTime = String.IsNullOrEmpty(item.L_ExecutionDateTime) ? "--" : Convert.ToDateTime(item.L_ExecutionDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_DateOfStamp = String.IsNullOrEmpty(item.L_DateOfStamp) ? "--" : Convert.ToDateTime(item.L_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_WithdrawalDate = String.IsNullOrEmpty(item.L_WithdrawalDate) ? "--" : Convert.ToDateTime(item.L_WithdrawalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_RefusalDate = String.IsNullOrEmpty(item.L_RefusalDate) ? "--" : Convert.ToDateTime(item.L_RefusalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp1DateTime = String.IsNullOrEmpty(item.C_Stamp1DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp1DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp2DateTime = String.IsNullOrEmpty(item.C_Stamp2DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp2DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp3DateTime = String.IsNullOrEmpty(item.C_Stamp3DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp3DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_Stamp4DateTime = String.IsNullOrEmpty(item.C_Stamp4DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp4DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_PresentDateTime = String.IsNullOrEmpty(item.C_PresentDateTime) ? "--" : Convert.ToDateTime(item.C_PresentDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_ExecutionDateTime = String.IsNullOrEmpty(item.C_ExecutionDateTime) ? "--" : Convert.ToDateTime(item.C_ExecutionDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_DateOfStamp = String.IsNullOrEmpty(item.C_DateOfStamp) ? "--" : Convert.ToDateTime(item.C_DateOfStamp).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_WithdrawalDate = String.IsNullOrEmpty(item.C_WithdrawalDate) ? "--" : Convert.ToDateTime(item.C_WithdrawalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_RefusalDate = String.IsNullOrEmpty(item.C_RefusalDate) ? "--" : Convert.ToDateTime(item.C_RefusalDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        //extras default fields from Js
                        resModel.C_Stamp5DateTime = String.IsNullOrEmpty(item.C_Stamp5DateTime) ? "--" : Convert.ToDateTime(item.C_Stamp5DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_FRN = String.IsNullOrEmpty(item.C_FRN) ? "--" : item.C_FRN;
                        resModel.C_ScannedFileName = String.IsNullOrEmpty(item.C_ScannedFileName) ? "--" : item.C_ScannedFileName;
                        resModel.C_CDNumber = String.IsNullOrEmpty(item.C_CDNumber) ? "--" : item.C_CDNumber;
                        resModel.C_ScanFileUploadDateTime = String.IsNullOrEmpty(item.C_ScanFileUploadDateTime) ? "--" : Convert.ToDateTime(item.C_ScanFileUploadDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Stamp5DateTime = String.IsNullOrEmpty(item.L_Stamp5DateTime) ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.DocumentTypeID = item.DocumentTypeID;
                        resModel.BatchDateTime = String.IsNullOrEmpty(item.BatchDateTime) ? "--" : Convert.ToDateTime(item.BatchDateTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.C_ScannedFileName = String.IsNullOrEmpty(item.C_ScannedFileName) ? "--" : item.C_ScannedFileName;
                        resModel.L_FRN = String.IsNullOrEmpty(item.L_FRN) ? "--" : item.L_FRN;
                        resModel.L_ScannedFileName = String.IsNullOrEmpty(item.L_ScannedFileName) ? "--" : item.L_ScannedFileName;
                        resModel.L_CDNumber = String.IsNullOrEmpty(item.L_CDNumber) ? "--" : item.L_CDNumber;
                        resModel.L_ScanDate = String.IsNullOrEmpty(item.L_ScanDate) ? "--" : Convert.ToDateTime(item.L_ScanDate).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_StartTime = String.IsNullOrEmpty(item.L_StartTime) ? "--" : Convert.ToDateTime(item.L_StartTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_EndTime = String.IsNullOrEmpty(item.L_EndTime) ? "--" : Convert.ToDateTime(item.L_EndTime).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.L_Filesize = String.IsNullOrEmpty(item.L_Filesize) ? "--" : item.L_Filesize;
                        resModel.L_Pages = item.L_Pages == null ? 0 : item.L_Pages;
                        resModel.L_Checksum = item.L_Checksum == null ? 0 : item.L_Checksum;
                        //End  by rushikesh 9 feb 2023

                        RegistrationNoVerificationDetailsList.Add(resModel);
		       }
                }
            }
            catch (Exception ex)
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
            return RegistrationNoVerificationDetailsList;
        }


        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
        public string RefreshPropertyAreaDetailsDetails(int SROfficeID)
        {
            try
            {
                if (SROfficeID != 0)
                {
                    dbContext = new KaveriEntities();

                    var QueryRPT_PropertyAreaDetails = @"DELETE FROM RPT_PropertyAreaDetails
                              WHERE SROCode=" + SROfficeID;

                    using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(QueryRPT_PropertyAreaDetails, connection))
                        {                            
                            connection.Open();
                            command.CommandText = QueryRPT_PropertyAreaDetails;
                            command.CommandTimeout = 120;
                            command.ExecuteNonQuery();
                            command.Dispose();
                        }
                        connection.Close();
                    }
                    
                    return "Refresh Completed with Success for Registration No Verification Details.";
                }
                else
                {
                    return "Please select SRO Name rather than All.";
                }
            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex, "RefreshPropertyAreaDetailsDetails");
                return null;
                throw ex;
            }
            
        }
        //Ended by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 




        public RegistrationNoVerificationDetailsTableModel GetDocRegNoCLBatchDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            //List<RPT_DocReg_NoCLBatchDetailsTable> GetDocRegNoCLBatchDetailsList = new List<RPT_DocReg_NoCLBatchDetailsTable>();
            RPT_DocReg_NoCLBatchDetailsTable resModel = null;
            //
            RegistrationNoVerificationDetailsTableModel resultModel = new RegistrationNoVerificationDetailsTableModel();
            resultModel.RPT_DocReg_NoCLBatchDetailsExcelSheet = new List<RPT_DocReg_NoCLBatchDetailsTable>();
            long SrCount = 1;
            var ToDateFinal = registrationNoVerificationDetailsModel.DateTime_ToDate.AddDays(1).Date;
            //
            dbContext = new KaveriEntities();
            var Result = new List<RPT_DocReg_NoCLBatchDetailsTable>();

            if (registrationNoVerificationDetailsModel.SROfficeID != 0)
            {
                 Result = dbContext.RPT_DocReg_NoCLBatchDetails.Where(x => x.SROCode == registrationNoVerificationDetailsModel.SROfficeID
                && (x.BatchDateTime >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                                && x.BatchDateTime <= ToDateFinal))
                .Select(c => new RPT_DocReg_NoCLBatchDetailsTable
                {
                    BatchID = c.BatchID,
                    SROCode = c.SROCode,
                    FromDocumentID = c.FromDocumentID,
                    ToDocumentID = c.ToDocumentID,
                    DocumentTypeID = c.DocumentTypeID,
                    BatchDateTime = c.BatchDateTime.ToString(),
                    IsVerified = c.IsVerified,
                    IsMismatchFound = c.IsMismatchFound
                }
               ).ToList();
            }
            else
            {
                //Commented and Added By Tushar on 3 Nov 2022 
                //Result = dbContext.RPT_DocReg_NoCLBatchDetails.Where(x =>(x.BatchDateTime >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                //             && x.BatchDateTime <= registrationNoVerificationDetailsModel.DateTime_ToDate.Date))
                
                Result = dbContext.RPT_DocReg_NoCLBatchDetails.Where(x => (x.BatchDateTime >= registrationNoVerificationDetailsModel.DateTime_Date.Date
                        && x.BatchDateTime <= ToDateFinal))
        .Select(c => new RPT_DocReg_NoCLBatchDetailsTable
             {
                 BatchID = c.BatchID,
                 SROCode = c.SROCode,
                 FromDocumentID = c.FromDocumentID,
                 ToDocumentID = c.ToDocumentID,
                 DocumentTypeID = c.DocumentTypeID,
                 BatchDateTime = c.BatchDateTime.ToString(),
                 IsVerified = c.IsVerified,
                 IsMismatchFound = c.IsMismatchFound
             }
            ).ToList();
            }
            if(Result != null)
            {
                foreach (var item in Result)
                {
                   
                    resModel = new RPT_DocReg_NoCLBatchDetailsTable();
                    resModel.srNo = SrCount++;
                    resModel.BatchID = item.BatchID ;
                    //resModel.SROCode = item.SROCode ==  null ? 0 : Convert.ToInt32(item.SROCode);
                    //resModel.FromDocumentID = item.FromDocumentID ==  null ? 0 : item.FromDocumentID;
                    //resModel.ToDocumentID = item.ToDocumentID ==  null ? 0: item.ToDocumentID;
                    //resModel.DocumentTypeID = item.DocumentTypeID ==  null ? 0 : item.DocumentTypeID;
                    //resModel.BatchDateTime = item.BatchDateTime == "" ? "--" : Convert.ToDateTime(item.BatchDateTime).ToString("dd/MM/yyyy");
                    //resModel.IsVerified = item.IsVerified == null ? false : item.IsVerified;
                    //resModel.IsMismatchFound = item.IsMismatchFound == null ? false : item.IsMismatchFound;
                    //
                    resModel.SROCode = item.SROCode;
                    resModel.FromDocumentID = item.FromDocumentID;
                    resModel.ToDocumentID = item.ToDocumentID;
                    resModel.DocumentTypeID = item.DocumentTypeID;
                    resModel.BatchDateTime = item.BatchDateTime == "" ? "--" : Convert.ToDateTime(item.BatchDateTime).ToString("dd/MM/yyyy");
                    resModel.IsVerified = item.IsVerified;
                    resModel.IsMismatchFound = item.IsMismatchFound;
                    //


                    resultModel.RPT_DocReg_NoCLBatchDetailsExcelSheet.Add(resModel);


                }
            }
            return resultModel;
        }

        //Added By Tushar on 7 Nov 2022
        public RegistrationNoVerificationDetailsTableModel GetScannedFileDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            //List<RPT_DocReg_NoCLBatchDetailsTable> GetDocRegNoCLBatchDetailsList = new List<RPT_DocReg_NoCLBatchDetailsTable>();
            ScannedFileDetails resModel = null;
            //
            RegistrationNoVerificationDetailsTableModel resultModel = new RegistrationNoVerificationDetailsTableModel();
            resultModel.scannedFileDetailsList = new List<ScannedFileDetails>();
            long SrCount = 1;
            int DocumentTypeID = Convert.ToInt32(registrationNoVerificationDetailsModel.DocumentTypeId);
            int DocumentTypeIDFinal = 0;
            switch (DocumentTypeID)
            {
                case 1:
                    {
                        DocumentTypeIDFinal = 1;
                        break;
                    }
                case 2:
                    {
                        DocumentTypeIDFinal = 2;
                        break;
                    }
            }
            dbContext = new KaveriEntities();
            var Result = new List<ScannedFileDetails>();

            if (registrationNoVerificationDetailsModel.SROfficeID != 0)
            {
                Result = dbContext.ScannedFileUploadDetails.GroupBy(c => new { c.SROCode, c.ScannedFileName, c.DocumentTypeID })
                        .Where(grp => grp.Count() > 1 && grp.Key.DocumentTypeID == DocumentTypeIDFinal && grp.Key.SROCode == registrationNoVerificationDetailsModel.SROfficeID)
                        .Select(grp => new ScannedFileDetails
                        {
                            SROCode = grp.Key.SROCode,
                            ScannedFileName = grp.Key.ScannedFileName,
                            //Count = grp.Key.ScannedFileName.Count().ToString()
                            SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                            CountD = grp.Count()
                        }
              ).ToList();
            }
            else
            {
                Result = dbContext.ScannedFileUploadDetails.GroupBy(c => new { c.SROCode, c.ScannedFileName, c.DocumentTypeID })
                          .Where(grp => grp.Count() > 1 && grp.Key.DocumentTypeID == DocumentTypeIDFinal)
                          .Select(grp => new ScannedFileDetails
                          {
                              SROCode = grp.Key.SROCode,
                              ScannedFileName = grp.Key.ScannedFileName,
                              SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                              //Count = grp.Key.ScannedFileName.Count().ToString()
                              CountD = grp.Count()
                          }
                ).ToList();

            }

          // var FinalResult  = Result.GroupBy(x => new { x.SROCode, x.ScannedFileName }).Select(a => a.First()).ToList();
            if (Result != null)
            {
                foreach (var item in Result)
                {

                    resModel = new ScannedFileDetails();

                    resModel.srNo = SrCount++;
                    resModel.SROCode = item.SROCode;
                    resModel.ScannedFileName = item.ScannedFileName == "" ? "--" : item.ScannedFileName;
                    resModel.SroName = item.SroName;
                    resModel.CountD = item.CountD;
      
                    resultModel.scannedFileDetailsList.Add(resModel);


                }
            }
            return resultModel;
        }
        //End By Tushar on 7 Nov 2022
        //Added By Tushar on 7 Nov 2022
        public RegistrationNoVerificationDetailsTableModel GetFinalRegistrationNumberDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            //List<RPT_DocReg_NoCLBatchDetailsTable> GetDocRegNoCLBatchDetailsList = new List<RPT_DocReg_NoCLBatchDetailsTable>();
            DocumentMasterFRN resModel = null;
            //
            RegistrationNoVerificationDetailsTableModel resultModel = new RegistrationNoVerificationDetailsTableModel();
            resultModel.DocumentMasterFRNList = new List<DocumentMasterFRN>();
            long SrCount = 1;
            int DocumentTypeID = Convert.ToInt32(registrationNoVerificationDetailsModel.DocumentTypeId);
            int DocumentTypeIDFinal = 0;
            switch (DocumentTypeID)
            {
                case 1:
                    {
                        DocumentTypeIDFinal = 1;
                        break;
                    }
                case 2:
                    {
                        DocumentTypeIDFinal = 2;
                        break;
                    }
            }
            dbContext = new KaveriEntities();
            var Result = new List<DocumentMasterFRN>();

            if (registrationNoVerificationDetailsModel.SROfficeID != 0)
            {
                if (DocumentTypeIDFinal ==Convert.ToInt32( Common.ApiCommonEnum.DocumentType.Document))
                {
                    Result = dbContext.DocumentMaster.GroupBy(c => new { c.SROCode, c.FinalRegistrationNumber })
                                   .Where(grp => grp.Count() > 1 && grp.Key.SROCode == registrationNoVerificationDetailsModel.SROfficeID)
                                   .Select(grp => new DocumentMasterFRN
                                   {
                                       SROCode = grp.Key.SROCode,
                                       FinalRegistrationNumber = grp.Key.FinalRegistrationNumber,
                            //Count = grp.Key.ScannedFileName.Count().ToString()
                            SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                                       Count = grp.Count()
                                   }
                         //).ToList();
                         ).ToList(); 
                }
                else
                {
                    Result = dbContext.MarriageRegistration.GroupBy(c => new { c.PSROCode, c.MarriageCaseNo })
                                  .Where(grp => grp.Count() > 1 && grp.Key.PSROCode == registrationNoVerificationDetailsModel.SROfficeID)
                                  .Select(grp => new DocumentMasterFRN
                                  {
                                      SROCode = grp.Key.PSROCode,
                                      FinalRegistrationNumber = grp.Key.MarriageCaseNo,
                                       //Count = grp.Key.ScannedFileName.Count().ToString()
                                       SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.PSROCode).Select(c => c.SRONameE).FirstOrDefault(),
                                      Count = grp.Count()
                                  }
                        //).ToList();
                        ).ToList();
                }
            }
            else
            {
                if (  DocumentTypeIDFinal == Convert.ToInt32(Common.ApiCommonEnum.DocumentType.Document))
                {
                    Result = dbContext.DocumentMaster.GroupBy(c => new { c.SROCode, c.FinalRegistrationNumber })
                               .Where(grp => grp.Count() > 1)
                               .Select(grp => new DocumentMasterFRN
                               {
                                   SROCode = grp.Key.SROCode,
                                   FinalRegistrationNumber = grp.Key.FinalRegistrationNumber,
                                   SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                              //Count = grp.Key.ScannedFileName.Count().ToString()
                              Count = grp.Count()
                               }
                     ).OrderBy(x => x.SROCode).ToList(); 
                }
                else
                {
                    Result = dbContext.MarriageRegistration.GroupBy(c => new { c.PSROCode, c.MarriageCaseNo })
                               .Where(grp => grp.Count() > 1)
                               .Select(grp => new DocumentMasterFRN
                               {
                                   SROCode = grp.Key.PSROCode,
                                   FinalRegistrationNumber = grp.Key.MarriageCaseNo,
                                   SroName = dbContext.SROMaster.Where(d => d.SROCode == grp.Key.PSROCode).Select(c => c.SRONameE).FirstOrDefault(),
                                   //Count = grp.Key.ScannedFileName.Count().ToString()
                                   Count = grp.Count()
                               }
                     ).OrderBy(x => x.SROCode).ToList();
                }

            }

            // var FinalResult  = Result.GroupBy(x => new { x.SROCode, x.ScannedFileName }).Select(a => a.First()).ToList();
            if (Result != null)
            {
                foreach (var item in Result)
                {

                    resModel = new DocumentMasterFRN();

                    resModel.srNo = SrCount++;
                    resModel.SROCode = item.SROCode;
                    resModel.FinalRegistrationNumber = item.FinalRegistrationNumber == "" ? "--": item.FinalRegistrationNumber;
                    resModel.SroName = item.SroName;
                    resModel.Count = item.Count;

                    resultModel.DocumentMasterFRNList.Add(resModel);


                }
            }
            return resultModel;
        }
        //End By Tushar on 7 Nov 2022
        //Added By Tushar on 8 Feb 2023

        public RegistrationNoVerificationDetailsTableModel GetDateDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                RPT_DocReg_DateDetailsList resModel = null;

                RegistrationNoVerificationDetailsTableModel resultModel = new RegistrationNoVerificationDetailsTableModel();
                resultModel.RPT_DocReg_DateDetailsList = new List<RPT_DocReg_DateDetailsList>();
                long SrCount = 1;
                dbContext = new KaveriEntities();
                var ResultList = new List<RPT_DocReg_DateDetailsList>();

                if (registrationNoVerificationDetailsModel.SROfficeID != 0)
                {
                    ResultList = dbContext.RPT_DocReg_DateDetails.Where(s => s.SROCode == registrationNoVerificationDetailsModel.SROfficeID)
                        .Select(x => new RPT_DocReg_DateDetailsList
                        {
                            ID = x.ID,
                            DocumentID = x.DocumentID,
                            SROCode = x.SROCode,
                            TableName = x.TableName,
                            ReceiptID = x.ReceiptID,
                            L_DateOfReceipt = x.L_DateOfReceipt.ToString(),
                            C_DateOfReceipt = x.C_DateOfReceipt.ToString(),
                            StampDetailsID = x.StampDetailsID,
                            L_DateOfStamp = x.L_DateOfStamp.ToString(),
                            C_DateOfStamp = x.C_DateOfStamp.ToString(),
                            L_DDChalDate = x.L_DDChalDate.ToString(),
                            C_DDChalDate = x.C_DDChalDate.ToString(),
                            L_StampPaymentDate = x.L_StampPaymentDate.ToString(),
                            C_StampPaymentDate = x.C_StampPaymentDate.ToString(),
                            L_DateOfReturn = x.L_DateOfReturn.ToString(),
                            C_DateOfReturn = x.C_DateOfReturn.ToString(),
                            PartyID = x.PartyID,
                            L_AdmissionDate = x.L_AdmissionDate.ToString(),
                            C_AdmissionDate = x.C_AdmissionDate.ToString(),
                            BatchID = x.BatchID,
                            ErrorType = x.ErrorType



                        }).ToList();

                }
                else
                {
                    ResultList = dbContext.RPT_DocReg_DateDetails.Select(x => new RPT_DocReg_DateDetailsList
                    {
                        ID = x.ID,
                        DocumentID = x.DocumentID,
                        SROCode = x.SROCode,
                        TableName = x.TableName,
                        ReceiptID = x.ReceiptID,
                        L_DateOfReceipt = x.L_DateOfReceipt.ToString(),
                        C_DateOfReceipt = x.C_DateOfReceipt.ToString(),
                        StampDetailsID = x.StampDetailsID,
                        L_DateOfStamp = x.L_DateOfStamp.ToString(),
                        C_DateOfStamp = x.C_DateOfStamp.ToString(),
                        L_DDChalDate = x.L_DDChalDate.ToString(),
                        C_DDChalDate = x.C_DDChalDate.ToString(),
                        L_StampPaymentDate = x.L_StampPaymentDate.ToString(),
                        C_StampPaymentDate = x.C_StampPaymentDate.ToString(),
                        L_DateOfReturn = x.L_DateOfReturn.ToString(),
                        C_DateOfReturn = x.C_DateOfReturn.ToString(),
                        PartyID = x.PartyID,
                        L_AdmissionDate = x.L_AdmissionDate.ToString(),
                        C_AdmissionDate = x.C_AdmissionDate.ToString(),
                        BatchID = x.BatchID,
                        ErrorType = x.ErrorType



                    }).ToList();
                }
                if (ResultList != null)
                {
                    foreach (var item in ResultList)
                    {

                        resModel = new RPT_DocReg_DateDetailsList();

                        resModel.srNo = SrCount++;
                        resModel.SROCode = item.SROCode;
                        resModel.ID = item.ID;
                        resModel.DocumentID = item.DocumentID;
                        resModel.SROCode = item.SROCode;
                        resModel.TableName = string.IsNullOrEmpty(item.TableName) ? "--" : item.TableName;
                        resModel.ReceiptID = item.ReceiptID == null ? 0 : item.ReceiptID;
                        resModel.L_DateOfReceipt = string.IsNullOrEmpty(item.L_DateOfReceipt) ? "--" : Convert.ToDateTime(item.L_DateOfReceipt).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_DateOfReceipt = string.IsNullOrEmpty(item.C_DateOfReceipt) ? "--" : Convert.ToDateTime(item.C_DateOfReceipt).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.StampDetailsID = item.StampDetailsID == null ? 0 : item.StampDetailsID;
                        resModel.L_DateOfStamp = string.IsNullOrEmpty(item.L_DateOfStamp) ? "--" : Convert.ToDateTime(item.L_DateOfStamp).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_DateOfStamp = string.IsNullOrEmpty(item.C_DateOfStamp) ? "--" : Convert.ToDateTime(item.C_DateOfStamp).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_DDChalDate = string.IsNullOrEmpty(item.C_DDChalDate) ? "--" : Convert.ToDateTime(item.C_DDChalDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.L_DDChalDate = string.IsNullOrEmpty(item.L_DDChalDate) ? "--" : Convert.ToDateTime(item.L_DDChalDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.L_StampPaymentDate = string.IsNullOrEmpty(item.L_StampPaymentDate) ? "--" : Convert.ToDateTime(item.L_StampPaymentDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_StampPaymentDate = string.IsNullOrEmpty(item.C_StampPaymentDate) ? "--" : Convert.ToDateTime(item.C_StampPaymentDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.L_DateOfReturn = string.IsNullOrEmpty(item.L_DateOfReturn) ? "--" : Convert.ToDateTime(item.L_DateOfReturn).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_DateOfReturn = string.IsNullOrEmpty(item.C_DateOfReturn) ? "--" : Convert.ToDateTime(item.C_DateOfReturn).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.PartyID = item.PartyID == null ? 0 : item.PartyID;
                        resModel.L_AdmissionDate = string.IsNullOrEmpty(item.L_AdmissionDate) ? "--" : Convert.ToDateTime(item.L_AdmissionDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.C_AdmissionDate = string.IsNullOrEmpty(item.C_AdmissionDate) ? "--" : Convert.ToDateTime(item.C_AdmissionDate).ToString("dd/MM/yyyy HH:mm:ss");
                        resModel.BatchID = item.BatchID == null ? 0 : item.BatchID;
                        resModel.ErrorType = string.IsNullOrEmpty(item.ErrorType) ? "--" : item.ErrorType;


                        resultModel.RPT_DocReg_DateDetailsList.Add(resModel);


                    }
                }
                return resultModel;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //End By Tushar on 8 Feb 2023
    }

    public enum DocumentVerificationErrorcode
    {
       
        MisMatch =1,
        Deleted=2, 
        Added=3,
        CentralScanDocumentNotPresentLocalPresent =4,
        CentralandLocalScanDocumentNotPresent =5,
        CentralScanDocumentPresentLocalNotPresent =6,
        ScanFileMismatch = 7,

        //Added by Rushikesh 6 Feb 2023
        Stamp1Date = 10,
        Stamp2Date = 11, 
        Stamp3Date = 12, 
        Stamp4Date = 14, 
        Stamp5Date = 9, 
        ExecutionDate = 15, 
        PresentationDate = 16, 
        StampDate = 17, 
        WithdrawlDate=18, 
        RefusalDate=19,
        Stamp5Date1 = 20
        //Added by Rushikesh 6 Feb 2023
    }


}