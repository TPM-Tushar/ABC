using CustomModels.Models.DisableKaveri;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ECDataAPI.Areas.DisableKaveri.DAL
{
    public class DisableKaveriDAL
    {
        public DisableKaveriViewModel DisableKaveriView()
        {
           try
            {
                int srNo = 1;
                DisableKaveriViewModel resModel = new DisableKaveriViewModel();
                resModel.disableKaveriViewsList = new List<DisableKaveriViewDetails>();
                int OfficeTypes = Convert.ToInt32(Common.ApiCommonEnum.OfficeTypes.SRO);
                using (KaveriEntities dbContext = new KaveriEntities())
                {
                    var ResultList = dbContext.MAS_OfficeMaster_DisableInfo.Where(s => s.OfficeTypeID == OfficeTypes)
                        .Select(x => new
                        {
                            OfficeName = x.OfficeName,
                            IsDisabled = x.IsDisabled,
                            DisableDate = x.DisableDate,
                            Kaveri1Code =x.Kaveri1Code
                        }).OrderBy(x=>x.DisableDate).ToList();


                    var Result = ResultList.Select(x => new DisableKaveriViewDetails
                    { SrNo = srNo++,
                        OfficeName = x.OfficeName,
                        IsDisabled = x.IsDisabled != null ? Convert.ToBoolean(x.IsDisabled)?"YES":"NO" : "NO",
                        //DisableDate =x.DisableDate!= null ? Convert.ToDateTime(x.DisableDate).ToString("dd/MM/yyyy hh:mm:ss") : "--",
                        DisableDate = x.DisableDate != null ? Convert.ToDateTime(x.DisableDate).ToString("dd/MM/yyyy hh:mm:ss") : "--",
                        Kaveri1Code = x.Kaveri1Code
                    }).ToList();

                    resModel.disableKaveriViewsList.AddRange(Result);
                }
                return resModel;

            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public UpdateDetailsModel UpdateDisableKaveriDetails(DisableKaveriViewModel disableKaveriViewModel)
        {
            UpdateDetailsModel updateDetailsModel = new UpdateDetailsModel();
            KaveriEntities dbContext = new KaveriEntities();
            try
            {
               
                int OfficeTypes = Convert.ToInt32(Common.ApiCommonEnum.OfficeTypes.SRO);
               // string result = String.Join(",", disableKaveriViewModel.KaveriCode);

                List<int> kaveriList = disableKaveriViewModel.KaveriCode.ToList();
                List<MAS_OfficeMaster_DisableInfo> ResultDisableInfo = new List<MAS_OfficeMaster_DisableInfo>();
                List<AppVersion> appVersionKaveriReg = new List<AppVersion>();
                List<AppVersion> appVersionUpdatePatch = new List<AppVersion>();
                var KaveriDisableDate = DateTime.Now;
                
                   
                        ResultDisableInfo = dbContext.MAS_OfficeMaster_DisableInfo
                            .Where(x => x.OfficeTypeID == OfficeTypes && kaveriList.Contains(x.Kaveri1Code))

                            .ToList();
                    //
                    appVersionKaveriReg = dbContext.AppVersion.Where(x => kaveriList.Contains(x.SROCode) && x.AppName == "Kaveri Registration").ToList();

                    appVersionUpdatePatch = dbContext.AppVersion.Where(x => kaveriList.Contains(x.SROCode) && x.AppName == "Update Patch").ToList();
                //
                var maxRowId = dbContext.Kaveri2Pilot.Max(k => k.RowID);

                if (ResultDisableInfo != null && ResultDisableInfo.Count > 0)
                        {

                            foreach (var item in ResultDisableInfo)
                            {
                                item.IsDisabled = true;
                                item.DisableDate = KaveriDisableDate;

                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }

                           
                            updateDetailsModel.IsKaveriUpdate = true;

                        }
                        else
                        {
                            updateDetailsModel.IsKaveriUpdate = false;
                        }
                    if(appVersionKaveriReg != null && appVersionKaveriReg.Count > 0)
                    {
                        foreach (var item in appVersionKaveriReg)
                        {
                            item.AppMajor = 20;
                            item.AppMinor = 0;
                            item.ReleaseDate = KaveriDisableDate.AddDays(-1); ;
                            item.LastDateForPatchUpdate = KaveriDisableDate.AddDays(-1);
                           
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    if(appVersionUpdatePatch != null && appVersionUpdatePatch.Count >0)
                    {
                        foreach (var item in appVersionUpdatePatch)
                        {
                            item.AppMajor = 99;
                            item.AppMinor = 0;
                        item.ReleaseDate = KaveriDisableDate.AddDays(-1);
                        item.LastDateForPatchUpdate = KaveriDisableDate.AddDays(-1);

                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                foreach (var item in kaveriList)
                {
                    var newKaveri2Pilot = new Kaveri2Pilot
                    {
                        RowID = maxRowId + 1,
                        SROCode = item,
                        PilotDate = KaveriDisableDate,
                        EnteredBy = "DBA",
                        IsValid = 1
                    };
                    dbContext.Kaveri2Pilot.Add(newKaveri2Pilot);
                    maxRowId++;
                    //dbContext.Entry(newKaveri2Pilot).State = System.Data.Entity.EntityState.Added;
                }



                using (TransactionScope TranScope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                     dbContext.SaveChanges();
                    TranScope.Complete();
                }
                return updateDetailsModel;
            }
            catch(Exception ex)
            {
                throw;
                //return null;
            }finally
            {
                if(dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //Added By Tushar on 5 apr 2023
        public MenuDisabledOfficeIDModel GetMenuDisabledOfficeID(MenuDisabledOfficeIDModel menuDisabledOfficeIDModel)
        {
            try
            {
                MenuDisabledOfficeIDModel officeIDModel = new MenuDisabledOfficeIDModel();
                var ResultList = new List<MenuDisabledOfficeIDModel>();
                int OfficeTypes = Convert.ToInt32(Common.ApiCommonEnum.OfficeTypes.SRO);
                using (KaveriEntities dbContext = new KaveriEntities())
                {
                    var result = dbContext.MAS_OfficeMaster_DisableInfo.Where(x => x.IsDisabled == true && x.DisableDate != null && x.OfficeTypeID == OfficeTypes)
                        .Select(s => s.OfficeID).ToList();

                    string menuDisabled = null;

                    foreach (var i in result)
                    {

                        menuDisabled = menuDisabled + i + ",";
                    }
                   var menuDisabledResult = menuDisabled.Substring(0, menuDisabled.Length - 1);
                    officeIDModel.DisabledOfficeID = menuDisabledResult;
                }
                return officeIDModel;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        //End By Tushar on 5 apr 2023
    }
}