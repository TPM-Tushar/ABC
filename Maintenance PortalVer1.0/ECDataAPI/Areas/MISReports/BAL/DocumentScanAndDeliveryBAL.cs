using CustomModels.Models.MISReports.DocumentScanAndDeliveryReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DocumentScanAndDeliveryBAL : IDocumentScanAndDelivery
    {
        public DocumentScanAndDeliveryREQModel DocumentScanAndDeliveryView(int OfficeID)
        {
            return (new DocumentScanAndDeliveryDAL().DocumentScanAndDeliveryView(OfficeID));
        }

        public DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetails(DocumentScanAndDeliveryREQModel model)
        {
            return (new DocumentScanAndDeliveryDAL().DocumentScanAndDeliveryDetails(model));
        }

        public int DocumentScanAndDeliveryCount(DocumentScanAndDeliveryREQModel model)
        {
            return (new DocumentScanAndDeliveryDAL().DocumentScanAndDeliveryCount(model));
        }
        public DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetailsForSRO(DocumentScanAndDeliveryREQModel model)
        {
            return (new DocumentScanAndDeliveryDAL().DocumentScanAndDeliveryDetailsForSRO(model));
        }
    }
}