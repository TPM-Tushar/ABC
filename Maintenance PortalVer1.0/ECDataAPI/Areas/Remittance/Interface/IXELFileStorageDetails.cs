using CustomModels.Models.Remittance.XELFileStorageDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IXELFileStorageDetails
    {
        XELFileStorageViewModel XELFileStorageView(int OfficeID);

        XELFileStorageWrapperModel XELFileOfficeList(XELFileStorageViewModel reqModel);

        XELFileStorageWrapperModel XELFileListOfficeWise(XELFileStorageViewModel reqModel);

        XELFileStorageViewModel RootDirectoryTable(XELFileStorageViewModel reqModel);
        
        XELFileStorageViewModel XELFileDownloadPathVerify(XELFileStorageViewModel reqModel);

        XELFileStorageWrapperModel XELFileDownload(XELFileStorageViewModel reqModel);
        
        // FileDownload();
    }
}
