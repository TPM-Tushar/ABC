using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ErrorDetails
{
    public class ErrorDetailsResponseModel
    {
        public List<DriveInformationModel> DriveInfoModelList { get; set; }
        public bool isError { get; set; }
        public string sErrorMsg { get; set; }
        public string[] DirectoryNameArray { get; set; }
        public string[] FileNameArray { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 18-05-2019
        public List<SelectListItem> ErrorDirectoryList { get; set; }
    }

    public class DriveInformationModel
    {
        public string DriveName { get; set; }
        public string FreeSpace { get; set; }
        public string TotalSpace { get; set; }
        public string FileSystem { get; set; }
        public string FreeSpacePercentage { get; set; }
        public string DriveType { get; set; }

    }
    public class FileContentResponseModel
    {
        public bool isError { get; set; }
        public string sErrorMsg { get; set; }
        public string DownloadFileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class FileContentRequestModel
    {
        public bool isZipRequired { get; set; }
        public string FilePath { get; set; }
    }
}
