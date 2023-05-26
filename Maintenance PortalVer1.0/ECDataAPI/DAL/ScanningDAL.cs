#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri-I
    * File Name         :   ScanningDAL.cs
    * Author Name       :   -Avinash Gawali
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for Sacanning Details
*/
#endregion


#region references
using CustomModels.Models.Common;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
#endregion

namespace ECDataAPI.DAL
{
    public class ScanningDAL :IScanningInterface,IDisposable
    {
        #region properties
        KaveriEntities dbContext = null;
        #endregion

        #region Method
        /// <summary>
        /// Save Newly entered scan details
        /// </summary>
        /// <param name="model">InsertScanDetails</param>
        /// <returns>string message as success of exception occurrs</returns>
        public string InsertScanDetails(ScanDetails model)
        {
            dbContext = new KaveriEntities();
            try
            {
              //  Int64 maxScanID = 0;
                //1.Need To Change
                /*Commented By Avinash
                 
                 
                Int16 iProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == model.ScanProcessID).Select(m => m.ProcessMappingID).FirstOrDefault();

                if (iProcessMappingID == 0)
                   throw new Exception("No Records Found");
                 */

              
                #region For DocumentRegistration
                //if (model.ServiceID == Convert.ToInt16(ApiCommonEnum.enumServiceTypes.DocumentRegistration))  //for Document Registration Service
                //{
                //    if (dbContext.SCN_REG_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.ProcessMappingID == iProcessMappingID && c.DocumentID == model.DocumentID).Any())
                //        throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);

                //    List<SCN_REG_ScanDetails> oScanDetailsList = new List<SCN_REG_ScanDetails>();
                //    SCN_REG_ScanDetails scanDetailsModel = new SCN_REG_ScanDetails();

                //    if (dbContext.SCN_REG_ScanDetails.Any())
                //        maxScanID = dbContext.SCN_REG_ScanDetails.Select(c => c.ScanID).Max();

                //    scanDetailsModel.ScanID = maxScanID + 1;
                //    scanDetailsModel.OfficeID = model.OfficeID;
                //    scanDetailsModel.ProcessMappingID = iProcessMappingID;
                //    scanDetailsModel.DocumentID = model.DocumentID;
                //    scanDetailsModel.Pages = model.Pages;
                //    scanDetailsModel.ScanForwardDate = DateTime.Now;
                //    oScanDetailsList.Add(scanDetailsModel);

                //    if (model.IsEnclosure)
                //    {
                //        scanDetailsModel = new SCN_REG_ScanDetails();
                //        scanDetailsModel.ScanID = maxScanID + 2;
                //        scanDetailsModel.OfficeID = model.OfficeID;
                //        int iScanningEnclosures = Convert.ToInt16(CommonEnum.ScaningServicesProcess.ScanningEnclosures);
                //        scanDetailsModel.ProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == iScanningEnclosures).Select(m => m.ProcessMappingID).FirstOrDefault();
                //        scanDetailsModel.DocumentID = model.DocumentID;
                //        scanDetailsModel.Pages = 0;
                //        scanDetailsModel.ScanForwardDate = DateTime.Now;

                //        if (dbContext.SCN_REG_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.ProcessMappingID == scanDetailsModel.ProcessMappingID && c.DocumentID == model.DocumentID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //        oScanDetailsList.Add(scanDetailsModel);
                //    }
                //    else
                //        throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.ParameterEmpty);

                //    dbContext.SCN_REG_ScanDetails.AddRange(oScanDetailsList);
                //    dbContext.SaveChanges();
                //}

                #endregion


                #region For MarriageRegistration
                //else if (model.ServiceID == Convert.ToInt32(CommonEnum.enumServiceTypes.MarriageRegistration))  //for Marriage Registration Service
                //{
                //    if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.Notice)
                //    {
                //        if (dbContext.SCN_MAR_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.MarriageApplicationID == model.MarriageApplicationID && c.NoticeID == model.NoticeID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }
                //    else if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.Registration)
                //    {
                //        if (dbContext.SCN_MAR_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.MarriageApplicationID == model.MarriageApplicationID && c.MarriageID == model.MarriageID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }
                //    else if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.Objection)
                //    {
                //        if (dbContext.SCN_MAR_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.MarriageApplicationID == model.MarriageApplicationID && c.ObjectionID == model.ObjectionID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }
                //    else if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.Enquiry)
                //    {
                //        if (dbContext.SCN_MAR_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.MarriageApplicationID == model.MarriageApplicationID && c.EnquiryID == model.EnquiryID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }

                //    List<SCN_MAR_ScanDetails> oScanDetailsList = new List<SCN_MAR_ScanDetails>();
                //    SCN_MAR_ScanDetails scanDetailsModel = new SCN_MAR_ScanDetails();

                //    if (dbContext.SCN_MAR_ScanDetails.Any())
                //        maxScanID = dbContext.SCN_MAR_ScanDetails.Select(c => c.ScanID).Max();

                //    scanDetailsModel.ScanID = maxScanID + 1;
                //    scanDetailsModel.OfficeID = model.OfficeID;
                //    scanDetailsModel.ProcessMappingID = iProcessMappingID;
                //    scanDetailsModel.MarriageApplicationID = model.MarriageApplicationID;
                //    scanDetailsModel.MarriageID = model.MarriageID;
                //    scanDetailsModel.NoticeID = model.NoticeID;
                //    scanDetailsModel.ObjectionID = model.ObjectionID;
                //    scanDetailsModel.EnquiryID = model.EnquiryID;
                //    scanDetailsModel.Pages = model.Pages;
                //    scanDetailsModel.ScanForwardDate = DateTime.Now;
                //    oScanDetailsList.Add(scanDetailsModel);

                //    if (model.IsEnclosure)
                //    {
                //        scanDetailsModel = new SCN_MAR_ScanDetails();
                //        scanDetailsModel.ScanID = maxScanID + 2;
                //        scanDetailsModel.OfficeID = model.OfficeID;
                //        int iScanningEnclosures = Convert.ToInt16(CommonEnum.ScaningServicesProcess.ScanningEnclosures);
                //        scanDetailsModel.ProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == iScanningEnclosures).Select(m => m.ProcessMappingID).FirstOrDefault();
                //        scanDetailsModel.MarriageApplicationID = model.MarriageApplicationID;
                //        scanDetailsModel.MarriageID = model.MarriageID;
                //        scanDetailsModel.NoticeID = model.NoticeID;
                //        scanDetailsModel.ObjectionID = model.ObjectionID;
                //        scanDetailsModel.EnquiryID = model.EnquiryID;
                //        scanDetailsModel.Pages = 0;
                //        scanDetailsModel.ScanForwardDate = DateTime.Now;
                //        oScanDetailsList.Add(scanDetailsModel);
                //    }
                //    else
                //        throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.ParameterEmpty);

                //    dbContext.SCN_MAR_ScanDetails.AddRange(oScanDetailsList);
                //    dbContext.SaveChanges();
                //}
                #endregion


                #region For FirmRegistration      
               
                //else if (model.ServiceID == Convert.ToInt16(ApiCommonEnum.enumServiceTypes.FirmRegistration))  
                //{
                //    if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmAmendment)
                //    {
                //        if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID && c.AmendmentID == model.AmendmentID).Count() >= 2)
                //            throw new Exception("Document Already Exists");
                //    }
                //    else if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmDissolution)
                //    {
                //        if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID && c.DissolutionID == model.DissolutionID).Count() >= 2)
                //            throw new Exception("Document Already Exists");
                //    }
                //    else if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmRegistration)
                //    {
                //        if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID).Count() >= 2)
                //            throw new Exception("Document Already Exists");
                //    }

                //    if (model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmRegistration) || model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmAmendment) || model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmDissolution))
                //    {
                //        List<SCN_FRM_ScanDetails> oScanDetailsList = new List<SCN_FRM_ScanDetails>();
                //        SCN_FRM_ScanDetails scanDetailsModel = new SCN_FRM_ScanDetails();

                //        if (dbContext.SCN_FRM_ScanDetails.Any())
                //            maxScanID = dbContext.SCN_FRM_ScanDetails.Select(c => c.ScanID).Max();


                //        if (model.IsEnclosure)
                //        {
                //            scanDetailsModel = new SCN_FRM_ScanDetails();
                //            scanDetailsModel.ScanID = maxScanID + 1;
                //            scanDetailsModel.OfficeID = model.OfficeID;
                //            int iScanningEnclosures = Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.ScanningEnclosures);
                //            scanDetailsModel.ProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == iScanningEnclosures).Select(m => m.ProcessMappingID).FirstOrDefault();
                //            scanDetailsModel.FirmID = model.FirmID;
                //            scanDetailsModel.AmendmentID = model.AmendmentID;
                //            scanDetailsModel.DissolutionID = model.DissolutionID;
                //            scanDetailsModel.Pages = 0;
                //            scanDetailsModel.ScanForwardDate = DateTime.Now;
                //            oScanDetailsList.Add(scanDetailsModel);
                //        }

                //        dbContext.SCN_FRM_ScanDetails.AddRange(oScanDetailsList);
                //        dbContext.SaveChanges();
                //    }
                //}

                #endregion


                #region For FirmRegistration


                if (model.ServiceID == Convert.ToInt16(ApiCommonEnum.enumServiceTypes.FirmRegistration))  
                {
                    if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmAmendment)
                    {
                        //if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID && c.AmendmentID == model.AmendmentID).Count() >= 2)
                        //    throw new Exception("Document Already Exists");
                    }
                    else if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmDissolution)
                    {
                        //if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID && c.DissolutionID == model.DissolutionID).Count() >= 2)
                        //    throw new Exception("Document Already Exists");
                    }
                        //ProcessID=11
                    else if (model.ProcessID == (short)ApiCommonEnum.ScaningServicesProcess.FirmRegistration)
                    {
                        //if (dbContext.SCN_FRM_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.FirmID == model.FirmID).Count() >= 2)
                        //    throw new Exception("Document Already Exists");
                    }

                    //FirmRegistration=1      FirmAmendment=2    FirmDissolution=3
                    if (model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmRegistration) || model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmAmendment) || model.ProcessID == Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.FirmDissolution))
                    {
                        //List<SCN_FRM_ScanDetails> oScanDetailsList = new List<SCN_FRM_ScanDetails>();
                        //SCN_FRM_ScanDetails scanDetailsModel = new SCN_FRM_ScanDetails();

                        ////if (dbContext.SCN_FRM_ScanDetails.Any())
                        ////    maxScanID = dbContext.SCN_FRM_ScanDetails.Select(c => c.ScanID).Max();

                        //if (model.IsEnclosure)     // always true 
                        //{
                        //    scanDetailsModel = new SCN_FRM_ScanDetails();
                        //    scanDetailsModel.ScanID = maxScanID + 1;
                        //    scanDetailsModel.OfficeID = model.OfficeID;
                        //    int iScanningEnclosures = Convert.ToInt16(ApiCommonEnum.ScaningServicesProcess.Enclosures);   //5
                        //    scanDetailsModel.ProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == iScanningEnclosures).Select(m => m.ProcessMappingID).FirstOrDefault();
                        //    scanDetailsModel.FirmID = model.FirmID;
                        //    scanDetailsModel.AmendmentID = model.AmendmentID;
                        //    scanDetailsModel.DissolutionID = model.DissolutionID;
                        //    scanDetailsModel.Pages = 0;
                        //    scanDetailsModel.ScanForwardDate = DateTime.Now;
                        //    oScanDetailsList.Add(scanDetailsModel);
                        //}
                            
                        //dbContext.SCN_FRM_ScanDetails.AddRange(oScanDetailsList);
                        //dbContext.SaveChanges();
                    }
                }
              

                #endregion









                #region DR Order
                //else if (model.ServiceID == Convert.ToInt16(CommonEnum.enumServiceTypes.DROrder) || model.ServiceID == Convert.ToInt16(CommonEnum.enumServiceTypes.CourtOrder) || model.ServiceID == Convert.ToInt16(CommonEnum.enumServiceTypes.Liability))
                //{
                //    if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.DROrder)
                //    {
                //        if (dbContext.SCN_Other_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.DROrderID == model.DROrderID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }
                //    else if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.CourtOrder)
                //    {
                //        if (dbContext.SCN_Other_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.CourtOrderID == model.CourtOrderID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }
                //    else if (model.ProcessID == (short)CommonEnum.ScaningServicesProcess.Liability)
                //    {
                //        if (dbContext.SCN_Other_ScanDetails.Where(c => c.OfficeID == model.OfficeID && c.LiabilityID == model.LiabilityID).Any())
                //            throw new ValidationException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.DocumentAlreadyExists);
                //    }

                //    if (model.IsEnclosure)
                //    {
                //        SCN_Other_ScanDetails scanDetailsModel = new SCN_Other_ScanDetails();

                //        if (dbContext.SCN_Other_ScanDetails.Any())
                //            maxScanID = dbContext.SCN_Other_ScanDetails.Select(c => c.ScanID).Max();
                //        scanDetailsModel.ScanID = maxScanID + 1;
                //        scanDetailsModel.OfficeID = model.OfficeID;
                //        int iScanningEnclosures = Convert.ToInt16(CommonEnum.ScaningServicesProcess.ScanningEnclosures);
                //        scanDetailsModel.ProcessMappingID = dbContext.MAS_SCN_ScanProcessMapping.Where(m => m.ProcessID == model.ProcessID && m.ScanProcessID == iScanningEnclosures).Select(m => m.ProcessMappingID).FirstOrDefault();
                //        scanDetailsModel.CourtOrderID = model.CourtOrderID;
                //        scanDetailsModel.DROrderID = model.DROrderID;
                //        scanDetailsModel.LiabilityID = model.LiabilityID;
                //        scanDetailsModel.Pages = model.Pages;
                //        scanDetailsModel.ScanForwardDate = DateTime.Now;
                //        dbContext.SCN_Other_ScanDetails.Add(scanDetailsModel);
                //        dbContext.SaveChanges();
                //    }
                //    else
                //        throw new Exception("Parameter Empty");
                //}

                #endregion


                return string.Empty;
            }
                /*
            catch (NoRecordsFoundException ex)
            {
                throw ex;
            }
            catch (ValidationException ex)
            {
                throw ex;
            }
             
            catch
            {
                throw new BusinessException(CoreException.ExceptionMessages.DocumentScanning.UnlockDocumentsExceptions.AddScanningDetailsException);
            }
                 */

            catch (Exception )
            {
                throw ;
            }
           
        }

        #endregion


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

    }
}
