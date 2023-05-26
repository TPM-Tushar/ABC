#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DataRestorationReportDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for MIS Reports module.
	* ECR No			:	431
*/
#endregion

using CustomModels.Models.MISReports.DataRestorationReport;
using CustomModels.Security;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.SRToCentralComService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using ECDataAPI.SRToCentralComService;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DataRestorationReportDAL : IDataRestorationReport
    {
        KaveriEntities dbContext = null;

        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

        string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];
        //static string debugLogFilePath = directoryPath + "\\2021\\Mar\\DatabaseRestorationDebugAPI.txt";

        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

        /// <summary>
        /// DataRestorationReport
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns DataRestorationReportViewModel Model</returns>
        public DataRestorationReportViewModel DataRestorationReport(int OfficeID)
        {
            //// HARDCODED FOR SRO OFFICE ID
            //OfficeID = 55;

            DataRestorationReportViewModel resModel = new DataRestorationReportViewModel()
            {
                SROfficeList = new List<SelectListItem>(),
                SROfficeID = 0
            };

            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReport-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    resModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                }
                else
                {
                    // BELOW CODE COMMENTED AND CHANGED ON 24-07-2020 AT 1:15 PM BY SHUBHAM BHAGAT
                    //resModel.SROfficeList.Add(new SelectListItem() { Text = "Select", Value = "0" });
                    resModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    resModel.SROfficeList.AddRange(dbContext.SROMaster.OrderBy(c => c.SRONameE).Select(m => new SelectListItem() { Value = m.SROCode.ToString(), Text = m.SRONameE }));
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReport-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

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

        /// <summary>
        /// DataRestorationReportStatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel DataRestorationReportStatus(DataRestorationReportViewModel model)
        {
            DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
            bool IsScriptReadyForInsertion = false, IsScriptExecutedSuccessfully = false, IsScriptExecutedWithError = false;
            String scriptExecutionErrorMsg = String.Empty;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();


                var officeID = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                     Select(x => x.OfficeID).FirstOrDefault();

                // OfficeName
                var OfficeName = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                     Select(x => x.OfficeName).FirstOrDefault();

                var userID = dbContext.UMG_UserDetails.Where(x => x.OfficeID == officeID).Select(x => x.UserID).FirstOrDefault();

                // SR Name
                var SRName = dbContext.UMG_UserProfile.Where(x => x.UserID == userID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                // SR ContactNumber
                var SRContactNumber = dbContext.UMG_UserProfile.Where(x => x.UserID == userID).Select(x => x.MobileNumber).FirstOrDefault();

                // SR Name
                resModel.SRName = SRName;
                // OfficeName
                resModel.OfficeName = OfficeName + " Office";
                // SR ContactNumber
                resModel.SRContactNumber = SRContactNumber;

                // for adding validation on generte key btn 

                List<DB_RES_INITIATE_MASTER> dB_RES_INITIATE_MASTERList = null;

                // FOR SR FOR INITIALIZING NEW PROCESS
                if (model.INIT_ID_INT == 0)
                {
                    // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-07-2020 AT 3:00 PM
                    // FOR DRAWING CLICK HERE BUTTON TO INITIATE NEW REQUEST

                    // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                    dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                           Where(x => x.SROCODE == model.SROfficeID &&
                                      x.IS_DRO == false &&
                                      x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated).ToList();

                    //dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                    //        Where(x => x.SROCODE == model.SROfficeID &&
                    //                   x.IS_DRO == false && x.STATUS_ID == null).ToList();

                    // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 

                    //dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                    //       Where(x => x.SROCODE == model.SROfficeID &&
                    //                  x.IS_DRO == false).ToList();
                }
                // FOR FETCH DETAILS OF PREVIOUS PPROCESS
                else
                {
                    dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                            Where(x => x.SROCODE == model.SROfficeID &&
                                       x.IS_DRO == false &&
                                       x.INIT_ID == model.INIT_ID_INT).ToList();
                }

                // ENTRY EXIST FOR A SRO
                if (dB_RES_INITIATE_MASTERList != null)
                {
                    if (dB_RES_INITIATE_MASTERList.Count() > 0)
                    {
                        // FINDING LATEST INITIATION REQUEST
                        dB_RES_INITIATE_MASTERList = dB_RES_INITIATE_MASTERList.OrderByDescending(x => x.INIT_DATE).ToList();

                        // FOR INITID
                        DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dB_RES_INITIATE_MASTERList.FirstOrDefault();

                        #region ADDED BY SHUBHAM BHAGAT ON 09-04-2021 FOR MAKING INSERT OF DATABASE RESTORATION TABLES

                        // DECLARE FILE PATH HERE BECAUSE WE HAVE TO CREATE DEBUG FILE ACCORDING TO INITID
                        //string debugLogFilePath = directoryPath + "\\2021\\Mar\\DatabaseRestorationDebugAPI_" + dB_RES_INITIATE_MASTER.INIT_ID + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";



                        //String debugScript = @"DECLARE @PRED VARCHAR(8000)
                        //                DECLARE @TEMPTAB TABLE
                        //                (
                        //                    SCRIPTTEXT VARCHAR(MAX),
                        //                    QuerySeqNo int identity(1, 1)
                        //                )
                                        
                                        
                        //                ----------------------------------- 1---------------------------------------------
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_STATUS_MASTER'
                                        
                                        
                        //                ----------------------------------- 2---------------------------------------------
                        //                INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_TABLE_MASTER ON'
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_TABLE_MASTER'
                        //                INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_TABLE_MASTER OFF'
                                        
                                        
                        //                ----------------------------------- 3---------------------------------------------
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_MISC_SCRIPTS'
                                        
                                        
                        //                ----------------------------------- 4---------------------------------------------
                                        
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_ACTIONS_MASTER'
                                        
                                        
                        //                ----------------------------------- 5---------------------------------------------
                                        
                        //                SET @PRED = 'FROM DB_RES_INITIATE_MASTER WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_INITIATE_MASTER'  , @FROM = @PRED
                                        
                                        
                        //                ----------------------------------- 6---------------------------------------------
                        //                SET @PRED = 'FROM DB_RES_ACTIVATION_KEY_OTP WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_ACTIVATION_KEY_OTP', @FROM = @PRED
                                        
                                        
                        //                ----------------------------------- 7---------------------------------------------
                        //                SET @PRED = 'FROM DB_RES_INSERT_SCRIPT_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_INSERT_SCRIPT_DETAILS', @FROM = @PRED
                                        
                                        
                                        
                        //                ----------------------------------- 8---------------------------------------------
                                        
                        //                SET @PRED = 'FROM DB_RES_ACTIONS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_ACTIONS', @FROM = @PRED
                                        
                        //                ----------------------------------- 9---------------------------------------------
                        //                SET @PRED = 'FROM DB_RES_SERVICE_COMM_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_SERVICE_COMM_DETAILS ON'
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_SERVICE_COMM_DETAILS', @FROM = @PRED
                        //                INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_SERVICE_COMM_DETAILS OFF'
                                        
                                        
                        //                ----------------------------------- 10---------------------------------------------
                        //                --@PRED VARCHAR(8000)
                        //                SET @PRED = 'FROM DB_RES_OFFICE_TABLE_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_OFFICE_TABLE_DETAILS ON'
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_OFFICE_TABLE_DETAILS', @FROM = @PRED
                        //                INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_TABLE_DETAILS OFF'
                                        
                                        
                        //                ----------------------------------- 11---------------------------------------------
                        //                SET @PRED = 'FROM DB_RES_OFFICE_COLUMN_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_COLUMN_DETAILS ON'
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_OFFICE_COLUMN_DETAILS', @FROM = @PRED
                        //                INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_COLUMN_DETAILS OFF'
                                        
                                        
                        //                ----------------------------------- 12---------------------------------------------
                        //                SET @PRED = 'FROM DB_RES_TABLEWISE_COUNT WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                        //                INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_TABLEWISE_COUNT ON'
                        //                INSERT INTO @TEMPTAB
                        //                EXEC DBO.sp_generate_inserts 'DB_RES_TABLEWISE_COUNT', @FROM = @PRED
                        //                INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_TABLEWISE_COUNT ON'
                                        
                                        
                                        
                        //                SELECT* FROM @TEMPTAB ORDER BY QuerySeqNo";

                        // addde to be 
                        //  waitfor delay '00:00:40';

                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-Before-Debug log script creation");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                        //using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                        //{
                        //    using (SqlCommand command = new SqlCommand(debugScript, connection))
                        //    {
                        //        connection.Open();

                        //        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021
                        //        String DBRESGenerateScriptCommandTimeout = ConfigurationManager.AppSettings["DBRESGenerateScriptCommandTimeout"];
                        //        command.CommandTimeout = Convert.ToInt32(DBRESGenerateScriptCommandTimeout);
                        //        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021

                        //        command.CommandType = CommandType.Text;
                                                              
                        //        command.Parameters.AddWithValue("@INIT_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;
                               
                        //        SqlDataReader detailsReader = command.ExecuteReader();
                        //        DataTable scriptsDetails = new DataTable();
                        //        scriptsDetails.Load(detailsReader);

                        //        StringBuilder debugScriptBuilder_STR = new StringBuilder();

                        //        for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                        //        {
                        //            debugScriptBuilder_STR = debugScriptBuilder_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["SCRIPTTEXT"]));
                        //        }

                        //        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //        using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //        {
                        //            string format = "{0} : {1}";
                        //            file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //            file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //            file.WriteLine(debugScriptBuilder_STR.ToString());
                        //            file.Flush();
                        //        }
                        //        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //    }
                        //}

                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-After-Debug log script creation");                            
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                        #endregion

                        // FOR KEYDATETIME
                        // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-09-2020
                        //DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = null;
                        // IF THE DATABASE RESTORATION PROCESS IS COMPLETED THEN USE IS_ACTIVE==FALSE 
                        if (dB_RES_INITIATE_MASTER.IS_COMPLETED)
                            dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == false && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        // IF THE DATABASE RESTORATION PROCESS IS IN PROGRESS THEN USE IS_ACTIVE==TRUE 
                        else
                            dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-09-2020

                        // INITIATE DATE SHOULD BE DISPLAYED IN ALL CASE IF ENTRY EXIST FOR A SRO
                        resModel.InitiationDate = dB_RES_INITIATE_MASTER.INIT_DATE.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        // PROCESS IS STILL NOT STARTED BY HCL USER
                        //if (dB_RES_INITIATE_MASTER.STATUS_ID == null)
                        //{
                        ////Initiate Date
                        //resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        //Initiate Date btn                                                                                                                       
                        resModel.InitiationDateBtn = String.Empty;

                        // Show Init Date And Generated Key Msg in all below cases
                        resModel.ShowInitDateAndGeneratedKeyMsg = true;

                        DateTime KEY_DATE_DateTime, today_Date_DateTime;
                        String today_Date_STR = String.Empty;
                        int noOfDays = 0;

                        today_Date_STR = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime.TryParse(DateTime.ParseExact(today_Date_STR, "dd/MM/yyyy HH:mm:ss", null).ToString(), out today_Date_DateTime);

                        var KEY_DATE_STR = ((DateTime)dB_RES_ACTIVATION_KEY_OTP.KEY_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime.TryParse(DateTime.ParseExact(KEY_DATE_STR, "dd/MM/yyyy HH:mm:ss", null).ToString(), out KEY_DATE_DateTime);
                        //var d = today_Date_DateTime.Subtract(INIT_DATE_DateTime);
                        //noOfDays = (today_Date_DateTime - INIT_DATE_DateTime).Days;
                        noOfDays = today_Date_DateTime.Subtract(KEY_DATE_DateTime).Days;


                        // ASK SIR TO CONSIDER STATUSID AND NOOFDAYS BOTH
                        //if (noOfDays >= 5) // IF THE KEY IS EXPIRED i.e 5 days over
                        // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        if (noOfDays >= 5 && dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated) // IF THE KEY IS EXPIRED i.e 5 days over
                        //if (noOfDays >= 5 && dB_RES_INITIATE_MASTER.STATUS_ID == null) // IF THE KEY IS EXPIRED i.e 5 days over
                        // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        {
                            // KEY IS EXPIRED
                            // SEND MSG AND POPULATE GENERATE KEY BTN
                            //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Your activation key is expired for generating another key please click <button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyAfterExpiration()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button><span id='GenerateKeyAfterExpirationValueSpanID'>" + GenerateKey() + "</span>.</label>";
                            // SEND BTN WITHOUT KEY
                            //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;width:64%;'>" +
                            //    "Your activation key is expired for generating another key please click " +
                            //    "<button type ='button' style='width:20%;' class='btn btn-group-md btn-success'" +
                            //    " onclick=GenerateKeyAfterExpiration('" + dB_RES_INITIATE_MASTER.INIT_ID + "','" + dB_RES_ACTIVATION_KEY_OTP.KEYID + "')>" +
                            //    "<i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button></label>";

                            // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                            if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;width:64%;color: #3177b4;'>" +
                                   "Your activation key is expired for generating another key please click " +
                                   "<button type ='button' style='width:20%;' class='btn btn-group-md btn-success'" +
                                   " onclick=GenerateKeyAfterExpiration('" + dB_RES_INITIATE_MASTER.INIT_ID + "','" + dB_RES_ACTIVATION_KEY_OTP.KEYID + "')>" +
                                   "<i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button></label>";
                            else
                                resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                            // SET ACTIVATION KEY ACTIVE STATUS TO 0 FOR MAKING INACTIVE
                            //DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTPExpired = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                            //dB_RES_ACTIVATION_KEY_OTPExpired.IS_ACTIVE = false;
                            //dbContext.SaveChanges();
                        }
                        else // IF THE KEY IS NOT EXPIRED
                        {
                            DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTPPrevious = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();

                            // DISPLAY KEY WITH MSG
                            if (dB_RES_ACTIVATION_KEY_OTPPrevious != null)
                            {
                                //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Activation key for data restoration utility is : " + Decrypt(dB_RES_ACTIVATION_KEY_OTPPrevious.KEYVALUE) + " which is valid for 5 days only Please share this activation key to support Engineer.</label>";
                                // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                    resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : " + Decrypt(dB_RES_ACTIVATION_KEY_OTPPrevious.KEYVALUE) + " which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                                else
                                    resModel.GenerateKeyBtnAndTextMsg = String.Empty;
                            }


                        }
                        //}
                        //else  // IF THE PROCESS IS STARTED BY HCL USER
                        //{

                        //    // CHECK STATUS AND ACTIONS
                        //    // ADD SWITCH CASE AND ADD STATUS TABLE ROWS IN CASES
                        //    //DB_RES_STATUS_MASTER dB_RES_STATUS_MASTER = dbContext.DB_RES_STATUS_MASTER.Where(x => x.STATUS_ID == dB_RES_INITIATE_MASTER.STATUS_ID).FirstOrDefault();
                        //    //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>" + dB_RES_STATUS_MASTER.STATUS_DESCRIPTION + "</label>";
                        //}

                        // Database Restoration Details :
                        #region Database Restoration Details: 
                        // CHANGED IF CONDITION ON 06-07-2020 AT 5:30 PM
                        //BECAUSE IF WE COMPARE WITH STATUSID=1 SO IT WILL NOT SHOW BOTH THE SECTIONS I.E DATABASE RESTORATION DETAILS
                        //AND DATA INSERTION DETAILS
                        //if (dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DatabaseRestored)
                        if (dB_RES_INITIATE_MASTER.STATUS_ID >= (int)ApiCommonEnum.DB_RES_STATUSMASTER.DatabaseRestored)
                        {
                            bool databaseRestoredSuccessfully = true;

                            // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                            // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                            // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR
                            // List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).ToList();

                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                            // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                            //    x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && 
                            //                       (
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema 
                            //                       )
                            //    ).ToList();

                            List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                               x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                  (
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                                  )
                               ).ToList();
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021

                            // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                            if (db_RES_ACTIONSList != null)
                            {
                                if (db_RES_ACTIONSList.Count() > 0)
                                {
                                    //ADDED BELOW CODE BY SHUBHAM BHAGAT ON 17-07-2020 AT 6:00 PM 
                                    //TO GET LATEST ACTIONS RELATED TO CURRENT INIT_ID
                                    // ORDER BY DESCENDING
                                    db_RES_ACTIONSList = db_RES_ACTIONSList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                    // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                    // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                                    // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR

                                    //// BEFORE TAKING TOP 6 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 6 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 6)
                                    //{
                                    //    // TAKING TOP 6 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(6).ToList();
                                    //}

                                    // BEFORE TAKING TOP 7 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 7 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 7)
                                    //{
                                    //    // TAKING TOP 7 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(7).ToList();
                                    //}
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020


                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                                    // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                                    // ABOVE CODE IS COMMENTED BECAUSE PREVIOUSLY WE ARE SENDING ONLY 7 ACTIONS FROM UTILITY TO SRTOCENTRALCOMM SERVICE NOW WE ARE SENDING 10 ACTIONS
                                    // BEFORE TAKING TOP 10 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 10 ELEMENTS
                                    if (db_RES_ACTIONSList.Count > 10)
                                    {
                                        // TAKING TOP 7 ELEMENTS 
                                        db_RES_ACTIONSList = db_RES_ACTIONSList.Take(10).ToList();
                                    }
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021


                                    //ADDED ABOVE CODE BY SHUBHAM BHAGAT ON 17-07-2020 AT 6:00 PM 

                                    foreach (var item in db_RES_ACTIONSList)
                                    {
                                        if (!item.IS_SUCCESSFUL)
                                            databaseRestoredSuccessfully = false;
                                    }
                                    if (databaseRestoredSuccessfully)
                                    {
                                        // DATABASE RESTORATION DATE
                                        DB_RES_ACTIONS db_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                                    x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).FirstOrDefault();

                                        // DATABASE RESTORATION DATE
                                        resModel.DataRestorationDate = db_RES_ACTIONS.ACTION_DATETIME.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                       

                                        //List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SERVICE_TYPE == ApiCommonEnum.DB_RES_SERVICE_COMM_DETAILS_MASTER.DataBaseRestored.ToString()).ToList();
                                        List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID).ToList();
                                        if (db_RES_SERVICE_COMM_DETAILSList != null)
                                        {
                                            if (db_RES_SERVICE_COMM_DETAILSList.Count() > 0)
                                            {
                                                // DATABASE RESTORATION FROM BAK FILE DATETIME
                                                var DB_Restored_upto_BAK_DT_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_DT").Select(x => x.KEY_VALUE);
                                                resModel.DB_RES_BAK_FILE_DATETIME = DB_Restored_upto_BAK_DT_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_DT_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (resModel.DB_RES_BAK_FILE_DATETIME != String.Empty)
                                                {
                                                    resModel.DB_RES_BAK_FILE_DATETIME = resModel.DB_RES_BAK_FILE_DATETIME.Replace('-', '/');
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                // DATABASE RESTORATION FROM BAK FILE size
                                                var DB_Restored_upto_BAK_Size_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_Size").Select(x => x.KEY_VALUE);
                                                resModel.DB_RES_BAK_FILE_Size = DB_Restored_upto_BAK_Size_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_Size_IEnumerable.FirstOrDefault();
                                                resModel.DB_RES_BAK_FILE_Size = resModel.DB_RES_BAK_FILE_Size == null ? "0 GB" : resModel.DB_RES_BAK_FILE_Size;

                                                // LastDocumentID
                                                var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                                String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();
                                                long DocumentID_INT = Convert.ToInt64(DocumentID_STR);

                                                // LastRegistrationID
                                                var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                                String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();
                                                long RegistrationID_INT = Convert.ToInt64(RegistrationID_STR);

                                                // LastNoticeID
                                                var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                                String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();
                                                long NoticeID_INT = Convert.ToInt64(NoticeID_STR);

                                                // DON'T COMMENT BELOW CODE BECAUSE IT IS USING FOR GENERATE SCRIPT USING STORED PROCEDURE, STORED PROCEDURE IS USING FINAL REGISTRATION NUMBER AND SRO CODE 
                                                var FinalRegistrationNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "FinalRegistrationNumber").Select(x => x.KEY_VALUE);
                                                String FinalRegistrationNumber_ForGenerateScript = FinalRegistrationNumber_IEnumerable == null ? String.Empty : FinalRegistrationNumber_IEnumerable.FirstOrDefault();

                                                // LastDocumentRegistrationNumber
                                                //var FinalRegistrationNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "FinalRegistrationNumber").Select(x => x.KEY_VALUE);
                                                //resModel.LastDocumentRegistrationNumber = FinalRegistrationNumber_IEnumerable == null ? String.Empty : FinalRegistrationNumber_IEnumerable.FirstOrDefault();

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 30-09-2020
                                                //resModel.LastDocumentRegistrationNumber = FinalRegistrationNumber_ForGenerateScript;
                                                resModel.LastDocumentRegistrationNumber = String.IsNullOrEmpty(FinalRegistrationNumber_ForGenerateScript) ? "-" : FinalRegistrationNumber_ForGenerateScript;
                                                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 30-09-2020
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

                                                // both below code is working
                                                //var foo = from db_RES_SERVICE_COMM_DETAILS in db_RES_SERVICE_COMM_DETAILSList.AsQueryable<DB_RES_SERVICE_COMM_DETAILS>()
                                                //          where db_RES_SERVICE_COMM_DETAILS.KEY_COLUMN.Contains("FinalRegistrationNumber")
                                                //          select db_RES_SERVICE_COMM_DETAILS.KEY_VALUE;
                                                // on output in immediate window foo.FirstOrDefault(); or foo.SingleOrDefault() will give output
                                                //var foo = from db_RES_SERVICE_COMM_DETAILS in db_RES_SERVICE_COMM_DETAILSList.AsQueryable<DB_RES_SERVICE_COMM_DETAILS>()
                                                //          where db_RES_SERVICE_COMM_DETAILS.KEY_COLUMN.Contains("FinalRegistrationNumber")
                                                //          select db_RES_SERVICE_COMM_DETAILS.KEY_VALUE.FirstOrDefault();

                                                // BELOW CODE UNCOMMENTED AND DATA FROM DOCUMENT MASTER IS COMMENTED BY SHUBHAM BHAGAT 
                                                // ON 30-09-2020 AFTER DISCUSSION WITH SIR
                                                //LastDocumentRegistrationDate
                                                var LastDocumentRegistrationDate_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "Stamp5DateTime").Select(x => x.KEY_VALUE);
                                                resModel.LastDocumentRegistrationDate = LastDocumentRegistrationDate_IEnumerable == null ? String.Empty : LastDocumentRegistrationDate_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 30-09-2020 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                //if (resModel.LastDocumentRegistrationDate != String.Empty)
                                                if (!String.IsNullOrEmpty(resModel.LastDocumentRegistrationDate))
                                                {
                                                    resModel.LastDocumentRegistrationDate = resModel.LastDocumentRegistrationDate.Replace('-', '/');
                                                }
                                                // BELOW ADDED BY SHUBHAM BHAGAT ON 8-12-2020
                                                // BECAUSE WE WANT TO DISPLAY '-' WHEN THE DOCUMENT IS NOT REGISTERED
                                                else
                                                {
                                                    resModel.LastDocumentRegistrationDate = "          -";
                                                }
                                                // ABOVE ADDED BY SHUBHAM BHAGAT ON 8-12-2020

                                                //// LastDocumentRegistrationNumber i.e Document Number
                                                //// LastDocumentRegistrationDate i.e Present
                                                //DocumentMaster documentMaster = dbContext.DocumentMaster.Where(x => x.DocumentID == DocumentID_INT && x.SROCode == model.SROfficeID).FirstOrDefault();
                                                //if (documentMaster != null)
                                                //{
                                                //    resModel.LastDocumentRegistrationNumber = documentMaster.DocumentNumber.ToString();
                                                //    resModel.LastDocumentRegistrationDate = documentMaster.PresentDateTime == null ? String.Empty : ((DateTime)documentMaster.PresentDateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                //}
                                                // ABOVE CODE UNCOMMENTED AND DATA FROM DOCUMENT MASTER IS COMMENTED BY SHUBHAM BHAGAT 
                                                // ON 30-09-2020 AFTER DISCUSSION WITH SIR

                                                // LastMarriageRegistrationNumber
                                                var MarriageCaseNo_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "MarriageCaseNo").Select(x => x.KEY_VALUE);
                                                resModel.LastMarriageRegistrationNumber = MarriageCaseNo_IEnumerable == null ? String.Empty : MarriageCaseNo_IEnumerable.FirstOrDefault();

                                                // LastMarriageRegistrationDate
                                                var DateOfRegistration_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DateOfRegistration").Select(x => x.KEY_VALUE);
                                                resModel.LastMarriageRegistrationDate = DateOfRegistration_IEnumerable == null ? String.Empty : DateOfRegistration_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (!String.IsNullOrEmpty(resModel.LastMarriageRegistrationDate ))
                                                {
                                                    resModel.LastMarriageRegistrationDate = resModel.LastMarriageRegistrationDate.Replace('-', '/');
                                                }

                                                // LastNoticeRegistrationNumber
                                                var NoticeNo_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeNo").Select(x => x.KEY_VALUE);
                                                resModel.LastNoticeRegistrationNumber = NoticeNo_IEnumerable == null ? String.Empty : NoticeNo_IEnumerable.FirstOrDefault();

                                                // LastNoticeRegistrationDate
                                                var NoticeIssuedDate_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeIssuedDate").Select(x => x.KEY_VALUE);
                                                resModel.LastNoticeRegistrationDate = NoticeIssuedDate_IEnumerable == null ? String.Empty : NoticeIssuedDate_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (!String.IsNullOrEmpty(resModel.LastNoticeRegistrationDate))
                                                {
                                                    resModel.LastNoticeRegistrationDate = resModel.LastNoticeRegistrationDate.Replace('-', '/');
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM BELOW CODE

                                                // DocumentRegistrationCDNumber
                                                var DocumentRegistrationCDNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentRegistrationCDNumber").Select(x => x.KEY_VALUE);
                                                resModel.DocumentRegistrationCDNumber = DocumentRegistrationCDNumber_IEnumerable == null ? String.Empty : DocumentRegistrationCDNumber_IEnumerable.FirstOrDefault();

                                                // MarriageRegistrationCDNumber
                                                var MarriageRegistrationCDNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "MarriageRegistrationCDNumber").Select(x => x.KEY_VALUE);
                                                resModel.MarriageRegistrationCDNumber = MarriageRegistrationCDNumber_IEnumerable == null ? String.Empty : MarriageRegistrationCDNumber_IEnumerable.FirstOrDefault();

                                                // DocumentRegistrationCDNumber
                                                var DB_Restored_upto_BAK_FileName_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_FileName").Select(x => x.KEY_VALUE);
                                                resModel.DB_Restored_upto_BAK_FileName = DB_Restored_upto_BAK_FileName_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_FileName_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM ABOVE CODE


                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 8-12-2020
                                                var PresentDateTime_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "PresentDateTime").Select(x => x.KEY_VALUE);
                                                resModel.PresentDateTime = PresentDateTime_IEnumerable == null ? String.Empty : PresentDateTime_IEnumerable.FirstOrDefault();
                                                
                                                if (!String.IsNullOrEmpty(resModel.PresentDateTime))
                                                {
                                                    resModel.PresentDateTime = resModel.PresentDateTime.Replace('-', '/');
                                                }

                                                var DocumentNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentNumber").Select(x => x.KEY_VALUE);
                                                resModel.DocumentNumber = DocumentNumber_IEnumerable == null ? String.Empty : DocumentNumber_IEnumerable.FirstOrDefault();
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 8-12-2020

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                var OtherReceiptID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "OtherReceiptID").Select(x => x.KEY_VALUE);
                                                resModel.OtherReceiptID = OtherReceiptID_IEnumerable == null ? String.Empty : OtherReceiptID_IEnumerable.FirstOrDefault();
                                                long OtherReceiptID_LONG = Convert.ToInt64(resModel.OtherReceiptID);
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021
                                                var ReceiptNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "ReceiptNumber").Select(x => x.KEY_VALUE);
                                                resModel.ReceiptNumber = ReceiptNumber_IEnumerable == null ? String.Empty : ReceiptNumber_IEnumerable.FirstOrDefault();
                                                
                                                var ReceiptDateTime_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "ReceiptDateTime").Select(x => x.KEY_VALUE);
                                                resModel.ReceiptDateTime = ReceiptDateTime_IEnumerable == null ? String.Empty : ReceiptDateTime_IEnumerable.FirstOrDefault();

                                                if (!String.IsNullOrEmpty(resModel.ReceiptDateTime))
                                                {
                                                    resModel.ReceiptDateTime = resModel.ReceiptDateTime.Replace('-', '/');
                                                }
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021


                                                // FOR DISPLAYING DATA RESTORATION DETAILS
                                                resModel.IsDataRestored = true;

                                                #region FOR GENERATING SCRIPT AND SAVING AT SERVER 
                                                // CHANGED BELOW IF CONDITION BY SHUBHAM BHAGAT ON 04-07-2020 AT 10:50 PM
                                                //if (resModel.LastDocumentRegistrationNumber != null)
                                                if (FinalRegistrationNumber_ForGenerateScript != null)
                                                {
                                                    // IF INSERT SCRIPT IS ALREADY GENERATED AND SAVED ON FILE SERVER AND WAITING FOR APPROVAL THEN POPULATE ONLY APPROVE BUTTON DON'T GENERATE SCRIPT AND SAVE ON FILE SERVER 
                                                    //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null).FirstOrDefault();

                                                    //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();

                                                    // ON 10-07-2020 AT 5:30 PM ABOVE LINE COMMENTED AND LIST IS FETCHED BECAUSE WE HAVE TO GET LIST 
                                                    List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                    if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn != null)
                                                    {
                                                        if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.Count() == 0)
                                                        {
                                                            #region BELOW CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021 ADD BUTTON TO GENERATE SCRIPT
                                                            //if (model.Is_GenerateScriptBTN_UI)
                                                            //{
                                                                resModel.Is_GenerateScriptBTN_UI = true;
                                                            if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                resModel.GenerateScriptBTN_UI = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=GenerateScriptBySR('" + model.SROfficeID + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Compare with Central Database</button>";
                                                            else
                                                                resModel.GenerateScriptBTN_UI = "Script need to be generated by sub registrar.";

                                                            resModel.dB_RES_ACTIONsForErrorHistory = new List<DB_RES_ACTIONS_CLASS>();
                                                            return resModel;
                                                            //}
                                                            #endregion

                                                            //int a = dbContext.USP_GenerateDignosticData_D_ECDATA_SB(model.SROfficeID, resModel.LastDocumentRegistrationNumber);

                                                            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                            //{
                                                            //    string format = "{0} : {1}";
                                                            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                            //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-Before-USP_GenerateDignosticData_D_ECDATA");
                                                            //    file.Flush();
                                                            //}
                                                            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                                                            using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                                                            {
                                                                using (SqlCommand command = new SqlCommand("USP_GenerateDignosticData_D_ECDATA", connection))
                                                                {
                                                                    connection.Open();

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021
                                                                    String DBRESGenerateScriptCommandTimeout = ConfigurationManager.AppSettings["DBRESGenerateScriptCommandTimeout"];
                                                                    command.CommandTimeout = Convert.ToInt32(DBRESGenerateScriptCommandTimeout);
                                                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021

                                                                    command.CommandType = CommandType.StoredProcedure;

                                                                    command.Parameters.AddWithValue("@SROCODE", SqlDbType.Int).Value = model.SROfficeID;

                                                                    // COMMENTED AND ADDED BELOW PARAMETERS ON 07-07-2020 AT 2:55 PM
                                                                    //command.Parameters.AddWithValue("@FRN", SqlDbType.VarChar).Value = FinalRegistrationNumber_ForGenerateScript;                                                             
                                                                    command.Parameters.AddWithValue("@documentID", SqlDbType.Int).Value = DocumentID_INT;

                                                                    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                                                    command.Parameters.AddWithValue("@RegistrationID", SqlDbType.Int).Value = RegistrationID_INT;
                                                                    command.Parameters.AddWithValue("@NoticeID", SqlDbType.Int).Value = NoticeID_INT;
                                                                    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                                                                    command.Parameters.AddWithValue("@Init_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                    // COMMENTED AND ADDED ABOVE PARAMETERS ON 07-07-2020 AT 2:55 PM

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                                    command.Parameters.AddWithValue("@ReceiptID", SqlDbType.Int).Value = resModel.OtherReceiptID;
                                                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021

                                                                    SqlDataReader detailsReader = command.ExecuteReader();
                                                                    DataTable scriptsDetails = new DataTable();
                                                                    scriptsDetails.Load(detailsReader);

                                                                    StringBuilder scriptText_STR = new StringBuilder();

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 10-08-2020
                                                                    String IsErrorMsg_String = Convert.ToString(scriptsDetails.Rows[scriptsDetails.Rows.Count - 2]["StrQuery"]);
                                                                    if (IsErrorMsg_String.Contains("Error occured for table"))
                                                                    {
                                                                        // FOR FETCHING LAST ERROR FOR SAME INIT_ID IF THE ERROR IS
                                                                        // SAME THEN DON'T SAVE LATEST ERROR IN DB_RES_ACTIONS
                                                                        List<DB_RES_ACTIONS> scriptGeneraError_ActionList = dbContext.DB_RES_ACTIONS.Where(x =>
                                                                                                x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                                                                x.IS_SUCCESSFUL == false &&
                                                                                                x.ACTION_ID == (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript
                                                                                                ).ToList();

                                                                        if (scriptGeneraError_ActionList != null)
                                                                        {
                                                                            // ORDER BY DESC ACCORDING TO ACTION_DATETIME
                                                                            scriptGeneraError_ActionList = scriptGeneraError_ActionList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                                                            // FETCHING PREVIOUS ACTION MODEL IF EXIST
                                                                            DB_RES_ACTIONS previousScriGeneActionModel = scriptGeneraError_ActionList.FirstOrDefault();
                                                                            if (previousScriGeneActionModel != null)
                                                                            {
                                                                                // IF PREVIOUS ERROR IS SAME AS CURRENT ERROR THEN DON'T SAVE CURRENT ERROR 
                                                                                if (!previousScriGeneActionModel.ERROR_DESCRIPTION.Equals(IsErrorMsg_String))
                                                                                {
                                                                                    // FOR SAVING ERROR MESSAGE IN DB_RES_ACTIONS TABLE
                                                                                    DB_RES_ACTIONS scriptGeneraErrorActionModel = new DB_RES_ACTIONS();
                                                                                    scriptGeneraErrorActionModel.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                    scriptGeneraErrorActionModel.ACTION_ID = (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                    scriptGeneraErrorActionModel.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                    scriptGeneraErrorActionModel.SCRIPT_ID = null;
                                                                                    scriptGeneraErrorActionModel.ACTION_DATETIME = DateTime.Now;
                                                                                    scriptGeneraErrorActionModel.IS_SUCCESSFUL = false;
                                                                                    scriptGeneraErrorActionModel.ERROR_DESCRIPTION = IsErrorMsg_String;
                                                                                    scriptGeneraErrorActionModel.CORRECTIVEACTION = null;

                                                                                    // SAVE 
                                                                                    dbContext.DB_RES_ACTIONS.Add(scriptGeneraErrorActionModel);
                                                                                    dbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            // IF ERROR OCCURED FIRST TIME FOR INIT_ID
                                                                            else
                                                                            {
                                                                                // FOR SAVING ERROR MESSAGE IN DB_RES_ACTIONS TABLE
                                                                                DB_RES_ACTIONS scriptGeneraErrorActionModel = new DB_RES_ACTIONS();
                                                                                scriptGeneraErrorActionModel.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                scriptGeneraErrorActionModel.ACTION_ID = (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                scriptGeneraErrorActionModel.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                scriptGeneraErrorActionModel.SCRIPT_ID = null;
                                                                                scriptGeneraErrorActionModel.ACTION_DATETIME = DateTime.Now;
                                                                                scriptGeneraErrorActionModel.IS_SUCCESSFUL = false;
                                                                                scriptGeneraErrorActionModel.ERROR_DESCRIPTION = IsErrorMsg_String;
                                                                                scriptGeneraErrorActionModel.CORRECTIVEACTION = null;

                                                                                // SAVE 
                                                                                dbContext.DB_RES_ACTIONS.Add(scriptGeneraErrorActionModel);
                                                                                dbContext.SaveChanges();
                                                                            }
                                                                        }

                                                                        // FLAG TO DISPLAY SCRIPT GENERATION ERROR 
                                                                        resModel.IsScriptGenerationError = true;

                                                                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                        {
                                                                            // FLAG TO DISPLAY SCRIPT GENERATION ERROR MESSAGE
                                                                            resModel.IsScriptGenerationMsg = "Some error occured while generating scripts.";
                                                                        }
                                                                        else
                                                                        {
                                                                            // FLAG TO DISPLAY SCRIPT GENERATION ERROR MESSAGE
                                                                            resModel.IsScriptGenerationMsg = IsErrorMsg_String;
                                                                        }
                                                                        resModel.dB_RES_TABLE_WISE_COUNT_List = new List<DB_RES_TABLE_WISE_COUNT>();
                                                                        resModel.dB_RES_ACTIONsForErrorHistory = new List<DB_RES_ACTIONS_CLASS>();
                                                                        resModel.ScriptExecutionErrorMsg = String.Empty;
                                                                        resModel.DataInsertionDetailsList = new List<DataInsertionDetails>();

                                                                        return resModel;
                                                                    }
                                                                    // ABOVE CODE ADDED BU SHUBHAM BHAGAT ON 10-08-2020

                                                                    if (!resModel.IsScriptGenerationError)
                                                                    {

                                                                        for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                                                                        {
                                                                            scriptText_STR = scriptText_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["StrQuery"]));
                                                                        }


                                                                        ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                                                                        {
                                                                            IsDro = false,
                                                                            SROCode = model.SROfficeID,
                                                                            // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                                                                            //ScriptContent = ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString())
                                                                            ScriptContent = EncryptScript(ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString()))
                                                                            // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                                                                        };

                                                                        // CREATE SERVICE OBJECT 
                                                                        SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();

                                                                        // CALL SAVE GENERATE SCRIPT FILE AT FILE SERVER USING SERVICE

                                                                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                                        //{
                                                                        //    string format = "{0} : {1}";
                                                                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                        //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-BEFORE-service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel) to save the script at the file server");
                                                                        //    file.Flush();
                                                                        //}
                                                                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                                        ScriptSaveRESModel scriptSaveRESModel = service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel);

                                                                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                                        //{
                                                                        //    string format = "{0} : {1}";
                                                                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                        //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-AFTER-service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel) to save the script at the file server");
                                                                        //    file.Flush();
                                                                        //}
                                                                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                                        if (scriptSaveRESModel != null)
                                                                        {
                                                                            // IF SCRIPT SAVED SUCCEFULLY AND ERROR MESSAGE IS EMPTY THEN 
                                                                            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                            // WITH  SUCCESS STATUS
                                                                            if (scriptSaveRESModel.IsScriptSavedSuccessfully && scriptSaveRESModel.ErrorMsg == String.Empty)
                                                                            {
                                                                                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        #region WORK DONE FOR ADDING DUMMY DATA BY SHUBHAM ON 30-09-2020


                                                                                        //// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        ////RegistrationID_INT
                                                                                        ////DocumentID_INT
                                                                                        ////NoticeID_INT
                                                                                        //var DocumentMasterOnCentralList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT).ToList();
                                                                                        //var MarriageRegOnCentralList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT).ToList();
                                                                                        //var NoticeMasterOnCentralList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT).ToList();

                                                                                        //int DocumentMasterOnCentralCount = 0;
                                                                                        //int MarriageRegOnCentralCount = 0;
                                                                                        //int NoticeMasterOnCentralCount = 0;

                                                                                        //if (DocumentMasterOnCentralList != null)
                                                                                        //{
                                                                                        //    if (DocumentMasterOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        DocumentMasterOnCentralCount = DocumentMasterOnCentralList.Count;
                                                                                        //    }
                                                                                        //}
                                                                                        //if (MarriageRegOnCentralList != null)
                                                                                        //{
                                                                                        //    if (MarriageRegOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        MarriageRegOnCentralCount = MarriageRegOnCentralList.Count;
                                                                                        //    }
                                                                                        //}
                                                                                        //if (NoticeMasterOnCentralList != null)
                                                                                        //{
                                                                                        //    if (NoticeMasterOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        NoticeMasterOnCentralCount = NoticeMasterOnCentralList.Count;
                                                                                        //    }
                                                                                        //}

                                                                                        //// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020


                                                                                        //// ADDED IF BLOCK BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        //// BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        //if (DocumentMasterOnCentralCount > 0 || MarriageRegOnCentralCount > 0 || NoticeMasterOnCentralCount > 0)
                                                                                        //{
                                                                                        //    // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        //    int SCRIPT_ID_For_ADD = 0;
                                                                                        //    DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        //    SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        //    save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        //    save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        //    save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        //    save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                                        //    save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        //    save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        //    save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        //    save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        //    dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        //    dbContext.SaveChanges();

                                                                                        //    // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        //    DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        //    saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        //    saveGenerateActionDetails.INIT_ID = null;
                                                                                        //    saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        //    saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        //    saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                                        //    saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        //    saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                                        //    dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        //    dbContext.SaveChanges();

                                                                                        //    dbContextTransaction.Commit();

                                                                                        //    // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                                        //    resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                                                                                        //    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                                        //    resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";
                                                                                        //}
                                                                                        //// ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        //// BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        //else
                                                                                        //{
                                                                                        //    // FOR DUMMY ENTRY 

                                                                                        //    dbContextTransaction.Commit();

                                                                                        //    // POPULATE EMPTY BUTTON IN CASE OF DUMMY DATA
                                                                                        //    resModel.ApproveBtn = String.Empty;

                                                                                        //    // POPULATE EMPTY BUTTON IN CASE OF DUMMY DATA
                                                                                        //    resModel.NoteForApprovalForSR = String.Empty;

                                                                                        //}
                                                                                        #endregion

                                                                                        //BELOW CODE COMMENTED AND SHIFTED BY SHUBHAM BHAGAT ON 30 - 09 - 2020
                                                                                        // IN IF BLOCK BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        int SCRIPT_ID_For_ADD = 0;
                                                                                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        dbContext.SaveChanges();

                                                                                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        saveGenerateActionDetails.INIT_ID = null;
                                                                                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        dbContext.SaveChanges();

                                                                                        dbContextTransaction.Commit();

                                                                                        // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                                        resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";


                                                                                    }
                                                                                    catch (Exception)
                                                                                    {
                                                                                        dbContextTransaction.Rollback();
                                                                                    }
                                                                                }
                                                                            }
                                                                            // IF SCRIPT NOT SAVED SUCCEFULLY AND ERROR MESSAGE IS NOT EMPTY THEN 
                                                                            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                            // WITH  FALSE SUCCESS STATUS AND ERROR MESSAGE
                                                                            else
                                                                            {
                                                                                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        //DocumentID_INT
                                                                                        //reg

                                                                                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        int SCRIPT_ID_For_ADD = 0;
                                                                                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        save_Script_DETAILS.SCRIPT_PATH = String.Empty;
                                                                                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        dbContext.SaveChanges();

                                                                                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        saveGenerateActionDetails.INIT_ID = null;
                                                                                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        saveGenerateActionDetails.IS_SUCCESSFUL = false;
                                                                                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        saveGenerateActionDetails.ERROR_DESCRIPTION = scriptSaveRESModel.ErrorMsg;

                                                                                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        dbContext.SaveChanges();

                                                                                        dbContextTransaction.Commit();
                                                                                    }
                                                                                    catch (Exception)
                                                                                    {
                                                                                        dbContextTransaction.Rollback();
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                            //{
                                                            //    string format = "{0} : {1}";
                                                            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                            //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-After-USP_GenerateDignosticData_D_ECDATA");
                                                            //    file.Flush();
                                                            //}
                                                            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                            dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                        }

                                                        if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.Count() > 0)
                                                        {
                                                            dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                                            //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                            DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.FirstOrDefault();

                                                            if (insertScriptAlreadyGenerated != null)
                                                            {
                                                                // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                // IF SCRIPT GENERATED AND SAVED BUT NOT APPROVED THEN POPULATE APPROVE BUTTON 
                                                                if (insertScriptAlreadyGenerated.SCRIPT_APPROVAL_DATETIME == null)
                                                                {
                                                                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                                                                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                    {
                                                                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + insertScriptAlreadyGenerated.SCRIPT_ID + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";
                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                        resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";
                                                                    }
                                                                    else
                                                                        resModel.ApproveBtn = String.Empty;
                                                                    // FOR SETTING STATUS OF SCRIPT
                                                                    IsScriptReadyForInsertion = false;
                                                                }
                                                                // IF SCRIPT GENERATED AND SAVED BUT APPROVED THEN DON'T POPULATE APPROVE BUTTON 
                                                                else
                                                                {
                                                                    // resModel.ApproveBtn = "";
                                                                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                    // ADDED IF ELSE CONDITION BECAUSE FOR TECHADMIN WE HAVE TO DRAW EMPTY BUTTON AND FOR SR WE HAVE TO SHOW MESSAGE AFTER APPROVAL
                                                                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                    {
                                                                        // COMMENTED AND CHANGED ON 07-08-2020 AT 4:30 PM
                                                                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020
                                                                        // ADDED CONDITION TO HIDE APPROVE MESSAGE IF SCRIPT IS EXECUTED AND ERROR IS OCCURED
                                                                        if (insertScriptAlreadyGenerated.SCRIPT_EXECUTION_DATETIME != null && insertScriptAlreadyGenerated.IS_SUCCESSFUL == false)
                                                                        {
                                                                            resModel.ApproveBtn = String.Empty;
                                                                        }
                                                                        else
                                                                        {
                                                                            resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask system integrator to execute restoration utility to restore above data.</label>";
                                                                        }
                                                                        //resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask system integrator to execute restoration utility to restore above data.</label>";
                                                                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020


                                                                        //resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask support engineer to execute restoration utility to restore above data.</label>";
                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                        resModel.NoteForApprovalForSR = String.Empty;
                                                                    }
                                                                    else
                                                                        resModel.ApproveBtn = String.Empty;

                                                                    // FOR SETTING STATUS OF SCRIPT
                                                                    IsScriptReadyForInsertion = true;
                                                                }
                                                            }
                                                            // IF INSERT SCRIPT IS NOT GENERATED THEN GENERATE SCRIPT AND SAVE IT ON FILE SERVER AND ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_ACTIONS TABLE
                                                            else
                                                            {
                                                                // FOR SETTING STATUS OF SCRIPT
                                                                IsScriptReadyForInsertion = false;

                                                                ////int a = dbContext.USP_GenerateDignosticData_D_ECDATA_SB(model.SROfficeID, resModel.LastDocumentRegistrationNumber);
                                                                //using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                                                                //{
                                                                //    using (SqlCommand command = new SqlCommand("USP_GenerateDignosticData_D_ECDATA", connection))
                                                                //    {
                                                                //        connection.Open();
                                                                //        command.CommandType = CommandType.StoredProcedure;

                                                                //        command.Parameters.AddWithValue("@SROCODE", SqlDbType.Int).Value = model.SROfficeID;

                                                                //        // COMMENTED AND ADDED BELOW PARAMETERS ON 07-07-2020 AT 2:55 PM
                                                                //        //command.Parameters.AddWithValue("@FRN", SqlDbType.VarChar).Value = FinalRegistrationNumber_ForGenerateScript;
                                                                //        command.Parameters.AddWithValue("@documentID", SqlDbType.Int).Value = DocumentID_INT;
                                                                //        command.Parameters.AddWithValue("@Init_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //        // COMMENTED AND ADDED ABOVE PARAMETERS ON 07-07-2020 AT 2:55 PM

                                                                //        SqlDataReader detailsReader = command.ExecuteReader();
                                                                //        DataTable scriptsDetails = new DataTable();
                                                                //        scriptsDetails.Load(detailsReader);

                                                                //        StringBuilder scriptText_STR = new StringBuilder();

                                                                //        for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                                                                //        {
                                                                //            scriptText_STR = scriptText_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["StrQuery"]));
                                                                //        }

                                                                //        #region FOR SAVING FILE LOCALLY
                                                                //        ////var path = @"F:\2020\GeneratedScript.txt";
                                                                //        ////var path = @"F:\2020\generatescripttesting.txt";
                                                                //        //var path = @"F:\2020\generated_script_3-07-2020.txt";

                                                                //        ////string text = "old falcon";
                                                                //        //File.WriteAllText(path, scriptText_STR.ToString()); 
                                                                //        #endregion

                                                                //        //scriptText_STR.
                                                                //        //ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString());
                                                                //        //model.SROfficeID

                                                                //        ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                                                                //        {
                                                                //            IsDro = false,
                                                                //            SROCode = model.SROfficeID,
                                                                //            ScriptContent = ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString())
                                                                //        };

                                                                //        // CREATE SERVICE OBJECT 
                                                                //        SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();

                                                                //        // CALL SAVE GENERATE SCRIPT FILE AT FILE SERVER USING SERVICE
                                                                //        ScriptSaveRESModel scriptSaveRESModel = service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel);
                                                                //        if (scriptSaveRESModel != null)
                                                                //        {
                                                                //            // IF SCRIPT SAVED SUCCEFULLY AND ERROR MESSAGE IS EMPTY THEN 
                                                                //            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                //            // WITH  SUCCESS STATUS
                                                                //            if (scriptSaveRESModel.IsScriptSavedSuccessfully && scriptSaveRESModel.ErrorMsg == String.Empty)
                                                                //            {
                                                                //                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                //                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                //                {
                                                                //                    try
                                                                //                    {
                                                                //                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                //                        int SCRIPT_ID_For_ADD = 0;
                                                                //                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                //                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                //                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                //                        save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                //                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                //                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                //                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                //                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                //                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                //                        dbContext.SaveChanges();

                                                                //                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                //                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                //                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                //                        saveGenerateActionDetails.INIT_ID = null;
                                                                //                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                //                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                //                        saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                //                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                //                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                //                        dbContext.SaveChanges();

                                                                //                        dbContextTransaction.Commit();

                                                                //                        // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                //                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";
                                                                //                    }
                                                                //                    catch (Exception)
                                                                //                    {
                                                                //                        dbContextTransaction.Rollback();
                                                                //                    }
                                                                //                }
                                                                //            }
                                                                //            // IF SCRIPT NOT SAVED SUCCEFULLY AND ERROR MESSAGE IS NOT EMPTY THEN 
                                                                //            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                //            // WITH  FALSE SUCCESS STATUS AND ERROR MESSAGE
                                                                //            else
                                                                //            {
                                                                //                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                //                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                //                {
                                                                //                    try
                                                                //                    {
                                                                //                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                //                        int SCRIPT_ID_For_ADD = 0;
                                                                //                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                //                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                //                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                //                        save_Script_DETAILS.SCRIPT_PATH = String.Empty;
                                                                //                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                //                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                //                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                //                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                //                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                //                        dbContext.SaveChanges();

                                                                //                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                //                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                //                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                //                        saveGenerateActionDetails.INIT_ID = null;
                                                                //                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                //                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                //                        saveGenerateActionDetails.IS_SUCCESSFUL = false;
                                                                //                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        saveGenerateActionDetails.ERROR_DESCRIPTION = scriptSaveRESModel.ErrorMsg;

                                                                //                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                //                        dbContext.SaveChanges();

                                                                //                        dbContextTransaction.Commit();
                                                                //                    }
                                                                //                    catch (Exception)
                                                                //                    {
                                                                //                        dbContextTransaction.Rollback();
                                                                //                    }
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //    }
                                                                //}
                                                            }
                                                        }
                                                    }

                                                }
                                                #endregion


                                                // Data Insertion Details : 
                                                #region Data Insertion Details: 
                                                // MEMORY ALLOCATION FOR Data Insertion Details LIST
                                                resModel.DataInsertionDetailsList = new List<DataInsertionDetails>();

                                                //// LastDocumentID
                                                //var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                                //String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();

                                                //// LastRegistrationID
                                                //var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                                //String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();

                                                //// LastNoticeID
                                                //var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                                //String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();


                                                // BEFORE THIS GENERATE SCRIPT AND SAVE IT ON SERVER AND SAVE ENTRY IN DATABASE
                                                // SET COMMON ITERATION
                                                int ITERATION_ID_INT = 0;
                                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null && x.SCRIPT_EXECUTION_DATETIME == null).FirstOrDefault();
                                                // COMMENETED ABOVE CODE BY SHUBHAM BHAGAT ON 04-07-2020 AT 9:45 PM BECAUSE AFTER APPROVAL AND EXECUTION ABOVE QUERY WILL NOT FETCH ITERATION ID


                                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();
                                                // COMMENTED AND CHANGED CONDITION ON 10-07-2020 AT 3:45 PM 
                                                // COMMENTED ABOVE LINE BECAUSE IT WILL FETCH FIRST ENTRY WE SHOULD FETCH ALL SCRIPTS LIST ASSOCIATED WITH INITID THEN GET MAX SCRIPTID OBJECT FOR OPERATIOSN
                                                List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSList = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                if (dB_RES_INSERT_SCRIPT_DETAILSList != null)
                                                {
                                                    dB_RES_INSERT_SCRIPT_DETAILSList = dB_RES_INSERT_SCRIPT_DETAILSList.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                                    DB_RES_INSERT_SCRIPT_DETAILS latestInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.FirstOrDefault();
                                                    if (latestInsertedScript != null)
                                                    {
                                                        ITERATION_ID_INT = latestInsertedScript.ITERATION_ID ?? 0;

                                                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 05-08-2020 AT 4:00 PM 
                                                        // IF ITERATION ID IS MORE THAN 1 THEN ONLY SHOW SCRIPT RECTIFICATION HISTORY
                                                        if (ITERATION_ID_INT > 1)
                                                        {
                                                            // SKIP CURRENT SCRIPT AND FETCH PREVIOUS SCRIPT OBJECT
                                                            DB_RES_INSERT_SCRIPT_DETAILS previousInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.Skip(1).Take(1).FirstOrDefault();
                                                            if (previousInsertedScript != null)
                                                            {
                                                                // IT WILL ALWAYS CONTAIN 1 ELEMENT BUT WE ARE FETCHING LIST
                                                                List<DB_RES_ACTIONS> scriptRectiHistyList = dbContext.DB_RES_ACTIONS.Where(
                                                                                    x => x.SCRIPT_ID == previousInsertedScript.SCRIPT_ID &&
                                                                                    x.IS_SUCCESSFUL == false).ToList();

                                                                if (scriptRectiHistyList != null)
                                                                {
                                                                    foreach (var item in scriptRectiHistyList)
                                                                    {
                                                                        // CONCATINATING THE SCRIPT RECTIFICATION HISTORY
                                                                        resModel.ScriptRectificationHistory = resModel.ScriptRectificationHistory + ". " + item.CORRECTIVEACTION;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 05-08-2020 AT 4:00 PM


                                                        // ADDED ON 10-07-2020 AT 3:03 PM
                                                        // ADDED FOR FETCHING ERROR MESSAGE FOR TECHADMIN AND SR TO SHOW IN STATUS COLUMN 
                                                        DB_RES_ACTIONS errorInScriptExecution = dbContext.DB_RES_ACTIONS.Where(x => x.SCRIPT_ID == latestInsertedScript.SCRIPT_ID && x.IS_SUCCESSFUL == false && (
                                                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript || x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                                        if (errorInScriptExecution != null)
                                                        {
                                                            IsScriptExecutedWithError = true;
                                                            scriptExecutionErrorMsg = errorInScriptExecution.ERROR_DESCRIPTION;
                                                        }

                                                        DB_RES_ACTIONS successInScriptExecution = dbContext.DB_RES_ACTIONS.Where(x => x.SCRIPT_ID == latestInsertedScript.SCRIPT_ID && x.IS_SUCCESSFUL == true && (
                                                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript || x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                                        if (successInScriptExecution != null)
                                                        {
                                                            IsScriptExecutedSuccessfully = true;
                                                        }
                                                    }
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                // BELOW CODE ADDED TO SHOW ERROR MESSAGE TO TECHADMIN WHEN ERROR
                                                // OCCURED WHILE EXECUTING SCRIPT AT SR OFFICE BY HCL USER
                                                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                                                {
                                                    resModel.ScriptExecutionErrorMsg = IsScriptExecutedWithError ? scriptExecutionErrorMsg : String.Empty;
                                                }
                                                else
                                                {
                                                    resModel.ScriptExecutionErrorMsg = String.Empty;
                                                }


                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                // TO CHANGE HIDE ITERATION NUMBER COLUMN AND ADD SERIAL NO COLUMN
                                                int SerialNoCounter = 1;

                                                // GET DOCUMENT CENTRALIZED AT CENTRALIZED SERVER
                                                //int DocumentID_INT = Convert.ToInt32(DocumentID_STR);
                                                List<DocumentMaster> DocumentMasterList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT && x.SROCode == model.SROfficeID).ToList();
                                                if (DocumentMasterList != null)
                                                {
                                                    if (DocumentMasterList.Count() > 0)
                                                    {
                                                        foreach (var item in DocumentMasterList)
                                                        {
                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                            // FOR DISPLAYING COUNT OF DOCUMENT MASTER IN SUMMARY TABLE
                                                            resModel.Summary_Tbl_DM_Count = DocumentMasterList.Count();

                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Document";
                                                            dataInsertionDetails.RegistrationNumber = item.FinalRegistrationNumber == null ? String.Empty : item.FinalRegistrationNumber;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.Stamp5DateTime == null ? String.Empty : ((DateTime)item.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // GET MARRIAGE CENTRALIZED AT CENTRALIZED SERVER
                                                //int RegistrationID_INT = Convert.ToInt32(RegistrationID_STR);

                                                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                                // COMPARED WITH PSROCode INSTEAD OF SROCode
                                                //List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.SROCode == model.SROfficeID).ToList();
                                                List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.PSROCode == model.SROfficeID).ToList();
                                                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                                if (MarriageRegistrationList != null)
                                                {
                                                    if (MarriageRegistrationList.Count() > 0)
                                                    {
                                                        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                        // FOR DISPLAYING COUNT OF MARRIAGE REGISTRATION IN SUMMARY TABLE
                                                        resModel.Summary_Tbl_MR_Count = MarriageRegistrationList.Count();

                                                        foreach (var item in MarriageRegistrationList)
                                                        {
                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Marriage";
                                                            dataInsertionDetails.RegistrationNumber = item.MarriageCaseNo == null ? String.Empty : item.MarriageCaseNo;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.DateOfRegistration == null ? String.Empty : item.DateOfRegistration.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // GET NOTICE CENTRALIZED AT CENTRALIZED SERVER                                                
                                                //int NoticeID_INT = Convert.ToInt32(NoticeID_STR);

                                                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                                // COMPARED WITH PSROCode INSTEAD OF SROCode
                                                //List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.SROCode == model.SROfficeID).ToList();
                                                List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.PSROCode == model.SROfficeID).ToList();
                                                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                                if (NoticeMasterList != null)
                                                {
                                                    if (NoticeMasterList.Count() > 0)
                                                    {
                                                        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                        // FOR DISPLAYING COUNT OF NOTICE MASTER IN SUMMARY TABLE
                                                        resModel.Summary_Tbl_NM_Count = NoticeMasterList.Count();

                                                        foreach (var item in NoticeMasterList)
                                                        {
                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Notice";
                                                            dataInsertionDetails.RegistrationNumber = item.NoticeNo == null ? String.Empty : item.NoticeNo;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.NoticeIssuedDate == null ? String.Empty : item.NoticeIssuedDate.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                IQueryable<ReceiptMaster> receiptMasterRecords = dbContext.ReceiptMaster.Where(x => x.ReceiptID > OtherReceiptID_LONG && x.SROCode == model.SROfficeID);
                                                // FOR DISPLAYING COUNT OF Other Receipts IN SUMMARY TABLE
                                                resModel.Summary_Tbl_OtherReceipt_Count = receiptMasterRecords.Count();
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                #endregion
                                            }
                                        }

                                        //DateTime actionDateTime = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                        //x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).FirstOrDefault().Select(new { a=x.ACTION_DATETIME
                                        //});
                                        //var actionDateTime = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                        //x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).Select(x=>new
                                        //{
                                        //    a = x.ACTION_DATETIME
                                        //});
                                    }
                                }
                            }


                        }

                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                        // ADDED BELOW CODE ON 15-07-2020 AT 07:01 PM
                        // ADDED BELOW CODE TO SHOW ERROR MESSAGE WHEN ERROR OCCURED WHILE RESTORING 
                        // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        else if (dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated)
                        //else if (dB_RES_INITIATE_MASTER.STATUS_ID == null)
                        // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        {
                            //dB_RES_INITIATE_MASTER.
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where
                            //    (x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                            //    x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID &&
                            //    x.IS_SUCCESSFUL == false && x.ERROR_DESCRIPTION != null).ToList();



                            #region BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                            // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                            // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR

                            //// FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            //// ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            //// WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            //// TIMES THEN WE SHOULD FETCH LASTET 6 ROWS FROM ACTIONS TABLE
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where
                            // (x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                            // x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID &&
                            // x.IS_SUCCESSFUL == false).ToList();



                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                            // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val

                            //// FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            //// ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            //// WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            //// TIMES THEN WE SHOULD FETCH LASTET 7 ROWS FROM ACTIONS TABLE
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                            //                                        x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID 
                            //                                            &&
                            //                                            (
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema
                            //                                            )
                            //                                            &&  x.IS_SUCCESSFUL == false).ToList();

                            // FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            // ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            // WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            // TIMES THEN WE SHOULD FETCH LASTET 10 ROWS FROM ACTIONS TABLE
                            List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                                                                    x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID
                                                                        &&
                                                                        (
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                                                        )
                                                                        && x.IS_SUCCESSFUL == false).ToList();

                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021


                            #endregion ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                            if (db_RES_ACTIONSList != null)
                            {
                                if (db_RES_ACTIONSList.Count > 0)
                                {
                                    // ORDER BY DESCENDING
                                    db_RES_ACTIONSList = db_RES_ACTIONSList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                    // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                    
                                    //// BEFORE TAKING TOP 6 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 6 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 6)
                                    //{
                                    //    // TAKING TOP 6 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(6).ToList();
                                    //}

                                    // BEFORE TAKING TOP 7 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 7 ELEMENTS
                                    if (db_RES_ACTIONSList.Count > 7)
                                    {
                                        // TAKING TOP 7 ELEMENTS 
                                        db_RES_ACTIONSList = db_RES_ACTIONSList.Take(7).ToList();
                                    }
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                                    resModel.DatabaseNotRestoredErrorMsg = "<label style='font-size:initial;font-weight:bold;'>";
                                    foreach (var item in db_RES_ACTIONSList)
                                    {
                                        if (!String.IsNullOrEmpty(item.ERROR_DESCRIPTION))
                                        {
                                            resModel.IsDatabaseNotRestored = true;
                                            resModel.DatabaseNotRestoredErrorMsg = resModel.DatabaseNotRestoredErrorMsg + " " + item.ERROR_DESCRIPTION;
                                        }
                                    }
                                    resModel.DatabaseNotRestoredErrorMsg = resModel.DatabaseNotRestoredErrorMsg + " .</label>";
                                }
                            }
                        }
                        // ADDED ABOVE CODE TO SHOW ERROR MESSAGE WHEN ERROR OCCURED WHILE RESTORING

                        #endregion
                    }
                    else// IS SAME AS BELOW ELSE -IT WILL EXECUTE WHEN NO ENTRY EXIST IN DB FOR PARTICULAR SRO EXIST
                    {
                        // checked below code, it is running
                        //Initiate Date
                        //resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                        //Initiate Date btn
                        //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";

                        // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                            resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                        else
                            resModel.InitiationDateBtn = String.Empty;

                        //Generate Key Btn
                        //resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                        //Generate Key Value
                        //resModel.GenerateKeyValue = GenerateKey();//1
                        // delete space after key and span close
                        //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days only Please share this activation key to support Engineer.</label>";

                        // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                            resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                        else
                            resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                        resModel.ShowInitDateAndGeneratedKeyMsg = false;
                    }
                }
                else// IS SAME AS ABOVE ELSE 
                {
                    //Initiate Date
                    //resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                    //Initiate Date btn
                    //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";

                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                    else
                        resModel.InitiationDateBtn = String.Empty;

                    //Generate Key Btn
                    //resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                    //Generate Key Value
                    //resModel.GenerateKeyValue = GenerateKey();//1
                    //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days only Please share this activation key to support Engineer.</label>";

                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                    else
                        resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                    resModel.ShowInitDateAndGeneratedKeyMsg = false;
                }

                // POPULATE DOWNLOAD BUTTON FOR TECHADMIN IF ERROR OCCURED WHILE EXECUTING SCRIPT
                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                {
                    // CHECK FOR PARTICULAR SRO IN DB_RES_INITIATE_MASTER TABLE WHERE STATUS_ID=3  I.E ErrorInDataInsertion
                    DB_RES_INITIATE_MASTER db_initMasterErrorInInsertion = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.SROCODE == model.SROfficeID && x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.ErrorInDataInsertion).FirstOrDefault();
                    if (db_initMasterErrorInInsertion != null)
                    {
                        // GET THE LIST OF DB_RES_INSERT_SCRIPT_DETAILS USING INIT_ID AND SCRIPT_CREATION_DATETIME !=NULL AND SCRIPT_APPROVAL_DATETIME!=NULL
                        // WE HAVE TO FETCH LIST BECAUSE ERROR CAN OCCUR IN RECTIFIED SCRIPT ALSO
                        //List<DB_RES_INSERT_SCRIPT_DETAILS> db_insert_script_for_recti_List = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == db_initMasterErrorInInsertion.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME != null).ToList();
                        // ABOVE CODE FOR LISTING IS COMMENTED BY SHUBHAM BHAGAT ON 10-07-2020 AT 2:45 PM 
                        // BECAUSE AFTER RECTIFIED SCRIPT IS DOWNLOADED AND RECTIFIED IT SHOULD NOT BE DOWNLOADED AND UPLOADED ANOTHER TIME
                        List<DB_RES_INSERT_SCRIPT_DETAILS> db_insert_script_for_recti_List = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == db_initMasterErrorInInsertion.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                        if (db_insert_script_for_recti_List != null)
                        {
                            if (db_insert_script_for_recti_List.Count() > 0)
                            {
                                // AFTER GETTING LIST FETCH THE LATEST SCRIPT_ID FOR THE INIT_ID
                                int maxScriptID = db_insert_script_for_recti_List.Max(x => x.SCRIPT_ID);

                                // AFTER GETTING LATEST SCRIPT_ID CHECK IN DB_RES_ACTIONS TABLE FOR LATEST SCRIPT_ID WHERE ACTIONID = ExecuteScript OR ExecuteRecitifiedScript
                                DB_RES_ACTIONS errMSGActionModel = dbContext.DB_RES_ACTIONS.Where
                                    (x => x.SCRIPT_ID == maxScriptID && x.IS_SUCCESSFUL == false &&
                                    (x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript ||
                                    x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                if (errMSGActionModel != null)
                                {
                                    resModel.DownloadScriptForTechAdmin = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=DownloadScriptForRectification('" + db_initMasterErrorInInsertion.INIT_ID + "','" + maxScriptID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Download</button>";
                                    resModel.UploadScriptForTechAdmin = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=UploadRectifiedScript('" + db_initMasterErrorInInsertion.INIT_ID + "','" + maxScriptID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Upload</button>";
                                }
                            }
                        }
                    }
                }

                //Pankaj work here
                #region ADDED BY PANKAJ ON 01-07-2020
                #region ADDED BY PANKAJ ON 31-07-2020
                var id = model.INIT_ID_INT;

                #region BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR
                //IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == id && x.ACTION_ID < (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript).OrderBy(X => X.ROW_ID);

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                //IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(
                //                                            x => x.INIT_ID == id &&
                //                                               (
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema

                //                                               )).OrderBy(X => X.ROW_ID);
               


                IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(
                                            x => x.INIT_ID == id &&
                                               (
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema||                                                   
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                               )).OrderBy(X => X.ROW_ID);
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021

                #endregion ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                List<DB_RES_ACTIONS_MASTER> dB_RES_ACTIONS_MASTERsList = dbContext.DB_RES_ACTIONS_MASTER.ToList();
                // List<DB_RES_ACTIONS> dB_RES_ACTIONSListForErrorHistory = new List<DB_RES_ACTIONS>();
                List<DB_RES_ACTIONS_CLASS> DB_RES_ACTIONS_CLASS = new List<DB_RES_ACTIONS_CLASS>();
                if (dB_RES_ACTIONS != null)
                {
                    foreach (DB_RES_ACTIONS db in dB_RES_ACTIONS)
                    {
                        if (db.ERROR_DESCRIPTION != null)
                        {
                            DB_RES_ACTIONS_CLASS.Add(new DB_RES_ACTIONS_CLASS
                            {
                                RowId = db.ROW_ID,
                                InitId = db.INIT_ID == null ? 0 : (int)db.INIT_ID,
                                ActionId = db.ACTION_ID == null ? 0 : (int)db.ACTION_ID,
                                ActionDateTime = db.ACTION_DATETIME.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                                IsSuccessful = db.IS_SUCCESSFUL,
                                ScriptId = db.SCRIPT_ID == null ? 0 : (int)db.SCRIPT_ID,
                                ErrorDescription = db.ERROR_DESCRIPTION,
                                CorrectiveAction = db.CORRECTIVEACTION,
                                ActionDescription = dB_RES_ACTIONS_MASTERsList.Where(x => x.ACTION_ID == db.ACTION_ID).Select(x => x.ACTION_DESCRIPTION).FirstOrDefault()
                            }); ;
                        }
                    }
                }
                resModel.dB_RES_ACTIONsForErrorHistory = DB_RES_ACTIONS_CLASS;
                #endregion
                resModel.dB_RES_TABLE_WISE_COUNT_List = null;
                //var SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == model.SROfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //IQueryable<DB_RES_INITIATE_MASTER> iQueryableOfInitiateMaster =  dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID);
                //var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();

                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-07-2020 AT 3:20 PM
                // FOR CONSIDERING NEW REQUEST ALSO
                var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(
                        x =>
                            (
                                x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification ||
                                x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified
                            ) &&
                            x.IS_DRO == false &&
                            x.SROCODE == model.SROfficeID &&
                            x.INIT_ID == model.INIT_ID_INT
                            ).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();
                //var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(x => (x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification || x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified) && x.IS_DRO == false && x.SROCODE == model.SROfficeID).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();
                if (InitId > 0)
                {

                    resModel.IsCompleted = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID && x.CONFIRM_DATETIME == null).OrderByDescending(x => x.INIT_ID).Select(x => x.IS_COMPLETED).FirstOrDefault();

                    var ScriptId = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == InitId).Select(x => x.SCRIPT_ID).FirstOrDefault();

                    //populate confirm button for sro if IsCompleted is 1
                    if (resModel.IsCompleted)
                    {
                        // resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                        // ADDED BELOW QUERY AND IF CONDITION BY SHUBHAM BHAGAT ON 29-07-2020 AT 8:30 PM
                        // FOR POPULATING CONFIRM BUTTON IF NOT CONFIRMED
                        var CONFIRM_DATETIME_IsNullOrNot = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == InitId).Select(x => x.CONFIRM_DATETIME).FirstOrDefault();
                        if (CONFIRM_DATETIME_IsNullOrNot == null)
                            resModel.ConfirmBtn = "<button type = 'button' style='width:10%', class='btn btn-group-md btn-success' onclick=ConfirmDataInsertion('" + InitId + "','" + model.SROfficeID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Confirm</button>";
                    }

                    else
                    {
                        DataRestorationReportReqModel mdl = new DataRestorationReportReqModel();
                        mdl.INITID_INT = InitId;
                        mdl.SROfficeID = model.SROfficeID;
                        DataRestorationReportResModel mdl2 = ConfirmDataInsertion(mdl);
                        resModel.ConfirmBtnMsg = mdl2.DataInsertionConfrimationMsg;
                    }

                    // ADDED BY PANKAJ ON 16-07-2020 
                    // TO DISPLAY CONCLUSION SECTION IN BOTH CASES IF DATA IS INSERTED AND PENDING FOR VERIFICATION OR DATA IS INSERTED AND VERIFIED
                    resModel.IsConclusionToDisplay = true;
                    //InitId
                    DB_RES_INITIATE_MASTER confirmMsgInitMaster = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == InitId).FirstOrDefault();
                    if (confirmMsgInitMaster.CONFIRM_DATETIME == null)
                    {
                        resModel.ConclusionMsg = "<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data restoration process completed successfully kindly verify and confirm.</label>";
                    }
                    else
                    {
                        resModel.ConclusionMsg = "<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data Restoration verified and confirmed by SR on date : " + ((DateTime)confirmMsgInitMaster.CONFIRM_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "</label>";
                    }
                    //var result = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == InitId).ToList();
                    //List<DB_RES_SERVICE_COMM_DETAILS> DB_RES_SERVICE_COM_DETAILS_LIST = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == InitId).ToList();

                    //// below code is for count(*) queries
                    //if(DB_RES_SERVICE_COM_DETAILS_LIST.Count > 0)
                    //{
                    //    int LastDocumentID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE).FirstOrDefault());
                    //    int LastMarriageID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE).FirstOrDefault());
                    //    int LastNoticeID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE).FirstOrDefault());

                    //    resModel.DocumentRegistrationsToBeRestored = dbContext.DocumentMaster.Where(x => x.DocumentID > LastDocumentID).Count();
                    //    resModel.MarriageRegistratiosToBeRestored = dbContext.MarriageRegistration.Where(x => x.RegistrationID > LastMarriageID).Count();
                    //    resModel.NoticeRegistrationsToBeRestored = dbContext.NoticeMaster.Where(x => x.NoticeID > LastNoticeID).Count();
                    //}


                    List<DB_RES_TABLE_MASTER> dB_RES_TABLE_MASTERs = dbContext.DB_RES_TABLE_MASTER.ToList();
                    List<DB_RES_TABLEWISE_COUNT> dB_RES_TABLEWISE_COUNTs = dbContext.DB_RES_TABLEWISE_COUNT.Where(x => x.INIT_ID == InitId).ToList();
                    List<DB_RES_TABLE_WISE_COUNT> newList = new List<DB_RES_TABLE_WISE_COUNT>();
                    if (dB_RES_TABLEWISE_COUNTs.Count > 0)
                    {

                        foreach (var a in dB_RES_TABLEWISE_COUNTs)
                        {
                            newList.Add(new DB_RES_TABLE_WISE_COUNT
                            {
                                RowID = a.ROWID,
                                InitId = a.INIT_ID,
                                TableId = a.TABLE_ID,
                                RowsToBeInserted = a.ROWS_TO_BE_INSERTED == null ? 0 : (int)a.ROWS_TO_BE_INSERTED,
                                RowsInserted = a.ROWS_INSERTED == null ? 0 : (int)a.ROWS_INSERTED,
                                TableName = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_ID == a.TABLE_ID).Select(x => x.TABLE_NAME).FirstOrDefault()
                            }); ;
                        }
                    }
                    resModel.dB_RES_TABLE_WISE_COUNT_List = newList;

                    //for data verification details table in dataRestorationReportStatus
                    if (dB_RES_TABLEWISE_COUNTs.Count > 0)
                    {
                        int docMasterTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "DocumentMaster").Select(x => x.TABLE_ID).FirstOrDefault();
                        int marriageRegTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "MarriageRegistration").Select(x => x.TABLE_ID).FirstOrDefault();
                        int noticeMasterTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "NoticeMaster").Select(x => x.TABLE_ID).FirstOrDefault();

                        var docToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == docMasterTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();
                        var marToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == marriageRegTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();
                        var notToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == noticeMasterTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();

                        resModel.DocumentRegistrationsToBeRestored = docToBeRestored == null ? 0 : (int)docToBeRestored;
                        resModel.MarriageRegistratiosToBeRestored = marToBeRestored == null ? 0 : (int)marToBeRestored;
                        resModel.NoticeRegistrationsToBeRestored = notToBeRestored == null ? 0 : (int)notToBeRestored;

                        var docRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == docMasterTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();
                        var marRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == marriageRegTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();
                        var notRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == noticeMasterTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();

                        resModel.DocumentRegistratiosRestored = docRestored == null ? 0 : (int)docRestored;
                        resModel.MarriageRegistrationsRestored = marRestored == null ? 0 : (int)marRestored;
                        resModel.NoticeRegistrationsRestored = notRestored == null ? 0 : (int)notRestored;
                    }

                }
                #endregion

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return resModel;

                ////Initiate Date
                ////resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                ////Initiate Date btn
                ////resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";
                //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                ////Generate Key Btn
                ////resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                ////Generate Key Value
                ////resModel.GenerateKeyValue = GenerateKey();//1
                //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Your activation key is: <span id='GenerateKeyValueSpanID'>" + GenerateKey() + " </span>which is valid for 2 days only Please share this activation key to support person.</label>";

                #region extra
                //var officeID = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                //     Select(y => y.OfficeID).FirstOrDefault();

                //var userID = dbContext.UMG_UserDetails.Where(z => z.OfficeID == officeID).Select(p => p.UserID).FirstOrDefault();

                //var SRName = dbContext.UMG_UserProfile.Where(q =>q.UserID == userID).Select(r => r.FirstName + r.LastName).FirstOrDefault();

                //var SRName1 = dbContext.UMG_UserProfile.Where(q => q.UserID == dbContext.UMG_UserDetails.
                //Where(z => z.OfficeID == dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                //Select(y => y.OfficeID).FirstOrDefault()).Select(p => p.UserID).FirstOrDefault()).
                //Select(r => r.FirstName + r.LastName).FirstOrDefault();
                #endregion

                
            }

            catch (Exception ex) {
                throw ex; }
            finally { if (dbContext != null) dbContext.Dispose(); }
        }

        /// <summary>
        /// InitiateDatabaseRestoration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel InitiateDatabaseRestoration(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-InitiateDatabaseRestoration-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                //string hashPwdAfterEncryAndAddSalt = SHA512ChecksumWrapper.ComputeHash(EncryptKeyorOTP(model.GenerateKeyValue), GenerateSalt());
                string hashPwdAfterEncryAndAddSalt = Encrypt(model.GenerateKeyValue);
                dbContext = new KaveriEntities();
                //String EncryptedKey = EncryptKeyorOTP(model.GenerateKeyValue);
                // Insert entries in two tables in transaction

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020 AT 5:55 PM
                // FOR MOBILE NUMBER VALIDATION
                int OfficeMaster_OfficeID = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).Select(x => x.OfficeID).FirstOrDefault();
                long userID = dbContext.UMG_UserDetails.Where(x => x.OfficeID == OfficeMaster_OfficeID).Select(x => x.UserID).FirstOrDefault();
                UMG_UserProfile userProfile = dbContext.UMG_UserProfile.Where(x => x.UserID == userID).FirstOrDefault();
                if (userProfile != null)
                {
                    if (String.IsNullOrEmpty(userProfile.MobileNumber))
                    {
                        resModel.IsDataSavedSuccefully = false;
                        resModel.InitiationProcessStartedOrNotMSG = @"Please go to your profile and update your mobile number first,
                            because this mobile number will be used in otp verfification.";
                        return resModel;
                    }
                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020 AT 5:55 PM

                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {


                        int initID;

                        // SAVE INITIATION DETAILS
                        DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = new DB_RES_INITIATE_MASTER();
                        initID = (dbContext.DB_RES_INITIATE_MASTER.Any() ? dbContext.DB_RES_INITIATE_MASTER.Max(a => a.INIT_ID) : 0) + 1;
                        dB_RES_INITIATE_MASTER.INIT_ID = initID;
                        dB_RES_INITIATE_MASTER.SROCODE = model.SROfficeID;
                        dB_RES_INITIATE_MASTER.INIT_DATE = model.InitiationDate;
                        // BELOW CODE COMMENTED AND CHANGED ON 01-08-2020 BY SHUBHAM BHAGAT
                        //dB_RES_INITIATE_MASTER.STATUS_ID = null;
                        dB_RES_INITIATE_MASTER.STATUS_ID = (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated;
                        dB_RES_INITIATE_MASTER.IS_COMPLETED = false;
                        dB_RES_INITIATE_MASTER.DROCODE = null;
                        dB_RES_INITIATE_MASTER.IS_DRO = false;

                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                        dB_RES_INITIATE_MASTER.COMPLETE_DATETIME = null;
                        dB_RES_INITIATE_MASTER.CONFIRM_DATETIME = null;

                        dbContext.DB_RES_INITIATE_MASTER.Add(dB_RES_INITIATE_MASTER);
                        dbContext.SaveChanges();

                        // SAVE KEY DETAILS
                        DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = new DB_RES_ACTIVATION_KEY_OTP();
                        dB_RES_ACTIVATION_KEY_OTP.KEYID = (dbContext.DB_RES_ACTIVATION_KEY_OTP.Any() ? dbContext.DB_RES_ACTIVATION_KEY_OTP.Max(a => a.KEYID) : 0) + 1;
                        dB_RES_ACTIVATION_KEY_OTP.INIT_ID = initID;
                        dB_RES_ACTIVATION_KEY_OTP.KEY_TYPE = (int)ApiCommonEnum.DB_RES_KeyType.Key;
                        dB_RES_ACTIVATION_KEY_OTP.KEYVALUE = hashPwdAfterEncryAndAddSalt;
                        dB_RES_ACTIVATION_KEY_OTP.IS_ACTIVE = true;
                        dB_RES_ACTIVATION_KEY_OTP.KEY_DATETIME = DateTime.Now;
                        dbContext.DB_RES_ACTIVATION_KEY_OTP.Add(dB_RES_ACTIVATION_KEY_OTP);
                        dbContext.SaveChanges();

                        dbContextTransaction.Commit();
                        resModel.IsDataSavedSuccefully = true;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        resModel.IsDataSavedSuccefully = false;
                        throw;
                    }
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-InitiateDatabaseRestoration-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            }
            catch (Exception)
            {
                resModel.IsDataSavedSuccefully = false;
                throw;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }

            return resModel;
        }

        /// <summary>
        /// GenerateKey
        /// </summary>
        /// <returns>returns generated key string</returns>
        public static string GenerateKey()
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-GenerateKey-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            string OTP = "";
            int i = 0;
            //for alphanumberi otp
            string Alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int randomAlpha = 0;
            bool[] IsAlpabetAlreadyGenerated = new bool[35];
            for (i = 0; i < IsAlpabetAlreadyGenerated.Length; i++)
            {
                IsAlpabetAlreadyGenerated[i] = false;
            }

            for (i = 0; i < 8; i++)
            {
                randomAlpha = new Random().Next(0, 35);

                while (IsAlpabetAlreadyGenerated[randomAlpha])
                {
                    randomAlpha = new Random().Next(0, 35);
                }

                OTP = OTP + Alphabets[randomAlpha];
                IsAlpabetAlreadyGenerated[randomAlpha] = true;
            }

            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-GenerateKey-OUT");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            return OTP;
        }

        //public String EncryptKeyorOTP(String KeyorOTP)
        //{
        //    string fw = SHA512Checksum.CalculateSHA512Hash(KeyorOTP);
        //    return fw;
        //}

        //public string GenerateSalt()
        //{
        //    string EncryptionKey = "Gxv@#123";
        //    string ecy = SHA512Checksum.CalculateSHA512Hash(EncryptionKey);
        //    return ecy;
        //}


        /// <summary>
        /// encryptString
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns>returns encrypted string</returns>
        public static string Encrypt(string encryptString)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-Encrypt-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            string EncryptionKey = "Gxv@#123";  //we can change the code converstion key as per our requirement    
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString.Trim());
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }

            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-Encrypt-OUT");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            return encryptString;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns>returns Decrypted string</returns>
        public static string Decrypt(string cipherText)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-Decrypt-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            string EncryptionKey = "Gxv@#123";  //we can change the code converstion key as per our requirement, but the decryption key should be same as encryption key    
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-Decrypt-OUT");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            return cipherText;
        }

        /// <summary>
        /// GenerateKeyAfterExpiration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel GenerateKeyAfterExpiration(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-GenerateKeyAfterExpiration-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();
                String GenerateKey_STR = GenerateKey();
                String GenerateKey_STR_Encrypted = Encrypt(GenerateKey_STR);
                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-GenerateKeyAfterExpiration-internal try-IN");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        
                        // SET ACTIVATION KEY ACTIVE STATUS TO 0 FOR MAKING PREVIOUS KEY INACTIVE
                        DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTPExpired = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == model.INITID_INT && x.KEYID == model.KEYID_INT && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        dB_RES_ACTIVATION_KEY_OTPExpired.IS_ACTIVE = false;
                        dbContext.SaveChanges();

                        // SAVE NEW KEY DETAILS
                        DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = new DB_RES_ACTIVATION_KEY_OTP();
                        dB_RES_ACTIVATION_KEY_OTP.KEYID = (dbContext.DB_RES_ACTIVATION_KEY_OTP.Any() ? dbContext.DB_RES_ACTIVATION_KEY_OTP.Max(a => a.KEYID) : 0) + 1;
                        dB_RES_ACTIVATION_KEY_OTP.INIT_ID = model.INITID_INT;
                        dB_RES_ACTIVATION_KEY_OTP.KEY_TYPE = (int)ApiCommonEnum.DB_RES_KeyType.Key;
                        dB_RES_ACTIVATION_KEY_OTP.KEYVALUE = GenerateKey_STR_Encrypted;
                        dB_RES_ACTIVATION_KEY_OTP.IS_ACTIVE = true;
                        dB_RES_ACTIVATION_KEY_OTP.KEY_DATETIME = DateTime.Now;
                        dbContext.DB_RES_ACTIVATION_KEY_OTP.Add(dB_RES_ACTIVATION_KEY_OTP);
                        dbContext.SaveChanges();

                        dbContextTransaction.Commit();
                        resModel.IsKeyGeneratedSuccefully = true;
                        //resModel.GeneratedKeyWithMsgAfterExpiration = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Your activation key is: " + GenerateKey_STR + " which is valid for 5 days Please share this activation key to support Engineer.</label>";
                        resModel.GeneratedKeyWithMsgAfterExpiration = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Your activation key is: " + GenerateKey_STR + " which is valid for 5 days Please share this activation key to Support Engineer.</label>";

                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-GenerateKeyAfterExpiration-internal try-OUT");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        resModel.IsKeyGeneratedSuccefully = false;
                        throw;
                    }
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-GenerateKeyAfterExpiration-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            }
            catch (Exception)
            {
                resModel.IsKeyGeneratedSuccefully = false;
                throw;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }

            return resModel;
        }

        /// <summary>
        /// ApproveScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel ApproveScript(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();
            try
            {       
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-ApproveScript-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-ApproveScript-InternalTry-IN");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                        // UPDATE SCRIPT APPROVE DATETIME FROM NULL TO DATETIME.NOW
                        DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT && x.INIT_ID == model.INITID_INT && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null).FirstOrDefault();
                        if (dB_RES_INSERT_SCRIPT_DETAILS != null)
                        {
                            // UPDATE SCRIPT APPROVE DATETIME
                            dB_RES_INSERT_SCRIPT_DETAILS.SCRIPT_APPROVAL_DATETIME = DateTime.Now;

                            dbContext.SaveChanges();

                            // ADD ENTRY IN DB_RES_ACTIONS TABLE
                            DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                            saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                            saveGenerateActionDetails.INIT_ID = null;
                            saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ApproveScript;
                            saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                            saveGenerateActionDetails.IS_SUCCESSFUL = true;
                            saveGenerateActionDetails.SCRIPT_ID = model.SCRIPT_ID_INT;
                            saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                            dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                            dbContext.SaveChanges();

                            dbContextTransaction.Commit();

                            resModel.IsScriptApprovedSuccefully = true;
                            resModel.ScriptApprovedMSG = "Scripts approved";
                            // COMMENTED AND CHANGED ON 07-08-2020 AT 4:30 PM
                            resModel.ApproveBtnORMessage = "<label style='font-size:initial;font-weight:bold;'>Please ask system integrator to execute restoration utility to restore above data.</label>";
                            //resModel.ApproveBtnORMessage = "<label style='font-size:initial;font-weight:bold;'>Please ask support engineer to execute restoration utility to restore above data.</label>";
                        }
                        else
                        {
                            resModel.IsScriptApprovedSuccefully = false;
                            resModel.ScriptApprovedMSG = "Scripts not approved";
                        }

                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-ApproveScript-InternalTry-OUT");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        resModel.IsScriptApprovedSuccefully = false;
                        resModel.ScriptApprovedMSG = "Scripts not approved";
                        throw;
                    }
                }
                
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-ApproveScript-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            }
            catch (Exception)
            {
                resModel.IsScriptApprovedSuccefully = false;
                resModel.ScriptApprovedMSG = "Scripts not approved";
                throw;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }

            return resModel;
        }

        /// <summary>
        /// DataInsertionTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel DataInsertionTable(DataRestorationReportReqModel model)
        {
            DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel()
            {
                DataInsertionDetailsList = new List<DataInsertionDetails>()
            };
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataInsertionTable-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                if (model.IsRectifiedScriptUploaded)
                {
                    //model.SCRIPT_ID_INT
                    //     model.INITID_INT
                    DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == model.INITID_INT && x.IS_SUCCESSFUL == false && x.SCRIPT_APPROVAL_DATETIME == null && x.SCRIPT_EXECUTION_DATETIME == null).FirstOrDefault();
                    if (dB_RES_INSERT_SCRIPT_DETAILS != null)
                    {
                        List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == model.INITID_INT).ToList();
                        if (db_RES_SERVICE_COMM_DETAILSList != null)
                        {
                            if (db_RES_SERVICE_COMM_DETAILSList.Count() > 0)
                            {
                                int ITERATION_ID_INT = 0;
                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null && x.SCRIPT_EXECUTION_DATETIME == null).FirstOrDefault();
                                // COMMENETED ABOVE CODE BY SHUBHAM BHAGAT ON 04-07-2020 AT 9:45 PM BECAUSE AFTER APPROVAL AND EXECUTION ABOVE QUERY WILL NOT FETCH ITERATION ID

                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == model.INITID_INT && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();

                                // COMMENTED AND CHANGED CONDITION ON 10-07-2020 AT 3:45 PM 
                                // COMMENTED ABOVE LINE BECAUSE IT WILL FETCH FIRST ENTRY WE SHOULD FETCH ALL SCRIPTS LIST ASSOCIATED WITH INITID THEN GET MAX SCRIPTID OBJECT FOR OPERATIOSN
                                List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSList = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == model.INITID_INT && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                if (dB_RES_INSERT_SCRIPT_DETAILSList != null)
                                {
                                    dB_RES_INSERT_SCRIPT_DETAILSList = dB_RES_INSERT_SCRIPT_DETAILSList.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                    DB_RES_INSERT_SCRIPT_DETAILS latestInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.FirstOrDefault();
                                    if (latestInsertedScript != null)
                                    {
                                        ITERATION_ID_INT = latestInsertedScript.ITERATION_ID ?? 0;
                                    }

                                    // LastDocumentID
                                    var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                    String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();
                                    long DocumentID_INT = Convert.ToInt64(DocumentID_STR);

                                    // LastRegistrationID
                                    var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                    String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();
                                    long RegistrationID_INT = Convert.ToInt64(RegistrationID_STR);

                                    // LastNoticeID
                                    var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                    String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();
                                    long NoticeID_INT = Convert.ToInt64(NoticeID_STR);

                                    // FIND SRO CODE USING INIT ID 
                                    int SROCode = 0;
                                    DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INITID_INT).FirstOrDefault();
                                    if (dB_RES_INITIATE_MASTER != null)
                                    {
                                        SROCode = dB_RES_INITIATE_MASTER.SROCODE ?? 0;
                                    }

                                    // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                    // TO CHANGE HIDE ITERATION NUMBER COLUMN AND ADD SERIAL NO COLUMN
                                    int SerialNoCounter = 1;

                                    // GET DOCUMENT CENTRALIZED AT CENTRALIZED SERVER
                                    //int DocumentID_INT = Convert.ToInt32(DocumentID_STR);
                                    List<DocumentMaster> DocumentMasterList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT && x.SROCode == SROCode).ToList();
                                    if (DocumentMasterList != null)
                                    {
                                        if (DocumentMasterList.Count() > 0)
                                        {
                                            foreach (var item in DocumentMasterList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Document";
                                                dataInsertionDetails.RegistrationNumber = item.FinalRegistrationNumber == null ? String.Empty : item.FinalRegistrationNumber;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.Stamp5DateTime == null ? String.Empty : ((DateTime)item.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Pending for Approval";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }

                                        }
                                    }

                                    // GET MARRIAGE CENTRALIZED AT CENTRALIZED SERVER
                                    //int RegistrationID_INT = Convert.ToInt32(RegistrationID_STR);

                                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                    // COMPARED WITH PSROCode INSTEAD OF SROCode
                                    //List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.SROCode == SROCode).ToList();
                                    List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.PSROCode == SROCode).ToList();
                                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                    if (MarriageRegistrationList != null)
                                    {
                                        if (MarriageRegistrationList.Count() > 0)
                                        {
                                            foreach (var item in MarriageRegistrationList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Marriage";
                                                dataInsertionDetails.RegistrationNumber = item.MarriageCaseNo == null ? String.Empty : item.MarriageCaseNo;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.DateOfRegistration == null ? String.Empty : item.DateOfRegistration.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Pending for Approval";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }
                                        }
                                    }

                                    // GET NOTICE CENTRALIZED AT CENTRALIZED SERVER                                                
                                    //int NoticeID_INT = Convert.ToInt32(NoticeID_STR);

                                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                    // COMPARED WITH PSROCode INSTEAD OF SROCode
                                    //List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.SROCode == SROCode).ToList();
                                    List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.PSROCode == SROCode).ToList();
                                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                    if (NoticeMasterList != null)
                                    {
                                        if (NoticeMasterList.Count() > 0)
                                        {
                                            foreach (var item in NoticeMasterList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Notice";
                                                dataInsertionDetails.RegistrationNumber = item.NoticeNo == null ? String.Empty : item.NoticeNo;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.NoticeIssuedDate == null ? String.Empty : item.NoticeIssuedDate.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Pending for Approval";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }
                                        }
                                    }



                                }
                            }
                        }
                    }
                }
                else
                {
                    DB_RES_ACTIONS dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT && x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ApproveScript).FirstOrDefault();
                    if (dB_RES_ACTIONS != null)
                    {
                        List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == model.INITID_INT).ToList();
                        if (db_RES_SERVICE_COMM_DETAILSList != null)
                        {
                            if (db_RES_SERVICE_COMM_DETAILSList.Count() > 0)
                            {
                                int ITERATION_ID_INT = 0;
                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null && x.SCRIPT_EXECUTION_DATETIME == null).FirstOrDefault();
                                // COMMENETED ABOVE CODE BY SHUBHAM BHAGAT ON 04-07-2020 AT 9:45 PM BECAUSE AFTER APPROVAL AND EXECUTION ABOVE QUERY WILL NOT FETCH ITERATION ID

                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == model.INITID_INT && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();

                                // COMMENTED AND CHANGED CONDITION ON 10-07-2020 AT 3:45 PM 
                                // COMMENTED ABOVE LINE BECAUSE IT WILL FETCH FIRST ENTRY WE SHOULD FETCH ALL SCRIPTS LIST ASSOCIATED WITH INITID THEN GET MAX SCRIPTID OBJECT FOR OPERATIOSN
                                List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSList = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == model.INITID_INT && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                if (dB_RES_INSERT_SCRIPT_DETAILSList != null)
                                {
                                    dB_RES_INSERT_SCRIPT_DETAILSList = dB_RES_INSERT_SCRIPT_DETAILSList.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                    DB_RES_INSERT_SCRIPT_DETAILS latestInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.FirstOrDefault();
                                    if (latestInsertedScript != null)
                                    {
                                        ITERATION_ID_INT = latestInsertedScript.ITERATION_ID ?? 0;
                                    }

                                    // LastDocumentID
                                    var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                    String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();
                                    long DocumentID_INT = Convert.ToInt64(DocumentID_STR);

                                    // LastRegistrationID
                                    var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                    String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();
                                    long RegistrationID_INT = Convert.ToInt64(RegistrationID_STR);

                                    // LastNoticeID
                                    var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                    String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();
                                    long NoticeID_INT = Convert.ToInt64(NoticeID_STR);

                                    // FIND SRO CODE USING INIT ID 
                                    int SROCode = 0;
                                    DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INITID_INT).FirstOrDefault();
                                    if (dB_RES_INITIATE_MASTER != null)
                                    {
                                        SROCode = dB_RES_INITIATE_MASTER.SROCODE ?? 0;
                                    }

                                    // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                    // TO CHANGE HIDE ITERATION NUMBER COLUMN AND ADD SERIAL NO COLUMN
                                    int SerialNoCounter = 1;

                                    // GET DOCUMENT CENTRALIZED AT CENTRALIZED SERVER
                                    //int DocumentID_INT = Convert.ToInt32(DocumentID_STR);
                                    List<DocumentMaster> DocumentMasterList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT && x.SROCode == SROCode).ToList();
                                    if (DocumentMasterList != null)
                                    {
                                        if (DocumentMasterList.Count() > 0)
                                        {
                                            foreach (var item in DocumentMasterList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Document";
                                                dataInsertionDetails.RegistrationNumber = item.FinalRegistrationNumber == null ? String.Empty : item.FinalRegistrationNumber;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.Stamp5DateTime == null ? String.Empty : ((DateTime)item.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Ready for Insertion";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }
                                        }
                                    }

                                    // GET MARRIAGE CENTRALIZED AT CENTRALIZED SERVER
                                    //int RegistrationID_INT = Convert.ToInt32(RegistrationID_STR);

                                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                    // COMPARED WITH PSROCode INSTEAD OF SROCode
                                    //List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.SROCode == SROCode).ToList();
                                    List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.PSROCode == SROCode).ToList();
                                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                    if (MarriageRegistrationList != null)
                                    {
                                        if (MarriageRegistrationList.Count() > 0)
                                        {
                                            foreach (var item in MarriageRegistrationList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Marriage";
                                                dataInsertionDetails.RegistrationNumber = item.MarriageCaseNo == null ? String.Empty : item.MarriageCaseNo;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.DateOfRegistration == null ? String.Empty : item.DateOfRegistration.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Ready for Insertion";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }
                                        }
                                    }

                                    // GET NOTICE CENTRALIZED AT CENTRALIZED SERVER                                                
                                    //int NoticeID_INT = Convert.ToInt32(NoticeID_STR);

                                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                    // COMPARED WITH PSROCode INSTEAD OF SROCode
                                    //List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.SROCode == SROCode).ToList();
                                    List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.PSROCode == SROCode).ToList();
                                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                    if (NoticeMasterList != null)
                                    {
                                        if (NoticeMasterList.Count() > 0)
                                        {
                                            foreach (var item in NoticeMasterList)
                                            {
                                                DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                dataInsertionDetails.RegistrationType = "Notice";
                                                dataInsertionDetails.RegistrationNumber = item.NoticeNo == null ? String.Empty : item.NoticeNo;
                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                dataInsertionDetails.RegistrationDate = item.NoticeIssuedDate == null ? String.Empty : item.NoticeIssuedDate.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                dataInsertionDetails.Status = "Ready for Insertion";
                                                dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                dataInsertionDetails.SrNo = SerialNoCounter++;

                                                // ADD IN LIST
                                                resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    resModel.IsDataInsertionTableFetched = true;
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataInsertionTable-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            }
            catch (Exception)
            {
                resModel.IsDataInsertionTableFetched = false;
                //resModel.ScriptApprovedMSG = "Scripts not approved";
                throw;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }

            return resModel;
        }

        /// <summary>
        /// DownloadScriptPathVerify
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel DownloadScriptPathVerify(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();

            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DownloadScriptPathVerify-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                {
                    IsDro = false,
                    SROCode = model.SROfficeID,
                    FileDownloadPath = String.Empty
                };

                // FETCH FILE PATH
                DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT && x.INIT_ID == model.INITID_INT && x.SCRIPT_EXECUTION_DATETIME != null && x.IS_SUCCESSFUL == false).FirstOrDefault();
                if (dB_RES_INSERT_SCRIPT_DETAILS != null)
                {
                    scriptSaveREQModel.FileDownloadPath = dB_RES_INSERT_SCRIPT_DETAILS.SCRIPT_PATH;
                }
                // CREATE SERVICE OBJECT 
                SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();
                ScriptSaveRESModel scriptSaveRESModel = service.DownloadScriptPathVerify(scriptSaveREQModel);
                if (scriptSaveRESModel != null)
                {
                    if (scriptSaveRESModel.IsFileExistAtDownloadPath)
                        resModel.IsFileExistAtDownloadPath = true;
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DownloadScriptPathVerify-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            }
            catch (Exception)
            {
                resModel.IsFileExistAtDownloadPath = false;
                throw;
            }
            finally
            {
                if (dbContext != null) dbContext.Dispose();
            }
            return resModel;

        }

        /// <summary>
        /// DownloadScriptForRectification
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel DownloadScriptForRectification(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();

            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DownloadScriptForRectification-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                {
                    IsDro = false,
                    SROCode = model.SROfficeID,
                    FileDownloadPath = String.Empty,

                };

                // FETCH FILE PATH
                DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT && x.INIT_ID == model.INITID_INT && x.SCRIPT_EXECUTION_DATETIME != null && x.IS_SUCCESSFUL == false).FirstOrDefault();
                if (dB_RES_INSERT_SCRIPT_DETAILS != null)
                {
                    scriptSaveREQModel.FileDownloadPath = dB_RES_INSERT_SCRIPT_DETAILS.SCRIPT_PATH;
                    //scriptSaveREQModel.FileDownloadPath = scriptSaveREQModel.FileDownloadPath.Replace(' ', '_').Replace(':', '$');
                }
                // CREATE SERVICE OBJECT 
                SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();
                ScriptSaveRESModel scriptSaveRESModel = service.DownloadScriptForRectification(scriptSaveREQModel);
                if (scriptSaveRESModel != null)
                {
                    if (scriptSaveRESModel.IsFileFetchedSuccesfully)
                    {
                        resModel.IsFileFetchedSuccesfully = true;
                        // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                        resModel.FileContentField = DecryptScript(scriptSaveRESModel.FileContentField);
                        //resModel.FileContentField = scriptSaveRESModel.FileContentField;
                        // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                    }
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DownloadScriptForRectification-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            }
            catch (Exception)
            {
                resModel.IsFileFetchedSuccesfully = false;
                throw;
            }
            finally
            {
                if (dbContext != null) dbContext.Dispose();
            }
            return resModel;

        }

        /// <summary>
        /// SaveUplodedRectifiedScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel SaveUplodedRectifiedScript(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();

            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-SaveUplodedRectifiedScript-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                int SROCODE_INT = 0;
                DB_RES_INITIATE_MASTER db_RES_INITIATE_MASTER = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INITID_INT).FirstOrDefault();
                if (db_RES_INITIATE_MASTER != null)
                {
                    SROCODE_INT = db_RES_INITIATE_MASTER.SROCODE ?? 0;
                }

                // CREATE SERVICE REQUEST MODEL 
                ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                {
                    IsDro = false,
                    SROCode = SROCODE_INT,
                    // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                    ScriptContent = EncryptScript(model.FileContentField)
                    //ScriptContent = model.FileContentField
                    // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                };

                // CREATE SERVICE OBJECT 
                SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();

                // CALL SAVE GENERATE SCRIPT FILE AT FILE SERVER USING SERVICE
                ScriptSaveRESModel scriptSaveRESModel = service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel);
                if (scriptSaveRESModel != null)
                {

                    // IF SCRIPT SAVED SUCCEFULLY AND ERROR MESSAGE IS EMPTY THEN 
                    // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                    // WITH  SUCCESS STATUS
                    if (scriptSaveRESModel.IsScriptSavedSuccessfully && scriptSaveRESModel.ErrorMsg == String.Empty)
                    {
                        //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                        using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                // ADD CORRECTIVE ACTION IN PREVIOUS ACTIONS ROW WHERE SCRIPT= model.SCRIPT_ID_INT AND IS_SUCCESSFULL=0

                                DB_RES_ACTIONS dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.IS_SUCCESSFUL == false && x.SCRIPT_ID == model.SCRIPT_ID_INT
                                 && (x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript ||
                                 x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();

                                if (dB_RES_ACTIONS != null)
                                {
                                    // update corrective actions using model.ScriptRectificationHistory
                                    dB_RES_ACTIONS.CORRECTIVEACTION = model.ScriptRectificationHistory;
                                    dbContext.SaveChanges();
                                }

                                // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                int SCRIPT_ID_For_ADD = 0;
                                DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                save_Script_DETAILS.INIT_ID = model.INITID_INT;
                                // FETCH MAX ITERATION ID USING MODEL.SCRIPT_ID AND ADDED 1 TO SET ITERATION ID
                                save_Script_DETAILS.ITERATION_ID = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT).Max(a => a.ITERATION_ID) : 0) + 1;
                                save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                save_Script_DETAILS.IS_SUCCESSFUL = false;

                                dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                dbContext.SaveChanges();

                                // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                saveGenerateActionDetails.INIT_ID = null;
                                saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateRecitifiedScript;
                                saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                dbContext.SaveChanges();

                                dbContextTransaction.Commit();

                                // SET FLAG IsRectifiedScriptUploadedSuccefully and RectifiedScriptUploadedMsg
                                resModel.IsRectifiedScriptUploadedSuccefully = true;
                                resModel.RectifiedScriptUploadedMsg = "Rectified script uploaded successfully.";

                                // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                //resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                // SET FLAG IsRectifiedScriptUploadedSuccefully and RectifiedScriptUploadedMsg
                                resModel.IsRectifiedScriptUploadedSuccefully = false;
                                resModel.RectifiedScriptUploadedMsg = "Rectified script not uploaded.";
                            }
                        }
                    }
                    // IF SCRIPT NOT SAVED SUCCEFULLY AND ERROR MESSAGE IS NOT EMPTY THEN 
                    // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                    // WITH  FALSE SUCCESS STATUS AND ERROR MESSAGE
                    else
                    {
                        //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                        using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                int SCRIPT_ID_For_ADD = 0;
                                DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                save_Script_DETAILS.INIT_ID = model.INITID_INT;
                                // FETCH MAX ITERATION ID USING MODEL.SCRIPT_ID AND ADDED 1 TO SET ITERATION ID
                                save_Script_DETAILS.ITERATION_ID = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.SCRIPT_ID == model.SCRIPT_ID_INT).Max(a => a.ITERATION_ID) : 0) + 1;
                                save_Script_DETAILS.SCRIPT_PATH = String.Empty;
                                save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                save_Script_DETAILS.IS_SUCCESSFUL = false;

                                dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                dbContext.SaveChanges();

                                // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                saveGenerateActionDetails.INIT_ID = null;
                                saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateRecitifiedScript;
                                saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                saveGenerateActionDetails.IS_SUCCESSFUL = false;
                                saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                saveGenerateActionDetails.ERROR_DESCRIPTION = scriptSaveRESModel.ErrorMsg;

                                dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                dbContext.SaveChanges();

                                dbContextTransaction.Commit();

                                // SET FLAG IsRectifiedScriptUploadedSuccefully and RectifiedScriptUploadedMsg
                                resModel.IsRectifiedScriptUploadedSuccefully = false;
                                resModel.RectifiedScriptUploadedMsg = "Rectified script not uploaded.";
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                // SET FLAG IsRectifiedScriptUploadedSuccefully and RectifiedScriptUploadedMsg
                                resModel.IsRectifiedScriptUploadedSuccefully = false;
                                resModel.RectifiedScriptUploadedMsg = "Rectified script not uploaded.";
                            }
                        }
                    }
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-SaveUplodedRectifiedScript-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            }
            catch (Exception)
            {   // SET FLAG IsRectifiedScriptUploadedSuccefully and RectifiedScriptUploadedMsg
                resModel.IsRectifiedScriptUploadedSuccefully = false;
                resModel.RectifiedScriptUploadedMsg = "Rectified script not uploaded."; throw;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }
            return resModel;
        }

        #region ADDED BY PANKAJ ON 15-07-2020
        /// <summary>
        /// ConfirmDataInsertion
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel ConfirmDataInsertion(DataRestorationReportReqModel model)
        {
            DataRestorationReportResModel resModel = new DataRestorationReportResModel();
            KaveriEntities dbContext2 = new KaveriEntities();
            try
            {    
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-ConfirmDataInsertion-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                using (DbContextTransaction dbContextTransaction = dbContext2.Database.BeginTransaction())
                {
                    try
                    {
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-ConfirmDataInsertion-Internaltry-IN");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                        DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dbContext2.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INITID_INT && x.SROCODE == model.SROfficeID).FirstOrDefault();
                        if (dB_RES_INITIATE_MASTER != null)
                        {
                            if (dB_RES_INITIATE_MASTER.CONFIRM_DATETIME == null)
                            {
                                var todaydatetime = DateTime.Now;
                                dB_RES_INITIATE_MASTER.CONFIRM_DATETIME = todaydatetime;
                                dB_RES_INITIATE_MASTER.STATUS_ID = (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified;

                                dbContext2.SaveChanges();

                                // COMMENTED ON 22-07-2020
                                // AFTER CONFIRM ONCE CANNOT LOGIN AND SEE INFORMATION ANOTHER TIME NEED TO CHANGE
                                //DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = dbContext2.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == model.INITID_INT && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                                //if(dB_RES_ACTIVATION_KEY_OTP != null)
                                //{
                                //    dB_RES_ACTIVATION_KEY_OTP.IS_ACTIVE = false;

                                //    dbContext2.SaveChanges();
                                //}

                                dbContextTransaction.Commit();

                                resModel.IsDataInsertionConfirmed = true;
                                //resModel.DataInsertionConfrimationMsg = "Data confirmation done";
                                resModel.DataInsertionConfrimationMsg = @"<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data Restoration verified and confirmed by SR on date: " + todaydatetime.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "</label>";
                            }
                            else
                            {
                                DateTime? CONFIRM_DATETIME = dB_RES_INITIATE_MASTER.CONFIRM_DATETIME;
                                resModel.IsDataInsertionConfirmed = true;
                                resModel.DataInsertionConfrimationMsg = @"<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data Restoration verified and confirmed by SR on date: " +
                                    (CONFIRM_DATETIME == null ? "</label>" :
                                    ((DateTime)CONFIRM_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "</label>");

                            }
                        }
                        else
                        {
                            resModel.IsDataInsertionConfirmed = false;
                            resModel.DataInsertionConfrimationMsg = "<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data confirmation failed</label>";
                        }

                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        //{
                        //    string format = "{0} : {1}";
                        //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                        //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                        //    file.WriteLine("-DataRestorationReportDAL-ConfirmDataInsertion-Internaltry-OUT");
                        //    file.Flush();
                        //}
                        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        resModel.IsDataInsertionConfirmed = false;
                        resModel.DataInsertionConfrimationMsg = "<label style='font-size:initial;font-weight:bold;'>Data confirmation failed</label>";
                        throw;
                    }
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-ConfirmDataInsertion-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            }
            catch (Exception)
            {
                resModel.IsDataInsertionConfirmed = false;
                resModel.DataInsertionConfrimationMsg = "<label style='font-size:initial;font-weight:bold;'>Data confirmation failed</label>";
                throw;
            }
            finally { if (dbContext2 != null) dbContext2.Dispose(); }

            return resModel;
        }
        ////2nd method for partail view
        //public DataRestorationReportResModel GetConfirmationButtonMessage(DataRestorationReportReqModel model)
        //{
        //    DataRestorationReportResModel resModel = new DataRestorationReportResModel();
        //    try
        //    {
        //         dbContext = new KaveriEntities();

        //        using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INITID_INT && x.SROCODE == model.SROfficeID).FirstOrDefault();
        //                if (dB_RES_INITIATE_MASTER != null)
        //                {
        //                    DateTime? CONFIRM_DATETIME = dB_RES_INITIATE_MASTER.CONFIRM_DATETIME;
        //                    if(CONFIRM_DATETIME != null)
        //                    {
        //                        resModel.DataInsertionConfrimationMsg = @"Data Restoration verified and confirmed by SR on date:- "+ CONFIRM_DATETIME.ToString();
        //                    }

        //                    resModel.IsDataInsertionConfirmed = true;
        //                }
        //                else
        //                {
        //                    resModel.IsDataInsertionConfirmed = false;
        //                    resModel.DataInsertionConfrimationMsg = "Data confirmation failed";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();
        //                resModel.IsDataInsertionConfirmed = false;
        //                resModel.DataInsertionConfrimationMsg = "Data confirmation failed";
        //                throw;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        resModel.IsDataInsertionConfirmed = false;
        //        resModel.DataInsertionConfrimationMsg = "Data confirmation failed";
        //        throw;
        //    }
        // //   finally { if (dbContext != null) dbContext.Dispose(); }
        //    return resModel;
        //}

        #endregion



        #region ADDED BY SHUBHAM BHAGAT ON 23-07-2020
        /// <summary>
        /// LoadInitiateMasterTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        // METHOD TO PUPULATE DATATABLE
        public DataRestorationPartialViewModel LoadInitiateMasterTable(DataRestorationReportReqModel model)
        {
            DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
            resModel.InitMasterModelList = new List<InitMasterModel>();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-LoadInitiateMasterTable-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                var detailsList = dbContext.USP_DB_RES_INITIATE_MASTER_DETAILS(model.SROfficeID).ToList();
                int count = (model.startLen + 1); //To start Serial Number 

                resModel.TotalRecords = detailsList.Count;
                //int count = 1;    


                var USP_DB_RES_INITIATE_MASTER_DETAILS = dbContext.USP_DB_RES_INITIATE_MASTER_DETAILS(model.SROfficeID).ToList();
                if (USP_DB_RES_INITIATE_MASTER_DETAILS != null)
                {
                    if (USP_DB_RES_INITIATE_MASTER_DETAILS.Count > 0)
                    {
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        // BEFORE DOING ANY CHANGES IN USP_DB_RES_INITIATE_MASTER_DETAILS.OrderByDescending(x => x.INIT_DATE).ToList() 
                        // KEEP IN MIND IT WILL IMPACT ON "ABORT" BUTTON POPULATION LOGIC AS "ABORT" BUTTON IS ONLY POPULATED TO ONLY LATEST INIT_ID
                        USP_DB_RES_INITIATE_MASTER_DETAILS = USP_DB_RES_INITIATE_MASTER_DETAILS.OrderByDescending(x => x.INIT_DATE).ToList();

                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        // FOR FETCHING LASTEST INIT_ID CONSIDER SROCODE IN WHERE CLAUSE when we are thinking to compare latest init_id and init_id from sp but no need to do it

                        // FETCH INIT_ID OF THE LATEST DATABASE RESTORATION PROCESS
                        int latestInitID=USP_DB_RES_INITIATE_MASTER_DETAILS.Take(1).Select(x=>x.INIT_ID).FirstOrDefault();
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

                        if (String.IsNullOrEmpty(model.SearchValue))
                        {
                            USP_DB_RES_INITIATE_MASTER_DETAILS = USP_DB_RES_INITIATE_MASTER_DETAILS.Skip(model.startLen).Take(model.totalNum).ToList();
                        }

                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020



                        foreach (var item in USP_DB_RES_INITIATE_MASTER_DETAILS)
                        {
                            InitMasterModel initMasterModel = new InitMasterModel();
                            // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 09-04-2021 AS DISCUSSED WITH SIR
                            // BECAUSE WE WANT TO SHOW INIT ID IN PLACE OF SERIAL NO
                            //initMasterModel.SrNo = count++;
                            initMasterModel.SrNo = item.INIT_ID;
                            // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 09-04-2021 AS DISCUSSED WITH SIR

                            initMasterModel.SroCode = item.DB_RES_IM_OBJ_SROCODE;
                            initMasterModel.SROName = item.SRONameE ?? String.Empty;
                            initMasterModel.InitiationDateTime = item.INIT_DATE.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            initMasterModel.STATUS_DESCRIPTION = item.STATUS_DESCRIPTION ?? String.Empty;
                            initMasterModel.Is_Completed_STR = item.IS_COMPLETED ? "<i class='fa fa-check  'style='color:black'></i>" : "<i class='fa fa-spinner  'style='color:black'></i>";
                            //initMasterModel.Is_Completed_STR = item.IS_COMPLETED ? "<i class='fa fa-check  'style='color:black'></i>" : "<i class='fa fa-close  'style='color:black'></i>";

                            initMasterModel.CompleteDateTime = item.COMPLETE_DATETIME == null ? String.Empty : ((DateTime)item.COMPLETE_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            initMasterModel.ConfirmDateTime = item.CONFIRM_DATETIME == null ? String.Empty : ((DateTime)item.CONFIRM_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                            // ADDED BY SHUBHAM BHAGAT ON 24-07-2020
                            initMasterModel.INIT_ID = item.INIT_ID;


                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                            //initMasterModel.AbortBtn = item.IS_COMPLETED ? String.Empty : "<button type ='button' class='btn btn-group-md btn-danger' onclick=AbortProcess('" + item.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Abort</ button>";
                            if (item.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified || item.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationAborted)
                            {
                                initMasterModel.AbortBtn = String.Empty;
                            }
                            else
                            {
                                if (latestInitID == item.INIT_ID)
                                {
                                    initMasterModel.AbortBtn = "<button type ='button' class='btn btn-group-md btn-danger' onclick=AbortProcess('" + item.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Abort</ button>";
                                }
                            }
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	

                            resModel.InitMasterModelList.Add(initMasterModel);
                        }
                    }
                }

                // INITIATION DATABASE RESTORATION BUTTON TO BE DRAWN ONLY FOR SR
                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                {
                    // FETCH THE TOTAL ENTRIES IN INITIATE MASTER TABLE LIST FOR SRO
                    List<DB_RES_INITIATE_MASTER> dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                    Where(x => x.SROCODE == model.SROfficeID && x.IS_DRO == false).ToList();

                    // ENTRY EXIST FOR A SRO
                    if (dB_RES_INITIATE_MASTERList != null)
                    {
                        if (dB_RES_INITIATE_MASTERList.Count() > 0)
                        {
                            #region Commented by shubham bhagat on 30-09-2020
                            //// FINDING ALL COMPLETED DATABASE RESTORATION PROCESS
                            //var COMPLETED_INITIATE_MASTER_LIST = dB_RES_INITIATE_MASTERList.Where(x => x.IS_COMPLETED == true).ToList();
                            //// ON 02-10-2020 START FROM HERE
                            //// CHANGE LOGIC OF LIST, FIND LATEST DATABASE RESTORATION PROCESS BY ORDER BY DESC INIT_DATETIME AND THAN FETCH 
                            //// 1ST OBJ TAHN CHECK IS_COMPLETED AND STATUS_ID=7 
                            //// IF THE DATABASE RESTORATION PROCESS IS COMPLETED SO POPULATE Initiate Database Restoration BUTTON
                            //if (dB_RES_INITIATE_MASTERList.Count == COMPLETED_INITIATE_MASTER_LIST.Count)
                            //{
                            //    resModel.InitiateBTNForSR = "<button type ='button' style='width:18%;' class='btn btn-group-md btn-primary' onclick=InitiateNewProcess('" + model.SROfficeID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Initiate new restoration request</ button>";
                            //}
                            //else // IF A PROCESS IS IN EXECUTION SO DON'T POPULATE BUTTON
                            //    resModel.InitiateBTNForSR = String.Empty; 
                            #endregion

                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                            dB_RES_INITIATE_MASTERList = dB_RES_INITIATE_MASTERList.OrderByDescending(x => x.INIT_DATE).ToList();
                            DB_RES_INITIATE_MASTER latestInitModel = dB_RES_INITIATE_MASTERList.Take(1).FirstOrDefault();

                            // POPULATE INITIATE BUTTON IF ALL PREVIOUS PROCESS IS COMPLETED OR ABORTED
                            if (latestInitModel.IS_COMPLETED == true || latestInitModel.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationAborted)
                            {
                                resModel.InitiateBTNForSR = "<button type ='button' style='width:18%;' class='btn btn-group-md btn-primary' onclick=InitiateNewProcess('" + model.SROfficeID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Initiate new restoration request</ button>";
                            }
                            else // IF A PROCESS IS IN EXECUTION SO DON'T POPULATE BUTTON
                                resModel.InitiateBTNForSR = String.Empty;
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        }
                        else // IF THERE IS NO ENTRY EXIST FOR SRO SO POPULATE Initiate Database Restoration BUTTON
                        {
                            resModel.InitiateBTNForSR = "<button type ='button' style='width:18%;' class='btn btn-group-md btn-primary' onclick=InitiateNewProcess('" + model.SROfficeID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Initiate new restoration request</ button>";
                        }
                    }
                }
                else
                    resModel.InitiateBTNForSR = String.Empty;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-LoadInitiateMasterTable-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
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


            //// HARD CODE DATA FOR TESTING
            //int counter = 2;
            //for (int i = 2; i < 20; i++)
            //{
            //    InitMasterModel initMasterModel = new InitMasterModel();
            //    initMasterModel.SrNo = counter++;
            //    initMasterModel.SroCode = 182;
            //    initMasterModel.SROName = "Yelburga";
            //    initMasterModel.InitiationDateTime = "2020-07-23 14:16:39.347";
            //    initMasterModel.STATUS_DESCRIPTION = "Dummy Data";
            //    initMasterModel.Is_Completed_STR = true ? "<i class='fa fa-check  'style='color:black'></i>" : "<i class='fa fa-close  'style='color:black'></i>";
            //    initMasterModel.CompleteDateTime = "2020-07-23 14:16:39.347";
            //    initMasterModel.ConfirmDateTime = "2020-07-23 14:16:39.347";

            //    // ADDED BY SHUBHAM BHAGAT ON 24-07-2020
            //    initMasterModel.INIT_ID = i;
            //    resModel.InitMasterModelList.Add(initMasterModel);
            //}
            //resModel.TotalRecords = resModel.InitMasterModelList.Count;
            //// HARD CODE DATA FOR TESTING

            return resModel;
        }

        #endregion


        // ADDED BY SHUBHAM BHAGAT ON 06-08-2020
        /// <summary>
        /// EncryptScript
        /// </summary>
        /// <param name="encryptByteArray"></param>
        /// <returns>returns encrypt Byte Array</returns>
        public static byte[] EncryptScript(byte[] encryptByteArray)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-EncryptScript-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            string EncryptionKey = "Gxv@#123";  //we can change the code converstion key as per our requirement    
            byte[] clearBytes = encryptByteArray;
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptByteArray = ms.ToArray();
                }
            }

            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-EncryptScript-OUT");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            return encryptByteArray;
        }

        // ADDED BY SHUBHAM BHAGAT ON 06-08-2020
        /// <summary>
        /// DecryptScript
        /// </summary>
        /// <param name="cipherByteArray"></param>
        /// <returns>returns Decrypt Byte Array</returns>
        public static byte[] DecryptScript(byte[] cipherByteArray)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-DecryptScript-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            string EncryptionKey = "Gxv@#123";  //we can change the code converstion key as per our requirement, but the decryption key should be same as encryption key    
            //cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = cipherByteArray;
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherByteArray = ms.ToArray();
                }
            }

            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportDAL-DecryptScript-OUT");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            return cipherByteArray;
        }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        /// <summary>
        /// AbortView
        /// </summary>
        /// <param name="INIT_ID"></param>
        /// <returns>returns AbortViewModel Model</returns>
        public AbortViewModel AbortView(String INIT_ID)
        {
            return new AbortViewModel();
        }

        /// <summary>
        /// SaveAbortData
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns AbortViewModel Model</returns>
        public AbortViewModel SaveAbortData(AbortViewModel model)
        {
            AbortViewModel resModel = new AbortViewModel();
            try
            {

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-SaveAbortData-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();

                DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == model.INIT_ID_INT).FirstOrDefault();
                if (dB_RES_INITIATE_MASTER != null)
                {
                    dB_RES_INITIATE_MASTER.DATE_OF_ABORT = DateTime.Now;
                    //dB_RES_INITIATE_MASTER.ABORT_DESCRIPTION = model.AbortDescription;
                    dB_RES_INITIATE_MASTER.ABORT_DESCRIPTION = model.AbortDescription;
                    dB_RES_INITIATE_MASTER.STATUS_ID = (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationAborted;

                    dbContext.SaveChanges();

                    resModel.Message = "Reason for Abort saved.";
                }
                else
                {
                    resModel.Message = "Reason for Abort not saved.";
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-SaveAbortData-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

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

        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020


        /// <summary>
        /// DataRestorationReportStatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel DataRestorationReportStatusForScript(DataRestorationReportViewModel model)
        {
            DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
            bool IsScriptReadyForInsertion = false, IsScriptExecutedSuccessfully = false, IsScriptExecutedWithError = false;
            String scriptExecutionErrorMsg = String.Empty;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                dbContext = new KaveriEntities();


                var officeID = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                     Select(x => x.OfficeID).FirstOrDefault();

                // OfficeName
                var OfficeName = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                     Select(x => x.OfficeName).FirstOrDefault();

                var userID = dbContext.UMG_UserDetails.Where(x => x.OfficeID == officeID).Select(x => x.UserID).FirstOrDefault();

                // SR Name
                var SRName = dbContext.UMG_UserProfile.Where(x => x.UserID == userID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                // SR ContactNumber
                var SRContactNumber = dbContext.UMG_UserProfile.Where(x => x.UserID == userID).Select(x => x.MobileNumber).FirstOrDefault();

                // SR Name
                resModel.SRName = SRName;
                // OfficeName
                resModel.OfficeName = OfficeName + " Office";
                // SR ContactNumber
                resModel.SRContactNumber = SRContactNumber;

                // for adding validation on generte key btn 

                List<DB_RES_INITIATE_MASTER> dB_RES_INITIATE_MASTERList = null;

                // FOR SR FOR INITIALIZING NEW PROCESS
                if (model.INIT_ID_INT == 0)
                {
                    // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-07-2020 AT 3:00 PM
                    // FOR DRAWING CLICK HERE BUTTON TO INITIATE NEW REQUEST

                    // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                    dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                           Where(x => x.SROCODE == model.SROfficeID &&
                                      x.IS_DRO == false &&
                                      x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated).ToList();

                    //dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                    //        Where(x => x.SROCODE == model.SROfficeID &&
                    //                   x.IS_DRO == false && x.STATUS_ID == null).ToList();

                    // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 

                    //dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                    //       Where(x => x.SROCODE == model.SROfficeID &&
                    //                  x.IS_DRO == false).ToList();
                }
                // FOR FETCH DETAILS OF PREVIOUS PPROCESS
                else
                {
                    dB_RES_INITIATE_MASTERList = dbContext.DB_RES_INITIATE_MASTER.
                            Where(x => x.SROCODE == model.SROfficeID &&
                                       x.IS_DRO == false &&
                                       x.INIT_ID == model.INIT_ID_INT).ToList();
                }

                // ENTRY EXIST FOR A SRO
                if (dB_RES_INITIATE_MASTERList != null)
                {
                    if (dB_RES_INITIATE_MASTERList.Count() > 0)
                    {
                        // FINDING LATEST INITIATION REQUEST
                        dB_RES_INITIATE_MASTERList = dB_RES_INITIATE_MASTERList.OrderByDescending(x => x.INIT_DATE).ToList();

                        // FOR INITID
                        DB_RES_INITIATE_MASTER dB_RES_INITIATE_MASTER = dB_RES_INITIATE_MASTERList.FirstOrDefault();

                        #region ADDED BY SHUBHAM BHAGAT ON 09-04-2021 FOR MAKING INSERT OF DATABASE RESTORATION TABLES

                        // DECLARE FILE PATH HERE BECAUSE WE HAVE TO CREATE DEBUG FILE ACCORDING TO INITID
                        string debugLogFilePath = directoryPath + "\\2021\\Mar\\DatabaseRestorationDebugAPI_" + dB_RES_INITIATE_MASTER.INIT_ID + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";



                        String debugScript = @"DECLARE @PRED VARCHAR(8000)
                                        DECLARE @TEMPTAB TABLE
                                        (
                                            SCRIPTTEXT VARCHAR(MAX),
                                            QuerySeqNo int identity(1, 1)
                                        )
                                        
                                        
                                        ----------------------------------- 1---------------------------------------------
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_STATUS_MASTER'
                                        
                                        
                                        ----------------------------------- 2---------------------------------------------
                                        INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_TABLE_MASTER ON'
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_TABLE_MASTER'
                                        INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_TABLE_MASTER OFF'
                                        
                                        
                                        ----------------------------------- 3---------------------------------------------
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_MISC_SCRIPTS'
                                        
                                        
                                        ----------------------------------- 4---------------------------------------------
                                        
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_ACTIONS_MASTER'
                                        
                                        
                                        ----------------------------------- 5---------------------------------------------
                                        
                                        SET @PRED = 'FROM DB_RES_INITIATE_MASTER WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_INITIATE_MASTER'  , @FROM = @PRED
                                        
                                        
                                        ----------------------------------- 6---------------------------------------------
                                        SET @PRED = 'FROM DB_RES_ACTIVATION_KEY_OTP WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_ACTIVATION_KEY_OTP', @FROM = @PRED
                                        
                                        
                                        ----------------------------------- 7---------------------------------------------
                                        SET @PRED = 'FROM DB_RES_INSERT_SCRIPT_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_INSERT_SCRIPT_DETAILS', @FROM = @PRED
                                        
                                        
                                        
                                        ----------------------------------- 8---------------------------------------------
                                        
                                        SET @PRED = 'FROM DB_RES_ACTIONS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_ACTIONS', @FROM = @PRED
                                        
                                        ----------------------------------- 9---------------------------------------------
                                        SET @PRED = 'FROM DB_RES_SERVICE_COMM_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_SERVICE_COMM_DETAILS ON'
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_SERVICE_COMM_DETAILS', @FROM = @PRED
                                        INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_SERVICE_COMM_DETAILS OFF'
                                        
                                        
                                        ----------------------------------- 10---------------------------------------------
                                        --@PRED VARCHAR(8000)
                                        SET @PRED = 'FROM DB_RES_OFFICE_TABLE_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB SELECT 'SET IDENTITY_INSERT DB_RES_OFFICE_TABLE_DETAILS ON'
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_OFFICE_TABLE_DETAILS', @FROM = @PRED
                                        INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_TABLE_DETAILS OFF'
                                        
                                        
                                        ----------------------------------- 11---------------------------------------------
                                        SET @PRED = 'FROM DB_RES_OFFICE_COLUMN_DETAILS WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_COLUMN_DETAILS ON'
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_OFFICE_COLUMN_DETAILS', @FROM = @PRED
                                        INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_OFFICE_COLUMN_DETAILS OFF'
                                        
                                        
                                        ----------------------------------- 12---------------------------------------------
                                        SET @PRED = 'FROM DB_RES_TABLEWISE_COUNT WHERE INIT_ID = ' + CONVERT(VARCHAR, @INIT_ID) + ''
                                        INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_TABLEWISE_COUNT ON'
                                        INSERT INTO @TEMPTAB
                                        EXEC DBO.sp_generate_inserts 'DB_RES_TABLEWISE_COUNT', @FROM = @PRED
                                        INSERT INTO @TEMPTAB SELECT  'SET IDENTITY_INSERT DB_RES_TABLEWISE_COUNT ON'
                                        
                                        
                                        
                                        SELECT* FROM @TEMPTAB ORDER BY QuerySeqNo";

                        // addde to be 
                        //  waitfor delay '00:00:40';

                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        {
                            string format = "{0} : {1}";
                            file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                            file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                            file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-Before-Debug log script creation");
                            file.Flush();
                        }
                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                        using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                        {
                            using (SqlCommand command = new SqlCommand(debugScript, connection))
                            {
                                connection.Open();

                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021
                                String DBRESGenerateScriptCommandTimeout = ConfigurationManager.AppSettings["DBRESGenerateScriptCommandTimeout"];
                                command.CommandTimeout = Convert.ToInt32(DBRESGenerateScriptCommandTimeout);
                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021

                                command.CommandType = CommandType.Text;

                                command.Parameters.AddWithValue("@INIT_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;

                                SqlDataReader detailsReader = command.ExecuteReader();
                                DataTable scriptsDetails = new DataTable();
                                scriptsDetails.Load(detailsReader);

                                StringBuilder debugScriptBuilder_STR = new StringBuilder();

                                for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                                {
                                    debugScriptBuilder_STR = debugScriptBuilder_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["SCRIPTTEXT"]));
                                }

                                // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                {
                                    string format = "{0} : {1}";
                                    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                    file.WriteLine(debugScriptBuilder_STR.ToString());
                                    file.Flush();
                                }
                                // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                            }
                        }

                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                        using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                        {
                            string format = "{0} : {1}";
                            file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                            file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                            file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-After-Debug log script creation");
                            file.Flush();
                        }
                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                        #endregion

                        // FOR KEYDATETIME
                        // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-09-2020
                        //DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTP = null;
                        // IF THE DATABASE RESTORATION PROCESS IS COMPLETED THEN USE IS_ACTIVE==FALSE 
                        if (dB_RES_INITIATE_MASTER.IS_COMPLETED)
                            dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == false && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        // IF THE DATABASE RESTORATION PROCESS IS IN PROGRESS THEN USE IS_ACTIVE==TRUE 
                        else
                            dB_RES_ACTIVATION_KEY_OTP = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                        // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-09-2020

                        // INITIATE DATE SHOULD BE DISPLAYED IN ALL CASE IF ENTRY EXIST FOR A SRO
                        resModel.InitiationDate = dB_RES_INITIATE_MASTER.INIT_DATE.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        // PROCESS IS STILL NOT STARTED BY HCL USER
                        //if (dB_RES_INITIATE_MASTER.STATUS_ID == null)
                        //{
                        ////Initiate Date
                        //resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        //Initiate Date btn                                                                                                                       
                        resModel.InitiationDateBtn = String.Empty;

                        // Show Init Date And Generated Key Msg in all below cases
                        resModel.ShowInitDateAndGeneratedKeyMsg = true;

                        DateTime KEY_DATE_DateTime, today_Date_DateTime;
                        String today_Date_STR = String.Empty;
                        int noOfDays = 0;

                        today_Date_STR = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime.TryParse(DateTime.ParseExact(today_Date_STR, "dd/MM/yyyy HH:mm:ss", null).ToString(), out today_Date_DateTime);

                        var KEY_DATE_STR = ((DateTime)dB_RES_ACTIVATION_KEY_OTP.KEY_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime.TryParse(DateTime.ParseExact(KEY_DATE_STR, "dd/MM/yyyy HH:mm:ss", null).ToString(), out KEY_DATE_DateTime);
                        //var d = today_Date_DateTime.Subtract(INIT_DATE_DateTime);
                        //noOfDays = (today_Date_DateTime - INIT_DATE_DateTime).Days;
                        noOfDays = today_Date_DateTime.Subtract(KEY_DATE_DateTime).Days;


                        // ASK SIR TO CONSIDER STATUSID AND NOOFDAYS BOTH
                        //if (noOfDays >= 5) // IF THE KEY IS EXPIRED i.e 5 days over
                        // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        if (noOfDays >= 5 && dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated) // IF THE KEY IS EXPIRED i.e 5 days over
                        //if (noOfDays >= 5 && dB_RES_INITIATE_MASTER.STATUS_ID == null) // IF THE KEY IS EXPIRED i.e 5 days over
                        // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        {
                            // KEY IS EXPIRED
                            // SEND MSG AND POPULATE GENERATE KEY BTN
                            //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Your activation key is expired for generating another key please click <button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyAfterExpiration()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button><span id='GenerateKeyAfterExpirationValueSpanID'>" + GenerateKey() + "</span>.</label>";
                            // SEND BTN WITHOUT KEY
                            //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;width:64%;'>" +
                            //    "Your activation key is expired for generating another key please click " +
                            //    "<button type ='button' style='width:20%;' class='btn btn-group-md btn-success'" +
                            //    " onclick=GenerateKeyAfterExpiration('" + dB_RES_INITIATE_MASTER.INIT_ID + "','" + dB_RES_ACTIVATION_KEY_OTP.KEYID + "')>" +
                            //    "<i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button></label>";

                            // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                            if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;width:64%;color: #3177b4;'>" +
                                   "Your activation key is expired for generating another key please click " +
                                   "<button type ='button' style='width:20%;' class='btn btn-group-md btn-success'" +
                                   " onclick=GenerateKeyAfterExpiration('" + dB_RES_INITIATE_MASTER.INIT_ID + "','" + dB_RES_ACTIVATION_KEY_OTP.KEYID + "')>" +
                                   "<i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button></label>";
                            else
                                resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                            // SET ACTIVATION KEY ACTIVE STATUS TO 0 FOR MAKING INACTIVE
                            //DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTPExpired = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();
                            //dB_RES_ACTIVATION_KEY_OTPExpired.IS_ACTIVE = false;
                            //dbContext.SaveChanges();
                        }
                        else // IF THE KEY IS NOT EXPIRED
                        {
                            DB_RES_ACTIVATION_KEY_OTP dB_RES_ACTIVATION_KEY_OTPPrevious = dbContext.DB_RES_ACTIVATION_KEY_OTP.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.IS_ACTIVE == true && x.KEY_TYPE == (int)ApiCommonEnum.DB_RES_KeyType.Key).FirstOrDefault();

                            // DISPLAY KEY WITH MSG
                            if (dB_RES_ACTIVATION_KEY_OTPPrevious != null)
                            {
                                //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Activation key for data restoration utility is : " + Decrypt(dB_RES_ACTIVATION_KEY_OTPPrevious.KEYVALUE) + " which is valid for 5 days only Please share this activation key to support Engineer.</label>";
                                // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                    resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : " + Decrypt(dB_RES_ACTIVATION_KEY_OTPPrevious.KEYVALUE) + " which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                                else
                                    resModel.GenerateKeyBtnAndTextMsg = String.Empty;
                            }


                        }
                        //}
                        //else  // IF THE PROCESS IS STARTED BY HCL USER
                        //{

                        //    // CHECK STATUS AND ACTIONS
                        //    // ADD SWITCH CASE AND ADD STATUS TABLE ROWS IN CASES
                        //    //DB_RES_STATUS_MASTER dB_RES_STATUS_MASTER = dbContext.DB_RES_STATUS_MASTER.Where(x => x.STATUS_ID == dB_RES_INITIATE_MASTER.STATUS_ID).FirstOrDefault();
                        //    //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>" + dB_RES_STATUS_MASTER.STATUS_DESCRIPTION + "</label>";
                        //}

                        // Database Restoration Details :
                        #region Database Restoration Details: 
                        // CHANGED IF CONDITION ON 06-07-2020 AT 5:30 PM
                        //BECAUSE IF WE COMPARE WITH STATUSID=1 SO IT WILL NOT SHOW BOTH THE SECTIONS I.E DATABASE RESTORATION DETAILS
                        //AND DATA INSERTION DETAILS
                        //if (dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DatabaseRestored)
                        if (dB_RES_INITIATE_MASTER.STATUS_ID >= (int)ApiCommonEnum.DB_RES_STATUSMASTER.DatabaseRestored)
                        {
                            bool databaseRestoredSuccessfully = true;

                            // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                            // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                            // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR
                            // List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).ToList();

                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                            // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                            //    x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && 
                            //                       (
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                            //                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema 
                            //                       )
                            //    ).ToList();

                            List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                               x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                  (
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                                  )
                               ).ToList();
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021

                            // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                            if (db_RES_ACTIONSList != null)
                            {
                                if (db_RES_ACTIONSList.Count() > 0)
                                {
                                    //ADDED BELOW CODE BY SHUBHAM BHAGAT ON 17-07-2020 AT 6:00 PM 
                                    //TO GET LATEST ACTIONS RELATED TO CURRENT INIT_ID
                                    // ORDER BY DESCENDING
                                    db_RES_ACTIONSList = db_RES_ACTIONSList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                    // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                    // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                                    // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR

                                    //// BEFORE TAKING TOP 6 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 6 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 6)
                                    //{
                                    //    // TAKING TOP 6 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(6).ToList();
                                    //}

                                    // BEFORE TAKING TOP 7 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 7 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 7)
                                    //{
                                    //    // TAKING TOP 7 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(7).ToList();
                                    //}
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020


                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                                    // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                                    // ABOVE CODE IS COMMENTED BECAUSE PREVIOUSLY WE ARE SENDING ONLY 7 ACTIONS FROM UTILITY TO SRTOCENTRALCOMM SERVICE NOW WE ARE SENDING 10 ACTIONS
                                    // BEFORE TAKING TOP 10 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 10 ELEMENTS
                                    if (db_RES_ACTIONSList.Count > 10)
                                    {
                                        // TAKING TOP 7 ELEMENTS 
                                        db_RES_ACTIONSList = db_RES_ACTIONSList.Take(10).ToList();
                                    }
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021


                                    //ADDED ABOVE CODE BY SHUBHAM BHAGAT ON 17-07-2020 AT 6:00 PM 

                                    foreach (var item in db_RES_ACTIONSList)
                                    {
                                        if (!item.IS_SUCCESSFUL)
                                            databaseRestoredSuccessfully = false;
                                    }
                                    if (databaseRestoredSuccessfully)
                                    {
                                        // DATABASE RESTORATION DATE
                                        DB_RES_ACTIONS db_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                                    x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).FirstOrDefault();

                                        // DATABASE RESTORATION DATE
                                        resModel.DataRestorationDate = db_RES_ACTIONS.ACTION_DATETIME.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);



                                        //List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SERVICE_TYPE == ApiCommonEnum.DB_RES_SERVICE_COMM_DETAILS_MASTER.DataBaseRestored.ToString()).ToList();
                                        List<DB_RES_SERVICE_COMM_DETAILS> db_RES_SERVICE_COMM_DETAILSList = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID).ToList();
                                        if (db_RES_SERVICE_COMM_DETAILSList != null)
                                        {
                                            if (db_RES_SERVICE_COMM_DETAILSList.Count() > 0)
                                            {
                                                // DATABASE RESTORATION FROM BAK FILE DATETIME
                                                var DB_Restored_upto_BAK_DT_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_DT").Select(x => x.KEY_VALUE);
                                                resModel.DB_RES_BAK_FILE_DATETIME = DB_Restored_upto_BAK_DT_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_DT_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (resModel.DB_RES_BAK_FILE_DATETIME != String.Empty)
                                                {
                                                    resModel.DB_RES_BAK_FILE_DATETIME = resModel.DB_RES_BAK_FILE_DATETIME.Replace('-', '/');
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                // DATABASE RESTORATION FROM BAK FILE size
                                                var DB_Restored_upto_BAK_Size_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_Size").Select(x => x.KEY_VALUE);
                                                resModel.DB_RES_BAK_FILE_Size = DB_Restored_upto_BAK_Size_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_Size_IEnumerable.FirstOrDefault();
                                                resModel.DB_RES_BAK_FILE_Size = resModel.DB_RES_BAK_FILE_Size == null ? "0 GB" : resModel.DB_RES_BAK_FILE_Size;

                                                // LastDocumentID
                                                var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                                String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();
                                                long DocumentID_INT = Convert.ToInt64(DocumentID_STR);

                                                // LastRegistrationID
                                                var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                                String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();
                                                long RegistrationID_INT = Convert.ToInt64(RegistrationID_STR);

                                                // LastNoticeID
                                                var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                                String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();
                                                long NoticeID_INT = Convert.ToInt64(NoticeID_STR);

                                                // DON'T COMMENT BELOW CODE BECAUSE IT IS USING FOR GENERATE SCRIPT USING STORED PROCEDURE, STORED PROCEDURE IS USING FINAL REGISTRATION NUMBER AND SRO CODE 
                                                var FinalRegistrationNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "FinalRegistrationNumber").Select(x => x.KEY_VALUE);
                                                String FinalRegistrationNumber_ForGenerateScript = FinalRegistrationNumber_IEnumerable == null ? String.Empty : FinalRegistrationNumber_IEnumerable.FirstOrDefault();

                                                // LastDocumentRegistrationNumber
                                                //var FinalRegistrationNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "FinalRegistrationNumber").Select(x => x.KEY_VALUE);
                                                //resModel.LastDocumentRegistrationNumber = FinalRegistrationNumber_IEnumerable == null ? String.Empty : FinalRegistrationNumber_IEnumerable.FirstOrDefault();

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 30-09-2020
                                                //resModel.LastDocumentRegistrationNumber = FinalRegistrationNumber_ForGenerateScript;
                                                resModel.LastDocumentRegistrationNumber = String.IsNullOrEmpty(FinalRegistrationNumber_ForGenerateScript) ? "-" : FinalRegistrationNumber_ForGenerateScript;
                                                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 30-09-2020
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

                                                // both below code is working
                                                //var foo = from db_RES_SERVICE_COMM_DETAILS in db_RES_SERVICE_COMM_DETAILSList.AsQueryable<DB_RES_SERVICE_COMM_DETAILS>()
                                                //          where db_RES_SERVICE_COMM_DETAILS.KEY_COLUMN.Contains("FinalRegistrationNumber")
                                                //          select db_RES_SERVICE_COMM_DETAILS.KEY_VALUE;
                                                // on output in immediate window foo.FirstOrDefault(); or foo.SingleOrDefault() will give output
                                                //var foo = from db_RES_SERVICE_COMM_DETAILS in db_RES_SERVICE_COMM_DETAILSList.AsQueryable<DB_RES_SERVICE_COMM_DETAILS>()
                                                //          where db_RES_SERVICE_COMM_DETAILS.KEY_COLUMN.Contains("FinalRegistrationNumber")
                                                //          select db_RES_SERVICE_COMM_DETAILS.KEY_VALUE.FirstOrDefault();

                                                // BELOW CODE UNCOMMENTED AND DATA FROM DOCUMENT MASTER IS COMMENTED BY SHUBHAM BHAGAT 
                                                // ON 30-09-2020 AFTER DISCUSSION WITH SIR
                                                //LastDocumentRegistrationDate
                                                var LastDocumentRegistrationDate_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "Stamp5DateTime").Select(x => x.KEY_VALUE);
                                                resModel.LastDocumentRegistrationDate = LastDocumentRegistrationDate_IEnumerable == null ? String.Empty : LastDocumentRegistrationDate_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 30-09-2020 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                //if (resModel.LastDocumentRegistrationDate != String.Empty)
                                                if (!String.IsNullOrEmpty(resModel.LastDocumentRegistrationDate))
                                                {
                                                    resModel.LastDocumentRegistrationDate = resModel.LastDocumentRegistrationDate.Replace('-', '/');
                                                }
                                                // BELOW ADDED BY SHUBHAM BHAGAT ON 8-12-2020
                                                // BECAUSE WE WANT TO DISPLAY '-' WHEN THE DOCUMENT IS NOT REGISTERED
                                                else
                                                {
                                                    resModel.LastDocumentRegistrationDate = "          -";
                                                }
                                                // ABOVE ADDED BY SHUBHAM BHAGAT ON 8-12-2020

                                                //// LastDocumentRegistrationNumber i.e Document Number
                                                //// LastDocumentRegistrationDate i.e Present
                                                //DocumentMaster documentMaster = dbContext.DocumentMaster.Where(x => x.DocumentID == DocumentID_INT && x.SROCode == model.SROfficeID).FirstOrDefault();
                                                //if (documentMaster != null)
                                                //{
                                                //    resModel.LastDocumentRegistrationNumber = documentMaster.DocumentNumber.ToString();
                                                //    resModel.LastDocumentRegistrationDate = documentMaster.PresentDateTime == null ? String.Empty : ((DateTime)documentMaster.PresentDateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                //}
                                                // ABOVE CODE UNCOMMENTED AND DATA FROM DOCUMENT MASTER IS COMMENTED BY SHUBHAM BHAGAT 
                                                // ON 30-09-2020 AFTER DISCUSSION WITH SIR

                                                // LastMarriageRegistrationNumber
                                                var MarriageCaseNo_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "MarriageCaseNo").Select(x => x.KEY_VALUE);
                                                resModel.LastMarriageRegistrationNumber = MarriageCaseNo_IEnumerable == null ? String.Empty : MarriageCaseNo_IEnumerable.FirstOrDefault();

                                                // LastMarriageRegistrationDate
                                                var DateOfRegistration_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DateOfRegistration").Select(x => x.KEY_VALUE);
                                                resModel.LastMarriageRegistrationDate = DateOfRegistration_IEnumerable == null ? String.Empty : DateOfRegistration_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (!String.IsNullOrEmpty(resModel.LastMarriageRegistrationDate))
                                                {
                                                    resModel.LastMarriageRegistrationDate = resModel.LastMarriageRegistrationDate.Replace('-', '/');
                                                }

                                                // LastNoticeRegistrationNumber
                                                var NoticeNo_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeNo").Select(x => x.KEY_VALUE);
                                                resModel.LastNoticeRegistrationNumber = NoticeNo_IEnumerable == null ? String.Empty : NoticeNo_IEnumerable.FirstOrDefault();

                                                // LastNoticeRegistrationDate
                                                var NoticeIssuedDate_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeIssuedDate").Select(x => x.KEY_VALUE);
                                                resModel.LastNoticeRegistrationDate = NoticeIssuedDate_IEnumerable == null ? String.Empty : NoticeIssuedDate_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 14-07-20220 AT 12:10 PM
                                                // REPLACED '-' WITH  '/' TO SHOW DATETIME IN SAME FORMAT WE WILLCHECK
                                                // IF STRING HAS VALUE THEN WE WILL REPLACE IT AND ASSIGN TTO SAME VARIABLE
                                                if (!String.IsNullOrEmpty(resModel.LastNoticeRegistrationDate))
                                                {
                                                    resModel.LastNoticeRegistrationDate = resModel.LastNoticeRegistrationDate.Replace('-', '/');
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM BELOW CODE

                                                // DocumentRegistrationCDNumber
                                                var DocumentRegistrationCDNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentRegistrationCDNumber").Select(x => x.KEY_VALUE);
                                                resModel.DocumentRegistrationCDNumber = DocumentRegistrationCDNumber_IEnumerable == null ? String.Empty : DocumentRegistrationCDNumber_IEnumerable.FirstOrDefault();

                                                // MarriageRegistrationCDNumber
                                                var MarriageRegistrationCDNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "MarriageRegistrationCDNumber").Select(x => x.KEY_VALUE);
                                                resModel.MarriageRegistrationCDNumber = MarriageRegistrationCDNumber_IEnumerable == null ? String.Empty : MarriageRegistrationCDNumber_IEnumerable.FirstOrDefault();

                                                // DocumentRegistrationCDNumber
                                                var DB_Restored_upto_BAK_FileName_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DB_Restored_upto_BAK_FileName").Select(x => x.KEY_VALUE);
                                                resModel.DB_Restored_upto_BAK_FileName = DB_Restored_upto_BAK_FileName_IEnumerable == null ? String.Empty : DB_Restored_upto_BAK_FileName_IEnumerable.FirstOrDefault();

                                                // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM ABOVE CODE


                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 8-12-2020
                                                var PresentDateTime_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "PresentDateTime").Select(x => x.KEY_VALUE);
                                                resModel.PresentDateTime = PresentDateTime_IEnumerable == null ? String.Empty : PresentDateTime_IEnumerable.FirstOrDefault();

                                                if (!String.IsNullOrEmpty(resModel.PresentDateTime))
                                                {
                                                    resModel.PresentDateTime = resModel.PresentDateTime.Replace('-', '/');
                                                }

                                                var DocumentNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentNumber").Select(x => x.KEY_VALUE);
                                                resModel.DocumentNumber = DocumentNumber_IEnumerable == null ? String.Empty : DocumentNumber_IEnumerable.FirstOrDefault();
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 8-12-2020

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                var OtherReceiptID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "OtherReceiptID").Select(x => x.KEY_VALUE);
                                                resModel.OtherReceiptID = OtherReceiptID_IEnumerable == null ? String.Empty : OtherReceiptID_IEnumerable.FirstOrDefault();
                                                long OtherReceiptID_LONG = Convert.ToInt64(resModel.OtherReceiptID);
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021
                                                var ReceiptNumber_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "ReceiptNumber").Select(x => x.KEY_VALUE);
                                                resModel.ReceiptNumber = ReceiptNumber_IEnumerable == null ? String.Empty : ReceiptNumber_IEnumerable.FirstOrDefault();

                                                var ReceiptDateTime_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "ReceiptDateTime").Select(x => x.KEY_VALUE);
                                                resModel.ReceiptDateTime = ReceiptDateTime_IEnumerable == null ? String.Empty : ReceiptDateTime_IEnumerable.FirstOrDefault();

                                                if (!String.IsNullOrEmpty(resModel.ReceiptDateTime))
                                                {
                                                    resModel.ReceiptDateTime = resModel.ReceiptDateTime.Replace('-', '/');
                                                }
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021

                                                // FOR DISPLAYING DATA RESTORATION DETAILS
                                                resModel.IsDataRestored = true;

                                                #region FOR GENERATING SCRIPT AND SAVING AT SERVER 
                                                // CHANGED BELOW IF CONDITION BY SHUBHAM BHAGAT ON 04-07-2020 AT 10:50 PM
                                                //if (resModel.LastDocumentRegistrationNumber != null)
                                                if (FinalRegistrationNumber_ForGenerateScript != null)
                                                {
                                                    // IF INSERT SCRIPT IS ALREADY GENERATED AND SAVED ON FILE SERVER AND WAITING FOR APPROVAL THEN POPULATE ONLY APPROVE BUTTON DON'T GENERATE SCRIPT AND SAVE ON FILE SERVER 
                                                    //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null).FirstOrDefault();

                                                    //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();

                                                    // ON 10-07-2020 AT 5:30 PM ABOVE LINE COMMENTED AND LIST IS FETCHED BECAUSE WE HAVE TO GET LIST 
                                                    List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                    if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn != null)
                                                    {
                                                        if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.Count() == 0)
                                                        {
                                                            #region BELOW CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021 ADD BUTTON TO GENERATE SCRIPT
                                                            ////if (model.Is_GenerateScriptBTN_UI)
                                                            ////{
                                                            //resModel.Is_GenerateScriptBTN_UI = true;
                                                            //if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                            //    resModel.GenerateScriptBTN_UI = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=GenerateScriptBySR('" + model.SROfficeID + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Generate Script</button>";
                                                            //else
                                                            //    resModel.GenerateScriptBTN_UI = "Script need to be generated by sub registrar.";
                                                            //return resModel;
                                                            ////}
                                                            #endregion

                                                            //int a = dbContext.USP_GenerateDignosticData_D_ECDATA_SB(model.SROfficeID, resModel.LastDocumentRegistrationNumber);

                                                            // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                            using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                            {
                                                                string format = "{0} : {1}";
                                                                file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-Before-USP_GenerateDignosticData_D_ECDATA");
                                                                file.Flush();
                                                            }
                                                            // ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                                                            using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                                                            {
                                                                using (SqlCommand command = new SqlCommand("USP_GenerateDignosticData_D_ECDATA", connection))
                                                                {
                                                                    connection.Open();

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021
                                                                    String DBRESGenerateScriptCommandTimeout = ConfigurationManager.AppSettings["DBRESGenerateScriptCommandTimeout"];
                                                                    command.CommandTimeout = Convert.ToInt32(DBRESGenerateScriptCommandTimeout);
                                                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-04-2021

                                                                    command.CommandType = CommandType.StoredProcedure;

                                                                    command.Parameters.AddWithValue("@SROCODE", SqlDbType.Int).Value = model.SROfficeID;

                                                                    // COMMENTED AND ADDED BELOW PARAMETERS ON 07-07-2020 AT 2:55 PM
                                                                    //command.Parameters.AddWithValue("@FRN", SqlDbType.VarChar).Value = FinalRegistrationNumber_ForGenerateScript;                                                             
                                                                    command.Parameters.AddWithValue("@documentID", SqlDbType.Int).Value = DocumentID_INT;

                                                                    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                                                                    command.Parameters.AddWithValue("@RegistrationID", SqlDbType.Int).Value = RegistrationID_INT;
                                                                    command.Parameters.AddWithValue("@NoticeID", SqlDbType.Int).Value = NoticeID_INT;
                                                                    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                                                                    command.Parameters.AddWithValue("@Init_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                    // COMMENTED AND ADDED ABOVE PARAMETERS ON 07-07-2020 AT 2:55 PM

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                                    command.Parameters.AddWithValue("@ReceiptID", SqlDbType.Int).Value = resModel.OtherReceiptID;
                                                                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021

                                                                    SqlDataReader detailsReader = command.ExecuteReader();
                                                                    DataTable scriptsDetails = new DataTable();
                                                                    scriptsDetails.Load(detailsReader);

                                                                    StringBuilder scriptText_STR = new StringBuilder();

                                                                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 10-08-2020
                                                                    String IsErrorMsg_String = Convert.ToString(scriptsDetails.Rows[scriptsDetails.Rows.Count - 2]["StrQuery"]);
                                                                    if (IsErrorMsg_String.Contains("Error occured for table"))
                                                                    {
                                                                        // FOR FETCHING LAST ERROR FOR SAME INIT_ID IF THE ERROR IS
                                                                        // SAME THEN DON'T SAVE LATEST ERROR IN DB_RES_ACTIONS
                                                                        List<DB_RES_ACTIONS> scriptGeneraError_ActionList = dbContext.DB_RES_ACTIONS.Where(x =>
                                                                                                x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                                                                                x.IS_SUCCESSFUL == false &&
                                                                                                x.ACTION_ID == (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript
                                                                                                ).ToList();

                                                                        if (scriptGeneraError_ActionList != null)
                                                                        {
                                                                            // ORDER BY DESC ACCORDING TO ACTION_DATETIME
                                                                            scriptGeneraError_ActionList = scriptGeneraError_ActionList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                                                            // FETCHING PREVIOUS ACTION MODEL IF EXIST
                                                                            DB_RES_ACTIONS previousScriGeneActionModel = scriptGeneraError_ActionList.FirstOrDefault();
                                                                            if (previousScriGeneActionModel != null)
                                                                            {
                                                                                // IF PREVIOUS ERROR IS SAME AS CURRENT ERROR THEN DON'T SAVE CURRENT ERROR 
                                                                                if (!previousScriGeneActionModel.ERROR_DESCRIPTION.Equals(IsErrorMsg_String))
                                                                                {
                                                                                    // FOR SAVING ERROR MESSAGE IN DB_RES_ACTIONS TABLE
                                                                                    DB_RES_ACTIONS scriptGeneraErrorActionModel = new DB_RES_ACTIONS();
                                                                                    scriptGeneraErrorActionModel.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                    scriptGeneraErrorActionModel.ACTION_ID = (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                    scriptGeneraErrorActionModel.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                    scriptGeneraErrorActionModel.SCRIPT_ID = null;
                                                                                    scriptGeneraErrorActionModel.ACTION_DATETIME = DateTime.Now;
                                                                                    scriptGeneraErrorActionModel.IS_SUCCESSFUL = false;
                                                                                    scriptGeneraErrorActionModel.ERROR_DESCRIPTION = IsErrorMsg_String;
                                                                                    scriptGeneraErrorActionModel.CORRECTIVEACTION = null;

                                                                                    // SAVE 
                                                                                    dbContext.DB_RES_ACTIONS.Add(scriptGeneraErrorActionModel);
                                                                                    dbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            // IF ERROR OCCURED FIRST TIME FOR INIT_ID
                                                                            else
                                                                            {
                                                                                // FOR SAVING ERROR MESSAGE IN DB_RES_ACTIONS TABLE
                                                                                DB_RES_ACTIONS scriptGeneraErrorActionModel = new DB_RES_ACTIONS();
                                                                                scriptGeneraErrorActionModel.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                scriptGeneraErrorActionModel.ACTION_ID = (int)Common.ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                scriptGeneraErrorActionModel.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                scriptGeneraErrorActionModel.SCRIPT_ID = null;
                                                                                scriptGeneraErrorActionModel.ACTION_DATETIME = DateTime.Now;
                                                                                scriptGeneraErrorActionModel.IS_SUCCESSFUL = false;
                                                                                scriptGeneraErrorActionModel.ERROR_DESCRIPTION = IsErrorMsg_String;
                                                                                scriptGeneraErrorActionModel.CORRECTIVEACTION = null;

                                                                                // SAVE 
                                                                                dbContext.DB_RES_ACTIONS.Add(scriptGeneraErrorActionModel);
                                                                                dbContext.SaveChanges();
                                                                            }
                                                                        }

                                                                        // FLAG TO DISPLAY SCRIPT GENERATION ERROR 
                                                                        resModel.IsScriptGenerationError = true;

                                                                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                        {
                                                                            // FLAG TO DISPLAY SCRIPT GENERATION ERROR MESSAGE
                                                                            resModel.IsScriptGenerationMsg = "Some error occured while generating scripts.";
                                                                        }
                                                                        else
                                                                        {
                                                                            // FLAG TO DISPLAY SCRIPT GENERATION ERROR MESSAGE
                                                                            resModel.IsScriptGenerationMsg = IsErrorMsg_String;
                                                                        }
                                                                        resModel.dB_RES_TABLE_WISE_COUNT_List = new List<DB_RES_TABLE_WISE_COUNT>();
                                                                        resModel.dB_RES_ACTIONsForErrorHistory = new List<DB_RES_ACTIONS_CLASS>();
                                                                        resModel.ScriptExecutionErrorMsg = String.Empty;
                                                                        resModel.DataInsertionDetailsList = new List<DataInsertionDetails>();

                                                                        return resModel;
                                                                    }
                                                                    // ABOVE CODE ADDED BU SHUBHAM BHAGAT ON 10-08-2020

                                                                    if (!resModel.IsScriptGenerationError)
                                                                    {

                                                                        for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                                                                        {
                                                                            scriptText_STR = scriptText_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["StrQuery"]));
                                                                        }


                                                                        ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                                                                        {
                                                                            IsDro = false,
                                                                            SROCode = model.SROfficeID,
                                                                            // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                                                                            //ScriptContent = ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString())
                                                                            ScriptContent = EncryptScript(ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString()))
                                                                            // FOR ENCRYPTION AND DECRYPTION ADDED ON 06-08-2020
                                                                        };

                                                                        // CREATE SERVICE OBJECT 
                                                                        SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();

                                                                        // CALL SAVE GENERATE SCRIPT FILE AT FILE SERVER USING SERVICE

                                                                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                                        using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                                        {
                                                                            string format = "{0} : {1}";
                                                                            file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                            file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                            file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-BEFORE-service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel) to save the script at the file server");
                                                                            file.Flush();
                                                                        }
                                                                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                                        ScriptSaveRESModel scriptSaveRESModel = service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel);

                                                                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                                        using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                                        {
                                                                            string format = "{0} : {1}";
                                                                            file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                            file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                            file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-AFTER-service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel) to save the script at the file server");
                                                                            file.Flush();
                                                                        }
                                                                        // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                                        if (scriptSaveRESModel != null)
                                                                        {
                                                                            // IF SCRIPT SAVED SUCCEFULLY AND ERROR MESSAGE IS EMPTY THEN 
                                                                            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                            // WITH  SUCCESS STATUS
                                                                            if (scriptSaveRESModel.IsScriptSavedSuccessfully && scriptSaveRESModel.ErrorMsg == String.Empty)
                                                                            {
                                                                                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        #region WORK DONE FOR ADDING DUMMY DATA BY SHUBHAM ON 30-09-2020


                                                                                        //// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        ////RegistrationID_INT
                                                                                        ////DocumentID_INT
                                                                                        ////NoticeID_INT
                                                                                        //var DocumentMasterOnCentralList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT).ToList();
                                                                                        //var MarriageRegOnCentralList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT).ToList();
                                                                                        //var NoticeMasterOnCentralList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT).ToList();

                                                                                        //int DocumentMasterOnCentralCount = 0;
                                                                                        //int MarriageRegOnCentralCount = 0;
                                                                                        //int NoticeMasterOnCentralCount = 0;

                                                                                        //if (DocumentMasterOnCentralList != null)
                                                                                        //{
                                                                                        //    if (DocumentMasterOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        DocumentMasterOnCentralCount = DocumentMasterOnCentralList.Count;
                                                                                        //    }
                                                                                        //}
                                                                                        //if (MarriageRegOnCentralList != null)
                                                                                        //{
                                                                                        //    if (MarriageRegOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        MarriageRegOnCentralCount = MarriageRegOnCentralList.Count;
                                                                                        //    }
                                                                                        //}
                                                                                        //if (NoticeMasterOnCentralList != null)
                                                                                        //{
                                                                                        //    if (NoticeMasterOnCentralList.Count > 0)
                                                                                        //    {
                                                                                        //        NoticeMasterOnCentralCount = NoticeMasterOnCentralList.Count;
                                                                                        //    }
                                                                                        //}

                                                                                        //// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020


                                                                                        //// ADDED IF BLOCK BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        //// BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        //if (DocumentMasterOnCentralCount > 0 || MarriageRegOnCentralCount > 0 || NoticeMasterOnCentralCount > 0)
                                                                                        //{
                                                                                        //    // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        //    int SCRIPT_ID_For_ADD = 0;
                                                                                        //    DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        //    SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        //    save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        //    save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        //    save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        //    save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                                        //    save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        //    save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        //    save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        //    save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        //    dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        //    dbContext.SaveChanges();

                                                                                        //    // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        //    DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        //    saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        //    saveGenerateActionDetails.INIT_ID = null;
                                                                                        //    saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        //    saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        //    saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                                        //    saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        //    saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                                        //    dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        //    dbContext.SaveChanges();

                                                                                        //    dbContextTransaction.Commit();

                                                                                        //    // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                                        //    resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                                                                                        //    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                                        //    resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";
                                                                                        //}
                                                                                        //// ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                                                                                        //// BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        //else
                                                                                        //{
                                                                                        //    // FOR DUMMY ENTRY 

                                                                                        //    dbContextTransaction.Commit();

                                                                                        //    // POPULATE EMPTY BUTTON IN CASE OF DUMMY DATA
                                                                                        //    resModel.ApproveBtn = String.Empty;

                                                                                        //    // POPULATE EMPTY BUTTON IN CASE OF DUMMY DATA
                                                                                        //    resModel.NoteForApprovalForSR = String.Empty;

                                                                                        //}
                                                                                        #endregion

                                                                                        //BELOW CODE COMMENTED AND SHIFTED BY SHUBHAM BHAGAT ON 30 - 09 - 2020
                                                                                        // IN IF BLOCK BECAUSE FOR INSERTING DUMMY DATA WE HAVE USE STORED PROCEDURE IN ELSE BLOCK
                                                                                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        int SCRIPT_ID_For_ADD = 0;
                                                                                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        dbContext.SaveChanges();

                                                                                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        saveGenerateActionDetails.INIT_ID = null;
                                                                                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        dbContext.SaveChanges();

                                                                                        dbContextTransaction.Commit();

                                                                                        // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                                        resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";


                                                                                    }
                                                                                    catch (Exception)
                                                                                    {
                                                                                        dbContextTransaction.Rollback();
                                                                                    }
                                                                                }
                                                                            }
                                                                            // IF SCRIPT NOT SAVED SUCCEFULLY AND ERROR MESSAGE IS NOT EMPTY THEN 
                                                                            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                            // WITH  FALSE SUCCESS STATUS AND ERROR MESSAGE
                                                                            else
                                                                            {
                                                                                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        //DocumentID_INT
                                                                                        //reg

                                                                                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                                        int SCRIPT_ID_For_ADD = 0;
                                                                                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                                        save_Script_DETAILS.SCRIPT_PATH = String.Empty;
                                                                                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                                        dbContext.SaveChanges();

                                                                                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                                        saveGenerateActionDetails.INIT_ID = null;
                                                                                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                                        saveGenerateActionDetails.IS_SUCCESSFUL = false;
                                                                                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                                        saveGenerateActionDetails.ERROR_DESCRIPTION = scriptSaveRESModel.ErrorMsg;

                                                                                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                                        dbContext.SaveChanges();

                                                                                        dbContextTransaction.Commit();
                                                                                    }
                                                                                    catch (Exception)
                                                                                    {
                                                                                        dbContextTransaction.Rollback();
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            // ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                                                            using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                                                            {
                                                                string format = "{0} : {1}";
                                                                file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                                                                file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                                                                file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-After-USP_GenerateDignosticData_D_ECDATA");
                                                                file.Flush();
                                                            }
                                                            // ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                                                            dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                        }

                                                        if (dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.Count() > 0)
                                                        {
                                                            dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn = dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                                            //DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                            DB_RES_INSERT_SCRIPT_DETAILS insertScriptAlreadyGenerated = dB_RES_INSERT_SCRIPT_DETAILSListForApproveBtn.FirstOrDefault();

                                                            if (insertScriptAlreadyGenerated != null)
                                                            {
                                                                // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                // IF SCRIPT GENERATED AND SAVED BUT NOT APPROVED THEN POPULATE APPROVE BUTTON 
                                                                if (insertScriptAlreadyGenerated.SCRIPT_APPROVAL_DATETIME == null)
                                                                {
                                                                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                                                                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                    {
                                                                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + insertScriptAlreadyGenerated.SCRIPT_ID + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";
                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                        resModel.NoteForApprovalForSR = "<label style='font-size:initial;font-weight:bold;'>Note : Following data will be restored into Sr office database from central database. Kindly review and approve</label>";
                                                                    }
                                                                    else
                                                                        resModel.ApproveBtn = String.Empty;
                                                                    // FOR SETTING STATUS OF SCRIPT
                                                                    IsScriptReadyForInsertion = false;
                                                                }
                                                                // IF SCRIPT GENERATED AND SAVED BUT APPROVED THEN DON'T POPULATE APPROVE BUTTON 
                                                                else
                                                                {
                                                                    // resModel.ApproveBtn = "";
                                                                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                    // ADDED IF ELSE CONDITION BECAUSE FOR TECHADMIN WE HAVE TO DRAW EMPTY BUTTON AND FOR SR WE HAVE TO SHOW MESSAGE AFTER APPROVAL
                                                                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                                                                    {
                                                                        // COMMENTED AND CHANGED ON 07-08-2020 AT 4:30 PM
                                                                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020
                                                                        // ADDED CONDITION TO HIDE APPROVE MESSAGE IF SCRIPT IS EXECUTED AND ERROR IS OCCURED
                                                                        if (insertScriptAlreadyGenerated.SCRIPT_EXECUTION_DATETIME != null && insertScriptAlreadyGenerated.IS_SUCCESSFUL == false)
                                                                        {
                                                                            resModel.ApproveBtn = String.Empty;
                                                                        }
                                                                        else
                                                                        {
                                                                            resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask system integrator to execute restoration utility to restore above data.</label>";
                                                                        }
                                                                        //resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask system integrator to execute restoration utility to restore above data.</label>";
                                                                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020


                                                                        //resModel.ApproveBtn = "<label style='font-size:initial;font-weight:bold;'>Please ask support engineer to execute restoration utility to restore above data.</label>";
                                                                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                        resModel.NoteForApprovalForSR = String.Empty;
                                                                    }
                                                                    else
                                                                        resModel.ApproveBtn = String.Empty;

                                                                    // FOR SETTING STATUS OF SCRIPT
                                                                    IsScriptReadyForInsertion = true;
                                                                }
                                                            }
                                                            // IF INSERT SCRIPT IS NOT GENERATED THEN GENERATE SCRIPT AND SAVE IT ON FILE SERVER AND ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_ACTIONS TABLE
                                                            else
                                                            {
                                                                // FOR SETTING STATUS OF SCRIPT
                                                                IsScriptReadyForInsertion = false;

                                                                ////int a = dbContext.USP_GenerateDignosticData_D_ECDATA_SB(model.SROfficeID, resModel.LastDocumentRegistrationNumber);
                                                                //using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                                                                //{
                                                                //    using (SqlCommand command = new SqlCommand("USP_GenerateDignosticData_D_ECDATA", connection))
                                                                //    {
                                                                //        connection.Open();
                                                                //        command.CommandType = CommandType.StoredProcedure;

                                                                //        command.Parameters.AddWithValue("@SROCODE", SqlDbType.Int).Value = model.SROfficeID;

                                                                //        // COMMENTED AND ADDED BELOW PARAMETERS ON 07-07-2020 AT 2:55 PM
                                                                //        //command.Parameters.AddWithValue("@FRN", SqlDbType.VarChar).Value = FinalRegistrationNumber_ForGenerateScript;
                                                                //        command.Parameters.AddWithValue("@documentID", SqlDbType.Int).Value = DocumentID_INT;
                                                                //        command.Parameters.AddWithValue("@Init_ID", SqlDbType.Int).Value = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //        // COMMENTED AND ADDED ABOVE PARAMETERS ON 07-07-2020 AT 2:55 PM

                                                                //        SqlDataReader detailsReader = command.ExecuteReader();
                                                                //        DataTable scriptsDetails = new DataTable();
                                                                //        scriptsDetails.Load(detailsReader);

                                                                //        StringBuilder scriptText_STR = new StringBuilder();

                                                                //        for (int i = 0; i < scriptsDetails.Rows.Count; i++)
                                                                //        {
                                                                //            scriptText_STR = scriptText_STR.AppendLine(Convert.ToString(scriptsDetails.Rows[i]["StrQuery"]));
                                                                //        }

                                                                //        #region FOR SAVING FILE LOCALLY
                                                                //        ////var path = @"F:\2020\GeneratedScript.txt";
                                                                //        ////var path = @"F:\2020\generatescripttesting.txt";
                                                                //        //var path = @"F:\2020\generated_script_3-07-2020.txt";

                                                                //        ////string text = "old falcon";
                                                                //        //File.WriteAllText(path, scriptText_STR.ToString()); 
                                                                //        #endregion

                                                                //        //scriptText_STR.
                                                                //        //ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString());
                                                                //        //model.SROfficeID

                                                                //        ScriptSaveREQModel scriptSaveREQModel = new ScriptSaveREQModel()
                                                                //        {
                                                                //            IsDro = false,
                                                                //            SROCode = model.SROfficeID,
                                                                //            ScriptContent = ASCIIEncoding.ASCII.GetBytes(scriptText_STR.ToString())
                                                                //        };

                                                                //        // CREATE SERVICE OBJECT 
                                                                //        SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();

                                                                //        // CALL SAVE GENERATE SCRIPT FILE AT FILE SERVER USING SERVICE
                                                                //        ScriptSaveRESModel scriptSaveRESModel = service.SaveGeneratedScriptFileForDB_RES(scriptSaveREQModel);
                                                                //        if (scriptSaveRESModel != null)
                                                                //        {
                                                                //            // IF SCRIPT SAVED SUCCEFULLY AND ERROR MESSAGE IS EMPTY THEN 
                                                                //            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                //            // WITH  SUCCESS STATUS
                                                                //            if (scriptSaveRESModel.IsScriptSavedSuccessfully && scriptSaveRESModel.ErrorMsg == String.Empty)
                                                                //            {
                                                                //                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                //                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                //                {
                                                                //                    try
                                                                //                    {
                                                                //                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                //                        int SCRIPT_ID_For_ADD = 0;
                                                                //                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                //                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                //                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                //                        save_Script_DETAILS.SCRIPT_PATH = scriptSaveRESModel.SavedScriptFilePath;
                                                                //                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                //                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                //                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                //                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                //                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                //                        dbContext.SaveChanges();

                                                                //                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                //                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                //                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                //                        saveGenerateActionDetails.INIT_ID = null;
                                                                //                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                //                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                //                        saveGenerateActionDetails.IS_SUCCESSFUL = true;
                                                                //                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        saveGenerateActionDetails.ERROR_DESCRIPTION = null;

                                                                //                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                //                        dbContext.SaveChanges();

                                                                //                        dbContextTransaction.Commit();

                                                                //                        // POPULATE APPROVE BUTTON FOR SRO AFTER SCRIPT SAVED AT FILE SERVER
                                                                //                        resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";
                                                                //                    }
                                                                //                    catch (Exception)
                                                                //                    {
                                                                //                        dbContextTransaction.Rollback();
                                                                //                    }
                                                                //                }
                                                                //            }
                                                                //            // IF SCRIPT NOT SAVED SUCCEFULLY AND ERROR MESSAGE IS NOT EMPTY THEN 
                                                                //            // ADD BOTH DB_RES_INSERT_SCRIPT_DETAILS AND DB_RES_SERVICE_COMM_DETAILS
                                                                //            // WITH  FALSE SUCCESS STATUS AND ERROR MESSAGE
                                                                //            else
                                                                //            {
                                                                //                //using (DbContextTransaction dbContextTransaction = dbContext.Database.CurrentTransaction)
                                                                //                using (DbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                                                                //                {
                                                                //                    try
                                                                //                    {
                                                                //                        // ADD ENTRY IN DB_RES_INSERT_SCRIPT_DETAILS TABLE
                                                                //                        int SCRIPT_ID_For_ADD = 0;
                                                                //                        DB_RES_INSERT_SCRIPT_DETAILS save_Script_DETAILS = new DB_RES_INSERT_SCRIPT_DETAILS();
                                                                //                        SCRIPT_ID_For_ADD = (dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Any() ? dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Max(a => a.SCRIPT_ID) : 0) + 1;
                                                                //                        save_Script_DETAILS.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        save_Script_DETAILS.INIT_ID = dB_RES_INITIATE_MASTER.INIT_ID;
                                                                //                        save_Script_DETAILS.ITERATION_ID = 1;
                                                                //                        save_Script_DETAILS.SCRIPT_PATH = String.Empty;
                                                                //                        save_Script_DETAILS.SCRIPT_CREATION_DATETIME = DateTime.Now;
                                                                //                        save_Script_DETAILS.SCRIPT_APPROVAL_DATETIME = null;
                                                                //                        save_Script_DETAILS.SCRIPT_EXECUTION_DATETIME = null;
                                                                //                        save_Script_DETAILS.IS_SUCCESSFUL = false;

                                                                //                        dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Add(save_Script_DETAILS);
                                                                //                        dbContext.SaveChanges();

                                                                //                        // ADD ENTRY IN DB_RES_ACTIONS TABLE
                                                                //                        DB_RES_ACTIONS saveGenerateActionDetails = new DB_RES_ACTIONS();
                                                                //                        saveGenerateActionDetails.ROW_ID = (dbContext.DB_RES_ACTIONS.Any() ? dbContext.DB_RES_ACTIONS.Max(a => a.ROW_ID) : 0) + 1;
                                                                //                        saveGenerateActionDetails.INIT_ID = null;
                                                                //                        saveGenerateActionDetails.ACTION_ID = (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript;
                                                                //                        saveGenerateActionDetails.ACTION_DATETIME = DateTime.Now;
                                                                //                        saveGenerateActionDetails.IS_SUCCESSFUL = false;
                                                                //                        saveGenerateActionDetails.SCRIPT_ID = SCRIPT_ID_For_ADD;
                                                                //                        saveGenerateActionDetails.ERROR_DESCRIPTION = scriptSaveRESModel.ErrorMsg;

                                                                //                        dbContext.DB_RES_ACTIONS.Add(saveGenerateActionDetails);
                                                                //                        dbContext.SaveChanges();

                                                                //                        dbContextTransaction.Commit();
                                                                //                    }
                                                                //                    catch (Exception)
                                                                //                    {
                                                                //                        dbContextTransaction.Rollback();
                                                                //                    }
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //    }
                                                                //}
                                                            }
                                                        }
                                                    }

                                                }
                                                #endregion


                                                // Data Insertion Details : 
                                                #region Data Insertion Details: 
                                                // MEMORY ALLOCATION FOR Data Insertion Details LIST
                                                resModel.DataInsertionDetailsList = new List<DataInsertionDetails>();

                                                //// LastDocumentID
                                                //var DocumentID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE);
                                                //String DocumentID_STR = DocumentID_IEnumerable == null ? String.Empty : DocumentID_IEnumerable.FirstOrDefault();

                                                //// LastRegistrationID
                                                //var RegistrationID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE);
                                                //String RegistrationID_STR = RegistrationID_IEnumerable == null ? String.Empty : RegistrationID_IEnumerable.FirstOrDefault();

                                                //// LastNoticeID
                                                //var NoticeID_IEnumerable = db_RES_SERVICE_COMM_DETAILSList.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE);
                                                //String NoticeID_STR = NoticeID_IEnumerable == null ? String.Empty : NoticeID_IEnumerable.FirstOrDefault();


                                                // BEFORE THIS GENERATE SCRIPT AND SAVE IT ON SERVER AND SAVE ENTRY IN DATABASE
                                                // SET COMMON ITERATION
                                                int ITERATION_ID_INT = 0;
                                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME == null && x.SCRIPT_EXECUTION_DATETIME == null).FirstOrDefault();
                                                // COMMENETED ABOVE CODE BY SHUBHAM BHAGAT ON 04-07-2020 AT 9:45 PM BECAUSE AFTER APPROVAL AND EXECUTION ABOVE QUERY WILL NOT FETCH ITERATION ID


                                                //DB_RES_INSERT_SCRIPT_DETAILS dB_RES_INSERT_SCRIPT_DETAILS = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).FirstOrDefault();
                                                // COMMENTED AND CHANGED CONDITION ON 10-07-2020 AT 3:45 PM 
                                                // COMMENTED ABOVE LINE BECAUSE IT WILL FETCH FIRST ENTRY WE SHOULD FETCH ALL SCRIPTS LIST ASSOCIATED WITH INITID THEN GET MAX SCRIPTID OBJECT FOR OPERATIOSN
                                                List<DB_RES_INSERT_SCRIPT_DETAILS> dB_RES_INSERT_SCRIPT_DETAILSList = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                                                if (dB_RES_INSERT_SCRIPT_DETAILSList != null)
                                                {
                                                    dB_RES_INSERT_SCRIPT_DETAILSList = dB_RES_INSERT_SCRIPT_DETAILSList.OrderByDescending(x => x.SCRIPT_ID).ToList();

                                                    DB_RES_INSERT_SCRIPT_DETAILS latestInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.FirstOrDefault();
                                                    if (latestInsertedScript != null)
                                                    {
                                                        ITERATION_ID_INT = latestInsertedScript.ITERATION_ID ?? 0;

                                                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 05-08-2020 AT 4:00 PM 
                                                        // IF ITERATION ID IS MORE THAN 1 THEN ONLY SHOW SCRIPT RECTIFICATION HISTORY
                                                        if (ITERATION_ID_INT > 1)
                                                        {
                                                            // SKIP CURRENT SCRIPT AND FETCH PREVIOUS SCRIPT OBJECT
                                                            DB_RES_INSERT_SCRIPT_DETAILS previousInsertedScript = dB_RES_INSERT_SCRIPT_DETAILSList.Skip(1).Take(1).FirstOrDefault();
                                                            if (previousInsertedScript != null)
                                                            {
                                                                // IT WILL ALWAYS CONTAIN 1 ELEMENT BUT WE ARE FETCHING LIST
                                                                List<DB_RES_ACTIONS> scriptRectiHistyList = dbContext.DB_RES_ACTIONS.Where(
                                                                                    x => x.SCRIPT_ID == previousInsertedScript.SCRIPT_ID &&
                                                                                    x.IS_SUCCESSFUL == false).ToList();

                                                                if (scriptRectiHistyList != null)
                                                                {
                                                                    foreach (var item in scriptRectiHistyList)
                                                                    {
                                                                        // CONCATINATING THE SCRIPT RECTIFICATION HISTORY
                                                                        resModel.ScriptRectificationHistory = resModel.ScriptRectificationHistory + ". " + item.CORRECTIVEACTION;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 05-08-2020 AT 4:00 PM


                                                        // ADDED ON 10-07-2020 AT 3:03 PM
                                                        // ADDED FOR FETCHING ERROR MESSAGE FOR TECHADMIN AND SR TO SHOW IN STATUS COLUMN 
                                                        DB_RES_ACTIONS errorInScriptExecution = dbContext.DB_RES_ACTIONS.Where(x => x.SCRIPT_ID == latestInsertedScript.SCRIPT_ID && x.IS_SUCCESSFUL == false && (
                                                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript || x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                                        if (errorInScriptExecution != null)
                                                        {
                                                            IsScriptExecutedWithError = true;
                                                            scriptExecutionErrorMsg = errorInScriptExecution.ERROR_DESCRIPTION;
                                                        }

                                                        DB_RES_ACTIONS successInScriptExecution = dbContext.DB_RES_ACTIONS.Where(x => x.SCRIPT_ID == latestInsertedScript.SCRIPT_ID && x.IS_SUCCESSFUL == true && (
                                                        x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript || x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                                        if (successInScriptExecution != null)
                                                        {
                                                            IsScriptExecutedSuccessfully = true;
                                                        }
                                                    }
                                                }

                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                // BELOW CODE ADDED TO SHOW ERROR MESSAGE TO TECHADMIN WHEN ERROR
                                                // OCCURED WHILE EXECUTING SCRIPT AT SR OFFICE BY HCL USER
                                                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                                                {
                                                    resModel.ScriptExecutionErrorMsg = IsScriptExecutedWithError ? scriptExecutionErrorMsg : String.Empty;
                                                }
                                                else
                                                {
                                                    resModel.ScriptExecutionErrorMsg = String.Empty;
                                                }


                                                // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                // TO CHANGE HIDE ITERATION NUMBER COLUMN AND ADD SERIAL NO COLUMN
                                                int SerialNoCounter = 1;

                                                // GET DOCUMENT CENTRALIZED AT CENTRALIZED SERVER
                                                //int DocumentID_INT = Convert.ToInt32(DocumentID_STR);
                                                List<DocumentMaster> DocumentMasterList = dbContext.DocumentMaster.Where(x => x.DocumentID > DocumentID_INT && x.SROCode == model.SROfficeID).ToList();
                                                if (DocumentMasterList != null)
                                                {
                                                    if (DocumentMasterList.Count() > 0)
                                                    {
                                                        foreach (var item in DocumentMasterList)
                                                        {
                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                            // FOR DISPLAYING COUNT OF DOCUMENT MASTER IN SUMMARY TABLE
                                                            resModel.Summary_Tbl_DM_Count = DocumentMasterList.Count();

                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Document";
                                                            dataInsertionDetails.RegistrationNumber = item.FinalRegistrationNumber == null ? String.Empty : item.FinalRegistrationNumber;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.Stamp5DateTime == null ? String.Empty : ((DateTime)item.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // GET MARRIAGE CENTRALIZED AT CENTRALIZED SERVER
                                                //int RegistrationID_INT = Convert.ToInt32(RegistrationID_STR);

                                                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                                // COMPARED WITH PSROCode INSTEAD OF SROCode
                                                //List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.SROCode == model.SROfficeID).ToList();
                                                List<MarriageRegistration> MarriageRegistrationList = dbContext.MarriageRegistration.Where(x => x.RegistrationID > RegistrationID_INT && x.PSROCode == model.SROfficeID).ToList();
                                                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                                if (MarriageRegistrationList != null)
                                                {
                                                    if (MarriageRegistrationList.Count() > 0)
                                                    {
                                                        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                        // FOR DISPLAYING COUNT OF MARRIAGE REGISTRATION IN SUMMARY TABLE
                                                        resModel.Summary_Tbl_MR_Count = MarriageRegistrationList.Count();

                                                        foreach (var item in MarriageRegistrationList)
                                                        {
                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Marriage";
                                                            dataInsertionDetails.RegistrationNumber = item.MarriageCaseNo == null ? String.Empty : item.MarriageCaseNo;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.DateOfRegistration == null ? String.Empty : item.DateOfRegistration.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // GET NOTICE CENTRALIZED AT CENTRALIZED SERVER                                                
                                                //int NoticeID_INT = Convert.ToInt32(NoticeID_STR);

                                                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020
                                                // COMPARED WITH PSROCode INSTEAD OF SROCode
                                                //List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.SROCode == model.SROfficeID).ToList();
                                                List<NoticeMaster> NoticeMasterList = dbContext.NoticeMaster.Where(x => x.NoticeID > NoticeID_INT && x.PSROCode == model.SROfficeID).ToList();
                                                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 1-12-2020

                                                if (NoticeMasterList != null)
                                                {
                                                    if (NoticeMasterList.Count() > 0)
                                                    {
                                                        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM
                                                        // FOR DISPLAYING COUNT OF NOTICE MASTER IN SUMMARY TABLE
                                                        resModel.Summary_Tbl_NM_Count = NoticeMasterList.Count();

                                                        foreach (var item in NoticeMasterList)
                                                        {
                                                            DataInsertionDetails dataInsertionDetails = new DataInsertionDetails();
                                                            dataInsertionDetails.IterationNo = ITERATION_ID_INT;
                                                            dataInsertionDetails.RegistrationType = "Notice";
                                                            dataInsertionDetails.RegistrationNumber = item.NoticeNo == null ? String.Empty : item.NoticeNo;
                                                            // ADDED BY SHUBHAM BHAGAT ON 14-07-2020 TO CHANGE DATETIME FORMAT FROM - TO /
                                                            dataInsertionDetails.RegistrationDate = item.NoticeIssuedDate == null ? String.Empty : item.NoticeIssuedDate.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                            // dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            // ABOVE LINE COMMENTED AND BELOW CONDITION ADDED ON 10-07-2020 AT 4:00 PM

                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN BELOW CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT
                                                            dataInsertionDetails.Status = IsScriptReadyForInsertion ? "Ready for Insertion" : "Pending for Approval";
                                                            if (IsScriptExecutedWithError)
                                                            {
                                                                // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                                                                //dataInsertionDetails.Status = scriptExecutionErrorMsg;
                                                                dataInsertionDetails.Status = "Error occured";
                                                            }
                                                            else if (IsScriptExecutedSuccessfully)
                                                                dataInsertionDetails.Status = "Restored";
                                                            // ADDED COMMENT ON 15-07-2020
                                                            // IN ABOVE CODE WE HAVE TO SET  STATUS OF SCRIPT INSERT

                                                            dataInsertionDetails.ScriptRectificationHistory = String.Empty;

                                                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:45 PM
                                                            dataInsertionDetails.SrNo = SerialNoCounter++;

                                                            // ADD IN LIST
                                                            resModel.DataInsertionDetailsList.Add(dataInsertionDetails);
                                                        }
                                                    }
                                                }

                                                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                IQueryable<ReceiptMaster> receiptMasterRecords = dbContext.ReceiptMaster.Where(x => x.ReceiptID > OtherReceiptID_LONG && x.SROCode == model.SROfficeID);
                                                // FOR DISPLAYING COUNT OF Other Receipts IN SUMMARY TABLE
                                                resModel.Summary_Tbl_OtherReceipt_Count = receiptMasterRecords.Count();
                                                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
                                                #endregion
                                            }
                                        }

                                        //DateTime actionDateTime = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                        //x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).FirstOrDefault().Select(new { a=x.ACTION_DATETIME
                                        //});
                                        //var actionDateTime = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                                        //x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID).Select(x=>new
                                        //{
                                        //    a = x.ACTION_DATETIME
                                        //});
                                    }
                                }
                            }


                        }

                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                        // ADDED BELOW CODE ON 15-07-2020 AT 07:01 PM
                        // ADDED BELOW CODE TO SHOW ERROR MESSAGE WHEN ERROR OCCURED WHILE RESTORING 
                        // BELOW CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        else if (dB_RES_INITIATE_MASTER.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataRestorationInitiated)
                        //else if (dB_RES_INITIATE_MASTER.STATUS_ID == null)
                        // ABOVE CODE IS COMMENTED AND CHANGED ON 01-08-2020 
                        {
                            //dB_RES_INITIATE_MASTER.
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where
                            //    (x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                            //    x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID &&
                            //    x.IS_SUCCESSFUL == false && x.ERROR_DESCRIPTION != null).ToList();



                            #region BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                            // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                            // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR

                            //// FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            //// ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            //// WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            //// TIMES THEN WE SHOULD FETCH LASTET 6 ROWS FROM ACTIONS TABLE
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where
                            // (x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID &&
                            // x.ACTION_ID <= (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID &&
                            // x.IS_SUCCESSFUL == false).ToList();



                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                            // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val

                            //// FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            //// ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            //// WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            //// TIMES THEN WE SHOULD FETCH LASTET 7 ROWS FROM ACTIONS TABLE
                            //List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                            //                                        x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID 
                            //                                            &&
                            //                                            (
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                            //                                                x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema
                            //                                            )
                            //                                            &&  x.IS_SUCCESSFUL == false).ToList();

                            // FETCHING ACTIONS LIST FOR CURRENT INIT_ID AND HAVING 
                            // ACTION_ID LESS THEN OR EQUAL TO SendLastID AND IS_SUCCESSFUL IS FALSE
                            // WE WILL GET ALL ACTIONS IF DATABASE RESTORATION CONTINUOUSLY FAILS 2 
                            // TIMES THEN WE SHOULD FETCH LASTET 10 ROWS FROM ACTIONS TABLE
                            List<DB_RES_ACTIONS> db_RES_ACTIONSList = dbContext.DB_RES_ACTIONS.Where(
                                                                    x => x.INIT_ID == dB_RES_INITIATE_MASTER.INIT_ID
                                                                        &&
                                                                        (
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                                            x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                                                        )
                                                                        && x.IS_SUCCESSFUL == false).ToList();

                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021


                            #endregion ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                            if (db_RES_ACTIONSList != null)
                            {
                                if (db_RES_ACTIONSList.Count > 0)
                                {
                                    // ORDER BY DESCENDING
                                    db_RES_ACTIONSList = db_RES_ACTIONSList.OrderByDescending(x => x.ACTION_DATETIME).ToList();

                                    // BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                                    //// BEFORE TAKING TOP 6 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 6 ELEMENTS
                                    //if (db_RES_ACTIONSList.Count > 6)
                                    //{
                                    //    // TAKING TOP 6 ELEMENTS 
                                    //    db_RES_ACTIONSList = db_RES_ACTIONSList.Take(6).ToList();
                                    //}

                                    // BEFORE TAKING TOP 7 ELEMENTS WE WILL IF LIST CONTAINS LESS THAN 7 ELEMENTS
                                    if (db_RES_ACTIONSList.Count > 7)
                                    {
                                        // TAKING TOP 7 ELEMENTS 
                                        db_RES_ACTIONSList = db_RES_ACTIONSList.Take(7).ToList();
                                    }
                                    // ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                                    resModel.DatabaseNotRestoredErrorMsg = "<label style='font-size:initial;font-weight:bold;'>";
                                    foreach (var item in db_RES_ACTIONSList)
                                    {
                                        if (!String.IsNullOrEmpty(item.ERROR_DESCRIPTION))
                                        {
                                            resModel.IsDatabaseNotRestored = true;
                                            resModel.DatabaseNotRestoredErrorMsg = resModel.DatabaseNotRestoredErrorMsg + " " + item.ERROR_DESCRIPTION;
                                        }
                                    }
                                    resModel.DatabaseNotRestoredErrorMsg = resModel.DatabaseNotRestoredErrorMsg + " .</label>";
                                }
                            }
                        }
                        // ADDED ABOVE CODE TO SHOW ERROR MESSAGE WHEN ERROR OCCURED WHILE RESTORING

                        #endregion
                    }
                    else// IS SAME AS BELOW ELSE -IT WILL EXECUTE WHEN NO ENTRY EXIST IN DB FOR PARTICULAR SRO EXIST
                    {
                        // checked below code, it is running
                        //Initiate Date
                        //resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                        //Initiate Date btn
                        //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";

                        // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                            resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                        else
                            resModel.InitiationDateBtn = String.Empty;

                        //Generate Key Btn
                        //resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                        //Generate Key Value
                        //resModel.GenerateKeyValue = GenerateKey();//1
                        // delete space after key and span close
                        //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days only Please share this activation key to support Engineer.</label>";

                        // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                        if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                            resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                        else
                            resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                        resModel.ShowInitDateAndGeneratedKeyMsg = false;
                    }
                }
                else// IS SAME AS ABOVE ELSE 
                {
                    //Initiate Date
                    //resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                    //Initiate Date btn
                    //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";

                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                    else
                        resModel.InitiationDateBtn = String.Empty;

                    //Generate Key Btn
                    //resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                    //Generate Key Value
                    //resModel.GenerateKeyValue = GenerateKey();//1
                    //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days only Please share this activation key to support Engineer.</label>";

                    // ADDED IF SR CONDITION ON 06-07-2020 AT 3:08 PM
                    if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;color: #3177b4;'>Activation key for data restoration utility is : <span id='GenerateKeyValueSpanID'>" + GenerateKey() + "</span> which is valid for 5 days Please share this activation key to Support Engineer.</label>";
                    else
                        resModel.GenerateKeyBtnAndTextMsg = String.Empty;

                    resModel.ShowInitDateAndGeneratedKeyMsg = false;
                }

                // POPULATE DOWNLOAD BUTTON FOR TECHADMIN IF ERROR OCCURED WHILE EXECUTING SCRIPT
                if (model.CurrentRoleID == (int)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                {
                    // CHECK FOR PARTICULAR SRO IN DB_RES_INITIATE_MASTER TABLE WHERE STATUS_ID=3  I.E ErrorInDataInsertion
                    DB_RES_INITIATE_MASTER db_initMasterErrorInInsertion = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.SROCODE == model.SROfficeID && x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.ErrorInDataInsertion).FirstOrDefault();
                    if (db_initMasterErrorInInsertion != null)
                    {
                        // GET THE LIST OF DB_RES_INSERT_SCRIPT_DETAILS USING INIT_ID AND SCRIPT_CREATION_DATETIME !=NULL AND SCRIPT_APPROVAL_DATETIME!=NULL
                        // WE HAVE TO FETCH LIST BECAUSE ERROR CAN OCCUR IN RECTIFIED SCRIPT ALSO
                        //List<DB_RES_INSERT_SCRIPT_DETAILS> db_insert_script_for_recti_List = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == db_initMasterErrorInInsertion.INIT_ID && x.SCRIPT_CREATION_DATETIME != null && x.SCRIPT_APPROVAL_DATETIME != null).ToList();
                        // ABOVE CODE FOR LISTING IS COMMENTED BY SHUBHAM BHAGAT ON 10-07-2020 AT 2:45 PM 
                        // BECAUSE AFTER RECTIFIED SCRIPT IS DOWNLOADED AND RECTIFIED IT SHOULD NOT BE DOWNLOADED AND UPLOADED ANOTHER TIME
                        List<DB_RES_INSERT_SCRIPT_DETAILS> db_insert_script_for_recti_List = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == db_initMasterErrorInInsertion.INIT_ID && x.SCRIPT_CREATION_DATETIME != null).ToList();
                        if (db_insert_script_for_recti_List != null)
                        {
                            if (db_insert_script_for_recti_List.Count() > 0)
                            {
                                // AFTER GETTING LIST FETCH THE LATEST SCRIPT_ID FOR THE INIT_ID
                                int maxScriptID = db_insert_script_for_recti_List.Max(x => x.SCRIPT_ID);

                                // AFTER GETTING LATEST SCRIPT_ID CHECK IN DB_RES_ACTIONS TABLE FOR LATEST SCRIPT_ID WHERE ACTIONID = ExecuteScript OR ExecuteRecitifiedScript
                                DB_RES_ACTIONS errMSGActionModel = dbContext.DB_RES_ACTIONS.Where
                                    (x => x.SCRIPT_ID == maxScriptID && x.IS_SUCCESSFUL == false &&
                                    (x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteScript ||
                                    x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.ExecuteRecitifiedScript)).FirstOrDefault();
                                if (errMSGActionModel != null)
                                {
                                    resModel.DownloadScriptForTechAdmin = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=DownloadScriptForRectification('" + db_initMasterErrorInInsertion.INIT_ID + "','" + maxScriptID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Download</button>";
                                    resModel.UploadScriptForTechAdmin = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=UploadRectifiedScript('" + db_initMasterErrorInInsertion.INIT_ID + "','" + maxScriptID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Upload</button>";
                                }
                            }
                        }
                    }
                }

                //Pankaj work here
                #region ADDED BY PANKAJ ON 01-07-2020
                #region ADDED BY PANKAJ ON 31-07-2020
                var id = model.INIT_ID_INT;

                #region BELOW CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020
                // BECAUSE NEW ACTION "Send Database Schema" IN ACTION MASTER TABLE IS ADDED FOR 
                // FETCHING SCHEMA FOR LOCAL DATABASE AS DISCUSSED WITH SIR
                //IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(x => x.INIT_ID == id && x.ACTION_ID < (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.GenerateScript).OrderBy(X => X.ROW_ID);

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
                // BECAUSE 3 NEW ACTION ARE ADDED FOR CONSIDERING KAIGR_val
                //IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(
                //                                            x => x.INIT_ID == id &&
                //                                               (
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                //                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema

                //                                               )).OrderBy(X => X.ROW_ID);



                IQueryable<DB_RES_ACTIONS> dB_RES_ACTIONS = dbContext.DB_RES_ACTIONS.Where(
                                            x => x.INIT_ID == id &&
                                               (
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_REGDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.DatabaseConsistencyCheck ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.VerifyDatabaseOfficeCode ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.CreateKAVERIUser ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.EnableDatabaseAuditLog ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendLastID ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.SendDatabaseSchema ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.KAIGR_VALDatabaseRestore ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_REG ||
                                                   x.ACTION_ID == (int)ApiCommonEnum.DB_RES_ACTIONSMASTER.MapKaveriUserToKAIGR_VAL
                                               )).OrderBy(X => X.ROW_ID);
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021

                #endregion ABOVE CODE COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 27-11-2020

                List<DB_RES_ACTIONS_MASTER> dB_RES_ACTIONS_MASTERsList = dbContext.DB_RES_ACTIONS_MASTER.ToList();
                // List<DB_RES_ACTIONS> dB_RES_ACTIONSListForErrorHistory = new List<DB_RES_ACTIONS>();
                List<DB_RES_ACTIONS_CLASS> DB_RES_ACTIONS_CLASS = new List<DB_RES_ACTIONS_CLASS>();
                if (dB_RES_ACTIONS != null)
                {
                    foreach (DB_RES_ACTIONS db in dB_RES_ACTIONS)
                    {
                        if (db.ERROR_DESCRIPTION != null)
                        {
                            DB_RES_ACTIONS_CLASS.Add(new DB_RES_ACTIONS_CLASS
                            {
                                RowId = db.ROW_ID,
                                InitId = db.INIT_ID == null ? 0 : (int)db.INIT_ID,
                                ActionId = db.ACTION_ID == null ? 0 : (int)db.ACTION_ID,
                                ActionDateTime = db.ACTION_DATETIME.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                                IsSuccessful = db.IS_SUCCESSFUL,
                                ScriptId = db.SCRIPT_ID == null ? 0 : (int)db.SCRIPT_ID,
                                ErrorDescription = db.ERROR_DESCRIPTION,
                                CorrectiveAction = db.CORRECTIVEACTION,
                                ActionDescription = dB_RES_ACTIONS_MASTERsList.Where(x => x.ACTION_ID == db.ACTION_ID).Select(x => x.ACTION_DESCRIPTION).FirstOrDefault()
                            }); ;
                        }
                    }
                }
                resModel.dB_RES_ACTIONsForErrorHistory = DB_RES_ACTIONS_CLASS;
                #endregion
                resModel.dB_RES_TABLE_WISE_COUNT_List = null;
                //var SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == model.SROfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //IQueryable<DB_RES_INITIATE_MASTER> iQueryableOfInitiateMaster =  dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID);
                //var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();

                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 29-07-2020 AT 3:20 PM
                // FOR CONSIDERING NEW REQUEST ALSO
                var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(
                        x =>
                            (
                                x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification ||
                                x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified
                            ) &&
                            x.IS_DRO == false &&
                            x.SROCODE == model.SROfficeID &&
                            x.INIT_ID == model.INIT_ID_INT
                            ).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();
                //var InitId = dbContext.DB_RES_INITIATE_MASTER.Where(x => (x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification || x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndVerified) && x.IS_DRO == false && x.SROCODE == model.SROfficeID).OrderByDescending(x => x.INIT_ID).Select(x => x.INIT_ID).FirstOrDefault();
                if (InitId > 0)
                {

                    resModel.IsCompleted = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.STATUS_ID == (int)ApiCommonEnum.DB_RES_STATUSMASTER.DataInsertedAndPendingForVerification && x.IS_DRO == false && x.SROCODE == model.SROfficeID && x.CONFIRM_DATETIME == null).OrderByDescending(x => x.INIT_ID).Select(x => x.IS_COMPLETED).FirstOrDefault();

                    var ScriptId = dbContext.DB_RES_INSERT_SCRIPT_DETAILS.Where(x => x.INIT_ID == InitId).Select(x => x.SCRIPT_ID).FirstOrDefault();

                    //populate confirm button for sro if IsCompleted is 1
                    if (resModel.IsCompleted)
                    {
                        // resModel.ApproveBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=ApproveScript('" + SCRIPT_ID_For_ADD + "','" + dB_RES_INITIATE_MASTER.INIT_ID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Approve</button>";

                        // ADDED BELOW QUERY AND IF CONDITION BY SHUBHAM BHAGAT ON 29-07-2020 AT 8:30 PM
                        // FOR POPULATING CONFIRM BUTTON IF NOT CONFIRMED
                        var CONFIRM_DATETIME_IsNullOrNot = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == InitId).Select(x => x.CONFIRM_DATETIME).FirstOrDefault();
                        if (CONFIRM_DATETIME_IsNullOrNot == null)
                            resModel.ConfirmBtn = "<button type = 'button' style='width:10%', class='btn btn-group-md btn-success' onclick=ConfirmDataInsertion('" + InitId + "','" + model.SROfficeID + "') data-toggle='tooltip' data-placement='top' title='Click here'>Confirm</button>";
                    }

                    else
                    {
                        DataRestorationReportReqModel mdl = new DataRestorationReportReqModel();
                        mdl.INITID_INT = InitId;
                        mdl.SROfficeID = model.SROfficeID;
                        DataRestorationReportResModel mdl2 = ConfirmDataInsertion(mdl);
                        resModel.ConfirmBtnMsg = mdl2.DataInsertionConfrimationMsg;
                    }

                    // ADDED BY PANKAJ ON 16-07-2020 
                    // TO DISPLAY CONCLUSION SECTION IN BOTH CASES IF DATA IS INSERTED AND PENDING FOR VERIFICATION OR DATA IS INSERTED AND VERIFIED
                    resModel.IsConclusionToDisplay = true;
                    //InitId
                    DB_RES_INITIATE_MASTER confirmMsgInitMaster = dbContext.DB_RES_INITIATE_MASTER.Where(x => x.INIT_ID == InitId).FirstOrDefault();
                    if (confirmMsgInitMaster.CONFIRM_DATETIME == null)
                    {
                        resModel.ConclusionMsg = "<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data restoration process completed successfully kindly verify and confirm.</label>";
                    }
                    else
                    {
                        resModel.ConclusionMsg = "<label style='font-size:initial;font-weight:bold;margin-left:1%;'>Data Restoration verified and confirmed by SR on date : " + ((DateTime)confirmMsgInitMaster.CONFIRM_DATETIME).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "</label>";
                    }
                    //var result = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == InitId).ToList();
                    //List<DB_RES_SERVICE_COMM_DETAILS> DB_RES_SERVICE_COM_DETAILS_LIST = dbContext.DB_RES_SERVICE_COMM_DETAILS.Where(x => x.INIT_ID == InitId).ToList();

                    //// below code is for count(*) queries
                    //if(DB_RES_SERVICE_COM_DETAILS_LIST.Count > 0)
                    //{
                    //    int LastDocumentID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "DocumentID").Select(x => x.KEY_VALUE).FirstOrDefault());
                    //    int LastMarriageID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "RegistrationID").Select(x => x.KEY_VALUE).FirstOrDefault());
                    //    int LastNoticeID = Int32.Parse(DB_RES_SERVICE_COM_DETAILS_LIST.Where(x => x.KEY_COLUMN == "NoticeID").Select(x => x.KEY_VALUE).FirstOrDefault());

                    //    resModel.DocumentRegistrationsToBeRestored = dbContext.DocumentMaster.Where(x => x.DocumentID > LastDocumentID).Count();
                    //    resModel.MarriageRegistratiosToBeRestored = dbContext.MarriageRegistration.Where(x => x.RegistrationID > LastMarriageID).Count();
                    //    resModel.NoticeRegistrationsToBeRestored = dbContext.NoticeMaster.Where(x => x.NoticeID > LastNoticeID).Count();
                    //}


                    List<DB_RES_TABLE_MASTER> dB_RES_TABLE_MASTERs = dbContext.DB_RES_TABLE_MASTER.ToList();
                    List<DB_RES_TABLEWISE_COUNT> dB_RES_TABLEWISE_COUNTs = dbContext.DB_RES_TABLEWISE_COUNT.Where(x => x.INIT_ID == InitId).ToList();
                    List<DB_RES_TABLE_WISE_COUNT> newList = new List<DB_RES_TABLE_WISE_COUNT>();
                    if (dB_RES_TABLEWISE_COUNTs.Count > 0)
                    {

                        foreach (var a in dB_RES_TABLEWISE_COUNTs)
                        {
                            newList.Add(new DB_RES_TABLE_WISE_COUNT
                            {
                                RowID = a.ROWID,
                                InitId = a.INIT_ID,
                                TableId = a.TABLE_ID,
                                RowsToBeInserted = a.ROWS_TO_BE_INSERTED == null ? 0 : (int)a.ROWS_TO_BE_INSERTED,
                                RowsInserted = a.ROWS_INSERTED == null ? 0 : (int)a.ROWS_INSERTED,
                                TableName = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_ID == a.TABLE_ID).Select(x => x.TABLE_NAME).FirstOrDefault()
                            }); ;
                        }
                    }
                    resModel.dB_RES_TABLE_WISE_COUNT_List = newList;

                    //for data verification details table in dataRestorationReportStatus
                    if (dB_RES_TABLEWISE_COUNTs.Count > 0)
                    {
                        int docMasterTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "DocumentMaster").Select(x => x.TABLE_ID).FirstOrDefault();
                        int marriageRegTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "MarriageRegistration").Select(x => x.TABLE_ID).FirstOrDefault();
                        int noticeMasterTableId = dB_RES_TABLE_MASTERs.Where(x => x.TABLE_NAME == "NoticeMaster").Select(x => x.TABLE_ID).FirstOrDefault();

                        var docToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == docMasterTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();
                        var marToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == marriageRegTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();
                        var notToBeRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == noticeMasterTableId).Select(x => x.ROWS_TO_BE_INSERTED).FirstOrDefault();

                        resModel.DocumentRegistrationsToBeRestored = docToBeRestored == null ? 0 : (int)docToBeRestored;
                        resModel.MarriageRegistratiosToBeRestored = marToBeRestored == null ? 0 : (int)marToBeRestored;
                        resModel.NoticeRegistrationsToBeRestored = notToBeRestored == null ? 0 : (int)notToBeRestored;

                        var docRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == docMasterTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();
                        var marRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == marriageRegTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();
                        var notRestored = dB_RES_TABLEWISE_COUNTs.Where(x => x.TABLE_ID == noticeMasterTableId).Select(x => x.ROWS_INSERTED).FirstOrDefault();

                        resModel.DocumentRegistratiosRestored = docRestored == null ? 0 : (int)docRestored;
                        resModel.MarriageRegistrationsRestored = marRestored == null ? 0 : (int)marRestored;
                        resModel.NoticeRegistrationsRestored = notRestored == null ? 0 : (int)notRestored;
                    }

                }
                #endregion

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportDAL-DataRestorationReportStatus-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return resModel;

                ////Initiate Date
                ////resModel.InitiationDate = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //resModel.InitiationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);//1
                ////Initiate Date btn
                ////resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun()><i style='padding-right:3%;' class='fa fa-calendar'></i>Initiate Date</button>";
                //resModel.InitiationDateBtn = "<button type ='button' style='width:30%;' class='btn btn-group-md btn-success' onclick=InitiateDateFun() data-toggle='tooltip' data-placement='top' title='Click here'>Click here</button>";//1
                ////Generate Key Btn
                ////resModel.GenerateKeyBtn = "<button type ='button' style='width:25%;' class='btn btn-group-md btn-success' onclick=GenerateKeyFun()><i style='padding-right:3%;' class='fa fa-key'></i>Generate Key</button>";
                ////Generate Key Value
                ////resModel.GenerateKeyValue = GenerateKey();//1
                //resModel.GenerateKeyBtnAndTextMsg = "<label style='font-size:initial;font-weight:bold;background-color:#94b5d1;'>Your activation key is: <span id='GenerateKeyValueSpanID'>" + GenerateKey() + " </span>which is valid for 2 days only Please share this activation key to support person.</label>";

                #region extra
                //var officeID = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                //     Select(y => y.OfficeID).FirstOrDefault();

                //var userID = dbContext.UMG_UserDetails.Where(z => z.OfficeID == officeID).Select(p => p.UserID).FirstOrDefault();

                //var SRName = dbContext.UMG_UserProfile.Where(q =>q.UserID == userID).Select(r => r.FirstName + r.LastName).FirstOrDefault();

                //var SRName1 = dbContext.UMG_UserProfile.Where(q => q.UserID == dbContext.UMG_UserDetails.
                //Where(z => z.OfficeID == dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == model.SROfficeID).
                //Select(y => y.OfficeID).FirstOrDefault()).Select(p => p.UserID).FirstOrDefault()).
                //Select(r => r.FirstName + r.LastName).FirstOrDefault();
                #endregion


            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally { if (dbContext != null) dbContext.Dispose(); }
        }


    }
}

