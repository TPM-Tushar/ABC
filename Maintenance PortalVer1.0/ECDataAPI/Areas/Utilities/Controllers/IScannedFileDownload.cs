using CustomModels.Models.Utilities.ScannedfileDownload;

namespace ECDataAPI.Areas.Utilities.Controllers
{
    internal interface IScannedFileDownload
    {
        ScannedFileDownloadView ScannedFileDownloadView(int OfficeID);
        ScannedFileDownloadView LoadScannedFileDownloadLogTable(long UserID);
        ScannedFileDownloadResModel GetScannedFileByteArray(ScannedFileDownloadView ReqModel);
        bool SaveScannedFileDownloadDetails(ScannedFileDownloadView ReqModel);

    }
}