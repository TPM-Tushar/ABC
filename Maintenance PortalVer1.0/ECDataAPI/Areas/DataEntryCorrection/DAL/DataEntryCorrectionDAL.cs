#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsDAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   DAL layer for Support Enclosure
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
using System.Transactions;
using System.Configuration;
using System.Net;

namespace ECDataAPI.Areas.SupportEnclosure.DAL
{
    public class DataEntryCorrectionDAL : IDataEntryCorrection
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public DataEntryCorrectionViewModel LoadInsertUpdateDeleteView(int OfficeID)
        {
            return new DataEntryCorrectionViewModel();
        }


        public List<DataEntryCorrectionOrderTableModel> LoadDocDetailsTable(int DROCode, int SROCode, int RoleID, bool IsExcel)
        {
            int SNo = 0;
            try
            {
                dbContext = new KaveriEntities();

                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                List<DataEntryCorrectionOrderTableModel> OTL = new List<DataEntryCorrectionOrderTableModel>();
                List<USP_DEC_ORDER_LIST_Result> ODList = dbContext.USP_DEC_ORDER_LIST(DROCode, SROCode).ToList();
                if (ODList != null)
                {
                    //if (RoleID == (int)ApiCommonEnum.RoleDetails.AIGRComp || RoleID == (int)ApiCommonEnum.RoleDetails.SR || IsExcel)
                    //    ODList = ODList.Where(m => m.IsFinalized == 1).ToList();

                    //Added by mayank on 06/09/2021 to get only finalized order
                    //if ((RoleID != (int)ApiCommonEnum.RoleDetails.DR) || IsExcel)
                    //    ODList = ODList.Where(m => m.IsFinalized == 1).ToList();

                    //Commented by Madhusoodan on 16/12/2021 to show status to every login (Involved in DEC)
                    //if (RoleID == (int)ApiCommonEnum.RoleDetails.DR || RoleID == (int)ApiCommonEnum.RoleDetails.SR)
                    //{
                    //    if (IsExcel)
                    //    {
                    //       // ODList = ODList.Where(m => m.IsFinalized == 1).ToList();
                    //    }
                    //}
                    //else
                    //{
                    //    if (IsExcel)
                    //        ODList = ODList.Where(m => m.IsFinalized == 1).ToList();
                    //}

                }
                foreach (var a in ODList)
                {
                    SNo++;
                    string OD = a.OrderDate.ToString("dd-MM-yyyy");
                    //IsFinalized changed by mayank and fetched from SP result
                    //int finalize = dbContext.DEC_DROrderMaster.Where(y => y.OrderNumber == a.DROrderNumber).Select(z => z.IsFinalized).FirstOrDefault();

                    //Added by Madhusoodan on 10/08/2021 for adding tooltip for section 68 note
                    DataEntryCorrectionOrderTableModel decDROrderModel = new DataEntryCorrectionOrderTableModel();
                    decDROrderModel.SNo = SNo.ToString();
                    decDROrderModel.SROName = a.SROName;
                    decDROrderModel.DROrderNumber = a.DROrderNumber;
                    decDROrderModel.OrderDate = OD;
                    //decDROrderModel.Section68Note = @"<a data-toggle='tooltip' data-placement='top' title='" + a.Section68Note + "' style='color:black;'><u>" + (a.Section68Note.Length > 10 ? a.Section68Note.Substring(0, 10) + "...</u></a>" : a.Section68Note);
                    //decDROrderModel.Section68Note = @"<a data-toggle='tooltip' data-placement='top' title='" + a.Section68Note + "' style='color:black;'>" + (a.Section68Note.Length > 10 ? a.Section68Note.Substring(0, 10) + "<b>...</b></a>" : a.Section68Note);
                    decDROrderModel.Section68Note = a.Section68Note;
                    //Added by mayank on 02/09/2021 for Excel
                    decDROrderModel.Section68NoteForExcel = a.Section68Note;
                    decDROrderModel.RegistrationNumber = a.RegistrationNumber;
                    decDROrderModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + a.OrderID + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>";
                    //Commented by mayank on 17/09/2021 uncommneted after sp execution
                    decDROrderModel.DistrictName = a.DRONAME;
                    decDROrderModel.EnteredBY = a.ENTERED_BY;

                    if (a.IsFinalized == 1)
                    {
                        //Commented and added by Madhusoodan on 20/08/2021 to show View Index II report for finalized orders
                        //decDROrderModel.Action = "-";
                        decDROrderModel.Action = "<button type ='button' class='btn btn-group-md btn-warning' onclick='ViewIndexIIReport(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='View Index II Report'>View</ button>";
                        decDROrderModel.Status = "Finalized";
                    }
                    else
                    {
                        //decDROrderModel.Action = "<button style='margin: 1px;background: #74cc74;border-color: #5cb85c;margin-right: 13px;' type ='button' class='btn btn-group-md btn-primary' onclick='AddEditOrder(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Edit DR Order'>Edit</ button><button style='margin: 1px;background: #fbb654;border-color: #ec971f;' type ='button' class='btn btn-group-md btn-danger' onclick='DeleteDROrder(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Delete DR Order'>Delete</ button>";
                        //decDROrderModel.Action = "<button style='margin: 1px;margin-right: 13px;' type ='button' class='btn btn-group-md btn-success' onclick='AddEditOrder(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Edit DR Order'>Edit</ button><button style='margin: 1px;background: #BA3215;' type ='button' class='btn btn-group-md btn-danger' onclick='DeleteDROrder(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Delete DR Order'>Delete</ button>";
                        if (RoleID == (int)ApiCommonEnum.RoleDetails.DR)
                        {
                            decDROrderModel.Action = "<button class='btn btn-success'  OnClick='AddEditOrder(\"" + a.OrderID + "\")' data-toggle='tooltip'  title='Edit DR Order'>Edit</button>      ";
                        }
                        //Added by Madhusoodan on 15/12/2021
                        //else
                        else if (RoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        {
                            decDROrderModel.Action = "<button class='btn btn-success'  OnClick='OpenDocumentTab(\"" + a.OrderID + "\")' data-toggle='tooltip'  title='Edit DR Order'>Edit</button>      ";
                            //decDROrderModel.Action += "<button class='btn btn-danger'  OnClick='DeleteDROrder(\"" + a.OrderID + "\")' data-toggle='tooltip'  title='Delete DR Order'>Delete</button>";
                            //Changed by mayank on 05/12/2021 for DEC changes
                            decDROrderModel.Action += "<button class='btn btn-danger'  OnClick='DeleteDROrder(\"" + a.OrderID + "\")' data-toggle='tooltip'  title='Reset DR Order'>Reset</button>";

                        }
                        //Added by Madhusoodan on 15/12/2021
                        else
                        {
                            decDROrderModel.Action = string.Empty;

                        }

                        //decDROrderModel.Action += "<button class='btn btn-danger'  OnClick='DeleteDROrder(\"" + a.OrderID + "\")' data-toggle='tooltip'  title='Delete DR Order'>Delete</button>";
                        //#BA3215
                        if (string.IsNullOrEmpty(a.RegistrationNumber) || a.RegistrationNumber == "-")
                            decDROrderModel.Status = "Pending at SRO to enter Correction Note";
                        else
                            decDROrderModel.Status = "Pending at SRO to finalize";
                    }

                    OTL.Add(decDROrderModel);
                }
                ////

                //if (finalize == 1)
                //    {

                //        //Commented by Madhusoodan on 10/08/2021 for adding tooltip for section 68 note
                //        //OTL.Add(new DataEntryCorrectionOrderTableModel { SNo = SNo.ToString(), SROName = a.SROName, DROrderNumber = a.DROrderNumber, OrderDate = OD, Section68Note = a.Section68Note, RegistrationNumber = a.RegistrationNumber, ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + a.DROrderNumber + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>", Action = "-" });//View icon pdf
                //        OTL.Add(new DataEntryCorrectionOrderTableModel { SNo = SNo.ToString(), SROName = a.SROName, DROrderNumber = a.DROrderNumber, OrderDate = OD, Section68Note = a.Section68Note, RegistrationNumber = a.RegistrationNumber, ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + a.DROrderNumber + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>", Action = "-" });//View icon pdf

                //    }
                //    else
                //    {
                //        //Commented and added OrderID by Madhusoodan on 05/08/2021
                //        //OTL.Add(new DataEntryCorrectionOrderTableModel { SNo = SNo.ToString(), SROName = a.SROName, DROrderNumber = a.DROrderNumber, OrderDate = OD, Section68Note = a.Section68Note, RegistrationNumber = a.RegistrationNumber, ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + a.DROrderNumber + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>", Action = "<button style='margin: 1px;background: #74cc74;border-color: #5cb85c;margin-right: 13px;' type ='button' class='btn btn-group-md btn-primary' onclick='EditBtnClickOrderTable(\"" + a.DROrderNumber + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Edit</ button><button style='margin: 1px;background: #fbb654;border-color: #ec971f;' type ='button' class='btn btn-group-md btn-primary' onclick='DeleteBtnClickOrderTable(\"" + a.DROrderNumber + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>" });//View icon pdf //green-orange-margin
                //        OTL.Add(new DataEntryCorrectionOrderTableModel { SNo = SNo.ToString(), SROName = a.SROName, DROrderNumber = a.DROrderNumber, OrderDate = OD, Section68Note = a.Section68Note, RegistrationNumber = a.RegistrationNumber, ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + a.DROrderNumber + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i></a>", Action = "<button style='margin: 1px;background: #74cc74;border-color: #5cb85c;margin-right: 13px;' type ='button' class='btn btn-group-md btn-primary' onclick='AddEditOrder(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Edit</ button><button style='margin: 1px;background: #fbb654;border-color: #ec971f;' type ='button' class='btn btn-group-md btn-primary' onclick='DeleteBtnClickOrderTable(\"" + a.OrderID + "\")' data-toggle='tooltip' data-placement='top' title='Click here'>Delete</ button>" });//View icon pdf //green-orange-margin
                //    }
                //}
                return OTL;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhur
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

        //Modified by Madhusoodan on 12/08/2021
        //Added by Madhur
        public bool DeleteDECOrder(int orderID)
        {
            try
            {
                dbContext = new KaveriEntities();
                //var decOrderDetails = dbContext.DEC_DROrderMaster.SingleOrDefault(x => x.OrderID == orderID);
                var decOrderDeletionDetails = dbContext.USP_DEC_DeleteNoteFromSRO(orderID).FirstOrDefault();
                if (decOrderDeletionDetails != null)
                {
                    if (decOrderDeletionDetails.STATUS == 1)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception(decOrderDeletionDetails.MESSAGE);
                    }
                }
                else
                {
                    throw new Exception("Error Occured while deleting Order Details");
                }
                //if (decOrderDetails != null)
                //{
                //    dbContext.DEC_DROrderMaster.Remove(decOrderDetails);
                //    dbContext.SaveChanges();
                //    return true;
                //}
                //else
                //{
                //    throw new Exception("Order details doesn't exist.");
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DROrderFilePathResultModel ViewBtnClickOrderTable(int OrderID)
        {
            //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry 
            try
            {
                DROrderFilePathResultModel dROrderFilePathResultModel = new DROrderFilePathResultModel();
                using (dbContext = new KaveriEntities())
                {
                    long Lng_FileID = Convert.ToInt64(OrderID);
                    string filepath = dbContext.DEC_DROrderMaster.Where(y => y.OrderID == OrderID).Select(z => z.Relativepath).FirstOrDefault().ToString();
                    string MaintaincePortalVirtualSitePath = ConfigurationManager.AppSettings["MaintaincePortalVirtualSiteOrders"];

                    MaintaincePortalVirtualSitePath = MaintaincePortalVirtualSitePath + "//Section68";

                    WebClient webClient = new WebClient();
                    Stream strm = webClient.OpenRead(new Uri(MaintaincePortalVirtualSitePath + "//" + filepath));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        strm.CopyTo(ms);
                        dROrderFilePathResultModel.DataEntryCorrectionFileBytes = ms.ToArray();
                    }
                    return dROrderFilePathResultModel;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry

        }


        public DataEntryCorrectionOrderResultModel SaveOrderDetails(DataEntryCorrectionOrderViewModel dataEntryCorrectionOrderViewModel)
        {
            DataEntryCorrectionOrderResultModel dataEntryCorrectionOrderResultModel = new DataEntryCorrectionOrderResultModel();
            try
            {

                using (dbContext = new KaveriEntities())
                {
                    DEC_DROrderMaster dEC_DROrderMaster = null;
                    int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionOrderViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                    string districtNameE = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    //var ofcMasterData = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == dataEntryCorrectionOrderViewModel.OfficeID).Select(x => new { x.Kaveri1Code, x.DistrictID }).FirstOrDefault(); 
                    //int  SROCode = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SROCode).FirstOrDefault();

                    //dEC_DROrderMaster.OrderID = Convert.ToInt32(dbContext.USP_GET_SEQID_DEC_OrderMaster().FirstOrDefault());


                    dEC_DROrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == dataEntryCorrectionOrderViewModel.OrderID).FirstOrDefault();

                    if (dEC_DROrderMaster == null)
                    {
                        dEC_DROrderMaster = new DEC_DROrderMaster();

                        dEC_DROrderMaster.OrderID = dataEntryCorrectionOrderViewModel.OrderID;

                        dEC_DROrderMaster.DistrictCode = Kaveri1Code;

                        dEC_DROrderMaster.OrderNumber = dataEntryCorrectionOrderViewModel.OrderNo;
                        dEC_DROrderMaster.OrderDate = Convert.ToDateTime(dataEntryCorrectionOrderViewModel.OrderDate);
                        dEC_DROrderMaster.IsFinalized = 0;
                        dEC_DROrderMaster.UserID = dataEntryCorrectionOrderViewModel.UserID;
                        dEC_DROrderMaster.IPAddress = dataEntryCorrectionOrderViewModel.IPAddress;
                        dEC_DROrderMaster.InsertDateTime = DateTime.Now;
                        dEC_DROrderMaster.AbsoluteFilePath = dataEntryCorrectionOrderViewModel.FilePath;
                        dEC_DROrderMaster.Relativepath = dataEntryCorrectionOrderViewModel.RelativeFilePath;
                        dEC_DROrderMaster.FileName = dataEntryCorrectionOrderViewModel.ExistingFileName;
                        dEC_DROrderMaster.SROCode = dataEntryCorrectionOrderViewModel.SROfficeID;

                        dEC_DROrderMaster.IsValid = 1;
                        dbContext.DEC_DROrderMaster.Add(dEC_DROrderMaster);
                        dataEntryCorrectionOrderResultModel.ResponseMessage = "Order Details Saved Succesfully";
                    }
                    else
                    {
                        //Code added by mayank on 01/12/2021 for Section 68 Note Change request
                        if (dbContext.DEC_DRPNDNote.Where(m => m.OrderID == dataEntryCorrectionOrderViewModel.OrderID).Any())
                        {
                            dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                            return dataEntryCorrectionOrderResultModel;
                        }
                        if (dbContext.ECPropertySearchKeyValues.Where(m => m.OrderID == dataEntryCorrectionOrderViewModel.OrderID).Any())
                        {
                            dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                            return dataEntryCorrectionOrderResultModel;
                        }
                        if (dbContext.ECNameSearchKeyValues.Where(m => m.OrderID == dataEntryCorrectionOrderViewModel.OrderID).Any())
                        {
                            dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                            return dataEntryCorrectionOrderResultModel;
                        }
                        //Added by mayank on 30/05/2022 as Order got finalised without file existance
                        if (dEC_DROrderMaster.DocumentID != null)
                        {
                            dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                            return dataEntryCorrectionOrderResultModel;
                        }
                        //Added by Madhusoodan on 16/08/2021
                        dEC_DROrderMaster.OrderNumber = dataEntryCorrectionOrderViewModel.OrderNo;
                        dEC_DROrderMaster.OrderDate = Convert.ToDateTime(dataEntryCorrectionOrderViewModel.OrderDate);
                        dEC_DROrderMaster.SROCode = dataEntryCorrectionOrderViewModel.SROfficeID;

                        //Added by Madhusoodan on 16/08/2021
                        //If filepath is null then only update Order No and Order Date
                        if (dataEntryCorrectionOrderViewModel.FilePath != null)
                        {
                            dEC_DROrderMaster.AbsoluteFilePath = dataEntryCorrectionOrderViewModel.FilePath;
                            dEC_DROrderMaster.Relativepath = dataEntryCorrectionOrderViewModel.RelativeFilePath;
                            dEC_DROrderMaster.FileName = dataEntryCorrectionOrderViewModel.ExistingFileName;
                        }

                        dbContext.Entry(dEC_DROrderMaster).State = System.Data.Entity.EntityState.Modified;

                        dataEntryCorrectionOrderResultModel.ResponseMessage = "Order Details updated Successfully";
                    }

                    //Added by Madhusoodan on 17/08/2021 to Update Note_PreparedPart in DEC_DRPNDNote table
                    //if data is present in DEC_DRPNDNote then update Note
                    DEC_DRPNDNote decDRPNDNote = dbContext.DEC_DRPNDNote.Where(x => x.OrderID == dataEntryCorrectionOrderViewModel.OrderID).FirstOrDefault();

                    if (decDRPNDNote != null)
                    {
                        decDRPNDNote.Note_PreparedPart = "as per the order number " + dataEntryCorrectionOrderViewModel.OrderNo + " dated " + dataEntryCorrectionOrderViewModel.OrderDate + " by District Registrar " + districtNameE;
                    }
                    //

                    dbContext.SaveChanges();

                    dataEntryCorrectionOrderResultModel.OrderID = dEC_DROrderMaster.OrderID;
                }

                return dataEntryCorrectionOrderResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(@"E:\UploadImages\KaveriPreRegError\PaymentLog.txt"))
                //{
                //    sw.WriteLine("-------------------------------------------------");
                //    sw.WriteLine("dbEx :: " + dbEx.ToString());
                //    sw.Close();
                //}
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                dataEntryCorrectionOrderResultModel.ErrorMessage = "Entity Error Occured in saving Order Details.";
                return dataEntryCorrectionOrderResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        public string FinalizeDECOrder(int currentOrderID)
        {
            try
            {
                dbContext = new KaveriEntities();

                string finalizeSPResp = dbContext.USP_DEC_Finalize(currentOrderID).FirstOrDefault();

                if (finalizeSPResp == null)
                {

                    DEC_DROrderMaster decDROrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == currentOrderID).FirstOrDefault();

                    if (decDROrderMaster != null)
                    {
                        #region Added by mayank on 30/05/2022 to check order file exist while finalising order
                        if (decDROrderMaster.Relativepath != "--")
                        {
                            string filePath = System.Configuration.ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"] + "\\Section68\\" + decDROrderMaster.Relativepath;
                            if (!File.Exists(filePath))
                                return "Order file doesn't exist.Please ask DR to upload Order file before finalizing Order.";
                        }
                        else
                        {
                            finalizeSPResp = "Please ask DR to upload Order file before finalizing Order.";
                            return finalizeSPResp;
                        }
                        #endregion
                        decDROrderMaster.IsFinalized = 1;
                        dbContext.Entry(decDROrderMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    //Added by mayank on 25/08/2021 
                    finalizeSPResp = string.Empty;
                }

                return finalizeSPResp;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        //Added by Madhusoodan on 05/08/2021
        public DataEntryCorrectionOrderViewModel AddEditOrderDetails(int orderID, int OfficeID)
        {
            try
            {
                DataEntryCorrectionOrderViewModel decOrderModel = new DataEntryCorrectionOrderViewModel();
                dbContext = new KaveriEntities();

                //Check isEdit mode from session
                //if orderID is not zero then load existing data for that orderID
                if (orderID != 0)
                {

                    DEC_DROrderMaster decDrOrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == orderID).FirstOrDefault();

                    decOrderModel.OrderNo = decDrOrderMaster.OrderNumber;
                    decOrderModel.OrderDate = decDrOrderMaster.OrderDate == null ? string.Empty : ((DateTime)decDrOrderMaster.OrderDate).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    //Added by Madhusoodan on 11/08/2021 (For order in edit mode)
                    decOrderModel.RelativeFilePath = decDrOrderMaster.Relativepath;
                    decOrderModel.ExistingFileName = decDrOrderMaster.FileName;
                    decOrderModel.IsOrderInEditMode = true;
                    decOrderModel.SROfficeID = Convert.ToInt32(decDrOrderMaster.SROCode);
                    //
                }
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                decOrderModel.SROfficeList = new List<SelectListItem>();

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
                    decOrderModel.SROfficeList.Add(sroNameItem);
                }
                else if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(mas_OfficeMaster.Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    decOrderModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(mas_OfficeMaster.Kaveri1Code, "Select");
                }
                else
                {
                    SelectListItem ItemAll = new SelectListItem();
                    ItemAll.Text = "Select";
                    ItemAll.Value = "0";
                    decOrderModel.SROfficeList.Add(ItemAll);
                }
                //decOrderModel.SROfficeList=
                return decOrderModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        //Added by Madhusoodan on 11/08/2021 (To take Order ID to save file)
        public DROrderFilePathResultModel GenerateNewOrderID(int OfficeID, int currentOrderID)
        {
            try
            {
                //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                DROrderFilePathResultModel dROrderFilePathResultModel = new DROrderFilePathResultModel();
                string rootPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"];
                rootPath = rootPath + "\\Section68";
                //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry

                dbContext = new KaveriEntities();

                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                string districtTriLetter = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.ShortNameE).FirstOrDefault();

                //if currentOrderID is not zero then generate new OrderID as it'll be new DR Orders otherwise it user is uploading new file for same Order No. by deleting earlier one.
                if (currentOrderID == 0)
                {
                    
                    int newOrderID = Convert.ToInt32(dbContext.USP_DEC_GetSequenceID_145(1).FirstOrDefault());

                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //string fileName = "Order_" + districtTriLetter + "_" + newOrderID + ".pdf";
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    string fileName = "Order_" + districtTriLetter + "_" + newOrderID + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+ ".pdf";
                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    #region 
                    //Added by Shivam B for upload DR Order pdf in section 68 for current financial year on 24/05/2022.

                    //Commented and added by Shivam B for Financial Year in section68 on 24/05/2022  
                    //finYear = Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.AddYears(1).Year).Substring(2, 2);

                    DateTime currentDateTime = DateTime.Now;
                    //string year = currentDateTime.Year.ToString();
                    int month = currentDateTime.Month;
                    
                    string finYear;
                    if (month > 3)
                    {
                        finYear = Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.AddYears(1).Year).Substring(2, 2);
                    }
                    else
                    {
                        
                        finYear = Convert.ToString(DateTime.Now.Year - 1) + "-" + Convert.ToString(DateTime.Now.Year).Substring(2, 2);
                    }
                    
                    #endregion //Added by Shivam B for upload DR Order pdf in section 68 for current financial year on 24/05/2022.


                    dROrderFilePathResultModel.RelativeFilePath = "\\" + districtTriLetter + "\\" + finYear + "\\" + newOrderID;
                    dROrderFilePathResultModel.FileName = fileName;
                    dROrderFilePathResultModel.NewOrderID = newOrderID;

                    //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                    dROrderFilePathResultModel.rootPath = rootPath;
                    //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                    
                }
                else
                {
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //string fileName = "Order_" + districtTriLetter + "_" + currentOrderID + ".pdf";
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    string fileName = "Order_" + districtTriLetter + "_" + currentOrderID + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".pdf";
                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    #region 
                    //Added by Shivam B for upload DR Order pdf in section 68 for current financial year on 24/05/2022.

                    //Commented and added by Shivam B for Financial Year in section68 on 24/05/2022
                    //string finYear = Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.AddYears(1).Year).Substring(2, 2);
                    
                    var DBDetails = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == currentOrderID).Select(x => new { x.IsFinalized, x.AbsoluteFilePath, x.Relativepath, x.FileName, x.InsertDateTime,x.DocumentID }).FirstOrDefault();
                    
                    
                    int month = DBDetails.InsertDateTime.Month;
                    string finYear;
                    if (month > 3)
                    {
                        finYear = Convert.ToString(DBDetails.InsertDateTime.Year) + "-" + Convert.ToString(DBDetails.InsertDateTime.AddYears(1).Year).Substring(2, 2);
                    }
                    else
                    {
                        finYear = Convert.ToString(DBDetails.InsertDateTime.Year - 1) + "-" + Convert.ToString(DBDetails.InsertDateTime.Year).Substring(2, 2);
                    }

                    #endregion //Added by Shivam B for upload DR Order pdf in section 68 for current financial year on 24/05/2022.


                    dROrderFilePathResultModel.RelativeFilePath = "\\" + districtTriLetter + "\\" + finYear + "\\" + currentOrderID;
                    dROrderFilePathResultModel.FileName = fileName;
                    dROrderFilePathResultModel.NewOrderID = currentOrderID;
                    //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                    dROrderFilePathResultModel.rootPath = rootPath;
                    //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                    
                    

                    #region
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //Added by Shivam B for uploading of DR order pdf. If pdf is already present in path delete it and then upload new pdf there in section68 on 24/05/2022
                    #region
                    //string completeReletiveFilePath = dROrderFilePathResultModel.RelativeFilePath + "\\" + dROrderFilePathResultModel.FileName;

                    //string filePathToDelete = dROrderFilePathResultModel.rootPath + completeReletiveFilePath;

                    //if (DBDetails.IsFinalized == 0)
                    //{
                    //    if (DBDetails.AbsoluteFilePath == "--" && DBDetails.Relativepath == "--" && DBDetails.FileName == "--" )
                    //    //if (DBDetails.AbsoluteFilePath == "--" && DBDetails.Relativepath == "--" && DBDetails.FileName == "--" && DBDetails.DocumentID==null)
                    //    {
                    //        //System.IO.File.Delete(filePathToDelete);
                    //        if (System.IO.File.Exists(filePathToDelete))
                    //        {
                    //            System.IO.File.Delete(filePathToDelete);
                    //        }
                    //    }
                    //}
                    #endregion //Added by Shivam B for uploading of DR order pdf. If pdf is already present in path delete it and then upload new pdf there in section68 on 24/05/2022
                    #endregion //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                }

                return dROrderFilePathResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }
        

        public DataEntryCorrectionOrderResultModel DeleteCurrentOrderFile(int orderID)
        {
			//Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
            DataEntryCorrectionOrderResultModel dataEntryCorrectionOrderResultModel = new DataEntryCorrectionOrderResultModel();
            //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
			
			try
            {
                dbContext = new KaveriEntities();

                DEC_DROrderMaster decDROrderMaster = dbContext.DEC_DROrderMaster.Where(x => x.OrderID == orderID).FirstOrDefault();
                
				//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
				//DataEntryCorrectionOrderResultModel dataEntryCorrectionOrderResultModel = new DataEntryCorrectionOrderResultModel();
                //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
				
				if (decDROrderMaster != null)
                {
                    //Code added by mayank on 01/12/2021 for Section 68 Note Change request
                    if (dbContext.DEC_DRPNDNote.Where(m => m.OrderID == orderID).Any())
                    {
                        dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                        return dataEntryCorrectionOrderResultModel;
                    }
                    if (dbContext.ECPropertySearchKeyValues.Where(m => m.OrderID == orderID).Any())
                    {
                        dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                        return dataEntryCorrectionOrderResultModel;
                    }
                    if (dbContext.ECNameSearchKeyValues.Where(m => m.OrderID == orderID).Any())
                    {
                        dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                        return dataEntryCorrectionOrderResultModel;
                    }
                    //Added by mayank on 30/05/2022 as Order got finalised without file existance
                    if (decDROrderMaster.DocumentID != null)
                    {
                        dataEntryCorrectionOrderResultModel.ErrorMessage = "As SR has initiated note entry for this order, Please ask SR to reset his entries on this note immediately, then only you wil be able to modify order related information";
                        return dataEntryCorrectionOrderResultModel;
                    }
					
					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.	
                    //string relativeFilePath = decDROrderMaster.Relativepath;
					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //Before changing below values check SelectDocumentDAL -> CheckOrderFileExists() --> as AbsoluteFilePath string is matched there
                    //also in AddEditOrderDetails.cshtml
                    decDROrderMaster.AbsoluteFilePath = "--";
                    decDROrderMaster.Relativepath = "--";
                    decDROrderMaster.FileName = "--";



                    dbContext.Entry(decDROrderMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //Delete file from File location
                    #region // Added by Shivam B For DR Order Pdf VirtualPathDirectory in Section68 on 23-May-2022 

                    //Comment by Shivam B for DR Order Pdf VirtualPathDirectory in Section68 on 24/05/2022
                    //string filePathToDelete = System.Configuration.ConfigurationManager.AppSettings["KaveriSupportPath"] + "\\" + relativeFilePath;

                    //Delete file from File location
                    //string filePathToDelete = System.Configuration.ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"] + "\\Section68\\" + relativeFilePath;

                    #endregion // Added by Shivam B For DR Order Pdf VirtualPathDirectory in Section68 on 24/05/2022


                    //if (File.Exists(filePathToDelete))
                    //{
                    //    File.Delete(filePathToDelete);
                    //}
                    //return true;
					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
					
                    dataEntryCorrectionOrderResultModel.ResponseMessage = "Order file Delete Successfully";
                    return dataEntryCorrectionOrderResultModel;
                }
                else
                {
                    dataEntryCorrectionOrderResultModel.ErrorMessage = "Order Details Not found";
                    return dataEntryCorrectionOrderResultModel;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                //throw ex;

                //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                dataEntryCorrectionOrderResultModel.ErrorMessage = "Problem Occured while deleting the pdf";
                return dataEntryCorrectionOrderResultModel;
                //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
            }
        }

        //Added by mayank on 13/08/2021
        public bool CheckifOrderNoExist(string OrderNo, int OrderID, int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                int DistrictCode = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(m => m.Kaveri1Code).FirstOrDefault();
                if (OrderID != 0)
                {
                    return dbContext.DEC_DROrderMaster.Where(x => x.OrderNumber == OrderNo && x.OrderID != OrderID && x.DistrictCode == DistrictCode).Any();

                }
                else
                {
                    return dbContext.DEC_DROrderMaster.Where(x => x.OrderNumber == OrderNo && x.DistrictCode == DistrictCode).Any();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 20/08/2021 to show Index II report for finalized orders
        public DataEntryCorrectionResultModel LoadIndexIIDetails(int OrderID)
        {
            try
            {
                dbContext = new KaveriEntities();
                DataEntryCorrectionResultModel dataEntryCorrectionResultModel = new DataEntryCorrectionResultModel();
                dataEntryCorrectionResultModel.IndexIIReportList = new List<IndexIIReportsDetailsModel>();

                List<USP_DEC_GETECREPORT_Finalized_Result> SelectResults = dbContext.USP_DEC_GETECREPORT_Finalized(OrderID).ToList();
                int SroCode = Convert.ToInt32(dbContext.DEC_DROrderMaster.Where(m => m.OrderID == OrderID).Select(m => m.SROCode).FirstOrDefault());
                string SroName = dbContext.SROMaster.Where(m => m.SROCode == SroCode).Select(m => m.SRONameE).FirstOrDefault();
                string FinalRegistrationNo = string.Empty;
                if (SelectResults != null)
                {
                    FinalRegistrationNo = SelectResults.Select(m => m.FinalRegistrationNumber).FirstOrDefault();
                }
                int Sno = 1;

                foreach (USP_DEC_GETECREPORT_Finalized_Result ResultItem in SelectResults)
                {
                    //int finalize = dbContext.DEC_DROrderMaster.Where(y => y.OrderNumber == ResultItem.DR_ORDER_NO).Select(z => z.IsFinalized).FirstOrDefault();
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
                    IndexIIReportsDetailsModel detailModel = new IndexIIReportsDetailsModel();
                    detailModel.Sno = Sno++;
                    detailModel.FinalRegistrationNumber = ResultItem.FinalRegistrationNumber ?? "--";
                    detailModel.VillageNameE = ResultItem.VillageNameE ?? "--";
                    detailModel.Stamp5Datetime = Convert.ToString(ResultItem.Stamp5DateTime);
                    detailModel.Schedule = ResultItem.PropertySchedule;
                    detailModel.ArticleNameE = ResultItem.ArticleNameE;
                    detailModel.marketvalue = Convert.ToDecimal(ResultItem.MarketValue);
                    detailModel.consideration = ResultItem.Consideration;
                    detailModel.Claimant = ResultItem.Claimant;
                    detailModel.Executant = ResultItem.Executant;
                    detailModel.CDNumber = ResultItem.CDNumber;
                    detailModel.PageCount = Convert.ToString(ResultItem.PageCount);
                    detailModel.PropertyDetails = ResultItem.Description + Section68NoteHTML;
                    detailModel.Section68Note = ResultItem.SECTION68NOTE;
                    detailModel.PropertyDescription = ResultItem.Description;

                    //if (finalize == 1)

                    dataEntryCorrectionResultModel.IndexIIReportList.Add(detailModel);
                }
                dataEntryCorrectionResultModel.IndexIIFinalRegistrationNo = FinalRegistrationNo;
                dataEntryCorrectionResultModel.IndexIISroName = SroName;
                return dataEntryCorrectionResultModel;
            }

            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw;
            }
        }

        public DataEntryCorrectionViewModel DataEntryCorrectionView(int officeID, int LevelID, long UserID)
        {
            try
            {
                DataEntryCorrectionViewModel dataEntryCorrectionViewModel = new DataEntryCorrectionViewModel();
                dataEntryCorrectionViewModel.SROfficeOrderList = new List<SelectListItem>();
                dataEntryCorrectionViewModel.DROfficeOrderList = new List<SelectListItem>();
                dbContext = new KaveriEntities();
                int kaveriCode = dbContext.MAS_OfficeMaster.Where(m => m.OfficeID == officeID).Select(m => m.Kaveri1Code).FirstOrDefault();
                #region Added by mayank District enabled for DEC
                if (LevelID == (int)ApiCommonEnum.LevelDetails.DR)
                {
                    //List<int> DistrictEnabledForDEC = new List<int>();
                    //DistrictEnabledForDEC.Add(2);
                    //DistrictEnabledForDEC.Add(7);
                    //DistrictEnabledForDEC.Add(10);
                    //DistrictEnabledForDEC.Add(13);
                    //DistrictEnabledForDEC.Add(17);
                    //DistrictEnabledForDEC.Add(22);
                    //DistrictEnabledForDEC.Add(24);
                    //DistrictEnabledForDEC.Add(26);
                    //DistrictEnabledForDEC.Add(27);
                    //DistrictEnabledForDEC.Add(35);
                    //List<short> OfficeIDListEnabledForDEC = (from OM in dbContext.MAS_OfficeMaster
                    //                                         join DM in dbContext.DistrictMaster
                    //                                         on OM.Kaveri1Code equals DM.DistrictCode
                    //                                         where OM.OfficeTypeID == 2 &&
                    //                                         DistrictEnabledForDEC.Contains(OM.Kaveri1Code)
                    //                                         select OM.OfficeID
                    //                                   ).ToList();
                    List<short?> OfficeIDListEnabledForDEC = dbContext.USP_DEC_GET_ENABLED_DR().ToList();
                    if (OfficeIDListEnabledForDEC.Contains(Convert.ToInt16(officeID)))
                    {
                        dataEntryCorrectionViewModel.IsDRLoginEnabledforDEC = true;
                    }
                    else
                    {
                        dataEntryCorrectionViewModel.IsDRLoginEnabledforDEC = false;

                    }
                }
                else
                {
                    dataEntryCorrectionViewModel.IsDRLoginEnabledforDEC = false;
                }
                //List<string> UserEnabledforDEC = new List<string>();
                //UserEnabledforDEC.Add("DR.BLR");
                //UserEnabledforDEC.Add("DR.BEL");
                //UserEnabledforDEC.Add("DR.BIJ");
                //UserEnabledforDEC.Add("DR.CDG");
                //UserEnabledforDEC.Add("DR.GLB");
                //UserEnabledforDEC.Add("DR.UKK");
                //UserEnabledforDEC.Add("DR.DKM");
                //UserEnabledforDEC.Add("DR.MYS");
                //UserEnabledforDEC.Add("DR.RCR");
                //UserEnabledforDEC.Add("DR.GDN");
                //string UserName = dbContext.UMG_UserDetails.Where(m => m.UserID == UserID).Select(m => m.UserName).FirstOrDefault();
                //if (UserEnabledforDEC.Contains(UserName))
                //{
                //    dataEntryCorrectionViewModel.IsDRLoginEnabledforDEC = true;
                //}
                //else
                //{
                //    dataEntryCorrectionViewModel.IsDRLoginEnabledforDEC = false;
                //}
                #endregion
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    //string SroName = dbContext.SROMaster.Where(x => x.SROCode == kaveriCode).Select(x => x.SRONameE).FirstOrDefault();
                    var SRODetails = dbContext.SROMaster.Where(x => x.SROCode == kaveriCode).Select(x => new { x.DistrictCode, x.SRONameE }).FirstOrDefault();
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == SRODetails.DistrictCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    dataEntryCorrectionViewModel.DROfficeOrderList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(SRODetails.DistrictCode) });
                    dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem() { Text = SRODetails.SRONameE, Value = kaveriCode.ToString() });
                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == kaveriCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    dataEntryCorrectionViewModel.DROfficeOrderList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(kaveriCode) });
                    dataEntryCorrectionViewModel.SROfficeOrderList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(kaveriCode, "All");

                }
                else
                {
                    dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    dataEntryCorrectionViewModel.DROfficeOrderList = objCommon.GetDROfficesList("All");
                }
                return dataEntryCorrectionViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataEntryCorrectionViewModel GetSroCodebyDistrict(int DroCode)
        {
            try
            {
                DataEntryCorrectionViewModel dataEntryCorrectionViewModel = new DataEntryCorrectionViewModel();
                dataEntryCorrectionViewModel.SROfficeOrderList = new List<SelectListItem>();
                dbContext = new KaveriEntities();
                var SroList = dbContext.SROMaster.Where(m => m.DistrictCode == DroCode).OrderBy(t => t.SRONameE).Select(x => new { x.SRONameE, x.SROCode }).ToList();
                dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem { Text = "All", Value = "0" });

                foreach (var item in SroList)
                {
                    dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem { Text = item.SRONameE, Value = Convert.ToString(item.SROCode) });
                }
                return dataEntryCorrectionViewModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
    }

    
}