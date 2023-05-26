using CustomModels.Models.MISReports.DocumentScanAndDeliveryReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDocumentScanAndDelivery
    {
        DocumentScanAndDeliveryREQModel DocumentScanAndDeliveryView(int OfficeID);
        DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetails(DocumentScanAndDeliveryREQModel model);
        int DocumentScanAndDeliveryCount(DocumentScanAndDeliveryREQModel model);
        DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetailsForSRO(DocumentScanAndDeliveryREQModel model);

    }
}
