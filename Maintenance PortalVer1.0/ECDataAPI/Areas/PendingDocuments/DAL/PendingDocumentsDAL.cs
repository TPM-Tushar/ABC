using CustomModels.Models.PendingDocuments;
using ECDataAPI.Areas.PendingDocuments.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.ECDATA_PENDOCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.PendingDocuments.DAL
{
    public class PendingDocumentsDAL : IPendingDocuments
    {
        //KaveriEntities dbContext = null;
        ECDATA_PENDOCSEntities dbContext_PENDOCS = null;

        public PendingDocumentsViewModel PendingDocumentsView(int officeID)
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();


            try
            {
                dbContext_PENDOCS = new ECDATA_PENDOCSEntities();
                PendingDocumentsViewModel pd = new PendingDocumentsViewModel();
                pd.SROfficeList = new List<SelectListItem>();
                pd.SROfficeList.Insert(0, objCommon.GetDefaultSelectListItem("Select", "0"));
                pd.SROfficeList.AddRange(dbContext_PENDOCS.SROMaster_PENDOCS.OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
                return pd;
            }
            catch (Exception ex)
            {
                PendingDocumentsViewModel pd = null;
                return pd;
            }
        }







        public List<PendingDocumentsTableModel> PendingDocumentsAvailaibility(int SROCode)
        {
            List<PendingDocumentsTableModel> finalList = new List<PendingDocumentsTableModel>();

            try
            {
                int sno = 1;
                dbContext_PENDOCS = new ECDATA_PENDOCSEntities();


                List<Entity.ECDATA_PENDOCS.DocPendingHistory_PENDOCS> PendingList = dbContext_PENDOCS.DocPendingHistory_PENDOCS.Where(c => c.SROCode == SROCode).ToList<Entity.ECDATA_PENDOCS.DocPendingHistory_PENDOCS>();

                List<Entity.ECDATA_PENDOCS.NoticeMaster_PENDOCS> PendingNoticeList = dbContext_PENDOCS.NoticeMaster_PENDOCS.Where(c => c.SROCode == SROCode).ToList<Entity.ECDATA_PENDOCS.NoticeMaster_PENDOCS>();


                //finalList = dbContext.DocumentMaster.Where(x => x.SROCode == SROCode).Select(y => new PendingDocumentsTableModel { PendingNumber = y.PendingDocumentNumber, PresentationDate = y.PresentDateTime.ToString() }).ToList<PendingDocumentsTableModel>();

                string SROName = dbContext_PENDOCS.SROMaster_PENDOCS.Where(z => z.SROCode == SROCode).Select(k => k.SRONameE).FirstOrDefault();

                foreach (Entity.ECDATA_PENDOCS.DocPendingHistory_PENDOCS dph in PendingList)
                {
                    PendingDocumentsTableModel pd = new PendingDocumentsTableModel();
                    pd.SROName = SROName;
                    pd.PendingReason = dph.PendingReasonMaster.ReasonInEnglish;
                    pd.PendingNumber = "[ Document Pending Number: " + (string.IsNullOrEmpty(dph.DocumentMaster.PendingDocumentNumber)?dph.DocumentMaster.DocumentNumber.ToString() : dph.DocumentMaster.PendingDocumentNumber) + " ]";
                    pd.PresentationDate = string.IsNullOrEmpty(dph.DocumentMaster.PresentDateTime.ToString()) ? "-" : Convert.ToDateTime(dph.DocumentMaster.PresentDateTime.ToString()).ToString("dd-MM-yyyy hh:mm:ss");
                    pd.DateOfPending = dph.PendingDate.ToString("dd-MM-yyyy hh:mm:ss");
                    pd.SrNo = sno++;
                    finalList.Add(pd);
                }

                foreach (Entity.ECDATA_PENDOCS.NoticeMaster_PENDOCS notice in PendingNoticeList)
                {
                    PendingDocumentsTableModel pd = new PendingDocumentsTableModel();
                    pd.SROName = SROName;
                    pd.PendingReason = "Not Applicable";
                    pd.PendingNumber = "[ Marriage Notice Number: " + (string.IsNullOrEmpty(notice.NoticeNo) ? notice.NoticeNo.ToString() : notice.NoticeNo.ToString()) + " ]";
                    pd.PresentationDate = string.IsNullOrEmpty(notice.NoticeIssuedDate.ToString()) ? "-" : Convert.ToDateTime(notice.NoticeIssuedDate.ToString()).ToString("dd-MM-yyyy hh:mm:ss");
                    pd.DateOfPending = "Not Applicable";
                    pd.SrNo = sno++;
                    finalList.Add(pd);
                }


                return finalList;
               
            }
            catch (Exception ex)
            {
                finalList = null;

                return finalList;

            }
        }

    }
}