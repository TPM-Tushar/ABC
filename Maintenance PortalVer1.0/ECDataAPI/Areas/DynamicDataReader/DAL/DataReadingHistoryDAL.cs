using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.DynamicDataReader.DAL
{
    public class DataReadingHistoryDAL : IDataReadingHistory
    {

        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        public DataReadingHistoryResModel GetDataReadingHistoryReport(DataReadingHistoryModel model)
        {
            DataReadingHistoryResModel resModel = new DataReadingHistoryResModel();
            try
            {
                List<DataReadingHistoryDetailModel> detailList = new List<DataReadingHistoryDetailModel>();
                DateTime fromDate = DateTime.Parse(model.FromDate);
                DateTime toDate = DateTime.Parse(model.ToDate);
                int SrNo = 1;

                var resultset = dbContext.QueryAnalyserDetails.Where(m => m.DBName == model.DatabaseName).ToList().OrderByDescending(X=>X.QueryId);
                var queryAnalyserDetails = resultset.Where(m => m.InsertDateTime.Date >= fromDate && m.InsertDateTime.Date <= toDate).ToList();

                if (queryAnalyserDetails != null)
                {
                    if (!model.IsExcel)
                    {
                        if (model.CurrentRoleID == Convert.ToInt16(ApiCommonEnum.RoleDetails.AIGRComp))
                        {
                            foreach (var item in queryAnalyserDetails)
                            {
                                DataReadingHistoryDetailModel obj = new DataReadingHistoryDetailModel();
                                obj.SrNo = SrNo++;
                                obj.QueryID = item.QueryId;
                                obj.DBName = item.DBName;
                                obj.Date = item.InsertDateTime.ToString();
                                obj.NoOfRows = item.NoOfRows ?? 0;
                                obj.Purpose = item.Purpose;
                                obj.LoginName = item.LoginName;
                                obj.DBUserName = item.DBUserName;
                                obj.QueryText = item.QueryText;
                                //for buttons

                                string ViewDataBtn = "<button type ='button' class='btn btn-group-md btn-success' onclick=ViewData('" + obj.QueryID + "') data-toggle='tooltip' data-placement='top' title='Click here' style='margin-right:10px;'>View Data</ button> ";
                                obj.QueryResultButtons = ViewDataBtn;
                                //obj.QueryResultButtons = "<button type ='button' class='btn btn-group-md btn-success' onclick=DownloadQueryPlanXML('" + detailModel.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'><i style = 'padding-right:3%;' class='fa fa-download'></i> Download</ button>";

                                detailList.Add(obj);
                            }
                        }
                        else
                        {
                            foreach (var item in queryAnalyserDetails)
                            {
                                DataReadingHistoryDetailModel obj = new DataReadingHistoryDetailModel();
                                obj.SrNo = SrNo++;
                                obj.QueryID = item.QueryId;
                                obj.DBName = item.DBName;
                                obj.Date = item.InsertDateTime.ToString();
                                obj.NoOfRows = item.NoOfRows ?? 0;
                                obj.Purpose = item.Purpose;
                                obj.LoginName = item.LoginName;
                                obj.DBUserName = item.DBUserName;
                                obj.QueryText = item.QueryText;
                                //for buttons
                                //if (model.CurrentRoleID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                                //{
                                //    string QueryResultBtn = "<button type ='button' class='btn btn-group-md btn-success' data-toggle='modal' data-target='#exampleModalLong' onclick=showmodelpopup('" + obj.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'>View Query</ button>";
                                //}

                                string ViewDataBtn = "<button type ='button' class='btn btn-group-md btn-success' onclick=ViewData('" + obj.QueryID + "') data-toggle='tooltip' data-placement='top' title='Click here' style='margin-right:10px;'>View Data</ button> ";
                                obj.QueryResultButtons = ViewDataBtn + "<button type ='button' class='btn btn-group-md btn-success' data-toggle='modal' data-target='#exampleModalLong' onclick=showmodelpopup('" + obj.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'>View Query</ button>";
                                //obj.QueryResultButtons = "<button type ='button' class='btn btn-group-md btn-success' onclick=DownloadQueryPlanXML('" + detailModel.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'><i style = 'padding-right:3%;' class='fa fa-download'></i> Download</ button>";

                                detailList.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in queryAnalyserDetails)
                        {
                            DataReadingHistoryDetailModel obj = new DataReadingHistoryDetailModel();
                            obj.SrNo = SrNo++;
                            obj.QueryID = item.QueryId;
                            obj.DBName = item.DBName;
                            obj.Date = item.InsertDateTime.ToString();
                            obj.NoOfRows = item.NoOfRows ?? 0;
                            obj.Purpose = item.Purpose;
                            obj.LoginName = item.LoginName;
                            obj.DBUserName = item.DBUserName;
                            obj.QueryText = item.QueryText;
                            
                            detailList.Add(obj);
                        }
                    }

                }

                resModel.TotalRecords = detailList.Count;
                resModel.dataReadingHistoryDetailModels = detailList;

            }
            catch (Exception e)
            {
                throw;
            }
            return resModel;
        }

        public DataReadingHistoryDetailModel GetDetailByQueryId(DataReadingHistoryDetailModel model)
        {
            DataReadingHistoryDetailModel resModel = new DataReadingHistoryDetailModel();
            try
            {
                QueryAnalyserDetails obj =  dbContext.QueryAnalyserDetails.Where(x => x.QueryId == model.QueryID).FirstOrDefault();
                if(obj != null)
                {
                    resModel.QueryID = obj.QueryId;
                    resModel.QueryText = obj.QueryText;
                    resModel.DBName = obj.DBName;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return resModel;
        }
    }
}