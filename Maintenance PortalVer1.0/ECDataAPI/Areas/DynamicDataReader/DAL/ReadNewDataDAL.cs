using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.DynamicDataReader.DAL
{
    public class ReadNewDataDAL : IReadNewData
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion


        public ReadNewDataResModel SaveNewQuerySearchParameter(ReadNewDataModel model)
        {
            ReadNewDataResModel newQueryResModel = new ReadNewDataResModel();
            try
            {
                QueryAnalyserDetails queryAnalyserDetails = new QueryAnalyserDetails();
                queryAnalyserDetails.QueryId = (int)dbContext.USP_GET_SEQID_QueryAnalyserDetails().FirstOrDefault();
                queryAnalyserDetails.Purpose = model.Purpose;
                queryAnalyserDetails.DBName = model.DatabaseName;
                queryAnalyserDetails.QueryText = model.QueryText;
                queryAnalyserDetails.LoginName = model.LoginName;
                queryAnalyserDetails.InsertDateTime = DateTime.Now;
                queryAnalyserDetails.NoOfRows = 0;
                queryAnalyserDetails.IsNoError = true;
                queryAnalyserDetails.ErrorDesc = string.Empty;
                queryAnalyserDetails.DBUserName = "kaveriro";

                dbContext.QueryAnalyserDetails.Add(queryAnalyserDetails);
                dbContext.SaveChanges();
                newQueryResModel.IsError = false;

            }
            catch (Exception ex)
            {
                throw;
            }
            return newQueryResModel;
        }
    }
}