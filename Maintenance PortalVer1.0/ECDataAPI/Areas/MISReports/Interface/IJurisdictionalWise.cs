#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IJurisdictionalWise.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.JurisdictionalWise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IJurisdictionalWise
    {
        JurisdictionalWiseModel JurisdictionalWiseView(int OfficeID);

        //JurisdictionalWiseSummary JurisdictionalWiseSummary(JurisdictionalWiseModel model);

        JurisdictionalWiseDetailWrapper JurisdictionalWiseDetail(JurisdictionalWiseModel model);

        int JurisdictionalWiseTotalCount(JurisdictionalWiseModel model);

        string GetSroName(int OfficeID);
    }
}