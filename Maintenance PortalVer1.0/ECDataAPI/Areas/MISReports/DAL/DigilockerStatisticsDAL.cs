using CustomModels.Models.MISReports.DigilockerStatistics;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DigilockerStatisticsDAL : IDigilockerStatistics
    {
        KaveriEntities dbContext = null;
        
        public DigiLockerStatisticsViewModel DigilockerStatisticsView(int OfficeID)
        {
            try
            {
                DigiLockerStatisticsViewModel model = new DigiLockerStatisticsViewModel();
                
                DateTime now = DateTime.Now;
                var startDate = new DateTime(2021, 04, 24);
                var toDate = new DateTime(now.Year, now.Month, now.Day);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = toDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);


                return model;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        
        public DigilockerStatisticsResponseModel DigilockerStatisticsReportDetails(DigiLockerStatisticsViewModel model)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {
                dbContext = new KaveriEntities();
                DigilockerStatisticsResponseModel resModel = new DigilockerStatisticsResponseModel();
                DigilockerStatisticsDetailsModel ReportsDetails = null;
                resModel.DigilockerStatisticsDetailsList = new List<DigilockerStatisticsDetailsModel>();

                
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                
                
                DataSet DigiLockerStatisticsList = objService.GetDigiLockerStatisticsData( FromDate, ToDate);

                resModel.TotalCount = DigiLockerStatisticsList.Tables.Count;

                resModel.DigilockerStatisticsDetailsList = new List<DigilockerStatisticsDetailsModel>();

                if (DigiLockerStatisticsList != null)
                {
                    foreach (DataTable dataTable in DigiLockerStatisticsList.Tables)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ReportsDetails = new DigilockerStatisticsDetailsModel();

                            ReportsDetails.TotalKOSUsers = Convert.ToInt64(dataRow["TotalKOSUsers"]);
                            ReportsDetails.TotalKOSUsersLinkedToDigilocker = Convert.ToInt64(dataRow["TotalKOSUsersLinkedToDigilocker"]);
                            ReportsDetails.TotalApplicationsSubmitted = Convert.ToInt64(dataRow["TotalApplicationsSubmitted"]);
                            ReportsDetails.TotalApplicationsLinkedToDigilocker = Convert.ToInt64(dataRow["TotalApplicationsLinkedToDigilocker"]);
                            ReportsDetails.TotalCertificatesIssued = Convert.ToInt64(dataRow["TotalCertificatesIssued"]);
                            ReportsDetails.TotalCertificatesPushedToDigilocker = Convert.ToInt64(dataRow["TotalCertificatesPushedToDigilocker"]);
                            ReportsDetails.TotalCertificatesSearchedFromDigilocker = Convert.ToInt64(dataRow["TotalCertificatesSearchedFromDigilocker"]);


                            resModel.DigilockerStatisticsDetailsList.Add(ReportsDetails);

                        }
                    }
                    
                }
                return resModel;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }

            

        }



    }
}