using CustomModels.Models.Remittance.MissingSacnDocument;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class MissingScanDocumentBAL: IMissingScanDocument
    {
        MissingScanDocumentDAL missingSacnDocumentDAL = new MissingScanDocumentDAL();
        public MissingScanDocumentModel MissingSacnDocumentView()
        {
            return missingSacnDocumentDAL.MissingSacnDocumentView();
        }
        public MissingScanDocumentResModel GetMissingScanDocumentDetails(MissingScanDocumentModel missingSacnDocumentModel)
        {
            return missingSacnDocumentDAL.GetMissingScanDocumentDetails(missingSacnDocumentModel);
        }
    }
}