#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   HighValuePropertiesBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.HighValueProperties;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class HighValuePropertiesBAL : IHighValueProperties
    {
        IHighValueProperties highValuePropertiesDAL = new HighValuePropertiesDAL();


        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        public HighValuePropertiesReqModel HighValuePropertiesView()
        {
            return highValuePropertiesDAL.HighValuePropertiesView();

        }


        /// <summary>
        /// returns HighValuePropDetailsResponseModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HighValuePropDetailsResModel GetHighValuePropertyDetails(HighValuePropDetailsReqModel model)
        {
            return highValuePropertiesDAL.GetHighValuePropertyDetails(model);

        }
    }
}