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
using ECDataAPI.Areas.DataEntryCorrection.Interface;
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
    public class SelectDocumentBAL : ISelectDocument
    {
        //IDataEntryCorrection dataEntryCorrectionDal = new DataEntryCorrectionDAL();
        ISelectDocument selectDocumentDAL = new SelectDocumentDAL();

        public DataEntryCorrectionViewModel SelectDocumentTabView(int OfficeID, bool isEditMode, int currentOrderID)
        {
            return selectDocumentDAL.SelectDocumentTabView(OfficeID, isEditMode, currentOrderID);
        }

        public DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return selectDocumentDAL.LoadPropertyDetailsData(dataEntryCorrectionViewModel);
        }
        //Added by Madhur on 27-7-21

        public List<DataEntryCorrectionPreviousPropertyDetailModel> LoadPreviousPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return selectDocumentDAL.LoadPreviousPropertyDetailsData(dataEntryCorrectionViewModel);
        }
        //Added by Madhur on 27-7-21


        public string ViewBtnClickPreviousPropTable(int OrderID)
        {
            return selectDocumentDAL.ViewBtnClickPreviousPropTable(OrderID);
        }

        public SelectDocumentResultModel SaveSection68Note(DataEntryCorrectionViewModel decViewModel)
        {
            return selectDocumentDAL.SaveSection68Note(decViewModel);
        }

        public Section68NoteResultModel LoadPreviousSec68Note(int orderID, long propertyID, int officeID)
        {
            return selectDocumentDAL.LoadPreviousSec68Note(orderID, propertyID, officeID);
        }
        
        public bool CheckOrderFileExists(int currentOrderID)
        {
            return selectDocumentDAL.CheckOrderFileExists(currentOrderID);
        }

        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        public bool DeleteSection68Note(int NoteID)
        {
            return selectDocumentDAL.DeleteSection68Note(NoteID);
        }

        //Added by Madhusoodan on 13/08/2021 to load finalize btn if Section 68 Note is added for Current Order ID
        public bool IsSection68NoteAddedForOrderID(int currentOrderID)
        {
            return selectDocumentDAL.IsSection68NoteAddedForOrderID(currentOrderID);
        }
		//Added by mayank on 12/08/2021
        public bool CheckifOrderNoteExist(int OrderId, long PropertyID)
        {
            return selectDocumentDAL.CheckifOrderNoteExist(OrderId, PropertyID);
        }
    }
}