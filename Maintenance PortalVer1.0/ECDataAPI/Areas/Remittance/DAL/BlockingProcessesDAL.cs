using CustomModels.Models.Remittance.BlockingProcesses;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class BlockingProcessesDAL : IBlockingProcesses
    {
        private KaveriEntities dbContext = null;

        public BlockingProcessWrapperModel GetBlockingProcessDetails()
        {
            BlockingProcessWrapperModel retModel = new BlockingProcessWrapperModel();
            retModel.BlockingProcessList = new List<BlockingProcessResponseModel>();

            try
            {
                dbContext = new KaveriEntities();
                List<USP_BLOCKING_PROCESSES_Result> bLOCKING_PROCESSES_Results = dbContext.USP_BLOCKING_PROCESSES().ToList();
                if (bLOCKING_PROCESSES_Results != null && bLOCKING_PROCESSES_Results.Count > 0)
                {
                    foreach (var item in bLOCKING_PROCESSES_Results)
                    {
                        BlockingProcessResponseModel obj = new BlockingProcessResponseModel();
                        obj.session_id = item.session_id.HasValue ? item.session_id.Value.ToString() : "";
                        obj.command = string.IsNullOrEmpty(item.command) ? "" : item.command;
                        obj.blocking_session_id = item.blocking_session_id.HasValue ? item.blocking_session_id.Value.ToString() : "";
                        obj.wait_type = string.IsNullOrEmpty(item.wait_type) ? "" : item.wait_type;
                        obj.wait_time = item.wait_time;
                        obj.wait_resource = string.IsNullOrEmpty(item.wait_resource) ? "" : item.wait_resource;
                        obj.TEXT = string.IsNullOrEmpty(item.TEXT) ? "" : item.TEXT;
                        obj.DateTime = item.DateTime;

                        retModel.BlockingProcessList.Add(obj);
                    }


                }

                //BlockingProcessResponseModel objTemp = new BlockingProcessResponseModel();
                //objTemp.session_id = "1";
                //objTemp.command = "1- command";
                //objTemp.blocking_session_id = "1";
                //objTemp.wait_type = "1";
                //objTemp.wait_time = 1;
                //objTemp.wait_resource = "1- resource";
                //objTemp.TEXT = "1";
                //objTemp.DateTime = DateTime.Now;
                //retModel.BlockingProcessList.Add(objTemp);


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


            return retModel;
        }
    }
}