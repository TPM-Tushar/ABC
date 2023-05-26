using CustomModels.Models.XELFiles;
using ECDataAPI.Areas.XELFiles.DAL;
using ECDataAPI.Areas.XELFiles.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.XELFiles.BAL
{
    public class XELFilesDetailsBAL : IXELFilesDetails
    {
        IXELFilesDetails xelFilesDal = new XELFilesDetailsDAL();

        public XELFilesViewModel GetAuditSpecificationDetails(int OfficeID)
        {
            return xelFilesDal.GetAuditSpecificationDetails(OfficeID);
        }

        public XELFilesResModel GetAuditSpecificationDetailsTableData(XELFilesViewModel model)
        {
            return xelFilesDal.GetAuditSpecificationDetailsTableData(model);
        }

        public int GetAuditSpecificationDetailsTotalCount(XELFilesViewModel model)
        {
            return xelFilesDal.GetAuditSpecificationDetailsTotalCount(model);
        }

        public XELFilesViewModel GetJobsDetails(int OfficeID)
        {
            return xelFilesDal.GetJobsDetails(OfficeID);
        }

        public RegisteredJobsListModel GetRegisteredJobsTableData(XELFilesViewModel model)
        {
            return xelFilesDal.GetRegisteredJobsTableData(model);
        }

        public int GetRegisteredJobsTotalCount(XELFilesViewModel model)
        {
            return xelFilesDal.GetRegisteredJobsTotalCount(model);
        }

        public XELFilesViewModel RegisterJobsDetails(XELFilesViewModel model)
        {
            return xelFilesDal.RegisterJobsDetails(model);
        }

        public XELLogViewModel GetXELLogView()
        {
            return xelFilesDal.GetXELLogView();
        }
        public XELLogViewModel LoadXELLogDetails(XELLogViewModel model)
        {
            return xelFilesDal.LoadXELLogDetails(model);
        }
        public XELLogViewModel GetOfficeList(String OfficeType)
        {
            return xelFilesDal.GetOfficeList(OfficeType);
        }

    }
}