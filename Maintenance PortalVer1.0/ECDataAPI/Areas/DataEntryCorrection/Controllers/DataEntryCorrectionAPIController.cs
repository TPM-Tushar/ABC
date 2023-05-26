#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsAPIController.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   API controller for Support Enclosure
*/
#endregion

using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.BAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;

//using ECDataAPI.Areas.DataEntryCorrection.BAL;
//using ECDataAPI.Areas.DataEntryCorrection.Interface;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.SupportEnclosure.Controllers
{
    public class DataEntryCorrectionAPIController : ApiController
    {
        IDataEntryCorrection balObject = null;

        //Added by Madhusoodan on 05/08/2021
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/AddEditOrderDetails")]
        public IHttpActionResult AddEditOrderDetails(int orderID, int OfficeID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                DataEntryCorrectionOrderViewModel resModel = balObject.AddEditOrderDetails(orderID, OfficeID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhur
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/ViewBtnClickOrderTable")]
        public IHttpActionResult ViewBtnClickOrderTable(int OrderID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();
                DROrderFilePathResultModel resModel = balObject.ViewBtnClickOrderTable(OrderID);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }



        //Can be removed. Not in use
        //Added by Madhur
        //[HttpGet]
        //[Route("api/DataEntryCorrectionAPIController/EditSaveBtnClickOrderTable")]
        //public IHttpActionResult EditBtnClickOrderTable(string DROrderNumber)
        //{
        //    try
        //    {
        //        balObject = new DataEntryCorrectionBAL();
        //        string resModel = balObject.EditBtnClickOrderTable(DROrderNumber);

        //        return Ok(resModel);
        //    }

        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //Modified by Madhusoodan on 12/08/2021
        //Added by Madhur
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/DeleteDECOrder")]
        public IHttpActionResult DeleteDECOrder(int orderID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();
                bool isOrderDeleted = balObject.DeleteDECOrder(orderID);

                return Ok(isOrderDeleted);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhur
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/LoadDocDetailsTable")]
        public IHttpActionResult LoadDocDetailsTable(int DroCode, int SROCode, int RoleID,bool IsExcel )
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();
                List<DataEntryCorrectionOrderTableModel> resModel = new List<DataEntryCorrectionOrderTableModel>();
                resModel = balObject.LoadDocDetailsTable(DroCode, SROCode,  RoleID, IsExcel);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/LoadInsertUpdateDeleteView")]
        public IHttpActionResult LoadInsertUpdateDeleteView(int officeid)
        {
            try
            {
                return Ok(new DataEntryCorrectionBAL().LoadInsertUpdateDeleteView(officeid));
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 11/08/2021 (To take Order ID to save file)
        //GenerateNewOrderID
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/GenerateNewOrderID")]
        public IHttpActionResult GenerateNewOrderID(int OfficeID, int currentOrderID)
        {
            try
            {
                return Ok(new DataEntryCorrectionBAL().GenerateNewOrderID(OfficeID, currentOrderID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DataEntryCorrectionAPIController/SaveOrderDetails")]
        public IHttpActionResult SaveOrderDetails(DataEntryCorrectionOrderViewModel dataEntryCorrectionOrderViewModel)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();
                DataEntryCorrectionOrderResultModel resModel = new DataEntryCorrectionOrderResultModel();

                resModel = balObject.SaveOrderDetails(dataEntryCorrectionOrderViewModel);

                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpGet]
        [HttpPost]
        [Route("api/DataEntryCorrectionAPIController/DeleteCurrentOrderFile")]
        public IHttpActionResult DeleteCurrentOrderFile(DataEntryCorrectionOrderViewModel viewmodel)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                DataEntryCorrectionOrderResultModel resultModel = balObject.DeleteCurrentOrderFile(viewmodel.OrderID);

                return Ok(resultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/FinalizeDECOrder")]
        //public IHttpActionResult FinalizeDECOrder(int currentOrderID)
        public IHttpActionResult FinalizeDECOrder(int OrderID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                //string finalizeDECResp = balObject.FinalizeDECOrder(currentOrderID);
                string finalizeDECResp = balObject.FinalizeDECOrder(OrderID);

                return Ok(finalizeDECResp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by mayank on 13/08/2021
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/CheckifOrderNoExist")]
        public IHttpActionResult CheckifOrderNoExist(string OrderNo, int OrderID,int OfficeID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();
                bool resModel = balObject.CheckifOrderNoExist(OrderNo, OrderID, OfficeID);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 20/08/2021 to show Index II report for finalized orders
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/LoadIndexIIDetails")]
        public IHttpActionResult LoadIndexIIDetails(int OrderID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.LoadIndexIIDetails(OrderID);

                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Added by mayank District enabled for DEC
        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/DataEntryCorrectionView")]
        public IHttpActionResult DataEntryCorrectionView(int officeID,int LevelID, long UserID)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                DataEntryCorrectionViewModel resModel = balObject.DataEntryCorrectionView(officeID,LevelID,UserID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/DataEntryCorrectionAPIController/GetSroCodebyDistrict")]
        public IHttpActionResult GetSroCodebyDistrict(int DROCode)
        {
            try
            {
                balObject = new DataEntryCorrectionBAL();

                DataEntryCorrectionViewModel resModel = balObject.GetSroCodebyDistrict(DROCode);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
