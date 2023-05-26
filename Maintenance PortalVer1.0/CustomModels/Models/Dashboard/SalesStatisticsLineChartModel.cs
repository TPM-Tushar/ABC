using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class SalesStatisticsLineChartModel
    {
        public String[] NonAgriLessThanTenLakhs_DT_CLM { get; set; }
        public String[] AgriLessThanTenLakhs_DT_CLM { get; set; }
        public String[] FaltsApartments_DT_CLM { get; set; }
        public String[] NonAgriGreaterThanTenLakhs_DT_CLM { get; set; }
        public String[] AgriGreaterThanTenLakhs_DT_CLM { get; set; }
        public String[] Lease_DT_CLM { get; set; }
        public String[] FinYear_DT_CLM { get; set; }

        public decimal[] NonAgriLessThanTenLakhs { get; set; }
        public decimal[] AgriLessThanTenLakhs { get; set; }
        public decimal[] FaltsApartments { get; set; }
        public decimal[] NonAgriGreaterThanTenLakhs { get; set; }
        public decimal[] AgriGreaterThanTenLakhs { get; set; }
        public decimal[] Lease { get; set; }
        public String[] FinYear { get; set; }

        public string Lbl_NonAgriLessThanTenLakhs { get; set; }
        public string Lbl_AgriLessThanTenLakhs { get; set; }
        public string Lbl_FaltsApartments { get; set; }
        public string Lbl_Lease { get; set; }
        public string Lbl_NonAgriGreaterThanTenLakhs { get; set; }
        public string Lbl_AgriGreaterThanTenLakhs { get; set; }


    }
    public class OneRow
    {
        public List<int> RowList { get; set; }

    }
}
