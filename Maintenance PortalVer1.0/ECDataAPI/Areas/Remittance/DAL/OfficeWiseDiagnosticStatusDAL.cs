#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   OfficeWiseDiagnosticStatusDAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class OfficeWiseDiagnosticStatusDAL : IOfficeWiseDiagnosticStatus
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        /// <summary>
        /// OfficeWiseDiagnosticStatusModelView
        /// </summary>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusModel OfficeWiseDiagnosticStatusModelView()
        {
            OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel();
            try
            {
                model.OfficeTypeList = new List<SelectListItem>();
                model.StatusList = new List<SelectListItem>();
                List<NumberOfErrorsInAction> numberOfErrorsInActionsList = new List<NumberOfErrorsInAction>();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.TilesData = new TilesModel();
                DateTime date = DateTime.Now.Date;
                USP_DiagSummary_Result result = dbContext.USP_DiagSummary(date).FirstOrDefault();
                List<USP_DiagDetail_Result> DetailResult = dbContext.USP_DiagDetail(date).ToList();
                #region new for tiles
                #region Commented by mayank on 09/03/2022
                //TilesModel tilesModel = GetPopupTileData(date);
                //model.TilesData.TotalNo = tilesModel.TotalNo;
                //model.TilesData.StatusAvailabelNo = tilesModel.StatusAvailabelNo;
                //model.TilesData.StatusNotAvailable = tilesModel.StatusNotAvailable;
                //model.TilesData.AllOkNo = tilesModel.AllOkNo;
                //model.TilesData.IssueFoundNo = tilesModel.IssueFoundNo; 
                //model.TilesData.ActionErrorsList = tilesModel.ActionErrorsList;

                #endregion
                if (result != null)
                {
                    model.TilesData.TotalNo = result.TotalOffices;
                    model.TilesData.StatusAvailabelNo = result.DataAvailable;
                    model.TilesData.StatusNotAvailable = result.DataNotAvailable;
                    model.TilesData.AllOkNo = result.AllCheckSuccessful;
                    model.TilesData.IssueFoundNo = result.WhereIssuesFound;
                    model.TilesData.TotalIssueFound = result.TotalIssuesFound;
                    model.TilesData.StatusAvailabelDesc = result.DataAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
                    model.TilesData.StatusNotAvailableDesc = result.DataNotAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataNotAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
                    model.TilesData.AllOkDesc = result.AllCheckSuccessful == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.AllCheckSuccessful) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
                    model.TilesData.IssueFoundNoDesc = result.WhereIssuesFound == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.WhereIssuesFound) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
                }
                else
                {
                    throw new Exception("No Data Received from SP DiagSummary");
                }
                if (DetailResult != null)
                {
                    DetailResult.ForEach(m =>
                    {
                        NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
                        numberOfErrorsInAction.ActionDesc = m.ActionDescription;
                        numberOfErrorsInAction.NumberOfErrors = Convert.ToInt32(m.ISSUECOUNT);
                        numberOfErrorsInActionsList.Add(numberOfErrorsInAction);
                    });
                    model.TilesData.ActionErrorsList = numberOfErrorsInActionsList;
                }
                else
                {
                    throw new Exception("No Data Received from SP DiagDetail");
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }


        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusModel GetOfficeList(String OfficeType)
        {
            OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel();
            model.OfficeTypeList = new List<SelectListItem>();
            SelectListItem selectListItem = new SelectListItem();
            selectListItem.Text = "All";
            selectListItem.Value = "0";
            model.OfficeTypeList.Add(selectListItem);

            if (OfficeType.ToLower().Equals("sro"))
            {
                List<SROMaster> SROMasterList = dbContext.SROMaster.Where(x => x.SROCode > 0).ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            selectListOBJ.Text = item.SRONameE;// + " (" + item.SROCode.ToString() + ")";
                            selectListOBJ.Value = item.SROCode.ToString();
                            model.OfficeTypeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            if (OfficeType.ToLower().Equals("dro"))
            {
                //var DistrictMasterList = dbContext.DistrictMaster.ToList();
                List<DistrictMaster> DistrictMasterList = dbContext.DistrictMaster.ToList();
                DistrictMasterList = DistrictMasterList.OrderBy(x => x.DistrictNameE).ToList();
                if (DistrictMasterList != null)
                {
                    if (DistrictMasterList.Count() > 0)
                    {
                        foreach (var item in DistrictMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            selectListOBJ.Text = item.DistrictNameE;// + " (" + item.DistrictCode.ToString() + ")";
                            selectListOBJ.Value = item.DistrictCode.ToString();
                            model.OfficeTypeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            return model;
        }


        #region Code Commented by Mayank in 09/03/2022
        //public TilesModel GetPopupTileData(DateTime date)
        //{
        //    #region FOR TILES
        //    try
        //    {
        //        TilesModel model = new TilesModel();
        //        List<DiagnosticMaster> diagnosticMastersCopy = new List<DiagnosticMaster>();
        //        List<DiagnosticDetails> diagnosticDetailsCopy = new List<DiagnosticDetails>();
        //        List<DiagnosticMaster> diagnosticMasters = new List<DiagnosticMaster>();

        //        var outerjoinres = from s in dbContext.SROMaster
        //                           join d in dbContext.DiagnosticMaster on s.SROCode equals d.OfficeCode
        //                           into cu
        //                           from co in cu.DefaultIfEmpty()
        //                           where (s.SROCode > 0 && ((co.DiagnosticDate == date) || co.DiagnosticDate == null))
        //                           orderby co.IsSuccessful descending
        //                           select new
        //                           {
        //                               DiagnosticId = (Int32?)co.DiagnosticId,
        //                               SROCode = s.SROCode,
        //                               DiagnosticDate = (DateTime?)co.DiagnosticDate,
        //                               StartTime = (DateTime?)co.DiagnosticDate,
        //                               CompletionTime = (DateTime?)co.CompletionTime,
        //                               RequestIP = co.RequestIP,
        //                               LoginId = co.LoginId,
        //                               IsSuccessful = (short?)co.IsSuccessful,
        //                               IsCentralized = (bool?)co.IsCentralized,
        //                               SecureCode = co.SecureCode,
        //                               isDRO = (Int32?)co.isDRO,
        //                               MasterID = (Int32?)co.MasterID

        //                           };
        //        var res2 = outerjoinres.ToList();
        //        foreach (var item in res2)
        //        {
        //            DiagnosticMaster obj = new DiagnosticMaster();
        //            obj.DiagnosticId = item.DiagnosticId ?? 0;
        //            obj.OfficeCode = item.SROCode;
        //            obj.DiagnosticDate = item.DiagnosticDate ?? DateTime.Now;
        //            obj.StartTime = item.StartTime ?? DateTime.Now;
        //            obj.CompletionTime = item.CompletionTime ?? DateTime.Now;
        //            obj.RequestIP = item.RequestIP;
        //            obj.LoginId = item.LoginId;
        //            obj.IsSuccessful = item.IsSuccessful ?? 0;
        //            obj.IsCentralized = item.IsCentralized ?? false;
        //            obj.SecureCode = item.SecureCode;
        //            obj.isDRO = item.isDRO ?? 0;
        //            obj.MasterID = item.MasterID ?? 0;

        //            diagnosticMasters.Add(obj);
        //        }
        //        model.TotalNo = diagnosticMasters.Count;
        //        List<DiagnosticDetails> diagnosticDetails = new List<DiagnosticDetails>();
        //        if (diagnosticMasters != null && diagnosticMasters.Count > 0)
        //        {
        //            foreach (var item in diagnosticMasters)
        //            {
        //                var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();
        //                foreach (var detail in res)
        //                {
        //                    diagnosticDetails.Add(detail);
        //                }
        //            }
        //        }
        //        //FOR ALL OK(SUCCESS)
        //        List<int> MasterIDs = new List<int>();

        //        diagnosticMastersCopy = diagnosticMasters.ToList();
        //        diagnosticDetailsCopy = diagnosticDetails.ToList();

        //        diagnosticMastersCopy.RemoveAll(m => m.DiagnosticId == 0);
        //        foreach (var item in diagnosticMastersCopy)
        //        {
        //            foreach (var dd in diagnosticDetailsCopy.Where(m => m.MasterID == item.MasterID).ToList())
        //            {
        //                if (dd.ActionStatus == false)
        //                {
        //                    diagnosticDetailsCopy.RemoveAll(m => m.MasterID == item.MasterID);
        //                    //diagnosticMasters.Remove(item);
        //                    MasterIDs.Add(item.MasterID);
        //                    break;
        //                }
        //            }
        //        }
        //        for (int i = 0; i < MasterIDs.Count; i++)
        //        {
        //            diagnosticMastersCopy.Remove(diagnosticMastersCopy.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //        }
        //        model.AllOkNo = diagnosticMastersCopy.Count;
        //        //FOR ALL OK(SUCCESS)

        //        //FOR ISSUED FOUND(FAILED)
        //        MasterIDs.Clear();
        //        diagnosticMastersCopy.Clear();
        //        diagnosticDetailsCopy.Clear();
        //        diagnosticMastersCopy = diagnosticMasters.ToList();
        //        diagnosticDetailsCopy = diagnosticDetails.ToList();
        //        foreach (var item in diagnosticMastersCopy)
        //        {
        //            int count = diagnosticDetailsCopy.Where(m => m.MasterID == item.MasterID).Count();
        //            if (count == 0)
        //            {
        //                MasterIDs.Add(item.MasterID);
        //            }
        //            foreach (var dd in diagnosticDetailsCopy.Where(m => m.MasterID == item.MasterID).ToList())
        //            {
        //                if (dd.ActionStatus == true)
        //                {
        //                    count--;
        //                    if (count == 0)
        //                    {
        //                        diagnosticDetailsCopy.RemoveAll(m => m.MasterID == item.MasterID);
        //                        //diagnosticMasters.Remove(item);
        //                        MasterIDs.Add(item.MasterID);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < MasterIDs.Count; i++)
        //        {
        //            diagnosticMastersCopy.Remove(diagnosticMastersCopy.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //        }
        //        model.IssueFoundNo = diagnosticMastersCopy.Count;
        //        //FOR ISSUED FOUND(FAILED)

        //        //FOR STATUS NOT AVAILABLE(NOT EXECUTED OR NOT FOUND)
        //        MasterIDs.Clear();
        //        diagnosticMastersCopy.Clear();
        //        diagnosticDetailsCopy.Clear();
        //        diagnosticMastersCopy = diagnosticMasters.ToList();
        //        diagnosticDetailsCopy = diagnosticDetails.ToList();
        //        foreach (var item in diagnosticMastersCopy)
        //        {
        //            if (item.DiagnosticId > 0)
        //            {
        //                MasterIDs.Add(item.MasterID);
        //            }
        //        }

        //        for (int i = 0; i < MasterIDs.Count; i++)
        //        {
        //            diagnosticMastersCopy.Remove(diagnosticMastersCopy.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //        }
        //        model.StatusNotAvailable = diagnosticMastersCopy.Count;
        //        //FOR STATUS NOT AVAILABLE

        //        #region STATUS AVAILABEL
        //        model.StatusAvailabelNo = model.TotalNo - model.StatusNotAvailable;
        //        #endregion

        //        //for all desc
        //        model.StatusAvailabelDesc = model.StatusAvailabelNo == 0 ? "0 % of All office" : ((Convert.ToDouble(model.StatusAvailabelNo) / model.TotalNo) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //        model.StatusNotAvailableDesc = model.StatusNotAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(model.StatusNotAvailable) / model.TotalNo) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //        model.AllOkDesc = model.AllOkNo == 0 ? "0 % of Status Available" : ((Convert.ToDouble(model.AllOkNo) / model.StatusAvailabelNo) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
        //        model.IssueFoundNoDesc = model.IssueFoundNo == 0 ? "0 % of Status Available" : ((Convert.ToDouble(model.IssueFoundNo) / model.StatusAvailabelNo) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";

        //        //for number of errors in each action
        //        //SELECT COUNT(*) FROM dbo.DiagnosticDetails DD INNER JOIN DiagnosticMaster DM ON DD.MasterID = DM.MasterID WHERE DD.ActionId = 7 AND DD.ActionStatus = 0 AND DM.DiagnosticDate = '2021-01-14';
        //        //int[] ActionId = new int[] { 3,4,5,6,7,8,9,10,11,12,14};
        //        model.ActionErrorsList = new List<NumberOfErrorsInAction>();
        //        var diagnosticActionMasterList = dbContext.DiagnosticActionMaster.Where(x => x.IsActive == true).ToList();
        //        //int index = 0;
        //        //foreach (var item in diagnosticActionMasterList)
        //        //{
        //        //    NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
        //        //    numberOfErrorsInAction.ActionDesc = item.ActionDescription;
        //        //    numberOfErrorsInAction.NumberOfErrors = (from DD in dbContext.DiagnosticDetails
        //        //                                             join DM in dbContext.DiagnosticMaster on DD.MasterID equals DM.MasterID
        //        //                                             where (DM.DiagnosticDate == date && DD.ActionId == item.ActionId && DD.ActionStatus == false)
        //        //                                             select DD).Count();
        //        //    model.ActionErrorsList.Add(numberOfErrorsInAction);
        //        //    model.TotalIssueFound += numberOfErrorsInAction.NumberOfErrors;

        //        //}
        //        foreach (var item in diagnosticActionMasterList)
        //        {

        //            NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
        //            if (item.ActionId == 4)
        //            {
        //                numberOfErrorsInAction.ActionDesc = item.ActionDescription;
        //                var result = (from DD in dbContext.DiagnosticDetails
        //                              join DM in dbContext.DiagnosticMaster on DD.MasterID equals DM.MasterID
        //                              where (DM.DiagnosticDate == date && DD.ActionId == item.ActionId)
        //                              select DD).ToList();
        //                foreach (var obj in result)
        //                {
        //                    if (obj.ActionStatus == true && obj.ErrorDescription == "")
        //                    {
        //                        if (!string.IsNullOrEmpty(obj.Dignostic_Output))
        //                        {
        //                            string[] lines = obj.Dignostic_Output.Replace("\r", "").Split('\n');
        //                            string ResultLine = lines[lines.Length - 2];
        //                            string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //                            int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //                            int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //                            if (AllocationErrors > 0 || ConsistencyErrors > 0)
        //                            {
        //                                numberOfErrorsInAction.NumberOfErrors = numberOfErrorsInAction.NumberOfErrors + 1;
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        numberOfErrorsInAction.NumberOfErrors += 1;
        //                    }
        //                }
        //                model.ActionErrorsList.Add(numberOfErrorsInAction);
        //                model.TotalIssueFound += numberOfErrorsInAction.NumberOfErrors;

        //            }
        //            else if (item.ActionId == 12)
        //            {
        //                numberOfErrorsInAction.ActionDesc = item.ActionDescription;
        //                var result = (from DD in dbContext.DiagnosticDetails
        //                              join DM in dbContext.DiagnosticMaster on DD.MasterID equals DM.MasterID
        //                              where (DM.DiagnosticDate == date && DD.ActionId == item.ActionId)
        //                              select DD).ToList();

        //                foreach (var obj in result)
        //                {
        //                    if (obj.ActionStatus == true && obj.ErrorDescription == "")
        //                    {
        //                        var dbSpaceDetail = obj.Dignostic_Output;
        //                        var SpaceDetail = dbSpaceDetail.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);
        //                        if (SpaceDetail != null && SpaceDetail.Length == 2)
        //                        {
        //                            var output = SpaceDetail[1].Replace(':', ' ').Trim();

        //                            if (Convert.ToDouble(output) <= 25)
        //                            {
        //                                numberOfErrorsInAction.NumberOfErrors = numberOfErrorsInAction.NumberOfErrors + 1;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        numberOfErrorsInAction.NumberOfErrors += 1;
        //                    }

        //                }

        //                model.ActionErrorsList.Add(numberOfErrorsInAction);
        //                model.TotalIssueFound += numberOfErrorsInAction.NumberOfErrors;
        //            }
        //            else if (item.ActionId == 11)
        //            {
        //                numberOfErrorsInAction.ActionDesc = item.ActionDescription;
        //                var result = (from DD in dbContext.DiagnosticDetails
        //                              join DM in dbContext.DiagnosticMaster on DD.MasterID equals DM.MasterID
        //                              where (DM.DiagnosticDate == date && DD.ActionId == item.ActionId)
        //                              select DD).ToList();

        //                foreach (var obj in result)
        //                {
        //                    if (obj.ActionStatus == true && obj.ErrorDescription == "")
        //                    {

        //                        double logfilesizedetail = Convert.ToDouble(obj.Dignostic_Output);
        //                        double datafilesizedetail = Convert.ToDouble(dbContext.DiagnosticDetails.Where(x => x.MasterID == obj.MasterID && x.ActionId == 10).Select(x => x.Dignostic_Output).FirstOrDefault());
        //                        var percentoflogfilesize = ((logfilesizedetail / datafilesizedetail) * 100);
        //                        if (percentoflogfilesize > 25)
        //                        {
        //                            numberOfErrorsInAction.NumberOfErrors = numberOfErrorsInAction.NumberOfErrors + 1;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        numberOfErrorsInAction.NumberOfErrors += 1;
        //                    }

        //                }

        //                model.ActionErrorsList.Add(numberOfErrorsInAction);
        //                model.TotalIssueFound += numberOfErrorsInAction.NumberOfErrors;
        //            }
        //            else
        //            {
        //                numberOfErrorsInAction.ActionDesc = item.ActionDescription;
        //                numberOfErrorsInAction.NumberOfErrors = (from DD in dbContext.DiagnosticDetails
        //                                                         join DM in dbContext.DiagnosticMaster on DD.MasterID equals DM.MasterID
        //                                                         where (DM.DiagnosticDate == date && DD.ActionId == item.ActionId && DD.ActionStatus == false)
        //                                                         select DD).Count();
        //                model.ActionErrorsList.Add(numberOfErrorsInAction);
        //                model.TotalIssueFound += numberOfErrorsInAction.NumberOfErrors;
        //            }
        //        }
        //        return model;
        //        #endregion
        //    }
        //    catch (Exception) { throw; }
        //} 
        #endregion

        /// <summary>
        /// GetOfficeWiseDiagnosticStatusDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        #region Old Code Commented by mayank on 17/05/2022
        //public OfficeWiseDiagnosticStatusListModel GetOfficeWiseDiagnosticStatusDetail(OfficeWiseDiagnosticStatusModel model)
        //{
        //    OfficeWiseDiagnosticStatusListModel resModel = new OfficeWiseDiagnosticStatusListModel();
        //    List<OfficeWiseDiagnosticStatusDetailModel> officeWiseDiagnosticStatusDetailModels = new List<OfficeWiseDiagnosticStatusDetailModel>();

        //    DateTime Date = DateTime.Parse(model.Date);
        //    int SrNo = 1;
        //    try
        //    {
        //        List<DiagnosticMaster> diagnosticMasters = new List<DiagnosticMaster>();

        //        if (model.OfficeTypeID == 0)
        //        {
        //            //COMMENTED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //            //diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.DiagnosticDate >= fromDate && m.DiagnosticDate <= toDate).ToList();

        //            //ADDED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //            var outerjoinres = from s in dbContext.SROMaster
        //                               join d in dbContext.DiagnosticMaster on s.SROCode equals d.OfficeCode
        //                               into cu
        //                               from co in cu.DefaultIfEmpty()
        //                                   //where (s.SROCode > 0 && ((co.DiagnosticDate >= fromDate && co.DiagnosticDate <= toDate) || co.DiagnosticDate == null))
        //                               where (s.SROCode > 0 && ((co.DiagnosticDate == Date) || co.DiagnosticDate == null))
        //                               orderby co.IsSuccessful descending
        //                               select new
        //                               {
        //                                   DiagnosticId = (Int32?)co.DiagnosticId,
        //                                   SROCode = s.SROCode,
        //                                   DiagnosticDate = (DateTime?)co.DiagnosticDate,
        //                                   StartTime = (DateTime?)co.DiagnosticDate,
        //                                   CompletionTime = (DateTime?)co.CompletionTime,
        //                                   RequestIP = co.RequestIP,
        //                                   LoginId = co.LoginId,
        //                                   IsSuccessful = (short?)co.IsSuccessful,
        //                                   IsCentralized = (bool?)co.IsCentralized,
        //                                   SecureCode = co.SecureCode,
        //                                   isDRO = (Int32?)co.isDRO,
        //                                   MasterID = (Int32?)co.MasterID

        //                               };
        //            var res2 = outerjoinres.ToList();
        //            foreach (var item in res2)
        //            {
        //                DiagnosticMaster obj = new DiagnosticMaster();
        //                obj.DiagnosticId = item.DiagnosticId ?? 0;
        //                obj.OfficeCode = item.SROCode;
        //                obj.DiagnosticDate = item.DiagnosticDate ?? DateTime.Now;
        //                obj.StartTime = item.StartTime ?? DateTime.Now;
        //                obj.CompletionTime = item.CompletionTime ?? DateTime.Now;
        //                obj.RequestIP = item.RequestIP;
        //                obj.LoginId = item.LoginId;
        //                obj.IsSuccessful = item.IsSuccessful ?? 0;
        //                obj.IsCentralized = item.IsCentralized ?? false;
        //                obj.SecureCode = item.SecureCode;
        //                obj.isDRO = item.isDRO ?? 0;
        //                obj.MasterID = item.MasterID ?? 0;

        //                diagnosticMasters.Add(obj);


        //            }
        //            //diagnosticMasters = outerjoinres.ToList();
        //            //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //        }
        //        //else
        //        //{
        //        //    diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.OfficeCode == model.OfficeTypeID && (m.DiagnosticDate == Date)).ToList();

        //        //    if (diagnosticMasters.Count == 0)
        //        //    {
        //        //        DiagnosticMaster obj = new DiagnosticMaster();
        //        //        obj.DiagnosticId = 0;
        //        //        obj.OfficeCode = model.OfficeTypeID;
        //        //        obj.DiagnosticDate = DateTime.Now;
        //        //        obj.StartTime = DateTime.Now;
        //        //        obj.CompletionTime = DateTime.Now;
        //        //        obj.RequestIP = String.Empty;
        //        //        obj.LoginId = String.Empty;
        //        //        obj.IsSuccessful = 0;
        //        //        obj.IsCentralized = false;
        //        //        obj.SecureCode = null;
        //        //        obj.isDRO = 0;
        //        //        obj.MasterID = 0;

        //        //        diagnosticMasters.Add(obj);
        //        //    }
        //        //}

        //        List<DiagnosticDetails> diagnosticDetails = new List<DiagnosticDetails>();
        //        if (diagnosticMasters != null && diagnosticMasters.Count > 0)
        //        {
        //            foreach (var item in diagnosticMasters)
        //            {

        //                var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();
        //                foreach (var detail in res)
        //                {
        //                    diagnosticDetails.Add(detail);
        //                }
        //                //diagnosticDetails = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();

        //            }
        //        }
        //        List<int> MasterIDs = new List<int>();
        //        if (model.Status == 1)
        //        {
        //            diagnosticMasters.RemoveAll(m => m.DiagnosticId == 0);

        //            //all status = success
        //            foreach (var item in diagnosticMasters)
        //            {
        //                foreach (var dd in diagnosticDetails.Where(m => m.MasterID == item.MasterID ).ToList())
        //                {
        //                    if (dd.ActionStatus == false)
        //                    {
        //                        diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //                        //diagnosticMasters.Remove(item);
        //                        MasterIDs.Add(item.MasterID);
        //                        break;
        //                    }
        //                }
        //            }
        //            for (int i = 0; i < MasterIDs.Count; i++)
        //            {
        //                diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //            }
        //        }
        //        else if (model.Status == 2)
        //        {
        //            //all status = fail
        //            foreach (var item in diagnosticMasters)
        //            {
        //                int count = diagnosticDetails.Where(m => m.MasterID == item.MasterID).Count();
        //                if (count == 0)
        //                {
        //                    MasterIDs.Add(item.MasterID);
        //                    //break;
        //                }
        //                foreach (var dd in diagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList())
        //                {
        //                    if (dd.ActionStatus == true)
        //                    {
        //                        count--;
        //                        if (count == 0)
        //                        {
        //                            diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //                            //diagnosticMasters.Remove(item);
        //                            MasterIDs.Add(item.MasterID);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            for (int i = 0; i < MasterIDs.Count; i++)
        //            {

        //                diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //            }

        //        }
        //        //ADDED  ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //        else if (model.Status == 3)
        //        {
        //            //for all not executed
        //            foreach (var item in diagnosticMasters)
        //            {
        //                if (item.DiagnosticId > 0)
        //                {
        //                    MasterIDs.Add(item.MasterID);
        //                }
        //            }

        //            for (int i = 0; i < MasterIDs.Count; i++)
        //            {
        //                diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //            }

        //        }
        //        else if (model.Status == 4)
        //        {
        //            //for all status available
        //            foreach (var item in diagnosticMasters)
        //            {
        //                if (item.DiagnosticId == 0)
        //                {
        //                    MasterIDs.Add(item.MasterID);
        //                }
        //            }

        //            for (int i = 0; i < MasterIDs.Count; i++)
        //            {
        //                diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //            }
        //        }
        //        //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //        //to take latest actions details if more than one is present for 1 master id
        //        if (diagnosticDetails.Count > 0)
        //        {
        //            diagnosticDetails = diagnosticDetails.OrderByDescending(x => x.DetailId).ToList();
        //        }


        //        foreach (var item in diagnosticMasters)
        //        {
        //            bool ErrorInDBCCCheck = false;
        //            List<DiagnosticDetails> listObj = new List<DiagnosticDetails>();
        //            OfficeWiseDiagnosticStatusDetailModel obj = new OfficeWiseDiagnosticStatusDetailModel();
        //            listObj = diagnosticDetails.Where(x => x.MasterID == item.MasterID).ToList();

        //            //obj.SrNo = SrNo++;
        //            obj.OfficeName = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == item.OfficeCode).Select(m => m.OfficeName).FirstOrDefault().ToString();

        //            if (item.IsSuccessful == 0)
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";

        //            }
        //            else if (item.IsSuccessful == 1)
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }



        //            //ADDED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //            if (item.DiagnosticId == 0)
        //            {
        //                obj.DiagnosticDate = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.DBCCCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.AuditEventCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.AllActionCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                //ADDED ON 28-01-2021 FOR NEW CHANGES
        //                obj.DBDiskSpace = string.Empty;
        //                officeWiseDiagnosticStatusDetailModels.Add(obj);
        //                continue;
        //            }
        //            //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //            obj.DiagnosticDate = item.DiagnosticDate.ToString("dd/MM/yyyy");

        //            //for data and log file size
        //            obj.DataFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            //commented and added on 29-01-2021 for new changes
        //            //obj.LogFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            double logfilesizedetail = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());

        //            var percentoflogfilesize = ((logfilesizedetail / obj.DataFileSize) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
        //            //obj.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize + "% of data file size)";
        //            if (Convert.ToDouble(percentoflogfilesize) > 25)
        //            {
        //                obj.LogFileSize = @"<i class='action-error' style='color: lightslategray;' aria-hidden='true'>" + logfilesizedetail.ToString() + " (" + percentoflogfilesize + "% of data file size)" + "</i>";
        //            }
        //            else
        //            {
        //                obj.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize + "% of data file size)";
        //            }
        //            //for db disk space
        //            //commented on 28-01-2021 for new changes and also datatype is changed from double to string of obj.DBDiskSpace
        //            //obj.DBDiskSpace = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault());

        //            var dbSpaceDetail = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            var SpaceDetail = dbSpaceDetail.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);

        //            if (SpaceDetail != null && SpaceDetail.Length == 2)
        //            {
        //                //obj.DBDiskSpace = SpaceDetail[0] + "(" + SpaceDetail[1].Replace(':', ' ') + "% of total disk space)";
        //                if (Convert.ToDouble(SpaceDetail[1].Replace(':', ' ')) < 25)
        //                {
        //                    obj.DBDiskSpace = @"<i class='action-error' style='color: lightslategray;' aria-hidden='true'>" + SpaceDetail[0] + "(" + SpaceDetail[1].Replace(':', ' ') + "% of total disk space)" + "</i>";
        //                }
        //                else
        //                {
        //                    obj.DBDiskSpace = SpaceDetail[0] + "(" + SpaceDetail[1].Replace(':', ' ') + "% of total disk space)";
        //                }
        //            }
        //            else
        //                obj.DBDiskSpace = string.Empty;



        //            obj.DBCCStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.DBCCErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.DBCCOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            int ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ActionId).FirstOrDefault();
        //            int DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.DetailId).FirstOrDefault();
        //            int? MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.MasterID).FirstOrDefault();
        //            //string allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.DBCCStatus == true && obj.DBCCErrorDesc == "")
        //            {
        //                ////runned successfully
        //                //obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";

        //                //TEST FOR DBCC 0 ERROR 
        //                if (!string.IsNullOrEmpty(obj.DBCCOutput))
        //                {
        //                    string[] lines = obj.DBCCOutput.Replace("\r", "").Split('\n');
        //                    string ResultLine = lines[lines.Length - 2];
        //                    string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //                    int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //                    int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //                    if (AllocationErrors > 0 || ConsistencyErrors > 0)
        //                    {
        //                        ErrorInDBCCCheck = true;
        //                        obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //                    }
        //                    else
        //                    {
        //                        obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //                    }
        //                }
        //                else
        //                {
        //                    //runned successfully
        //                    obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";

        //                }

        //            }
        //            else if (obj.DBCCErrorDesc == null)
        //            {
        //                //not runned
        //                obj.DBCCCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }


        //            obj.ConstraintIntegrityStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.ConstraintIntegrityErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.ConstraintIntegrityOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.ConstraintIntegrityStatus == true && obj.ConstraintIntegrityErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.ConstraintIntegrityErrorDesc == null)
        //            {
        //                //not runned
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.AuditEventStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.AuditEventErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.AuditEventOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.AuditEventStatus == true && obj.AuditEventErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.AuditEventCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.AuditEventErrorDesc == null)3
        //            {
        //                //not runned
        //                obj.AuditEventCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.AuditEventCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.Optimizer1Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.Optimizer1ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.Optimizer1Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.Optimizer1Status == true && obj.Optimizer1ErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.Optimizer1ErrorDesc == null)
        //            {
        //                //not runned
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.Optimizer2Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.Optimizer2ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.Optimizer2Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.Optimizer2Status == true && obj.Optimizer2ErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.Optimizer2ErrorDesc == null)
        //            {
        //                //not runned
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.LastFullBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.LastFullBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.LastFullBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.LastFullBackupStatus == true && obj.LastFullBackupErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.LastFullBackupErrorDesc == null)
        //            {
        //                //not runned
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.LastDiffBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.LastDiffBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.LastDiffBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.LastDiffBackupStatus == true && obj.LastDiffBackupErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.LastDiffBackupErrorDesc == null)
        //            {
        //                //not runned
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            //for all ok status
        //            if (listObj.Where(x => x.ActionStatus == false).Any() || ErrorInDBCCCheck == true)
        //            {
        //                obj.AllActionStatus = false;
        //                obj.AllActionCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                obj.AllActionStatus = true;
        //                obj.AllActionCellHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }



        //            //for time zone check
        //            obj.TimeZoneStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.TimeZoneErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.TimeZoneOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.TimeZoneStatus == true && obj.TimeZoneErrorDesc == "" && obj.TimeZoneOutput == "India Standard Time")
        //            {
        //                //runned successfully
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.TimeZoneErrorDesc == null)
        //            {
        //                //not runned
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }



        //            officeWiseDiagnosticStatusDetailModels.Add(obj);

        //        }

        //        //ADDED TO SORT LIST BY OFFICE NAME
        //        officeWiseDiagnosticStatusDetailModels = officeWiseDiagnosticStatusDetailModels.OrderBy(x => x.OfficeName).ToList();
        //        foreach (var item in officeWiseDiagnosticStatusDetailModels)
        //        {
        //            item.SrNo = SrNo++;
        //        }

        //        #region for getting tiles data
        //        resModel.TilesData = new TilesModel();
        //        //TilesModel tilesModel = GetPopupTileData(Date);
        //        //resModel.TilesData.TotalNo = tilesModel.TotalNo;
        //        //resModel.TilesData.StatusAvailabelNo = tilesModel.StatusAvailabelNo;
        //        //resModel.TilesData.StatusNotAvailable = tilesModel.StatusNotAvailable;
        //        //resModel.TilesData.AllOkNo = tilesModel.AllOkNo;
        //        //resModel.TilesData.IssueFoundNo = tilesModel.IssueFoundNo;
        //        //resModel.TilesData.TotalIssueFound = tilesModel.TotalIssueFound;
        //        //resModel.TilesData.StatusAvailabelDesc = tilesModel.StatusAvailabelDesc;
        //        //resModel.TilesData.StatusNotAvailableDesc = tilesModel.StatusNotAvailableDesc;
        //        //resModel.TilesData.AllOkDesc = tilesModel.AllOkDesc;
        //        //resModel.TilesData.IssueFoundNoDesc = tilesModel.IssueFoundNoDesc;

        //        //resModel.TilesData.ActionErrorsList = tilesModel.ActionErrorsList;
        //        List<NumberOfErrorsInAction> numberOfErrorsInActionsList = new List<NumberOfErrorsInAction>();
        //        USP_DiagSummary_Result result = dbContext.USP_DiagSummary(Date).FirstOrDefault();
        //        List<USP_DiagDetail_Result> DetailResult = dbContext.USP_DiagDetail(Date).ToList();
        //        if (result != null)
        //        {
        //            resModel.TilesData.TotalNo = result.TotalOffices;
        //            resModel.TilesData.StatusAvailabelNo = result.DataAvailable;
        //            resModel.TilesData.StatusNotAvailable = result.DataNotAvailable;
        //            resModel.TilesData.AllOkNo = result.AllCheckSuccessful;
        //            resModel.TilesData.IssueFoundNo = result.WhereIssuesFound;
        //            resModel.TilesData.TotalIssueFound = result.TotalIssuesFound;
        //            resModel.TilesData.StatusAvailabelDesc = result.DataAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //            resModel.TilesData.StatusNotAvailableDesc = result.DataNotAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataNotAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //            resModel.TilesData.AllOkDesc = result.AllCheckSuccessful == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.AllCheckSuccessful) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
        //            resModel.TilesData.IssueFoundNoDesc = result.WhereIssuesFound == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.WhereIssuesFound) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
        //        }
        //        else
        //        {
        //            throw new Exception("No Data Received from SP DiagSummary");
        //        }
        //        if (DetailResult != null)
        //        {
        //            DetailResult.ForEach(m =>
        //            {
        //                NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
        //                numberOfErrorsInAction.ActionDesc = m.ActionDescription;
        //                numberOfErrorsInAction.NumberOfErrors = Convert.ToInt32(m.ISSUECOUNT);
        //                numberOfErrorsInActionsList.Add(numberOfErrorsInAction);
        //            });
        //            resModel.TilesData.ActionErrorsList = numberOfErrorsInActionsList;
        //        }   
        //        else
        //        {
        //            throw new Exception("No Data Received from SP DiagDetail");
        //        }
        //        #endregion

        //        resModel.TotalRecords = officeWiseDiagnosticStatusDetailModels.Count;
        //        resModel.OfficeWiseDiagnosticStatusList = officeWiseDiagnosticStatusDetailModels;

        //        #region commented code
        //        //foreach (var item in diagnosticDetails)
        //        //{
        //        //    OfficeWiseDiagnosticStatusDetailModel obj = new OfficeWiseDiagnosticStatusDetailModel();
        //        //    obj.SrNo = SrNo++;
        //        //    obj.OfficeName =  dbContext.MAS_OfficeMaster.Where(x=>x.Kaveri1Code == 
        //        //    diagnosticMasters.Where(m => m.MasterID == item.MasterID).Select(a => a.OfficeCode).FirstOrDefault()).Select(z=>z.OfficeName).FirstOrDefault();
        //        //    obj.DiagnosticDate = diagnosticMasters.Where(m => m.MasterID == item.MasterID).Select(m => m.DiagnosticDate).FirstOrDefault().ToString();

        //        //    obj.DBCCStatus = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.DBCCErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.DBCCOutput = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.ConstraintIntegrityStatus = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.ConstraintIntegrityErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.ConstraintIntegrityOutput = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.AuditEventStatus = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.AuditEventErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.AuditEventOutput = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.Optimizer1Status = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.Optimizer1ErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.Optimizer1Output = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.Optimizer2Status = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.Optimizer2ErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.Optimizer2Output = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.LastFullBackupStatus = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.LastFullBackupErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.LastFullBackupOutput = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;

        //        //    obj.LastDiffBackupStatus = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ActionStatus : false;
        //        //    obj.LastDiffBackupErrorDesc = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.ErrorDescription : string.Empty;
        //        //    obj.LastDiffBackupOutput = item.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency ? item.Dignostic_Output : string.Empty;
        //        //}
        //        #endregion


        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return resModel;
        //} 
        #endregion

        public OfficeWiseDiagnosticStatusListModel GetOfficeWiseDiagnosticStatusDetail(OfficeWiseDiagnosticStatusModel model)
        {
            try
            {
                OfficeWiseDiagnosticStatusListModel officeWiseDiagnosticStatusListModel = new OfficeWiseDiagnosticStatusListModel();
                officeWiseDiagnosticStatusListModel.TilesData = new TilesModel();
                officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList = new List<OfficeWiseDiagnosticStatusDetailModel>();
                dbContext = new KaveriEntities();
                List<USP_ActionWiseDiagnoticsDetails_ByTile_Result> resultModel = new List<USP_ActionWiseDiagnoticsDetails_ByTile_Result>();

                int getDataBy = 1;
                if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.AllCheckSuccessful || model.Status == (int)ApiCommonEnum.DiagnosticsTile.IssueFound || model.Status == (int)ApiCommonEnum.DiagnosticsTile.StatusAvailabel)
                    getDataBy = 1;
                else if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.AllOffice)
                    getDataBy = 2;
                else if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.StatusNotAvailabel)
                    getDataBy = 3;
                resultModel = dbContext.USP_ActionWiseDiagnoticsDetails_ByTile(Convert.ToDateTime(model.Date), getDataBy).ToList();

                int i = 1;
                List<List<USP_ActionWiseDiagnoticsDetails_ByTile_Result>> Grpresult = resultModel.GroupBy(m => new { m.MasterID, m.OfficeCode }).
                                                                    Select(grp => grp.ToList()).ToList();
                List<List<USP_ActionWiseDiagnoticsDetails_ByTile_Result>> Detailresult = new List<List<USP_ActionWiseDiagnoticsDetails_ByTile_Result>>();
                if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.AllCheckSuccessful)
                {
                    foreach (var item in Grpresult)
                    {
                        if (!item.Where(m => m.ActionStatus == false).Any())
                        {
                            Detailresult.Add(item);
                        }
                    }
                }
                else if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.IssueFound)
                {
                    foreach (var item in Grpresult)
                    {
                        if (item.Where(m => m.ActionStatus == false).Any())
                        {
                            Detailresult.Add(item);
                        }
                    }
                }
                //else if (model.Status == (int)ApiCommonEnum.DiagnosticsTile.StatusAvailabel)
                else
                {
                    Detailresult = Grpresult;
                }

                USP_DiagSummary_Result result = dbContext.USP_DiagSummary(Convert.ToDateTime(model.Date)).FirstOrDefault();

                List<USP_DiagDetail_Result> DetailResult = dbContext.USP_DiagDetail(Convert.ToDateTime(model.Date)).ToList();
                List<NumberOfErrorsInAction> numberOfErrorsInActionsList = new List<NumberOfErrorsInAction>();


                if (result != null)
                {
                    officeWiseDiagnosticStatusListModel.TilesData.TotalNo = result.TotalOffices;
                    officeWiseDiagnosticStatusListModel.TilesData.StatusAvailabelNo = result.DataAvailable;
                    officeWiseDiagnosticStatusListModel.TilesData.StatusNotAvailable = result.DataNotAvailable;
                    officeWiseDiagnosticStatusListModel.TilesData.AllOkNo = result.AllCheckSuccessful;
                    officeWiseDiagnosticStatusListModel.TilesData.IssueFoundNo = result.WhereIssuesFound;
                    officeWiseDiagnosticStatusListModel.TilesData.TotalIssueFound = result.TotalIssuesFound;
                    officeWiseDiagnosticStatusListModel.TilesData.StatusAvailabelDesc = result.DataAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
                    officeWiseDiagnosticStatusListModel.TilesData.StatusNotAvailableDesc = result.DataNotAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataNotAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
                    officeWiseDiagnosticStatusListModel.TilesData.AllOkDesc = result.AllCheckSuccessful == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.AllCheckSuccessful) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
                    officeWiseDiagnosticStatusListModel.TilesData.IssueFoundNoDesc = result.WhereIssuesFound == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.WhereIssuesFound) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
                }
                else
                {
                    throw new Exception("No Data Received from SP DiagSummary");
                }
                if (DetailResult != null)
                {
                    DetailResult.ForEach(m =>
                    {
                        NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
                        numberOfErrorsInAction.ActionDesc = m.ActionDescription;
                        numberOfErrorsInAction.NumberOfErrors = Convert.ToInt32(m.ISSUECOUNT);
                        numberOfErrorsInActionsList.Add(numberOfErrorsInAction);
                    });
                    officeWiseDiagnosticStatusListModel.TilesData.ActionErrorsList = numberOfErrorsInActionsList;
                }
                else
                {
                    throw new Exception("No Data Received from SP DiagDetail");
                }

                foreach (var item in Detailresult)
                {
                    OfficeWiseDiagnosticStatusDetailModel detailmodel = new OfficeWiseDiagnosticStatusDetailModel();
                    detailmodel.SrNo = i++;
                    detailmodel.OfficeName = item.FirstOrDefault().OfficeName;
                    detailmodel.DiagnosticDate = item.FirstOrDefault().ExecutionDate.ToString("dd/MM/yyyy");
                    if (item.Where(m=>m.ActionStatus==false).Any())
                    {
                        detailmodel.AllActionCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";

                    }
                    else
                    {
                        detailmodel.AllActionCellHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                    }
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).Any())
                    {
                        double datafilesize = 0;
                        if (double.TryParse(item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).FirstOrDefault().Dignostic_Output, out datafilesize))
                            detailmodel.DataFileSize = datafilesize;
                    }
                    else
                        detailmodel.DataFileSize = 0;

                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Any())
                    {
                        double logfilesizedetail = 0;
                        bool doubleConv = double.TryParse(item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).FirstOrDefault().Dignostic_Output, out logfilesizedetail);
                        if (doubleConv && detailmodel.DataFileSize>0)
                        {
                            double percentoflogfilesize = ((logfilesizedetail / detailmodel.DataFileSize) * 100);
                            if (percentoflogfilesize > 25)
                            {
                                detailmodel.LogFileSize = @"<i class='action-error' style='color: lightslategray;' aria-hidden='true'>" + logfilesizedetail.ToString() + " (" + percentoflogfilesize.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "% of data file size)" + "</i>";
                            }
                            else
                            {
                                detailmodel.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "% of data file size)";
                            }
                        }
                        else
                            detailmodel.LogFileSize = @"<i class='fa fa-times fa-lg action-error' style='color: lightslategray;' aria-hidden='true'></i>";

                    }
                    else
                    {
                        detailmodel.LogFileSize = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";
                    }
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Any())
                        detailmodel.DBDiskSpace = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).FirstOrDefault().Dignostic_Output;
                    else
                        detailmodel.DBDiskSpace = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";


                    #region TimeZone
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objTimeZone = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).FirstOrDefault();

                        if (objTimeZone.ActionStatus == true && objTimeZone.ErrorDescription == "" && objTimeZone.Dignostic_Output == "India Standard Time")
                        {
                            //runned successfully
                            detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objTimeZone.ActionId + "','" + objTimeZone.DetailId + "','" + objTimeZone.MasterID + "')></i>";
                        }
                        else if (objTimeZone.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objTimeZone.ActionId + "','" + objTimeZone.DetailId + "','" + objTimeZone.MasterID + "')></i>";
                        }
                    }
                    else
                        detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";

                    #endregion

                    #region DatabaseConsistency
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objDatabaseConsistency = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).FirstOrDefault();

                        if (objDatabaseConsistency.ActionStatus == true && objDatabaseConsistency.ErrorDescription == "")
                        {
                            if (!string.IsNullOrEmpty(objDatabaseConsistency.Dignostic_Output))
                            {
                                string[] lines = objDatabaseConsistency.Dignostic_Output.Replace("\r", "").Split('\n');
                                if (lines.Length > 2)
                                {
                                    try
                                    {
                                        string ResultLine = lines[lines.Length - 2];
                                        string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
                                        int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
                                        int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
                                        if (AllocationErrors > 0 || ConsistencyErrors > 0)
                                        {
                                            detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
                                        }
                                        else
                                        {
                                            detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
                                        }
                                    }
                                    catch
                                    {
                                        detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
                                    }
                                }
                                else
                                {
                                    detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
                                }
                            }
                            else
                            {
                                //runned successfully
                                detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";

                            }

                        }
                        else if (objDatabaseConsistency.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.DBCCCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
                        }
                    }
                    else
                        detailmodel.DBCCCCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";
                    #endregion

                    #region ConstraintsIntegrity
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objConstraintsIntegrity = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).FirstOrDefault();

                        if (objConstraintsIntegrity.ActionStatus == true && objConstraintsIntegrity.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objConstraintsIntegrity.ActionId + "','" + objConstraintsIntegrity.DetailId + "','" + objConstraintsIntegrity.MasterID + "')></i>";
                        }
                        else if (objConstraintsIntegrity.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objConstraintsIntegrity.ActionId + "','" + objConstraintsIntegrity.DetailId + "','" + objConstraintsIntegrity.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";
                    }
                    #endregion

                    #region AuditEventStarted
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objAuditEventStarted = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).FirstOrDefault();

                        if (objAuditEventStarted.ActionStatus == true && objAuditEventStarted.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.AuditEventCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objAuditEventStarted.ActionId + "','" + objAuditEventStarted.DetailId + "','" + objAuditEventStarted.MasterID + "')></i>";
                        }
                        else if (objAuditEventStarted.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.AuditEventCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.AuditEventCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objAuditEventStarted.ActionId + "','" + objAuditEventStarted.DetailId + "','" + objAuditEventStarted.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.AuditEventCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";

                    }
                    #endregion

                    #region Optimizer1
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objOptimizer1 = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).FirstOrDefault();

                        if (objOptimizer1.ActionStatus == true && objOptimizer1.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objOptimizer1.ActionId + "','" + objOptimizer1.DetailId + "','" + objOptimizer1.MasterID + "')></i>";
                        }
                        else if (objOptimizer1.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.Optimizer1CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objOptimizer1.ActionId + "','" + objOptimizer1.DetailId + "','" + objOptimizer1.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.Optimizer1CellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";

                    }
                    #endregion

                    #region Optimizer2
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objOptimizer2 = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).FirstOrDefault();

                        if (objOptimizer2.ActionStatus == true && objOptimizer2.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objOptimizer2.ActionId + "','" + objOptimizer2.DetailId + "','" + objOptimizer2.MasterID + "')></i>";
                        }
                        else if (objOptimizer2.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.Optimizer2CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objOptimizer2.ActionId + "','" + objOptimizer2.DetailId + "','" + objOptimizer2.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.Optimizer2CellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";

                    }
                    #endregion

                    #region LastFullBackupVerification
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objLastFullBackupVerification = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).FirstOrDefault();

                        if (objLastFullBackupVerification.ActionStatus == true && objLastFullBackupVerification.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objLastFullBackupVerification.ActionId + "','" + objLastFullBackupVerification.DetailId + "','" + objLastFullBackupVerification.MasterID + "')></i>";
                        }
                        else if (objLastFullBackupVerification.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objLastFullBackupVerification.ActionId + "','" + objLastFullBackupVerification.DetailId + "','" + objLastFullBackupVerification.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";
                    }
                    #endregion

                    #region LastDifferentialBackupVerification
                    if (item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Any())
                    {
                        USP_ActionWiseDiagnoticsDetails_ByTile_Result objLastDifferentialBackupVerification = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).FirstOrDefault();

                        if (objLastDifferentialBackupVerification.ActionStatus == true && objLastDifferentialBackupVerification.ErrorDescription == "")
                        {
                            //runned successfully
                            detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objLastDifferentialBackupVerification.ActionId + "','" + objLastDifferentialBackupVerification.DetailId + "','" + objLastDifferentialBackupVerification.MasterID + "')></i>";
                        }
                        else if (objLastDifferentialBackupVerification.ErrorDescription == null)
                        {
                            //not runned
                            detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
                        }
                        else
                        {
                            //runned with error
                            detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objLastDifferentialBackupVerification.ActionId + "','" + objLastDifferentialBackupVerification.DetailId + "','" + objLastDifferentialBackupVerification.MasterID + "')></i>";
                        }
                    }
                    else
                    {
                        detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-ban ' style='color: lightslategray;' aria-hidden='true'></i>";
                    }
                    #endregion

                    officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList.Add(detailmodel);
                }
                officeWiseDiagnosticStatusListModel.TotalRecords = officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList.Count;
                return officeWiseDiagnosticStatusListModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// GetActionDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DiagnosticActionDetail GetActionDetail(DiagnosticActionDetail model)
        {
            DiagnosticActionDetail resModel = new DiagnosticActionDetail();
            try
            {
                DiagnosticDetails diagnosticDetails = new DiagnosticDetails();
                //diagnosticDetails = dbContext.DiagnosticDetails.OrderByDescending(x => x.DetailId).Where(m => m.MasterID == model.MasterId && m.DetailId == model.DetailId && m.ActionId == model.ActionId).FirstOrDefault();
                diagnosticDetails = dbContext.DiagnosticDetails.Where(m => m.MasterID == model.MasterId && m.DetailId == model.DetailId && m.ActionId == model.ActionId).OrderByDescending(x => x.DetailId).FirstOrDefault();

                if (diagnosticDetails != null)
                {

                    if (diagnosticDetails.ErrorDescription == string.Empty)
                    {
                        resModel.ActionDetail = diagnosticDetails.Dignostic_Output;
                    }
                    else
                    {
                        resModel.ActionDetail = diagnosticDetails.ErrorDescription;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            return resModel;
        }

        /// <summary>
        /// ExportOfficeWiseDiagnosticStatusToExcel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusListModel ExportOfficeWiseDiagnosticStatusToExcel(OfficeWiseDiagnosticStatusModel model)
        {
            OfficeWiseDiagnosticStatusListModel resModel = new OfficeWiseDiagnosticStatusListModel();
            List<OfficeWiseDiagnosticStatusDetailModel> officeWiseDiagnosticStatusDetailModels = new List<OfficeWiseDiagnosticStatusDetailModel>();
            //DateTime fromDate = DateTime.Parse(model.FromDate);
            //DateTime toDate = DateTime.Parse(model.ToDate);
            DateTime Date = DateTime.Parse(model.Date);
            int SrNo = 1;
            try
            {
                List<DiagnosticMaster> diagnosticMasters = new List<DiagnosticMaster>();

                if (model.OfficeTypeID == 0)
                {
                    //diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.DiagnosticDate >= fromDate && m.DiagnosticDate <= toDate).ToList();

                    //ADDED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

                    var outerjoinres = from s in dbContext.SROMaster
                                       join d in dbContext.DiagnosticMaster on s.SROCode equals d.OfficeCode
                                       into cu
                                       from co in cu.DefaultIfEmpty()
                                           //where (s.SROCode > 0 && ((co.DiagnosticDate >= fromDate && co.DiagnosticDate <= toDate) || co.DiagnosticDate == null))
                                       where (s.SROCode > 0 && ((co.DiagnosticDate == Date) || co.DiagnosticDate == null))
                                       orderby co.IsSuccessful descending
                                       select new
                                       {
                                           DiagnosticId = (Int32?)co.DiagnosticId,
                                           SROCode = s.SROCode,
                                           DiagnosticDate = (DateTime?)co.DiagnosticDate,
                                           StartTime = (DateTime?)co.DiagnosticDate,
                                           CompletionTime = (DateTime?)co.CompletionTime,
                                           RequestIP = co.RequestIP,
                                           LoginId = co.LoginId,
                                           IsSuccessful = (short?)co.IsSuccessful,
                                           IsCentralized = (bool?)co.IsCentralized,
                                           SecureCode = co.SecureCode,
                                           isDRO = (Int32?)co.isDRO,
                                           MasterID = (Int32?)co.MasterID

                                       };
                    var res2 = outerjoinres.ToList();
                    foreach (var item in res2)
                    {
                        DiagnosticMaster obj = new DiagnosticMaster();
                        obj.DiagnosticId = item.DiagnosticId ?? 0;
                        obj.OfficeCode = item.SROCode;
                        obj.DiagnosticDate = item.DiagnosticDate ?? DateTime.Now;
                        obj.StartTime = item.StartTime ?? DateTime.Now;
                        obj.CompletionTime = item.CompletionTime ?? DateTime.Now;
                        obj.RequestIP = item.RequestIP;
                        obj.LoginId = item.LoginId;
                        obj.IsSuccessful = item.IsSuccessful ?? 0;
                        obj.IsCentralized = item.IsCentralized ?? false;
                        obj.SecureCode = item.SecureCode;
                        obj.isDRO = item.isDRO ?? 0;
                        obj.MasterID = item.MasterID ?? 0;

                        diagnosticMasters.Add(obj);


                    }
                }
                //else
                //{
                //    diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.OfficeCode == model.OfficeTypeID && (m.DiagnosticDate == Date)).ToList();

                //    if (diagnosticMasters.Count == 0)
                //    {
                //        DiagnosticMaster obj = new DiagnosticMaster();
                //        obj.DiagnosticId = 0;
                //        obj.OfficeCode = model.OfficeTypeID;
                //        obj.DiagnosticDate = DateTime.Now;
                //        obj.StartTime = DateTime.Now;
                //        obj.CompletionTime = DateTime.Now;
                //        obj.RequestIP = String.Empty;
                //        obj.LoginId = String.Empty;
                //        obj.IsSuccessful = 0;
                //        obj.IsCentralized = false;
                //        obj.SecureCode = null;
                //        obj.isDRO = 0;
                //        obj.MasterID = 0;

                //        diagnosticMasters.Add(obj);
                //    }

                //}

                List<DiagnosticDetails> diagnosticDetails = new List<DiagnosticDetails>();
                if (diagnosticMasters != null && diagnosticMasters.Count > 0)
                {
                    foreach (var item in diagnosticMasters)
                    {

                        var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();
                        foreach (var detail in res)
                        {
                            diagnosticDetails.Add(detail);
                        }
                        //diagnosticDetails = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();

                    }
                }
                List<int> MasterIDs = new List<int>();
                if (model.Status == 1)
                {
                    //all status = success
                    foreach (var item in diagnosticMasters)
                    {
                        foreach (var dd in diagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList())
                        {
                            if (dd.ActionStatus == false)
                            {
                                diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
                                //diagnosticMasters.Remove(item);
                                MasterIDs.Add(item.MasterID);
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < MasterIDs.Count; i++)
                    {
                        diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
                    }
                }
                else if (model.Status == 2)
                {
                    //all status = fail
                    foreach (var item in diagnosticMasters)
                    {
                        int count = diagnosticDetails.Where(m => m.MasterID == item.MasterID).Count();
                        if (count == 0)
                        {
                            MasterIDs.Add(item.MasterID);
                            //break;
                        }
                        foreach (var dd in diagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList())
                        {
                            if (dd.ActionStatus == true)
                            {
                                count--;
                                if (count == 0)
                                {
                                    diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
                                    //diagnosticMasters.Remove(item);
                                    MasterIDs.Add(item.MasterID);
                                    break;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < MasterIDs.Count; i++)
                    {

                        diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
                    }

                }
                //ADDED  ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
                else if (model.Status == 3)
                {
                    //for all not executed
                    foreach (var item in diagnosticMasters)
                    {
                        if (item.DiagnosticId > 0)
                        {
                            MasterIDs.Add(item.MasterID);
                        }
                    }

                    for (int i = 0; i < MasterIDs.Count; i++)
                    {
                        diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
                    }

                }
                //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

                //to take latest actions details if more than one is present for 1 master id
                if (diagnosticDetails.Count > 0)
                {
                    diagnosticDetails = diagnosticDetails.OrderByDescending(x => x.DetailId).ToList();
                }

                #region COMMENDTED
                //List<DiagnosticMaster> diagnosticMasters = new List<DiagnosticMaster>();
                //if (model.OfficeTypeID == 0)
                //{
                //    diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.DiagnosticDate >= fromDate && m.DiagnosticDate <= toDate).ToList();
                //}
                //else
                //{
                //    diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.OfficeCode == model.OfficeTypeID && (m.DiagnosticDate >= fromDate && m.DiagnosticDate <= toDate)).ToList();
                //}
                //List<DiagnosticDetails> diagnosticDetails = new List<DiagnosticDetails>();
                //if (diagnosticMasters != null && diagnosticMasters.Count > 0)
                //{
                //    foreach (var item in diagnosticMasters)
                //    {

                //        var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();
                //        foreach (var detail in res)
                //        {
                //            diagnosticDetails.Add(detail);
                //        }

                //    }
                //}
                #endregion

                foreach (var item in diagnosticMasters)
                {
                    List<DiagnosticDetails> listObj = new List<DiagnosticDetails>();
                    OfficeWiseDiagnosticStatusDetailModel obj = new OfficeWiseDiagnosticStatusDetailModel();
                    listObj = diagnosticDetails.Where(x => x.MasterID == item.MasterID).ToList();

                    //obj.SrNo = SrNo++;
                    obj.OfficeName = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == item.OfficeCode).Select(m => m.OfficeName).FirstOrDefault().ToString();
                    obj.DiagnosticDate = item.DiagnosticDate.ToString("dd/MM/yyyy");

                    if (item.IsSuccessful == 0)
                    {
                        obj.SecureCodeHtml = @"-";
                        obj.DiagnosticDate = @"-";

                    }
                    else if (item.IsSuccessful == 1)
                    {
                        obj.SecureCodeHtml = @"Valid";
                    }
                    else
                    {
                        obj.SecureCodeHtml = @"InValid";
                    }


                    //for data and log file size
                    obj.DataFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
                    //obj.LogFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
                    double logfilesizedetail = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());

                    var percentoflogfilesize = ((logfilesizedetail / obj.DataFileSize) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    obj.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize + "% of data file size)";
                    //for db disk space
                    //commented and added on 28-01-2021 for new changes
                    //obj.DBDiskSpace = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault());
                    var dbSpaceDetail = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault();
                    var SpaceDetail = dbSpaceDetail.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);

                    if (SpaceDetail != null && SpaceDetail.Length == 2)
                        obj.DBDiskSpace = SpaceDetail[0] + "(" + SpaceDetail[1].Replace(':', ' ') + "% of total disk space)";
                    else
                        obj.DBDiskSpace = string.Empty;

                    obj.DBCCStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.DBCCErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.DBCCOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.DBCCStatus == true && obj.DBCCErrorDesc == "")
                    {
                        //runned successfully
                        obj.DBCCCCellHtml = obj.DBCCOutput;
                    }
                    else if (obj.DBCCErrorDesc == null)
                    {
                        //not runned
                        obj.DBCCCCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.DBCCCCellHtml = obj.DBCCErrorDesc;
                    }



                    obj.ConstraintIntegrityStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.ConstraintIntegrityErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.ConstraintIntegrityOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.ConstraintIntegrityStatus == true && obj.ConstraintIntegrityErrorDesc == "")
                    {
                        //runned successfully
                        obj.ConstraintIntegrityCellHtml = obj.ConstraintIntegrityOutput;
                    }
                    else if (obj.ConstraintIntegrityErrorDesc == null)
                    {
                        //not runned
                        obj.ConstraintIntegrityCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.ConstraintIntegrityCellHtml = obj.ConstraintIntegrityErrorDesc;
                    }

                    obj.AuditEventStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.AuditEventErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.AuditEventOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.AuditEventStatus == true && obj.AuditEventErrorDesc == "")
                    {
                        //runned successfully
                        obj.AuditEventCellHtml = obj.AuditEventOutput;
                    }
                    else if (obj.AuditEventErrorDesc == null)
                    {
                        //not runned
                        obj.AuditEventCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.AuditEventCellHtml = obj.AuditEventErrorDesc;
                    }

                    obj.Optimizer1Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.Optimizer1ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.Optimizer1Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.Optimizer1Status == true && obj.Optimizer1ErrorDesc == "")
                    {
                        //runned successfully
                        obj.Optimizer1CellHtml = obj.Optimizer1Output;
                    }
                    else if (obj.Optimizer1ErrorDesc == null)
                    {
                        //not runned
                        obj.Optimizer1CellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.Optimizer1CellHtml = obj.Optimizer1ErrorDesc;
                    }

                    obj.Optimizer2Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.Optimizer2ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.Optimizer2Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.Optimizer2Status == true && obj.Optimizer2ErrorDesc == "")
                    {
                        //runned successfully
                        obj.Optimizer2CellHtml = obj.Optimizer2Output;
                    }
                    else if (obj.Optimizer1ErrorDesc == null)
                    {
                        //not runned
                        obj.Optimizer2CellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.Optimizer2CellHtml = obj.Optimizer2ErrorDesc;
                    }

                    obj.LastFullBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.LastFullBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.LastFullBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.LastFullBackupStatus == true && obj.LastFullBackupErrorDesc == "")
                    {
                        //runned successfully
                        obj.LastFullBackupCellHtml = obj.LastFullBackupOutput;
                    }
                    else if (obj.LastFullBackupErrorDesc == null)
                    {
                        //not runned
                        obj.LastFullBackupCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.LastFullBackupCellHtml = obj.LastFullBackupErrorDesc;
                    }

                    obj.LastDiffBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.LastDiffBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.LastDiffBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.LastDiffBackupStatus == true && obj.LastDiffBackupErrorDesc == "")
                    {
                        //runned successfully
                        obj.LastDiffBackupCCellHtml = obj.LastDiffBackupOutput;
                    }
                    else if (obj.LastDiffBackupErrorDesc == null)
                    {
                        //not runned
                        obj.LastDiffBackupCCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.LastDiffBackupCCellHtml = obj.LastDiffBackupErrorDesc;
                    }

                    //for all ok status
                    if (listObj.Where(x => x.ActionStatus == false).Any())
                    {
                        obj.AllActionStatus = false;
                        obj.AllActionCellHtml = "Runned With Error";
                    }
                    else if (item.IsSuccessful == 0)
                    {
                        obj.AllActionStatus = true;
                        obj.AllActionCellHtml = "-";
                    }
                    else
                    {
                        obj.AllActionStatus = true;
                        obj.AllActionCellHtml = "Runned Successfully";
                    }


                    //for time zone check
                    obj.TimeZoneStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ActionStatus).FirstOrDefault();
                    obj.TimeZoneErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ErrorDescription).FirstOrDefault();
                    obj.TimeZoneOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.Dignostic_Output).FirstOrDefault();
                    if (obj.TimeZoneStatus == true && obj.TimeZoneErrorDesc == "" && obj.TimeZoneOutput == "India Standard Time")
                    {
                        //runned successfully
                        obj.TimeZoneCCellHtml = obj.TimeZoneOutput;
                    }
                    else if (obj.TimeZoneErrorDesc == null)
                    {
                        //not runned
                        obj.TimeZoneCCellHtml = string.Empty;
                    }
                    else
                    {
                        //runned with error
                        obj.TimeZoneCCellHtml = obj.TimeZoneErrorDesc;
                    }

                    officeWiseDiagnosticStatusDetailModels.Add(obj);

                }

                //ADDED TO SORT LIST BY OFFICE NAME
                officeWiseDiagnosticStatusDetailModels = officeWiseDiagnosticStatusDetailModels.OrderBy(x => x.OfficeName).ToList();
                foreach (var item in officeWiseDiagnosticStatusDetailModels)
                {
                    item.SrNo = SrNo++;
                }

                resModel.TotalRecords = officeWiseDiagnosticStatusDetailModels.Count;
                resModel.OfficeWiseDiagnosticStatusList = officeWiseDiagnosticStatusDetailModels;



            }
            catch (Exception ex)
            {
                throw;
            }
            return resModel;
        }

        public OfficeWiseDiagnosticStatusListModel GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel model)
        {
            throw new NotImplementedException();
        }

        #region Old Code Commented by mayank on 18/05/2022
        //public OfficeWiseDiagnosticStatusListModel GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel model)
        //{
        //    OfficeWiseDiagnosticStatusListModel resModel = new OfficeWiseDiagnosticStatusListModel();
        //    List<OfficeWiseDiagnosticStatusDetailModel> officeWiseDiagnosticStatusDetailModels = new List<OfficeWiseDiagnosticStatusDetailModel>();

        //    DateTime Date = DateTime.Parse(model.Date);
        //    int SrNo = 1;
        //    try
        //    {
        //        List<DiagnosticMaster> diagnosticMasters = new List<DiagnosticMaster>();

        //        if (model.OfficeTypeID == 0)
        //        {
        //            //COMMENTED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //            diagnosticMasters = dbContext.DiagnosticMaster.Where(m => m.DiagnosticDate == Date).ToList();

        //            #region COMMENTED
        //            //ADDED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //            //var outerjoinres = from s in dbContext.SROMaster
        //            //                   join d in dbContext.DiagnosticMaster on s.SROCode equals d.OfficeCode
        //            //                   into cu
        //            //                   from co in cu.DefaultIfEmpty()
        //            //                       //where (s.SROCode > 0 && ((co.DiagnosticDate >= fromDate && co.DiagnosticDate <= toDate) || co.DiagnosticDate == null))
        //            //                   where (s.SROCode > 0 && ((co.DiagnosticDate == Date) || co.DiagnosticDate == null))
        //            //                   orderby co.IsSuccessful descending
        //            //                   select new
        //            //                   {
        //            //                       DiagnosticId = (Int32?)co.DiagnosticId,
        //            //                       SROCode = s.SROCode,
        //            //                       DiagnosticDate = (DateTime?)co.DiagnosticDate,
        //            //                       StartTime = (DateTime?)co.DiagnosticDate,
        //            //                       CompletionTime = (DateTime?)co.CompletionTime,
        //            //                       RequestIP = co.RequestIP,
        //            //                       LoginId = co.LoginId,
        //            //                       IsSuccessful = (short?)co.IsSuccessful,
        //            //                       IsCentralized = (bool?)co.IsCentralized,
        //            //                       SecureCode = co.SecureCode,
        //            //                       isDRO = (Int32?)co.isDRO,
        //            //                       MasterID = (Int32?)co.MasterID

        //            //                   };
        //            //var res2 = outerjoinres.ToList();
        //            //foreach (var item in res2)
        //            //{
        //            //    DiagnosticMaster obj = new DiagnosticMaster();
        //            //    obj.DiagnosticId = item.DiagnosticId ?? 0;
        //            //    obj.OfficeCode = item.SROCode;
        //            //    obj.DiagnosticDate = item.DiagnosticDate ?? DateTime.Now;
        //            //    obj.StartTime = item.StartTime ?? DateTime.Now;
        //            //    obj.CompletionTime = item.CompletionTime ?? DateTime.Now;
        //            //    obj.RequestIP = item.RequestIP;
        //            //    obj.LoginId = item.LoginId;
        //            //    obj.IsSuccessful = item.IsSuccessful ?? 0;
        //            //    obj.IsCentralized = item.IsCentralized ?? false;
        //            //    obj.SecureCode = item.SecureCode;
        //            //    obj.isDRO = item.isDRO ?? 0;
        //            //    obj.MasterID = item.MasterID ?? 0;

        //            //    diagnosticMasters.Add(obj);


        //            //}
        //            #endregion

        //        }

        //        List<DiagnosticDetails> diagnosticDetails = new List<DiagnosticDetails>();
        //        if (diagnosticMasters != null && diagnosticMasters.Count > 0)
        //        {
        //            foreach (var item in diagnosticMasters)
        //            {

        //                //var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID && m.ActionId == model.ActionIDForDetailTable).ToList();
        //                var res = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();
        //                foreach (var detail in res)
        //                {
        //                    diagnosticDetails.Add(detail);
        //                }
        //                //diagnosticDetails = dbContext.DiagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList();

        //            }
        //        }

        //        //test
        //        List<int?> idtoremove = new List<int?>();
        //        foreach (var item in diagnosticDetails.Where(m => m.ActionId == model.ActionIDForDetailTable))
        //        {
        //            if (item.ActionStatus == true && item.ActionId == 4)  //for db DBCC CHECK
        //            {
        //                if (!string.IsNullOrEmpty(item.Dignostic_Output))
        //                {
        //                    string[] lines = item.Dignostic_Output.Replace("\r", "").Split('\n');
        //                    string ResultLine = lines[lines.Length - 2];
        //                    string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //                    int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //                    int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //                    if (AllocationErrors == 0 && ConsistencyErrors == 0)
        //                    {
        //                        idtoremove.Add(item.MasterID);
        //                    }

        //                }
        //            }
        //            else if (item.ActionStatus == true && item.ActionId == 12)  //for db disk space check
        //            {
        //                if (!string.IsNullOrEmpty(item.Dignostic_Output))
        //                {
        //                    var SpaceDetail = item.Dignostic_Output.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);
        //                    var percentFree = SpaceDetail[1].Replace(':', ' ').Trim();
        //                    if (Convert.ToDouble(percentFree) > 25)
        //                    {
        //                        idtoremove.Add(item.MasterID);
        //                    }

        //                }
        //            }
        //            else if (item.ActionStatus == true && item.ActionId == 11)
        //            {
        //                if (!string.IsNullOrEmpty(item.Dignostic_Output))
        //                {
        //                    double logfilesizedetail = Convert.ToDouble(item.Dignostic_Output);
        //                    double datafilesizedetail = Convert.ToDouble(dbContext.DiagnosticDetails.Where(x => x.MasterID == item.MasterID && x.ActionId == 10).Select(x => x.Dignostic_Output).FirstOrDefault());
        //                    var percentoflogfilesize = ((logfilesizedetail / datafilesizedetail) * 100);
        //                    if (percentoflogfilesize < 25)
        //                    {
        //                        idtoremove.Add(item.MasterID);
        //                    }

        //                }
        //            }
        //            else if (item.ActionStatus == true)
        //            {
        //                idtoremove.Add(item.MasterID);
        //            }

        //        }

        //        for (int i = 0; i < idtoremove.Count; i++)
        //        {
        //            diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == idtoremove.ElementAt(i)).FirstOrDefault());
        //        }
        //        //test

        //        #region COMMENTED ON 02-02-2021 AND ADDED ABOVE FOR SAME
        //        //List<int> MasterIDs = new List<int>();
        //        ////all status = fail
        //        //foreach (var item in diagnosticMasters)
        //        //{
        //        //    int count = diagnosticDetails.Where(m => m.MasterID == item.MasterID).Count();
        //        //    if (count == 0)
        //        //    {
        //        //        MasterIDs.Add(item.MasterID);
        //        //        //break;
        //        //    }
        //        //    foreach (var dd in diagnosticDetails.Where(m => m.MasterID == item.MasterID).ToList())
        //        //    {

        //        //        if (dd.ActionStatus == true && dd.ActionId == 4)  //for db DBCC CHECK
        //        //        {
        //        //            if (!string.IsNullOrEmpty(dd.Dignostic_Output))
        //        //            {
        //        //                string[] lines = dd.Dignostic_Output.Replace("\r", "").Split('\n');
        //        //                string ResultLine = lines[lines.Length - 2];
        //        //                string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //        //                int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //        //                int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //        //                if (AllocationErrors == 0 && ConsistencyErrors == 0)
        //        //                {
        //        //                    count--;
        //        //                    if (count == 0)
        //        //                    {
        //        //                        diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //        //                        //diagnosticMasters.Remove(item);
        //        //                        MasterIDs.Add(item.MasterID);
        //        //                        break;
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //        else if (dd.ActionStatus == true && dd.ActionId == 12)  //for db disk space check
        //        //        {
        //        //            if (!string.IsNullOrEmpty(dd.Dignostic_Output))
        //        //            {
        //        //                var SpaceDetail = dd.Dignostic_Output.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);
        //        //                var percentFree = SpaceDetail[1].Replace(':', ' ').Trim();
        //        //                if (Convert.ToDouble(percentFree) > 25)
        //        //                {
        //        //                    count--;
        //        //                    if (count == 0)
        //        //                    {
        //        //                        diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //        //                        //diagnosticMasters.Remove(item);
        //        //                        MasterIDs.Add(item.MasterID);
        //        //                        break;
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //        else if (dd.ActionStatus == true && dd.ActionId == 11)
        //        //        {
        //        //            if (!string.IsNullOrEmpty(dd.Dignostic_Output))
        //        //            {
        //        //                double logfilesizedetail = Convert.ToDouble(dd.Dignostic_Output);
        //        //                double datafilesizedetail = Convert.ToDouble(dbContext.DiagnosticDetails.Where(x => x.MasterID == dd.MasterID && x.ActionId == 10).Select(x => x.Dignostic_Output).FirstOrDefault());
        //        //                var percentoflogfilesize = ((logfilesizedetail / datafilesizedetail) * 100);
        //        //                if (percentoflogfilesize < 25)
        //        //                {
        //        //                    count--;
        //        //                    if (count == 0)
        //        //                    {
        //        //                        diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //        //                        //diagnosticMasters.Remove(item);
        //        //                        MasterIDs.Add(item.MasterID);
        //        //                        break;
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //        else if (dd.ActionStatus == true)
        //        //        {
        //        //            count--;
        //        //            if (count == 0)
        //        //            {
        //        //                diagnosticDetails.RemoveAll(m => m.MasterID == item.MasterID);
        //        //                //diagnosticMasters.Remove(item);
        //        //                MasterIDs.Add(item.MasterID);
        //        //                break;
        //        //            }
        //        //        }

        //        //    }
        //        //}
        //        //for (int i = 0; i < MasterIDs.Count; i++)
        //        //{
        //        //    diagnosticMasters.Remove(diagnosticMasters.Where(m => m.MasterID == MasterIDs.ElementAt(i)).FirstOrDefault());
        //        //}
        //        #endregion


        //        //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //        //to take latest actions details if more than one is present for 1 master id
        //        if (diagnosticDetails.Count > 0)
        //        {
        //            diagnosticDetails = diagnosticDetails.OrderByDescending(x => x.DetailId).ToList();
        //        }


        //        foreach (var item in diagnosticMasters)
        //        {
        //            bool ErrorInDBCCCheck = false;
        //            List<DiagnosticDetails> listObj = new List<DiagnosticDetails>();
        //            OfficeWiseDiagnosticStatusDetailModel obj = new OfficeWiseDiagnosticStatusDetailModel();
        //            listObj = diagnosticDetails.Where(x => x.MasterID == item.MasterID).ToList();

        //            //obj.SrNo = SrNo++;
        //            obj.OfficeName = dbContext.MAS_OfficeMaster.Where(x => x.Kaveri1Code == item.OfficeCode).Select(m => m.OfficeName).FirstOrDefault().ToString();

        //            if (item.IsSuccessful == 0)
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";

        //            }
        //            else if (item.IsSuccessful == 1)
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                obj.SecureCodeHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }



        //            //ADDED ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST
        //            if (item.DiagnosticId == 0)
        //            {
        //                obj.DiagnosticDate = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.DBCCCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.AuditEventCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.AllActionCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //                //ADDED ON 28-01-2021 FOR NEW CHANGES
        //                obj.DBDiskSpace = string.Empty;
        //                officeWiseDiagnosticStatusDetailModels.Add(obj);
        //                continue;
        //            }
        //            //ADDED ABOVE CODE ON 16-12-2020 FOR ALL OFFICE TO SHOW IN LIST

        //            obj.DiagnosticDate = item.DiagnosticDate.ToString("dd/MM/yyyy");

        //            //for data and log file size
        //            obj.DataFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            //obj.LogFileSize = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            if (model.ActionIDForDetailTable == 11)
        //            {
        //                obj.DataFileSize = Convert.ToDouble(dbContext.DiagnosticDetails.Where(x => x.ActionId == 10 && x.MasterID == item.MasterID).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            }

        //            double logfilesizedetail = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).Select(x => x.Dignostic_Output).FirstOrDefault());

        //            if (logfilesizedetail > 0 && obj.DataFileSize > 0)
        //            {
        //                var percentoflogfilesize = ((logfilesizedetail / obj.DataFileSize) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

        //                obj.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize + "% of data file size)";
        //            }
        //            else
        //            {
        //                obj.LogFileSize = string.Empty;
        //            }
        //            //for db disk space
        //            //commented and added on 28-01-2021 for new changes
        //            //obj.DBDiskSpace = Convert.ToDouble(listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault());
        //            var dbSpaceDetail = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            if (dbSpaceDetail != null)
        //            {
        //                var SpaceDetail = dbSpaceDetail.Split(new string[] { "FreeSpace:", "FreePercentage" }, StringSplitOptions.RemoveEmptyEntries);

        //                if (SpaceDetail != null && SpaceDetail.Length == 2)
        //                    obj.DBDiskSpace = SpaceDetail[0] + "(" + SpaceDetail[1].Replace(':', ' ') + "% of total disk space)";
        //                else
        //                    obj.DBDiskSpace = string.Empty;
        //            }
        //            obj.DBCCStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.DBCCErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.DBCCOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            int ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.ActionId).FirstOrDefault();
        //            int DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.DetailId).FirstOrDefault();
        //            int? MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).Select(x => x.MasterID).FirstOrDefault();
        //            //string allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.DBCCStatus == true && obj.DBCCErrorDesc == "")
        //            {
        //                ////runned successfully
        //                //obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";

        //                //TEST FOR DBCC 0 ERROR 
        //                if (!string.IsNullOrEmpty(obj.DBCCOutput))
        //                {
        //                    string[] lines = obj.DBCCOutput.Replace("\r", "").Split('\n');
        //                    string ResultLine = lines[lines.Length - 2];
        //                    string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //                    int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //                    int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //                    if (AllocationErrors > 0 || ConsistencyErrors > 0)
        //                    {
        //                        ErrorInDBCCCheck = true;
        //                        obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //                    }
        //                    else
        //                    {
        //                        obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //                    }
        //                }
        //                else
        //                {
        //                    //runned successfully
        //                    obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";

        //                }

        //            }
        //            else if (obj.DBCCErrorDesc == null)
        //            {
        //                //not runned
        //                obj.DBCCCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }


        //            obj.ConstraintIntegrityStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.ConstraintIntegrityErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.ConstraintIntegrityOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.ConstraintIntegrityStatus == true && obj.ConstraintIntegrityErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.ConstraintIntegrityErrorDesc == null)
        //            {
        //                //not runned
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.AuditEventStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.AuditEventErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.AuditEventOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.AuditEventStatus == true && obj.AuditEventErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.AuditEventCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.AuditEventErrorDesc == null)
        //            {
        //                //not runned
        //                obj.AuditEventCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.AuditEventCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.Optimizer1Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.Optimizer1ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.Optimizer1Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.Optimizer1Status == true && obj.Optimizer1ErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.Optimizer1ErrorDesc == null)
        //            {
        //                //not runned
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.Optimizer2Status = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.Optimizer2ErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.Optimizer2Output = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.Optimizer2Status == true && obj.Optimizer2ErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.Optimizer2ErrorDesc == null)
        //            {
        //                //not runned
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.LastFullBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.LastFullBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.LastFullBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.LastFullBackupStatus == true && obj.LastFullBackupErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.LastFullBackupErrorDesc == null)
        //            {
        //                //not runned
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            obj.LastDiffBackupStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.LastDiffBackupErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.LastDiffBackupOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.LastDiffBackupStatus == true && obj.LastDiffBackupErrorDesc == "")
        //            {
        //                //runned successfully
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.LastDiffBackupErrorDesc == null)
        //            {
        //                //not runned
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }

        //            //for all ok status
        //            if (listObj.Where(x => x.ActionStatus == false).Any() || ErrorInDBCCCheck == true)
        //            {
        //                obj.AllActionStatus = false;
        //                obj.AllActionCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                obj.AllActionStatus = true;
        //                obj.AllActionCellHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }



        //            //for time zone check
        //            obj.TimeZoneStatus = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ActionStatus).FirstOrDefault();
        //            obj.TimeZoneErrorDesc = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ErrorDescription).FirstOrDefault();
        //            obj.TimeZoneOutput = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.Dignostic_Output).FirstOrDefault();
        //            ActionId = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.ActionId).FirstOrDefault();
        //            DetailID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.DetailId).FirstOrDefault();
        //            MasterID = listObj.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).Select(x => x.MasterID).FirstOrDefault();
        //            //allID = DetailID + "#" + ActionId + "#" + MasterID;
        //            if (obj.TimeZoneStatus == true && obj.TimeZoneErrorDesc == "" && obj.TimeZoneOutput == "India Standard Time")
        //            {
        //                //runned successfully
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }
        //            else if (obj.TimeZoneErrorDesc == null)
        //            {
        //                //not runned
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                obj.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + ActionId + "','" + DetailID + "','" + MasterID + "')></i>";
        //            }



        //            officeWiseDiagnosticStatusDetailModels.Add(obj);

        //        }

        //        //ADDED TO SORT LIST BY OFFICE NAME
        //        officeWiseDiagnosticStatusDetailModels = officeWiseDiagnosticStatusDetailModels.OrderBy(x => x.OfficeName).ToList();
        //        foreach (var item in officeWiseDiagnosticStatusDetailModels)
        //        {
        //            item.SrNo = SrNo++;
        //        }

        //        #region for getting tiles data
        //        //resModel.TilesData = new TilesModel();
        //        //TilesModel tilesModel = GetPopupTileData(Date);
        //        //resModel.TilesData.TotalNo = tilesModel.TotalNo;
        //        //resModel.TilesData.StatusAvailabelNo = tilesModel.StatusAvailabelNo;
        //        //resModel.TilesData.StatusNotAvailable = tilesModel.StatusNotAvailable;
        //        //resModel.TilesData.AllOkNo = tilesModel.AllOkNo;
        //        //resModel.TilesData.IssueFoundNo = tilesModel.IssueFoundNo;

        //        //resModel.TilesData.StatusAvailabelDesc = tilesModel.StatusAvailabelDesc;
        //        //resModel.TilesData.StatusNotAvailableDesc = tilesModel.StatusNotAvailableDesc;
        //        //resModel.TilesData.AllOkDesc = tilesModel.AllOkDesc;
        //        //resModel.TilesData.IssueFoundNoDesc = tilesModel.IssueFoundNoDesc;

        //        //resModel.TilesData.ActionErrorsList = tilesModel.ActionErrorsList;
        //        #endregion

        //        resModel.TotalRecords = officeWiseDiagnosticStatusDetailModels.Count;
        //        resModel.OfficeWiseDiagnosticStatusList = officeWiseDiagnosticStatusDetailModels;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return resModel;
        //} 
        #endregion
        //public OfficeWiseDiagnosticStatusListModel GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel model)
        //{
        //    try
        //    {
        //        OfficeWiseDiagnosticStatusListModel officeWiseDiagnosticStatusListModel = new OfficeWiseDiagnosticStatusListModel();
        //        officeWiseDiagnosticStatusListModel.TilesData = new TilesModel();
        //        officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList = new List<OfficeWiseDiagnosticStatusDetailModel>();
        //        dbContext = new KaveriEntities();
        //        List<USP_DiagnoticsDetails_Result> resultModel = new List<USP_DiagnoticsDetails_Result>();
        //        if (model.GetDetailsDataBy == GetDetailsDataBy.Action)
        //        {
        //            resultModel = dbContext.USP_DiagnoticsDetails(model.ActionIDForDetailTable, Convert.ToDateTime(model.Date), 0, (int)GetDetailsDataBy.Action).ToList();
        //        }
        //        int i = 1;
        //        List<List<USP_DiagnoticsDetails_Result>> Detailresult = resultModel.GroupBy(m => m.MasterID).
        //                                                            Select(grp => grp.ToList()).ToList();

        //        USP_DiagSummary_Result result = dbContext.USP_DiagSummary(Convert.ToDateTime(model.Date)).FirstOrDefault();

        //        List<USP_DiagDetail_Result> DetailResult = dbContext.USP_DiagDetail(Convert.ToDateTime(model.Date)).ToList();
        //        List<NumberOfErrorsInAction> numberOfErrorsInActionsList = new List<NumberOfErrorsInAction>();


        //        if (result != null)
        //        {
        //            officeWiseDiagnosticStatusListModel.TilesData.TotalNo = result.TotalOffices;
        //            officeWiseDiagnosticStatusListModel.TilesData.StatusAvailabelNo = result.DataAvailable;
        //            officeWiseDiagnosticStatusListModel.TilesData.StatusNotAvailable = result.DataNotAvailable;
        //            officeWiseDiagnosticStatusListModel.TilesData.AllOkNo = result.AllCheckSuccessful;
        //            officeWiseDiagnosticStatusListModel.TilesData.IssueFoundNo = result.WhereIssuesFound;
        //            officeWiseDiagnosticStatusListModel.TilesData.TotalIssueFound = result.TotalIssuesFound;
        //            officeWiseDiagnosticStatusListModel.TilesData.StatusAvailabelDesc = result.DataAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //            officeWiseDiagnosticStatusListModel.TilesData.StatusNotAvailableDesc = result.DataNotAvailable == 0 ? "0 % of All office" : ((Convert.ToDouble(result.DataNotAvailable) / result.TotalOffices) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of All office";
        //            officeWiseDiagnosticStatusListModel.TilesData.AllOkDesc = result.AllCheckSuccessful == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.AllCheckSuccessful) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
        //            officeWiseDiagnosticStatusListModel.TilesData.IssueFoundNoDesc = result.WhereIssuesFound == 0 ? "0 % of Status Available" : ((Convert.ToDouble(result.WhereIssuesFound) / result.DataAvailable) * 100).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " % of Status Available";
        //        }
        //        else
        //        {
        //            throw new Exception("No Data Received from SP DiagSummary");
        //        }
        //        if (DetailResult != null)
        //        {
        //            DetailResult.ForEach(m =>
        //            {
        //                NumberOfErrorsInAction numberOfErrorsInAction = new NumberOfErrorsInAction();
        //                numberOfErrorsInAction.ActionDesc = m.ActionDescription;
        //                numberOfErrorsInAction.NumberOfErrors = Convert.ToInt32(m.ISSUECOUNT);
        //                numberOfErrorsInActionsList.Add(numberOfErrorsInAction);
        //            });
        //            officeWiseDiagnosticStatusListModel.TilesData.ActionErrorsList = numberOfErrorsInActionsList;
        //        }
        //        else
        //        {
        //            throw new Exception("No Data Received from SP DiagDetail");
        //        }

        //        foreach (var item in Detailresult)
        //        {
        //            OfficeWiseDiagnosticStatusDetailModel detailmodel = new OfficeWiseDiagnosticStatusDetailModel();
        //            detailmodel.SrNo = i++;
        //            detailmodel.OfficeName = item.FirstOrDefault().OfficeName;
        //            detailmodel.DiagnosticDate = item.FirstOrDefault().ExecutionDate.ToString("dd/MM/yyyy");
        //            if (item.FirstOrDefault().Overall_Status == -1)
        //            {
        //                detailmodel.AllActionCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";

        //            }
        //            else
        //            {
        //                detailmodel.AllActionCellHtml = @"<i class='fa fa-check fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            detailmodel.DataFileSize = Convert.ToDouble(item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetDataFileSize).FirstOrDefault().Dignostic_Output);
        //            double logfilesizedetail = Convert.ToDouble(item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.GetLogFileSize).FirstOrDefault().Dignostic_Output);
        //            double percentoflogfilesize = ((logfilesizedetail / detailmodel.DataFileSize) * 100);
        //            if (percentoflogfilesize > 25)
        //            {
        //                detailmodel.LogFileSize = @"<i class='action-error' style='color: lightslategray;' aria-hidden='true'>" + logfilesizedetail.ToString() + " (" + percentoflogfilesize.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "% of data file size)" + "</i>";
        //            }
        //            else
        //            {
        //                detailmodel.LogFileSize = logfilesizedetail.ToString() + " (" + percentoflogfilesize.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "% of data file size)";
        //            }
        //            detailmodel.DBDiskSpace = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseDiskSpace).FirstOrDefault().Dignostic_Output;


        //            #region TimeZone
        //            USP_DiagnoticsDetails_Result objTimeZone = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckTimeZone).FirstOrDefault();

        //            if (objTimeZone.ActionStatus == true && objTimeZone.ErrorDescription == "" && objTimeZone.Dignostic_Output == "India Standard Time")
        //            {
        //                //runned successfully
        //                detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objTimeZone.ActionId + "','" + objTimeZone.DetailId + "','" + objTimeZone.MasterID + "')></i>";
        //            }
        //            else if (objTimeZone.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.TimeZoneCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objTimeZone.ActionId + "','" + objTimeZone.DetailId + "','" + objTimeZone.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region DatabaseConsistency
        //            USP_DiagnoticsDetails_Result objDatabaseConsistency = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckDatabaseConsistency).FirstOrDefault();

        //            if (objDatabaseConsistency.ActionStatus == true && objDatabaseConsistency.ErrorDescription == "")
        //            {
        //                if (!string.IsNullOrEmpty(objDatabaseConsistency.Dignostic_Output))
        //                {
        //                    string[] lines = objDatabaseConsistency.Dignostic_Output.Replace("\r", "").Split('\n');
        //                    string ResultLine = lines[lines.Length - 2];
        //                    string[] ResultOutput = ResultLine.Split(new[] { "CHECKDB found", "allocation errors and", "consistency" }, StringSplitOptions.RemoveEmptyEntries);
        //                    int AllocationErrors = Convert.ToInt32(ResultOutput[0]);
        //                    int ConsistencyErrors = Convert.ToInt32(ResultOutput[1]);
        //                    if (AllocationErrors > 0 || ConsistencyErrors > 0)
        //                    {
        //                        detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
        //                    }
        //                    else
        //                    {
        //                        detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
        //                    }
        //                }
        //                else
        //                {
        //                    //runned successfully
        //                    detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;'  onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";

        //                }

        //            }
        //            else if (objDatabaseConsistency.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.DBCCCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.DBCCCCellHtml = @"<i class='fa fa-check fa-lg action-error' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objDatabaseConsistency.ActionId + "','" + objDatabaseConsistency.DetailId + "','" + objDatabaseConsistency.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region ConstraintsIntegrity
        //            USP_DiagnoticsDetails_Result objConstraintsIntegrity = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckConstraintsIntegrity).FirstOrDefault();

        //            if (objConstraintsIntegrity.ActionStatus == true && objConstraintsIntegrity.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objConstraintsIntegrity.ActionId + "','" + objConstraintsIntegrity.DetailId + "','" + objConstraintsIntegrity.MasterID + "')></i>";
        //            }
        //            else if (objConstraintsIntegrity.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.ConstraintIntegrityCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objConstraintsIntegrity.ActionId + "','" + objConstraintsIntegrity.DetailId + "','" + objConstraintsIntegrity.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region AuditEventStarted
        //            USP_DiagnoticsDetails_Result objAuditEventStarted = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckAuditEventStarted).FirstOrDefault();

        //            if (objAuditEventStarted.ActionStatus == true && objAuditEventStarted.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.AuditEventCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objAuditEventStarted.ActionId + "','" + objAuditEventStarted.DetailId + "','" + objAuditEventStarted.MasterID + "')></i>";
        //            }
        //            else if (objAuditEventStarted.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.AuditEventCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.AuditEventCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objAuditEventStarted.ActionId + "','" + objAuditEventStarted.DetailId + "','" + objAuditEventStarted.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region Optimizer1
        //            USP_DiagnoticsDetails_Result objOptimizer1 = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer1).FirstOrDefault();

        //            if (objOptimizer1.ActionStatus == true && objOptimizer1.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objOptimizer1.ActionId + "','" + objOptimizer1.DetailId + "','" + objOptimizer1.MasterID + "')></i>";
        //            }
        //            else if (objOptimizer1.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.Optimizer1CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.Optimizer1CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objOptimizer1.ActionId + "','" + objOptimizer1.DetailId + "','" + objOptimizer1.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region Optimizer2
        //            USP_DiagnoticsDetails_Result objOptimizer2 = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.CheckOptimizer2).FirstOrDefault();

        //            if (objOptimizer2.ActionStatus == true && objOptimizer2.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objOptimizer2.ActionId + "','" + objOptimizer2.DetailId + "','" + objOptimizer2.MasterID + "')></i>";
        //            }
        //            else if (objOptimizer2.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.Optimizer2CellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.Optimizer2CellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objOptimizer2.ActionId + "','" + objOptimizer2.DetailId + "','" + objOptimizer2.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region LastFullBackupVerification
        //            USP_DiagnoticsDetails_Result objLastFullBackupVerification = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastFullBackupVerification).FirstOrDefault();

        //            if (objLastFullBackupVerification.ActionStatus == true && objLastFullBackupVerification.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objLastFullBackupVerification.ActionId + "','" + objLastFullBackupVerification.DetailId + "','" + objLastFullBackupVerification.MasterID + "')></i>";
        //            }
        //            else if (objLastFullBackupVerification.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.LastFullBackupCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objLastFullBackupVerification.ActionId + "','" + objLastFullBackupVerification.DetailId + "','" + objLastFullBackupVerification.MasterID + "')></i>";
        //            }
        //            #endregion

        //            #region LastDifferentialBackupVerification
        //            USP_DiagnoticsDetails_Result objLastDifferentialBackupVerification = item.Where(m => m.ActionId == (int)ApiCommonEnum.DiagnosticActionMaster.LastDifferentialBackupVerification).FirstOrDefault();

        //            if (objLastDifferentialBackupVerification.ActionStatus == true && objLastDifferentialBackupVerification.ErrorDescription == "")
        //            {
        //                //runned successfully
        //                detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg' aria-hidden='true' style='color: lightslategray;' onClick = GetActionData('" + objLastDifferentialBackupVerification.ActionId + "','" + objLastDifferentialBackupVerification.DetailId + "','" + objLastDifferentialBackupVerification.MasterID + "')></i>";
        //            }
        //            else if (objLastDifferentialBackupVerification.ErrorDescription == null)
        //            {
        //                //not runned
        //                detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-times fa-lg' style='color: lightslategray;' aria-hidden='true'></i>";
        //            }
        //            else
        //            {
        //                //runned with error
        //                detailmodel.LastDiffBackupCCellHtml = @"<i class='fa fa-check fa-lg action-error' style='color: lightslategray;' aria-hidden='true' onClick = GetActionData('" + objLastDifferentialBackupVerification.ActionId + "','" + objLastDifferentialBackupVerification.DetailId + "','" + objLastDifferentialBackupVerification.MasterID + "')></i>";
        //            }
        //            #endregion

        //            officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList.Add(detailmodel);
        //        }
        //        officeWiseDiagnosticStatusListModel.TotalRecords = officeWiseDiagnosticStatusListModel.OfficeWiseDiagnosticStatusList.Count;
        //        return officeWiseDiagnosticStatusListModel;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}
