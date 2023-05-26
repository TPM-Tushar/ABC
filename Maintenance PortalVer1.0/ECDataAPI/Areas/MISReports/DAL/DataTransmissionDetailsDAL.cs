using CustomModels.Models.MISReports.DataTransmissionDetails;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DataTransmissionDetailsDAL: IDataTransmissionDetails
    {
        KaveriEntities dbContext = null;

        public DataTransReqModel DataTransmissionDetailsView(int OfficeID)
        {
            DataTransReqModel model = new DataTransReqModel();
            model.SROList = new List<SelectListItem>();
            try
            {
                dbContext = new KaveriEntities();
                var SROMasterList = dbContext.SROMaster.OrderBy(c => c.SRONameE).ToList();
                model.SROList.Add(new SelectListItem { Text = "All", Value = "0" });
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem select = new SelectListItem()
                            {
                                Text = item.SRONameE,
                                Value = item.SROCode.ToString()
                            };
                            model.SROList.Add(select);
                        }
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return model;
        }

        public DataTransWrapperModel LoadDataTransmissionDetails(DataTransReqModel model)
        {
            DataTransWrapperModel resModel = new DataTransWrapperModel();
            DataTransDetailsModel ReportsDetails = null;
            List<DataTransDetailsModel> ReportsDetailsList = new List<DataTransDetailsModel>();
            resModel.DataTransDetailsModelList = new List<DataTransDetailsModel>();

            try
            {
                dbContext = new KaveriEntities();

                //var ReceiptDetailsList = dbContext.USP_GET_ROWCOUNT_OF_TABLE_WRAPPER(model.SROID).Skip(model.StartLen).Take(model.TotalNum).ToList();
                var ReceiptDetailsList = dbContext.USP_GET_ROWCOUNT_OF_TABLE_WRAPPER(model.SROID).ToList();




                resModel.TotalRecords = ReceiptDetailsList.Count;
                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        ReceiptDetailsList = ReceiptDetailsList.Skip(model.StartLen).Take(model.TotalNum).ToList();
                    }
                }


                int counter = (model.StartLen + 1); //To start Serial Number 
                foreach (var item in ReceiptDetailsList)
                {
                    ReportsDetails = new DataTransDetailsModel();
                    ReportsDetails.SerialNumber = counter++;                    
                    ReportsDetails.TableName= string.IsNullOrEmpty(item.tablename) ? string.Empty : item.tablename;
                    ReportsDetails.Count = item.TotalRows==null ? 0 : Convert.ToInt64(item.TotalRows);
                    resModel.DataTransDetailsModelList.Add(ReportsDetails);
                }

                resModel.SroName = model.SROID == 0 ? "All" : dbContext.SROMaster.Where(x => x.SROCode == model.SROID).Select(x => x.SRONameE).FirstOrDefault();
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return resModel;
        }

        public int DataTransmissionDetailsCount(DataTransReqModel model)
        {
            try
            {
                dbContext = new KaveriEntities();
                var ReceiptDetailsList = dbContext.USP_GET_ROWCOUNT_OF_TABLE_WRAPPER(model.SROID).ToList();
                return ReceiptDetailsList.Count();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
    }
}