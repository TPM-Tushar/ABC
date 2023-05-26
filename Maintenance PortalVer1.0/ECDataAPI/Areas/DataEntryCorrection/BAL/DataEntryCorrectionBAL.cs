#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsBAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   BAL for Support Enclosure
*/
#endregion

using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.DAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SupportEnclosure.BAL
{
    public class DataEntryCorrectionBAL : IDataEntryCorrection
    {

        public DataEntryCorrectionViewModel LoadInsertUpdateDeleteView(int OfficeID)
        {
            return (new DataEntryCorrectionDAL().LoadInsertUpdateDeleteView(OfficeID));
        }

        IDataEntryCorrection dataEntryCorrectionDal = new DataEntryCorrectionDAL();

        //Added by Madhur
        public DROrderFilePathResultModel ViewBtnClickOrderTable(int OrderID)
        {
            return dataEntryCorrectionDal.ViewBtnClickOrderTable(OrderID);
        }

        //Can be removed. Not in use
        //Added by Madhur
        //public string EditBtnClickOrderTable(string DROrderNumber)
        //{
        //    return dataEntryCorrectionDal.EditBtnClickOrderTable(DROrderNumber);
        //}

        //Added by Madhur
        public bool DeleteDECOrder(int orderID)
        {
            return dataEntryCorrectionDal.DeleteDECOrder(orderID);
        }

        //Added by Madhur
        public List<DataEntryCorrectionOrderTableModel> LoadDocDetailsTable(int DroCode, int SROCode, int RoleID,bool IsExcel)
        {
            return dataEntryCorrectionDal.LoadDocDetailsTable(DroCode, SROCode,  RoleID,IsExcel);
        }
       
        public DataEntryCorrectionOrderResultModel SaveOrderDetails(DataEntryCorrectionOrderViewModel dataEntryCorrectionOrderViewModel)
        {
            return dataEntryCorrectionDal.SaveOrderDetails(dataEntryCorrectionOrderViewModel);
        }

        public string FinalizeDECOrder(int currentOrderID)
        {
            return dataEntryCorrectionDal.FinalizeDECOrder(currentOrderID);
        }

        //Added by Madhusoodan on 05/08/2021
        public DataEntryCorrectionOrderViewModel AddEditOrderDetails(int orderID, int OfficeID)
        {
            return dataEntryCorrectionDal.AddEditOrderDetails(orderID, OfficeID);
        }

        //Added by Madhusoodan on 11/08/2021 (To take Order ID to save file)
        public DROrderFilePathResultModel GenerateNewOrderID(int OfficeID, int currentOrderID)
        {
            return dataEntryCorrectionDal.GenerateNewOrderID(OfficeID, currentOrderID);
        }

        public DataEntryCorrectionOrderResultModel DeleteCurrentOrderFile(int orderID)
        {
            return dataEntryCorrectionDal.DeleteCurrentOrderFile(orderID);
        }

        //Added by mayank on 13/08/2021
        public bool CheckifOrderNoExist(string OrderNo, int OrderId,int OfficeID)
        {
            return dataEntryCorrectionDal.CheckifOrderNoExist(OrderNo, OrderId, OfficeID);
        }

        public DataEntryCorrectionResultModel LoadIndexIIDetails(int OrderID)
        {
            return dataEntryCorrectionDal.LoadIndexIIDetails(OrderID);
        }
        //Added by mayank District enabled for DEC
        public DataEntryCorrectionViewModel DataEntryCorrectionView(int officeID, int LevelID, long UserID)
        {
            return dataEntryCorrectionDal.DataEntryCorrectionView(officeID, LevelID,UserID);
        }

        public DataEntryCorrectionViewModel GetSroCodebyDistrict(int DroCode)
        {
            return dataEntryCorrectionDal.GetSroCodebyDistrict(DroCode);
        }
    }
}