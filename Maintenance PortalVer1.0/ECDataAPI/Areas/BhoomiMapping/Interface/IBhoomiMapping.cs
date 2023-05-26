
using CustomModels.Models.BhoomiMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.BhoomiMapping.Interface
{
    public interface IBhoomiMapping
    {

        BhoomiMappingViewModel BhoomiMappingView(int officeID, int LevelID, long UserID);
        string Upload(BhoomiMappingUpdateModel model);
    }
}
