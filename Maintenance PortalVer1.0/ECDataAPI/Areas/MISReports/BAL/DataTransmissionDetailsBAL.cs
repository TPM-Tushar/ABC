using CustomModels.Models.MISReports.DataTransmissionDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DataTransmissionDetailsBAL : IDataTransmissionDetails
    {
        public DataTransReqModel DataTransmissionDetailsView(int OfficeID)
        {
            return (new DataTransmissionDetailsDAL().DataTransmissionDetailsView(OfficeID));
        }

        public DataTransWrapperModel LoadDataTransmissionDetails(DataTransReqModel model)
        {
            return (new DataTransmissionDetailsDAL().LoadDataTransmissionDetails(model));
        }

        public int DataTransmissionDetailsCount(DataTransReqModel model)
        {
            return (new DataTransmissionDetailsDAL().DataTransmissionDetailsCount(model));
        }

    }
}