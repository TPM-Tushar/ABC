#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ICCConversionLog.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15-09-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for CC Conversion Logs.
*/
#endregion

using CustomModels.Models.Remittance.CCConversionLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface ICCConversionLog
    {
        CCConversionLogWrapperModel CCConversionLogView();

        CCConversionLogWrapperModel CCConversionLogDetails(CCConversionLogReqModel model);

        int GetCCConversionLogDetailsTotalCount(CCConversionLogReqModel model);

    }
}
