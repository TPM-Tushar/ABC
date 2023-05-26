#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ISaleDeedRevCollection.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SaleDeedRevCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface ISaleDeedRevCollection
    {
        SaleDeedRevCollectionModel SaleDeedRevCollectionView(int OfficeID);

        SaleDeedRevCollectionOuterModel GetSaleDeedRevCollectionDetails(SaleDeedRevCollectionModel model);

        int GetSaleDeedRevCollectionDetailsTotalCount(SaleDeedRevCollectionModel model);
      
    }
}
