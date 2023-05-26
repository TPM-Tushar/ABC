using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class TableDataWrapper
    {
        public TableData[] TableDataArray { get; set; }
        public TableDataProgressChart[] TableDataArrayofProgressChart { get; set; }
        public TableDataRevTargetVsAchieved[] tableDataRevTargetVsAchieved { get; set; }
        public TableDataSurchargeCess[] tableDataSurchargeAndCess { get; set; }
        public TableDataSalesStatisticsDocReg[] tableSalesStatisticsDocReg { get; set; }
        public ColumnArray[] ColumnArray { get; set; }

        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020        
        public TableData_HIGH_VALUE_REVENUE_COLLECTED[] TableData_HIGH_VALUE_REVENUE_COLLECTED { get; set; }

    }
    public class TableDataSalesStatisticsDocReg
    {
        public string NonAgriGreaterThan10Lakhs { get; set; }
        public string NonAgriLessThan10Lakhs { get; set; }
        public string AgriGreaterThan10Lakhs { get; set; }
        public string AgriLessThan10Lakhs { get; set; }
        public string FlatsApartment { get; set; }
        public string Lease { get; set; }
        public int SRNo { get; set; }
        public string FinYear { get; set; }

    }
    public class ColumnArray
    {
        public string title { get; set; } // represents Label at table header
        public string data { get; set; } // represents property name which you want to bind a corresponding label

    }

    public class TableData
    {
        public int SrNo { get; set; }
        public string Duration { get; set; }
      
    }

    public class TableDataProgressChart
    {
        public int REGFYEAR { get; set; }
        public string NO_OF_DOCS_REGISTERED { get; set; }
        public string TOTAL_REVENUE { get; set; }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
        public string MonthName { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020

    }

    public class TableDataRevTargetVsAchieved
    {
        public String Fin_Years { get; set; }
        public string RevTarget { get; set; }
        public string RevAchieved { get; set; }


    }
    public class TableDataSurchargeCess
    {
        public String Fin_Years { get; set; }
        public String Surcharge { get; set; }
        public String Cess { get; set; }
        public String Total { get; set; }

        public int SRNo { get; set; }


    }

    //ADDED BY SHUBHAM BHAGAT ON 1-4-2020   

    public class TableData_HIGH_VALUE_REVENUE_COLLECTED
    {
        public String OneLakhToTenLakhs { get; set; }
        public String TenLakhsToOneCrore { get; set; }
        public String OneCroreToFiveCrore { get; set; }
        public String FiveCroreToTenCrore { get; set; }
        public String AboveTenCrore { get; set; }
        public int SRNo { get; set; }
        public string FinYear { get; set; }

    }
}
