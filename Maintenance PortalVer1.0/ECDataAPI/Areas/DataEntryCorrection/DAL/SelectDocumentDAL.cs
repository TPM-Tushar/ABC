#region File Header
/*
    * Project Id        :   -
    * Project Name      :   
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
using System.Globalization;

namespace ECDataAPI.Areas.SupportEnclosure.DAL
{
    public class SelectDocumentDAL : ISelectDocument
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public List<SelectListItem> GetFinancialYearList()
        {
            List<SelectListItem> financialYearList = new List<SelectListItem>();
            DateTime startYear = Convert.ToDateTime(new DateTime(2003, 1, 1)); //("1/1/2000");
            DateTime today = DateTime.Now.AddYears(1);
            string financialYear = null;
            SelectListItem item = new SelectListItem();
            item.Value = "";
            item.Text = "Select";
            while (today.Year >= startYear.Year)
            {
                SelectListItem listItem = new SelectListItem();
                financialYear = string.Format("{0}-{1}", startYear.AddYears(-1).Year.ToString(), startYear.Year.ToString().Substring(2, 2));
                //listItem.Value = financialYear;
                listItem.Value = startYear.AddYears(-1).Year.ToString();
                listItem.Text = financialYear;
                financialYearList.Insert(0, listItem);
                startYear = startYear.AddYears(1);
            }

            financialYearList.Insert(0, item);
            return financialYearList;
        }

        public List<SelectListItem> GetAllBookTypes()
        {
            dbContext = new KaveriEntities();
            List<SelectListItem> bookTypes = null;

            try
            {
                List<BookTypeMaster> bookTypesList = dbContext.BookTypeMaster.ToList();

                //Added by Madhusoodan on 16/08/2021 to remove Book-3, 4, 5 and Power of Attorney (section 33) 
                //bookTypesList.RemoveRange(2, 2);
                bookTypesList.RemoveRange(1, 3);
                bookTypesList.RemoveRange(7, 2);

                bookTypes = bookTypesList.Select(x => new SelectListItem
                {
                    Text = x.BookNameEnglish,
                    Value = x.BookID.ToString()
                }).ToList();

                bookTypes.Insert(0, new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

                return bookTypes;
            }
            catch
            {
                return new List<SelectListItem>();
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        //public DataEntryCorrectionViewModel DataEntryCorrectionView(int OfficeID)
        public DataEntryCorrectionViewModel SelectDocumentTabView(int OfficeID, bool isEditMode, int currentOrderID)
        {
            DataEntryCorrectionViewModel ViewModel = new DataEntryCorrectionViewModel();

            try
            {
                dbContext = new KaveriEntities();
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
                ViewModel.FinancialYear = GetFinancialYearList();
                ViewModel.BookType = GetAllBookTypes();


                //Added by Madhusoodan on 09*/08/2021 to populate Search parameters in edit mode
                if (isEditMode && currentOrderID > 0)
                {
                    dbContext = new KaveriEntities();
                    //List<USP_DEC_GetDOCParameters_Result> getDocParamList = dbContext.USP_DEC_GetDOCParameters(currentOrderID).ToList();
                    var getDocParamList = dbContext.USP_DEC_GetDOCParameters(currentOrderID).FirstOrDefault();

                    if (getDocParamList != null)
                    {
                        ViewModel.DocumentNumber = getDocParamList.DocumentNumber;
                        ViewModel.BookTypeID = getDocParamList.BookID;
                        ViewModel.SROfficeID = getDocParamList.SROCODE;

                        string fYear = getDocParamList.FINYEAR.ToString();

                        foreach (var item in ViewModel.FinancialYear)
                        {
                            if (item.Value == fYear)
                            {
                                item.Selected = true;
                                break;
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
            return ViewModel;
        }

        public DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
            dataEntryCorrectionResultModel.DataEntryCorrectionPropertyDetailList = new List<DataEntryCorrectionPropertyDetailModel>();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    int fyear = Int32.Parse(dataEntryCorrectionViewModel.FinancialYearStr.Split('-')[0]);
                    //dataEntryCorrectionViewModel.SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                    //List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), fyear).ToList();
                    //List<USP_DEC_INDEX2_DETAILS_Result> IndexIIResults = dbContext.USP_DEC_INDEX2_DETAILS(4, 1, "2368", 2004).ToList();
                    //List<USP_DEC_GETECREPORT_Result> ecReportResult = dbContext.USP_DEC_GETECREPORT(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), fyear).ToList();
                    List<USP_DEC_GETECREPORT_Result> ecReportResult = dbContext.USP_DEC_GETECREPORT(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), fyear, dataEntryCorrectionViewModel.OrderID).ToList();

                    int Sno = 1;
                    //foreach (USP_DEC_INDEX2_DETAILS_Result ResultItem in IndexIIResults)
                    foreach (USP_DEC_GETECREPORT_Result ResultItem in ecReportResult)
                    {
                        DataEntryCorrectionPropertyDetailModel detailModel = new DataEntryCorrectionPropertyDetailModel();

                        //Added by Madhusoodan on 06/08/2021
                        //detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='SelectProperty(" + Sno + "," + ResultItem.DocumentID + "," + ResultItem.PropertyID + ")' title='Add Section 68 Note'>Select</ button>";

                        if (ResultItem.IsPropertySelected == 1)
                        {
                            detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='SelectProperty(" + Sno + "," + ResultItem.DocumentID + "," + ResultItem.PropertyID + ")' title='Select this Property'>Select</ button>";
                            detailModel.ButtonPropertyNoDetails = "<button type ='button' class='btn btn-group-md btn-primary PNDdeactive' id='PNDTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPropertyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Property Number Search Key' >Add/View</ button>";
                            detailModel.ButtonPartyDetails = "<button type ='button' class='btn btn-group-md btn-primary Partydeactive' id='PartyTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPartyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Party Search Key' >Add/View</ button>";
                        }
                        else
                        {
                            detailModel.Select = "<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='SelectProperty(" + Sno + "," + ResultItem.DocumentID + "," + ResultItem.PropertyID + ")' title='Add Section 68(2) Note'>Select</ button>";
                            detailModel.ButtonPropertyNoDetails = "<button type ='button' class='btn btn-group-md btn-primary PNDdeactive' id='PNDTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPropertyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Property Number Search Key' disabled='disabled'>Add/View</ button>";
                            detailModel.ButtonPartyDetails = "<button type ='button' class='btn btn-group-md btn-primary Partydeactive' id='PartyTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPartyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Party Search Key' disabled='disabled'>Add/View</ button>";
                        }

                        //detailModel.ButtonPropertyNoDetails = "<button type ='button' class='btn btn-group-md btn-primary PNDdeactive' id='PNDTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPropertyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Property No Details' disabled>Add/View</ button>";
                        //detailModel.ButtonPartyDetails = "<button type ='button' class='btn btn-group-md btn-primary Partydeactive' id='PartyTabButton" + Sno + "' data-toggle='tooltip' data-placement='top' onclick='LoadPartyPopup(" + ResultItem.DocumentID + "," + ResultItem.PropertyID + "," + Sno + ")' title='Add/View Party Details' disabled>Add/View</ button>";
                        string Section68Note = string.Empty;
                        string Section68NoteHTML = string.Empty;
                        if (!string.IsNullOrEmpty(ResultItem.SECTION68NOTE))
                        {
                            string[] Section68NoteList = ResultItem.SECTION68NOTE.Split(new string[] { "#*#" }, StringSplitOptions.None);

                            foreach (string str in Section68NoteList)
                            {
                                Section68Note += str + "<br>";
                            }
                        }
                        if (!string.IsNullOrEmpty(Section68Note))
                        {
                            Section68NoteHTML = "<table border='1' style='font-family: KNB-TTUmaEN;'><tbody><tr><td style='text-align:left;'><label style='color: black;font-size:medium'><b>Note: </b></label><br><span style='color: black'><b>" + Section68Note + "</b></span></td></tr></tbody></table>";
                        }
                        

                        detailModel.SerialNo = Sno++;
                        //detailModel.ScheduleDescription = ResultItem.Description + " <table border="1" style="font - family: KNB - TTUmaEN; "><tbody><tr><td style="text - align:left; "><label style=''color:black;font-size:large'><b>Note: </b></label><br><span style='color:black'><b></b></span></td></tr></tbody></table>";
                        detailModel.ScheduleDescription = ResultItem.Description + Section68NoteHTML;
                        detailModel.ExecutionDateTime = Convert.ToDateTime(ResultItem.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        detailModel.NatureOfDocument = "Article Name: " + ResultItem.ArticleNameE + "\n Market Value: " + ResultItem.MarketValue + "\n Consideration: " + ResultItem.Consideration;
                        detailModel.Executant = ResultItem.Executant;
                        detailModel.Claimant = ResultItem.Claimant;
                        detailModel.CDNumber = ResultItem.CDNumber;
                        detailModel.PageCount = ResultItem.PageCount;
                        detailModel.FinalRegistrationNo = ResultItem.FinalRegistrationNumber;


                        //detailModel.FinalRegistrationNo = ResultItem.FinalRegistrationNumber;
                        //detailModel.RegistrationDate = Convert.ToString(ResultItem.RegistrationDate);
                        //detailModel.ExecutionDateTime = Convert.ToString(ResultItem.ExecutionDate);
                        //detailModel.NatureOfDocument = ResultItem.NatureOfDocument;
                        //detailModel.PropertyDetails = ResultItem.PropertyDetails;
                        //detailModel.VillageName = ResultItem.VillageName;
                        //detailModel.TotalArea = ResultItem.TotalArea;
                        //detailModel.ScheduleDescription = ResultItem.ScheduleDescription;
                        //detailModel.Marketvalue = ResultItem.marketvalue;
                        //detailModel.Consideration = ResultItem.consideration;
                        //detailModel.Claimant = ResultItem.Claimant;
                        //detailModel.Executant = ResultItem.Executant;

                        //Added by Madhusoodanon 02/08/2021
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


        //Added by Madhur on 27-7-21

        public List<DataEntryCorrectionPreviousPropertyDetailModel> LoadPreviousPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            List<DataEntryCorrectionPreviousPropertyDetailModel> dataEntryCorrectionPreviousPropertyDetailList = new List<DataEntryCorrectionPreviousPropertyDetailModel>();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    //int fyear = Int32.Parse(dataEntryCorrectionViewModel.FinancialYearStr.Split('-')[0]);
                    //dataEntryCorrectionViewModel.SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                    //List<USP_DEC_ORDER_LIST_GIVENDOC_Result> IndexIIResults = dbContext.USP_DEC_ORDER_LIST_GIVENDOC(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.BookTypeID, dataEntryCorrectionViewModel.DocumentNumber.ToString(), fyear).ToList();
                    //List<USP_DEC_ORDER_LIST_GIVENDOC_Result> IndexIIResults = dbContext.USP_DEC_ORDER_LIST_GIVENDOC(4, 1, "0001", 2019).ToList();

                    List<USP_DEC_ORDER_LIST_GIVENProperty_Result> givenPropResult = dbContext.USP_DEC_ORDER_LIST_GIVENProperty(dataEntryCorrectionViewModel.SROfficeID, dataEntryCorrectionViewModel.DocumentID, dataEntryCorrectionViewModel.PropertyID).ToList();

                    int Sno = 1;
                    foreach (USP_DEC_ORDER_LIST_GIVENProperty_Result ResultItem in givenPropResult)
                    {
                        DataEntryCorrectionPreviousPropertyDetailModel detailModel = new DataEntryCorrectionPreviousPropertyDetailModel();
                        detailModel.SerialNo = Sno++;
                        detailModel.DROrderNumber = ResultItem.DROrderNumber;
                        detailModel.OrderUploadDate = Convert.ToDateTime(ResultItem.OrderUploadDate).ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                        detailModel.OrderDate = Convert.ToDateTime(ResultItem.OrderDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        detailModel.Section68Note = Convert.ToString(ResultItem.Section68Note);
                        detailModel.ViewDocument = "<a href='javascript:void(0)' onClick='ViewBtnClickPreviousPropTable(\"" + ResultItem.OrderID + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>";//View icon pdf

                        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
                        if (dataEntryCorrectionViewModel.OrderID == ResultItem.OrderID)
                        {
                            detailModel.Section68NoteDeleteBtn = "<button style='margin: 1px;' type = 'button' class='btn btn-group-md btn-danger' onclick='DeleteSection68Note(\"" + ResultItem.NoteID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>"; //Change Order ID to NoteID
                        }
                        else
                        {
                            detailModel.Section68NoteDeleteBtn = "-";

                        }
                            dataEntryCorrectionPreviousPropertyDetailList.Add(detailModel);
                    }
                }
                return dataEntryCorrectionPreviousPropertyDetailList;
            }

            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                return dataEntryCorrectionPreviousPropertyDetailList;
            }
        }
        //Added by Madhur on 27-7-21



        public string ViewBtnClickPreviousPropTable(int OrderID)
        {
            try
            {
                dbContext = new KaveriEntities();
                string reqModel = dbContext.DEC_DROrderMaster.Where(y => y.OrderID == OrderID).Select(z => z.AbsoluteFilePath).FirstOrDefault().ToString();
                return reqModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //Added by Madhusoodan on 02/08/2021 to save DocumentID and SROCode
        public SelectDocumentResultModel SaveSection68Note(DataEntryCorrectionViewModel decViewModel)
        {
            SelectDocumentResultModel selectDocumenntResultModel = new SelectDocumentResultModel();

            try
            {
                dbContext = new KaveriEntities();

                //int SROCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == decViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                DEC_DROrderMaster decDROrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == decViewModel.OrderID).FirstOrDefault();

                DEC_DRPNDNote decDRPNDNote = new DEC_DRPNDNote();

                decDRPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.OrderID == decViewModel.OrderID && x.PropertyID == decViewModel.PropertyID).FirstOrDefault();

                //Added by Madhusoodan on 17/08/2021 to append Note_PreparedPart
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == decViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //string districtNameE = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                string orderDate = Convert.ToDateTime(decDROrderMaster.OrderDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //Changed by mayank on 05/12/2021 for DEC changes
                string districtNameE = dbContext.DistrictMaster.Where(m => m.DistrictCode == decDROrderMaster.DistrictCode).Select(t => t.DistrictNameE).FirstOrDefault();

                //For DEC_DROrderMaster
                if (decDROrderMaster != null)
                {
                    //Remove later and shift to controller
                    //KaveriSession.Current.OrderID = decDROrderMaster.OrderID;

                    decDROrderMaster.SROCode = decViewModel.SROfficeID;
                    decDROrderMaster.DocumentID = decViewModel.DocumentID;

                    dbContext.Entry(decDROrderMaster).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    selectDocumenntResultModel.ErrorMessage = "Please add a new order to select Document Details.";
                    selectDocumenntResultModel.ResponseMessage = "";
                    return selectDocumenntResultModel;
                }

                //For DEC_DRPNDNote
                if (decDRPNDNote != null) //update (Edit Mode)
                {
                    decDRPNDNote.Note_EnteredPart = decViewModel.Section68NoteForProperty;
                    decDRPNDNote.Note_PreparedPart = "as per the order number " + decDROrderMaster.OrderNumber + " dated " + orderDate + " by District Registrar " + districtNameE;
                    dbContext.Entry(decDRPNDNote).State = System.Data.Entity.EntityState.Modified;

                    selectDocumenntResultModel.ResponseMessage = "Section 68(2) Note updated successfully.";
                }
                else //Add new
                {
                    decDRPNDNote = new DEC_DRPNDNote();

                    //Add entries in DEC_DRPNDNote
                    decDRPNDNote.NoteID = Convert.ToInt32(dbContext.USP_DEC_GetSequenceID_23(2).FirstOrDefault());  //2 -> Table Id for DEC_DRPNDNote
                    decDRPNDNote.OrderID = decViewModel.OrderID;
                    decDRPNDNote.PropertyID = decViewModel.PropertyID;
                    decDRPNDNote.Note_EnteredPart = decViewModel.Section68NoteForProperty;
                    decDRPNDNote.Note_PreparedPart = "as per the order number " + decDROrderMaster.OrderNumber + " dated " + orderDate + " by District Registrar " + districtNameE;
                    decDRPNDNote.UserID = decViewModel.UserID;
                    decDRPNDNote.IPAddress = decViewModel.IPAddress;
                    decDRPNDNote.InsertDateTime = DateTime.Now;

                    dbContext.DEC_DRPNDNote.Add(decDRPNDNote);

                    selectDocumenntResultModel.ResponseMessage = "Section 68(2) Note saved successfully.";
                }

                using (TransactionScope tScope = new TransactionScope())
                {
                    dbContext.SaveChanges();
                    tScope.Complete();

                    //selectDocumenntResultModel.ResponseMessage = "Section 68 Note saved successfully.";
                }

                return selectDocumenntResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                selectDocumenntResultModel.ErrorMessage = ex.Message;
                selectDocumenntResultModel.ResponseMessage = string.Empty;
                return selectDocumenntResultModel;
            }


        }

        public Section68NoteResultModel LoadPreviousSec68Note(int orderID, long propertyID, int officeID)
        {
            Section68NoteResultModel section68NoteResModel = new Section68NoteResultModel();
            try
            {
                dbContext = new KaveriEntities();

                //Added by Madhusoodan on 11/08/2021 (to get Order Date for prefilled text)
                //string orderDate = Convert.ToString(dbContext.DEC_DROrderMaster.Where(x => x.OrderID == orderID).Select(x => x.OrderDate).FirstOrDefault());

                //Added by Madhusoodan on 12/08/2021 to append District Registrar in prefillled text
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //string districtNameE = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                DEC_DROrderMaster decDROrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == orderID).FirstOrDefault();
                string orderDate = Convert.ToDateTime(decDROrderMaster.OrderDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Changed by mayank on 05/12/2021 for DEC changes
                string districtNameE = dbContext.DistrictMaster.Where(m => m.DistrictCode == decDROrderMaster.DistrictCode).Select(t=>t.DistrictNameE).FirstOrDefault();
                DEC_DRPNDNote decDrPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.OrderID == orderID && x.PropertyID == propertyID).FirstOrDefault();

                if (decDrPNDNote != null) 
                {
                    section68NoteResModel.previousSection68Note = decDrPNDNote.Note_EnteredPart ?? "-";
                    section68NoteResModel.ResponseMessage = "You have previously added a Section 68(2) Note for this property. Please check below.";
                    //Added by Madhusoodan on 17/08/2021
                    section68NoteResModel.Sec68NotePreparedPart = "+ as per the order number " + decDROrderMaster.OrderNumber + " dated " + orderDate + " by District Registrar " + districtNameE;
                }
                else
                {
                    section68NoteResModel.ResponseMessage = string.Empty;
                    //Added by Madhusoodan on 11/08/2021 (to get Order Date for prefilled text)
                    //section68NoteResModel.previousSection68Note = "as per the order number " + decDROrderMaster.OrderNumber + " dated " + orderDate + " by District Registrar " + districtNameE;
                    //Added by Madhusoodan on 17/08/2021
                    section68NoteResModel.previousSection68Note = "";
                    section68NoteResModel.Sec68NotePreparedPart = "+ as per the order number " + decDROrderMaster.OrderNumber + " dated " + orderDate + " by District Registrar " + districtNameE;

                }

                return section68NoteResModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                section68NoteResModel.ErrorMessage = ex.Message;
                return section68NoteResModel;
            }
        }

        public bool CheckOrderFileExists(int currentOrderID)
        {
            try
            {
                dbContext = new KaveriEntities();

                string isFilePathPresent = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == currentOrderID).Select(x => x.AbsoluteFilePath).FirstOrDefault();

                if (isFilePathPresent != "--")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        public bool DeleteSection68Note(int NoteID)
        {
            try
            {
                dbContext = new KaveriEntities();
                var decDRPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.NoteID == NoteID).FirstOrDefault();

                if (decDRPNDNote != null)
                {
                    dbContext.DEC_DRPNDNote.Remove(decDRPNDNote);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Section 68(2) Note doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        public bool IsSection68NoteAddedForOrderID(int currentOrderID)
        {
            try
            {
                dbContext = new KaveriEntities();

                var decDRPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.OrderID == currentOrderID).FirstOrDefault();

                if (decDRPNDNote != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }
		
		//Added by mayank on 12/08/2021
        public bool CheckifOrderNoteExist(int OrderId, long PropertyID)
        {
            try
            {
                dbContext = new KaveriEntities();
                return dbContext.DEC_DRPNDNote.Where(x => x.OrderID == OrderId && x.PropertyID == PropertyID).Any();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}