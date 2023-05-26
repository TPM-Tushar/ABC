#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsDAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   DAL layer for Support Enclosure
*/
#endregion

using CustomModels.Models.BhoomiMapping;
using ECDataAPI.Areas.BhoomiMapping.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Security;
using System.Text;
using System.IO;
using ECDataUI.Session;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Transactions;
using System.Data;

namespace ECDataAPI.Areas.BhoomiMapping.DAL
{
    public class IsVillageMatchingDAL : IIsVillageMatching
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public List<IsVillageMatchingViewTableModel> IsVillageMatchingView(int officeID, int LevelID, long UserID)
        {
            try
            {
                List<IsVillageMatchingViewTableModel> IVM = new List<IsVillageMatchingViewTableModel>();
                
                dbContext = new KaveriEntities();
                List<SROMaster> SroList = dbContext.SROMaster.OrderBy(t => t.SRONameE).ToList();
                int Sno = 0;
                string IVMstr = string.Empty;
                foreach (var item in SroList)
                {
                    Sno++;
                    IVMstr = string.Empty;
                    if (item.IsVillageMatching == true)
                    {
                        IVMstr = "Yes";
                    }
                    else
                    {
                        IVMstr = "No";
                    }
                    IVM.Add(new IsVillageMatchingViewTableModel { SrNo =Sno, SROName =item.SRONameE, IsVillageMatching =  IVMstr});
                }
                return IVM;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}