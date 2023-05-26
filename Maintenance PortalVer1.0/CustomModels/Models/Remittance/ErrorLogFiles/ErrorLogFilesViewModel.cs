using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ErrorLogFiles
{
    public class ErrorLogFilesViewModel
    {
        public List<DriveInfoModel> DriveInfoModelList { get; set; }       
        public bool isError { get; set; }
        public string sErrorMessage{ get; set; }
        public string PresentDirectory { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 17-05-2019
        public List<SelectListItem> ApplicationList { get; set; }

        [Display(Name = "Application Name")]
        public String ApplicationName { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 18-05-2019
        public List<SelectListItem> ErrorDirectoryList { get; set; }

        [Display(Name = "Error Directory Name")]
        public String ErrorDirectoryName { get; set; }
    }

    public class DriveInfoModel
    {
        //[Display(Name = "Drive Name")]
        public string DriveName { get; set; }

        // [Display(Name = "Free Space")]
        public string FreeSpace { get; set; }

        //[Display(Name = "Total Space")]
        public string TotalSpace { get; set; }

        //[Display(Name = "File System")]
        public string FileSystem { get; set; }

        //[Display(Name = "Free Space Percentage")]
        public string FreeSpacePercentage { get; set; }

        // [Display(Name = "Drive Type")]
        public string DriveType { get; set; }
    }


    public class FileInfoModel
    {
        public String FileName { get; set; }

        public String ActionBtn { get; set; }
    }


    public class FileInfoModelWrapper
    {
        public List<FileInfoModel> FolderNameList { get; set; }

        public List<FileInfoModel> FileNameList { get; set; }

        public bool isError { get; set; }

        public string sErrorMessage { get; set; }

        public string PresentDirectory { get; set; }

    }

    public class ErrorFileRequestModel
    {
        public string Path { get; set; }
        public bool GetFolderList { get; set; }
        public bool GetFileList { get; set; }
        public String ServiceName { get; set; }
    }

    public class FileDownloadModel
    {
        public bool IsErrorField { get; set; }

        public string SErrorMsgField { get; set; }

        public string DownloadFileNameField { get; set; }

        public byte[] FileContentField { get; set; }

    }

}
