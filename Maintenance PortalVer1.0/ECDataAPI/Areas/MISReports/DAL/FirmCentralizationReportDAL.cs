using CustomModels.Models.MISReports.FirmCentralizationReport;
using ECDataAPI.AnywhereCCService;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class FirmCentralizationReportDAL : IFirmCentralizationReport
    {
        public FirmCentralizationReportViewModel FirmCentralizationReportView(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                FirmCentralizationReportViewModel ResultViewModel = new FirmCentralizationReportViewModel();
                ResultViewModel.DROfficeList = new List<SelectListItem>();

                List<DistrictMaster> DROMasterList = new List<DistrictMaster>();
                CommonFunctions objCommon = new CommonFunctions();
                using (KaveriEntities dbContext = new KaveriEntities())
                {
                    ResultViewModel.DROfficeList.Add(objCommon.GetDefaultSelectListItem("Select", "0"));
                    DROMasterList = dbContext.DistrictMaster.ToList();
                    if (DROMasterList != null)
                    {
                        DROMasterList = DROMasterList.OrderBy(x => x.DistrictNameE).ToList();
                        foreach (var item in DROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.DistrictNameE;
                            select.Value = item.DistrictCode.ToString();
                            ResultViewModel.DROfficeList.Add(select);
                        }
                    }
                }
                return ResultViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FirmCentralizationReportResultModel GetFirmCentralizationDetails(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                FirmCentralizationReportResultModel firmCentralizationReportResultModel = new FirmCentralizationReportResultModel();
                firmCentralizationReportResultModel.DetailsList = new List<FirmCentralizationReportResultDetailModel>();
                CCFileExistReqModel cCFileExistReqModel = new CCFileExistReqModel();
                List<CCFileExistReqDetailModel> cCFileExistReqDetailModelList = new List<CCFileExistReqDetailModel>();
                using (KaveriEntities dbcontext = new KaveriEntities())
                {
                    List<FirmDataCentralizationDetails> firmDataCentralizationDetailList = dbcontext.FirmDataCentralizationDetails.
                                                                                         Where(m => m.DROCode ==
                                                                                         firmCentralizationReportViewModel.DROfficeID &&
                                                                                         m.DateOfRegistration.Value >= firmCentralizationReportViewModel.DateTime_FromDate &&
                                                                                         m.DateOfRegistration.Value < firmCentralizationReportViewModel.DateTime_ToDate
                                                                                         ).ToList();

                    firmCentralizationReportResultModel.DistrictName = dbcontext.DistrictMaster.Where(m => m.DistrictCode == firmCentralizationReportViewModel.DROfficeID).Select(t => t.DistrictNameE).FirstOrDefault();
                    if (firmDataCentralizationDetailList != null)
                    {
                        int SNo = 1;
                        foreach (var item in firmDataCentralizationDetailList)
                        {
                            bool IsFirmMAsterPresent = dbcontext.FirmMaster.Where(m => m.RegistrationID == item.RegistrationID && m.DRCode == item.DROCode).Any();
                            bool IsSocietyFirmScanmaster = dbcontext.SocietyFirmScanmaster.Where(m => m.RegistrationID == item.RegistrationID && m.DRCode == item.DROCode && m.IsSociety == false).Any();
                            ScannedFileUploadDetails scannedFileUploadDetails = dbcontext.ScannedFileUploadDetails.Where(m => m.DocumentID == item.RegistrationID && m.SROCode == -item.DROCode && m.DocumentTypeID == 7).FirstOrDefault();

                            FirmCentralizationReportResultDetailModel firmCentralizationReportResultDetailModel = new FirmCentralizationReportResultDetailModel();
                            firmCentralizationReportResultDetailModel.Sno = SNo++;
                            firmCentralizationReportResultDetailModel.RegistrationID = item.RegistrationID;
                            firmCentralizationReportResultDetailModel.FirmNumber = item.FirmNumber;
                            firmCentralizationReportResultDetailModel.CDNumber = item.CDNumber;
                            firmCentralizationReportResultDetailModel.IsLocalFirmDataCentralized = item.IsFirmDataCentralizaed ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            firmCentralizationReportResultDetailModel.bool_IsLocalFirmDataCentralized = item.IsFirmDataCentralizaed;
                            //firmCentralizationReportResultDetailModel.IsLocalScanDocumentUpload = item.IsFirmDataCentralizaed ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            //firmCentralizationReportResultDetailModel.bool_IsLocalScanDocumentUpload = item.IsFirmDataCentralizaed;
                            firmCentralizationReportResultDetailModel.IsLocalScanDocumentUpload = Convert.ToBoolean(item.IsScanDocumentUploaded)? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            firmCentralizationReportResultDetailModel.bool_IsLocalScanDocumentUpload = Convert.ToBoolean(item.IsScanDocumentUploaded);
                            firmCentralizationReportResultDetailModel.IsCDWriting = item.CDID == null ? "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>" : (item.CDID > 0 ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>");
                            firmCentralizationReportResultDetailModel.bool_IsCDWriting = item.CDID == null ? false : (item.CDID > 0);
                            firmCentralizationReportResultDetailModel.IsFirmDataCentralized = IsFirmMAsterPresent ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            firmCentralizationReportResultDetailModel.bool_IsFirmDataCentralized = IsFirmMAsterPresent;
                            firmCentralizationReportResultDetailModel.IsScanDocumentUploaded = IsSocietyFirmScanmaster ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            firmCentralizationReportResultDetailModel.bool_IsScanDocumentUploaded = IsSocietyFirmScanmaster;
                            firmCentralizationReportResultDetailModel.IsUploadedScanDocumentPresent = scannedFileUploadDetails != null ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" : "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                            firmCentralizationReportResultDetailModel.bool_IsUploadedScanDocumentPresent = scannedFileUploadDetails != null;
                            firmCentralizationReportResultDetailModel.DateOfRegistration = item.DateOfRegistration.Value.ToString("dd/MM/yyyy");
                            firmCentralizationReportResultDetailModel.DroCode = item.DROCode;
                            //if (scannedFileUploadDetails != null)
                            //{
                            //    firmCentralizationReportResultDetailModel.FilePath = scannedFileUploadDetails.RootDirectory ?? "~" + "\\" + scannedFileUploadDetails.UploadPath ?? "~" + "\\" + scannedFileUploadDetails.ScannedFileName ?? "~";

                            //}

                            firmCentralizationReportResultModel.DetailsList.Add(firmCentralizationReportResultDetailModel);

                            CCFileExistReqDetailModel cCFileExistReqDetailModel = new CCFileExistReqDetailModel();
                            cCFileExistReqDetailModel.CCFileDetailsBy = (CCFileDetailsBy)Enum.Parse(typeof(CCFileDetailsBy), firmCentralizationReportViewModel.CCFileDetailsBy);
                            //cCFileExistReqDetailModel.CCFileDetailsBy = CCFileDetailsBy.FileExist;
                            cCFileExistReqDetailModel.DocumentID = item.RegistrationID;
                            cCFileExistReqDetailModel.DocumentTypeID = 7;
                            cCFileExistReqDetailModel.FinalRegistrationNo = item.FirmNumber;
                            cCFileExistReqDetailModel.SroCode = -1 * item.DROCode;
                            cCFileExistReqDetailModelList.Add(cCFileExistReqDetailModel);
                        }
                        using (PreRegCCService objService = new PreRegCCService())
                        {
                            cCFileExistReqModel.CCFileExistReqDetailList = cCFileExistReqDetailModelList.ToArray();
                            CCFileExistResModel cCFileExistResModel = objService.CheckIfCCFileExist(cCFileExistReqModel);
                            if (cCFileExistResModel != null && cCFileExistResModel.ResponseMsg == String.Empty && cCFileExistResModel.ResponseStatus == "000")
                            {
                                foreach (var item in cCFileExistResModel.CCFileDetailList)
                                {
                                    firmCentralizationReportResultModel.DetailsList.
                                        Where(m => m.RegistrationID == item.DocumentID &&
                                        m.DroCode == (-1 * item.SroCode)).ToList().
                                        ForEach(m =>
                                        {
                                            m.FilePath = item.FilePath ?? "--";
                                            m.bool_IsFilePresent = item.IsFileExist;
                                            m.IsFilePresent = item.IsFileExist ? "<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" :
                                            "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                                            m.bool_IsFileReadable = item.IsFileReadable;
                                            m.IsFileReadable = item.IsFileReadable ?"<i class='fa fa-check' style='color:green' aria-hidden='true'></i>" :
                                            "<i class='fa fa-close' style='color:red' aria-hidden='true'></i>";
                                        });
                                }
                            }
                        }
                    }
                }
                return firmCentralizationReportResultModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}