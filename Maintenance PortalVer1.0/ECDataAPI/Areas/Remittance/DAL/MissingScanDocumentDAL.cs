using CustomModels.Models.Remittance.MissingSacnDocument;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class MissingScanDocumentDAL 
    {
        KaveriEntities dbContext = null;
        public MissingScanDocumentModel MissingSacnDocumentView()
        {
            MissingScanDocumentModel resModel = new MissingScanDocumentModel();

            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.SROfficeList = new List<SelectListItem>();
                SelectListItem selectListFirst = new SelectListItem();
                SelectListItem selectListSecond = new SelectListItem();
                selectListFirst.Text = "All";
                selectListFirst.Value = "0";
              
                resModel.SROfficeList.Insert(0, selectListFirst);
              
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                         
                            selectListOBJ.Text = item.SRONameE;
                            selectListOBJ.Value = item.SROCode.ToString();
                 
                            resModel.SROfficeList.Add(selectListOBJ);
                        }
                    }
                }
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var GetDocumentList = objCommon.GetDocumentType();
                GetDocumentList.RemoveRange(3, 2);
                resModel.DocumentType = GetDocumentList;
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

        public MissingScanDocumentResModel GetMissingScanDocumentDetails(MissingScanDocumentModel missingSacnDocumentModel)
        {

            MissingScanDocumentResModel resultModel = new MissingScanDocumentResModel();
            resultModel.MissingScanDocumentTableModelList = new List<MissingScanDocumentTableModel>();
            List<MissingScanDocumentTableModel> Result = new List<MissingScanDocumentTableModel>();
            MissingScanDocumentTableModel resModel = null;
            long SrCount = 1;
            var DocumentIDFinal = 15;

            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                //
                int DocumentTypeID = Convert.ToInt32(missingSacnDocumentModel.DocumentTypeId);
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
                var ErrorTypetxt = "";
                switch (missingSacnDocumentModel.ErrorCode)
                {
                
                    case 4:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralScanDocumentNotPresentLocalPresent).ToString();
                        break;
                    case 5:
                        ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralandLocalScanDocumentNotPresent).ToString();
                        break;
                    //case 6:
                    //    ErrorTypetxt = Convert.ToInt32(DocumentVerificationErrorcode.CentralScanDocumentPresentLocalNotPresent).ToString();
                    //    break;
                    default:
                        ErrorTypetxt = "0";
                        break;
                }
                //
                var ErrorTypetxtToExclude = Convert.ToInt16(DocumentVerificationErrorcode.Added).ToString();
                
                if (missingSacnDocumentModel.SROfficeID !=0)
                {
                    #region Errortype
                    if (missingSacnDocumentModel.IsErrorTypecheck && missingSacnDocumentModel.ErrorCode > 0)

                        {

                            Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                  join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                  from SF in SFJoin.DefaultIfEmpty()
                                  where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails
                                  && DN.SROCode == missingSacnDocumentModel.SROfficeID
                                  && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude
                                  && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= missingSacnDocumentModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (missingSacnDocumentModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal
                                 
                                  select new MissingScanDocumentTableModel
                                  {
                                     
                                      //SROCode = DN.SROCode,
                                      SRO_Name = dbContext.SROMaster.Where(d => d.SROCode == DN.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                               
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      L_Stamp5DateTime_DateTime = DN.L_Stamp5DateTime


                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion

                }
                else
                {
                    #region Errortype
                    if (missingSacnDocumentModel.IsErrorTypecheck && missingSacnDocumentModel.ErrorCode > 0)
                    {

                        Result = (from DN in dbContext.RPT_DocReg_NoCLDetails
                                  join DNB in dbContext.RPT_DocReg_NoCLBatchDetails on DN.BatchID equals DNB.BatchID
                                  join SF in dbContext.ScannedFileUploadDetails on new { p1 = DN.DocumentID, p2 = DN.SROCode, p3 = (int)DN.DocumentTypeID } equals new { p1 = SF.DocumentID, p2 = SF.SROCode, p3 = SF.DocumentTypeID } into SFJoin
                                  from SF in SFJoin.DefaultIfEmpty()
                                  where SF == null // Exclude DocumentIDs present in ScannedFileUploadDetails
                                  && !("," + DN.ErrorType + ",").Contains("," + ErrorTypetxtToExclude + ",") // Exclude ErrorType present in ErrorTypetxtToExclude
                                  && ("," + DN.ErrorType + ",").Contains("," + ErrorTypetxt + ",")
                                  && ((DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) >= missingSacnDocumentModel.DateTime_Date.Date
                                  && (DN.C_Stamp5DateTime == null ? DN.L_Stamp5DateTime : DN.C_Stamp5DateTime) <= (missingSacnDocumentModel.DateTime_ToDate.Date))
                                  && DN.DocumentTypeID == DocumentTypeIDFinal
                                  && DN.DocumentID >= DocumentIDFinal
                                 
                                  select new MissingScanDocumentTableModel
                                  {
                                     
                                      SRO_Name = dbContext.SROMaster.Where(d => d.SROCode == DN.SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                                      C_Stamp5DateTime = DN.C_Stamp5DateTime.ToString(),
                                      C_FRN = DN.C_FRN,
                                      C_ScannedFileName = DN.C_ScannedFileName,
                                      L_Stamp5DateTime = DN.L_Stamp5DateTime.ToString(),
                                      L_FRN = DN.L_FRN,
                                      L_ScannedFileName = DN.L_ScannedFileName,
                             
                                      C_CDNumber = DN.C_CDNumber,
                                      L_CDNumber = DN.L_CDNumber,
                                      L_Stamp5DateTime_DateTime = DN.L_Stamp5DateTime


                                  }
                                                         ).Distinct().ToList();

                    }
                    #endregion
                }

                Result = Result.OrderByDescending(x => x.L_Stamp5DateTime_DateTime).ToList();
                foreach (var item in Result)
                {
                    resModel = new MissingScanDocumentTableModel();

                    resModel.srNo = SrCount++;
                   
                    resModel.SRO_Name = item.SRO_Name;

                    resModel.C_Stamp5DateTime = item.C_Stamp5DateTime == "" ? "--" : Convert.ToDateTime(item.C_Stamp5DateTime).ToString("dd/MM/yyyy");
                    resModel.C_FRN = item.C_FRN ?? "--";
                    resModel.C_ScannedFileName = item.C_ScannedFileName ?? "--";

                    resModel.L_Stamp5DateTime = item.L_Stamp5DateTime == "" ? "--" : Convert.ToDateTime(item.L_Stamp5DateTime).ToString("dd/MM/yyyy");

                    resModel.L_FRN = item.L_FRN ?? "--";
                    resModel.L_ScannedFileName = item.L_ScannedFileName ?? "--";
               
                    resModel.C_CDNumber = item.C_CDNumber ?? "--";
                    resModel.L_CDNumber = item.L_CDNumber ?? "--";
                   

                    resultModel.MissingScanDocumentTableModelList.Add(resModel);
                }
                //

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
            return resultModel;
        }
    }
}