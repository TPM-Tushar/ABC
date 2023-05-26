using CustomModels.Models.Remittance.XELFileStorageDetails;
using ECDataAPI.Areas.Remittance.Interface;
//using ECDataAPI.EcDataService;
using ECDataAPI.SRToCentralComService;
using ECDataAPI.Entity.ECDATADOCS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class XELFileStorageDetailsDAL : IXELFileStorageDetails
    {
        ECDATA_DOCS_Entities ecDATA_DOCS_Entities = null;

        public XELFileStorageViewModel XELFileStorageView(int OfficeID)
        {
            XELFileStorageViewModel model = new XELFileStorageViewModel();
            //model.RootDirectory = "Root Directory path";
            //model.TotalSpace = "Total Space";
            //model.UsedSpace = "Used Space";
            //model.FreeSpace = "Free Space";
            //commented by shubham bhagat on 16-04-2020
            return model;
        }

        public XELFileStorageWrapperModel XELFileOfficeList(XELFileStorageViewModel reqModel)
        {
            try
            {
                XELFileStorageWrapperModel resModel = new XELFileStorageWrapperModel();
                resModel.XELFileOfficeListRESModelList = new List<XELFileOfficeListRESModel>();
                ecDATA_DOCS_Entities = new ECDATA_DOCS_Entities();
                bool isDR = reqModel.OfficeType == "SR" ? false : true;
                var resultList = ecDATA_DOCS_Entities.USP_XEL_FILE_COUNT_OFFICEWISE(isDR).ToList();
                int count = 1;

                if (reqModel.IsExcel)
                {
                    foreach (var item in resultList)
                    {
                        resModel.XELFileOfficeListRESModelList.Add(new XELFileOfficeListRESModel()
                        {
                            SerialNo = count++,
                            OfficeName = item.OFFICENAME,
                            NoOfFiles = item.FILE_COUNT.ToString(),
                            TotalSizeOnDiskInMB = item.TOTAL_SIZE_ON_DISK,
                            LastCentralizedOn = item.LAST_CENTRALIZED_ON == null ? "" : ((DateTime)item.LAST_CENTRALIZED_ON).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }
                }
                else
                {
                    foreach (var item in resultList)
                    {
                        resModel.XELFileOfficeListRESModelList.Add(new XELFileOfficeListRESModel()
                        {
                            SerialNo = count++,
                            OfficeName = item.OFFICENAME,
                            NoOfFiles = item.FILE_COUNT == 0 ? item.FILE_COUNT.ToString() : "<a  title='Click here' style = 'width:75%;font-weight:bolder;color:green' onclick=XELFileListOfficeWise('" + item.OFFICECODE + "')><i>" + item.FILE_COUNT + "</i></a>",
                            //NoOfFiles = item.FILE_COUNT == 0 ? item.FILE_COUNT.ToString() : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=XELFileListOfficeWise('" + item.OFFICECODE + "')><i style = 'padding-right:3%;'></i>" + item.FILE_COUNT + "</button>",
                            TotalSizeOnDiskInMB = item.TOTAL_SIZE_ON_DISK,
                            LastCentralizedOn = item.LAST_CENTRALIZED_ON == null ? "" : ((DateTime)item.LAST_CENTRALIZED_ON).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }
                }

                resModel.TotalRecords = resultList.Count();
                return resModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ecDATA_DOCS_Entities != null)
                {
                    ecDATA_DOCS_Entities.Dispose();
                }
            }

            //resModel.XELFileOfficeListRESModelList.Add(new XELFileOfficeListRESModel()
            //{
            //    SerialNo = 1,
            //    OfficeName = "gandhinagar",
            //    NoOfFiles = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=XELFileListOfficeWise('" + 35 + "')><i style = 'padding-right:3%;'></i>"+200+"</button>",
            //    TotalSizeOnDiskInMB = 5000,
            //    LastCentralizedOn = "10-03-2020"
            //});


        }

        public XELFileStorageWrapperModel XELFileListOfficeWise(XELFileStorageViewModel reqModel)
        {
            try
            {
                XELFileStorageWrapperModel resModel = new XELFileStorageWrapperModel();
                resModel.XELFileListOfficeWiseRESModelList = new List<XELFileListOfficeWiseRESModel>();
                ecDATA_DOCS_Entities = new ECDATA_DOCS_Entities();
                bool isDR = reqModel.OfficeType == "SR" ? false : true;
                int SROCode_INT = Convert.ToInt32(reqModel.SROCode);
                if (reqModel.IsExcel)
                {
                    var resultList = ecDATA_DOCS_Entities.USP_XEL_FILE_LIST_OFFICEWISE(SROCode_INT, isDR).ToList();

                    int count = 1;

                    foreach (var item in resultList)
                    {
                        resModel.XELFileListOfficeWiseRESModelList.Add(new XELFileListOfficeWiseRESModel()
                        {
                            SerialNo = count++,
                            FileName = item.FILENAME,
                            FileSizeInMB = item.FILESIZE == null ? 0 : item.FILESIZE.Value,
                            FileDateTime = item.FILEDATETIME == null ? "" : ((DateTime)item.FILEDATETIME).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            FilePath = item.FILEPATH,
                            EventStartDate = item.EventStartDate == null ? "" : ((DateTime)item.EventStartDate).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            EventEndDate = item.EventEndDate == null ? "" : ((DateTime)item.EventEndDate).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            FileReadDateTime = item.FileReadDateTime == null ? "" : ((DateTime)item.FileReadDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }

                    resModel.TotalRecords = resultList.Count();
                }
                else
                {
                    var resultList = String.IsNullOrEmpty(reqModel.SearchValue) ?
                        ecDATA_DOCS_Entities.USP_XEL_FILE_LIST_OFFICEWISE(SROCode_INT, isDR).Skip(reqModel.StartLen).Take(reqModel.TotalNum).ToList() :
                    ecDATA_DOCS_Entities.USP_XEL_FILE_LIST_OFFICEWISE(SROCode_INT, isDR).ToList();

                    int count = 1;

                    foreach (var item in resultList)
                    {
                        resModel.XELFileListOfficeWiseRESModelList.Add(new XELFileListOfficeWiseRESModel()
                        {
                            SerialNo = count++,
                            FileName = item.FILENAME,
                            FileSizeInMB = item.FILESIZE == null ? 0 : item.FILESIZE.Value,
                            FileDateTime = item.FILEDATETIME == null ? "" : ((DateTime)item.FILEDATETIME).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            //FilePath = item.FILEPATH,
                            FilePath = "<a  title='Click here' style = 'width:75%;' onclick=XELFileDownloadPathVerify('" + item.FILEPATH.Replace('\\', '*').Replace(' ', '$') + "')><i>" + item.FILEPATH + "</i></a>",
                            EventStartDate = item.EventStartDate == null ? "" : ((DateTime)item.EventStartDate).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            EventEndDate = item.EventEndDate == null ? "" : ((DateTime)item.EventEndDate).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            FileReadDateTime = item.FileReadDateTime == null ? "" : ((DateTime)item.FileReadDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }

                    resModel.TotalRecords = resultList.Count();
                }


                return resModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ecDATA_DOCS_Entities != null)
                {
                    ecDATA_DOCS_Entities.Dispose();
                }
            }
            //resModel.XELFileListOfficeWiseRESModelList.Add(new XELFileListOfficeWiseRESModel()
            //{
            //    SerialNo = 1,
            //    FileName = "gandhinagar.xel",
            //    FileSizeInMB = 10,
            //    FileDateTime = "10-03-2020",
            //    FilePath = "D:\\TFS\\Kaveri\\1. Source Code\\Maintenance Portal",
            //    EventStartDate = "8-03-2020",
            //    EventEndDate = "7-03-2020",
            //    FileDownload = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=FileDownload('" + "Filepath" + "')><i style = 'padding-right:3%;'></i>Download</button>",
            //});       
        }

        public XELFileStorageViewModel RootDirectoryTable(XELFileStorageViewModel reqModel)
        {
            try
            {
                XELFileStorageViewModel resModel = new XELFileStorageViewModel()
                {
                    RootDirInfoModelList = new List<CustomModels.Models.Remittance.XELFileStorageDetails.RootDirInfoModel>()
                };

                ecDATA_DOCS_Entities = new ECDATA_DOCS_Entities();
                bool isDR = reqModel.OfficeType == "SR" ? false : true;
                var USP_XEL_Files_RootDirPath_List_officewiseList = ecDATA_DOCS_Entities.USP_XEL_Files_RootDirPath_List_officewise(isDR).ToList();
                if (USP_XEL_Files_RootDirPath_List_officewiseList != null)
                {
                    if (USP_XEL_Files_RootDirPath_List_officewiseList.Count() > 0)
                    {
                        #region ADDED AND COMMENTED BY SHUBHAM BHAGAT ON 09-06-2020
                        //ECDataService service = new ECDataService();
                        //XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                        SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();
                        XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                        #endregion

                        serviceREQMODEL.RootDirectoryPathList = USP_XEL_Files_RootDirPath_List_officewiseList.ToArray();
                        //serviceREQMODEL.RootDirectoryPathList = new string[USP_XEL_Files_RootDirPath_List_officewiseList.Count()];
                        //int i = 0;
                        //foreach (var item in USP_XEL_Files_RootDirPath_List_officewiseList)
                        //{
                        //    serviceREQMODEL.RootDirectoryPathList[i++] = item;
                        //}


                        XELFilesResModel serviceRESMODEL = service.XELRootDirectoryInfo(serviceREQMODEL);

                        if (serviceRESMODEL != null)
                        {
                            if (serviceRESMODEL.RootDirInfoModelList != null)
                            {
                                //int count = 1;
                                //foreach (var item in serviceRESMODEL.RootDirInfoModelList)
                                //{
                                //    resModel.RootDirInfoModelList.Add(new CustomModels.Models.Remittance.XELFileStorageDetails.RootDirInfoModel()
                                //    {
                                //        SerialNo = count++,
                                //        RootDirectory = item.RootDirectory,
                                //        TotalSpace = item.TotalSpace,
                                //        UsedSpace = item.UsedSpace,
                                //        XELSpace = item.XELSpace,
                                //        FreeSpace = item.FreeSpace
                                //    });
                                //}

                                resModel.RootDirInfoModelList = serviceRESMODEL.RootDirInfoModelList.Select((item, i) => new CustomModels.Models.Remittance.XELFileStorageDetails.RootDirInfoModel()
                                {
                                    SerialNo = ++i,
                                    RootDirectory = item.RootDirectory,
                                    TotalSpace = item.TotalSpace,
                                    UsedSpace = item.UsedSpace,
                                    XELSpace = item.XELSpace,
                                    FreeSpace = item.FreeSpace
                                }).ToList();

                            }
                        }
                    }
                }
                else
                {
                }

                return resModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ecDATA_DOCS_Entities != null)
                {
                    ecDATA_DOCS_Entities.Dispose();
                }
            }

            //resModel.RootDirInfoModelList.Add(new RootDirInfoModel()
            //{
            //    FreeSpace = "1",
            //    RootDirectory = "2",
            //    TotalSpace = "3",
            //    UsedSpace = "4",
            //    XELSpace = "5"
            //});
            //resModel.RootDirInfoModelList.Add(new RootDirInfoModel()
            //{
            //    FreeSpace = "6",
            //    RootDirectory = "7",
            //    TotalSpace = "8",
            //    UsedSpace = "9",
            //    XELSpace = "10"
            //});
        }

        public XELFileStorageViewModel XELFileDownloadPathVerify(XELFileStorageViewModel reqModel)
        {
            try
            {
                XELFileStorageViewModel resModel = new XELFileStorageViewModel();

                #region ADDED AND COMMENTED BY SHUBHAM BHAGAT ON 09-06-2020
                //ECDataService service = new ECDataService();
                //XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();
                XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                #endregion

                serviceREQMODEL.FileDownloadPath = reqModel.FileDownloadPath;
                XELFilesResModel serviceRESMODEL = service.XELFileDownloadPathVerify(serviceREQMODEL);

                if (serviceRESMODEL != null)
                {
                    resModel.IsFileExistAtDownloadPath = serviceRESMODEL.IsFileExistAtDownloadPath;
                }
                return resModel;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        public XELFileStorageWrapperModel XELFileDownload(XELFileStorageViewModel reqModel)
        {
            try
            {
                XELFileStorageWrapperModel resModel = new XELFileStorageWrapperModel();
                #region ADDED AND COMMENTED BY SHUBHAM BHAGAT ON 09-06-2020
                //ECDataService service = new ECDataService();
                //XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                SRToCentralComService.SRToCentralComService service = new SRToCentralComService.SRToCentralComService();
                XELFilesReqModel serviceREQMODEL = new XELFilesReqModel();
                #endregion

                serviceREQMODEL.FileDownloadPath = reqModel.FileDownloadPath;
                XELFilesResModel serviceRESMODEL = service.XELFileDownload(serviceREQMODEL);

                if (serviceRESMODEL != null)
                {
                    resModel.FileContentField = serviceRESMODEL.FileContentField;
                }
                return resModel;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}