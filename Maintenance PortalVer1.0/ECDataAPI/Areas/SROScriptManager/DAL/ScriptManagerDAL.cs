using CustomModels.Models.SROScriptManager;
using ECDataAPI.Areas.SROScriptManager.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SROScriptManager.DAL
{
    public class ScriptManagerDAL : IScriptManager
    {
        public SROScriptManagerModel InsertInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    ScriptManager _dbScriptManager = new ScriptManager();
                    _dbScriptManager.Script = viewModel.ScriptContent;
                    _dbScriptManager.IsActive = viewModel.IsActive;
                    _dbScriptManager.Description = viewModel.ScriptDescription;
                    _dbScriptManager.ServicePack = viewModel.ServicePackNumber;
                    _dbScriptManager.DateOfScript = DateTime.Now;
                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            _dbScriptManager.ID = context.ScriptManager.Any() ? context.ScriptManager.Select(x => x.ID).Max() + 1 : 1;
                            context.ScriptManager.Add(_dbScriptManager);
                            context.SaveChanges();
                            tScope.Complete();

                            viewModel.IsInsertedSuccessfully = true;
                            viewModel.ResponseMessage = "Script added successfully to the Script Manager.";
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Add Script to the Script Manager.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while adding script to the Script Manager. Error :: " + ex.GetBaseException().Message;
            }

            return viewModel;
        }

        public ScriptManagerDetailWrapperModel LoadScriptManagerTable(SROScriptManagerModel viewModel)
        {
            try
            {
                ScriptManagerDetailWrapperModel resModel = new ScriptManagerDetailWrapperModel();
                resModel.ScriptManagerDetailList = new List<ScriptManagerDetailModel>();

                List<ScriptManager> _dbScriptManager = null;

                using (KaveriEntities context = new KaveriEntities())
                {
                    _dbScriptManager = context.ScriptManager.Where(x => x.ServicePack == viewModel.ServicePackNumber).ToList();

                    if (_dbScriptManager != null)
                        _dbScriptManager = _dbScriptManager.OrderByDescending(x => x.DateOfScript).ToList();

                    //_dbScriptManager = context.ScriptManager.Where(x => x.ServicePack == viewModel.ServicePackNumber).ToList();
                    resModel.TotalCount = _dbScriptManager.Count;

                    //Added by Omkar on 13-08-2020
                    if (viewModel.IsForSearch)
                    {
                        _dbScriptManager = _dbScriptManager.ToList();
                    }
                    else
                    {
                        _dbScriptManager = _dbScriptManager.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                    }

                    // _dbScriptManager = _dbScriptManager.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                }

                if (_dbScriptManager != null)
                {
                    int SrNo = 1;
                    ScriptManagerDetailModel objModel = null;
                    foreach (var item in _dbScriptManager)
                    {
                        objModel = new ScriptManagerDetailModel();
                        objModel.SerialNo = SrNo.ToString();
                        objModel.ID = item.ID.ToString();
                        objModel.Script = item.Script;
                        objModel.ServicePack = item.ServicePack;
                        objModel.IsActive = item.IsActive == null ? false : Convert.ToBoolean(item.IsActive);
                        objModel.Description = item.Description;
                        objModel.DateOfScript = item.DateOfScript.ToString();
                        //objModel.Action = @"<span class='fa fa-pencil' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditSROScriptManagerDetails('" + (item.ID) + "')></span>";
                        objModel.Action = "<a href='#'  onclick= EditSROScriptManagerDetails('" + (item.ID) + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                        SrNo++;
                        resModel.ScriptManagerDetailList.Add(objModel);
                    }
                }
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SROScriptManagerModel GetScriptDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                ScriptManager _dbScriptManager = new ScriptManager();
                using (KaveriEntities context = new KaveriEntities())
                {
                    _dbScriptManager = context.ScriptManager.Where(x => x.ID == viewModel.ScriptID).FirstOrDefault();
                }

                if (_dbScriptManager != null)
                {
                    viewModel.ScriptID = _dbScriptManager.ID;
                    viewModel.ScriptContent = _dbScriptManager.Script;
                    viewModel.IsActive = _dbScriptManager.IsActive == null ? false : (bool)_dbScriptManager.IsActive;
                    viewModel.ScriptDescription = _dbScriptManager.Description;
                    viewModel.ServicePackNumber = _dbScriptManager.ServicePack;
                    //_dbScriptManager.DateOfScript = _dbScriptManager.DateOfScript;

                }
                else
                {
                    viewModel.ErrorMessage = "Failed to Get details for selected Script from the Script Manager.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SROScriptManagerModel UpdateInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            ScriptManager _dbScriptManager = context.ScriptManager.Where(x => x.ID == viewModel.ScriptID).FirstOrDefault();

                            if (_dbScriptManager != null)
                            {
                                if (!string.IsNullOrEmpty(viewModel.ScriptContent))
                                    _dbScriptManager.Script = viewModel.ScriptContent;
                                _dbScriptManager.IsActive = viewModel.IsActive;
                                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                                    _dbScriptManager.Description = viewModel.ScriptDescription;
                                if (!string.IsNullOrEmpty(viewModel.ServicePackNumber))
                                    _dbScriptManager.ServicePack = viewModel.ServicePackNumber;
                                _dbScriptManager.DateOfScript = DateTime.Now;
                                context.Entry(_dbScriptManager).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                tScope.Complete();
                                viewModel.IsInsertedSuccessfully = true;
                                viewModel.ResponseMessage = "Script Updated successfully.";
                            }
                            else
                            {
                                viewModel.IsInsertedSuccessfully = false;
                                viewModel.ErrorMessage = "No Details found for the Selected Script Details.";
                            }
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Update Script.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while Updating script to the Script Manager. Error :: " + ex.GetBaseException().Message;
            }

            return viewModel;
        }

        public AppVersionDetailsModel AppVersionDetails(int OfficeID)
        {
            AppVersionDetailsModel ViewModel = new AppVersionDetailsModel();
            KaveriEntities dbContext = new KaveriEntities();
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                string FirstRecord = "Select";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                ViewModel.SROfficeList = new List<SelectListItem>();
                ViewModel.DROfficeList = new List<SelectListItem>();
                var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID)
                                        select new
                                        {
                                            OfficeMaster.LevelID,
                                            OfficeMaster.Kaveri1Code
                                        }).FirstOrDefault();


                if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);
                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, mas_OfficeMaster.Kaveri1Code.ToString());
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList.Add(sroNameItem);
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    ViewModel.DROfficeList.Add(droNameItem);
                    ViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, FirstRecord);
                }
                else
                {
                    SelectListItem ItemAll = new SelectListItem();
                    ItemAll.Text = "Select";
                    ItemAll.Value = "0";
                    ViewModel.SROfficeList.Add(ItemAll);
                    ViewModel.DROfficeList = objCommon.GetDROfficesList("Select");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return ViewModel;
        }


        public AppVersionDetailWrapperModel LoadAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                AppVersionDetailWrapperModel resModel = new AppVersionDetailWrapperModel();
                resModel.appVersionDetailList = new List<AppVersionDetaillstModel>();
                List<AppVersionDetaillstModel> dbLst = null;
                using (KaveriEntities context = new KaveriEntities())
                {
                    if (Convert.ToBoolean(viewModel.IsDROOffice))
                    {
                        dbLst = (from AV in context.AppVersion
                                 join DM in context.DistrictMaster
                                 on AV.DRCode equals DM.DistrictCode
                                 where AV.DRCode != null
                                 orderby AV.ReleaseDate descending
                                 select new AppVersionDetaillstModel
                                 {
                                     AppName = AV.AppName,
                                     AppMajor = AV.AppMajor.ToString(),
                                     AppMinor = AV.AppMinor.ToString(),
                                     SROId = "NA",
                                     SROOfficeName = "NA",



                                     ReleaseDate = AV.ReleaseDate.ToString(),
                                     LastDateForPatch = AV.LastDateForPatchUpdate == null ? "NA" : AV.LastDateForPatchUpdate.ToString(),
                                     DROId = AV.SROCode.ToString(),
                                     DROOfficeName = DM.DistrictNameE,
                                     SPExecutionDate = AV.SPExecutionDateTime == null ? "--" : AV.SPExecutionDateTime.ToString(),
                                     IsDROOffice = "YES",
                                     //Action = @"<span class='glyphicon glyphicon-edit' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditAppVersionDetails('" + (AV.AppName) + "'," + (AV.ID) + "," + (AV.DRCode) + ",'Y')></span>"
                                     //Action = @"<span class='fa fa-pencil' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditAppVersionDetails(" + (AV.ID) + "," + (AV.DRCode) + ",'Y')></span>"
                                     Action = "<a href='#'  onclick=EditAppVersionDetails(" + (AV.ID) + "," + (AV.DRCode) + ",'Y'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>"
                                 }).ToList();

                        resModel.TotalCount = dbLst.Count;
                        // Commented by Omkar on 31072020
                        //dbLst = dbLst.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();

                        if (viewModel.IsForSearch)
                        {
                            dbLst = dbLst.ToList();
                        }
                        else
                        {
                            dbLst = dbLst.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                        }

                    }
                    else
                    {
                        dbLst = (from AV in context.AppVersion
                                 join SM in context.SROMaster
                                 on AV.SROCode equals SM.SROCode
                                 where AV.DRCode == null
                                 orderby AV.ReleaseDate descending
                                 select new AppVersionDetaillstModel
                                 {
                                     AppName = AV.AppName,
                                     AppMajor = AV.AppMajor.ToString(),
                                     AppMinor = AV.AppMinor.ToString(),
                                     SROId = AV.SROCode.ToString(),
                                     SROOfficeName = SM.SRONameE,
                                     ReleaseDate = AV.ReleaseDate.ToString(),
                                     LastDateForPatch = AV.LastDateForPatchUpdate == null ? "NA" : AV.LastDateForPatchUpdate.ToString(),
                                     DROId = "NA",
                                     DROOfficeName = "NA",
                                     SPExecutionDate = "NA",
                                     IsDROOffice = "NO",
                                     //Action = @"<span class='glyphicon glyphicon-edit' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditAppVersionDetails('" + (AV.AppName) + "'," + (AV.ID) + "," + (AV.SROCode) + ",'N')></span>"
                                     //Action = @"<span class='fa fa-pencil' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditAppVersionDetails(" + (AV.ID) + "," + (AV.SROCode) + ",'N')></span>"
                                     Action = "<a href='#'  onclick= EditAppVersionDetails(" + (AV.ID) + "," + (AV.SROCode) + ",'N'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>"
                                 }).ToList();

                        resModel.TotalCount = dbLst.Count;
                        //Commented by Omkar on 31072020

                        if (viewModel.IsForSearch)
                        {
                            dbLst = dbLst.ToList();
                        }
                        else
                        {
                            dbLst = dbLst.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                        }

                        // dbLst = dbLst.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                    }
                }

                if (dbLst != null)
                {
                    int SrNo = 1;
                    AppVersionDetaillstModel objModel = null;
                    foreach (var item in dbLst)
                    {
                        objModel = new AppVersionDetaillstModel();
                        objModel.SerialNo = SrNo.ToString();
                        objModel.AppName = item.AppName;
                        objModel.AppMinor = item.AppMinor;
                        objModel.AppMajor = item.AppMajor;
                        objModel.SROId = item.SROId;
                        objModel.SROOfficeName = item.SROOfficeName;
                        objModel.ReleaseDate = item.ReleaseDate;
                        objModel.LastDateForPatch = item.LastDateForPatch;
                        objModel.DROId = item.DROId;
                        objModel.DROOfficeName = item.DROOfficeName;
                        objModel.SPExecutionDate = item.SPExecutionDate;
                        objModel.IsDROOffice = item.IsDROOffice;
                        objModel.Action = item.Action;
                        SrNo++;
                        resModel.appVersionDetailList.Add(objModel);
                    }
                }
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AppVersionDetailsModel AddAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    AppVersion _dbAppVersion = new AppVersion();
                    _dbAppVersion.AppName = viewModel.AppName;
                    _dbAppVersion.AppMajor = viewModel.AppMajor;
                    _dbAppVersion.AppMinor = viewModel.AppMinor;
                    _dbAppVersion.ReleaseDate = viewModel.ReleaseDate;
                    if (Convert.ToBoolean(viewModel.IsDROOffice))
                    {
                        _dbAppVersion.SPExecutionDateTime = viewModel.SPExecutionDateTime;
                        _dbAppVersion.DRCode = viewModel.DROfficeID;
                        _dbAppVersion.IsDROffice = true;
                        // BELOW CODE COMMENTED AND CHANGED BY OMKAR C ON 24-08-2020
                        _dbAppVersion.SROCode = -2;
                        //_dbAppVersion.SROCode = -1;
                        // ABOVE CODE COMMENTED AND CHANGED BY OMKAR C ON 24-08-2020
                    }
                    else
                    {
                        _dbAppVersion.SROCode = viewModel.SROfficeID;
                        _dbAppVersion.LastDateForPatchUpdate = viewModel.LastDateForPatchUpdate;
                        _dbAppVersion.IsDROffice = false;
                    }

                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            _dbAppVersion.ID = context.AppVersion.Any() ? context.AppVersion.Select(x => x.ID).Max() + 1 : 1;
                            context.AppVersion.Add(_dbAppVersion);
                            context.SaveChanges();
                            tScope.Complete();

                            viewModel.IsInsertedSuccessfully = true;
                            viewModel.ResponseMessage = "Application Version Details added successfully added successfully.";
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Add Application Version Details .";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while adding Application Version Details . Error :: " + ex.GetBaseException().Message;
            }

            return viewModel;
        }

        public AppVersionDetailsModel GetAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                int officeID = viewModel.IsDROOffice == true ? viewModel.DROfficeID : viewModel.SROfficeID;
                AppVersion _dbAppVersion = new AppVersion();
                using (KaveriEntities context = new KaveriEntities())
                {
                    if (!Convert.ToBoolean(viewModel.IsDROOffice))
                    {
                        _dbAppVersion = context.AppVersion.Where(x => x.ID == viewModel.VersionID
                                            //&& x.AppName == viewModel.AppName
                                            && x.SROCode == viewModel.SROfficeID
                                            ).FirstOrDefault();
                    }
                    else
                    {
                        _dbAppVersion = context.AppVersion.Where(x => x.ID == viewModel.VersionID
                                            //&& x.AppName == viewModel.AppName
                                            && x.DRCode == viewModel.DROfficeID
                                            // BELOW CODE COMMENTED AND ADDED BY OMKAR ON 20-08-2020 
                                            && x.IsDROffice == true
                                            //&& x.SROCode == -1
                                            // ABOVE CODE COMMENTED AND ADDED BY OMKAR ON 20-08-2020
                                            ).FirstOrDefault();

                    }


                    if (_dbAppVersion != null)
                    {
                        viewModel.VersionID = _dbAppVersion.ID;
                        viewModel.AppName = _dbAppVersion.AppName;
                        viewModel.SROfficeID = _dbAppVersion.SROCode;
                        viewModel.AppMajor = _dbAppVersion.AppMajor;
                        viewModel.AppMinor = _dbAppVersion.AppMinor;
                        viewModel.ReleaseDate = _dbAppVersion.ReleaseDate;
                        viewModel.LastDateForPatchUpdate = _dbAppVersion.LastDateForPatchUpdate;
                        viewModel.SPExecutionDateTime = _dbAppVersion.SPExecutionDateTime;
                        viewModel.DROfficeID = _dbAppVersion.DRCode == null ? Convert.ToInt32(context.SROMaster.Where(x => x.SROCode == _dbAppVersion.SROCode).Select(x => x.DistrictCode).FirstOrDefault()) : Convert.ToInt32(_dbAppVersion.DRCode);
                        viewModel.IsDROOffice = viewModel.IsDROOffice;

                    }
                    else
                    {
                        viewModel.ErrorMessage = "Failed to Add Script to the Script Manager.";
                    }
                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AppVersionDetailsModel UpdateAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            AppVersion _dbAppVersion = context.AppVersion.Where(x => x.ID == viewModel.VersionID).FirstOrDefault();

                            if (_dbAppVersion != null)
                            {
                                //Added by Omkar Chitnis on 22-04-2020 for validation - Start

                                if (!(_dbAppVersion.AppMajor == viewModel.AppMajor && _dbAppVersion.AppMinor == viewModel.AppMinor))
                                {
                                    if (_dbAppVersion.AppMajor < viewModel.AppMajor)
                                    {

                                        _dbAppVersion.AppMajor = viewModel.AppMajor;

                                        _dbAppVersion.AppMinor = viewModel.AppMinor;
                                    }
                                    else if (_dbAppVersion.AppMajor == viewModel.AppMajor && _dbAppVersion.AppMinor < viewModel.AppMinor)
                                    {

                                        _dbAppVersion.AppMajor = viewModel.AppMajor;

                                        _dbAppVersion.AppMinor = viewModel.AppMinor;

                                    }
                                    else
                                    {
                                        viewModel.ErrorMessage = "App Version can not be lesser than existing version";
                                    }

                                }


                                if (!string.IsNullOrEmpty(viewModel.AppName))
                                {
                                    if (_dbAppVersion.AppMajor >= 1 && _dbAppVersion.AppMinor >= 0)

                                        if (_dbAppVersion.AppName != viewModel.AppName)
                                        {
                                            viewModel.ErrorMessage = "Application name can not be changed for version above 1.0";
                                        }


                                }

                                //END


                                _dbAppVersion.ReleaseDate = viewModel.ReleaseDate;
                                if (viewModel.LastDateForPatchUpdate != null)
                                    _dbAppVersion.LastDateForPatchUpdate = viewModel.LastDateForPatchUpdate;
                                if (viewModel.SPExecutionDateTime != null)
                                    _dbAppVersion.LastDateForPatchUpdate = viewModel.LastDateForPatchUpdate;
                                if (viewModel.IsDROOffice == true)
                                {
                                    _dbAppVersion.DRCode = viewModel.DROfficeID;
                                    // BELOW CODE COMMENTED AND CHANGED BY OMKAR C ON 24-08-2020
                                    _dbAppVersion.SROCode = -2;
                                    //_dbAppVersion.SROCode = -1;
                                    // ABOVE CODE COMMENTED AND CHANGED BY OMKAR C ON 24-08-2020
                                }
                                else
                                {
                                    _dbAppVersion.SROCode = viewModel.SROfficeID;
                                }
                                if (viewModel.IsDROOffice != null)
                                    _dbAppVersion.IsDROffice = viewModel.IsDROOffice;


                                context.Entry(_dbAppVersion).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                tScope.Complete();
                                viewModel.IsInsertedSuccessfully = true;
                                viewModel.ResponseMessage = "Script Updated successfully.";
                            }
                            else
                            {
                                viewModel.IsInsertedSuccessfully = false;
                                viewModel.ErrorMessage = "No Details found selected App Version details";
                                return viewModel;
                            }
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Update Script.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while Updating script to the Script Manager. Error :: " + ex.GetBaseException().Message;
                return viewModel;
            }

        }

        public DROScriptManagerModel InsertInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {

            try
            {
                if (viewModel != null)
                {
                    DROScriptManager _dbDROScriptManager = new DROScriptManager();
                    _dbDROScriptManager.Script = viewModel.ScriptContent;
                    _dbDROScriptManager.IsActive = viewModel.IsActive;
                    _dbDROScriptManager.Description = viewModel.ScriptDescription;
                    _dbDROScriptManager.ServicePack = viewModel.ServicePackNumber;
                    _dbDROScriptManager.DateOfScript = DateTime.Now;
                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            _dbDROScriptManager.ID = context.DROScriptManager.Any() ? context.DROScriptManager.Select(x => x.ID).Max() + 1 : 1;
                            context.DROScriptManager.Add(_dbDROScriptManager);
                            context.SaveChanges();
                            tScope.Complete();

                            viewModel.IsInsertedSuccessfully = true;
                            viewModel.ResponseMessage = "Script added successfully to the Script Manager.";
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Add Script to the Script Manager.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while adding script to the Script Manager. Error :: " + ex.GetBaseException().Message;
            }

            return viewModel;
        }

        public DROScriptManagerDetailWrapperModel LoadDROScriptManagerTable(DROScriptManagerModel viewModel)
        {
            try
            {
                DROScriptManagerDetailWrapperModel resModel = new DROScriptManagerDetailWrapperModel();
                resModel.ScriptManagerDetailList = new List<DROScriptManagerDetailModel>();

                List<DROScriptManager> _dbDROScriptManager = null;

                using (KaveriEntities context = new KaveriEntities())
                {
                    _dbDROScriptManager = context.DROScriptManager.Where(x => x.ServicePack == viewModel.ServicePackNumber).ToList();
                    if (_dbDROScriptManager != null)
                        _dbDROScriptManager = _dbDROScriptManager.OrderByDescending(x => x.DateOfScript).ToList();

                    //_dbDROScriptManager = context.DROScriptManager.Where(x => x.ServicePack == viewModel.ServicePackNumber).ToList();
                    resModel.TotalCount = _dbDROScriptManager.Count;

                    //Added by Omkar on 13-08-2020
                    if (viewModel.IsForSearch)
                    {
                        _dbDROScriptManager = _dbDROScriptManager.ToList();
                    }
                    else
                    {
                        _dbDROScriptManager = _dbDROScriptManager.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                    }
                    // _dbDROScriptManager = _dbDROScriptManager.Skip(viewModel.StartLen).Take(viewModel.TotalNum).ToList();
                }

                if (_dbDROScriptManager != null)
                {
                    int SrNo = 1;
                    DROScriptManagerDetailModel objModel = null;
                    foreach (var item in _dbDROScriptManager)
                    {
                        objModel = new DROScriptManagerDetailModel();
                        objModel.SerialNo = SrNo.ToString();
                        objModel.ID = item.ID.ToString();
                        objModel.Script = item.Script;
                        objModel.ServicePack = item.ServicePack;
                        objModel.IsActive = item.IsActive == null ? false : Convert.ToBoolean(item.IsActive);
                        objModel.Description = item.Description;
                        objModel.DateOfScript = item.DateOfScript.ToString();
                        objModel.Action = @"<span class='glyphicon glyphicon-edit' style='cursor:pointer;font-size: x-large;color:blueviolet;' onclick= EditSROScriptManagerDetails('" + (item.ID) + "')></span>";
                        SrNo++;
                        resModel.ScriptManagerDetailList.Add(objModel);
                    }
                }
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DROScriptManagerModel GetDROScriptDetails(DROScriptManagerModel viewModel)
        {
            try
            {
                DROScriptManager _dbDROScriptManager = new DROScriptManager();
                using (KaveriEntities context = new KaveriEntities())
                {
                    _dbDROScriptManager = context.DROScriptManager.Where(x => x.ID == viewModel.ScriptID).FirstOrDefault();
                }

                if (_dbDROScriptManager != null)
                {
                    viewModel.ScriptID = _dbDROScriptManager.ID;
                    viewModel.ScriptContent = _dbDROScriptManager.Script;
                    viewModel.IsActive = _dbDROScriptManager.IsActive == null ? false : (bool)_dbDROScriptManager.IsActive;
                    viewModel.ScriptDescription = _dbDROScriptManager.Description;
                    viewModel.ServicePackNumber = _dbDROScriptManager.ServicePack;

                }
                else
                {
                    viewModel.ErrorMessage = "Failed to Get details for selected Script from the Script Manager.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DROScriptManagerModel UpdateInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    using (KaveriEntities context = new KaveriEntities())
                    {
                        using (TransactionScope tScope = new TransactionScope())
                        {
                            DROScriptManager _dbDROScriptManager = context.DROScriptManager.Where(x => x.ID == viewModel.ScriptID).FirstOrDefault();

                            if (_dbDROScriptManager != null)
                            {
                                if (!string.IsNullOrEmpty(viewModel.ScriptContent))
                                    _dbDROScriptManager.Script = viewModel.ScriptContent;
                                _dbDROScriptManager.IsActive = viewModel.IsActive;
                                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                                    _dbDROScriptManager.Description = viewModel.ScriptDescription;
                                if (!string.IsNullOrEmpty(viewModel.ServicePackNumber))
                                    _dbDROScriptManager.ServicePack = viewModel.ServicePackNumber;
                                _dbDROScriptManager.DateOfScript = DateTime.Now;
                                context.Entry(_dbDROScriptManager).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                tScope.Complete();
                                viewModel.IsInsertedSuccessfully = true;
                                viewModel.ResponseMessage = "Script Updated successfully.";
                            }
                            else
                            {
                                viewModel.IsInsertedSuccessfully = false;
                                viewModel.ErrorMessage = "No Details found for the Selected Script Details.";
                            }
                        }
                    }
                }
                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Update Script.";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while Updating script to the Script Manager. Error :: " + ex.GetBaseException().Message;
            }

            return viewModel;
        }

        //Added by Omkar on 17-08-2020
        public ApplyAppVersionModel ApplyAppVersionView()
        {
            ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

            KaveriEntities dbContext = null;


            try
            {

                dbContext = new KaveriEntities();

                //Display Application Names:
                var _dbApplyAppVersion = dbContext.AppVersion.Select(x => new SelectListItem { Text = x.AppName, Value = x.AppName }).Distinct().ToList();


                ViewModel.ApplicationNameList = _dbApplyAppVersion;
                // ViewModel.OfficeNameList = _dbOfficeNamelist;

            }
            catch (Exception ex)
            {
                ViewModel.IsInsertedSuccessfully = false;
                ViewModel.ErrorMessage = "Error occured. Error :: " + ex.GetBaseException().Message;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return ViewModel;

        }


        //Added by Omkar on 17-08-2020
        public ApplyAppVersionModel SRDRList()
        {
            ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

            KaveriEntities dbContext = null;

            List<ApplyAppVersionDetaillstModel> dbLst = null;

            try
            {

                dbContext = new KaveriEntities();

                //Display Application Names:
                var _dbApplyAppVersion = dbContext.AppVersion.Select(x => new SelectListItem { Text = x.AppName, Value = x.AppName }).Distinct().ToList();



                dbLst = (from SM in dbContext.SROMaster
                         join DM in dbContext.DistrictMaster
                         on SM.DistrictCode equals DM.DistrictCode
                         orderby DM.DistrictNameE descending
                         select new ApplyAppVersionDetaillstModel
                         {
                             SROOfficeName = SM.SRONameE,
                             SROId = SM.SROCode.ToString(),
                             DROOfficeName = DM.DistrictNameE



                         }).ToList();


                ViewModel.ApplyAppVersionViewList = dbLst;


            }
            catch (Exception ex)
            {
                ViewModel.IsInsertedSuccessfully = false;
                ViewModel.ErrorMessage = "Error occured. Error :: " + ex.GetBaseException().Message;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return ViewModel;

        }


        //Added by Omkar on 09-09-2020
        public ApplyAppVersionModel DRList()
        {
            ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

            KaveriEntities dbContext = null;

            List<ApplyAppVersionDetaillstModel> dbLst = null;

            try
            {

                dbContext = new KaveriEntities();


                dbLst = (from SM in dbContext.SROMaster
                         join DM in dbContext.DistrictMaster
                         on SM.DistrictCode equals DM.DistrictCode
                         orderby DM.DistrictNameE descending
                         select new ApplyAppVersionDetaillstModel
                         {
                             SROOfficeName = "NA",
                             DROId = DM.DistrictCode.ToString(),
                             DROOfficeName = DM.DistrictNameE



                         }).Distinct().ToList();


                ViewModel.ApplyAppVersionViewList = dbLst;


            }
            catch (Exception ex)
            {
                ViewModel.IsInsertedSuccessfully = false;
                ViewModel.ErrorMessage = "Error occured. Error :: " + ex.GetBaseException().Message;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return ViewModel;

        }

        //Added by Omkar on 24-08-2020

        public ApplyAppVersionModel ApplyAppVersion(ApplyAppVersionModel viewModel)
        {
            KaveriEntities context = null;
            try
            {
                if (viewModel != null)
                {
                    using (context = new KaveriEntities())
                    {

                        using (TransactionScope tScope = new TransactionScope())
                        {


                            string sroOfficeIn = viewModel.SROfficeList;

                            //Discard last "," from sroOfficeIn

                            string sroOfficeCodeList = sroOfficeIn.Remove(sroOfficeIn.Length - 1, 1);

                            // List<char> selectedSROlist = sroOfficeIn.ToList();



                            if (sroOfficeIn != null)
                            {

                                context.USP_ApplyAppVersion(sroOfficeCodeList, viewModel.AppMajor, viewModel.AppMinor, viewModel.ReleaseDate, viewModel.LastDateForPatchUpdate, viewModel.AppName);


                                context.SaveChanges();
                                tScope.Complete();
                                viewModel.IsInsertedSuccessfully = true;
                                viewModel.ResponseMessage = "Application Version upated successfully.";


                            }


                            else
                            {
                                viewModel.IsInsertedSuccessfully = false;
                                viewModel.ErrorMessage = " Data not inserted in table ";
                            }
                        }
                    }

                }

                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Update ";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while Updating. Error :: " + ex.GetBaseException().Message;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

            return viewModel;
        }

        //Added by Omkar on 09-09-2020

        public ApplyAppVersionModel ApplyAppVersionDR(ApplyAppVersionModel viewModel)
        {
            KaveriEntities context = null;
            try
            {
                if (viewModel != null)
                {
                    using (context = new KaveriEntities())
                    {

                        using (TransactionScope tScope = new TransactionScope())
                        {


                            string droOfficeIn = viewModel.DROfficeList;

                            //Discard last "," from sroOfficeIn

                            string droOfficeCodeList = droOfficeIn.Remove(droOfficeIn.Length - 1, 1);

                            // List<char> selectedSROlist = sroOfficeIn.ToList();



                            if (droOfficeIn != null)
                            {

                                context.USP_ApplyAppVersionDR(droOfficeCodeList, viewModel.AppMajor, viewModel.AppMinor, viewModel.ReleaseDate, viewModel.LastDateForPatchUpdate, viewModel.AppName);


                                context.SaveChanges();
                                tScope.Complete();
                                viewModel.IsInsertedSuccessfully = true;
                                viewModel.ResponseMessage = "Application Version upated successfully.";


                            }


                            else
                            {
                                viewModel.IsInsertedSuccessfully = false;
                                viewModel.ErrorMessage = " Data not inserted in table ";
                            }
                        }
                    }

                }

                else
                {
                    viewModel.IsInsertedSuccessfully = false;
                    viewModel.ErrorMessage = "Failed to Update ";
                }
                return viewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsInsertedSuccessfully = false;
                viewModel.ErrorMessage = "Error occured while Updating. Error :: " + ex.GetBaseException().Message;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

            return viewModel;


        }
    }
}