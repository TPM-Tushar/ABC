using CustomModels.Models.Remittance.XELFileStorageDetails;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class XELFileStorageDetailsBAL : IXELFileStorageDetails
    {
        public XELFileStorageViewModel XELFileStorageView(int OfficeID)
        {
            return new XELFileStorageDetailsDAL().XELFileStorageView(OfficeID);
        }

        public XELFileStorageWrapperModel XELFileOfficeList(XELFileStorageViewModel reqModel)
        {
            return new XELFileStorageDetailsDAL().XELFileOfficeList(reqModel);
        }

        public XELFileStorageWrapperModel XELFileListOfficeWise(XELFileStorageViewModel reqModel)
        {
            return new XELFileStorageDetailsDAL().XELFileListOfficeWise(reqModel);
        }
        public XELFileStorageViewModel RootDirectoryTable(XELFileStorageViewModel reqModel)
        {
            return new XELFileStorageDetailsDAL().RootDirectoryTable(reqModel);
        }

        public XELFileStorageViewModel XELFileDownloadPathVerify(XELFileStorageViewModel reqModel)
        {
            return new XELFileStorageDetailsDAL().XELFileDownloadPathVerify(reqModel);
        }

        public XELFileStorageWrapperModel XELFileDownload(XELFileStorageViewModel reqModel)
        {
            return new XELFileStorageDetailsDAL().XELFileDownload(reqModel);
        }
        
    }
}