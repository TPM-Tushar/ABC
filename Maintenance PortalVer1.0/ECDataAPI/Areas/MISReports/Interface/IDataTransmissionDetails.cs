using CustomModels.Models.MISReports.DataTransmissionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDataTransmissionDetails
    {
        DataTransReqModel DataTransmissionDetailsView(int OfficeID);

        DataTransWrapperModel LoadDataTransmissionDetails(DataTransReqModel model);

        int DataTransmissionDetailsCount(DataTransReqModel model);
    }
}
