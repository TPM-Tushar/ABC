#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   CommonFunctions.cs
    * Author Name       :   Avinash Gawali
    * Creation Date     :   15-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Common Functionality to be used in Application
*/
#endregion



#region references

using CustomModels.Models.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.ECDATADOCS;
using ECDataAPI.Models;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ECDataAPI.Entity.KaigrSearchDB;
using System.Data.Entity;
using CustomModels.Models.RefundChallan;
using ECDataAPI.PreRegApplicationDetailsService;

#endregion

namespace ECDataAPI.Common
{
    public class ApiCommonFunctions
    {

        //private String[] encryptedParameters = null;
        //private Dictionary<String, String> decryptedParameters = null;
        //FirmSummaryDAL firmsummaryDal = new FirmSummaryDAL();

        public byte[] AddWaterMark(byte[] SourceFileByteArray, string watermarkText)
        {
            PdfReader pdfReader = null;
            byte[] OutputArray = null;
            try
            {
                pdfReader = new PdfReader(SourceFileByteArray);
                PdfReader.unethicalreading = true;
                //  using (FileStream fs = new FileStream(DestinationFilePathWithName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                    {
                        int pageCount1 = pdfReader.NumberOfPages;
                        //Create a new layer 
                        PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                        for (int i = 1; i <= pageCount1; i++)
                        {
                            iTextSharp.text.Rectangle rect = pdfReader.GetPageSize(i);
                            //Get the ContentByte object
                            PdfContentByte cb = stamper.GetUnderContent(i);
                            //Tell the CB that the next commands should be "bound" to this new layer
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.25f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.BLACK);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, rect.Width / 2, rect.Height / 2, 45f);
                            cb.EndText();
                            //"Close" the layer
                            cb.EndLayer();
                        }
                    }
                    OutputArray = ms.ToArray();
                }
                return OutputArray;
            }
            catch
            {
                throw;
            }

            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();
            }
        }


        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        /// <summary>
        /// Fills Master Dropdowns on basis of Ids Provided. 
        /// refer Common.MasterDropDownEnum
        /// </summary>
        /// <param name="dropdownIdsTofill"></param>
        /// <returns></returns>
        public MasterDropDownModel FillMasterDropDownModel(EnumDropDownListModel dropdownEnum)
        {
            MasterDropDownModel model = new MasterDropDownModel();

            dropdownEnum.CountryId = dropdownEnum.CountryId == null ? (short)ApiCommonEnum.Countries.India : dropdownEnum.CountryId;   // 1 for india From Kaveri.MAS_Countries
            dropdownEnum.StateID = dropdownEnum.StateID == null ? (short)ApiCommonEnum.States.GOA : dropdownEnum.StateID;   // 11 for GOA From Kaveri.MAS_States

            foreach (var dropDownId in dropdownEnum.DropdownListToFill)
            {
                switch (dropDownId)
                {

                    case (int)(ApiCommonEnum.MasterDropDownEnum.CountryDropDown):
                        model.CountryDropDown = GetCountriesDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.StatesDropDown):
                        model.StatesDropDown = GetStateDropDown(dropdownEnum.CountryId);
                        break;

                    case (int)(ApiCommonEnum.MasterDropDownEnum.DistrictsDropDown):
                        model.DistrictsDropDown = GetDistrictsDropDown(dropdownEnum.StateID); 
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.TitleDropDown):
                        model.TitleDropDown = GetTitlesDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.GenderDropDown):
                        model.GenderDropDown = GetGenderDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.ProfessionDropDown):
                        model.ProfessionDropDown = GetProfessionDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.MaritalStatusDropDown):
                        model.MaritalStatusDropDown = GetMaritalStatusDropDown();
                        break;

                    case (int)(ApiCommonEnum.MasterDropDownEnum.NationalityDropDown):
                        model.NationalityDropDown = GetNationalityDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.IdProofsTypeDropDown):
                        model.IdProofsTypeDropDown = GetIdProofsDropDown();
                        break;
                    case (int)(ApiCommonEnum.MasterDropDownEnum.RelationTypeDropDown):
                        model.RelationTypeDropDown = GetRelationTypeDropDown();
                        break;

                    //case (int)(ApiCommonEnum.MasterDropDownEnum.BookTypeDropDown):
                    //    model.BookTypeDropDown = GetBookTypeDropDown();
                    //    break;



                    default:
                        break;
                }
            }
            return model;
        }

        #region Fill Master Dropdowns

        public List<SelectListItem> GetCountriesDropDown()
        {
            List<SelectListItem> CountryList = new List<SelectListItem>();
            CountryList.Insert(0, GetDefaultSelectListItem("Select Country", "0"));
            using (var dbContext = new KaveriEntities())
            {
                CountryList.AddRange(dbContext.MAS_Countries.Select(m => new SelectListItem { Value = m.CountryID.ToString(), Text = m.CountryName }).ToList());

            }

            return CountryList;

        }

        public List<SelectListItem> GetStateDropDown(short? CountryId = null) // 1 for india From Kaveri.MAS_Countries
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select State", "0"));
            using (var dbContext = new KaveriEntities())
            {
                var StateList = dbContext.MAS_States.Where(x => x.CountryID == CountryId).ToList();
                if (StateList.Count > 0)
                    List.AddRange(StateList.Select(m => new SelectListItem { Value = m.StateID.ToString(), Text = m.StateName }).ToList());
            }

            return List;

        }

        public List<SelectListItem> GetDistrictsDropDown(short? StateID = null) //  From Kaveri.MAS_States
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select District", "0"));
            using (var dbContext = new KaveriEntities())
            {
                var StateList = dbContext.MAS_Districts.Where(x => x.StateID == StateID).ToList();
                if (StateList.Count > 0)
                {
                    List.AddRange(StateList.Select(m => new SelectListItem { Value = m.DistrictID.ToString(), Text = m.DistrictName }).ToList());
                    List = List.Where(c => c.Value != ((long)ApiCommonEnum.GoaDistricts.Other).ToString()).ToList();
                }
            }
            return List;
        }

        public List<SelectListItem> GetProfessionDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Profession", "0"));
            using (var dbContext = new KaveriEntities())
            {
                List.AddRange(dbContext.MAS_Profession.Select(m => new SelectListItem { Value = m.ProfessionID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }
        public List<SelectListItem> GetGenderDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Gender", "0"));
            using (var dbContext = new KaveriEntities())
            {
                List.AddRange(dbContext.MAS_Gender.Select(m => new SelectListItem { Value = m.GenderID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }
        public List<SelectListItem> GetMaritalStatusDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Marital Status", "0"));
            using (var dbContext = new KaveriEntities())
            {
                List.AddRange(dbContext.MAS_MaritalStatus.Select(m => new SelectListItem { Value = m.MaritalStatusID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }

        public List<SelectListItem> GetTitlesDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Title", "0"));
            using (var dbContext = new KaveriEntities())
            {
                //   List.AddRange(dbContext.MAS_Titles.Select(m => new SelectListItem { Value = m.TitleID.ToString(), Text = m.Description }).ToList());
                List.AddRange((dbContext.MAS_Titles.Where(m => m.IsActive == true).OrderBy(m => m.OrderingSequence)).Select(m => new SelectListItem { Value = m.TitleID.ToString(), Text = m.Description }));//change by chetan

            }

            return List;

        }
        public List<SelectListItem> GetNationalityDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Nationality", "0"));
            using (var dbContext = new KaveriEntities())
            {
                List.AddRange(dbContext.MAS_Nationality.Select(m => new SelectListItem { Value = m.NationalityID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }
        public List<SelectListItem> GetIdProofsDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Id Proof", "0"));
            using (var dbContext = new KaveriEntities())
            {
                //List.AddRange(dbContext.MAS_IDProofTypes.Select(m => new SelectListItem { Value = m.IDProofTypeID.ToString(), Text = m.Description }).ToList());
                List.AddRange(dbContext.MAS_IDProofTypes.Where(c => c.IsActive == true).Select(m => new SelectListItem { Value = m.IDProofTypeID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }

        public List<SelectListItem> GetRelationTypeDropDown()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Relation", "0"));
            using (var dbContext = new KaveriEntities())
            {
                List.AddRange(dbContext.MAS_RelationTypes.Select(m => new SelectListItem { Value = m.RelationTypeID.ToString(), Text = m.Description }).ToList());
            }

            return List;

        }



        #endregion

        /// <summary>
        /// districtId= null for all Offices of OfficeTypeID
        /// or specific districtId wise
        /// </summary>
        /// <param name="OfficeTypeID"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetOfficesDropDown(short OfficeTypeID, short? districtId = null)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List.Insert(0, GetDefaultSelectListItem("Select Office", "0"));
            using (var dbContext = new KaveriEntities())
            {
                if (districtId == null)
                    List.AddRange(dbContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID == OfficeTypeID).Select(m => new SelectListItem { Value = m.OfficeID.ToString(), Text = m.OfficeName }).ToList());
                else
                    List.AddRange(dbContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID == OfficeTypeID && x.DistrictID == districtId).Select(m => new SelectListItem { Value = m.OfficeID.ToString(), Text = m.OfficeName }).ToList());


            }

            return List;
        }
                  

        public static string SendMail(EmailModel model)
        {
            try
            {

                PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();

                bool result;
                try
                {
                     result = service.SendEmail(model.RecepientAddress, model.EmailSubject, model.EmailContent);
                }
                catch (Exception ex)
                {
                    ApiExceptionLogs.LogError(ex);
                    return "Something went wrong while connecting to Email service, Please contact Admin";
                }

                if (result)
                    return string.Empty; //Success
               else
                    return "Unable to send email , Please contact Admin";  //Error



            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex);
                return "Unable to send email , Please contact Admin";
            }
        }


        /// <summary>
        /// Convert string datetime to DateTime object
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime? GetStringToDateTime(string strDate)
        {
            if (string.IsNullOrEmpty(strDate))
                return null;

            string[] formats = { "dd/MM/yyyy" };

            DateTime newDate;
            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDate, formats[0], null).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {

                throw new Exception("Invalid Date. Error in Parsing");
            }

            //return DateTime.ParseExact(strDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static DateTime ConvertStringTo24HrDateTime(string strDateTimeStamp)
        {

            DateTime newDate;
            //bool boolDate = DateTime.TryParse(DateTime.ParseExact("2/22/2015 9:54:02 AM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture), out newDate);
            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDateTimeStamp, "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {

                throw new Exception("Invalid Date. Error in Parsing");
            }
        }

        internal static bool GetStartEndDatesFinYear(DateTime now, out DateTime? firmDisAppFinYearStartDate, out DateTime? firmDisAppFinYearEndDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get DateTime object to string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetDateTimeToString(DateTime date)
        {
            if (date == null)
                return null;
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }


        public static string GetDbEntityValidationExceptionMsgs(DbEntityValidationException exception)
        {
            try
            {
                StringBuilder sbuilder = new StringBuilder();
                foreach (var eve in exception.EntityValidationErrors)
                {
                    sbuilder.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sbuilder.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                return sbuilder.ToString();
            }
            catch (Exception )
            {
                throw ;
            }
        }



        public static Dictionary<string, string> GetVillagesByDistrictID(int DistrictID)
        {
            KaveriEntities dbContext = null;
            ArrayList lstVillages = new ArrayList();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                dbContext = new KaveriEntities();
                dict = (from village in dbContext.MAS_Villages
                        join office in dbContext.MAS_OfficeMaster
                        on village.OfficeID equals office.OfficeID

                        where
                        office.DistrictID == DistrictID
                        select new
                        {
                            VillageCode = village.VillageID,
                            VillageName = village.VillageName + "  [" + office.OfficeName + "]"
                        }).ToDictionary(x => x.VillageCode.ToString(), x => x.VillageName);//  Select(x=> new KeyValuePair<long,string> (x.VillageCode,x.VillageName));


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
            return dict;
        }


        public static Dictionary<string, string> GetVillagesByTalukaID(int TalukaID)
        {
            KaveriEntities dbContext = null;
            ArrayList lstVillages = new ArrayList();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                dbContext = new KaveriEntities();
                dict = (from village in dbContext.MAS_Villages
                        join office in dbContext.MAS_OfficeMaster
                        on village.OfficeID equals office.OfficeID

                        where
                        village.TalukaID == TalukaID
                        select new
                        {
                            VillageCode = village.VillageID,
                            VillageName = village.VillageName + "  [" + office.OfficeName + "]"
                        }).ToDictionary(x => x.VillageCode.ToString(), x => x.VillageName);//  Select(x=> new KeyValuePair<long,string> (x.VillageCode,x.VillageName));
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
            return dict;
        }


        public static Dictionary<string, string> GetTalukasByDistrictID(int DistrictID)
        {
            KaveriEntities dbContext = null;
            ArrayList talukas = new ArrayList();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                dbContext = new KaveriEntities();
                dict = (from taluka in dbContext.MAS_Talukas
                        join office in dbContext.MAS_OfficeMaster
                        on taluka.OfficeID equals office.OfficeID

                        where
                        office.DistrictID == DistrictID
                        select new
                        {
                            TalukaCode = taluka.TalukaID,
                            TalukaName = taluka.TalukaName + "  [" + office.OfficeName + "]"
                        }).ToDictionary(x => x.TalukaCode.ToString(), x => x.TalukaName);//  Select(x=> new KeyValuePair<long,string> (x.VillageCode,x.VillageName));


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
            return dict;
        }






        /// <summary>
        /// Method to retreive start and end dates for a  year.
        /// </summary>
        /// <param name="calDate"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool GetStartEndDatesOfYear(DateTime? calDate, out DateTime? startDate, out DateTime? endDate)
        {
            string strStartDate;
            string strEndDate;
            try
            {
                if (calDate != null)
                {
                    DateTime currentDate = (DateTime)calDate;

                    strStartDate = "01/01/" + currentDate.Year;
                    strEndDate = "31/12/" + currentDate.Year;


                    startDate = (DateTime)GetStringToDateTime(strStartDate);
                    endDate = (DateTime)GetStringToDateTime(strEndDate);
                    return true;
                }
                else
                {
                    startDate = null;
                    endDate = null;
                    return false;
                }
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


        /// <summary>
        /// Get directory structure
        /// Added By Rohit Khatale On 16/05/2016
        /// Update On 22-06-2016 11:45 AM
        /// </summary>
        /// <param name="id"></param>
        /// <param name="officeId"></param>
        /// <param name="serviceType"></param>
        /// <param name="processType"></param>
        /// <param name="scanProcessId"></param>
        /// <returns></returns>
        public string GetDirectoryStructure(long FirmID,long id, short officeId, short serviceType, short processType, short scanProcessId)
        {

            //To call this method...............!

            //short serviceType = (short)IGRContracts.Common.CommonEnum.enumServiceTypes.Grievance;
            //ServiceCaller serviceCommon = new ServiceCaller("CommonFunctionsApi");
            //return serviceCommon.GetCall<string>("GetDirectoryStructure", new { id = applicationId, officeID = officeId, serviceType = serviceType, processType = 1, scanProcessId = 0 });



            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                string directoryStructure = string.Empty;
                string finYear = string.Empty;
                var officeDetails = (from office in dbContext.MAS_OfficeMaster
                                     where office.OfficeID == officeId
                                     select new
                                     {
                                         office.OfficeID,
                                         office.ShortName,
                                         office.OfficeTypeID,
                                         office.MAS_OfficeTypes.OfficeTypeDesc
                                     }).FirstOrDefault();
                directoryStructure = officeDetails.OfficeTypeDesc;
                switch (serviceType)
                {
                    case (short)ApiCommonEnum.enumServiceTypes.FirmRegistration:

                      //  directoryStructure = Path.Combine(directoryStructure, "REG", officeDetails.ShortName, DateTime.Now.Year.ToString(), "REG_" + id);
                        directoryStructure = Path.Combine(directoryStructure, "REG", officeDetails.ShortName, DateTime.Now.Year.ToString(), "FRM_" + FirmID);

                        break;
                    case (short)ApiCommonEnum.enumServiceTypes.CertifiedCopies:

                        directoryStructure = Path.Combine(directoryStructure, "MAR", officeDetails.ShortName, DateTime.Now.Year.ToString(), "MAR_" + id);
                        break;
                }

                //switch (processType)
                //{
                //    case (short)ApiCommonEnum.ServicesProcess.FirmRegistration:
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Notice:
                //        directoryStructure = Path.Combine(directoryStructure, "Notice");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Enquiry:
                //        directoryStructure = Path.Combine(directoryStructure, "Enquiry");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Memorandum:
                //        directoryStructure = Path.Combine(directoryStructure, "Memorandum");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.FirmAmendment:
                //        directoryStructure = Path.Combine(directoryStructure, "Amendment");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.FirmDissolution:
                //        directoryStructure = Path.Combine(directoryStructure, "Dissolution");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.FirmRegistration:
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Objection:
                //        directoryStructure = Path.Combine(directoryStructure, "Objection");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Endorsement:
                //        directoryStructure = Path.Combine(directoryStructure, "Endorsement");
                //        break;
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.Reject:
                //        directoryStructure = Path.Combine(directoryStructure, "Reject");
                //        break;
                //    //ADDED BY NILESH ON DATE 12-05-2017 FOR DATACORRECTION
                //    case (short)IGRContracts.Common.CommonEnum.ScaningServicesProcess.DataCorrection:
                //        directoryStructure = Path.Combine(directoryStructure, "DataCorrection");
                //        break;
                //    default:
                //        break;
                //}

                switch (scanProcessId)
                {
                    case (short)ApiCommonEnum.ServicesProcess.Enclosures:
                        directoryStructure = Path.Combine(directoryStructure, "Enclosures");
                        break;
                    default:
                        break;
                }

                return directoryStructure;
            }
            catch (Exception)
            {
                throw new Exception("Directory structure does not exist.");
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        /// <summary>
        /// Get directory structure
        /// Added By Rohit Khatale On 16/05/2016
        /// Update On 22-06-2016 11:45 AM
        /// </summary>
        /// <param name="id"></param>
        /// <param name="officeId"></param>
        /// <param name="serviceType"></param>
        /// <param name="processType"></param>
        /// <param name="scanProcessId"></param>
        /// <returns></returns>
        public string GetDirectoryStructureForSupportedDocument(long firmID, short officeId, short serviceType, short processType, short scanProcessId,short FirmApplicationTypeID)
        {

            //To call this method...............!

            //short serviceType = (short)IGRContracts.Common.CommonEnum.enumServiceTypes.Grievance;
            //ServiceCaller serviceCommon = new ServiceCaller("CommonFunctionsApi");
            //return serviceCommon.GetCall<string>("GetDirectoryStructure", new { id = applicationId, officeID = officeId, serviceType = serviceType, processType = 1, scanProcessId = 0 });



            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                string directoryStructure = string.Empty;
                string finYear = string.Empty;
                var officeDetails = (from office in dbContext.MAS_OfficeMaster
                                     where office.OfficeID == officeId
                                     select new
                                     {
                                         office.OfficeID,
                                         office.ShortName,
                                         office.OfficeTypeID,
                                         office.MAS_OfficeTypes.OfficeTypeDesc
                                     }).FirstOrDefault();
                directoryStructure = officeDetails.OfficeTypeDesc;
                switch (serviceType)
                {
                    case (short)ApiCommonEnum.enumServiceTypes.FirmRegistration:

                        directoryStructure = Path.Combine(directoryStructure, "REG", officeDetails.ShortName, DateTime.Now.Year.ToString(), "FRM_" + firmID);
                        break;
                    case (short)ApiCommonEnum.enumServiceTypes.CertifiedCopies:

                        //     directoryStructure = Path.Combine(directoryStructure, "MAR", officeDetails.ShortName, DateTime.Now.Year.ToString(), "MAR_" + id);
                        break;
                }

              

                switch (FirmApplicationTypeID)
                {

                    case (short)ApiCommonEnum.FirmApplicationType.FirmRegistrationFilling:
                        if (scanProcessId == (short)ApiCommonEnum.ServicesProcess.Enclosures)
                            directoryStructure = Path.Combine(directoryStructure, "Enclosures");
                        break;

                    case (short)ApiCommonEnum.ServicesProcess.FirmDissolution:
                            directoryStructure = Path.Combine(directoryStructure, "Dissolution");
                            break;
                    default:

                    case (short)ApiCommonEnum.ServicesProcess.FirmAmendment:
                        directoryStructure = Path.Combine(directoryStructure, "Dissolution");
                        break;

                }

                return directoryStructure;
            }
            catch (Exception)
            {
                throw new Exception("Directory structure does not exist.");
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        /// <summary>
        /// This method returns boolean value "TRUE" if MVC-Event Audit Logging details inserted successfully.
        /// </summary>
        /// <param name="objEventAuditModel"></param>
        /// <returns></returns>
        public bool InsertAuditMvcEventLoging(EventAuditLoging objEventAuditModel)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                CMN_AuditMvcEventLoging EventAuditLog = new CMN_AuditMvcEventLoging();
                //EventAuditLog.EventAuditID = dbContext.CMN_EventAuditLoging.Max(m => (Int32?)m.EventAuditID) == null ? 1 : (Int32)dbContext.CMN_EventAuditLoging.Max(m => (Int32?)m.EventAuditID) + 1;
                EventAuditLog.AuditID=(dbContext.CMN_AuditMvcEventLoging.Any() ? dbContext.CMN_AuditMvcEventLoging.Max(a => a.AuditID) : 0) + 1;
                EventAuditLog.UserID = objEventAuditModel.UserID == 0 ? (long?)null : objEventAuditModel.UserID;
                EventAuditLog.UrlAccessed = objEventAuditModel.UrlAccessed;
                EventAuditLog.IPAddress = objEventAuditModel.IPAddress;
                EventAuditLog.Description = objEventAuditModel.Description;
                //EventAuditLog.AuditDataTime = objEventAuditModel.AuditDataTime == null ? null : (DateTime?)CommonFunctions.GetStringToDateTime(objEventAuditModel.AuditDataTime);
                //EventAuditLog.AuditDataTime = objEventAuditModel.AuditDataTime == null ? null : (DateTime?)CommonFunctions.GetStringToDateTimeWithTimeStamp(objEventAuditModel.AuditDataTime);
                EventAuditLog.AuditDateTime = System.DateTime.Now;
                EventAuditLog.ServiceID = objEventAuditModel.ServiceID == 0 ? (short?)null : objEventAuditModel.ServiceID;
                dbContext.CMN_AuditMvcEventLoging.Add(EventAuditLog);
                dbContext.SaveChanges();
                return true;

            }
            catch(Exception )
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
        }


        /// <summary>
        /// This method returns boolean value "TRUE" if API-Event Audit Logging details inserted successfully.
        /// </summary>
        /// <param name="objEventAuditModel"></param>
        /// <returns></returns>
        public bool InsertApiEventAuditLog(EventAuditLoging objEventAuditModel)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                CMN_AuditApiLoging EventAuditLog = new CMN_AuditApiLoging();
                //EventAuditLog.EventAuditID = dbContext.CMN_EventAuditLoging.Max(m => (Int32?)m.EventAuditID) == null ? 1 : (Int32)dbContext.CMN_EventAuditLoging.Max(m => (Int32?)m.EventAuditID) + 1;

                EventAuditLog.ApiConsumerID = objEventAuditModel.UserID == 0 ? (long?)null : objEventAuditModel.UserID;
                EventAuditLog.UrlAccessed = objEventAuditModel.UrlAccessed;
                EventAuditLog.IPAddress = objEventAuditModel.IPAddress;
                EventAuditLog.Description = objEventAuditModel.Description;
                //EventAuditLog.AuditDataTime = objEventAuditModel.AuditDataTime == null ? null : (DateTime?)CommonFunctions.GetStringToDateTime(objEventAuditModel.AuditDataTime);
                //EventAuditLog.AuditDataTime = objEventAuditModel.AuditDataTime == null ? null : (DateTime?)CommonFunctions.GetStringToDateTimeWithTimeStamp(objEventAuditModel.AuditDataTime);
                EventAuditLog.AuditDateTime = System.DateTime.Now;
                EventAuditLog.ServiceID = objEventAuditModel.ServiceID == 0 ? (short?)null : objEventAuditModel.ServiceID;
                dbContext.CMN_AuditApiLoging.Add(EventAuditLog);
                dbContext.SaveChanges();
                return true;

            }
            catch
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
        }

        public long GetApiConsumerID(ClientAuthenticationModel model)
        {

            KaveriEntities dbContext = null;

            HttpContext context = HttpContext.Current;
            try
            {
                dbContext = new KaveriEntities();

                var user = dbContext.UMG_API_Users.Where(x => x.User_Name == model.ApiConsumerUserName && x.Password == model.ApiConsumerPassword).FirstOrDefault();

                return user.ApiConsumerID;
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }

        }


        public String GetDateTimeToStringwithTimeStamp(DateTime date)
        {
            // return date.ToString("dd/MM/yyyy");
            return date.ToString("dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public static void WriteErrorLog(string sText)
        {
            string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];
            directoryPath = directoryPath + "\\DeployedLog\\";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            directoryPath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            using (System.IO.StreamWriter file = System.IO.File.AppendText(directoryPath))
            {
                string format = "{0} : {1}";

                file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));

                file.WriteLine("text:" + sText);
                file.Flush();
            }


        }


        public static void WriteTofile(string text)
        {
            string sDir = @"C:\ApiErr\";
            if (!Directory.Exists(sDir))
                Directory.CreateDirectory(sDir);

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(sDir + "ApilandingPageLog.txt"))
            {
                sw.WriteLine("Date:" + DateTime.Now.ToString());
                sw.WriteLine(string.IsNullOrEmpty(text) ? "Parameter null." : text);
               // sw.Close();
            }
        }
        //Private Method to Generate PDF using a String with the help of iTextSharp
        //public FileDisplayModel CreatePDFfromString(FileDisplayModel objFileDisplayModel)
        //{
        //    try
        //    {
        //        StringReader sr = new StringReader(objFileDisplayModel.Message);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //            pdfDoc.Open();
        //            htmlparser.Parse(sr);
        //            pdfDoc.Close();
        //            byte[] bytes = memoryStream.ToArray();
        //            memoryStream.Close();
        //            objFileDisplayModel.isFileExist = false;
        //            objFileDisplayModel.fileBytes = bytes;
        //        }

        //        //--
        //        PdfReader pr = new PdfReader(objFileDisplayModel.fileBytes);
        //        MemoryStream ms = new MemoryStream();
        //        PdfStamper stamper = new PdfStamper(pr, ms);

        //        Dictionary<String, String> info = pr.Info;
        //        info["Title"] = objFileDisplayModel.TittleForPDF;
        //        stamper.MoreInfo = info;
        //        stamper.Close();
        //        objFileDisplayModel.fileBytes = ms.ToArray();
        //        //--
        //        return objFileDisplayModel;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}


        public  static string GetHashOfDocument(byte[] FileBytes)
        {
            StringBuilder sBuilder = new StringBuilder();

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] HashedBytes = sha256Hash.ComputeHash(FileBytes);
                for (int i = 0; i < HashedBytes.Length; i++)
                {
                    sBuilder.Append(HashedBytes[i].ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }

        public static byte[] FlattenSignature(byte[] SourceFileBytes)
        {

            PdfReader reader = new PdfReader(SourceFileBytes);
            //PdfStamper stamper = new PdfStamper(reader, new FileStream(@"C:\err", FileMode.Create));

            AcroFields fields = reader.AcroFields;
            List<string> SignatureNames = fields.GetBlankSignatureNames();
            List<string> SignNamesList = fields.GetSignatureNames();
            //"Signature2"


            var itemList = fields.Fields;
            return null;
        }

        //Added by Akash to Convert Numbers to words.
        public string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "ZERO";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " LAKES ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " RUPEES ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "AND ";
                var unitsMap = new[]
                {
                "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
            };
                var tensMap = new[]
                {
                "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
            };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        public static void WriteProductionLog(string text)
        {

            if (System.Configuration.ConfigurationManager.AppSettings["writeProductionErrorLogPath"].ToString().ToUpper() == "TRUE")
            {

                string sPath = System.Configuration.ConfigurationManager.AppSettings["KaveriWindowsServiceErrorLogPath1"];
                if (!System.IO.Directory.Exists(sPath))
                    System.IO.Directory.CreateDirectory(sPath);
                using (StreamWriter sw = System.IO.File.AppendText(sPath + @"\errorlog.txt"))
                {
                    sw.WriteLine("------------------------------");
                    sw.WriteLine("Time:" + DateTime.Now.ToString());
                    sw.WriteLine("text=>:" + text);
                    sw.WriteLine("----------------------------");

                }
            }
        }


        //added by amit 28-9-18

        public static List<SelectListItem> GetServiceList()
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> ServiceList = new List<SelectListItem>();
                SelectListItem objService = new SelectListItem();
                objService.Text = "--Select Service--";
                objService.Value = "0";
                ServiceList.Add(objService);
                foreach (var Service in dbKaveriOnlineContext.MAS_ServiceMaster.Where(x => x.IsActive == true).ToList())
                {
                    objService = new SelectListItem();
                    objService.Text = Service.ServiceName;
                    objService.Value = Service.ServiceID.ToString();
                    ServiceList.Add(objService);
                }
                return ServiceList;
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }
        }

        // Added by Shubham Bhagat 22/10/2018
        public static void SystemUserActivityLog(long userId, int activityTypeID, String logDesc)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                // For Activity Log
                CMN_System_User_ActivityLog cmn_System_User_ActivityLog_Obj = new CMN_System_User_ActivityLog();
                cmn_System_User_ActivityLog_Obj.ActivityID = Convert.ToInt16((dbContext.CMN_System_User_ActivityLog.Any() ? dbContext.CMN_System_User_ActivityLog.Max(a => a.ActivityID) : 0) + 1);
                cmn_System_User_ActivityLog_Obj.UserID = userId;
                cmn_System_User_ActivityLog_Obj.ActivityTypeID = activityTypeID; // Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails);
                cmn_System_User_ActivityLog_Obj.LogDesc = logDesc; //roleDetailsModel.RoleName + " Role Details Added";
                dbContext.CMN_System_User_ActivityLog.Add(cmn_System_User_ActivityLog_Obj);
                dbContext.SaveChanges();
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
               
        public static bool CompareObjectsBeforeUpdate<T>(T obj1, T obj2)
        {

            Type type = typeof(T);
            if (obj1 == null || obj2 == null)
                return false;

            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    string Object1Value = string.Empty;
                    string Object2Value = string.Empty;
                    if (type.GetProperty(property.Name).GetValue(obj1, null) != null)
                        Object1Value = type.GetProperty(property.Name).GetValue(obj1, null).ToString();
                    if (type.GetProperty(property.Name).GetValue(obj2, null) != null)
                        Object2Value = type.GetProperty(property.Name).GetValue(obj2, null).ToString();
                    if (Object1Value.Trim() != Object2Value.Trim())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public DataTable ConvertListToDataTable<T>(List<T> inputList)
        {
            //By Raman
            DataTable table = new DataTable();
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string[] array = new string[13] { "SNoOfDocument", "DateOfFiling", "DescOfDocumentsFiled", "Name", "Address", "DateOfJoining", "DateOfCharges", "PrincipalPlace", "OtherPlaces", "DateOfClosing", "DateOfOpening", "RecordingOfChanges", "Remarks" };


            //By Raman
            for (int item = 0; item < array.Count(); item++)
            {
                table.Columns.Add(array[item]);
            }

            foreach (T items in inputList)
            {
                var values = new object[13];
                for (int i = 0; i < 13; i++)
                {
                    values[i] = props[i].GetValue(items, null);
                }
                table.Rows.Add(values);
            }


            return table;
        }

        public byte[] AddpageNumber(byte[] inputArray)
        {
            byte[] pdfBytes = null;
            iTextSharp.text.Font fntrow = DefineNormaFont("Times New Roman", 8);

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(inputArray);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + i.ToString() + " of " + pages, fntrow), 568f, 16f, 0);
                    }
                }
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;

        }
        public iTextSharp.text.Font DefineItalicBoldFont(string strFontName, float Size)
        {
            iTextSharp.text.Font fontItalic = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont(strFontName, Size, iTextSharp.text.Font.BOLDITALIC));
            return fontItalic;
        }
        public iTextSharp.text.Font DefineNormaFont(string strFontName, float Size)
        {
            iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont(strFontName, Size, iTextSharp.text.Font.NORMAL));
            return fontNormal;
        }

        public iTextSharp.text.Font DefineBoldFont(string strFontName, float Size)
        {
            iTextSharp.text.Font fontBold = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont(strFontName, Size, iTextSharp.text.Font.BOLD));

            return fontBold;
        }

        //Added by chetan
        public static DateTime GetStringToDateTimeOfNotNullableType(string strDate)
        {
            //if (string.IsNullOrEmpty(strDate))
            //    return null;

            string[] formats = { "dd/MM/yyyy" };

            DateTime newDate;
            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDate, formats[0], null).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {

                throw new Exception("Invalid Date. Error in Parsing");
            }

            //return DateTime.ParseExact(strDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
               
        public DateTime ConvertStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
            dateTimeFormatInfo.ShortDatePattern = "dd/MM/yyyy";
            dateTimeFormatInfo.LongDatePattern = "dd/MM/yyyy hh:mm:ss";
            CultureInfo usaCulture = new CultureInfo("en-US");
            usaCulture.DateTimeFormat = dateTimeFormatInfo;
            return DateTime.ParseExact(strDate.Trim(), formats, usaCulture, DateTimeStyles.None);
        }
               
        public String ConvertDateTimeToString(DateTime date)
        {
            // return date.ToString("dd/MM/yyyy");
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        
        //Added by Raman kalegaonkar on 15-03-2019
        public List<SelectListItem> GetDROfficesList()
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                OfficeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
                OfficeList.AddRange(dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).Select(m => new SelectListItem { Value = m.DistrictCode.ToString(), Text = m.DistrictNameE }).ToList());
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

        public List<SelectListItem> GetDROfficesList(string FirstRecord)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                // COMMENTED BY SHUBHAM BHAGAT ON 6-6-2019 TO REMOVE ALL
                OfficeList.Insert(0, GetDefaultSelectListItem(FirstRecord, "0"));
                OfficeList.AddRange(dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).Select(m => new SelectListItem { Value = m.DistrictCode.ToString(), Text = m.DistrictNameE }).ToList());
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

        //Added by Raman kalegaonkar on 15-03-2019
        public List<SelectListItem> GetSROfficesList()
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                OfficeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
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

        public List<SelectListItem> getSROfficesList(bool isAllSelected = false)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                if (isAllSelected)
                    OfficeList.Insert(0, GetDefaultSelectListItem("All", "0"));
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
        
        public List<SelectListItem> GetProgramNameList(bool isAllSelected = false)
        {

            List<SelectListItem> ProgramNameList = new List<SelectListItem>();

            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                if (isAllSelected)
                    ProgramNameList.Insert(0, GetDefaultSelectListItem("All", "0"));
                List<USP_ECDATA_MODIFICATION_PROGRAM_NAMES_Result> result = dbContext.USP_ECDATA_MODIFICATION_PROGRAM_NAMES("0").ToList();

                foreach (var item in result)
                {

                    if (item != null)
                        ProgramNameList.Add(new SelectListItem { Value = item.ID.ToString(), Text = (string.IsNullOrEmpty(item.ProgramName) ? "-" : item.ProgramName.Trim()) });
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
            return ProgramNameList;
        }
               
        public string ConvertStringToMACAddress(string physicalAddress)
        {
            string regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            string replace = "$1:$2:$3:$4:$5:$6";
            return Regex.Replace(physicalAddress, regex, replace);
        }

        //Added By Raman Kalegaonkar on 03-04-2019
        public List<SelectListItem> GetNatureOfDocumentList()
        {
            List<SelectListItem> NatureOfDocumentList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();


                NatureOfDocumentList.Insert(0, GetDefaultSelectListItem("All", "0"));
                NatureOfDocumentList.AddRange(dbContext.RegistrationArticles.OrderBy(c => c.ArticleNameE).Select(m => new SelectListItem { Value = m.RegArticleCode.ToString(), Text = m.ArticleNameE}).ToList());
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

            return NatureOfDocumentList;


        }


        #region ADDED BY SHUBHAM BHAGAT 09-04-2020

        // ABOVE GetRegistrationArticles() IS UNCOMMENTED ON 30-06-2020 AND BELOW COMMENTED 
        //BECAUSE ABOVE IS RETURNING SIMPLE LIST AND BELOW IS RETURNING LIST WHICH CONTAINS
        // GROUPING OF TOP 10 ARTICLES AND REMAINING ARTICLE WHICH IS CREATED FOR DASHBOARD TAB2

        public List<SelectListItem> GetRegistrationArticles(bool IsAll)
        {
            List<SelectListItem> RegArticles = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                //Added By Raman Kalegaonkar on 25-03-2020
                if (IsAll)
                    RegArticles.Insert(0, GetDefaultSelectListItem("All", "0"));
                RegArticles.AddRange(dbContext.RegistrationArticles.OrderBy(c => c.ArticleNameE).Select(m => new SelectListItem { Value = m.RegArticleCode.ToString(), Text = m.ArticleNameE }).ToList());
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

            return RegArticles;

        }

        public List<SelectListItem> GetRegistrationArticlesTop10Wise(bool IsAll)
        {
            List<SelectListItem> RegArticles = new List<SelectListItem>();
            //Commented By Shubham Bhagat on 08-04-2020
            //KaveriEntities dbContext = null;
            KaigrSearchDB searchDBContext = null;
            try
            {
                //Commented By Shubham Bhagat on 08-04-2020
                //dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                //Added By Raman Kalegaonkar on 25-03-2020
                if (IsAll)
                    RegArticles.Insert(0, GetDefaultSelectListItem("All", "0"));
                //Added By Shubham Bhagat on 08-04-2020
                //Commented By Shubham Bhagat on 08-04-2020
                //RegArticles.AddRange(dbContext.RegistrationArticles.OrderBy(c => c.ArticleNameE).Select(m => new SelectListItem { Value = m.RegArticleCode.ToString(), Text = m.ArticleNameE }).ToList());
                //var fewefwe = searchDBContext.USP_DB_GET_ARTICLELIST_TOP10WISE().OrderBy(c => c.DISPLAYORDER).Select(m => new SelectListItem
                //{ Value = m.RegArticleCode.ToString(), Text = m.ArticleNameE, Group =m.GROUPCODE.ToString()});

                var ARTICLELIST_TOP10WISE_Group_List = searchDBContext.USP_DB_GET_ARTICLELIST_TOP10WISE().
                                                            OrderBy(c => c.DISPLAYORDER).ToList().
                                                            Select(x => x.GROUPCODE).Distinct();

                foreach (var item in ARTICLELIST_TOP10WISE_Group_List)
                {
                    // Create a SelectListGroup
                    String groupName = item.Value == 1 ? "Top 10 Articles" : "Remaining Articles";
                    var optionGroup = new SelectListGroup() { Name = groupName };
                    // Add SelectListItem's
                    var ARTICLELIST_TOP10WISE_GroupWise_List = searchDBContext.USP_DB_GET_ARTICLELIST_TOP10WISE().
                                                                    Where(x => x.GROUPCODE == item.Value).
                                                                    OrderBy(c => c.DISPLAYORDER).ToList();
                    bool isGroupAdded = false;//1
                    //bool isFirstGroup = item.Value == 1 ? true : false;//2
                    //int noofElementsInList = ARTICLELIST_TOP10WISE_GroupWise_List.Count();//2

                    foreach (var item2 in ARTICLELIST_TOP10WISE_GroupWise_List)
                    {
                        if (isGroupAdded)
                        {

                            RegArticles.Add(new SelectListItem() //1
                            {
                                Value = item2.RegArticleCode.ToString(),
                                Text = item2.ArticleNameE
                            }); //1

                        }
                        else
                        {
                            RegArticles.Add(new SelectListItem()
                            {
                                Value = item2.RegArticleCode.ToString(),
                                Text = item2.ArticleNameE,
                                Group = optionGroup
                            });
                            isGroupAdded = true;
                        }

                        //var dropdownList = new SelectList(entityList.Select(item => new SelectListItem
                        //{
                        //    Text = item.Name,
                        //    Value = item.Id,
                        //    // Assign the Group to the item by some appropriate selection method
                        //    Group = item.IsActive ? group1 : group2
                        //}).OrderBy(a => a.Group.Name).ToList(), "Value", "Text", "Group.Name", -1);

                    }
                }
                //  RegArticles.AddRange(

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //Commented By Shubham Bhagat on 08-04-2020
                //if (dbContext != null)
                //    dbContext.Dispose();
            }

            return RegArticles;

        }
        #endregion

        public List<SelectListItem> GetMonthList(bool isAllSelected = false)
        {
            List<SelectListItem> monthList = new List<SelectListItem>();

            try
            {


                monthList.AddRange(DateTimeFormatInfo
                      .InvariantInfo
                      .MonthNames
                      .Where(C => C != "")
                      .Select((monthName, index) => new SelectListItem
                      {
                          Value = (index + 1).ToString(),
                          Text = monthName
                      }).ToList());






            }
            catch (Exception)
            {

                throw;
            }

            return monthList;


        }
        
        public List<SelectListItem> GetYearDropdown(bool isAllSelected = false)
        {
            List<SelectListItem> yearList = new List<SelectListItem>();

            try
            {
                yearList.Insert(0, GetDefaultSelectListItem("Select", "0"));

                for (int i = 0; i < 3; i++)
                {
                    int CurrentYear = DateTime.Now.Year;
                    int year = (CurrentYear - i);
                    yearList.Add(new SelectListItem { Text = year.ToString(), Value = year.ToString(), Selected = (year == CurrentYear) });

                }








            }
            catch (Exception)
            {

                throw;
            }

            return yearList;


        }


        #region Kaveri Support (Added by Akash)

        public class Tuple<T1, T2>
        {
            public T1 PrivateKey { get; private set; }
            public T2 PublicKey { get; private set; }
            internal Tuple(T1 first, T2 second)
            {
                PrivateKey = first;
                PublicKey = second;
            }
        }

        public static Tuple<string, string> CreateKeyPair()
        {
            try
            {
                CspParameters cspParams = new CspParameters { ProviderType = 1 };

                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);

                string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
                string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

                return new Tuple<string, string>(privateKey, publicKey);
            }
            catch (Exception e)
            {

                ApiExceptionLogs.LogError(e);
                throw;
            }
        }


        public static byte[] Encrypt(string publicKey, string data)
        {
            try
            {

                CspParameters cspParams = new CspParameters { ProviderType = 1 };
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

                rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

                byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

                return encryptedBytes;
            }
            catch (Exception e)
            {

                ApiExceptionLogs.LogError(e);
                throw;
            }
        }


        public static string Decrypt(string privateKey, byte[] encryptedBytes)
        {
            try
            {
                CspParameters cspParams = new CspParameters { ProviderType = 1 };
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

                rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

                byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

                string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

                return plainText;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        //Added By Raman Kalegaonkar on 20-05-2019
        public string GetDroName(int DistrictID)
        {
            string DroName;
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DistrictID).Select(x => x.DistrictNameE).FirstOrDefault();
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
            return DroName;

        }

        //Added By Raman Kalegaonkar on 27-06-2019
        public List<SelectListItem> GetSROfficesListByDisrictID(long DistrictCode)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                //if (isAllSelected)
                OfficeList.Insert(0, objCommon.GetDefaultSelectListItem("ALL", "0"));
                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
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

        public List<SelectListItem> GetSROfficesListByDisrictIDUsingFirstRecord(long DistrictCode, string FirstRecord)
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                dbContext = new KaveriEntities();

                //if (isAllSelected)
                OfficeList.Insert(0, objCommon.GetDefaultSelectListItem(FirstRecord, "0"));
                OfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
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

        // ADDED BY SHUBHAM BHAGAT TO WRITE DEBUG LOG IN DATABASE ON 1-07-2019
        public static void WriteDebugLogInDB(int functionalityID, String logDesc, String className, String methodName)
        {
            ECDATA_DOCS_Entities ecDATA_DOCS_Entities = null;
            try
            {
                ecDATA_DOCS_Entities = new ECDATA_DOCS_Entities();
                bool result = ecDATA_DOCS_Entities.ECLOG_FUNCTIONALITY_MASTER.Where(x => x.FUNCTIONALITY_ID == functionalityID && x.STATUS == true).Any();
                if (result)
                {
                    ECLOG_FUNCTIONALITY_DEBUG_LOG obj = new ECLOG_FUNCTIONALITY_DEBUG_LOG();
                    obj.FUNCTIONALITY_ID = functionalityID;
                    obj.LOG_DESCRIPTION = logDesc;
                    obj.INSERT_DATETIME = DateTime.Now;
                    obj.CLASSNAME = className.Length > 499 ? className.Substring(0, 498) : className;
                    obj.METHODNAME = methodName.Length > 499 ? methodName.Substring(0, 498) : methodName;
                    ecDATA_DOCS_Entities.ECLOG_FUNCTIONALITY_DEBUG_LOG.Add(obj);
                    ecDATA_DOCS_Entities.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ecDATA_DOCS_Entities != null)
                    ecDATA_DOCS_Entities.Dispose();
            }
        }


        //Added By Raman Kalegaonkar on 16-07-2019
        public List<SelectListItem> GetFeeType(string FirstRecord)
        {
            List<SelectListItem> NatureOfDocumentList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                NatureOfDocumentList.Insert(0, GetDefaultSelectListItem(FirstRecord, "0"));
                NatureOfDocumentList.AddRange(dbContext.FeesRuleMaster.OrderBy(c => c.DescriptionE).Select(m => new SelectListItem { Value = m.FeeRuleCode.ToString(), Text = m.DescriptionE }).ToList());
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

            return NatureOfDocumentList;


        }


        //Added By RamanK on 25-07-2019
        public List<SelectListItem> GetPropertyTypeList()
        {
            List<SelectListItem> PropertyTypeList = new List<SelectListItem>();
            PropertyTypeList.Add(GetDefaultSelectListItem("All", (Convert.ToInt32(ApiCommonEnum.propertyTypes.All)).ToString()));
            PropertyTypeList.Add(GetDefaultSelectListItem("Agricultural",(Convert.ToInt32(ApiCommonEnum.propertyTypes.Agricultural)).ToString()));
            PropertyTypeList.Add(GetDefaultSelectListItem("Non Agricultural", (Convert.ToInt32(ApiCommonEnum.propertyTypes.NonAgricultural)).ToString()));
            //PropertyTypeList.Add(GetDefaultSelectListItem("Flat",(Convert.ToInt32(ApiCommonEnum.propertyTypes.Flat)).ToString()));
            PropertyTypeList.Add(GetDefaultSelectListItem("Apartment", (Convert.ToInt32(ApiCommonEnum.propertyTypes.Apartment)).ToString()));
            PropertyTypeList.Add(GetDefaultSelectListItem("Lease", (Convert.ToInt32(ApiCommonEnum.propertyTypes.Lease)).ToString()));

            return PropertyTypeList;
        }

        //public List<SelectListItem> GetBuildTypeList()
        //{
        //    List<SelectListItem> BuildTypeList = new List<SelectListItem>();
        //    BuildTypeList.Add(GetDefaultSelectListItem("All",(Convert.ToInt32(ApiCommonEnum.BuildTypes.All)).ToString()));
        //    BuildTypeList.Add(GetDefaultSelectListItem("Flat",(Convert.ToInt32(ApiCommonEnum.BuildTypes.Flat)).ToString()));
        //    BuildTypeList.Add(GetDefaultSelectListItem("Apartment",(Convert.ToInt32(ApiCommonEnum.BuildTypes.Apartment)).ToString()));
        //    BuildTypeList.Add(GetDefaultSelectListItem("Other", (Convert.ToInt32(ApiCommonEnum.BuildTypes.Other)).ToString()));

        //    return BuildTypeList;
        //}

        //Added By RamanK on 30-07-2019

        public List<SelectListItem> GetPropertyValueList()
        {
            try
            {

                List<SelectListItem> PropertyValueList = new List<SelectListItem>();
                PropertyValueList.Add(GetDefaultSelectListItem("All", (Convert.ToInt32(ApiCommonEnum.PropertyValues.All)).ToString()));
                PropertyValueList.Add(GetDefaultSelectListItem("Upto Ten Lakhs", (Convert.ToInt32(ApiCommonEnum.PropertyValues.UptoTenLakhs)).ToString()));
                PropertyValueList.Add(GetDefaultSelectListItem("Above Ten Lakhs", (Convert.ToInt32(ApiCommonEnum.PropertyValues.AboveTenLakhs)).ToString()));
                return PropertyValueList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added By RamanK on 22-08-2019 for Anywhere EC Log Report
        public List<SelectListItem> GetLogTypes(string FirstRecord)
        {
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                List<SelectListItem> logTypeList = new List<SelectListItem>();
                logTypeList.Insert(0, GetDefaultSelectListItem(FirstRecord, "0"));
                logTypeList.AddRange(dbContext.REG_LogTypes.OrderBy(c => c.LogTypeID).Select(m => new SelectListItem { Value = m.LogTypeID.ToString(), Text = m.Description }).ToList());
                logTypeList.RemoveRange(2,4);
                return logTypeList;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Raman Kalegaonkar on 27-09-2019
        public Dictionary<int, int> GetDistrictWiseSROCountDictionary(int DistrictID)
        {
            KaveriEntities dbContext = null;
            Dictionary<int, int> DistrictWiseSROCountDict = new Dictionary<int, int>();
            try
            {
                dbContext = new KaveriEntities();
                int Index = 0;
                var DistrictWiseSROCountList = (from SM in dbContext.SROMaster group SM by SM.DistrictCode into sm join DM in dbContext.DistrictMaster on sm.Key equals DM.DistrictCode orderby DM.DistrictNameE select new { DM.DistrictNameE, count = sm.Count() });
                foreach (var item in DistrictWiseSROCountList)
                {
                    DistrictWiseSROCountDict.Add(Index++, item.count);
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
            return DistrictWiseSROCountDict;
        }

        //Added by Raman Kalegaonkar on 30-09-2019
        public Dictionary<string, int> GetDistrictWiseSROCountDictForSingleDistrict(int DistrictID)
        {
            KaveriEntities dbContext = null;
            Dictionary<string, int> DistrictWiseSROCountDict = new Dictionary<string, int>();
            try
            {
                dbContext = new KaveriEntities();
                DistrictWiseSROCountDict = (from SM in dbContext.SROMaster group SM by SM.DistrictCode into sm join DM in dbContext.DistrictMaster on sm.Key equals DM.DistrictCode orderby DM.DistrictNameE select new { DM.DistrictNameE,count= sm.Count() }).ToDictionary(x => x.DistrictNameE, x => x.count); ;
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
            return DistrictWiseSROCountDict;
        }

        //Added by Raman 
        public void DeleteFileFromTemporaryFolder(string filePath)
        {
            //Try to delete file from server temparary folder
            //resume next if already used by another process/User.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            finally
            {
            }
        }

        public List<SelectListItem> CDNumberList(int SROCode,String FirstRecord="All")
        {
            List<SelectListItem> OfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                    OfficeList.Insert(0, GetDefaultSelectListItem(FirstRecord, "0"));
                OfficeList.AddRange(dbContext.CDMaster.OrderBy(c => c.VolumeName).Where(m=>m.SROCode== SROCode).Select(m => new SelectListItem { Value = m.CDID.ToString(), Text = m.VolumeName }).ToList());
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


        ///Added by Raman Kalegaonkar on 08-04-2020
        public List<SelectListItem> GetDocumentType(String FirstItem="Select")
        {
            List<SelectListItem> DoctypeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DoctypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                DoctypeList.Insert(1, GetDefaultSelectListItem("Document", (Convert.ToInt32(ApiCommonEnum.DocumentType.Document)).ToString()));
                DoctypeList.Insert(2, GetDefaultSelectListItem("Marriage", (Convert.ToInt32(ApiCommonEnum.DocumentType.Marriage)).ToString()));
                DoctypeList.Insert(3, GetDefaultSelectListItem("Notice", (Convert.ToInt32(ApiCommonEnum.DocumentType.Notice)).ToString()));
                //Added by mayank on 13/09/2021 for Firm registered report
                DoctypeList.Insert(4, GetDefaultSelectListItem("Firm", (Convert.ToInt32(ApiCommonEnum.DocumentType.Firm)).ToString()));
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

            return DoctypeList;
        }




        ///Added by Raman Kalegaonkar on 28-04-2020
        public List<SelectListItem> GetPaymentMode(String FirstItem = "Select")
        {
            List<SelectListItem> DoctypeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DoctypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 17-06-2020
                //DoctypeList.Insert(1, GetDefaultSelectListItem("Document", (Convert.ToInt32(ApiCommonEnum.PaymentMode.Cash)).ToString()));
                //DoctypeList.Insert(2, GetDefaultSelectListItem("Marriage", (Convert.ToInt32(ApiCommonEnum.PaymentMode.Challen)).ToString()));
                DoctypeList.Insert(1, GetDefaultSelectListItem("Cash", (Convert.ToInt32(ApiCommonEnum.PaymentMode.Cash)).ToString()));
                DoctypeList.Insert(2, GetDefaultSelectListItem("Challan", (Convert.ToInt32(ApiCommonEnum.PaymentMode.Challen)).ToString()));
                DoctypeList.Insert(3, GetDefaultSelectListItem("DD", (Convert.ToInt32(ApiCommonEnum.PaymentMode.DD)).ToString()));
                DoctypeList.Insert(4, GetDefaultSelectListItem("Cheque - DD - PayOrder", (Convert.ToInt32(ApiCommonEnum.PaymentMode.ChequeDDPayOrder)).ToString()));
                DoctypeList.Insert(5, GetDefaultSelectListItem("Others", (Convert.ToInt32(ApiCommonEnum.PaymentMode.Others)).ToString()));
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

            return DoctypeList;
        }
        ///Added by Raman Kalegaonkar on 30-04-2020
        public List<SelectListItem> GetReceiptTypeList(String FirstItem = "Select")
        {
            List<SelectListItem> DoctypeList = new List<SelectListItem>();
            try
            {
                DoctypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                DoctypeList.Insert(1, GetDefaultSelectListItem("Document", (Convert.ToInt32(ApiCommonEnum.ReceiptType.Document)).ToString()));
                DoctypeList.Insert(2, GetDefaultSelectListItem("Marriage", (Convert.ToInt32(ApiCommonEnum.ReceiptType.Marriage)).ToString()));
                DoctypeList.Insert(3, GetDefaultSelectListItem("Notice", (Convert.ToInt32(ApiCommonEnum.ReceiptType.Notice)).ToString()));
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021
                DoctypeList.Insert(4, GetDefaultSelectListItem("Other Receipts", (Convert.ToInt32(ApiCommonEnum.ReceiptType.OtherReceipts)).ToString()));
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021

            }
            catch (Exception)
            {
                throw;
            }


            return DoctypeList;
        }

   public List<SelectListItem> GetSroListOnDro(int dROCode)
        {
            KaveriEntities dbcontext = null;
            List<SelectListItem> sroList = new List<SelectListItem>();
            sroList.Insert(0, this.GetDefaultSelectListItem("All", "0"));

            using (dbcontext = new KaveriEntities())
            {
                try
                {
                    if (dROCode != 0)
                    {


                        var sros = (from sro in dbcontext.SROMaster
                                    where sro.DistrictCode == dROCode
                                    select sro).OrderBy(m => m.SRONameE);
                        if (sros != null)
                        {
                            sroList.AddRange(sros.Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE ?? string.Empty }));
                        }
                        return sroList;
                    }
                    else
                        return null;

                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        //Added by Madhusoodan on 28/07/2021 for Villages dropdown
        public List<SelectListItem> GetVillageListByOfficeID(int OfficeID)
        {
            List<SelectListItem> VillageList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                VillageList.Insert(0, GetDefaultSelectListItem("Select", "0"));
                VillageList.AddRange(dbContext.VillageMaster.OrderBy(c => c.VillageNameE).Where(c => c.SROCode == Kaveri1Code).Select(m => new SelectListItem { Value = m.VillageCode.ToString(), Text = m.VillageNameE }).ToList());
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
            return VillageList;
        }

        //Added by Madhusoodan on 28/07/2021 for Property Type dropdown
        public List<SelectListItem> GetPropertyNumberTypeList()
        {
            List<SelectListItem> propertyNoTypeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();

                propertyNoTypeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
                propertyNoTypeList.AddRange(dbContext.PropertyNoTypeMaster.OrderBy(a => a.TypeNameEnglish).Select(m => new SelectListItem { Value = m.PropertyTypeID.ToString(), Text = m.TypeNameEnglish }).ToList());
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
            return propertyNoTypeList;
        }

        //Added by Madhusoodan on 28/07/2021 for Party Type dropdown
        public List<SelectListItem> GetPartyTypeList()
        {
            List<SelectListItem> partyTypeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();

                partyTypeList.Insert(0, GetDefaultSelectListItem("Select", "0"));
                partyTypeList.AddRange(dbContext.PartyTypeMaster.Select(m => new SelectListItem { Value = m.PartyTypeID.ToString(), Text = m.DescriptionInEnglish }).ToList());
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
            return partyTypeList;
        }

        #region Added by mayank on 24/Mar/2022
        public List<SelectListItem> GetMarriageType(String FirstItem = "Select")
        {
            List<SelectListItem> MartypeList = new List<SelectListItem>();
            try
            {
                MartypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                MartypeList.Insert(1, GetDefaultSelectListItem("Hindu Marriage", "1"));
                MartypeList.Insert(2, GetDefaultSelectListItem("Special Marriage", "2"));
                MartypeList.Insert(3, GetDefaultSelectListItem("Special Other Marriage", "3"));
            }
            catch (Exception)
            {
                throw;
            }

            return MartypeList;
        } 
        #endregion
		//Added By Tushar on 1April 2022 for add Document Type Notice
        public List<SelectListItem> GetNoticeType(String FirstItem = "Select")
        {
            List<SelectListItem> NoticeList = new List<SelectListItem>();
            try
            {
                NoticeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                NoticeList.Insert(1, GetDefaultSelectListItem("Special Notice", "1"));
                NoticeList.Insert(2, GetDefaultSelectListItem("Special Others Notice", "2"));
            }
            catch (Exception)
            {

                throw;
            }
            return NoticeList;
        }
        #region added by Vijay 11-01-2023
        #endregion
        public List<SelectListItem> GetTableName(String FirstItem = "Select")
        {
            List<SelectListItem> DoctypeList = new List<SelectListItem>();
            
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DoctypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                DoctypeList.Insert(1, GetDefaultSelectListItem("VillageMaster", (Convert.ToInt32(ApiCommonEnum.TableType.villageMaster)).ToString()));
                DoctypeList.Insert(2, GetDefaultSelectListItem("HobliMaster", (Convert.ToInt32(ApiCommonEnum.TableType.HobliMaster)).ToString()));
                DoctypeList.Insert(3, GetDefaultSelectListItem("BhoomiMappingDetails", (Convert.ToInt32(ApiCommonEnum.TableType.Bhoomi)).ToString()));
                DoctypeList.Insert(4, GetDefaultSelectListItem("DistrictMaster", (Convert.ToInt32(ApiCommonEnum.TableType.DistrictMaster)).ToString()));
                DoctypeList.Insert(5, GetDefaultSelectListItem("VillageMasterVillagesMergingMapping", (Convert.ToInt32(ApiCommonEnum.TableType.VillageMasterVillagesMergingMapping)).ToString()));
                DoctypeList.Insert(6, GetDefaultSelectListItem("SROMaster", (Convert.ToInt32(ApiCommonEnum.TableType.SROMaster)).ToString()));
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

            return DoctypeList;
        }

        #region added by Vijay on 19-01-2023
        #endregion
        public List<SelectListItem>GetDBName(String FirstItem = "Select")
        {
            List<SelectListItem> DBList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                //DBList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                DBList.Insert(0, GetDefaultSelectListItem("ECData", (Convert.ToInt32(ApiCommonEnum.DBType.ECData)).ToString()));
                DBList.Insert(1, GetDefaultSelectListItem("KAIGR_Online", (Convert.ToInt32(ApiCommonEnum.DBType.KAIGR_ONLINE)).ToString()));
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

            return DBList;

        }

        #region added by vijay on 20-01-2023
        #endregion
        public List<SelectListItem> GetTableName_KG(String FirstItem = "Select")
        {
            List<SelectListItem> DoctypeList = new List<SelectListItem>();

            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DoctypeList.Insert(0, GetDefaultSelectListItem(FirstItem, "0"));
                DoctypeList.Insert(1, GetDefaultSelectListItem("MAS_OfficeMaster", (Convert.ToInt32(ApiCommonEnum.TableType.MAS_OfficeMaster)).ToString()));
                DoctypeList.Insert(2, GetDefaultSelectListItem("MAS_Villages", (Convert.ToInt32(ApiCommonEnum.TableType.MAS_Villages)).ToString()));
                DoctypeList.Insert(3, GetDefaultSelectListItem("MAS_Hoblis", (Convert.ToInt32(ApiCommonEnum.TableType.MAS_Hoblis)).ToString()));
               
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

            return DoctypeList;
        }


        
        //Added by BShivam on 31-03-2023 for double verification of challan payment details on khajane2 service 
        public static bool ChallanDoubleVerification(string instrumentNumber, decimal challanAmount,long UserID, ref string errorMessage)
        {

            PreRegApplicationDetailsService.ApplicationDetailsService applicationDetailsService = new PreRegApplicationDetailsService.ApplicationDetailsService();
            ECDataAPI.PreRegApplicationDetailsService.TransVerificationModel transVerificationModel = new ECDataAPI.PreRegApplicationDetailsService.TransVerificationModel();
            string isChallanDoubleVerLogEnabled = ConfigurationManager.AppSettings["EnableKhajane2ChallanDBLVerification"];

            try
            {
                transVerificationModel.agencyCode = ConfigurationManager.AppSettings["AgencyCode"];
                transVerificationModel.integrationCode = ConfigurationManager.AppSettings["IntegrationCodeVerification"];
                transVerificationModel.deptRefNum = instrumentNumber;
                transVerificationModel.UserID = UserID;
                transVerificationModel.IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                
                if ((!string.IsNullOrEmpty(isChallanDoubleVerLogEnabled)) && (isChallanDoubleVerLogEnabled.ToLower() == "true"))
                    LogKhajane2ChallanDoubleVerReqData(transVerificationModel);

                ECDataAPI.PreRegApplicationDetailsService.TransVerifyResponseModel transVerifyResponseModel = applicationDetailsService.DoubleVerification(transVerificationModel);


                if(transVerifyResponseModel == null)
                {
                    errorMessage = "Exception occurred while verifying the Khajane2 challan Payment details";
                    return false;
                }


                if ((!string.IsNullOrEmpty(isChallanDoubleVerLogEnabled)) && (isChallanDoubleVerLogEnabled.ToLower() == "true"))
                    LogKhajane2ChallanDoubleVerRespData(transVerifyResponseModel);


                if (transVerifyResponseModel.StatusCode.TrimEnd() == "KIIRCTER34")
                {
                    errorMessage = "Challan reference number " + instrumentNumber + " is not found at Khajane2";
                    return false;
                }
                else if(transVerifyResponseModel.StatusCode.TrimEnd() == "KIIRCTER00")
                {
                    if ((transVerifyResponseModel.paymentStatus.TrimEnd() == "10700066") || (transVerifyResponseModel.paymentStatus.TrimEnd() == "10700072"))
                    {
                        if (transVerifyResponseModel.paidAmount != challanAmount)
                        {
                            errorMessage = "Entered challan amount "+challanAmount+" is not matching with Khajane2 paid amount "+transVerifyResponseModel.paidAmount+" , for Challan reference number " + instrumentNumber;
                            return false;
                        }
                        errorMessage = null;
                        return true;
                    }
                    else if ((transVerifyResponseModel.paymentStatus.TrimEnd() == "10700068") || (transVerifyResponseModel.paymentStatus.TrimEnd() == "10700103"))
                    {
                        errorMessage = "Payment is failed at khajane2 for Challan reference number " + instrumentNumber;
                        return false;
                    }
                    else if ((transVerifyResponseModel.paymentStatus.TrimEnd() == "10700070") || (transVerifyResponseModel.paymentStatus.TrimEnd() == "10700092"))
                    {
                        errorMessage = "Payment is pending at khajane2 for Challan reference number " + instrumentNumber;
                        return false;
                    }
                    else if ((transVerifyResponseModel.paymentStatus.TrimEnd() == "10700098"))
                    {
                        errorMessage = "Challan reference number " + instrumentNumber + " is expired at khajane2";
                        return false;
                    }
                    else if ((transVerifyResponseModel.paymentStatus.TrimEnd() == "10700220"))
                    {
                        errorMessage = "Please check payment status of challan reference number "+instrumentNumber+" at khajane2";
                        return false;
                    }
                    else
                    {
                        errorMessage = "Exception occurred while verifying the khajane2 challan Payment details";
                        return false;
                    }
                }
                else
                {
                    errorMessage = "Exception occurred while verifying the khajane2 challan Payment details";
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }

        private static void LogKhajane2ChallanDoubleVerReqData( TransVerificationModel transVerificationModel)
        {
            try
            {
                string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"] + "//Khajan2ChallanDoubleVerReqAndResp";
                string fileName = directoryPath +  "//LogKhajan2ChallanDoubleVerReqAndResp_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

                if (!Directory.Exists(directoryPath)) { 
}                   Directory.CreateDirectory(directoryPath);

                if (transVerificationModel != null)
                {
                    using (StreamWriter writer =  File.AppendText(fileName))
                    {
                        writer.WriteLine("---------------------------------Start-RequestData------------------------------------");
                        writer.WriteLine("DateTime :" + DateTime.Now);
                        writer.WriteLine("AgencyCode :" + transVerificationModel.agencyCode);
                        writer.WriteLine("IntegrationCode :" + transVerificationModel.integrationCode);
                        writer.WriteLine("DeptRefNum :" + transVerificationModel.deptRefNum);
                        writer.WriteLine("UserID :" + transVerificationModel.UserID);
                        writer.WriteLine("IPAddress :" + transVerificationModel.IPAddress);
                        writer.WriteLine("---------------------------------End-RequestData--------------------------------------");
                    }
                }
                

            }
            catch(Exception ex)
            {

            }

        }

        private static void LogKhajane2ChallanDoubleVerRespData(TransVerifyResponseModel transVerifyResponseModel)
        {
            try
            {
                string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"] + "//Khajan2ChallanDoubleVerReqAndResp";
                string fileName = directoryPath + "//LogKhajan2ChallanDoubleVerReqAndResp_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (transVerifyResponseModel != null)
                {
                    using (StreamWriter writer = File.AppendText(fileName))
                    {
                        writer.WriteLine("---------------------------------Start-ResponseData-----------------------------------");
                        writer.WriteLine("DateTime :" + DateTime.Now);
                        writer.WriteLine("StatusCode :" + transVerifyResponseModel.StatusCode);
                        writer.WriteLine("StatusDesc :" + transVerifyResponseModel.StatusDesc);
                        writer.WriteLine("BankName :" + transVerifyResponseModel.bankName);
                        writer.WriteLine("BankRefNum :" + transVerifyResponseModel.bankRefNum);
                        writer.WriteLine("PaymentStatus :" + transVerifyResponseModel.paymentStatus);
                        writer.WriteLine("PaymentMode :" + transVerifyResponseModel.paymentMode);
                        writer.WriteLine("currentTimeStamp :" + transVerifyResponseModel.currentTimeStamp);
                        writer.WriteLine("IsValidDeptRefNumber :" + transVerifyResponseModel.isValidDeptRefNumber);
                        writer.WriteLine("TransactionID :" + transVerifyResponseModel.transactionID);
                        writer.WriteLine("PaidAmount :" + transVerifyResponseModel.paidAmount);
                        writer.WriteLine("---------------------------------End-ResponseData--------------------------------------");

                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        //Ended by BShivam on 31-03-2023 








    }
}