#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   CCConversionLogDAL.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15-09-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL for CC Conversion Logs.
*/
#endregion

using CustomModels.Models.Remittance.CCConversionLog;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.PreRegApplicationDetailsService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class CCConversionLogDAL : ICCConversionLog
    {
        /// <summary>
        /// To return model to show CC Conversion Logs View
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public CCConversionLogWrapperModel CCConversionLogView()
        {
            ApiCommonFunctions objCommon = new ApiCommonFunctions();
            CCConversionLogWrapperModel model = new CCConversionLogWrapperModel();
            try
            {
                model.DocumentType = objCommon.GetDocumentType();
                model.DocumentTypeID = 1; //For selecting by default 'Document Type'

                //Date of Deployment is set as default FromDate to be loaded in View
                model.FromDate = new DateTime(2020, 08, 31).ToString("dd/MM/yyyy");
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// To return CC Conversion Logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns model</returns>
        public CCConversionLogWrapperModel CCConversionLogDetails(CCConversionLogReqModel model)
        {
            CCConversionLogWrapperModel responseModel = new CCConversionLogWrapperModel();
            responseModel.CCConversionLogDetailsList = new List<CCConversionLogDetails>();

            try
            {
                ApplicationDetailsService service = new ApplicationDetailsService();
                CCConversionLogServiceReqModel serviceModel = new CCConversionLogServiceReqModel();
                serviceModel.FromDate = model.FromDate;
                serviceModel.ToDate = model.ToDate;
                serviceModel.DocumentTypeID = model.DocumentTypeID;
                serviceModel.DistinctLogs = model.DistinctLogs;

                //Commented by Madhusoodan on 29-09-2020
                //int i = 1;

                CCConversionLogServiceWrapperModel wrapperModel = service.GetCCConversionLog(serviceModel);

                var modelList = wrapperModel.CCConversionLogServiceDetailsList.Skip(model.startLen).Take(model.totalNum);
                
                if (wrapperModel.IsError)
                {
                    responseModel.IsErrorField = true;
                    responseModel.ErrorMessageField = wrapperModel.ErrorMessage;
                }
                else
                {
                    if (modelList != null && modelList.Count() > 0)
                    {
                        foreach (var item in modelList)
                        {
                            CCConversionLogDetails ccConvLogObj = new CCConversionLogDetails();
                            //Commented and added by Madhusoodan on 29-09-20 to display Sr. No. properly
                            //ccConvLogObj.SrNo = i++;
                            ccConvLogObj.SrNo = ++model.startLen;

                            ccConvLogObj.LogID = item.LogID;
                            ccConvLogObj.UserID = item.UserID;
                            ccConvLogObj.UserName = item.UserName;
                            ccConvLogObj.SROCode = item.SROCode;
                            ccConvLogObj.DocumentID = item.DocumentID;
                            ccConvLogObj.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            
                            //Commented and added by Madhusoodan on 07/10/2020
                            //ccConvLogObj.LogDateTime = item.LogDateTime.ToString();
                            ccConvLogObj.LogDateTime = item.LogDateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            
                            //Added by Madhusoodan on 07-10-2020 to add below column in datatable
                            ccConvLogObj.IsConvertedUsingImgMagick = item.IsConvertedUsingImgMagick;

                            //Added by Madhusoodan on 12-10-2020 to add below column in datatable
                            ccConvLogObj.CCID = item.CCID;

                            responseModel.CCConversionLogDetailsList.Add(ccConvLogObj);
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
        /// To return total count of CC Conversion Logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns>count</returns>
        public int GetCCConversionLogDetailsTotalCount(CCConversionLogReqModel model)
        {
            int count = 0;
            CCConversionLogWrapperModel responseModel = new CCConversionLogWrapperModel();
            
            try
            {
                ApplicationDetailsService service = new ApplicationDetailsService();
                CCConversionLogServiceReqModel serviceModel = new CCConversionLogServiceReqModel();
                serviceModel.FromDate = model.FromDate;
                serviceModel.ToDate = model.ToDate;
                serviceModel.DocumentTypeID = model.DocumentTypeID;
                serviceModel.DistinctLogs = model.DistinctLogs;

                CCConversionLogServiceWrapperModel wrapperModel = service.GetCCConversionLog(serviceModel);
                count = wrapperModel.CCConversionLogServiceDetailsList.Count();

                return count;
            }
            catch (Exception)
            {
                throw;
            }   
        }
        
    }
}
