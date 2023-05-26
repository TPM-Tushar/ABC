using CustomModels.Common;
using CustomModels.Models.Common;
using ECDataAPI.BAL;
using ECDataAPI.Common;
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc;
//using System.Web.Mvc;

namespace ECDataAPI.Controllers
{
    public class CommonsApiController : ApiController
    {
        ICommonInterface InterfaceReference = new CommonBAL();

        [HttpPost]
        [Route("api/CommonsApiController/FillMasterDropDownModel")]
        public IHttpActionResult FillMasterDropDownModel(EnumDropDownListModel dropdownEnum)
        {
            try
            {
                MasterDropDownModel model = new ApiCommonFunctions().FillMasterDropDownModel(dropdownEnum);
                return Ok(model);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        [Route("api/CommonsApiController/GetOfficesDropDown")]
        public IHttpActionResult GetOfficesDropDown(short OfficeTypeID, short? districtId)
        {
            try
            {
                List<System.Web.Mvc.SelectListItem> List = new ApiCommonFunctions().GetOfficesDropDown(OfficeTypeID, districtId);
                return Ok(List);
            }
            catch (Exception)
            {
                throw;
            }
        }




        [HttpGet]
        [Route("api/CommonsApiController/FillMasterDropDownModel")]
        public IHttpActionResult FillMasterDropDownModel()
        {
            try
            {
                MasterDropDownModel model = InterfaceReference.FillMasterDropDownModel();

                return Ok(model);
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPost]
        [Route("api/CommonsApiController/GetMenuDetailsToHighlight")]
        public IHttpActionResult GetMenuDetailsToHighlight(MenuHighlightReqModel reqModel)
        {
            try
            {
                MenuHighlightResponseModel model = InterfaceReference.GetMenuDetailsToHighlight(reqModel);

                return Ok(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

         




        //[HttpPost]
        //[Route("api/CommonsApiController/AddInformationToDocument")]
        //public IHttpActionResult AddInformationToDocument(FileDisplayModel model)
        //{
        //    try
        //    {
        //        FileDisplayModel objFileDisplayModel = InterfaceReference.AddInformationToDocument(model);
        //        return Ok(objFileDisplayModel);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}






        //[HttpPost]
        //[Route("api/CommonsApiController/UploadSignedDocument")]
        //public IHttpActionResult UploadSignedDocument(FileDisplayModel fileData)
        //{
        //    try
        //    {
        //        string signedDocument = InterfaceReference.UploadSignedDocument(fileData);
        //        return Ok(signedDocument);

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        [HttpPost]
        [Route("api/CommonsApiController/InsertAuditMvcEventLoging")]
        public IHttpActionResult InsertAuditMvcEventLoging(EventAuditLoging LogData)
        {
            ApiCommonFunctions common = new ApiCommonFunctions();
            try
            {
                bool result = common.InsertAuditMvcEventLoging(LogData);
                return Ok(result);

            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/CommonsApiController/GetSROOfficeListByDistrictID")]
        public IHttpActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<System.Web.Mvc.SelectListItem> sroOfficeList = InterfaceReference.GetSROfficesListByDisrictID(DistrictID);
                return Ok(sroOfficeList);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/CommonsApiController/GetCDNumberList")]
        public IHttpActionResult GetCDNumberList(int SROCode, String FirstRecord)
        {
            try
            {
                List<System.Web.Mvc.SelectListItem> CDNumberList = InterfaceReference.GetCDNumberList(SROCode, FirstRecord);
                return Ok(CDNumberList);
            }
            catch (Exception)
            {
                throw;
            }
        }



        //Added By Raman Kalegaonkar on 20-05-2019
        [HttpGet]
        [Route("api/CommonsApiController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                string SroName = InterfaceReference.GetSroName(SROfficeID);
                return Ok(SroName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added By Raman Kalegaonkar on 20-05-2019
        [HttpGet]
        [Route("api/CommonsApiController/GetDroName")]
        public IHttpActionResult GetDroName(int DistrictID)
        {
            try
            {
                string DroName = InterfaceReference.GetDroName(DistrictID);
                return Ok(DroName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Raman Kalegaonkar on 27-06-2019
        [HttpGet]
        [Route("api/CommonsApiController/GetSROOfficeListByDistrictIDWithFirstRecord")]
        public IHttpActionResult GetSROOfficeListByDistrictId(long DistrictID,string FirstRecord)
        {
            try
            {
                List<System.Web.Mvc.SelectListItem> sroOfficeList = InterfaceReference.GetSROOfficeListByDistrictIDWithFirstRecord(DistrictID, FirstRecord);
                return Ok(sroOfficeList);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Added by Raman Kalegaonkar on 27-06-2019
        [HttpGet]
        [Route("api/CommonsApiController/GetLevelIdByOfficeId")]
        public IHttpActionResult GetLevelIdByOfficeId(short OfficeID)
        {
            try
            {
                short LevelID= InterfaceReference.GetLevelIdByOfficeId(OfficeID);
                return Ok(LevelID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Raman Kalegaonkar on 03-08-2019
        [HttpGet]
        [Route("api/CommonsApiController/GetSROOfficeListDictionary")]
        public IHttpActionResult GetSROOfficeListDictionary(long DistrictID)
        {
            try
            {
                Dictionary<int, int> SROOfficeListDict = new Dictionary<int, int>();
                SROOfficeListDict = InterfaceReference.GetSROOfficeListDictionary(DistrictID);
                return Ok(SROOfficeListDict);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
