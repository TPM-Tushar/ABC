#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ECDataAuditDetailsDAL.cs
    * Author Name       :   Harshit
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Log Analysis  module.
*/
#endregion

using ECDataAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;

using Security;
using System.Security;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.Areas.LogAnalysis.Interface;
using CustomModels.Models.LogAnalysis.ECDataAuditDetails;
using System.Text;

namespace ECDataAPI.Areas.LogAnalysis.DAL
{
    public class ECDataAuditDetailsDAL : IECDataAuditDetails
    {

       // private Dictionary<String, String> decryptedParameters = null;
       // private String[] encryptedParameters = null;


        /// <summary>
        /// Needed For Passing Count to Data Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ECDataAuditDetailsListTotalCount(ECDataAuditDetailsRequestModel model)
        {
            try
            {
                KaveriEntities dbContext = null;

                dbContext = new KaveriEntities();
                List<USP_ECDATA_MODIFICATION_INFO_Result> Result = dbContext.USP_ECDATA_MODIFICATION_INFO(model.Datetime_FromDate, model.Datetime_ToDate, model.OfficeID, model.programs).ToList();
                return Result.Count();
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        /// <summary>
        /// Needed For Returning list for PDF(All Counts)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string LoadMasterTables(long LogID, int LOGTYPEID, int SROCode, long ItemID)
        {

            List<MasterTableModel> listViewModel = new List<MasterTableModel>();
            KaveriEntities dbContext = null;
            StringBuilder sBuilder = new StringBuilder();
            try
            {
                dbContext = new KaveriEntities();
                List<USP_ECDATA_MODIFICATION_DETAILS_COVER_Result> Result = dbContext.USP_ECDATA_MODIFICATION_DETAILS_COVER(LogID, LOGTYPEID, SROCode, ItemID).ToList();
                if (Result.Count() == 0)
                {
                    return string.Empty;
                }
                sBuilder.Append("<center><table style='border: 1px solid #ddd;'><tbody><tr style='border: 1px solid #ddd; padding: 4px; padding-bottom:2px;' ><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>Column Name</b></td><td style='padding: 4px; border: 1px solid #ddd;text-align:center;'><b>Previous Value</b></td><td style='border: 1px solid #ddd;text-align:center;'><b>Modified Value</b></td></tr>");
                foreach (var item in Result)
                {
                    sBuilder.Append("<tr>");
                    sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + item.COLUMN_NAME + "</td>");
                    sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + item.PREV_VALUE + "</td>");
                    sBuilder.Append("<td style='padding: 4px; border: 1px solid #ddd;'>" + item.MODIFIED_VALUE + "</td>");
                    sBuilder.Append("</tr>");
                }
                sBuilder.Append("</tbody>");
                sBuilder.Append("</table>");
                sBuilder.Append("</center>");
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
            return sBuilder.ToString();
        }


        /// <summary>
        /// Returns ECDataAuditDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ECDataAuditDetailsResponseModel> ECDataAuditDetailsList(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            List<ECDataAuditDetailsResponseModel> List = new List<ECDataAuditDetailsResponseModel>();
            ECDataAuditDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                List<USP_ECDATA_MODIFICATION_INFO_Result> Result = dbContext.USP_ECDATA_MODIFICATION_INFO(WrapperModel.Datetime_FromDate, WrapperModel.Datetime_ToDate, WrapperModel.OfficeID, WrapperModel.programs).Skip(WrapperModel.StartLength).Take(WrapperModel.TotalNum).ToList();
                foreach (var item in Result)
                {
                    obj = new ECDataAuditDetailsResponseModel();
                    obj.LogID = item.LogID;
                    obj.LogTypeID = item.LOGTYPEID;
                    obj.SRONAME = item.SRONAME;
                    obj.SROCODE = item.SROCODE ?? 0;
                    obj.DOCUMENTID = item.DOCUMENTID ?? 0;  
                    obj.ITEMID = item.ITEMID ?? 0;
                    obj.FRN = item.FRN;
                    obj.DATEOFMODIFICATION = item.DATEOFMODIFICATION;
                    obj.MODIFICATION_AREA = item.MODIFICATION_AREA;
                    obj.MODIFICATION_AREA_ID = item.MODIFICATION_AREA_ID;
                    obj.MODIFICATION_TYPE = item.MODIFICATION_TYPE;
                    obj.ModificationDescForPDF = item.MODIFICATION_DESCRIPTION;
                    if (item.MODIFICATION_TYPE == "Delete")
                    {
                        obj.MODIFICATION_DESCRIPTION = "";
                    }
                    else
                    {
                        obj.MODIFICATION_DESCRIPTION = LoadMasterTables(obj.LogID, obj.LogTypeID, obj.SROCODE, obj.ITEMID) ?? "";
                    }
                    obj.IPADRESS = item.IPADRESS;
                    obj.hostname = item.hostname;
                    obj.APPLICATION_NAME = item.APPLICATION_NAME;
                    obj.PHYSICAL_ADDRESS = item.PHYSICALADDRESS;
                    List.Add(obj);
                }
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
            return List;
        }

        /// <summary>
        /// On Page Model
        /// </summary>
        /// <returns></returns>
        public ECDataAuditDetailsRequestModel ECDataAuditDetailsRequestModel()
        {
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel();
                model.OfficeList = objCommon.getSROfficesList(true);

                model.ProgramNameList = objCommon.GetProgramNameList(false);
                model.ProgramID = model.ProgramNameList.Select(X => Convert.ToInt32(X.Value)).ToArray();
                return model;


            }
            catch (Exception ex)
            {
                throw;

            }
            }

        /// <summary>
        /// Returns Office Wise Modification Occurrance
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OfficeModificationOccurenceModel> GetOfficeWiseModificationOccurences(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            List<OfficeModificationOccurenceModel> returnList = new List<OfficeModificationOccurenceModel>();
            OfficeModificationOccurenceModel obj = null;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var result = dbContext.USP_ECDATA_MODIFICATION_OCCURANCES(WrapperModel.Datetime_FromDate, WrapperModel.Datetime_ToDate, WrapperModel.OfficeID, WrapperModel.programs).ToList();
                foreach (var item in result)
                {
                    obj = new OfficeModificationOccurenceModel();
                    obj.SROCode = item.SROCode ?? 0;
                    obj.SROName = item.SRONAME;
                    obj.NoOfOccurances = item.NO_OF_OCCURANCES ?? 0;
                    obj.LastModifiedDateTime = item.LAST_OCCURANCE_DATE;
                    returnList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnList;
        }


        //Added by Raman 28-03-2019
        /// <summary>
        /// Needed For Returning list for PDF(All Counts)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ECDataAuditDetailsResponseModel> GetECDataAuditDetailsListForPDF(ECDataAuditDetailsRequestModel model)
        {
            List<ECDataAuditDetailsResponseModel> List = new List<ECDataAuditDetailsResponseModel>();
            ECDataAuditDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                List<USP_ECDATA_MODIFICATION_INFO_Result> Result = dbContext.USP_ECDATA_MODIFICATION_INFO(model.Datetime_FromDate, model.Datetime_ToDate, model.OfficeID, model.programs).ToList();
                foreach (var item in Result)
                {
                    obj = new ECDataAuditDetailsResponseModel();
                    obj.LogID = item.LogID;
                    obj.LogTypeID = item.LOGTYPEID;
                    obj.SRONAME = item.SRONAME;
                    obj.SROCODE = item.SROCODE ?? 0;
                    obj.DOCUMENTID = item.DOCUMENTID ?? 0;
                    obj.ITEMID = item.ITEMID ?? 0;
                    obj.FRN = item.FRN;
                    obj.DATEOFMODIFICATION = item.DATEOFMODIFICATION;
                    obj.MODIFICATION_AREA = item.MODIFICATION_AREA;
                    obj.MODIFICATION_AREA_ID = item.MODIFICATION_AREA_ID;
                    obj.MODIFICATION_TYPE = item.MODIFICATION_TYPE;
                    obj.ModificationDescForPDF = item.MODIFICATION_DESCRIPTION;
                    if (item.MODIFICATION_TYPE == "Delete")
                    {
                        obj.MODIFICATION_DESCRIPTION = "";
                    }
                    else
                    {
                        obj.MODIFICATION_DESCRIPTION = LoadMasterTables(obj.LogID, obj.LogTypeID, obj.SROCODE, obj.ITEMID) ?? "";
                    }
                    obj.IPADRESS = item.IPADRESS;
                    obj.hostname = item.hostname;
                    obj.APPLICATION_NAME = item.APPLICATION_NAME;
                    obj.PHYSICAL_ADDRESS = item.PHYSICALADDRESS;
                    List.Add(obj);
                }
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
            return List;
        }



        /// <summary>
        /// Returns list of MasterTable
        /// </summary>

        /// <returns></returns>
        public List<MasterTableModel> MasterTablesListForPDF(WrapperModelForDescPDF wrapperModel)
        {

            List<MasterTableModel> listViewModel = new List<MasterTableModel>();
            MasterTableModel objMasterTableModel = new MasterTableModel();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                List<USP_ECDATA_MODIFICATION_DETAILS_COVER_Result> Result = dbContext.USP_ECDATA_MODIFICATION_DETAILS_COVER(wrapperModel.logID, wrapperModel.logTypeID, wrapperModel.sROCODE, wrapperModel.iTEMID).ToList();
                foreach (var item in Result)
                {
                    objMasterTableModel = new MasterTableModel();
                    objMasterTableModel.ColumnName = item.COLUMN_NAME;
                    objMasterTableModel.PrevValue = item.PREV_VALUE;
                    objMasterTableModel.ModifiedValue = item.MODIFIED_VALUE;
                    listViewModel.Add(objMasterTableModel);
                }
                return listViewModel;
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
        }
    }
}