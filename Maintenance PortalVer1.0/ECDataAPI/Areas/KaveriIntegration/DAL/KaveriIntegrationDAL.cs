/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationDAL.cs
* Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : DAL
 * ECR No : 
*/

using CustomModels.Models.KaveriIntegration;
using ECDataAPI.Areas.KaveriIntegration.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.KaveriIntegration.DAL
{
    public class KaveriIntegrationDAL : IKaveriIntegration
    {
        KaveriEntities dbContext = null;
        KaveriIntegrationDetail kaveriIntegrationDetail = null;

        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Kaveri Integration Model</returns>
        public KaveriIntegrationModel KaveriIntegrationView(int OfficeID)
        {
            KaveriIntegrationModel model = new KaveriIntegrationModel();
            model.DistrictList = new List<SelectListItem>();
            model.SROfficeList = new List<SelectListItem>();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            try
            {
                dbContext = new KaveriEntities();
                //var DistrictMasterList = dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).ToList();
                //model.DistrictList.Add(new SelectListItem { Text = "All", Value = "0" });
                //if (DistrictMasterList != null)
                //{
                //    if (DistrictMasterList.Count > 0)
                //    {
                //        foreach (var item in DistrictMasterList)
                //        {
                //            SelectListItem select = new SelectListItem()
                //            {
                //                Text = item.DistrictNameE,
                //                Value = item.DistrictCode.ToString()
                //            };
                //            model.DistrictList.Add(select);
                //        }
                //    }
                //}

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();

                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                //string kaveriCode = Kaveri1Code.ToString();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DistrictList.Add(droNameItem);
                    //model.SROfficeList.Add(sroNameItem);
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });

                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DistrictList.Add(droNameItem);
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //model.SROfficeList.Add(select);
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("All");
                }
            }
            catch (Exception) { throw; }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return model;
        }


        //public KaveriIntegrationModel KaveriIntegrationView(int OfficeID)
        //{
        //    KaveriIntegrationModel model = new KaveriIntegrationModel();
        //    model.DistrictList = new List<SelectListItem>();
        //    model.SROfficeList = new List<SelectListItem>();
        //    ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        //var DistrictMasterList = dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).ToList();
        //        //model.DistrictList.Add(new SelectListItem { Text = "All", Value = "0" });
        //        //if (DistrictMasterList != null)
        //        //{
        //        //    if (DistrictMasterList.Count > 0)
        //        //    {
        //        //        foreach (var item in DistrictMasterList)
        //        //        {
        //        //            SelectListItem select = new SelectListItem()
        //        //            {
        //        //                Text = item.DistrictNameE,
        //        //                Value = item.DistrictCode.ToString()
        //        //            };
        //        //            model.DistrictList.Add(select);
        //        //        }
        //        //    }
        //        //}


        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

        //        string kaveriCode = Kaveri1Code.ToString();
        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {

        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);

        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList.Add(sroNameItem);

        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {

        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
        //        }
        //        else
        //        {                  
        //            model.DistrictList = objCommon.GetDROfficesList("All");
        //            SelectListItem select = new SelectListItem();
        //            select.Text = "All";
        //            select.Value = "0";
        //            model.SROfficeList.Add(select);
        //        }
        //    }
        //    catch (Exception) { throw; }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }
        //    return model;
        //}

        /// <summary>
        /// Load Kaveri Integration Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Kaveri Integration Wrapper Model</returns>
        public KaveriIntegrationWrapperModel LoadKaveriIntegrationTable(KaveriIntegrationModel model)
        {
            KaveriIntegrationWrapperModel responseModel = new KaveriIntegrationWrapperModel()
            {
                KaveriIntegrationDetailList = new List<KaveriIntegrationDetail>()
            };
            try
            {
                dbContext = new KaveriEntities();
                if (model.IsForExcelDownload || model.IsForSearch)
                {
                    var result = dbContext.USP_RPT_KaveriIntegrationReportDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.DistrictID, model.SROfficeID)
                      .ToList();
                    //responseModel.TotalCount = dbContext.USP_RPT_KaveriIntegrationReportDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.DistrictID, model.SROfficeID).ToList().Count();

                    //Changed by M Rafe on 02/05/2020
                    responseModel.TotalCount = result.Count();

                    int serialNo = 1;
                    if (result != null)
                    {
                        if (result.Count() > 0)
                        {
                            foreach (var item in result)
                            {
                                kaveriIntegrationDetail = new KaveriIntegrationDetail();

                                kaveriIntegrationDetail.SerialNo = serialNo++;
                                kaveriIntegrationDetail.SROName = String.IsNullOrEmpty(item.SRONameE) ? "" : item.SRONameE;
                                kaveriIntegrationDetail.DistrictName = String.IsNullOrEmpty(item.DistrictNameE) ? "" : item.DistrictNameE;
                                kaveriIntegrationDetail.TotalPropertiesRegistered = item.Total_Properties_Registered.ToString();
                                kaveriIntegrationDetail.Bhoomi = item.Bhoomi.ToString();
                                kaveriIntegrationDetail.E_Swathu = item.E_Swathu.ToString();
                                kaveriIntegrationDetail.UPOR = item.UPOR.ToString();
                                kaveriIntegrationDetail.Mojani = item.Mojini.ToString();
                                //Added by M Rafe on 02/05/2020
                                kaveriIntegrationDetail.EAasthi = item.EAsthi.ToString();

                                kaveriIntegrationDetail.Total_Properties_Registered_Without_Importing = item.Total_Properties_Registered_Without_Importing.ToString();

                                responseModel.KaveriIntegrationDetailList.Add(kaveriIntegrationDetail);
                            }
                        }
                    }
                }
                else
                {

                    var Totalresult = dbContext.USP_RPT_KaveriIntegrationReportDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.DistrictID, model.SROfficeID).ToList();


                    //    var result = dbContext.USP_RPT_KaveriIntegrationReportDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.DistrictID, model.SROfficeID)
                    //.Skip(model.StartLen).Take(model.TotalNum).ToList();

                    //responseModel.TotalCount = dbContext.USP_RPT_KaveriIntegrationReportDetails(model.DateTime_FromDate, model.DateTime_ToDate, model.DistrictID,model.SROfficeID).ToList().Count();
                    //changed by M Rafe on 02/05/2020


                    var result = Totalresult.Skip(model.StartLen).Take(model.TotalNum).ToList();


                    responseModel.TotalCount = Totalresult.Count();


                    int serialNo = 1;
                    if (result != null)
                    {
                        if (result.Count() > 0)
                        {
                            foreach (var item in result)
                            {
                                kaveriIntegrationDetail = new KaveriIntegrationDetail();

                                kaveriIntegrationDetail.SerialNo = serialNo++;
                                kaveriIntegrationDetail.SROName = String.IsNullOrEmpty(item.SRONameE) ? "" : item.SRONameE;
                                kaveriIntegrationDetail.DistrictName = String.IsNullOrEmpty(item.DistrictNameE) ? "" : item.DistrictNameE;
                                kaveriIntegrationDetail.TotalPropertiesRegistered = item.Total_Properties_Registered.ToString();
                                //Commented by SB on 12-12-2019 as discussed with chetan sir
                                //kaveriIntegrationDetail.TotalPropertiesRegistered =
                                //    item.Total_Properties_Registered == 0 ? item.Total_Properties_Registered.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "B" + "','" + item.SROCode + "');><i>" + item.Total_Properties_Registered + "</i></a>";
                                kaveriIntegrationDetail.Bhoomi =
                                    item.Bhoomi == 0 ? item.Bhoomi.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "C" + "','" + item.SROCode + "');><i>" + item.Bhoomi + "</i></a>";
                                kaveriIntegrationDetail.E_Swathu =
                                    item.E_Swathu == 0 ? item.E_Swathu.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "D" + "','" + item.SROCode + "');><i>" + item.E_Swathu + "</i></a>";
                                kaveriIntegrationDetail.UPOR =
                                    item.UPOR == 0 ? item.UPOR.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "E" + "','" + item.SROCode + "');><i>" + item.UPOR + "</i></a>";
                                kaveriIntegrationDetail.Mojani =
                                   item.Mojini == 0 ? item.Mojini.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "F" + "','" + item.SROCode + "');><i>" + item.Mojini + "</i></a>";




                                kaveriIntegrationDetail.EAasthi =
                               item.EAsthi == 0 ? item.EAsthi.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "G" + "','" + item.SROCode + "');><i>" + item.EAsthi + "</i></a>";


                                kaveriIntegrationDetail.Total_Properties_Registered_Without_Importing =
                                    item.Total_Properties_Registered_Without_Importing == 0 ? item.Total_Properties_Registered_Without_Importing.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "H" + "','" + item.SROCode + "');><i>" + item.Total_Properties_Registered_Without_Importing + "</i></a>";

                                responseModel.KaveriIntegrationDetailList.Add(kaveriIntegrationDetail);
                            }
                        }
                    }
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

            return responseModel;
        }

        /// <summary>
        /// OtherTableDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns KI Details Wrapper Model</returns>
        public KIDetailsWrapperModel OtherTableDetails(KaveriIntegrationModel model)
        {
            KIDetailsWrapperModel resModel = new KIDetailsWrapperModel()
            {
                KIDetailsModelList = new List<KIDetailsModel>(),
                TotalCount = 0
            };
            try
            {
                dbContext = new KaveriEntities();
                int count = 1;
                switch (model.ColumnName)
                {
                    case "B":  //USP_RPT_KI_Total_Properties_Registered

                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vTotalPropList = dbContext.USP_RPT_KI_Total_Properties_Registered(model.SROCode, model.DateTime_FromDate,
                           model.DateTime_ToDate).ToList();

                            resModel.KIDetailsModelList = vTotalPropList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = String.Empty,
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();


                            //resModel.TotalCount = dbContext.USP_RPT_KI_Total_Properties_Registered(model.SROCode, model.DateTime_FromDate,
                            //    model.DateTime_ToDate).ToList().Count();

                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vTotalPropList.Count();

                            if (vTotalPropList != null && vTotalPropList.Count > 0)
                                resModel.SROName = vTotalPropList[0].SROName;
                            else
                                resModel.SROName = "-";
                            break;
                        }
                        else
                        {
                            //    var vTotalPropList = dbContext.USP_RPT_KI_Total_Properties_Registered(model.SROCode, model.DateTime_FromDate,
                            //model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();


                            var vTotalPropList_Complete = dbContext.USP_RPT_KI_Total_Properties_Registered(model.SROCode, model.DateTime_FromDate,
model.DateTime_ToDate).ToList();

                            var vTotalPropList = vTotalPropList_Complete.Skip(model.StartLen).Take(model.TotalNum).ToList();


                            resModel.KIDetailsModelList = vTotalPropList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = String.Empty,
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();


                            resModel.TotalCount = vTotalPropList_Complete.Count();

                            if (vTotalPropList != null && vTotalPropList.Count > 0)
                                resModel.SROName = vTotalPropList[0].SROName;
                            else
                                resModel.SROName = "-";
                            break;
                        }

                    case "C": //USP_RPT_KI_Bhoomi
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vBhoomiList = dbContext.USP_RPT_KI_Bhoomi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            resModel.KIDetailsModelList = vBhoomiList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            // resModel.TotalCount = dbContext.USP_RPT_KI_Bhoomi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();

                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vBhoomiList.Count();

                            if (vBhoomiList != null && vBhoomiList.Count > 0)
                                resModel.SROName = vBhoomiList[0].SROName;
                            else
                                resModel.SROName = "-";
                            break;
                        }
                        else
                        {
                            //   var vBhoomiList = dbContext.USP_RPT_KI_Bhoomi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).
                            //Skip(model.StartLen).Take(model.TotalNum).ToList();



                            var vBhoomiList_Complete = dbContext.USP_RPT_KI_Bhoomi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate)
                      .ToList();


                            var vBhoomiList = vBhoomiList_Complete.
                      Skip(model.StartLen).Take(model.TotalNum).ToList();

                            resModel.KIDetailsModelList = vBhoomiList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vBhoomiList_Complete.Count();

                            if (vBhoomiList != null && vBhoomiList.Count > 0)
                                resModel.SROName = vBhoomiList[0].SROName;
                            else
                                resModel.SROName = "-";
                            break;
                        }


                    case "D":  //USP_RPT_KI_ESwathu
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vESwathuList = dbContext.USP_RPT_KI_ESwathu(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            resModel.KIDetailsModelList = vESwathuList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.Referencenumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            //resModel.TotalCount = dbContext.USP_RPT_KI_ESwathu(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();
                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vESwathuList.Count();

                            if (vESwathuList != null && vESwathuList.Count > 0)
                                resModel.SROName = vESwathuList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }
                        else
                        {
                            //var vESwathuList = dbContext.USP_RPT_KI_ESwathu(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();



                            var vESwathuList_Complete = dbContext.USP_RPT_KI_ESwathu(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            var vESwathuList = vESwathuList_Complete.Skip(model.StartLen).Take(model.TotalNum).ToList();

                            resModel.KIDetailsModelList = vESwathuList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.Referencenumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vESwathuList_Complete.Count();

                            if (vESwathuList != null && vESwathuList.Count > 0)
                                resModel.SROName = vESwathuList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }

                    case "E": //USP_RPT_KI_UPOR
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vUPORList = dbContext.USP_RPT_KI_UPOR(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            resModel.KIDetailsModelList = vUPORList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            // resModel.TotalCount = dbContext.USP_RPT_KI_UPOR(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();
                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vUPORList.Count();

                            if (vUPORList != null && vUPORList.Count > 0)
                                resModel.SROName = vUPORList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }
                        else
                        {
                           // var vUPORList = dbContext.USP_RPT_KI_UPOR(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();
                            var vUPORList_Complete = dbContext.USP_RPT_KI_UPOR(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                            var vUPORList = vUPORList_Complete.Skip(model.StartLen).Take(model.TotalNum).ToList();

                            resModel.KIDetailsModelList = vUPORList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vUPORList_Complete.Count();

                            if (vUPORList != null && vUPORList.Count > 0)
                                resModel.SROName = vUPORList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }


                    //Added by shubham bhagat on 7-11-2019 for Mojani 
                    case "F": //USP_RPT_KI_Mojani
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vUSP_RPT_KI_MojaniList = dbContext.USP_RPT_KI_Mojani(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                            resModel.KIDetailsModelList = vUSP_RPT_KI_MojaniList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            //resModel.TotalCount = dbContext.USP_RPT_KI_Mojani(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();
                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vUSP_RPT_KI_MojaniList.Count();

                            if (vUSP_RPT_KI_MojaniList != null && vUSP_RPT_KI_MojaniList.Count > 0)
                                resModel.SROName = vUSP_RPT_KI_MojaniList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }
                        else
                        {
                           // var vUSP_RPT_KI_MojaniList = dbContext.USP_RPT_KI_Mojani(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();
                            var vUSP_RPT_KI_MojaniList_Complete = dbContext.USP_RPT_KI_Mojani(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            var vUSP_RPT_KI_MojaniList = vUSP_RPT_KI_MojaniList_Complete.Skip(model.StartLen).Take(model.TotalNum).ToList();
                            resModel.KIDetailsModelList = vUSP_RPT_KI_MojaniList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = x.ReferenceNumber.ToString(),
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    UploadDate = x.UploadDate.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vUSP_RPT_KI_MojaniList_Complete.Count();

                            if (vUSP_RPT_KI_MojaniList != null && vUSP_RPT_KI_MojaniList.Count > 0)
                                resModel.SROName = vUSP_RPT_KI_MojaniList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }




                    case "G":  //USP_RPT_KI_EAsthi
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vUSP_RPT_KI_EAsthiList = dbContext.USP_RPT_KI_EAsthi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                            resModel.KIDetailsModelList = vUSP_RPT_KI_EAsthiList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    UploadDate = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),

                                }).ToList();

                            resModel.TotalCount = vUSP_RPT_KI_EAsthiList.Count();

                            if (vUSP_RPT_KI_EAsthiList != null && vUSP_RPT_KI_EAsthiList.Count > 0)
                                resModel.SROName = vUSP_RPT_KI_EAsthiList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }
                        else
                        {
                            // var vUSP_RPT_KI_EAsthiList = dbContext.USP_RPT_KI_EAsthi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();


                            var vUSP_RPT_KI_EAsthiList_complete = dbContext.USP_RPT_KI_EAsthi(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                            var vUSP_RPT_KI_EAsthiList = vUSP_RPT_KI_EAsthiList_complete.Skip(model.StartLen).Take(model.TotalNum).ToList();
                            resModel.KIDetailsModelList = vUSP_RPT_KI_EAsthiList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    UploadDate = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vUSP_RPT_KI_EAsthiList_complete.Count(); 

                            if (vUSP_RPT_KI_EAsthiList != null && vUSP_RPT_KI_EAsthiList.Count > 0)
                                resModel.SROName = vUSP_RPT_KI_EAsthiList[0].SROName;
                            else
                                resModel.SROName = "-";

                            break;
                        }

                    case "H":
                        //USP_RPT_KI_Total_Properties_Regi_Without_Importing
                        if (model.IsForExcelDownload || model.IsForSearch)
                        {
                            var vTotal_Properties_Regi_Without_ImportingList = dbContext.USP_RPT_KI_Total_Properties_Regi_Without_Importing(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                            resModel.KIDetailsModelList = vTotal_Properties_Regi_Without_ImportingList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    UploadDate = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            //  resModel.TotalCount = dbContext.USP_RPT_KI_Total_Properties_Regi_Without_Importing(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList().Count();

                            //Added by M Rafe on 02/05/2020
                            resModel.TotalCount = vTotal_Properties_Regi_Without_ImportingList.Count();

                            if (vTotal_Properties_Regi_Without_ImportingList != null && vTotal_Properties_Regi_Without_ImportingList.Count > 0)
                                resModel.SROName = vTotal_Properties_Regi_Without_ImportingList[0].SRONameE;
                            else
                                resModel.SROName = "-";

                            break;
                        }
                        else
                        {
                           // var vTotal_Properties_Regi_Without_ImportingList = dbContext.USP_RPT_KI_Total_Properties_Regi_Without_Importing(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();
                            var vTotal_Properties_Regi_Without_ImportingList_Complete = dbContext.USP_RPT_KI_Total_Properties_Regi_Without_Importing(model.SROCode, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                            var vTotal_Properties_Regi_Without_ImportingList = vTotal_Properties_Regi_Without_ImportingList_Complete.Skip(model.StartLen).Take(model.TotalNum).ToList();
                            resModel.KIDetailsModelList = vTotal_Properties_Regi_Without_ImportingList.Select(
                                x => new KIDetailsModel()
                                {
                                    SerialNo = count++,
                                    DocumentNumber = x.DocumentNumber.ToString(),
                                    PropertyDetails = String.IsNullOrEmpty(x.Property_Details) ? String.Empty : x.Property_Details,
                                    Executant = String.IsNullOrEmpty(x.Executant) ? String.Empty : x.Executant,
                                    Claimant = String.IsNullOrEmpty(x.Claimant) ? String.Empty : x.Claimant,
                                    Reference_AcknowledgementNumber = String.Empty,
                                    UploadDate = String.Empty,
                                    FinalRegistrationNumber = x.Final_Registration_Number.ToString(),
                                    IntegrationDepartmentName = x.DepartmentName.ToString(),
                                    NatureOfDocument = x.NatureOfDocument.ToString(),
                                    VillageName = x.VillageName.ToString(),
                                }).ToList();

                            resModel.TotalCount = vTotal_Properties_Regi_Without_ImportingList_Complete.Count();

                            if (vTotal_Properties_Regi_Without_ImportingList != null && vTotal_Properties_Regi_Without_ImportingList.Count > 0)
                                resModel.SROName = vTotal_Properties_Regi_Without_ImportingList[0].SRONameE;
                            else
                                resModel.SROName = "-";

                            break;
                        }


                }
                return resModel;
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