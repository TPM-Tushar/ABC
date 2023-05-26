/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: ServicePackBAL.cs
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : DAL
 * ECR No : 300
*/

#region References
using CustomModels.Models.ServicePackDetails;
using CustomModels.Security;
using ECDataAPI.Areas.ServicePack.BAL;
using ECDataAPI.Areas.ServicePack.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
#endregion

namespace ECDataAPI.Areas.ServicePack.DAL
{
    public class ServicePackDAL : IServicePack
    {
        private KaveriEntities dbContext = null;

        #region Service Pack GET
        /// <summary>
        /// GetReleaseTypeList   
        /// </summary>
        /// <returns>returns release type list </returns>
        public List<SelectListItem> GetReleaseTypeList()
        {
            try
            {
                dbContext = new KaveriEntities();
                List<SelectListItem> levelList = new List<SelectListItem>();
                var dbLevelList = dbContext.SoftwareReleaseType.ToList();
                if (dbLevelList != null)
                {
                    foreach (var item in dbLevelList)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = item.TypeName;
                        selectListItem.Value = Convert.ToString(item.TypeID);
                        levelList.Add(selectListItem);
                    }
                }
                return levelList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// GetReleaseTypeListForEdit
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns>returns list of Release Type List For Edit</returns>
        public List<SelectListItem> GetReleaseTypeListForEdit(int servicePackID)
        {
            try
            {
                dbContext = new KaveriEntities();
                List<SelectListItem> levelList = new List<SelectListItem>();
                var dbLevelList = dbContext.ServicePackDetails.Where(x => x.SpID == servicePackID).ToList();
                if (dbLevelList != null)
                {
                    foreach (var item in dbLevelList)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = item.SoftwareReleaseType.TypeName;
                        selectListItem.Value = Convert.ToString(item.TypeID);
                        levelList.Add(selectListItem);
                    }
                }
                return levelList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// GetChangesTypesList
        /// </summary>
        /// <param name="ChangeTypeID"></param>
        /// <returns>return Changes Types List</returns>
        List<SelectListItem> IServicePack.GetChangesTypesList(int ChangeTypeID = 0)
        {
            try
            {
                dbContext = new KaveriEntities();
                List<SelectListItem> changesTypeList = new List<SelectListItem>();

                //Added by shubham bhagat on 18-09-2019 
                //changesTypeList.Add(new SelectListItem() {
                //    Text = "Select",
                //    Value = "0"
                //});

                var dbChangesTypeList = dbContext.MAS_ServicePackChangeType.ToList();
                if (dbChangesTypeList != null)
                {
                    foreach (var item in dbChangesTypeList)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = item.ChangeTypeDesc;
                        selectListItem.Value = Convert.ToString(item.ChangeTypeID);

                        if (ChangeTypeID > 0)
                            selectListItem.Selected = item.ChangeTypeID == ChangeTypeID;

                        changesTypeList.Add(selectListItem);
                    }
                    if (ChangeTypeID != 0)
                        changesTypeList.RemoveAt(2);
                }
                return changesTypeList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// GetChangesTypeListForEditCall
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns>return Changes Type List For Edit Call</returns>
        List<SelectListItem> IServicePack.GetChangesTypeListForEditCall(int servicePackID)
        {
            try
            {
                dbContext = new KaveriEntities();
                List<SelectListItem> changesTypeList = new List<SelectListItem>();
                var dbChangesTypeList = dbContext.ServicePackChangesDetails.Where(x => x.SpID == servicePackID).ToList();

                if (dbChangesTypeList != null)
                {
                    foreach (var item in dbChangesTypeList)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = item.MAS_ServicePackChangeType.ChangeTypeDesc;
                        selectListItem.Value = Convert.ToString(item.ChangeTypeID);
                        selectListItem.Selected = true;
                        changesTypeList.Add(selectListItem);
                    }
                }
                return changesTypeList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        /// <summary>
        /// Inserts In Three Tables ServicePackFixedDetails,ServicePackEnhancementDetails & ServicePackDetails
        /// </summary>
        /// <param name="ServicePackDetailsViewModel"></param>
        /// <returns></returns>
        public string AddServicePackDetails(ServicePackViewModel ServicePackDetailsViewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                Entity.KaveriEntities.ServicePackDetails servicePackDetailsTableObj;
                Entity.KaveriEntities.ServicePackChangesDetails servicePackChangesDetailsTableObj;

                using (TransactionScope scope = new TransactionScope())
                {
                    servicePackDetailsTableObj = new Entity.KaveriEntities.ServicePackDetails();
                    servicePackChangesDetailsTableObj = new Entity.KaveriEntities.ServicePackChangesDetails();
                    #region Service Pack Details
                    int servicePackID = dbContext.ServicePackDetails.Any() ? dbContext.ServicePackDetails.Max(x => x.SpID) + 1 : 1;
                    servicePackDetailsTableObj.SpID = servicePackID;
                    //------------------------------SAVE FILE----------------------------------------------------------
                    string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                    if (!Directory.Exists(rootPath))
                        Directory.CreateDirectory(rootPath);

                    string systemGeneratedFileName = servicePackDetailsTableObj.SpID.ToString() + "_" + ServicePackDetailsViewModel.ServicePackDetails.Major.ToString() + "_" + ServicePackDetailsViewModel.ServicePackDetails.Minor.ToString() + "_" + DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", "").Replace("/", "") + ServicePackDetailsViewModel.FileExtension; //Date Followed With Time  
                    ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath = rootPath + "\\" + systemGeneratedFileName;
                    string absolutePath = ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/ServicePacks/");
                    ServicePackDetailsViewModel.ServicePackDetails.FileServerVPath = absolutePath;

                    using (FileStream stream = new FileStream(ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath, FileMode.Create))
                    {
                        stream.Write(ServicePackDetailsViewModel.FileData, 0, ServicePackDetailsViewModel.FileData.Length);
                        stream.Close();
                    }
                    //------------------------------SAVE FILE----------------------------------------------------------


                    servicePackDetailsTableObj.TypeID = ServicePackDetailsViewModel.SoftwareReleaseType.TypeID;
                    servicePackDetailsTableObj.TestOrFinal = ServicePackDetailsViewModel.ServicePackDetails.IsTestOrFinal;
                    servicePackDetailsTableObj.Major = ServicePackDetailsViewModel.ServicePackDetails.Major;
                    servicePackDetailsTableObj.Minor = ServicePackDetailsViewModel.ServicePackDetails.Minor;
                    servicePackDetailsTableObj.Description = ServicePackDetailsViewModel.ServicePackDetails.Description;
                    servicePackDetailsTableObj.InstallationProcedure = ServicePackDetailsViewModel.ServicePackDetails.InstallationProcedure;
                    servicePackDetailsTableObj.FileServerFPath = ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath;
                    servicePackDetailsTableObj.FileServerVPath = ServicePackDetailsViewModel.ServicePackDetails.FileServerVPath;
                    servicePackDetailsTableObj.IsActive = true;
                    servicePackDetailsTableObj.ServicePackAddedDateTime = DateTime.Now;
                    //Added by shubham bhagat on 18-09-2019
                    //Commented on 25-09-2019 as discussed by MV Sir
                    //servicePackDetailsTableObj.ServicePackReleaseDateTime = DateTime.Now;
                    servicePackDetailsTableObj.IsReleased = false;
                    servicePackDetailsTableObj.ServicePackUploadedBy = ServicePackDetailsViewModel.ServicePackDetails.ServicePackUploadedBy;
                    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                    if (ServicePackDetailsViewModel.SoftwareReleaseType.TypeID == 3)
                    {
                        servicePackDetailsTableObj.TestOrFinal = null;
                        servicePackDetailsTableObj.IsReleased = true;
                        servicePackDetailsTableObj.ServicePackReleaseDateTime = DateTime.Now;
                        servicePackDetailsTableObj.ServicePackReleasedBy = ServicePackDetailsViewModel.ServicePackDetails.ServicePackUploadedBy;
                        //Added by shubham bhagat on 18-09-2019
                        //servicePackDetailsTableObj.ServicePackIntegratedBy = 261; //SysIntegrator
                        servicePackDetailsTableObj.ServicePackIntegratedBy = dbContext.UMG_UserDetails.Where(x => x.DefaultRoleID == (int)Common.ApiCommonEnum.RoleDetails.SystemIntegrator).Select(x => x.UserID).FirstOrDefault();

                    }
                    //End
                    dbContext.ServicePackDetails.Add(servicePackDetailsTableObj);
                    dbContext.SaveChanges();
                    #endregion
                    //Conditionally add the List Objects and Insert
                    if (!(string.IsNullOrEmpty(ServicePackDetailsViewModel.BugsListValues)))
                    {
                        servicePackChangesDetailsTableObj.SpFixedID = dbContext.ServicePackChangesDetails.Any() ? dbContext.ServicePackChangesDetails.Max(x => x.SpFixedID) : 0;
                        List<ServicePackChangesDetails> bugsValues = GetList(ServicePackDetailsViewModel.BugsListValues, servicePackID, servicePackChangesDetailsTableObj.SpFixedID, 1);
                        dbContext.ServicePackChangesDetails.AddRange(bugsValues);
                        dbContext.SaveChanges();
                    }

                    if (!(string.IsNullOrEmpty(ServicePackDetailsViewModel.EnhancementListValues)))
                    {
                        servicePackChangesDetailsTableObj.SpFixedID = dbContext.ServicePackChangesDetails.Any() ? dbContext.ServicePackChangesDetails.Max(x => x.SpFixedID) : 0;
                        List<ServicePackChangesDetails> enhancementList = GetList(ServicePackDetailsViewModel.EnhancementListValues, servicePackID, servicePackChangesDetailsTableObj.SpFixedID, 2);
                        dbContext.ServicePackChangesDetails.AddRange(enhancementList);
                        dbContext.SaveChanges();
                    }
                    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                    if (!(string.IsNullOrEmpty(ServicePackDetailsViewModel.SupportAnalysisListValues)))
                    {
                        servicePackChangesDetailsTableObj.SpFixedID = dbContext.ServicePackChangesDetails.Any() ? dbContext.ServicePackChangesDetails.Max(x => x.SpFixedID) : 0;
                        List<ServicePackChangesDetails> SupportAnalysisList = GetList(ServicePackDetailsViewModel.SupportAnalysisListValues, servicePackID, servicePackChangesDetailsTableObj.SpFixedID, 3);
                        dbContext.ServicePackChangesDetails.AddRange(SupportAnalysisList);
                        dbContext.SaveChanges();

                    }
                    
                        scope.Complete();
                    return "";
                }
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ApiExceptionLogs.LogError(ex);
                return "Error occurred while inserting the entry into database. Please try again.";
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        /// <summary>
        /// GetList
        /// </summary>
        /// <param name="ListValues"></param>
        /// <param name="servicePackID"></param>
        /// <param name="spFixedID"></param>
        /// <param name="changeTypeID"></param>
        /// <returns>returns list of service pack change list</returns>
        private static List<ServicePackChangesDetails> GetList(string ListValues, int servicePackID, int spFixedID, int changeTypeID)
        {
            List<ServicePackChangesDetails> obj = new List<ServicePackChangesDetails>();
            string[] bugsDetails = ListValues.Split('|').ToArray();
            ServicePackChangesDetails servicePackChangesDetails;
            foreach (string item in bugsDetails)
            {
                servicePackChangesDetails = new ServicePackChangesDetails();
                servicePackChangesDetails.SpFixedID = ++spFixedID;
                servicePackChangesDetails.SpID = servicePackID;
                servicePackChangesDetails.Description = item;
                servicePackChangesDetails.ChangeTypeID = changeTypeID;
                obj.Add(servicePackChangesDetails);
            }
            return obj;
        }

        /// <summary>
        /// GetServicePackDetailsList
        /// </summary>
        /// <param name="IsRequestForApprovalsList"></param>
        /// <param name="IsRequestForReleasedServiceList"></param>
        /// <returns>returns list of Service Pack Details List</returns>
        /// //Added and Changed by mayank on 14/09/2021 for Support Exe Release
        public List<ServicePackViewModel> GetServicePackDetailsList(bool IsRequestForApprovalsList, bool IsRequestForReleasedServiceList, int RoleID)
        {
            List<ServicePackViewModel> List = new List<ServicePackViewModel>();
            KaveriEntities dbContext = null;
            StringBuilder sbInstallationProc = new StringBuilder();
            try
            {
                dbContext = new KaveriEntities();
                ServicePackViewModel servicePackViewModel;
                List<Entity.KaveriEntities. ServicePackDetails> releaseList = new List<Entity.KaveriEntities.ServicePackDetails>();
                //SW Support gets all active Deactive List
                var Result = dbContext.ServicePackDetails.OrderByDescending(x => x.ServicePackAddedDateTime).ToList();

                //AIGR COMP
                if (IsRequestForApprovalsList)
                {
                    // Commented and changed condition by shubham bhagat on 05-10-2019 as discussed by MV Sir
                    //Result = Result.Where(m => m.IsReleased == false && m.IsActive == true).ToList();
                    Result = Result.Where(m =>  m.IsActive == true).ToList();
                }
                //if Techincal Consultant login
                //if(RoleID==(int)ApiCommonEnum.RoleDetails.TechnicalConsultant)
                //{
                //    Result = Result.Where(m => m.IsActive && m.TypeID == 3).ToList();

                //System Integrator
                if (IsRequestForReleasedServiceList)
                {
                    Result = Result.Where(m => m.IsReleased == true && m.IsActive == true).ToList();
                    //if the set of items are to be released
                    foreach (var item in Result)
                    {
                        if (IsDateGreaterThanForseeableDate((DateTime)item.ServicePackReleaseDateTime))
                        {
                            releaseList.Add(item);
                        }
                        else
                        {
                            item.FileServerVPath = "ForseeableRestriction";
                            releaseList.Add(item);
                        }
                    }
                    Result = releaseList;
                }
                int serialNo = 1;
                foreach (var item in Result)
                {
                    servicePackViewModel = new ServicePackViewModel();
                    servicePackViewModel.SerialNo = serialNo++;
                    servicePackViewModel.ServicePackDetails = new CustomModels.Models.ServicePackDetails.ServicePackDetail();
                    servicePackViewModel.ServicePackChangesDetails = new CustomModels.Models.ServicePackDetails.ServicePackChangesDetailsModel();
                    servicePackViewModel.SoftwareReleaseType = new CustomModels.Models.ServicePackDetails.SoftwareReleaseTypes();
                    servicePackViewModel.EncryptedID = URLEncrypt.EncryptParameters(new string[] { "ID=" + item.SpID.ToString().Trim() });
                    servicePackViewModel.ServicePackDetails.SpID = item.SpID;
                    servicePackViewModel.ChangesTable = LoadChangesForServicePackTable(item.SpID);
                    servicePackViewModel.SoftwareReleaseType.TypeName = item.SoftwareReleaseType.TypeName;
                    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                    //servicePackViewModel.ServicePackDetails.IsTestFinal = item.TestOrFinal ? "Test Release" : "Final Release";
                    servicePackViewModel.ServicePackDetails.IsTestFinal = Convert.ToBoolean(item.TestOrFinal) ? "Test Release" : "Final Release";
                    if (item.SoftwareReleaseType.TypeID == 3)
                    {
                        servicePackViewModel.ServicePackDetails.IsTestFinal = "Support Release";
                    }
                    servicePackViewModel.ServicePackDetails.Major = item.Major;
                    servicePackViewModel.ServicePackDetails.Minor = item.Minor;
                    servicePackViewModel.ServicePackDetails.Description = item.Description;
                    servicePackViewModel.ServicePackDetails.InstallationProcedure = item.InstallationProcedure;
                    servicePackViewModel.ServicePackDetails.FileServerFPath = item.FileServerFPath;
                    servicePackViewModel.ServicePackDetails.FileServerVPath = item.FileServerVPath;
                    servicePackViewModel.ServicePackDetails.IsReleased = item.IsReleased;
                    servicePackViewModel.ServicePackDetails.IsActive = item.IsActive;
                    //Added by shubham bhagat on 18-09-2019
                    //commented on 25-09-2019 because it is not used any where 
                    //servicePackViewModel.ServicePackDetails.ServicePackReleaseDateTime = (DateTime)item.ServicePackReleaseDateTime;
                    servicePackViewModel.ReleaseDetails = item.ServicePackReleaseDetails.Where(z => z.SpID == item.SpID).Select(z => z.ReleaseNotes).FirstOrDefault();
                    //Added by shubham bhagat on 18-09-2019
                    servicePackViewModel.ServicePackReleaseDate = item.ServicePackReleaseDateTime == null ? "" : ((DateTime)item.ServicePackReleaseDateTime).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //servicePackViewModel.ServicePackReleaseDate = item.ServicePackReleaseDateTime.ToString("MM/dd/yyyy");
                     //Added By Tushar on 2 Jan 2023 for Upload DateTime
                    servicePackViewModel.ServicePackAddedDateTime = item.ServicePackAddedDateTime == null ? "" : ((DateTime)item.ServicePackAddedDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //End By Tushar on 2 Jan 2023
                    List.Add(servicePackViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// IsDateGreaterThanForseeableDate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsDateGreaterThanForseeableDate(DateTime input)
        {
            //if (DateTime.Now.Date >= input)
            //Added and Changed by mayank on 14/09/2021 for Support Exe Release
            if (DateTime.Now.Date >= input.Date)
                return true;
            else
                return false;
        }

        /// <summary>
        /// LoadChangesForServicePackTable
        /// </summary>
        /// <param name="spID"></param>
        /// <returns>returns string containing Changes For Service Pack Table</returns>
        private string LoadChangesForServicePackTable(int spID)
        {
            KaveriEntities dbContext = null;
            StringBuilder sBuilder = new StringBuilder();
            try
            {
                dbContext = new KaveriEntities();
                List<ServicePackChangesDetails> Result = dbContext.ServicePackChangesDetails.Where(x => x.SpID == spID).ToList();
                if (Result.Count() == 0)
                {
                    sBuilder.Append("<center><table style='border: 1px solid #ddd;'><tbody><tr style='border: 1px solid #ddd; padding: 4px; padding-bottom:2px;' ><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>No Records</b></td></tr>");
                    return sBuilder.ToString();
                }
                else
                {
                    sBuilder.Append("<table style='border: 1px solid #ddd;'><tbody><tr style='border: 1px solid #ddd; padding: 4px; padding-bottom:2px;' ><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>Sr. No.</b></td><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>Description</b></td><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>Change Type</b></td></tr>");
                    int i = 0;
                    foreach (var item in Result)
                    {
                        sBuilder.Append("<tr>");
                        sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + ++i + "</td>");
                        sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + item.Description + "</td>");
                        sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + item.MAS_ServicePackChangeType.ChangeTypeDesc + "</td>");
                        sBuilder.Append("</tr>");
                    }
                    sBuilder.Append("</tbody>");
                    sBuilder.Append("</table>");
                    //sBuilder.Append("</center>");
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
            return sBuilder.ToString();
        }

        /// <summary>
        /// GetServicePackDetails
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns>returns Service Pack Details</returns>
        public ServicePackViewModel GetServicePackDetails(int servicePackID)
        {
            ServicePackViewModel servicePackViewModel = null;
            try
            {
                dbContext = new KaveriEntities();
                servicePackViewModel = new ServicePackViewModel();
                servicePackViewModel.ServicePackDetails = new CustomModels.Models.ServicePackDetails.ServicePackDetail();
                servicePackViewModel.ServicePackChangesDetails = new CustomModels.Models.ServicePackDetails.ServicePackChangesDetailsModel();
                servicePackViewModel.SoftwareReleaseType = new CustomModels.Models.ServicePackDetails.SoftwareReleaseTypes();

                var servicePackByID = dbContext.ServicePackDetails.Where(x => x.SpID == servicePackID).FirstOrDefault();
                int changeID = dbContext.ServicePackChangesDetails.Where(x => x.SpID == servicePackByID.SpID).Select(x => x.ChangeTypeID).FirstOrDefault();
                servicePackViewModel.EncryptedID = URLEncrypt.EncryptParameters(new string[] { "ID=" + servicePackID.ToString().Trim() });
                servicePackViewModel.ServicePackDetails.SpID = servicePackByID.SpID;
                servicePackViewModel.servicePackChagesDetails = GetChangesDetails(servicePackByID.SpID);
                servicePackViewModel.SoftwareReleaseType.TypeName = servicePackByID.SoftwareReleaseType.TypeName;
                servicePackViewModel.ServicePackDetails.IsTestOrFinal = Convert.ToBoolean(servicePackByID.TestOrFinal);
                servicePackViewModel.SoftwareReleaseType.TypeID = servicePackByID.TypeID;
                servicePackViewModel.ServicePackChangesDetails.ChangeType = changeID;
                servicePackViewModel.ServicePackDetails.Major = servicePackByID.Major;
                servicePackViewModel.ServicePackDetails.Minor = servicePackByID.Minor;
                servicePackViewModel.ServicePackDetails.Description = servicePackByID.Description;
                servicePackViewModel.ServicePackDetails.InstallationProcedure = servicePackByID.InstallationProcedure;
                servicePackViewModel.ServicePackDetails.FileServerFPath = servicePackByID.FileServerFPath;
                servicePackViewModel.ServicePackDetails.FileServerVPath = servicePackByID.FileServerVPath;
                servicePackViewModel.ServicePackDetails.IsActive = servicePackByID.IsActive;
                return servicePackViewModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// GetChangesDetails
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns>return Service Pack Changes Details Model</returns>
        private List<ServicePackChangesDetailsModel> GetChangesDetails(int servicePackID)
        {
            IServicePack objServicePackBAL = new ServicePackBAL();
            try
            {
                dbContext = new KaveriEntities();
                List<ServicePackChangesDetailsModel> levelList = new List<ServicePackChangesDetailsModel>();
                var dbLevelList = dbContext.ServicePackChangesDetails.Where(x => x.SpID == servicePackID).ToList(); ;
                if (dbLevelList != null)
                {
                    foreach (var item in dbLevelList)
                    {
                        levelList.Add(new ServicePackChangesDetailsModel()
                        {
                            Description = item.Description,
                            ChangeTypeDropDownNew = objServicePackBAL.GetChangesTypesList(item.ChangeTypeID),
                            SelectedValueChangeTypeDesc = item.MAS_ServicePackChangeType.ChangeTypeDesc,
                            SpFixedID = URLEncrypt.EncryptParameters(new string[] { "ID=" + item.SpFixedID.ToString().Trim() }),
                            ChangeType = item.ChangeTypeID
                        });
                    }
                }
                return levelList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// UpdateServicePackDetails
        /// </summary>
        /// <param name="ServicePackDetailsViewModel"></param>
        /// <returns>returns update status of service pack update operation</returns>
        public string UpdateServicePackDetails(ServicePackViewModel ServicePackDetailsViewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                Entity.KaveriEntities.ServicePackChangesDetails servicePackChangesDetailsTableObj;

                using (TransactionScope scope = new TransactionScope())
                {
                    //Service Pack Details
                    Entity.KaveriEntities.ServicePackDetails servicePackDetailsTableObj = dbContext.ServicePackDetails.Find(ServicePackDetailsViewModel.ServicePackDetails.SpID);
                    servicePackDetailsTableObj.TypeID = ServicePackDetailsViewModel.SoftwareReleaseType.TypeID;
                    servicePackDetailsTableObj.TestOrFinal = ServicePackDetailsViewModel.ServicePackDetails.IsTestOrFinal;
                    servicePackDetailsTableObj.Major = ServicePackDetailsViewModel.ServicePackDetails.Major;
                    servicePackDetailsTableObj.Minor = ServicePackDetailsViewModel.ServicePackDetails.Minor;
                    servicePackDetailsTableObj.Description = ServicePackDetailsViewModel.ServicePackDetails.Description;
                    servicePackDetailsTableObj.InstallationProcedure = ServicePackDetailsViewModel.ServicePackDetails.InstallationProcedure;
                    //Added by shubham bhagat on 18-09-2019
                    if (ServicePackDetailsViewModel.IsFileToUpdate && ServicePackDetailsViewModel.IsFileUploadedSuccessfully)
                    {
                        string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                        //------------------------------DELETE FILE-------------------------------------------------------
                        string filePathToDelete = servicePackDetailsTableObj.FileServerFPath;
                        File.Delete(filePathToDelete);

                        //------------------------------SAVE FILE----------------------------------------------------------

                        if (!Directory.Exists(rootPath))
                            Directory.CreateDirectory(rootPath);

                        string systemGeneratedFileName = servicePackDetailsTableObj.SpID.ToString() + "_" + ServicePackDetailsViewModel.ServicePackDetails.Major.ToString() + "_" + ServicePackDetailsViewModel.ServicePackDetails.Minor.ToString() + "_" + DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", "").Replace("/", "") + ServicePackDetailsViewModel.FileExtension; //Date Followed With Time  
                        ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath = rootPath + "\\" + systemGeneratedFileName;
                        string absolutePath = ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/ServicePacks/");
                        ServicePackDetailsViewModel.ServicePackDetails.FileServerVPath = absolutePath;

                        using (FileStream stream = new FileStream(ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath, FileMode.Create))
                        {
                            stream.Write(ServicePackDetailsViewModel.FileData, 0, ServicePackDetailsViewModel.FileData.Length);
                            stream.Close();
                        }


                        //------------------------------FILE PATH UPDATE IN DATABASE---------------------------------
                        servicePackDetailsTableObj.FileServerFPath = ServicePackDetailsViewModel.ServicePackDetails.FileServerFPath;
                        servicePackDetailsTableObj.FileServerVPath = ServicePackDetailsViewModel.ServicePackDetails.FileServerVPath;

                    }
                    //Commented by Shubham Bhagat on 05-10-2019
                    //servicePackDetailsTableObj.IsActive = true;
                    servicePackDetailsTableObj.ServicePackAddedDateTime = DateTime.Now;
                    //Commented on 25-09-2019 as discussed by MV Sir
                    //servicePackDetailsTableObj.ServicePackReleaseDateTime = DateTime.Now;
                    servicePackDetailsTableObj.IsReleased = false;
                    servicePackDetailsTableObj.ServicePackUploadedBy = ServicePackDetailsViewModel.ServicePackDetails.ServicePackUploadedBy;
                    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                    if (ServicePackDetailsViewModel.SoftwareReleaseType.TypeID == 3)
                    {
                        servicePackDetailsTableObj.TestOrFinal = null;

                    }
                    dbContext.Entry(servicePackDetailsTableObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    DeleteChangesDetailsForServicePack(ServicePackDetailsViewModel.ServicePackDetails.SpID);
                    foreach (var items in ServicePackDetailsViewModel.WebGridListValues)
                    {
                        servicePackChangesDetailsTableObj = new Entity.KaveriEntities.ServicePackChangesDetails();
                        servicePackChangesDetailsTableObj.SpFixedID = dbContext.ServicePackChangesDetails.Any() ? dbContext.ServicePackChangesDetails.Max(x => x.SpFixedID) + 1 : 1;
                        servicePackChangesDetailsTableObj.SpID = ServicePackDetailsViewModel.ServicePackDetails.SpID;
                        servicePackChangesDetailsTableObj.Description = items.Key;
                        servicePackChangesDetailsTableObj.ChangeTypeID = items.Value;
                        dbContext.ServicePackChangesDetails.Add(servicePackChangesDetailsTableObj);
                        dbContext.SaveChanges();
                    }
                    scope.Complete();
                    return "";
                }
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ApiExceptionLogs.LogError(ex);
                return "Error occurred while updating the entry into database. Please try again.";
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        /// <summary>
        /// DeleteChangesDetailsForServicePack
        /// </summary>
        /// <param name="spID"></param>
        /// <returns>returns Delete Changes Details For Service Pack status</returns>
        private string DeleteChangesDetailsForServicePack(int spID)
        {
            try
            {
                dbContext = new KaveriEntities();
                dbContext.ServicePackChangesDetails.RemoveRange(dbContext.ServicePackChangesDetails.Where(c => c.SpID == spID));
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DeleteServicePackDetailsEntry
        /// </summary>
        /// <param name="spFixedIDToBeDeleted"></param>
        /// <returns>returns Delete Service Pack Details Entry</returns>
        public string DeleteServicePackDetailsEntry(int spFixedIDToBeDeleted)
        {
            dbContext = new KaveriEntities();
            try
            {
                //Added by shubham bhagat on 18-09-2019
                int servicePackID = dbContext.ServicePackChangesDetails.Where(c => c.SpFixedID == spFixedIDToBeDeleted).Select(x => x.SpID).SingleOrDefault();
                if (servicePackID != 0)
                {
                    int ModificationTypeIDs = dbContext.ServicePackChangesDetails.Where(c => c.SpID == servicePackID).Count();

                    if (ModificationTypeIDs == 1)
                    {
                        return "Modification Type cannot be deleted, because at least one modification type should exists for a service pack.";
                    }
                }

                ServicePackChangesDetails detailsServicePackChangesDetails = dbContext.ServicePackChangesDetails.Where(c => c.SpFixedID == spFixedIDToBeDeleted).SingleOrDefault();
                if (null != detailsServicePackChangesDetails)
                    dbContext.ServicePackChangesDetails.Remove(detailsServicePackChangesDetails);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// DeactivateServicePackDetailsEntry
        /// </summary>
        /// <param name="spFixedIDToBeDeleted"></param>
        /// <returns>returns Deactivate Service Pack Details Entry status</returns>
        public string DeactivateServicePackDetailsEntry(int spFixedIDToBeDeleted)
        {
            dbContext = new KaveriEntities();
            try
            {
                ServicePackDetails detailsServicePackDetails = dbContext.ServicePackDetails.Where(c => c.SpID == spFixedIDToBeDeleted).SingleOrDefault();
                if (null != detailsServicePackDetails)
                {
                    detailsServicePackDetails.IsActive = false;
                }
                dbContext.Entry(detailsServicePackDetails).State = EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// SaveReleaseNotesDetails
        /// </summary>
        /// <param name="releaseDetailsModelObj"></param>
        /// <returns>returns Save Release Notes Details status</returns>
        public string SaveReleaseNotesDetails(ReleaseDetails releaseDetailsModelObj)
        {
            try
            {
                dbContext = new KaveriEntities();
                Entity.KaveriEntities.ServicePackReleaseDetails servicePackReleaseDetailsTableObj;
                using (TransactionScope scope = new TransactionScope())
                {
                    servicePackReleaseDetailsTableObj = new Entity.KaveriEntities.ServicePackReleaseDetails();
                    Entity.KaveriEntities.ServicePackDetails servicePackDetailsTableObj = dbContext.ServicePackDetails.Find(releaseDetailsModelObj.ServicePackID);

                    #region Service Pack Details
                    servicePackReleaseDetailsTableObj.SpReleaseID = dbContext.ServicePackReleaseDetails.Any() ? dbContext.ServicePackReleaseDetails.Max(x => x.SpReleaseID) + 1 : 1;
                    servicePackReleaseDetailsTableObj.SpID = releaseDetailsModelObj.ServicePackID;
                    servicePackReleaseDetailsTableObj.ReleaseNotes = releaseDetailsModelObj.ReleaseNotes;
                    dbContext.ServicePackReleaseDetails.Add(servicePackReleaseDetailsTableObj);
                    dbContext.SaveChanges();

                    servicePackDetailsTableObj.IsReleased = true;
                    servicePackDetailsTableObj.ServicePackReleaseDateTime = releaseDetailsModelObj.ReleaseDateTime;
                    servicePackDetailsTableObj.ServicePackReleasedBy = releaseDetailsModelObj.ServicePackReleasedBy;
                    //Added by shubham bhagat on 18-09-2019
                    //servicePackDetailsTableObj.ServicePackIntegratedBy = 261; //SysIntegrator
                    servicePackDetailsTableObj.ServicePackIntegratedBy = dbContext.UMG_UserDetails.Where(x => x.DefaultRoleID == (int)Common.ApiCommonEnum.RoleDetails.SystemIntegrator).Select(x => x.UserID).FirstOrDefault();

                    dbContext.Entry(servicePackDetailsTableObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    #endregion
                    scope.Complete();
                    return "";
                }
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ApiExceptionLogs.LogError(ex);
                return "Error occurred while inserting the entry into database. Please try again.";
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        /// <summary>
        /// ActivateServicePackDetailsEntry
        /// </summary>
        /// <param name="spFixedIDToBeDeleted"></param>
        /// <returns>returns Activate Service Pack Details Entry status </returns>
        public string ActivateServicePackDetailsEntry(int spFixedIDToBeDeleted)
        {
            dbContext = new KaveriEntities();
            try
            {
                Entity.KaveriEntities.ServicePackDetails detailsServicePackDetails = dbContext.ServicePackDetails.Where(c => c.SpID == spFixedIDToBeDeleted).SingleOrDefault();
                if (null != detailsServicePackDetails)
                {
                    detailsServicePackDetails.IsActive = true;
                }
                dbContext.Entry(detailsServicePackDetails).State = EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// CheckIfServicePackVersionAlreadyExists
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns>returns Check If Service Pack Version Already Exists status</returns>
        public bool CheckIfServicePackVersionAlreadyExists(int majorVersion, int minorVersion, bool releaseType)
        {
            dbContext = new KaveriEntities();
            try
            {
                int countSP = dbContext.ServicePackDetails.Where(c => c.Major == majorVersion && c.Minor == minorVersion && c.TestOrFinal == releaseType).Count();
                if (countSP > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// DownloadServicePackFile
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns>returns Download Service Pack File</returns>
        public DownloadResponseModel DownloadServicePackFile(int servicePackID)
        {
            dbContext = new KaveriEntities();
            DownloadResponseModel downloadResponseModel = new DownloadResponseModel()
            {
                errorMsg = string.Empty
            };
            try
            {
                var servicePackOBJ = dbContext.ServicePackDetails.Where(x => x.SpID == servicePackID).
                    Select(x => new
                    {
                        SPVirtualPath = x.FileServerVPath,
                        SPMajorVerID = x.Major,
                        SPMinorVerID = x.Minor,
                        SPTypeID = x.TypeID
                    })
                    .FirstOrDefault();

                if (servicePackOBJ != null)
                {
                    if (String.IsNullOrEmpty(servicePackOBJ.SPVirtualPath))
                        downloadResponseModel.errorMsg = "Invalid File Path.";
                    else
                    {
                        string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                        string filepath = rootPath + Path.GetFileName(servicePackOBJ.SPVirtualPath);
                        if (File.Exists(filepath))
                        {
                            downloadResponseModel.FileByte = File.ReadAllBytes(filepath);
                            downloadResponseModel.Filename =
                                (servicePackOBJ.SPTypeID == (short)Common.ApiCommonEnum.ServicePackSoftwareReleaseType.EXE_DLL
                                ? "EXE_DLL_"
                                : "ServicePack_"
                                ) + servicePackOBJ.SPMajorVerID + "_" + servicePackOBJ.SPMinorVerID+".zip";
                        }
                        else
                            downloadResponseModel.errorMsg = "File does not exists at the path.";
                    }
                }
                else
                    downloadResponseModel.errorMsg = "Service Pack does not exits.";

                return downloadResponseModel;
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
        }

        public DownloadResponseModel CheckIfFileExists(int servicePackID)
        {
            dbContext = new KaveriEntities();
            DownloadResponseModel downloadResponseModel = new DownloadResponseModel()
            {
                errorMsg = string.Empty
            };
            try
            {
                var servicePackOBJ = dbContext.ServicePackDetails.Where(x => x.SpID == servicePackID).
                    Select(x => new
                    {
                        SPVirtualPath = x.FileServerVPath,
                        SPMajorVerID = x.Major,
                        SPMinorVerID = x.Minor,
                        SPTypeID = x.TypeID
                    })
                    .FirstOrDefault();

                if (servicePackOBJ != null)
                {
                    if (String.IsNullOrEmpty(servicePackOBJ.SPVirtualPath))
                        downloadResponseModel.errorMsg = "Invalid File Path.";
                    else
                    {
                        string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                        string filepath = rootPath + Path.GetFileName(servicePackOBJ.SPVirtualPath);
                        if (!File.Exists(filepath))                     
                            downloadResponseModel.errorMsg = "File does not exists at the path.";
                    }
                }
                else
                    downloadResponseModel.errorMsg = "Service Pack does not exits.";

                return downloadResponseModel;
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
        }
    }
}