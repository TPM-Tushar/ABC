#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   
    * Author Name       :   
    * Creation Date     :   
    * Last Modified By  :   
    * Last Modified On  :   
    * Description       :   
*/
#endregion

using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.DataEntryCorrection;
using CustomModels.Security;
using System.Text;
using System.IO;
using ECDataUI.Session;
using System.Data.Entity.Validation;
using System.Diagnostics;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using System.Transactions;

namespace ECDataAPI.Areas.SupportEnclosure.DAL
{
    public class PropertyNumberDetailsDAL : IPropertyNumberDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public PropertyNumberDetailsViewModel GetPropertyNoDetailsView(int OfficeID, int PropertyID, long OrderId)
        {
            dbContext = new KaveriEntities();
            PropertyNumberDetailsViewModel ViewModel = new PropertyNumberDetailsViewModel()
            {
                PropertyNumberDetailsAddEditModel = new PropertyNumberDetailsAddEditModel()
            };

            try
            {
                //dbContext = new KaveriEntities();
                //OfficeID = 55;   //Remove Hardcoded+
                //Added by mayank on 16/08/2021 for DEC
                ViewModel.PropertyNumberDetailsAddEditModel.SROfficeList = this.getSROfficeListbyOfficeID(OfficeID);//District office


                //Added by Shivam B on 10/05/2022 for District Registrar DropDownList in Section 68
                ViewModel.PropertyNumberDetailsAddEditModel.DROfficeList = this.getDROfficeListbyOfficeID(OfficeID);
                //Added by Shivam B on 10/05/2022 for District Registrar DropDownList in Section 68


                int RegSroCode =Convert.ToInt32(dbContext.DEC_DROrderMaster.Where(m => m.OrderID == OrderId).Select(m => m.SROCode).FirstOrDefault());


                var PropertyJurisdiction = dbContext.PropertyMaster.Where(m => m.PropertyID == PropertyID && m.RegSROCode== RegSroCode).Select(m => new { m.SROCode, m.VillageCode }).FirstOrDefault();


                //Added by Shivam B on 10/05/2022 for Getting value of Current Session District Registrar in Section 68
                int ParentOfficeCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.ParentOfficeID).FirstOrDefault());
                int ParentKaveriCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == ParentOfficeCode).Select(x => x.Kaveri1Code).FirstOrDefault());
                var DistrictName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ParentKaveriCode).Select(x => x.DistrictNameE).FirstOrDefault();
                ViewModel.PropertyNumberDetailsAddEditModel.HiddenDroCode = ParentKaveriCode;
                ViewModel.PropertyNumberDetailsAddEditModel.HidddenDROName = DistrictName;
                var SROName = dbContext.SROMaster.Where(x => x.SROCode == PropertyJurisdiction.SROCode).Select(s => s.SRONameE).FirstOrDefault();
                ViewModel.PropertyNumberDetailsAddEditModel.HiddenSROName = SROName;

                //Added by Shivam B on 10/05/2022 for Getting value of Current Session District Registrar in Section 68


                ViewModel.PropertyNumberDetailsAddEditModel.SroCode = PropertyJurisdiction.SROCode;
                ViewModel.PropertyNumberDetailsAddEditModel.VillageID = PropertyJurisdiction.VillageCode;
                //END of Added by mayank on 16/08/2021 for DEC
                PropertyNumberDetailsAddEditModel HobliDetails = this.GetHobliDetailsOnVillageSroCode(PropertyJurisdiction.VillageCode, PropertyJurisdiction.SROCode);
                ViewModel.PropertyNumberDetailsAddEditModel.HobliID = Convert.ToInt32(HobliDetails.HobliID);
                ViewModel.PropertyNumberDetailsAddEditModel.HobliName = HobliDetails.HobliName;
                ViewModel.PropertyNumberDetailsAddEditModel.VillageList = this.GetVillageListBySroCode(ViewModel.PropertyNumberDetailsAddEditModel.SroCode);

                ViewModel.PropertyNumberDetailsAddEditModel.CurrentPropertyType = objCommon.GetPropertyNumberTypeList();
                ViewModel.PropertyNumberDetailsAddEditModel.OldPropertyType = objCommon.GetPropertyNumberTypeList();
                //ViewModel.SROfficeID =

            }
            catch (Exception ex)
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

        public DataEntryCorrectionResultModel SelectBtnClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
                dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList = new List<DataEntryCorrectionPropertyDetailModel>();

                //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                List<USP_DEC_GET_PROPERTYSearchKeyValues_Result> SelectResults = dbContext.USP_DEC_GET_PROPERTYSearchKeyValues(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.DocumentID, dataEntryCorrectionViewModel.PropertyID).ToList();
                //List<USP_DEC_GET_PROPERTYSearchKeyValues_Result> SelectResults = dbContext.USP_DEC_GET_PROPERTYSearchKeyValues(4, 122256, 203339).ToList();

                //For Deactivate btn //Do this in controller later
                //int sessionOrderID = KaveriSession.Current.OrderID;


                int Sno = 1;
                foreach (USP_DEC_GET_PROPERTYSearchKeyValues_Result ResultItem in SelectResults)
                {
                    int finalize = dbContext.DEC_DROrderMaster.Where(y => y.OrderNumber == ResultItem.DR_ORDER_NO).Select(z => z.IsFinalized).FirstOrDefault();
                    DataEntryCorrectionPropertyDetailModel detailModel = new DataEntryCorrectionPropertyDetailModel();
                    detailModel.SerialNo = Sno++;
                    detailModel.DROrderNumber = ResultItem.DR_ORDER_NO ?? "--";
                    detailModel.OrderDate = ResultItem.OrderDate == null ? "--" : Convert.ToDateTime(ResultItem.OrderDate).ToString("dd/MM/yyyy");
                    detailModel.Village = ResultItem.Village;
                    detailModel.CurrentPropertyType = ResultItem.CurrentPropertyType;
                    detailModel.CurrentNumber = ResultItem.CurrentNumber;
                    detailModel.OldPropertyType = ResultItem.OldPropertyType;
                    detailModel.OldNumber = ResultItem.OldNumber;
                    detailModel.Survey_No = ResultItem.Survey_No.ToString();
                    detailModel.Surnoc = ResultItem.Surnoc;
                    detailModel.Hissa_No = ResultItem.Hissa_No;
                    detailModel.CorrectionNote = ResultItem.CorrectionNote;

                    //if (finalize == 1)

                    if (dataEntryCorrectionViewModel.OrderID != ResultItem.OrderID)
                    {
                        if (ResultItem.IsInUse == 1)
                        {
                            detailModel.Action = "<button type ='button' class='btn btn-group-md btn-primary' onclick='DeactivatePropertyNoDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Deactivate this Property No Detail'>Deactivate</ button>";
                        }
                        else
                        {
                            //Commented added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
                            //detailModel.Action = "--";
                            detailModel.Action = "<button type ='button' class='btn btn-group-md btn-warning' onclick='ActivatePropertyNoDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Activate this Property No Detail'>Activate Again</ button>";
                        }
                    }
                    else
                    {
                        //Changed by Madhusoodan on 05/08/2021
                        //detailModel.Action = "<button style='margin: 1px;background: #74cc74;margin-right: 13px;border-color: #5cb85c;' type = 'button' class='btn btn-group-md btn-primary' onclick='EditBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Edit</ button><button style='margin: 1px;background: #fbb654;border-color: #ec971f;' type = 'button' class='btn btn-group-md btn-primary' onclick='DeleteBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>";
                        //detailModel.Action = "<button style='margin: 1px;background: #74cc74;margin-right: 13px;border-color: #5cb85c;' type = 'button' class='btn btn-group-md btn-primary' onclick='EditBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Edit</ button><button style='margin: 1px;background: #fbb654;border-color: #ec971f;' type = 'button' class='btn btn-group-md btn-primary' onclick='DeleteBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>";
                        string PropertyType = ResultItem.CurrentPropertyType ?? ResultItem.OldPropertyType;
                        string PropertyNO = ResultItem.CurrentNumber ?? ResultItem.OldNumber;
                        //detailModel.Action = "<button style='margin: 1px;' type = 'button' class='btn btn-group-md btn-danger' onclick='DeletePropertyNoDetails(\"" + ResultItem.KeyID + "\",\""+PropertyType+"\",\""+PropertyNO+ "\",\"" + ResultItem.Village + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>";
                        if (ResultItem.IsInUse==1)
                        {
                            detailModel.Action = "<button style='margin: 1px;' type = 'button' class='btn btn-group-md btn-danger' onclick='DeletePropertyNoDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>"; 
                        }
                        else
                        {
                            detailModel.Action = "<button type ='button' class='btn btn-group-md btn-warning' onclick='ActivatePropertyNoDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Activate this Property No Detail'>Activate Again</ button>";
                        }
                    }
                    dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList.Add(detailModel);
                }

                return dataEntryCorrectionResultModel;
            }

            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw;
            }
        }

        //Added by Madhur 29-07-2021
        public DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
            dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList = new List<DataEntryCorrectionPropertyDetailModel>();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    //Commented by Madhusoodan on 02/08/2021
                    //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                    List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), dataEntryCorrectionViewModel.FinancialYearID).ToList();
                    //List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(4, 1, "2368", 2004).ToList();

                    int Sno = 1;
                    foreach (USP_DEC_INDEX2_DETAILS_Result ResultItem in IndexIIResults)
                    {
                        DataEntryCorrectionPropertyDetailModel detailModel = new DataEntryCorrectionPropertyDetailModel();
                        //detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE"+Sno+"' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick("+Sno+")' title='Click here'>Select</ button>";
                        detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + Sno + ")' title='Click here" + Sno + "'>Select</ button>";
                        detailModel.SerialNo = Sno++;
                        detailModel.FinalRegistrationNo = ResultItem.FinalRegistrationNumber;
                        detailModel.RegistrationDate = Convert.ToString(ResultItem.RegistrationDate);
                        detailModel.ExecutionDateTime = Convert.ToString(ResultItem.ExecutionDate);
                        detailModel.NatureOfDocument = ResultItem.NatureOfDocument;
                        detailModel.PropertyDetails = ResultItem.PropertyDetails;
                        detailModel.VillageName = ResultItem.VillageName;
                        detailModel.TotalArea = ResultItem.TotalArea;
                        detailModel.ScheduleDescription = ResultItem.ScheduleDescription;
                        detailModel.Marketvalue = ResultItem.marketvalue;
                        detailModel.Consideration = ResultItem.consideration;
                        detailModel.Claimant = ResultItem.Claimant;
                        detailModel.Executant = ResultItem.Executant;
                        detailModel.DocumentID = ResultItem.DocumentID.ToString();
                        detailModel.PropertyID = ResultItem.PropertyID.ToString();
                        dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList.Add(detailModel);
                    }
                }
                return dataEntryCorrectionResultModel;
            }

            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                return dataEntryCorrectionResultModel;
            }
        }


        public AddPropertyNoDetailsResultModel AddUpdatePropertyNoDetails(PropertyNumberDetailsViewModel pndViewModel)
        {
            AddPropertyNoDetailsResultModel addPropertyNoDetailsResultModel = new AddPropertyNoDetailsResultModel();

            try
            {
                dbContext = new KaveriEntities();

                //DEC_DRPNDNote decDRPNDNote = new DEC_DRPNDNote();
                ECPropertySearchKeyValues eCPropertySearchKeyValues = new ECPropertySearchKeyValues();

                //Kaveri1Code is SROCode
                //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == pndViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                //decDRPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.OrderID == pndViewModel.OrderID).FirstOrDefault();

                //if (decDRPNDNote == null)
                //{
                //    decDRPNDNote = new DEC_DRPNDNote();

                //    //Add entries in DEC_DRPNDNote
                //    decDRPNDNote.NoteID = Convert.ToInt32(dbContext.USP_DEC_GetSequenceID_23(2).FirstOrDefault());  //2 -> Table Id for DEC_DRPNDNote
                //    decDRPNDNote.OrderID = pndViewModel.OrderID;
                //    decDRPNDNote.PropertyID = pndViewModel.PropertyID;
                //    decDRPNDNote.NoteDesc = pndViewModel.PropertyNumberDetailsAddEditModel.Section68NoteForProperty;
                //    decDRPNDNote.UserID = pndViewModel.UserID;
                //    decDRPNDNote.IPAddress = pndViewModel.IPAddress;
                //    decDRPNDNote.InsertDateTime = DateTime.Now;

                //    dbContext.DEC_DRPNDNote.Add(decDRPNDNote);

                //Add entries in ECPropertySearchKeyValues
                eCPropertySearchKeyValues.KeyID = Convert.ToInt64(dbContext.USP_DEC_GetSequenceID_145(4).FirstOrDefault());  //4 -> Table Id for Seq_ECPropertySearchKeyValues
                eCPropertySearchKeyValues.ExecutionDate = DateTime.Now;

                //Added by mayank on 12/08/2021 for EC search available
                eCPropertySearchKeyValues.CurrentNumber = string.Empty;
                eCPropertySearchKeyValues.OldNumber = string.Empty;
                eCPropertySearchKeyValues.Survey_No = 0;
                eCPropertySearchKeyValues.Surnoc = string.Empty;
                eCPropertySearchKeyValues.Hissa_No = string.Empty;
                if (pndViewModel.PropertyNumberDetailsAddEditModel.VillageID == 0)
                {
                    int RegSroCode = Convert.ToInt32(dbContext.DEC_DROrderMaster.Where(m => m.OrderID == pndViewModel.OrderID).Select(m => m.SROCode).FirstOrDefault());

                    pndViewModel.PropertyNumberDetailsAddEditModel.VillageID = 
                        dbContext.PropertyMaster.Where(m => m.PropertyID == pndViewModel.PropertyID && m.RegSROCode==RegSroCode).
                        Select(m => m.VillageCode).FirstOrDefault();
                }
                eCPropertySearchKeyValues.SROCode = Convert.ToInt32(pndViewModel.PropertyNumberDetailsAddEditModel.SroCode);
                eCPropertySearchKeyValues.VillageCode = pndViewModel.PropertyNumberDetailsAddEditModel.VillageID;  //Change this (Take from hidden column in Datatable 1)
                eCPropertySearchKeyValues.PropertyID = pndViewModel.PropertyID;
                eCPropertySearchKeyValues.RegSROCode = pndViewModel.SROfficeID;
                eCPropertySearchKeyValues.DocumentID = pndViewModel.DocumentID;
                //eCPropertySearchKeyValues.NewPropertyID =                   //Keep this columns null
                //eCPropertySearchKeyValues.NewRegSROCode =

                eCPropertySearchKeyValues.CurrentPropertyTypeID = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentPropertyTypeID;

                if (eCPropertySearchKeyValues.CurrentPropertyTypeID == 1) // 1 for current  Survey No
                {
                    //Concatanate Surney No Details and save in CurrentNumber with saving in resp individual columns also
                    string concatenatedSurveyNoDetails = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber + pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar + pndViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber;

                    eCPropertySearchKeyValues.CurrentNumber =IsFocConverter.GetIsfoc( concatenatedSurveyNoDetails);

                    eCPropertySearchKeyValues.Survey_No = Convert.ToInt32(pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber);
                    eCPropertySearchKeyValues.Surnoc = IsFocConverter.GetIsfoc(pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar);
                    eCPropertySearchKeyValues.Hissa_No = IsFocConverter.GetIsfoc(pndViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber);
                }
                else
                {
                    eCPropertySearchKeyValues.CurrentNumber = IsFocConverter.GetIsfoc(pndViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber ?? string.Empty);
                }

                eCPropertySearchKeyValues.OldPropertyTypeID = Convert.ToInt32(pndViewModel.PropertyNumberDetailsAddEditModel.OldPropertyTypeID);

                eCPropertySearchKeyValues.OldNumber =IsFocConverter.GetIsfoc(pndViewModel.PropertyNumberDetailsAddEditModel.OldNumber ?? string.Empty);

                eCPropertySearchKeyValues.OrderID = pndViewModel.OrderID;
                eCPropertySearchKeyValues.IsActivated = 1;
                eCPropertySearchKeyValues.ActionType = "I";
                dbContext.ECPropertySearchKeyValues.Add(eCPropertySearchKeyValues);

                using (TransactionScope tScope = new TransactionScope())
                {
                    dbContext.SaveChanges();
                    tScope.Complete();
                    addPropertyNoDetailsResultModel.ResponseMessage = "Property Number Details added successfully.";
                }
                //}
                //else
                //{
                //    //Update in DEC_DRPNDNOte
                //    //eCPropertySearchKeyValues = dbContext.ECPropertySearchKeyValues.Where(x => x.OrderID == propertyNumberDetailsViewModel.OrderID).FirstOrDefault();
                //}

                return addPropertyNoDetailsResultModel;

            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                addPropertyNoDetailsResultModel.ErrorMessage = "Entity Error Occured during adding data.";
                return addPropertyNoDetailsResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        /// 


        public bool DeletePropertyNoDetails(long keyId)
        {
            try
            {
                dbContext = new KaveriEntities();
                var PropertyNoDetails = dbContext.ECPropertySearchKeyValues.Where(x => x.KeyID == keyId).FirstOrDefault();

                if (PropertyNoDetails != null)
                {
                    dbContext.ECPropertySearchKeyValues.Remove(PropertyNoDetails);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Property Search key doesn't exist");
                }
                //dbContext.DEC_DROrderMaster.Attach(DRON);
                //dbContext.DEC_DROrderMaster.Remove(DRON);
                //dbContext.SaveChanges();
                //int reqModel = dbContext.SaveChanges();
                //return false;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeactivatePropertyNoDetails(long keyId, int OrderId)
        {
            try
            {
                dbContext = new KaveriEntities();
                var PropertyNoDetails = dbContext.ECPropertySearchKeyValues.Where(x => x.KeyID == keyId && x.IsActivated == 1).FirstOrDefault();


                if (PropertyNoDetails != null)
                {

                    //PropertyNoDetails.IsActivated = 0;
                    //PropertyNoDetails.OrderID = OrderId;
                    //dbContext.Entry(PropertyNoDetails).State = System.Data.Entity.EntityState.Modified;

                    //Added by Madhusoodan on 18/08/2021 to search that current number in ECPropertySearchKey table and set flag to 0 for all occurrences
                    //List<long> rowsToBeDeactivated = dbContext.ECPropertySearchKeyValues.Where(x => x.RegSROCode == PropertyNoDetails.RegSROCode && x.CurrentPropertyTypeID == PropertyNoDetails.CurrentPropertyTypeID && x.DocumentID == PropertyNoDetails.DocumentID && x.CurrentNumber == PropertyNoDetails.CurrentNumber).Select(x => x.KeyID).ToList();

                    List<ECPropertySearchKeyValues> rowsToBeDeactivated = dbContext.ECPropertySearchKeyValues.Where(x => x.RegSROCode == PropertyNoDetails.RegSROCode && x.CurrentPropertyTypeID == PropertyNoDetails.CurrentPropertyTypeID && x.DocumentID == PropertyNoDetails.DocumentID && x.CurrentNumber == PropertyNoDetails.CurrentNumber).ToList();

                    if (rowsToBeDeactivated != null)
                    {
                        foreach (var item in rowsToBeDeactivated)
                        {
                            item.IsActivated = 0;
                            item.OrderID = OrderId; //To save from which order it was deactivated
                            item.ActionType = "U";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Property Search key doesn't exist");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 18/08/2021 to activate orderID's
        public bool ActivatePropertyNoDetails(long keyId, int OrderId)
        {
            try
            {
                dbContext = new KaveriEntities();
                var PropNoDetailsToActivate = dbContext.ECPropertySearchKeyValues.Where(x => x.KeyID == keyId && x.IsActivated == 0).FirstOrDefault();


                if (PropNoDetailsToActivate != null)
                {

                    //PropNoDetailsToActivate.IsActivated = 1;
                    //PropNoDetailsToActivate.OrderID = OrderId;
                    //dbContext.Entry(PropNoDetailsToActivate).State = System.Data.Entity.EntityState.Modified;

                    //Added by Madhusoodan on 18/08/2021 to search that current number in ECPropertySearchKey table and set flag to 0 for all occurrences
                    //List<long> rowsToBeDeactivated = dbContext.ECPropertySearchKeyValues.Where(x => x.RegSROCode == PropertyNoDetails.RegSROCode && x.CurrentPropertyTypeID == PropertyNoDetails.CurrentPropertyTypeID && x.DocumentID == PropertyNoDetails.DocumentID && x.CurrentNumber == PropertyNoDetails.CurrentNumber).Select(x => x.KeyID).ToList();

                    List<ECPropertySearchKeyValues> rowsToBeDeactivated = dbContext.ECPropertySearchKeyValues.Where(x => x.RegSROCode == PropNoDetailsToActivate.RegSROCode && x.CurrentPropertyTypeID == PropNoDetailsToActivate.CurrentPropertyTypeID && x.DocumentID == PropNoDetailsToActivate.DocumentID && x.CurrentNumber == PropNoDetailsToActivate.CurrentNumber).ToList();

                    if (rowsToBeDeactivated != null)
                    {
                        foreach (var item in rowsToBeDeactivated)
                        {
                            item.IsActivated = 1;
                            item.OrderID = null; //To make it as it was before deactivation
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Property Search key doesn't exist");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change
        public PropertyNumberDetailsAddEditModel GetSROListByDROCode(int DroCode)
        {
            PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = new PropertyNumberDetailsAddEditModel();
            List<SelectListItem> SROOfficeList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();

                SROOfficeList.Insert(0, objCommon.GetDefaultSelectListItem("Select", "0"));

                SROOfficeList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == DroCode).OrderBy(c => c.ShortNameE).Select(m => new SelectListItem
                {
                    Value = m.SROCode.ToString(),
                    Text = m.SRONameE
                }).ToList());
                propertyNumberDetailsAddEditModel.SROfficeList = SROOfficeList;

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
            return propertyNumberDetailsAddEditModel;
        }
        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change


        public PropertyNumberDetailsAddEditModel GetVillageBySROCode(int SroCode)
        {
            try
            {
                PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = new PropertyNumberDetailsAddEditModel();
                List<SelectListItem> VillageSelectList = new List<SelectListItem>();
                dbContext = new KaveriEntities();

                //Added by Shivam B on 10/05/2022 For Getting VillageList on Change in Sub Registrar
                VillageSelectList.Insert(0, objCommon.GetDefaultSelectListItem("Select", "0"));
                //Added by Shivam B on 10/05/2022 For Getting VillageList on Change in Sub Registrar


                VillageSelectList.AddRange(dbContext.VillageMaster.Where(c => c.SROCode == SroCode).OrderBy(c => c.VillageNameE).Select(m => new SelectListItem { Value = m.VillageCode.ToString(), Text = m.VillageNameE }).ToList());
                propertyNumberDetailsAddEditModel.VillageList = VillageSelectList;
                return propertyNumberDetailsAddEditModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        //Added by Madhur 29-07-2021


        public EditbtnResultModel EditBtnProperty(int orderID)
        {
            try
            {
                EditbtnResultModel result = new EditbtnResultModel();

                //Check isEdit mode from session
                //if orderID is not zero then load existing data for that orderID
                if (orderID != 0)
                {
                    dbContext = new KaveriEntities();

                    ECPropertySearchKeyValues eCPropertySearchKeyValues = dbContext.ECPropertySearchKeyValues.Where(x => x.OrderID == orderID).FirstOrDefault();

                    result.villageID = eCPropertySearchKeyValues.VillageCode.ToString() == null ? string.Empty : eCPropertySearchKeyValues.VillageCode.ToString();
                    result.CurrentPTID = eCPropertySearchKeyValues.CurrentPropertyTypeID.ToString() == null ? string.Empty : eCPropertySearchKeyValues.CurrentPropertyTypeID.ToString();
                    result.CurrentNo = eCPropertySearchKeyValues.CurrentNumber == null ? string.Empty : eCPropertySearchKeyValues.CurrentNumber;
                    result.CurrentSurveyNo = eCPropertySearchKeyValues.Survey_No.ToString() == null ? string.Empty : eCPropertySearchKeyValues.Survey_No.ToString();
                    result.CurrentSurveyNoChar = eCPropertySearchKeyValues.Surnoc == null ? string.Empty : eCPropertySearchKeyValues.Surnoc;
                    result.CurrentHissaNumber = eCPropertySearchKeyValues.Hissa_No == null ? string.Empty : eCPropertySearchKeyValues.Hissa_No;
                    result.OldPTID = eCPropertySearchKeyValues.OldPropertyTypeID.ToString() == null ? string.Empty : eCPropertySearchKeyValues.OldPropertyTypeID.ToString();
                    result.OldNo = eCPropertySearchKeyValues.OldNumber == null ? string.Empty : eCPropertySearchKeyValues.OldNumber;
                    result.Flag = eCPropertySearchKeyValues.CurrentPropertyTypeID.ToString() == "1" ? "1" : "0";
                }
                return result;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }


        public AddPropertyNoDetailsResultModel UpdatePropertyNoDetails(PropertyNumberDetailsViewModel pndViewModel)
        {
            AddPropertyNoDetailsResultModel addPropertyNoDetailsResultModel = new AddPropertyNoDetailsResultModel();

            try
            {
                dbContext = new KaveriEntities();

                //DEC_DRPNDNote decDRPNDNote = new DEC_DRPNDNote();
                if (pndViewModel.PropertyNumberDetailsAddEditModel.keyID != 0)
                {
                    ECPropertySearchKeyValues eCPropertySearchKeyValues = dbContext.ECPropertySearchKeyValues.Where(m => m.KeyID == pndViewModel.PropertyNumberDetailsAddEditModel.keyID).FirstOrDefault();


                    
                    eCPropertySearchKeyValues.VillageCode = pndViewModel.PropertyNumberDetailsAddEditModel.VillageID;  //Change this (Take from hidden column in Datatable 1)
                    eCPropertySearchKeyValues.PropertyID = pndViewModel.PropertyID;
                    eCPropertySearchKeyValues.RegSROCode = pndViewModel.SROfficeID;
                    eCPropertySearchKeyValues.DocumentID = pndViewModel.DocumentID;
                    //eCPropertySearchKeyValues.NewPropertyID =                   //Keep this columns null
                    //eCPropertySearchKeyValues.NewRegSROCode =

                    eCPropertySearchKeyValues.CurrentPropertyTypeID = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentPropertyTypeID;

                    if (eCPropertySearchKeyValues.CurrentPropertyTypeID == 1) // 1 for current  Survey No
                    {
                        //Concatanate Surney No Details and save in CurrentNumber with saving in resp individual columns also
                        string concatenatedSurveyNoDetails = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber + pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar + pndViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber;

                        eCPropertySearchKeyValues.CurrentNumber = concatenatedSurveyNoDetails;

                        eCPropertySearchKeyValues.Survey_No = Convert.ToInt32(pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber);
                        eCPropertySearchKeyValues.Surnoc = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar;
                        eCPropertySearchKeyValues.Hissa_No = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber;
                    }
                    else
                    {
                        eCPropertySearchKeyValues.CurrentNumber = pndViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber;
                    }

                    eCPropertySearchKeyValues.OldPropertyTypeID = Convert.ToInt32(pndViewModel.PropertyNumberDetailsAddEditModel.OldPropertyTypeID);

                    eCPropertySearchKeyValues.OldNumber = pndViewModel.PropertyNumberDetailsAddEditModel.OldNumber;

                    eCPropertySearchKeyValues.OrderID = pndViewModel.OrderID;
                    eCPropertySearchKeyValues.IsActivated = 1;

                    dbContext.ECPropertySearchKeyValues.Add(eCPropertySearchKeyValues);

                    using (TransactionScope tScope = new TransactionScope())
                    {
                        dbContext.Entry(eCPropertySearchKeyValues).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();
                        tScope.Complete();
                        addPropertyNoDetailsResultModel.ResponseMessage = "Property Number Details updated successfully.";
                    }
                }

                return addPropertyNoDetailsResultModel;

            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                addPropertyNoDetailsResultModel.ErrorMessage = "Entity Error Occured during adding data.";
                return addPropertyNoDetailsResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        ////Added by Madhusoodan on 28/07/2021 for Villages dropdown
        ////Changed by mayank on 10/08/2021 for village from office id
        public List<SelectListItem> GetVillageListBySroCode(long SroCode)
        {
            List<SelectListItem> VillageList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                //int KaveriSroCode = Convert.ToInt32(dbContext.DEC_DROrderMaster.Where(x => x.OrderID == OrderID).Select(x => x.SROCode).FirstOrDefault());
                //List<int> SroCodeList = dbContext.SROMaster.Where(m => m.DistrictCode == KaveriDistrictCode).Select(m => m.SROCode).ToList();
                //VillageList.Insert(0, objCommon.GetDefaultSelectListItem("Select", "0"));
                //VillageList.AddRange(dbContext.VillageMaster.OrderBy(c => c.VillageNameE).Where(c=> c.D == Kaveri1Code).Select(m => new SelectListItem { Value = m.VillageCode.ToString(), Text = m.VillageNameE }).ToList());
                VillageList.AddRange(dbContext.VillageMaster.Where(c => c.SROCode == SroCode).OrderBy(c => c.VillageNameE).Select(m => new SelectListItem { Value = m.VillageCode.ToString(), Text = m.VillageNameE }).ToList());
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
            return VillageList;
        }
        //Added by mayank on 16/08/2021
        public List<SelectListItem> getSROfficeListbyOfficeID(long officeID)
        {
            List<SelectListItem> SROList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                //int KaveriDistrictCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID).Select(x => x.Kaveri1Code).FirstOrDefault());
                //SROList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == KaveriDistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
                int ParentOfficeCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID).Select(x => x.ParentOfficeID).FirstOrDefault());
                int ParentKaveriCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == ParentOfficeCode).Select(x => x.Kaveri1Code).FirstOrDefault());
                SROList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == ParentKaveriCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());

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
            return SROList;
        }

        //Added by mayank on 17/08/2021 for HobliDetails



        //Added by Shivam B on 10/05/2022 for Add New Property Number Search Parameter in Section 68
        public List<SelectListItem> getDROfficeListbyOfficeID(long officeID)
        {
            List<SelectListItem> DROList = new List<SelectListItem>();
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();
                //int KaveriDistrictCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID).Select(x => x.Kaveri1Code).FirstOrDefault());
                //SROList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == KaveriDistrictCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
                int ParentOfficeCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID).Select(x => x.ParentOfficeID).FirstOrDefault());
                int ParentKaveriCode = Convert.ToInt32(dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == ParentOfficeCode).Select(x => x.Kaveri1Code).FirstOrDefault());
                //SROList.AddRange(dbContext.SROMaster.Where(c => c.DistrictCode == ParentKaveriCode).OrderBy(c => c.SRONameE).Select(m => new SelectListItem { Value = m.SROCode.ToString(), Text = m.SRONameE }).ToList());
                var DroOffice = dbContext.DistrictMaster.Where(x => x.DistrictCode == ParentKaveriCode).Select(m => new SelectListItem
                {
                    Value = m.DistrictCode.ToString(),
                    Text = m.DistrictNameE
                }).FirstOrDefault(); 


                List<SelectListItem> DroList = new List<SelectListItem>();
                DROList.Insert(0, objCommon.GetDefaultSelectListItem(DroOffice.Text, DroOffice.Value));

                DROList.AddRange(dbContext.DistrictMaster.Select(m => new SelectListItem
                {
                    Value = m.DistrictCode.ToString(),
                    Text = m.DistrictNameE.ToString()
                }).ToList());

                


                //DROList.AddRange(dbContext.DistrictMaster.Where(c => c.DistrictCode == ParentKaveriCode).OrderBy(c => c.DistrictNameE).Select(m => new SelectListItem { Value = m.DistrictCode.ToString(), Text = m.DistrictNameE }).ToList());

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
            return DROList;
        }

        //Added by Shivam B on 10/05/2022 for Add New Property Number Search Parameter in Section 68

        

        public PropertyNumberDetailsAddEditModel GetHobliDetailsOnVillageSroCode(long VillageCode, int SroCode)
        {
            try
            {
                dbContext = new KaveriEntities();
                PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = new PropertyNumberDetailsAddEditModel();
                var HobliDetails = (from HM in dbContext.HobliMaster
                                    join VM in dbContext.VillageMaster
                                    on HM.HobliCode equals VM.HobliCode
                                    where VM.VillageCode == VillageCode
                                    select new
                                    {
                                        HM.HobliNameE,
                                        HM.HobliCode
                                    }).FirstOrDefault();
                propertyNumberDetailsAddEditModel.HobliID = Convert.ToInt32(HobliDetails.HobliCode);
                propertyNumberDetailsAddEditModel.HobliName = HobliDetails.HobliNameE;
                return propertyNumberDetailsAddEditModel;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}