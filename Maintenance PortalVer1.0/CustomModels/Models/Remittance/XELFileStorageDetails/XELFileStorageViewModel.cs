using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.XELFileStorageDetails
{
    public class XELFileStorageViewModel
    {
        public List<RootDirInfoModel> RootDirInfoModelList{get;set;}

        public String OfficeType { get; set; }

        public String SROCode { get; set; }

        public int StartLen { get; set; }

        public int TotalNum { get; set; }

        public String SearchValue{ get; set; }

        public bool IsExcel { get; set; }

        public String FileDownloadPath { get; set; }

        public bool IsFileExistAtDownloadPath { get; set; }



    }


    public class XELFileStorageWrapperModel
    {
        public List<XELFileOfficeListRESModel> XELFileOfficeListRESModelList { get; set; }

        public List<XELFileListOfficeWiseRESModel> XELFileListOfficeWiseRESModelList { get; set; }

        public String ErrorMessage { get; set; }

        public int TotalRecords { get; set; }

        public byte[] FileContentField { get; set; }
    }

    public class XELFileOfficeListRESModel
    {
        public int SerialNo { get; set; }

        public String OfficeName { get; set; }

        public String NoOfFiles { get; set; }

        public decimal TotalSizeOnDiskInMB { get; set; }

        public String LastCentralizedOn { get; set; }

        //public int TotalNoOfFiles { get; set; }
    }

    public class XELFileListOfficeWiseRESModel
    {
        public int SerialNo { get; set; }

        public String FileName { get; set; }

        public decimal FileSizeInMB { get; set; }

        public String FileDateTime { get; set; }

        public String FilePath { get; set; }

        public String EventStartDate { get; set; }

        public String EventEndDate { get; set; }

        //public String FileDownload { get; set; }

        public String FileReadDateTime { get; set; }

    }

    public class RootDirInfoModel
    {
        public int SerialNo { get; set; }

        public String RootDirectory { get; set; }

        public String TotalSpace { get; set; }

        public String UsedSpace { get; set; }

        public String FreeSpace { get; set; }

        public String XELSpace { get; set; }
    }
}
