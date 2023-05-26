using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
using static ECDataAPI.Common.ApiCommonFunctions;
using ECDataAPI.Entity;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using ECDataAPI.Common;
using System.Collections;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class OfficeDetailsDAL : IOfficeDetails
    {
        private String[] encryptedParameters = null;

        private Dictionary<String, String> decryptedParameters = null;

        public List<SelectListItem> GetOfficeTypeList()
        {
            //   GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> objOfficeTypeList = new List<SelectListItem>();
                SelectListItem objOfficeType = new SelectListItem();
                objOfficeType.Text = "--Select Office Type--";
                objOfficeType.Value = "0";
                objOfficeTypeList.Add(objOfficeType);
                List<MAS_OfficeTypes> mas_OfficeTypesList = dbKaveriOnlineContext.MAS_OfficeTypes.ToList();

                short OthersOfficeType = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.Others);
                short LawDepartmentOfficeType = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.LawDepartment);

                foreach (var OT in mas_OfficeTypesList)
                {
                    // Changes on 15-12-2018 Final Changes in User Management
                    if (OT.OfficeTypeID != OthersOfficeType && OT.OfficeTypeID != LawDepartmentOfficeType)
                    {
                        objOfficeType = new SelectListItem();
                        objOfficeType.Text = OT.OfficeTypeDesc;
                        objOfficeType.Value = OT.OfficeTypeID.ToString();
                        objOfficeTypeList.Add(objOfficeType);
                    }
                }
                return objOfficeTypeList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public List<SelectListItem> GetDistrictList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> objDistrictList = new List<SelectListItem>();
                SelectListItem objDistrict = new SelectListItem();

                objDistrict.Text = "--Select District--";
                objDistrict.Value = "0";
                objDistrictList.Add(objDistrict);

                //commented by shubham bhagat on 9-4-2019  
                //var DistrictList = dbKaveriOnlineContext.MAS_Districts.Where(x => x.StateID == 11 && x.DistrictID != 1043).ToList();
                //var DistrictList = dbKaveriOnlineContext.MAS_Districts.Where(x => x.StateID == 17).ToList();
                var DistrictList = dbKaveriOnlineContext.DistrictMaster.ToList();
                foreach (var District in DistrictList)
                {
                    objDistrict = new SelectListItem();
                    objDistrict.Text = District.DistrictNameE;
                    objDistrict.Value = District.DistrictCode.ToString();
                    objDistrictList.Add(objDistrict);
                }
                return objDistrictList;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public List<SelectListItem> GetParentOfficeNameList(int OfficeTypeId)
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                List<SelectListItem> objOfficeNametList = new List<SelectListItem>();
                SelectListItem objOfficeName = new SelectListItem();
                List<MAS_OfficeMaster> objOfficeNameData = new List<MAS_OfficeMaster>();
                MAS_OfficeMaster mas_OfficeMaster = new MAS_OfficeMaster();
                objOfficeName.Text = "--Select Office--";
                objOfficeName.Value = "0";
                objOfficeNametList.Add(objOfficeName);
                short LawDepartmentOfficeTypeId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.LawDepartment);
                if (OfficeTypeId != 0)
                {
                    // For Adding Office of law Secretary on First Number.
                    mas_OfficeMaster = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID == LawDepartmentOfficeTypeId).FirstOrDefault();
                    if (mas_OfficeMaster != null)
                    {
                        objOfficeName = new SelectListItem();
                        objOfficeName.Text = mas_OfficeMaster.OfficeName;
                        objOfficeName.Value = mas_OfficeMaster.OfficeID.ToString();
                        objOfficeNametList.Add(objOfficeName);
                    }
                    objOfficeNameData = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID < OfficeTypeId).ToList();
                    //objOfficeNameData = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID < OfficeTypeId || x.OfficeTypeID == LawDepartmentOfficeTypeId).ToList();
                }
                else
                {
                    return objOfficeNametList;
                }

                if (objOfficeNameData != null)
                {
                    foreach (var ON in objOfficeNameData)
                    {
                        objOfficeName = new SelectListItem();
                        objOfficeName.Text = ON.OfficeName;
                        objOfficeName.Value = ON.OfficeID.ToString();
                        objOfficeNametList.Add(objOfficeName);
                    }
                }

                return objOfficeNametList;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        //public OfficeDetailsModel GetAllOfficeDetailsList(int DistrictId)
        //{
        //    OfficeDetailsModel responseModel = new OfficeDetailsModel();
        //    responseModel.OfficeTypeList = GetOfficeTypeList();
        //    responseModel.DistrictsList = GetDistrictList();
        //    responseModel.ParentOfficeList = GetParentOfficeNameList(DistrictId);

        //    return responseModel;
        //}

        /// <summary>
        /// Gets AllOfficeDetailsList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public OfficeDetailsModel GetAllOfficeDetailsList(int OfficeTypeId)
        {
            OfficeDetailsModel responseModel = new OfficeDetailsModel();
            if (OfficeTypeId == 0)
            {
                responseModel.OfficeTypeList = GetOfficeTypeList();
                responseModel.DistrictsList = GetDistrictList();
            }
            responseModel.ParentOfficeList = GetParentOfficeNameList(OfficeTypeId);

            return responseModel;

            // Added By Shubham Bhagat on 7-1-2019
            //List<SelectListItem> objOfficeNametList = new List<SelectListItem>();
            //SelectListItem objOfficeName = new SelectListItem();
            //List<MAS_OfficeMaster> objOfficeNameData = new List<MAS_OfficeMaster>();
            //objOfficeName.Text = "--Select Office--";
            //objOfficeName.Value = "0";
            //objOfficeNametList.Add(objOfficeName);
            //responseModel.ParentOfficeList = objOfficeNametList;
        }

        /// <summary>
        /// Creates NewOffice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeDetailsModel CreateNewOffice(OfficeDetailsModel model)
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            MAS_OfficeMaster objNewOffice = new MAS_OfficeMaster();
            OfficeDetailsModel officeDetailsResponseModel = new OfficeDetailsModel();
            try
            {
                // To trim white spaces
                model.OfficeNameE = model.OfficeNameE.Trim();
                model.ShortNameE = model.ShortNameE.Trim();

                // For Checking if Office Name Already exists
                List<MAS_OfficeMaster> mas_officeMasterList = dbKaveriOnlineContext.MAS_OfficeMaster.ToList();
                if (mas_officeMasterList != null)
                {
                    if (mas_officeMasterList.Count() != 0)
                    {
                        foreach (MAS_OfficeMaster mas_officeMaster in mas_officeMasterList)
                        {
                            if (mas_officeMaster.OfficeName.ToLower().Equals(model.OfficeNameE.ToLower()))
                            {

                                officeDetailsResponseModel.ResponseMessage = "Office Name Already Exists.Please try another name.";
                                officeDetailsResponseModel.ResponseStatus = false;
                                return officeDetailsResponseModel;
                            }
                        }
                        foreach (MAS_OfficeMaster mas_officeMaster in mas_officeMasterList)
                        {
                            if (mas_officeMaster.ShortName.ToLower().Equals(model.ShortNameE.ToLower()))
                            {

                                officeDetailsResponseModel.ResponseMessage = "Short Name Already Exists.Please try another name.";
                                officeDetailsResponseModel.ResponseStatus = false;
                                return officeDetailsResponseModel;
                            }
                        }
                        #region Commented by shubham bhagat on 10-4-2019 requirement change
                        //var IsTalukaExist = mas_officeMasterList.Any(x => x.TalukaID == model.TalukaId);
                        //if (IsTalukaExist)
                        //{
                        //    officeDetailsResponseModel.ResponseMessage = "Office Already Exists in Selected Taluka.";
                        //    officeDetailsResponseModel.ResponseStatus = false;
                        //    return officeDetailsResponseModel;
                        //}
                        #endregion
                    }
                }
                if (model != null)
                {
                    //objNewOffice.OfficeID = dbKaveriOnlineContext.MAS_OfficeMaster.Max(x => x.OfficeID);
                    objNewOffice.OfficeID = Convert.ToInt16((dbKaveriOnlineContext.MAS_OfficeMaster.Any() ? dbKaveriOnlineContext.MAS_OfficeMaster.Max(x => x.OfficeID) : 0) + 1);
                    // Below line commented by shubham bhagat on 5-4-2019 officeID increasing by 2
                    //objNewOffice.OfficeID++;
                    objNewOffice.OfficeTypeID = (short)model.OfficeTypeId;
                    objNewOffice.OfficeName = model.OfficeNameE;
                    objNewOffice.OfficeNameR = null;//model.OfficeNameE;
                    objNewOffice.ShortName = model.ShortNameE;
                    objNewOffice.ShortNameR = null; //model.ShortNameR;
                    objNewOffice.DistrictID = (short)model.DistrictId;
                    objNewOffice.ParentOfficeID = (short)model.ParentOfficeId;

                    //constant assignment for this
                    //objNewOffice.GauriOfficeCode = 1;
                    objNewOffice.BhoomiCensusCode = null;

                    objNewOffice.AnyWhereRegEnabled = model.AnyWhereRegEnabled;
                    objNewOffice.OfficeAddress = model.OfficeAddress;
                    //taluka added by chetan
                    #region Commented by shubham bhagat on 10-4-2019 requirement change
                    //if (model.TalukaId == 0)
                    //{
                    //    objNewOffice.TalukaID = null;

                    //}
                    //else
                    //{
                    //    objNewOffice.TalukaID = Convert.ToInt16(model.TalukaId);

                    //}
                    #endregion

                    dbKaveriOnlineContext.MAS_OfficeMaster.Add(objNewOffice);
                    dbKaveriOnlineContext.SaveChanges();

                    // For Activity Log
                    String messageForActivityLog = "Office Detail Added # " + model.OfficeNameE + "- Office Detail Added.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.OfficeDetail), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.OfficeDetail), messageForActivityLog);
                    }
                    officeDetailsResponseModel.ResponseStatus = true;
                    officeDetailsResponseModel.ResponseMessage = "Office Details Added SuccessFully";
                    return officeDetailsResponseModel;
                }
                officeDetailsResponseModel.ResponseStatus = false;
                officeDetailsResponseModel.ResponseMessage = "Office Details Not Added";
                return officeDetailsResponseModel;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Loads OfficeDetailsGridData
        /// </summary>
        /// <returns></returns>

        public OfficeGridWrapperModel LoadOfficeDetailsGridData()
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {



                OfficeGridWrapperModel returnModel = new OfficeGridWrapperModel();
                List<OfficeDetailsModel> lstDatamodel = new List<OfficeDetailsModel>();
                OfficeDetailsModel GridModel = null;
                List<OfficeDataColumn> lstcolumn = new List<OfficeDataColumn>();
                List<MAS_OfficeMaster> resultList = null;

                resultList = dbKaveriOnlineContext.MAS_OfficeMaster.ToList();

                foreach (var item in resultList)
                {

                    GridModel = new OfficeDetailsModel();
                    GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "OfficeID=" + item.OfficeID });
                    GridModel.OfficeTypeName = dbKaveriOnlineContext.MAS_OfficeTypes.Where(x => x.OfficeTypeID == item.OfficeTypeID).FirstOrDefault().OfficeTypeDesc;
                    GridModel.OfficeNameE = item.OfficeName;
                    GridModel.ShortNameE = item.ShortName;                   
                    GridModel.DistrictName =(item.DistrictID != null)? dbKaveriOnlineContext.DistrictMaster.Where(x => x.DistrictCode == item.DistrictID).FirstOrDefault().DistrictNameE:"-";
                    GridModel.ParentOfficeName = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == item.OfficeID).FirstOrDefault().OfficeName;
                    GridModel.AnyWhereRegEnabled = (bool)item.AnyWhereRegEnabled;
                    //if (GridModel.AnyWhereRegEnabled)
                    //    GridModel.IsAnyWhereRegEnabledIcon = "<i class='fa fa-check  ' style='color:black'></i>";
                    //else
                    //    GridModel.IsAnyWhereRegEnabledIcon = "<i class='fa fa-close  ' style='color:black'></i>";
                    GridModel.OfficeAddress = item.OfficeAddress;
                    GridModel.EditBtn = "<a href='#'  onclick=UpdateOfficeDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                    //  GridModel.DeleteBtn = "<a href='#'  onclick=DeleteOfficeDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
                    lstDatamodel.Add(GridModel);
                }

                lstcolumn.Add(new OfficeDataColumn { title = "SR NO", data = "OfficeTypeId" });
                lstcolumn.Add(new OfficeDataColumn { title = "Office Type ", data = "OfficeTypeName" });
                lstcolumn.Add(new OfficeDataColumn { title = "Office Name", data = "OfficeNameE" });

                lstcolumn.Add(new OfficeDataColumn { title = "Short Name", data = "ShortNameE" });
                lstcolumn.Add(new OfficeDataColumn { title = "District ", data = "DistrictName" });
                lstcolumn.Add(new OfficeDataColumn { title = "Parent Office ", data = "ParentOfficeName" });

                //  lstcolumn.Add(new OfficeDataColumn { title = "Any Where Reg Enabled Status", data = "IsAnyWhereRegEnabledIcon" });
                lstcolumn.Add(new OfficeDataColumn { title = "Office Address", data = "OfficeAddress" });

                lstcolumn.Add(new OfficeDataColumn { title = "Edit", data = "EditBtn" });
                //  lstcolumn.Add(new OfficeDataColumn { title = "Delete", data = "DeleteBtn" });


                returnModel.dataArray = lstDatamodel.ToArray();
                returnModel.ColumnArray = lstcolumn.ToArray();
                return returnModel;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Gets OfficeDetails
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>

        public OfficeDetailsModel GetOfficeDetails(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                OfficeDetailsModel responseModel = new OfficeDetailsModel();
                MAS_OfficeMaster dbOfficeMaster = new MAS_OfficeMaster();

                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int officeid = Convert.ToInt32(decryptedParameters["OfficeID"].ToString().Trim());
                dbOfficeMaster = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeid).FirstOrDefault();

                responseModel.DistrictId = (dbOfficeMaster.DistrictID == null) ? 0 : (int)dbOfficeMaster.DistrictID;
                responseModel.OfficeId = dbOfficeMaster.OfficeID;
                responseModel.OfficeNameE = dbOfficeMaster.OfficeName;
                responseModel.OfficeNameR = dbOfficeMaster.OfficeNameR;
                responseModel.OfficeTypeId = dbOfficeMaster.OfficeTypeID;
                responseModel.ShortNameE = dbOfficeMaster.ShortName;
                responseModel.ShortNameR = dbOfficeMaster.ShortNameR;
                responseModel.ParentOfficeId = (short)dbOfficeMaster.ParentOfficeID;
                //responseModel.BhoomicensusCode =(int) dbOfficeMaster.BhoomiCensusCode;
                //responseModel.KaveriOfficeCode = (int)dbOfficeMaster.KaveriOfficeCode;
                responseModel.AnyWhereRegEnabled = (bool)dbOfficeMaster.AnyWhereRegEnabled;
                responseModel.OfficeAddress = dbOfficeMaster.OfficeAddress;

                responseModel.OfficeTypeList = GetOfficeTypeList();
                responseModel.DistrictsList = GetDistrictList();
                responseModel.ParentOfficeList = GetParentOfficeNameList(responseModel.OfficeTypeId);
                // 21-12-2018
                #region Commented by shubham bhagat on 10-4-2019 requirement change
                //if (dbOfficeMaster.TalukaID != null)
                //{
                //    responseModel.TalukaId = Convert.ToInt16(dbOfficeMaster.TalukaID);
                //    responseModel.TalukaList = GetTalukasByDistrictID(Convert.ToInt16(responseModel.DistrictId), true, responseModel.TalukaId);
                //}
                //else
                //{
                //    responseModel.TalukaId = 0;
                //    responseModel.TalukaList = GetTalukasByDistrictID(Convert.ToInt16(responseModel.DistrictId), true);
                //}
                #endregion

                // commented by Shubham Bhagat on 13-04-2019 due to requirement change
                //List<FRM_FirmDetails> frm_FirmDetailsList = dbKaveriOnlineContext.FRM_FirmDetails.Where(x => x.OfficeID == officeid).ToList();
                //if (frm_FirmDetailsList != null)
                //{
                //    if (frm_FirmDetailsList.Count() != 0)
                //    {
                //        responseModel.IsAnyFirmRegisteredForCurrentOffice = true;
                //    }
                //}
                return responseModel;
            }
            catch (Exception )
            {

                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Updates Office
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public OfficeDetailsModel UpdateOffice(OfficeDetailsModel model)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            OfficeDetailsModel officeDetailsResponseModel = new OfficeDetailsModel();
            try
            {
                if (model != null)
                {
                    encryptedParameters = model.EncryptedId.Split('/');

                    if (encryptedParameters.Length != 3)
                        throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");

                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                    int officeid = Convert.ToInt32(decryptedParameters["OfficeID"].ToString().Trim());

                    // To trim white spaces
                    model.OfficeNameE = model.OfficeNameE.Trim();
                    model.ShortNameE = model.ShortNameE.Trim();

                    // For Checking if Office Name Already exists
                    List<MAS_OfficeMaster> mas_officeMasterList = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID != officeid).ToList();
                    if (mas_officeMasterList != null)
                    {
                        if (mas_officeMasterList.Count() != 0)
                        {
                            foreach (MAS_OfficeMaster mas_officeMaster in mas_officeMasterList)
                            {
                                if (mas_officeMaster.OfficeName.ToLower().Equals(model.OfficeNameE.ToLower()))
                                {

                                    officeDetailsResponseModel.ResponseMessage = "Office Name Already Exists.Please try another name.";
                                    officeDetailsResponseModel.ResponseStatus = false;
                                    return officeDetailsResponseModel;
                                }
                            }
                            foreach (MAS_OfficeMaster mas_officeMaster in mas_officeMasterList)
                            {
                                if (mas_officeMaster.ShortName.ToLower().Equals(model.ShortNameE.ToLower()))
                                {

                                    officeDetailsResponseModel.ResponseMessage = "Short Name Already Exists.Please try another name.";
                                    officeDetailsResponseModel.ResponseStatus = false;
                                    return officeDetailsResponseModel;
                                }
                            }
                        }
                    }
                    // For Activity Log
                    String messageForActivityLog = "Office Detail Updated # " + model.OfficeNameE + "- Office Detail Updated.";

                    MAS_OfficeMaster objOffice = new MAS_OfficeMaster();
                    objOffice = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeid).FirstOrDefault();

                    OfficeDetailsModel OfficeObj1 = new OfficeDetailsModel();
                    OfficeObj1.OfficeNameE = model.OfficeNameE;
                    OfficeObj1.OfficeTypeId = model.OfficeTypeId;
                    OfficeObj1.ShortNameE = model.ShortNameE;
                    OfficeObj1.DistrictId = model.DistrictId;
                    OfficeObj1.ParentOfficeId = model.ParentOfficeId;
                    OfficeObj1.OfficeAddress = model.OfficeAddress;

                    OfficeDetailsModel OfficeObj2 = new OfficeDetailsModel();
                    OfficeObj2.OfficeNameE = objOffice.OfficeName;
                    OfficeObj2.OfficeTypeId = objOffice.OfficeTypeID;
                    OfficeObj2.ShortNameE = objOffice.ShortName;
                    OfficeObj2.DistrictId = objOffice.DistrictID ?? 0;
                    OfficeObj2.ParentOfficeId = objOffice.ParentOfficeID ?? 0;
                    OfficeObj2.OfficeAddress = objOffice.OfficeAddress;

                    bool IsObjectsSame = ApiCommonFunctions.CompareObjectsBeforeUpdate<OfficeDetailsModel>(OfficeObj1, OfficeObj2);

                    if (IsObjectsSame)
                    {
                        //Record not change......
                        officeDetailsResponseModel.ResponseStatus = true;
                        officeDetailsResponseModel.ResponseMessage = "No change found in office details";
                        return officeDetailsResponseModel;

                    }
                    else
                    {
                        //Record change......

                        // For Logging before update on 5-1-2019 By Shubham Bhagat  
                        MAS_OfficeMaster_Log mas_OfficeMaster_Log = new MAS_OfficeMaster_Log();
                        mas_OfficeMaster_Log.LogID = (dbKaveriOnlineContext.MAS_OfficeMaster_Log.Any() ? dbKaveriOnlineContext.MAS_OfficeMaster_Log.Max(x => x.LogID) : 0) + 1;
                        mas_OfficeMaster_Log.OfficeID = (short)objOffice.OfficeID;
                        mas_OfficeMaster_Log.OfficeTypeID = (short)objOffice.OfficeTypeID;
                        mas_OfficeMaster_Log.OfficeName = objOffice.OfficeName;
                        mas_OfficeMaster_Log.OfficeNameR = objOffice.OfficeNameR;
                        mas_OfficeMaster_Log.ShortName = objOffice.ShortName;
                        mas_OfficeMaster_Log.ShortNameR = objOffice.ShortNameR;
                        mas_OfficeMaster_Log.DistrictID = (short)objOffice.DistrictID;
                        mas_OfficeMaster_Log.ParentOfficeID = (short)objOffice.ParentOfficeID;
                        //mas_OfficeMaster_Log.GauriOfficeCode = (short?)objOffice.GauriOfficeCode;
                        mas_OfficeMaster_Log.BhoomiCensusCode = (short?)objOffice.BhoomiCensusCode;
                        mas_OfficeMaster_Log.AnyWhereRegEnabled = objOffice.AnyWhereRegEnabled;
                        mas_OfficeMaster_Log.OfficeAddress = objOffice.OfficeAddress;
                        mas_OfficeMaster_Log.TalukaID = objOffice.TalukaID;
                        mas_OfficeMaster_Log.UpdateDateTime = System.DateTime.Now;
                        #region 5-4-2019 For Table LOG by SB
                        mas_OfficeMaster_Log.UserID = model.UserIdForActivityLogFromSession;
                        mas_OfficeMaster_Log.UserIPAddress = model.UserIPAddress;
                        mas_OfficeMaster_Log.ActionPerformed = "Update";
                        #endregion
                        dbKaveriOnlineContext.MAS_OfficeMaster_Log.Add(mas_OfficeMaster_Log);
                        // For Logging before update on 5-1-2019 By Shubham Bhagat  
                        objOffice.OfficeTypeID = (short)model.OfficeTypeId;

                        if (!(objOffice.OfficeName.Equals(model.OfficeNameE)))
                        {
                            messageForActivityLog = messageForActivityLog + "Old Office Name : \"" + objOffice.OfficeName + "\", New Office Name : \"" + model.OfficeNameE + "\"";
                        }
                        objOffice.OfficeName = model.OfficeNameE;
                        objOffice.OfficeNameR = model.OfficeNameR;
                        objOffice.ShortName = model.ShortNameE;
                        objOffice.ShortNameR = model.ShortNameR;
                        objOffice.DistrictID = (short)model.DistrictId;
                        objOffice.ParentOfficeID = (short)model.ParentOfficeId;
                        objOffice.AnyWhereRegEnabled = model.AnyWhereRegEnabled;
                        objOffice.OfficeAddress = model.OfficeAddress;

                        //taluka added by chetan
                        #region Commented by shubham bhagat on 10-4-2019 requirement change
                        //if (model.TalukaId == 0)
                        //    objOffice.TalukaID = null;
                        //else
                        //    objOffice.TalukaID = Convert.ToInt16(model.TalukaId);
                        #endregion
                        //   dbKaveriOnlineContext.MAS_OfficeMaster.Add(objOffice);
                        dbKaveriOnlineContext.SaveChanges();

                        // For Activity Log
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.OfficeDetail), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.OfficeDetail), messageForActivityLog);
                        }
                        officeDetailsResponseModel.ResponseStatus = true;
                        officeDetailsResponseModel.ResponseMessage = "Office Details Updated SuccessFully";
                        return officeDetailsResponseModel;

                    }
                }
                else
                {
                    officeDetailsResponseModel.ResponseStatus = false;
                    officeDetailsResponseModel.ResponseMessage = "Office Details Not Updated";
                    return officeDetailsResponseModel;
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Deletes Office
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteOffice(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            MAS_OfficeMaster objdbOfficeMaster = new MAS_OfficeMaster();
            try
            {
                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int officeid = Convert.ToInt32(decryptedParameters["OfficeID"].ToString().Trim());
                objdbOfficeMaster = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeid).FirstOrDefault();
                if (objdbOfficeMaster != null)
                {
                    dbKaveriOnlineContext.MAS_OfficeMaster.Remove(objdbOfficeMaster);
                    dbKaveriOnlineContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<SelectListItem> RoleList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);
                foreach (var role in dbKaveriOnlineContext.UMG_RoleDetails.ToList())
                {
                    objRole = new SelectListItem();
                    objRole.Text = role.RoleName;
                    objRole.Value = role.RoleID.ToString();
                    roleList.Add(objRole);
                }
                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public List<SelectListItem> ActionList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> ActionList = new List<SelectListItem>();
                SelectListItem objAction = new SelectListItem();
                objAction.Text = "--Select Action--";
                objAction.Value = "0";
                ActionList.Add(objAction);
                foreach (var Action in dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.ToList())
                {
                    objAction = new SelectListItem();
                    objAction.Text = Action.Description;
                    objAction.Value = Action.ActionID.ToString();
                    ActionList.Add(objAction);
                }
                return ActionList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public static DataTable getDataTable(/*this IList<T> data*/)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            IList<UMG_ControllerActionDetails> data = dbKaveriOnlineContext.UMG_ControllerActionDetails.ToList();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(UMG_ControllerActionDetails));
            DataTable table = new DataTable();

            for (int i = 0; i < props.Count - 1; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count - 1];
            foreach (UMG_ControllerActionDetails item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;


        }

        void ExportDataTableToPdf(DataTable dtblTable, String strPdfPath, string strHeader)
        {
            FileStream fs = new FileStream(strPdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
            Document document = new Document();
            document.SetPageSize(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            BaseColor color = new BaseColor(202, 202, 202);

            //Report Header
            BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 16, 1, color);
            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_CENTER;
            prgHeading.Add(new Chunk(strHeader.ToUpper(), fntHead));
            document.Add(prgHeading);

            //Author
            Paragraph prgAuthor = new Paragraph();
            color = new BaseColor(202, 202, 202);
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(btnAuthor, 8, 2, color);
            prgAuthor.Alignment = Element.ALIGN_RIGHT;
            prgAuthor.Add(new Chunk("Author : Amit Sarvalkar", fntAuthor));
            prgAuthor.Add(new Chunk("\nRun Date : " + DateTime.Now.ToShortDateString(), fntAuthor));
            document.Add(prgAuthor);

            //Add a line seperation
            color = new BaseColor(0, 0, 0);
            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
            document.Add(p);

            //Add line break
            document.Add(new Chunk("\n", fntHead));

            //Write the table
            PdfPTable table = new PdfPTable(dtblTable.Columns.Count);
            table.WidthPercentage = 100;
            float[] anchoDeColumnas = new float[] { 10f, 20f, 25f, 35f, 10f };
            table.SetWidths(anchoDeColumnas);

            //Table header
            color = new BaseColor(255, 255, 255);
            BaseFont btnColumnHeader = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntColumnHeader = new iTextSharp.text.Font(btnColumnHeader, 10, 1, color);
            for (int i = 0; i < dtblTable.Columns.Count; i++)
            {
                color = new BaseColor(202, 202, 202);
                PdfPCell cell = new PdfPCell();
                cell.BackgroundColor = color;
                cell.AddElement(new Chunk(dtblTable.Columns[i].ColumnName.ToUpper(), fntColumnHeader));
                table.AddCell(cell);
            }

            //table Data
            for (int i = 0; i < dtblTable.Rows.Count; i++)
            {
                for (int j = 0; j < dtblTable.Columns.Count; j++)
                {
                    table.AddCell(dtblTable.Rows[i][j].ToString());
                }
            }

            document.Add(table);
            document.Close();
            writer.Close();
            fs.Close();
        }

        /// <summary>
        /// Gets TalukasByDistrictID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <param name="isForUpdate"></param>
        /// <param name="talukaId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetTalukasByDistrictID(short DistrictID, bool isForUpdate = false, int talukaId = 0)
        {
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();

                List<SelectListItem> objTalukaList = new List<SelectListItem>();
                List<MAS_Talukas> objTalukaMasterList = new List<MAS_Talukas>();
                SelectListItem objTaluka = new SelectListItem();
                objTaluka.Text = "--Select Taluka--";
                objTaluka.Value = "0";
                objTalukaList.Add(objTaluka);
                if (DistrictID == 0)
                {
                    objTalukaMasterList = dbContext.MAS_Talukas.ToList();
                    if (objTalukaMasterList != null)
                    {
                        foreach (var item in objTalukaMasterList)
                        {
                            objTaluka = new SelectListItem();
                            objTaluka.Text = item.TalukaName;
                            objTaluka.Value = item.TalukaID.ToString();
                            objTalukaList.Add(objTaluka);
                        }
                    }
                }

                else
                {

                    var talukaList = (from taluka in dbContext.MAS_Talukas
                                      join office in dbContext.MAS_OfficeMaster
                                      on taluka.OfficeID equals office.OfficeID

                                      where
                                      office.DistrictID == DistrictID
                                      select new
                                      {
                                          taluka.TalukaID,
                                          taluka.TalukaName
                                      }).ToList();



                    if (isForUpdate)
                    {

                        foreach (var item in talukaList)
                        {
                            MAS_OfficeMaster ObjOfficeMaster = dbContext.MAS_OfficeMaster.Where(x => x.TalukaID == talukaId).FirstOrDefault();
                            if (ObjOfficeMaster != null && ObjOfficeMaster.TalukaID == item.TalukaID)
                            {
                                objTaluka = new SelectListItem();
                                objTaluka.Text = item.TalukaName;
                                objTaluka.Value = item.TalukaID.ToString();
                                objTalukaList.Add(objTaluka);
                            }

                        }
                        foreach (var item in talukaList)
                        {
                            MAS_OfficeMaster ObjOfficeMaster = dbContext.MAS_OfficeMaster.Where(x => x.TalukaID == item.TalukaID).FirstOrDefault();
                            if (ObjOfficeMaster != null)
                            {
                            }
                            else
                            {
                                objTaluka = new SelectListItem();
                                objTaluka.Text = item.TalukaName;
                                objTaluka.Value = item.TalukaID.ToString();
                                objTalukaList.Add(objTaluka);
                            }

                        }
                    }
                    else
                    {

                        foreach (var item in talukaList)
                        {
                            MAS_OfficeMaster ObjOfficeMaster = dbContext.MAS_OfficeMaster.Where(x => x.TalukaID == item.TalukaID).FirstOrDefault();
                            if (ObjOfficeMaster != null)
                            {
                            }
                            else
                            {
                                objTaluka = new SelectListItem();
                                objTaluka.Text = item.TalukaName;
                                objTaluka.Value = item.TalukaID.ToString();
                                objTalukaList.Add(objTaluka);
                            }

                        }
                    }

                }
                return objTalukaList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}