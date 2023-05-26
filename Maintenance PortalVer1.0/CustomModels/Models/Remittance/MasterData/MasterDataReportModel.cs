using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.MasterData
{

    public class MasterDataReportTableModel
    {


         
        public int SrNo { get; set; }
        public long VillageCode { get; set; }

        public int SROCode { get; set; }
        public Nullable<int> HobliCode { get; set; }
        public string CensusCode { get; set; }
        public Nullable<int> TalukCode { get; set; }
        public string VillageNameK { get; set; }
        public string VillageNameE { get; set; }
        public bool IsUrban { get; set; }
        public Nullable<int> BhoomiTalukCode { get; set; }
        public Nullable<int> BhoomiVillageCode { get; set; }
        public string BhoomiVillageName { get; set; }
        public Nullable<int> BhoomiDistrictCode { get; set; }
        
        public string HobliNameK { get; set; }
        public string HobliNameE { get; set; }
        public string ShortNameK { get; set; }
        public string ShortNameE { get; set; } 
        public Nullable<int> BhoomiHobliCode { get; set; }
        public string BhoomiHobliName { get; set; }
        public string BhoomiTalukName { get; set; }
        //District Master
        public Nullable<int> DistrictCode { get; set; }
        public string DistrictNameE { get; set; }
        public string DistrictNameK { get; set; }
        public Nullable<int> DIGCode { get; set; }
        //VillageMastervillagesMergingMapping
        public Int64 ID { get; set; }
        public long  MergedVillageCode { get; set; }
        //bhoomiMapping
        public double BhoomiMappingID { get; set; }
        public Nullable<double> KaveriSROCode { get; set; }
        public string KaveriSROName { get; set; }
        public Nullable<double> KaveriVillageCode { get; set; }
        public string KaveriVillageName { get; set; }
        public Nullable<double> KaveriHobiCode { get; set; }
        public string KaveriHobiName { get; set; }
        public Nullable<double> BhoomiHobiCode { get; set; }
        public string BhoomiHobiName { get; set; }
        public bool IsUpdated { get; set; }
        //SROmASTER
        public string SRONameK { get; set; }
        public string SRONameE { get; set; }
        public string ShortnameK { get; set; }
        public bool GetBhoomiData { get; set; }
        public Nullable<bool> IsVillageMatching { get; set; }
        public MasterDataReportTableModel()
        {
            ShortNameE = "";
        }

        #region Added by vijay on 19-01-2023
        #endregion


        public short OfficeID { get; set; }
        public short OfficeTypeID { get; set; }
        public string OfficeName { get; set; }
        public string OfficeNameR { get; set; }
        public string ShortName { get; set; }
        public string ShortNameR { get; set; }
        public Nullable<short> DistrictID { get; set; }
        public Nullable<short> ParentOfficeID { get; set; }
        public Nullable<short> KaveriCode { get; set; }
        public Nullable<short> BhoomiCensusCode { get; set; }
        public Nullable<bool> AnyWhereRegEnabled { get; set; }
        public string OfficeAddress { get; set; }
        public string Landline { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> OnlineBookingEnabled { get; set; }

        #region Added by vijay on 20-01-2023
        #endregion

        public long VillageID { get; set; }
        public string VillageName { get; set; }
        public Nullable<int> UPORTownID { get; set; }

        public string VillageNameR { get; set; }
        public Nullable<int> HobliID { get; set; }
        public short TalukaID { get; set; }



        #region Added by vijay on 20-01-2023 for MAS_Hoblis
        public string HobliName { get; set; }
        public string HobliNameR { get; set; }
        #endregion

    }


    public class MasterDataResultModel
    {
        public List<MasterDataReportTableModel> MasterDataTableList;


    }
    public class MasterDataReportModel
    {
        public List<SelectListItem> TableName { get; set; }
        public List<SelectListItem> TableName_KG { get; set; }
        public int TableId { get; set; }
        public int DBID { get; set; }
        public List<SelectListItem> DBName { get; set; }
    }


}


