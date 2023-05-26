using CustomModels.Models.PhotoThumb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.PhotoThumb.Interface
{
    public interface IPhotoThumb
    {

        PhotoThumbViewModel PhotoThumbView(int officeID);
        PhotoThumbTableModel PhotoThumbAvailaibility(int SROCode, long DocumentNumber, int BookTypeID,string fyear);
    }
}
