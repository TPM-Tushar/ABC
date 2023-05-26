using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.SearchCertificate;
using GauriServicesApi.Interface;
using GauriServicesApi.DAL;
namespace GauriServicesApi.BAL
{
    public class SearchCertificateBAL : ISearchCertificate
    {
        ISearchCertificate ObjDAL = new SearchCertificateDAL();

        

        public SearchCertificateModel SearchCertificate()
        {
            return ObjDAL.SearchCertificate();
        }
        public SearchCertificateModel GetCertificateDetails(CertificateModel ObjCertificate)
        {
            return ObjDAL.GetCertificateDetails(ObjCertificate);
        }

        public SearchCertificateModel GetCertificateByteArray(CertificateModel ObjCertificate)
        {
            return ObjDAL.GetCertificateByteArray(ObjCertificate);
        }
    }
}