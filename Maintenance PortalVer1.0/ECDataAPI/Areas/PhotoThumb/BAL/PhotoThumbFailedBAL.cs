
using CustomModels.Models.PhotoThumb;
using ECDataAPI.Areas.PhotoThumb.DAL;
using ECDataAPI.Areas.PhotoThumb.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.PhotoThumb.BAL
{
    public class PhotoThumbFailedBAL : IPhotoThumbFailed
    {
        IPhotoThumbFailed PhotoThumbFailedDAL = new PhotoThumbFailedDAL();

        public PhotoThumbFailedViewModel PhotoThumbFailedView(int officeID)
        {
            return PhotoThumbFailedDAL.PhotoThumbFailedView(officeID);
        }


        public PhotoThumbFailedTableModel PhotoThumbFailed(int SROCode)
        {
            return PhotoThumbFailedDAL.PhotoThumbFailed(SROCode);
        }



        public PhotoThumbFailedTableModel PhotoThumbFailedDetail(long PartyID, int SROCode, bool IsPhoto, bool IsThumb)
        {
            return PhotoThumbFailedDAL.PhotoThumbFailedDetail(PartyID, SROCode, IsPhoto, IsThumb);
        }

    }
}