#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IErrorLogFiles.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.ErrorLogFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IErrorLogFiles
    {
        ErrorLogFilesViewModel ErrorLogFilesView();

        //ErrorLogFilesViewModel GetFileNameList(String FolderName);
        FileInfoModelWrapper LoadFolderNameGrid(ErrorFileRequestModel requestModel);
        FileDownloadModel DownLoadFile(ErrorFileRequestModel requestModel);
        FileDownloadModel DownLoadZippedFile(ErrorFileRequestModel model);

        ErrorLogFilesViewModel LoadDriveInfoGrid(ErrorFileRequestModel requestModel);

        ErrorLogFilesViewModel GetErrorDirectoryList(String ApplicationName);
    }
}
