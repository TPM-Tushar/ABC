///*----------------------------------------------------------------------------------------
// * Project Id:
// * Project Name: Kaveri Maintainance Portal
// * File Name: SROModificationDetailsBAL.cs
// * Author : Harshit Gupta
// * Creation Date :17/Jan/2019
// * Desc : Provides methods for DAL interaction
// * ECR No : 
// * ---------------------------------------------------------------------------------------*/
//using CustomModels.Models.Reports.DiagnosticReports;
//using ECDataAuditDetails.DAL;
//using System.Collections.Generic;

//namespace ECDataAuditDetails.BAL
//{
//    public class SROModificationDetailsBAL
//    {
//        SROModificationDetailsDAL objSROModificationRepository = new SROModificationDetailsDAL();

//        public List<SROModificationDetailsViewModel> GetModificationDetailsList(ECDataAuditDetailsRequestModel model)
//        {
//            return objSROModificationRepository.GetModificationDetailsList(model);
//        }

//        public SROModificationDetailsRequestModel GetSROModificationDetailsRequestModel()
//        {
//            return objSROModificationRepository.GetSROModificationDetailsRequestModel();
//        }

//        public List<SROModificationDetailsViewModel> GetModificationTypeDetails(long MasterId,long DetailsID)
//        {
//            return objSROModificationRepository.GetModificationTypeDetails(MasterId, DetailsID);
//        }

//        public List<SROModificationDetailsViewModel> GetModificationDetailsAllList(ECDataAuditDetailsRequestModel model)
//        {
//            return objSROModificationRepository.GetModificationDetailsAllList(model);
//        }
//    }
//}