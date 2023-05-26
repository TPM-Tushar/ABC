using CustomModels.Models.MISReports.RegistrationSummary;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.EcDataService;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class RegistrationSummaryDAL : IRegistrationSummary, IDisposable
    {
        KaveriEntities dbContext = null;

        /// <summary>
        /// returns IndexIIReports Response Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public RegistrationSummaryRESModel RegistrationSummaryView(int OfficeID)
        {
            RegistrationSummaryRESModel resModel = new RegistrationSummaryRESModel();

            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                resModel.SROfficeList = new List<SelectListItem>();
                //resModel.DROfficeList = new List<SelectListItem>();
                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList.Add(sroNameItem);

                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                }
                else
                {

                    SelectListItem select = new SelectListItem();
                    select.Text = "Select";
                    select.Value = "0";
                    resModel.SROfficeList.Add(select);
                    //resModel.DROfficeList = objCommon.GetDROfficesList("Select");

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

        /// <summary>
        /// returns a SelectList Item
        /// </summary>
        /// <param name="sTextValue"></param>
        /// <param name="sOptionValue"></param>
        /// <returns></returns>
        //public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        //{
        //    try
        //    {

        //        return new SelectListItem
        //        {
        //            Text = sTextValue,
        //            Value = sOptionValue,
        //        };

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        /// <summary>
        /// returns List of IndexIIReportsDetailsModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public List<RegistrationSummaryDetailModel> GetRegistrationSummaryDetails(RegistrationSummaryRESModel model)
        {

            RegistrationSummaryDetailModel indexIIReportsDetails = null;
            List<RegistrationSummaryDetailModel> indexIIReportsDetailsList = new List<RegistrationSummaryDetailModel>();
            KaveriEntities dbContext = null;
            //long Amount = Convert.ToInt64(model.Amount);
            decimal TotalConsideration = 0;
            decimal TotalMarketValue = 0;
            try
            {

                dbContext = new KaveriEntities();
                var TransactionList = dbContext.USP_INDEX2_DETAILS_AUDIT(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID).Skip(model.startLen).Take(model.totalNum).ToList();

                foreach (var item in TransactionList)
                {
                    indexIIReportsDetails = new RegistrationSummaryDetailModel();
                    if (item.ArticleNameE != null)
                        indexIIReportsDetails.ArticleNameE = item.ArticleNameE;
                    else
                        indexIIReportsDetails.ArticleNameE = "null";
                    if (item.Claimant != null)
                        indexIIReportsDetails.Claimant = item.Claimant;
                    else
                        indexIIReportsDetails.Claimant = "null";

                    indexIIReportsDetails.consideration = item.consideration == null ? 0 : Convert.ToDecimal(item.consideration);

                    if (item.Executant != null)
                        indexIIReportsDetails.Executant = item.Executant;
                    else
                        indexIIReportsDetails.Executant = "null";

                    if (item.FinalRegistrationNumber != null)
                    {
                        if (model.IsExcel == true || model.IsPdf == true)
                        {
                            indexIIReportsDetails.FinalRegistrationNumber = item.FinalRegistrationNumber;
                        }
                        else
                        {
                            //indexIIReportsDetails.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            if (!String.IsNullOrEmpty(item.RootDirectory) && !String.IsNullOrEmpty(item.UploadPath) && !String.IsNullOrEmpty(item.ScannedFileName))
                            {
                                String filePath = Path.Combine(item.RootDirectory, item.UploadPath, item.ScannedFileName);
                                filePath = filePath.Replace("\\", "*");
                                indexIIReportsDetails.FinalRegistrationNumber = "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=ValidateParameter('" + filePath + "');><i>" + item.FinalRegistrationNumber + "</i></a>";
                            }
                            else
                                indexIIReportsDetails.FinalRegistrationNumber = item.FinalRegistrationNumber;
                        }
                    }
                    else
                        indexIIReportsDetails.FinalRegistrationNumber = "null";

                    //28-06-2019 Raman
                    indexIIReportsDetails.marketvalue = item.marketvalue == null ? 0 : Convert.ToDecimal(item.marketvalue);



                    if (item.PropertyDetails != null)
                        indexIIReportsDetails.PropertyDetails = item.PropertyDetails;
                    else
                        indexIIReportsDetails.PropertyDetails = "null";

                    if (item.Stamp5Datetime != null)
                        //indexIIReportsDetails.Stamp5Datetime = Convert.ToString(item.Stamp5Datetime);
                        indexIIReportsDetails.Stamp5Datetime = Convert.ToDateTime(item.Stamp5Datetime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    else
                        indexIIReportsDetails.Stamp5Datetime = "null";


                    indexIIReportsDetails.TotalArea = Convert.ToString(item.TotalArea);

                    if (item.Unit != null)
                        indexIIReportsDetails.Unit = item.Unit;
                    else
                        indexIIReportsDetails.Unit = "null";

                    if (item.VillageNameE != null)
                        indexIIReportsDetails.VillageNameE = item.VillageNameE;
                    else
                        indexIIReportsDetails.VillageNameE = "null";
                    if (item.Schedule != null)
                    {
                        indexIIReportsDetails.Schedule = item.Schedule;
                    }
                    else
                    {
                        indexIIReportsDetails.Schedule = "null";
                    }

                    TotalConsideration = indexIIReportsDetails.consideration + TotalConsideration;
                    TotalMarketValue = TotalMarketValue + indexIIReportsDetails.marketvalue;
                    //indexIIReportsDetails.SroName=string.IsNullOrEmpty(item.sro)
                    indexIIReportsDetailsList.Add(indexIIReportsDetails);

                }
                if (model.IsExcel == true || model.IsPdf == true)
                {
                    indexIIReportsDetails = new RegistrationSummaryDetailModel();

                    indexIIReportsDetails.ArticleNameE = "";
                    indexIIReportsDetails.Claimant = "";
                    indexIIReportsDetails.consideration = TotalConsideration;
                    indexIIReportsDetails.Executant = "";
                    indexIIReportsDetails.FinalRegistrationNumber = "";
                    //28-06-2019 Raman
                    indexIIReportsDetails.marketvalue = TotalMarketValue;
                    indexIIReportsDetails.PropertyDetails = "";
                    indexIIReportsDetails.Stamp5Datetime = "";
                    indexIIReportsDetails.TotalArea = "";
                    indexIIReportsDetails.Unit = "";
                    indexIIReportsDetails.VillageNameE = "Total";
                    indexIIReportsDetails.Schedule = "";
                    indexIIReportsDetailsList.Add(indexIIReportsDetails);

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

            return indexIIReportsDetailsList;


        }

        /// <summary>
        /// returns TolatCount of GetIndexIIReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public int GetRegistrationSummaryDetailsTotalCount(RegistrationSummaryRESModel model)
        {

            List<RegistrationSummaryDetailModel> indexIIReportsDetailsList = new List<RegistrationSummaryDetailModel>();
            KaveriEntities dbContext = null;
            List<USP_INDEX2_DETAILS_AUDIT_Result> Result = null;
            //long Amount = Convert.ToInt64(model.Amount);
            try
            {
                dbContext = new KaveriEntities();
                Result = dbContext.USP_INDEX2_DETAILS_AUDIT(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID).ToList();
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

            return Result.Count();

        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>

        public string GetSroName(int SROfficeID)
        {
            RegistrationSummaryRESModel resModel = new RegistrationSummaryRESModel();
            string SroName;
            try
            {
                dbContext = new KaveriEntities();
                SroName = dbContext.SROMaster.Where(x => x.SROCode == SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
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
            return SroName;

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public RegistrationSummaryREQModel DisplayScannedFile(RegistrationSummaryREQModel model)
        {
            RegistrationSummaryREQModel resModel = new RegistrationSummaryREQModel();
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                
                //dbContext = new KaveriEntities();
                ECDataService service = new ECDataService();
                FileContentRequestModel reqModel = new FileContentRequestModel();
                FileContentResponseModel fileContentResponseModel = null;

                reqModel.FilePath = model.FinalRegistrationNumberFilePath.Replace("*", "\\");

                fileContentResponseModel = service.GetFileContent(reqModel);

                if (fileContentResponseModel != null)
                {
                    //To Create a temporary file to write contents of byte array
                    #region Temporary File
                    //if (!Directory.Exists(RootPath))
                    //    Directory.CreateDirectory(RootPath);

                    //using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    //{
                    //    stream.Write(fileContentResponseModel.FileContent, 0, fileContentResponseModel.FileContent.Length);
                    //    stream.Close();
                    //}
                    //long lngCheckSum = objTestCheckSum.GetCRCFromFile(FilePath);

                    //if (CheckSum == lngCheckSum)
                    //{
                    resModel.ScannedFileByteArray = fileContentResponseModel.FileContent;
                    //}
                    //else
                    //{
                    //    ResponseModel.ScannedFileByteArray = null;//Handeled in Controller
                    //}
                    #endregion

                    //Delete temporary file
                    // objCommon.DeleteFileFromTemporaryFolder(FilePath);
                    return resModel;
                }
                else
                {
                    resModel.IsError = true;
                    resModel.ErrorMessage = "No data found for entered Final Registration Number";
                    return resModel;
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {            }                                 
        }
    }
}