/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: IKaveriIntegration
 * Author : Shubham Bhagat
 * Creation Date : 14 Oct 2019
 * Desc : Contract  
 * ECR No : 
*/

using CustomModels.Models.KaveriIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.KaveriIntegration.Interface
{
    public interface IKaveriIntegration
    {
        KaveriIntegrationModel KaveriIntegrationView(int OfficeID);
        KaveriIntegrationWrapperModel LoadKaveriIntegrationTable(KaveriIntegrationModel model);
        KIDetailsWrapperModel OtherTableDetails(KaveriIntegrationModel model);
    }
}
