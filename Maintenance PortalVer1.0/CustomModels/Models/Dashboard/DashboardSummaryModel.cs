using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Dashboard
{
    public class DashboardSummaryModel
    {
        public string TargetAchieved { get; set; }

        public List<DashboardTileModel> Tiles { get; set; }

        public List<SelectListItem> DistrictList { get; set; }

        public int LevelId { get; set; }

        public RevenueCollectionWrapperModel _RevenueCollectionWrapperModel { get; set; }
        public CurrentAchievementsModel CurrentAchievementsModel { get; set; }

        public ProgressBarTargetVsAchieved _ProgressBarTargetVsAchieved { get; set; }

        public ProgressBarTargetVsAchieved progressBarTargetVsAchieved { get; set; }
        public string StartTimeIndicationTop { get; set; }
        public string StartTimeIndicationBottom { get; set; }
        public ProgressBarTargetVsAchieved ProgressBarTopModel { get; set; }

        //Added By Raman Kalegaonkar on 17-06-2020
        [Display(Name = "Fin. Year")]
        public List<SelectListItem> FinYearList { get; set; }
        public int FinYearId { get; set; }
        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        public String ReportInfo { get; set; }
        public string Top3AvgRegTime { get; set; }
        public string Bottom3AvgRegTime { get; set; }
        public string AVG_REGISTRASTION_TIME_FYWISE { get; set; }
    }
}
