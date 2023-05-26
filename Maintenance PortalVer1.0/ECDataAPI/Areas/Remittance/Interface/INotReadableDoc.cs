#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   INotReadableDoc.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Not Readable Documents Details.

*/
#endregion

using CustomModels.Models.Remittance.NotReadableDoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface INotReadableDoc
    {
        NotReadableDocModel NotReadableDocView(int OfficeID);

       NotReadableDocResultModel GetNotReadableDocDetails(NotReadableDocModel notReadableDocModel);
    }
}
