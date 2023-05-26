using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GauriServicesApi.Interface;
using CustomModels.Models.SearchCertificate;
using System.Web.Mvc;
using GauriServicesApi.Entity;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Configuration;
using GauriServicesApi.Common;

namespace GauriServicesApi.DAL
{
    public class SearchCertificateDAL : ISearchCertificate,IDisposable
    {
        GAURIEntities dbContext = null;
        public SearchCertificateModel SearchCertificate()
        {

            SearchCertificateModel ObjCertificate = new SearchCertificateModel();
            ObjCertificate.ServiceList = GetServiceList();
            ObjCertificate.DepartmentsList = GetDepartmentList();

            return ObjCertificate;
        }

        public SearchCertificateModel GetCertificateDetails(CertificateModel ObjCertificate)
        {
            SearchCertificateModel ObjCertDal = new SearchCertificateModel();

            try
            {
                dbContext = new GAURIEntities();

                 ObjCertDal.ServiceList = GetServiceList();
                ObjCertDal.DepartmentsList = GetDepartmentList();

                FRM_FirmDetails ObjfirmDetails = dbContext.FRM_FirmDetails.Where(f => f.FinalRegistrationNumber == ObjCertificate.CertificateNumber).FirstOrDefault();
                if (ObjfirmDetails == null)
                {

                    ObjCertDal.errorMessage = "Invalid Certificate Number";
                    return ObjCertDal;
                }
                ObjCertDal.ApplicationDate = ObjfirmDetails.ApplicationDate.ToString();
                
                ObjCertDal.NameOfEstablishment = ObjfirmDetails.FirmName;
                ObjCertDal.TypeOfEstablishment = "Firm";



                ObjCertDal.AddressString = GetAddressDetails(ObjfirmDetails.AddressID);



                FRM_FinalUploadedDocs ObjUploadDoc = dbContext.FRM_FinalUploadedDocs.Where(d => d.FirmID == ObjfirmDetails.FirmID).FirstOrDefault();

                if (ObjUploadDoc == null)
                {

                    ObjCertDal.errorMessage = "No Certificate Found";
                    ObjCertDal.CertificateName = "-";

                    return ObjCertDal;
                }
                else
                {
                    string ServiceName = dbContext.MAS_Services.Where(m => m.ServiceID == ObjCertificate.ServiceID).FirstOrDefault().ServiceName;
                    ObjCertDal.CertificateName = ServiceName;
                    ObjCertDal.IssuedOn = ObjUploadDoc.FileUploadDate.ToString();
                    if(ObjfirmDetails.AtWill)
                    {
                        ObjCertDal.ValidUpto = "At Will";
                    }
                    else
                    {
                       // DateTime datee = (DateTime)ObjfirmDetails.EstablishmentDate;
                        ObjCertDal.ValidUpto = ((DateTime)ObjfirmDetails.DateOfRegistration).AddMonths((short)ObjfirmDetails.DurationInMonth).ToString();
                    }
                    
                }
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }

            return ObjCertDal;
        }

        public List<SelectListItem> GetServiceList()
        {
            List<SelectListItem> ServiceList = new List<SelectListItem>();
            SelectListItem Objlitem = new SelectListItem();
            Objlitem.Selected = true;
            Objlitem.Text = "Select Service";
            Objlitem.Value = null;
            ServiceList.Add(Objlitem);
            GAURIEntities dbContext = null;
            using (dbContext = new GAURIEntities())
            {

                MAS_Services ServiceIdvar = dbContext.MAS_Services.Where(m => m.ServiceName == "Firm Registration").FirstOrDefault();
                SelectListItem Objselectlist = new SelectListItem();
                Objselectlist.Text = ServiceIdvar.ServiceName;
                Objselectlist.Value = Convert.ToString(ServiceIdvar.ServiceID);
                ServiceList.Add(Objselectlist);
                //for all service showing in dropdown
                ////foreach (var ServiceIdvar in dbContext.MAS_Services)
                ////{
                ////    SelectListItem Objselectlist = new SelectListItem();
                ////    Objselectlist.Text = ServiceIdvar.ServiceName;
                ////    Objselectlist.Value = Convert.ToString(ServiceIdvar.ServiceID);
                ////    ServiceList.Add(Objselectlist);
                ////}
            }
            return ServiceList;
        }

        public List<SelectListItem> GetDepartmentList()
        {
            List<SelectListItem> ObjDepartmentList = new List<SelectListItem>();

            SelectListItem objFirst = new SelectListItem();
            objFirst.Selected = true;
            objFirst.Text = "Select Department";
            objFirst.Value = null;
            ObjDepartmentList.Add(objFirst);

            SelectListItem objSecond = new SelectListItem();
            objSecond.Text = "Registration";
            objSecond.Value = "1";
            ObjDepartmentList.Add(objSecond);

            //SelectListItem objThird = new SelectListItem();
            //objThird.Text = "Rural";
            //objThird.Value = "2";
            //ObjDepartmentList.Add(objThird);

            //SelectListItem objFourth = new SelectListItem();
            //objFourth.Text = "Urban";
            //objFourth.Value = "3";
            //ObjDepartmentList.Add(objFourth);


            return ObjDepartmentList;
        }

        public string GetAddressDetails(long? objCertforAdd)
        {

            // FRM_FirmDetails ObjfirmDetails = dbContext.FRM_FirmDetails.Where(f => f.FinalRegistrationNumber == objCertforAdd.CertificateNumber).FirstOrDefault();


            FRM_AddressDetails ObjAddress = dbContext.FRM_AddressDetails.Where(a => a.AddressID == objCertforAdd).FirstOrDefault();

            // var villageName = dbContext.MAS_Villages.Where(v => v.VillageID == ObjAddress.VillageID).FirstOrDefault().VillageName;
            try
            {
                var stateName = dbContext.MAS_States.Where(s => s.StateID == ObjAddress.StateID).FirstOrDefault().StateName;
                var districtName = dbContext.MAS_Districts.Where(d => d.DistrictID == ObjAddress.DistrictID).FirstOrDefault().DistrictName;
                var countryName = dbContext.MAS_Countries.Where(c => c.CountryID == ObjAddress.CountryID).FirstOrDefault().CountryName;

                List<string> strr = new List<string>();


                if (ObjAddress.FloorNumber.HasValue)
                {
                    strr.Add(ObjAddress.FloorNumber.ToString());
                }
                if (ObjAddress.PremiseName != null)
                {
                    strr.Add(ObjAddress.PremiseName);
                }
                if (ObjAddress.WaddoName != null)
                {
                    strr.Add(ObjAddress.WaddoName);
                }
                if (ObjAddress.StreetSectorDetails != null)
                {
                    strr.Add(ObjAddress.StreetSectorDetails);
                }
                if (ObjAddress.AreaDetails != null)
                {
                    strr.Add(ObjAddress.AreaDetails);
                }
                if (ObjAddress.Village != null)
                {
                    strr.Add(ObjAddress.Village);
                }
                if (ObjAddress.Town != null)
                {
                    strr.Add(ObjAddress.Town);
                }
                if (ObjAddress.SubDistrict != null)
                {
                    strr.Add(ObjAddress.SubDistrict);
                }
                if (districtName != null)
                {
                    strr.Add(districtName);
                }
                if (stateName != null)
                {
                    strr.Add(stateName);
                }
                if (countryName != null)
                {
                    strr.Add(countryName);
                }
                if (ObjAddress.PIN != 0)
                {
                    strr.Add(ObjAddress.PIN.ToString());
                }


                string strAddRes = String.Join(", ", strr);

                return strAddRes;

            }
            catch (Exception )
            {

                throw ;
            }
        }

        //public short GetService(short varServiceID)
        //{

        //    switch (varServiceID)
        //    {
        //        case 1:
        //            return 1;


        //        case 2:
        //            return 2;


        //        case 3:
        //            return 3;


        //        case 4:
        //            return 4;


        //        case 5:
        //            return 5;


        //        case 6:
        //            return 6;


        //        default:
        //            return varServiceID;


        //    }
        //}

        public SearchCertificateModel GetCertificateByteArray(CertificateModel ObjCertificate)
        {
            // string FirmUploadPath = ConfigurationManager.AppSettings["FileStorageVirtualPath"];
            SearchCertificateModel objFileDisplayModel = null;
            try
            {
                dbContext = new GAURIEntities();
                objFileDisplayModel = new SearchCertificateModel();

                FRM_FirmDetails ObjfirmDetails = dbContext.FRM_FirmDetails.Where(f => f.FinalRegistrationNumber == ObjCertificate.CertificateNumber).FirstOrDefault();

                if (ObjfirmDetails == null)
                {

                    objFileDisplayModel.errorMessage = "Invalid Certificate Number";
                    return objFileDisplayModel;
                }

                FRM_FinalUploadedDocs objFinalUploadDocs = dbContext.FRM_FinalUploadedDocs.Where(m => m.FirmID == ObjfirmDetails.FirmID).FirstOrDefault();
                //if (objFinalUploadDocs == null)
                //{
                //    objFileDisplayModel.errorMessage = "File not found ,Please contact admin";
                //    SearchCertificateModel objFiledisplayModel = CreatePDFfromString(objFileDisplayModel);
                //    return objFiledisplayModel;
                //}
                //else
                //{
                    // string VirtualFilePath = objFinalUploadDocs.FileServerPath;      
                    objFileDisplayModel.FirmID = ObjfirmDetails.FirmID;

                    string virtualServerPath = objFinalUploadDocs.VirtualServerPath;

                    DownloadUploadFiles downloadUploadFiles = new DownloadUploadFiles();
                    //try
                    //{
                        //if (downloadUploadFiles.IsFileReadableWithVirtual(objFinalUploadDocs.VirtualServerPath)) 
                        //{
                            objFileDisplayModel.fileBytes = downloadUploadFiles.DownloadInByteArrayVirtualPath(virtualServerPath);
                            return objFileDisplayModel;
                       // }
                        //else
                        //{
                        //    objFileDisplayModel.errorMessage = "File not found ,Please contact admin";
                        //    SearchCertificateModel objFiledisplayModel = CreatePDFfromString(objFileDisplayModel);
                        //    objFiledisplayModel.FirmID = ObjfirmDetails.FirmID;
                        //    return objFiledisplayModel;
                        //}
                //    }
                //    catch (Exception)
                //    {
                //        objFileDisplayModel.errorMessage = "Unable to read file ,Please contact admin";
                //        SearchCertificateModel objFiledisplayModel = CreatePDFfromString(objFileDisplayModel);
                //        objFiledisplayModel.FirmID = ObjfirmDetails.FirmID;
                //        return objFiledisplayModel;
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private SearchCertificateModel CreatePDFfromString(SearchCertificateModel objFileDisplayModel)
        //{
        //    try
        //    {
        //        StringReader sr = new StringReader(objFileDisplayModel.errorMessage);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //            pdfDoc.Open();
        //            htmlparser.Parse(sr);
        //            pdfDoc.Close();
        //            byte[] bytes = memoryStream.ToArray();
        //            memoryStream.Close();
        //            objFileDisplayModel.isFileExist = false;
        //            objFileDisplayModel.fileBytes = bytes;
        //        }

        //        //--
        //        PdfReader pr = new PdfReader(objFileDisplayModel.fileBytes);
        //        MemoryStream ms = new MemoryStream();
        //        PdfStamper stamper = new PdfStamper(pr, ms);

        //        Dictionary<String, String> info = pr.Info;
        //        info["Title"] = "Goa Govt";
        //        stamper.MoreInfo = info;
        //        stamper.Close();
        //        objFileDisplayModel.fileBytes = ms.ToArray();
        //        //--
        //        return objFileDisplayModel;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}