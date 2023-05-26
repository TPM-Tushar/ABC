#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DblVerificationXMLLogDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.DblVerificationXMLLog;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.PreRegApplicationDetailsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class DblVerificationXMLLogDAL: IDblVerificationXMLLog, IDisposable
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        /// <summary>
        /// DblVeriXMLLogDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DblVeriXMLLogWrapperModel DblVeriXMLLogDetails(DblVeriReqXMLLogModel model)
        {
            DblVeriXMLLogWrapperModel responseModel = new DblVeriXMLLogWrapperModel();
            responseModel.DblVeriXMLLogDetailList = new List<DblVeriXMLLogDetail>();
            try
            {
                ApplicationDetailsService service = new ApplicationDetailsService();
                RequestModel reqModel = new RequestModel();
                reqModel.Datetime_FromDate = model.Datetime_FromDate;
                reqModel.Datetime_ToDate = model.Datetime_ToDate;              
                reqModel.Request = model.Request;
                reqModel.Response = model.Response;

                DblVerificationXMLWrapperModel wrapperModel = service.GetDblVerificationXMLLog(reqModel);
                if (wrapperModel.IsError)
                {
                    responseModel.IsErrorField = true;
                    responseModel.ErrorMessageField = wrapperModel.ErrorMessage;
                }
                else
                {
                    if (wrapperModel.DblVerificationXMLLogList != null)
                    {
                        if (wrapperModel.DblVerificationXMLLogList.Count() > 0)
                        {
                            foreach (var item in wrapperModel.DblVerificationXMLLogList)
                            {
                                DblVeriXMLLogDetail remOBJ = new DblVeriXMLLogDetail();
                                remOBJ.RequestXMLID = item.RequestXMLID;
                                remOBJ.SROCode = item.SROCode;
                                remOBJ.RequestDateTime = item.RequestDateTime;
                                remOBJ.ResponseDateTime = item.ResponseDateTime;
                                remOBJ.IsExceptionInRequest = item.IsExceptionInRequest;
                                remOBJ.RequestExceptionDetails = item.RequestExceptionDetails;
                                remOBJ.IsExceptionInResponse = item.IsExceptionInResponse;
                                remOBJ.ResponseExceptionDetails = item.ResponseExceptionDetails;                               
                                remOBJ.DownloadXmlBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownloadDblVeriXML('" + (item.RequestXMLID) + "')>Download</button>";
                                responseModel.DblVeriXMLLogDetailList.Add(remOBJ);
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
        /// DownloadDblVeriXML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownloadDblVeriXML(DblVeriReqXMLLogModel model)
        {
            FileDownloadModel resModel = new FileDownloadModel();
            ApplicationDetailsService service = new ApplicationDetailsService();

            DblVerificationRequestModel reqModel = new DblVerificationRequestModel();

            reqModel.RequestXMLID = model.RequestXMLID;
            DblVerificationDwnModel serviceResponse = service.DownloadDoubleVeriXMLLog(reqModel);
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