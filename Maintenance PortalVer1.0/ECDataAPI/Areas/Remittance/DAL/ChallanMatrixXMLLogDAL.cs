#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ChallanMatrixXMLLogDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.ChallanMatrixXMLLog;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;
//using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.PreRegApplicationDetailsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ChallanMatrixXMLLogDAL : IChallanMatrixXMLLog, IDisposable
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public ChallanMatrixWrapperModel GetOfficeList(String OfficeType)
        {
            ChallanMatrixWrapperModel model = new ChallanMatrixWrapperModel();
            model.OfficeTypeList = new List<SelectListItem>();
            SelectListItem selectListItem = new SelectListItem();
            selectListItem.Text = "Select";
            selectListItem.Value = "0";
            model.OfficeTypeList.Add(selectListItem);

            if (OfficeType.ToLower().Equals("sro"))
            {
                //var SROMasterList = dbContext.SROMaster.ToList();
                List<SROMaster> SROMasterList = dbContext.SROMaster.ToList();
                SROMasterList = SROMasterList.OrderBy(x => x.SRONameE).ToList();
                if (SROMasterList != null)
                {
                    if (SROMasterList.Count() > 0)
                    {
                        foreach (var item in SROMasterList)
                        {
                            SelectListItem selectListOBJ = new SelectListItem();
                            selectListOBJ.Text = item.SRONameE + " (" + item.SROCode.ToString() + ")";
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
                            selectListOBJ.Text = item.DistrictNameE + " (" + item.DistrictCode.ToString() + ")";
                            selectListOBJ.Value = item.DistrictCode.ToString();
                            model.OfficeTypeList.Add(selectListOBJ);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// ChallanMatrixDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChallanMatrixWrapperModel ChallanMatrixDetails(ChallanMatrixLogRequestModel model)
        {
            ChallanMatrixWrapperModel responseModel = new ChallanMatrixWrapperModel();
            responseModel.ChallanMatrixXMLLogDetailList = new List<ChallanMatrixXMLLogDetail>();
            try
            {
                ApplicationDetailsService service = new ApplicationDetailsService();
                RequestModel reqModel = new RequestModel();
                reqModel.IsDRO = model.IsDRO;
                reqModel.SROCode = model.SROCode;
                reqModel.Datetime_FromDate = model.Datetime_FromDate;
                reqModel.Datetime_ToDate = model.Datetime_ToDate;              
                reqModel.Request = model.Request;
                reqModel.Response = model.Response;

                ChallanMatrixXMLWrapperModel wrapperModel = service.GetChallanMatrixXMLLog(reqModel);
                if (wrapperModel.IsError)
                {
                    responseModel.IsErrorField = true;
                    responseModel.ErrorMessageField = wrapperModel.ErrorMessage;
                }
                else
                {
                    if (wrapperModel.ChallanMatrixXMLLogList != null)
                    {
                        if (wrapperModel.ChallanMatrixXMLLogList.Count() > 0)
                        {
                            foreach (var item in wrapperModel.ChallanMatrixXMLLogList)
                            {
                                ChallanMatrixXMLLogDetail remOBJ = new ChallanMatrixXMLLogDetail();
                                remOBJ.RequestXMLID = item.RequestXMLID;                                
                                remOBJ.SROCode = item.SROCode;
                                remOBJ.RequestDateTime = item.RequestDateTime;
                                remOBJ.ResponseDateTime = item.ResponseDateTime;
                                remOBJ.IsExceptionInRequest = item.IsExceptionInRequest;
                                remOBJ.RequestExceptionDetails = item.RequestExceptionDetails;
                                remOBJ.IsExceptionInResponse = item.IsExceptionInResponse;
                                remOBJ.ResponseExceptionDetails = item.ResponseExceptionDetails;
                                //remOBJ.IsDRO = item.IsDRO;
                                //remOBJ.DROCode = item.DROCode;
                                remOBJ.DownloadXmlBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownloadChallanMatrixXML('" + (item.RequestXMLID) + "')>Download</button>";
                                responseModel.ChallanMatrixXMLLogDetailList.Add(remOBJ);
                            }
                        }
                    }
                }
                return responseModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadChallanMatrixXML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownloadChallanMatrixXML(ChallanMatrixLogRequestModel model)
        {
            FileDownloadModel resModel = new FileDownloadModel();
            ApplicationDetailsService service = new ApplicationDetailsService();

            ChallanMatrixRequestModel reqModel = new ChallanMatrixRequestModel();           

            reqModel.RequestXMLID = model.RequestXMLID;
            ChallanMatrixDwnModel serviceResponse = service.DownloadChallanMatrixXMLLog(reqModel);
            if (serviceResponse.isError)
            {
                resModel.IsErrorField = serviceResponse.isError;
                resModel.SErrorMsgField = serviceResponse.sErrorMsg;
            }
            else
            {
                resModel.FileContentField = serviceResponse.FileContent;
            }
            return resModel;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}