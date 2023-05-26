
using CustomModels.Models.BhoomiMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.BhoomiMapping.Interface
{
    public interface IIsVillageMatching
    {

        List<IsVillageMatchingViewTableModel> IsVillageMatchingView(int officeID, int LevelID, long UserID);
    }
}
