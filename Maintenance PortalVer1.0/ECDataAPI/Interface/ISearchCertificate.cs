using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomModels.Models.SearchCertificate;
namespace GauriServicesApi.Interface
{
    public interface ISearchCertificate
    {
        SearchCertificateModel SearchCertificate();
        SearchCertificateModel GetCertificateDetails(CertificateModel ObjCertificate);
        SearchCertificateModel GetCertificateByteArray(CertificateModel ObjCertificate);

    }
}
