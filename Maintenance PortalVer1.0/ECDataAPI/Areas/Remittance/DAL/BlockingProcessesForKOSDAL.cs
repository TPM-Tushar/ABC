using CustomModels.Models.Remittance.BlockingProcessesForKOS;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.PreRegApplicationDetailsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class BlockingProcessesForKOSDAL : IBlockingProcessesForKOS
    {
        public BlocingProcessesForKOSWrapperModel GetBlockingProcessForKOSDetails(BlockingProcessesForKOSReqModel model)
        {
            BlocingProcessesForKOSWrapperModel resModel = new BlocingProcessesForKOSWrapperModel();
            resModel.blockingProcessesModelsList = new List<BlockingProcessesForKOSModel>();
            try
            {
                ApplicationDetailsService service = new ApplicationDetailsService();
                BlocingProcessesList blocingProcessesList = service.GetBlockingProcessesForKOS();
                int SrNo = 1;
                //var modelList = blocingProcessesList.blockingProcessesModelsList.Skip(model.startLen).Take(model.totalNum);

                if (blocingProcessesList.IsError)
                {
                    resModel.IsError = true;
                    resModel.ErrorMessage = blocingProcessesList.ErrorMessage;
                }
                else
                {
                    if (blocingProcessesList.blockingProcessesModelsList != null && blocingProcessesList.blockingProcessesModelsList.Count() > 0)
                    {
                        foreach (var item in blocingProcessesList.blockingProcessesModelsList)
                        {
                            BlockingProcessesForKOSModel obj = new BlockingProcessesForKOSModel();
                            obj.SrNo = SrNo++;
                            obj.session_id = item.session_id;
                            obj.command = item.command;
                            obj.blocking_session_id = item.blocking_session_id;
                            obj.wait_type = item.wait_type;
                            obj.wait_time = item.wait_time;
                            obj.wait_resource = item.wait_resource;
                            obj.TEXT = item.TEXT;
                            obj.DateTime = item.DateTime.ToString();

                            resModel.blockingProcessesModelsList.Add(obj);
                        }
                    }
                }

                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}