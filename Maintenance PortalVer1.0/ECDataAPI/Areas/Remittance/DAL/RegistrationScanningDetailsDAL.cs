#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetailsDAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for Registration Scanning Details Report .

*/
#endregion


using CustomModels.Models.Remittance.RegistrationScanningDetails;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class RegistrationScanningDetailsDAL
    {
        KaveriEntities dbContext = null;
        public RegistrationScanningDetailsModel RegistrationScanningDetailsView(int DocumentTypeId)
        {
            RegistrationScanningDetailsModel resModel = new RegistrationScanningDetailsModel();
            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.SROfficeList = new List<SelectListItem>();
                SelectListItem FirstItem = new SelectListItem();
                FirstItem.Text = "All";
                FirstItem.Value = "0";
                resModel.SROfficeList.Insert(0,FirstItem);
                if (DocumentTypeId != 4)
                {
                    List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                    SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                    if (SROMasterList != null)
                    {
                        if (SROMasterList.Count() > 0)
                        {
                            foreach (var item in SROMasterList)
                            {
                                SelectListItem selectListOBJ = new SelectListItem();

                                selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
                                selectListOBJ.Value = item.SROCode.ToString();

                                resModel.SROfficeList.Add(selectListOBJ);
                            }
                        }
                    } 
                }else
                {
                    List<DistrictMaster> DistrictMasterList = dbContext.DistrictMaster.ToList();
                    DistrictMasterList = DistrictMasterList.OrderBy(x => x.DistrictNameE).ToList();
                    if (DistrictMasterList != null)
                    {
                        if (DistrictMasterList.Count() > 0)
                        {
                            foreach (var item in DistrictMasterList)
                            {
                                SelectListItem selectListOBJ = new SelectListItem();

                                selectListOBJ.Text = item.DistrictNameE + " (" + item.DistrictCode.ToString() + ")";
                                selectListOBJ.Value = item.DistrictCode.ToString();

                                resModel.SROfficeList.Add(selectListOBJ);
                            }
                        }
                    }
                }
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var GetDocumentList = objCommon.GetDocumentType();
                GetDocumentList.RemoveRange(1, 1);
                //GetDocumentList.RemoveRange(3, 1);
                resModel.DocumentType = GetDocumentList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if(dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            return resModel;
        }

        public RegistrationScanningDetailsResultModel GetRegistrationScanningDetails(RegistrationScanningDetailsModel registrationScanningDetailsModel)
        {

            RegistrationScanningDetailsResultModel resultModel = new RegistrationScanningDetailsResultModel();
            resultModel.registrationScanningDetailsTableModelsList = new List<RegistrationScanningDetailsTableModel>();
            RegistrationScanningDetailsTableModel resModel = null;

            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                var Result = new List<RegistrationScanningDetailsTableModel>();
                var DocumentTypeID = registrationScanningDetailsModel.DocumentTypeId;
                var SROfficeID = registrationScanningDetailsModel.SROfficeID;
                long SrCount = 1;
                
                switch (DocumentTypeID)
                    {
                    case 2://DocumentType : Marriage
                        {

                            switch(SROfficeID)
                            {
                                case 0://SROfficeID == 0
                                    {

                                        Result = (dbContext.MarriageRegistration.Where(x=>(x.DateOfRegistration >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.DateOfRegistration <= (registrationScanningDetailsModel.DateTime_ToDate.Date)))
                                                   .GroupJoin(dbContext.ScanMaster.Where(d => d.DocumentTypeID == 2),
                                                    t1 => new { p1 = t1.RegistrationID, p2 = t1.PSROCode },
                                                    t2 => new { p1 = t2.DocumentID, p2 = t2.SROCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d => d.DocumentTypeID == 2),
                                                    t => new { p1 = t.t1.RegistrationID, p2 = t.t1.PSROCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.CDMaster.Where(d=>d.DocumentTypeID == 2),
                                                    t => new { p1 = t.t1.PSROCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.SROCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        MarriageCaseNo = x.t1.MarriageCaseNo,
                                                        SROCode = x.t1.PSROCode,
                                                        DateOfRegistration = x.t1.DateOfRegistration,
                                                        DateOfRegistration_Date = x.t1.DateOfRegistration.ToString(),
                                                        MarriageRegistrationID = x.t1.RegistrationID,
                                                        ScanMasterID = x.t2.DocumentID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        //Added By Tushar on 13 Jan 2023
                                                        ScanFilePath=x.t2.BackupPath+x.t2.FileName
                                                        //End By Tushar on 13 Jan 2023
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();
                                     
                                        break;
                                    }
                                default://SROfficeID != 0
                                    {

                                        Result = (dbContext.MarriageRegistration.Where(x => (x.DateOfRegistration >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.DateOfRegistration <= (registrationScanningDetailsModel.DateTime_ToDate.Date)) && x.PSROCode == registrationScanningDetailsModel.SROfficeID)
                                                   .GroupJoin(dbContext.ScanMaster.Where(d => d.DocumentTypeID == 2 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t1 => new { p1 = t1.RegistrationID, p2 = t1.PSROCode },
                                                    t2 => new { p1 = t2.DocumentID, p2 = t2.SROCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d => d.DocumentTypeID == 2 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = t.t1.RegistrationID, p2 = t.t1.PSROCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.CDMaster.Where(d => d.DocumentTypeID == 2 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = t.t1.PSROCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.SROCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        MarriageCaseNo = x.t1.MarriageCaseNo,
                                                        SROCode = x.t1.PSROCode,
                                                        DateOfRegistration = x.t1.DateOfRegistration,
                                                        DateOfRegistration_Date = x.t1.DateOfRegistration.ToString(),
                                                        MarriageRegistrationID = x.t1.RegistrationID,
                                                        ScanMasterID = x.t2.DocumentID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        //Added By Tushar on 13 Jan 2023
                                                        ScanFilePath = x.t2.BackupPath + x.t2.FileName
                                                        //End By Tushar on 13 Jan 2023
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();
                 
                                        break;
                                    }
                            }
                            break;
                        }
                    case 3://DocumentType : Notice
                        {
                            switch (SROfficeID)
                            {
                                case 0://SROfficeID == 0
                                    {
       
                                        Result = (dbContext.NoticeMaster.Where(x => (x.NoticeIssuedDate >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.NoticeIssuedDate <= (registrationScanningDetailsModel.DateTime_ToDate.Date)))
                                                   .GroupJoin(dbContext.ScanMaster.Where(d => d.DocumentTypeID == 5),
                                                    t1 => new { p1 = t1.NoticeID, p2 = t1.PSROCode },
                                                    t2 => new { p1 = t2.DocumentID, p2 = t2.SROCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d => d.DocumentTypeID == 5),
                                                    t => new { p1 = t.t1.NoticeID, p2 = t.t1.PSROCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.CDMaster.Where(d => d.DocumentTypeID == 2),
                                                    t => new { p1 = t.t1.PSROCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.SROCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        MarriageCaseNo = x.t1.NoticeNo,
                                                        SROCode = x.t1.PSROCode,
                                                        DateOfRegistration = x.t1.NoticeIssuedDate,
                                                        DateOfRegistration_Date = x.t1.NoticeIssuedDate.ToString(),
                                                        MarriageRegistrationID = x.t1.NoticeID,
                                                        ScanMasterID = x.t2.DocumentID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        //Added By Tushar on 13 Jan 2023
                                                        ScanFilePath = x.t2.BackupPath + x.t2.FileName
                                                        //End By Tushar on 13 Jan 2023
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();
                   
                                     
                                        break;
                                    }
                                default://SROfficeID != 0
                                    {
                                      
                                        Result = (dbContext.NoticeMaster.Where(x => (x.NoticeIssuedDate >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.NoticeIssuedDate <= (registrationScanningDetailsModel.DateTime_ToDate.Date)) && x.PSROCode == registrationScanningDetailsModel.SROfficeID)
                                                   .GroupJoin(dbContext.ScanMaster.Where(d => d.DocumentTypeID == 5 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t1 => new { p1 = t1.NoticeID, p2 = t1.PSROCode },
                                                    t2 => new { p1 = t2.DocumentID, p2 = t2.SROCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d => d.DocumentTypeID == 5 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = t.t1.NoticeID, p2 = t.t1.PSROCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.CDMaster.Where(d => d.DocumentTypeID == 2 && d.SROCode == registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = t.t1.PSROCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.SROCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        MarriageCaseNo = x.t1.NoticeNo,
                                                        SROCode = x.t1.PSROCode,
                                                        DateOfRegistration = x.t1.NoticeIssuedDate,
                                                        DateOfRegistration_Date = x.t1.NoticeIssuedDate.ToString(),
                                                        MarriageRegistrationID = x.t1.NoticeID,
                                                        ScanMasterID = x.t2.DocumentID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        //Added By Tushar on 13 Jan 2023
                                                        ScanFilePath = x.t2.BackupPath + x.t2.FileName
                                                        //End By Tushar on 13 Jan 2023
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();
                       
                                        break;
                                    }
                            }
                            break;
                        }
                        //Added By Tushar on 16 Jan 2023
                    case 4://DocumentType : Firm
                        {
                            switch (SROfficeID)
                            {
                                case 0://SROfficeID == 0
                                    {

                                        Result = (dbContext.FirmMaster.Where(x => (x.DateOfRegistration >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.DateOfRegistration <= (registrationScanningDetailsModel.DateTime_ToDate.Date)))
                                                   .GroupJoin(dbContext.SocietyFirmScanmaster.Where(s => s.IsSociety == false),
                                                    t1 => new { p1 = (long)t1.RegistrationID, p2 = t1.DRCode },
                                                    t2 => new { p1 = t2.RegistrationID, p2 = t2.DRCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d=>d.DocumentTypeID == 7),
                                                    t => new { p1 = (long)t.t1.RegistrationID, p2 = t.t1.DRCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = -t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.SocietyFirmCDMaster.Where(d => d.IsSociety == false),
                                                    t => new { p1 = t.t1.DRCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.DRCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        FirmNumber = x.t1.FirmNumber,
                                                        DRCode = x.t1.DRCode,
                                                        DateOfRegistration = x.t1.DateOfRegistration,
                                                        DateOfRegistration_Date = x.t1.DateOfRegistration.ToString(),
                                                        ScanMasterID = x.t2.RegistrationID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        ScanFilePath = x.t2.BackupPath + x.t2.FileName
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();


                                        break;
                                    }
                                default://SROfficeID != 0
                                    {

                                        Result = (dbContext.FirmMaster.Where(x => (x.DateOfRegistration >= registrationScanningDetailsModel.DateTime_FromDate.Date
                                                   && x.DateOfRegistration <= (registrationScanningDetailsModel.DateTime_ToDate.Date)) && x.DRCode == registrationScanningDetailsModel.SROfficeID)
                                                   .GroupJoin(dbContext.SocietyFirmScanmaster.Where(d => d.IsSociety == false && d.DRCode == registrationScanningDetailsModel.SROfficeID),
                                                    t1 => new { p1 = (long)t1.RegistrationID, p2 = t1.DRCode },
                                                    t2 => new { p1 = t2.RegistrationID, p2 = t2.DRCode },
                                                    (t1, t2Group) => new { t1, t2Group }
                                                    ).GroupJoin(dbContext.ScannedFileUploadDetails.Where(d => d.DocumentTypeID == 7 && d.SROCode == -registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = (long)t.t1.RegistrationID, p2 = t.t1.DRCode },
                                                    t3 => new { p1 = t3.DocumentID, p2 = -t3.SROCode },
                                                    (t, t3Group) => new { t.t1, t.t2Group, t3Group }
                                                    ).GroupJoin(dbContext.SocietyFirmCDMaster.Where(d => d.IsSociety == false && d.DRCode == registrationScanningDetailsModel.SROfficeID),
                                                    t => new { p1 = t.t1.DRCode, p2 = t.t1.CDNumber },
                                                    t4 => new { p1 = t4.DRCode, p2 = t4.VolumeName },
                                                    (t, t4Group) => new { t.t1, t.t2Group, t.t3Group, t4Group })
                                                    ).SelectMany(t => t.t2Group.DefaultIfEmpty(), (t, t2) => new { t.t1, t2, t.t3Group, t.t4Group }
                                                    ).SelectMany(t => t.t3Group.DefaultIfEmpty(), (t, t3) => new { t.t1, t.t2, t3, t.t4Group }
                                                    ).SelectMany(t => t.t4Group.DefaultIfEmpty(), (t, t4) => new { t.t1, t.t2, t.t3, t4 })
                                                    .Select(x => new RegistrationScanningDetailsTableModel
                                                    {
                                                        FirmNumber = x.t1.FirmNumber,
                                                        DRCode = x.t1.DRCode,
                                                        DateOfRegistration = x.t1.DateOfRegistration,
                                                        DateOfRegistration_Date = x.t1.DateOfRegistration.ToString(),
                                                        ScanMasterID = x.t2.RegistrationID.ToString(),
                                                        ScannedFileUploadDetailsID = x.t3.DocumentID.ToString(),
                                                        IsCDWritten = x.t4.VolumeName,
                                                        CDNumber = x.t1.CDNumber,
                                                        ScanFilePath = x.t2.BackupPath + x.t2.FileName
                                                    }
                                                     ).OrderByDescending(s => s.DateOfRegistration).ToList();

                                        break;
                                    }
                            }
                            break;
                        }
                        //End By Tushar on 16 jan 2023
                    default:
                        {
                            break;
                        }
                }
                foreach (var item in Result)
                {
                    resModel = new RegistrationScanningDetailsTableModel();
                    resModel.srNo= SrCount++;
                    resModel.MarriageCaseNo =string.IsNullOrEmpty(item.MarriageCaseNo)?"--": item.MarriageCaseNo;
                    resModel.SROCode = item.SROCode;
                    //Added By Tushar on 16 Jan 2023
                    resModel.FirmNumber = string.IsNullOrEmpty(item.FirmNumber) ? "--" : item.FirmNumber;
                    resModel.DRCode = item.DRCode;
                    //End By Tushar on 16 jan 2023
                    resModel.CDNumber = string.IsNullOrEmpty(item.CDNumber) ? "--" : item.CDNumber;
                    resModel.DateOfRegistration_Date = string.IsNullOrEmpty(item.DateOfRegistration_Date) ? "" :Convert.ToDateTime(item.DateOfRegistration_Date).ToString("dd/MM/yyyy  HH:mm:ss");
                    resModel.MarriageRegistrationID = item.MarriageRegistrationID;
                    resModel.ScanMasterID =string.IsNullOrEmpty(item.ScanMasterID)? "<i  style='color:red' aria-hidden='true'>NO</i>" : "<i  style='color:green' aria-hidden='true'>YES</i>";
                    resModel.ScannedFileUploadDetailsID =string.IsNullOrEmpty(item.ScannedFileUploadDetailsID)? "<i  style='color:red' aria-hidden='true'>NO</i>" : "<i  style='color:green' aria-hidden='true'>YES</i>";
                    resModel.IsCDWritten =string.IsNullOrEmpty(item.IsCDWritten)? "<i  style='color:red' aria-hidden='true'>NO</i>" : "<i  style='color:green' aria-hidden='true'>YES</i>";
                    //Added By Tushar on 13 Jan 2023
                    resModel.ScanFilePath = string.IsNullOrEmpty(item.ScanFilePath) ? "--" :item.ScanFilePath;
                    //End By Tushar on 13 Jan 2023
                    resultModel.registrationScanningDetailsTableModelsList.Add(resModel);
                }

               

            }catch(Exception ex)
            {
                throw;
            }

            return resultModel;
        }
    }
}