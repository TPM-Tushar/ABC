
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
    public class PhotoThumbBAL : IPhotoThumb
    {
        IPhotoThumb PhotoThumbDAL = new PhotoThumbDAL();

        public PhotoThumbViewModel PhotoThumbView(int officeID)
        {
            return PhotoThumbDAL.PhotoThumbView(officeID);
        }


        public PhotoThumbTableModel PhotoThumbAvailaibility(int SROCode, long DocumentNumber, int BookTypeID, string fyear)
        {
            return PhotoThumbDAL.PhotoThumbAvailaibility(SROCode, DocumentNumber, BookTypeID, fyear);
        }

    }
}