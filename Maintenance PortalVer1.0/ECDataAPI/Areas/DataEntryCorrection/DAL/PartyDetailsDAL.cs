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

namespace ECDataAPI.Areas.DataEntryCorrection.DAL
{
    public class PartyDetailsDAL : IPartyDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();


        public PartyDetailsViewModel GetPartyDetailsView(int OfficeID, int PartyID)
        {
            PartyDetailsViewModel partyViewModel = new PartyDetailsViewModel()
            {
                PartyDetailsAddEditModel = new PartyDetailsAddEditModel()
            };

            try
            {
                //dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                partyViewModel.PartyDetailsAddEditModel.PartyTypeList = objCommon.GetPartyTypeList();
                //Removed by mayank on 17/08/2021
                partyViewModel.PartyDetailsAddEditModel.PartyTypeList.RemoveAt(8);

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
            return partyViewModel;
        }

        public AddPartyDetailsResultModel AddUpdatePartyDetails(PartyDetailsViewModel partyDetailsViewModel)
        {
            AddPartyDetailsResultModel addPartyDetailsResultModel = new AddPartyDetailsResultModel();
            //bool isUpdateOperation = false;  //Use for edit Party Details Purpose
            try
            {
                dbContext = new KaveriEntities();



                //DEC_DRPartyNote decDRPartyNote = new DEC_DRPartyNote();
                ECNameSearchKeyValues ecNameSearchKeyValues = new ECNameSearchKeyValues();
                long VillageID = dbContext.PropertyMaster.Where(m => m.PropertyID == partyDetailsViewModel.PropertyID).Select(m => m.VillageCode).FirstOrDefault();

                //Kaveri1Code is SROCode
                //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == partyDetailsViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                //decDRPartyNote = dbContext.DEC_DRPartyNote.Where(x => x.OrderID == partyDetailsViewModel.OrderID).FirstOrDefault();
                //ecNameSearchKeyValues = dbContext.ECNameSearchKeyValues.Where(x => x.OrderID == 1).FirstOrDefault();

                //if (decDRPartyNote == null)
                //{
                //    decDRPartyNote = new DEC_DRPartyNote();
                //    //Add entries in DEC_DRPartyNote
                //    decDRPartyNote.NoteID = Convert.ToInt32(dbContext.USP_DEC_GetSequenceID_23(3).FirstOrDefault());  //3 -> Table Id for DEC_DRPartyNote
                //    decDRPartyNote.OrderID = partyDetailsViewModel.OrderID;
                //    decDRPartyNote.PropertyID = partyDetailsViewModel.PropertyID;
                //    decDRPartyNote.NoteDesc = partyDetailsViewModel.PartyDetailsAddEditModel.Section68NoteForParty;
                //    decDRPartyNote.UserID = partyDetailsViewModel.UserID;
                //    decDRPartyNote.IPAddress = partyDetailsViewModel.IPAddress;
                //    decDRPartyNote.InsertDateTime = DateTime.Now;

                //    dbContext.DEC_DRPartyNote.Add(decDRPartyNote);

                //Add entries in ECNameSearchKeyValues
                ecNameSearchKeyValues.KeyID = Convert.ToInt64(dbContext.USP_DEC_GetSequenceID_145(5).FirstOrDefault());  //5 -> Table Id for ECNameSearchKeyValues
                //commeneted by mayank on 16/08/2021
                //ecNameSearchKeyValues.ExecutionDate = DateTime.Now;
                
                //ecNameSearchKeyValues.VillageCode = 0;  //Change this (Take from hidden column in Datatable 1) //Remove Hardcoded

                //commeneted by mayank on 16/08/2021
                //ecNameSearchKeyValues. VillageCode = VillageID;
                //ecNameSearchKeyValues.PropertyID = partyDetailsViewModel.PropertyID;
                ecNameSearchKeyValues.DocumentID = partyDetailsViewModel.DocumentID;
                //commeneted by mayank on 16/08/2021
                ecNameSearchKeyValues.SROCode = partyDetailsViewModel.SROfficeID;
                ecNameSearchKeyValues.PartyTypeID = partyDetailsViewModel.PartyDetailsAddEditModel.PartyTypeID;
                    ecNameSearchKeyValues.FirstName =IsFocConverter.GetIsfoc(partyDetailsViewModel.PartyDetailsAddEditModel.FirstName);
                    ecNameSearchKeyValues.MiddleName =  IsFocConverter.GetIsfoc(partyDetailsViewModel.PartyDetailsAddEditModel.MiddleName ?? "");
                    ecNameSearchKeyValues.LastName =  IsFocConverter.GetIsfoc(partyDetailsViewModel.PartyDetailsAddEditModel.LastName ?? "");
                    ecNameSearchKeyValues.OrderID = partyDetailsViewModel.OrderID;
                    ecNameSearchKeyValues.IsActivated = 1;
                    ecNameSearchKeyValues.ActionType = "I";
                
                    dbContext.ECNameSearchKeyValues.Add(ecNameSearchKeyValues);

                    using (TransactionScope tScope = new TransactionScope())
                    {
                        dbContext.SaveChanges();
                        tScope.Complete();
                        addPartyDetailsResultModel.ResponseMessage = "Party Details added successfully.";
                    }
                //}
                //else
                //{
                //    //Update in DEC_DRPartyNote
                //    //ecNameSearchKeyValues = dbContext.ECNameSearchKeyValues.Where(x => x.OrderID == partyDetailsViewModel.OrderID).FirstOrDefault();

                //}

                return addPartyDetailsResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                 ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                addPartyDetailsResultModel.ErrorMessage = "Entity Error Occured during adding data.";
                return addPartyDetailsResultModel;
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


        //Added by Madhur 29-07-2021
        public DataEntryCorrectionResultModel LoadPropertyDetailsPartyTabData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
            dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList = new List<DataEntryCorrectionPropertyDetailModel>();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    //int fyear = Int32.Parse(dataEntryCorrectionViewModel.FinancialYearStr.Split('-')[0]);

                    //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                    List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), dataEntryCorrectionViewModel.FinancialYearID).ToList();
                    //List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(4, 1, "2368", 2004).ToList();
                    int Sno = 1;
                    foreach (USP_DEC_INDEX2_DETAILS_Result ResultItem in IndexIIResults)
                    {
                        DataEntryCorrectionPropertyDetailModel detailModel = new DataEntryCorrectionPropertyDetailModel();
                        detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + Sno + ")' title='Click here'>Select</ button>";
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

        public DataEntryCorrectionResultModel SelectBtnPartyTabClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
                dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList = new List<DataEntryCorrectionPropertyDetailModel>();

                //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //Commneted by mayank 16/08/2021
                //List<Usp_Dec> SelectResults = dbContext.USP_DEC_GET_NameSearchKeyValues(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.DocumentID, dataEntryCorrectionViewModel.PropertyID).ToList();
                List<USP_DEC_GET_NameSearchKeyValues_Result> SelectResults = dbContext.USP_DEC_GET_NameSearchKeyValues(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.DocumentID).ToList();
                //List<USP_DEC_GET_NameSearchKeyValues_Result> SelectResults = dbContext.USP_DEC_GET_NameSearchKeyValues(4, 122256, 203339).ToList();

                //For Deactivate btn //Do this in controller later
                //int sessionOrderID = KaveriSession.Current.OrderID;

                int Sno = 1;
                foreach (USP_DEC_GET_NameSearchKeyValues_Result ResultItem in SelectResults)
                {
                    int finalize = dbContext.DEC_DROrderMaster.Where(y => y.OrderNumber == ResultItem.DR_ORDER_NO).Select(z => z.IsFinalized).FirstOrDefault();
                    DataEntryCorrectionPropertyDetailModel detailModel = new DataEntryCorrectionPropertyDetailModel();
                    detailModel.SerialNo = Sno++;
                    detailModel.DROrderNumber = ResultItem.DR_ORDER_NO;
                    //detailModel.OrderDate = Convert.ToDateTime(ResultItem.OrderDate).ToString("dd/MM/yyyy");
                    detailModel.OrderDate= ResultItem.OrderDate == null ? "--" : Convert.ToDateTime(ResultItem.OrderDate).ToString("dd/MM/yyyy");
                    //detailModel.OrderDate = Convert.ToDateTime(ResultItem.OrderDate).ToString("dd/MM/yyyy");
                    detailModel.PartyType = ResultItem.PartyType;
                    detailModel.FirstName = ResultItem.FirstName;
                    detailModel.MiddleName = ResultItem.MiddleName;
                    detailModel.LastName = ResultItem.LastName;
                    detailModel.CorrectionNote = ResultItem.CorrectionNote;
                    //if (finalize == 1)
                    if (dataEntryCorrectionViewModel.OrderID != ResultItem.OrderID)
                    {
                        //detailModel.Action = "<button type ='button' class='btn btn-group-md btn-primary' onclick='DeactivatePartyDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Deactivate</ button>";
                        //if (ResultItem.IsInUse == 0)
                        //{
                        //    detailModel.Action = "--";
                        //}
                        detailModel.Action = "--";

                    }
                    else
                    {
                        //detailModel.Action = "<button type = 'button' style='background: #74cc74;border-color: #5cb85c;margin-right: 13px;' class='btn btn-group-md btn-primary' onclick='EditBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Edit</ button><button type = 'button' style='background: #fbb654;border-color: #ec971f;' class='btn btn-group-md btn-primary' onclick='DeleteBtnClickOrderTable(\"" + ResultItem.DR_ORDER_NO + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>";
                        detailModel.Action = "<button type = 'button' style='margin: 1px;' class='btn btn-group-md btn-danger' onclick='DeletePartyDetails(\"" + ResultItem.KeyID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>";
                        if (ResultItem.IsInUse == 0)
                        {
                            detailModel.Action = "--";
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

        public string EditBtnClickOrderTable(string DROrderNumber)
        {
            try
            {
                dbContext = new KaveriEntities();
                string reqModel = dbContext.DEC_DROrderMaster.Where(y => y.OrderNumber == DROrderNumber).Select(z => new { z.AbsoluteFilePath, z.OrderDate }).FirstOrDefault().ToString();
                return reqModel;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeletePartyDetails(long KeyId)
        {
            try
            {
                dbContext = new KaveriEntities();
                var DRON = dbContext.ECNameSearchKeyValues.Where(x => x.KeyID == KeyId).FirstOrDefault();

                if (DRON != null)
                {
                    dbContext.ECNameSearchKeyValues.Remove(DRON);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Party Search key doesn't exist");
                }
                //dbContext.DEC_DROrderMaster.Attach(DRON);
                //dbContext.DEC_DROrderMaster.Remove(DRON);
                //dbContext.SaveChanges();

                //return false;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeactivatePartyDetails(long KeyId, int OrderId)
        {
            try
            {
                dbContext = new KaveriEntities();
                var DRON = dbContext.ECNameSearchKeyValues.Where(x => x.KeyID == KeyId && x.IsActivated==1).FirstOrDefault();

                if (DRON != null)
                {
                    DRON.IsActivated = 0;
                    DRON.OrderID = OrderId;
                    DRON.ActionType ="U";
                    dbContext.Entry(DRON).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Party Search key doesn't exist");
                }
                //dbContext.DEC_DROrderMaster.Attach(DRON);
                //dbContext.DEC_DROrderMaster.Remove(DRON);
                //dbContext.SaveChanges();

                //return false;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhur 29-07-2021

    }
}