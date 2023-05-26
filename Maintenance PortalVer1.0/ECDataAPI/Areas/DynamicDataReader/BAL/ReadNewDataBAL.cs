using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.DAL;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.DynamicDataReader.BAL
{
    public class ReadNewDataBAL : IReadNewData
    {
        IReadNewData dalOBJ = new ReadNewDataDAL();

        public ReadNewDataResModel SaveNewQuerySearchParameter(ReadNewDataModel model)
        {
            return dalOBJ.SaveNewQuerySearchParameter(model);
        }
    }
}