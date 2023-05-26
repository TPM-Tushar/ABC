using CustomModels.Models.DataEntryCorrection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DataEntryCorrection.Interface
{
    interface ISelectDocument
    {
        DataEntryCorrectionViewModel SelectDocumentTabView(int OfficeID, bool isEditMode, int currentOrderID);

        DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        //Added by Madhur on 27-7-21
        List<DataEntryCorrectionPreviousPropertyDetailModel> LoadPreviousPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        //Added by Madhur on 27-7-21

        string ViewBtnClickPreviousPropTable(int OrderID);

        //Added by Madhusoodan on 06/08/2021
        SelectDocumentResultModel SaveSection68Note(DataEntryCorrectionViewModel decViewModel);
        Section68NoteResultModel LoadPreviousSec68Note(int orderID, long propertyID, int officeID);
       

        bool CheckOrderFileExists(int orderID);
        bool DeleteSection68Note(int NoteID);
        bool IsSection68NoteAddedForOrderID(int currentOrderID);
		//Added by mayank 12/08/2021
        bool CheckifOrderNoteExist(int OrderID, long PropertyID);
    }


}
