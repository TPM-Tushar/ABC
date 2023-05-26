using CustomModels.Models.PhotoThumb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.PhotoThumb.Interface
{
    public interface IPhotoThumbFailed
    {

        PhotoThumbFailedViewModel PhotoThumbFailedView(int officeID);
        PhotoThumbFailedTableModel PhotoThumbFailed(int SROCode);
        PhotoThumbFailedTableModel PhotoThumbFailedDetail(long PartyID, int SROCode, bool isPhoto, bool IsThumb);
    }
}
