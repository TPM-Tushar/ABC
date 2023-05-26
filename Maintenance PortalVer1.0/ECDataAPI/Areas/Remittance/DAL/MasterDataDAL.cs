using CustomModels.Models.Remittance.MasterData;
using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataAPI.Common;
using ECDataAPI.Entity.KAIGR_ONLINE;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class MasterDataDAL
    {
        MasterDataReportTableModel resModel = new MasterDataReportTableModel();

        MasterDataResultModel ResultModel = new MasterDataResultModel();
        MasterDataReportModel reportModel = new MasterDataReportModel();


        public List<MasterDataReportTableModel> VillageMasterTableList;

        public MasterDataReportModel MasterDataReportView()
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();

            var GetTableNameList = objCommon.GetTableName();

            var GetTable_KGNameList = objCommon.GetTableName_KG();
            var GetDBNameList = objCommon.GetDBName();
            reportModel.TableName = GetTableNameList;
            reportModel.TableName_KG = GetTable_KGNameList;
            reportModel.DBName = GetDBNameList;
            return reportModel;
        }


        public MasterDataResultModel GetMasterData(MasterDataReportModel masterDataReportModel)

        {
            MasterDataReportTableModel resultModel = new MasterDataReportTableModel();
            KaveriEntities dbContext_EC = null;
            KAIGR_ONLINEEntities dbContext_KG = null;
            MasterDataReportTableModel masterDataReportTableModel = new MasterDataReportTableModel();
            //  List<MasterDataReportTableModel> MasterDataTableList;

            ResultModel.MasterDataTableList = new List<MasterDataReportTableModel>();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();

            var GetTableNameList = objCommon.GetTableName();

            try

            {
                dbContext_EC = new KaveriEntities();

                dbContext_KG = new KAIGR_ONLINEEntities();

                switch (masterDataReportModel.TableId)
                {
                    case 1:
                        GetVillageMasterDetails();
                        break;
                    case 2:
                        GetHobliMasterDetails();
                        break;
                    case 3:
                        GetBhoomiMappingDetails();
                        break;
                    case 4:
                        GetDistrictMasterDetails();
                        break;
                    case 5:
                        GetVMasterVillagesMergingMappingDetails();
                        break;
                    case 6:
                        GetSROMasterDetails();
                        break;
                    case 7:
                        GetMAS_OfficeMasterDetails();
                        break;
                    case 8:
                        GetMAS_Villages();
                        break;

                    case 9:
                        GetMAS_Hoblis();
                        break;



                }



                void GetVillageMasterDetails()
                {
                    if (masterDataReportModel.TableId == 1)
                    {
                        var Result = dbContext_EC.VillageMaster.OrderBy(x => x.VillageCode);

                        int count = 0;
                        foreach (var item in Result)
                        {

                            masterDataReportTableModel = new MasterDataReportTableModel();
                            count++;
                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.VillageCode = item.VillageCode;
                            masterDataReportTableModel.SROCode = item.SROCode;
                            masterDataReportTableModel.HobliCode = item.HobliCode;
                            masterDataReportTableModel.CensusCode = item.CensusCode;
                            masterDataReportTableModel.TalukCode = item.TalukCode;
                            masterDataReportTableModel.IsUrban = item.IsUrban;
                            masterDataReportTableModel.BhoomiTalukCode = item.BhoomiTalukCode;
                            masterDataReportTableModel.VillageNameE = item.VillageNameE;
                            masterDataReportTableModel.VillageNameK = item.VillageNameK;
                            masterDataReportTableModel.BhoomiVillageCode = item.BhoomiVillageCode;
                            masterDataReportTableModel.BhoomiVillageName = item.BhoomiVillageName;
                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }
                }
                void GetHobliMasterDetails()
                {
                    if (masterDataReportModel.TableId == 2)
                    {
                        var Result = dbContext_EC.HobliMaster.ToList();

                        int count = 0;
                        foreach (var item in Result)
                        {

                            count++;
                            masterDataReportTableModel = new MasterDataReportTableModel();

                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.HobliCode = (int)item.HobliCode;
                            masterDataReportTableModel.TalukCode = (int?)item.TalukCode;
                            masterDataReportTableModel.HobliNameK = item.HobliNameK;
                            masterDataReportTableModel.HobliNameE = item.HobliNameE;
                            masterDataReportTableModel.ShortNameK = item.ShortNameK.ToString();
                            masterDataReportTableModel.BhoomiHobliCode = item.BhoomiHobliCode;
                            masterDataReportTableModel.BhoomiHobliName = item.BhoomiHobliName;

                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }

                }
                void GetBhoomiMappingDetails()
                {
                    if (masterDataReportModel.TableId == 3)
                    {

                        var Result = dbContext_EC.BhoomiMappingDetails.ToList();

                        int count = 0;
                        foreach (var item in Result)
                        {
                            count++;
                            masterDataReportTableModel = new MasterDataReportTableModel();

                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.BhoomiMappingID = item.BhoomiMappingID;
                            masterDataReportTableModel.DistrictCode = (int?)item.DistrictCode;
                            masterDataReportTableModel.DistrictNameE = item.DistrictNameE;
                            masterDataReportTableModel.KaveriSROCode = item.KaveriSROCode;
                            masterDataReportTableModel.KaveriSROName = item.KaveriSROName;
                            masterDataReportTableModel.KaveriSROName = item.KaveriSROName;
                            masterDataReportTableModel.KaveriVillageCode = item.KaveriVillageCode;
                            masterDataReportTableModel.KaveriVillageName = item.KaveriVillageName;
                            masterDataReportTableModel.KaveriHobiCode = item.KaveriHobiCode;
                            masterDataReportTableModel.KaveriHobiName = item.KaveriHobiName;
                            masterDataReportTableModel.BhoomiDistrictCode = (int?)item.BhoomiDistrictCode;
                            masterDataReportTableModel.BhoomiTalukCode = (int?)item.BhoomiTalukCode;
                            masterDataReportTableModel.BhoomiTalukName = item.BhoomiTalukName;
                            //changed on 06-02-2023
                            masterDataReportTableModel.BhoomiHobiCode = (int?)item.BhoomiHobiCode;
                            masterDataReportTableModel.BhoomiHobiName = item.BhoomiHobiName;
                            masterDataReportTableModel.BhoomiVillageCode = (int?)item.BhoomiVillageCode;
                            masterDataReportTableModel.BhoomiVillageName = item.BhoomiVillageName;
                            masterDataReportTableModel.IsUpdated = item.IsUpdated;












                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }

                }

                void GetDistrictMasterDetails()
                {
                    if (masterDataReportModel.TableId == 4)
                    {
                        var Result = dbContext_EC.DistrictMaster.ToList();

                        int count = 0;
                        foreach (var item in Result)
                        {

                            count++;
                            masterDataReportTableModel = new MasterDataReportTableModel();

                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.DistrictCode = item.DistrictCode;
                            masterDataReportTableModel.DistrictNameK = item.DistrictNameK;
                            masterDataReportTableModel.DistrictNameE = item.DistrictNameE;
                            masterDataReportTableModel.ShortNameK = item.ShortNameK;
                            masterDataReportTableModel.ShortNameE = item.ShortNameE;
                            masterDataReportTableModel.DIGCode = item.DIGCode;
                            masterDataReportTableModel.BhoomiDistrictCode = item.BhoomiDistrictCode;


                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }

                }
                void GetVMasterVillagesMergingMappingDetails()
                {
                    if (masterDataReportModel.TableId == 5)
                    {

                        var Result = dbContext_EC.VillageMasterVillagesMergingMapping.ToList();

                        int count = 0;
                        foreach (var item in Result)
                        {
                            count++;
                            masterDataReportTableModel = new MasterDataReportTableModel();

                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.ID = item.ID;
                            masterDataReportTableModel.SROCode = item.SROCode;
                            masterDataReportTableModel.VillageCode = item.VillageCode;
                            masterDataReportTableModel.MergedVillageCode = item.MergedVillageCode;



                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }

                }
                void GetSROMasterDetails()
                {
                    if (masterDataReportModel.TableId == 6)
                    {

                        var Result = dbContext_EC.SROMaster.ToList();

                        int count = 0;
                        foreach (var item in Result)
                        {
                            count++;
                            masterDataReportTableModel = new MasterDataReportTableModel();

                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.SROCode = item.SROCode;
                            masterDataReportTableModel.DistrictCode = item.DistrictCode;
                            masterDataReportTableModel.SRONameK = item.SRONameK;
                            masterDataReportTableModel.SRONameE = item.SRONameE;
                            masterDataReportTableModel.ShortNameK = item.ShortnameK;
                            masterDataReportTableModel.ShortNameE = item.ShortNameE;
                            masterDataReportTableModel.GetBhoomiData = item.GetBhoomiData;
                            masterDataReportTableModel.IsVillageMatching = item.IsVillageMatching;







                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }

                }

                #region Added by vijay on 19-01-2023 for MAS_OfficeMaster
                void GetMAS_OfficeMasterDetails()
                {
                    if (masterDataReportModel.TableId == 7)
                    {
                        var Result = dbContext_KG.MAS_OfficeMaster;

                        int count = 0;
                        foreach (var item in Result)
                        {

                            masterDataReportTableModel = new MasterDataReportTableModel();
                            count++;
                            masterDataReportTableModel.SrNo = count;
                            masterDataReportTableModel.OfficeID = item.OfficeID;
                            masterDataReportTableModel.OfficeTypeID = item.OfficeTypeID;
                            masterDataReportTableModel.OfficeName = item.OfficeName;
                            masterDataReportTableModel.OfficeNameR = item.OfficeNameR;
                            masterDataReportTableModel.ShortName = item.ShortName;
                            masterDataReportTableModel.ShortNameR = item.ShortNameR;
                            masterDataReportTableModel.DistrictID = item.DistrictID;
                            masterDataReportTableModel.ParentOfficeID = item.ParentOfficeID;
                            masterDataReportTableModel.KaveriCode = (short?)item.KaveriCode;
                            masterDataReportTableModel.BhoomiCensusCode = item.BhoomiCensusCode;
                            masterDataReportTableModel.AnyWhereRegEnabled = item.AnyWhereRegEnabled;
                            masterDataReportTableModel.OfficeAddress = item.OfficeAddress;
                            masterDataReportTableModel.Landline = item.Landline;
                            masterDataReportTableModel.Mobile = item.Mobile;
                            masterDataReportTableModel.OnlineBookingEnabled = item.OnlineBookingEnabled;
                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);
                        }

                    }
                }

                #endregion


                #region Added by vijay on 20-01-2023 for MAS_Villages

                void GetMAS_Villages()
                {
                    if (masterDataReportModel.TableId == 8)
                    {
                        var Result = dbContext_KG.MAS_Villages;

                        int count = 0;
                        foreach (var item in Result)
                        {

                            masterDataReportTableModel = new MasterDataReportTableModel();
                            count++;
                            masterDataReportTableModel.SrNo = count;

                            masterDataReportTableModel.VillageID = item.VillageID;
                            masterDataReportTableModel.OfficeID = item.OfficeID;
                            masterDataReportTableModel.HobliID = item.HobliID;
                            masterDataReportTableModel.TalukaID = item.TalukaID;
                            masterDataReportTableModel.VillageName = item.VillageName;
                            masterDataReportTableModel.VillageNameR = item.VillageNameR;
                            masterDataReportTableModel.IsUrban = item.IsUrban;
                            masterDataReportTableModel.CensusCode = item.CensusCode;
                            masterDataReportTableModel.BhoomiTalukCode = item.BhoomiTalukCode;
                            masterDataReportTableModel.BhoomiVillageCode = item.BhoomiVillageCode;
                            masterDataReportTableModel.BhoomiVillageName = item.BhoomiVillageName;
                            masterDataReportTableModel.BhoomiDistrictCode = item.BhoomiDistrictCode;
                            masterDataReportTableModel.UPORTownID = item.UPORTownID;

                            ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                        }

                    }
                }
                #endregion


                #region Added by vijay on 20-01-2023 for MAS_Hoblis

                void GetMAS_Hoblis()
                {

                    var Result = dbContext_KG.MAS_Hoblis;

                    int count = 0;
                    foreach (var item in Result)
                    {
                        masterDataReportTableModel = new MasterDataReportTableModel();
                        count++;
                        masterDataReportTableModel.SrNo = count;
                        masterDataReportTableModel.HobliID = item.HobliID;
                        masterDataReportTableModel.TalukaID = item.TalukaID;
                        masterDataReportTableModel.HobliName = item.HobliName;
                        masterDataReportTableModel.HobliNameR = item.HobliNameR;
                        masterDataReportTableModel.BhoomiHobliName = item.BhoomiHobliName;
                        masterDataReportTableModel.BhoomiTalukCode = item.BhoomiTalukCode;
                        masterDataReportTableModel.BhoomiHobliCode = item.BhoomiHobliCode;
                        masterDataReportTableModel.BhoomiTalukName = item.BhoomiTalukName;
                        masterDataReportTableModel.BhoomiDistrictCode = item.BhoomiDistrictCode;
                        ResultModel.MasterDataTableList.Add(masterDataReportTableModel);


                    }


                }
                #endregion


                return ResultModel;




            }
            catch (Exception e)
            {

                throw e;
            }


        }

    }
}
