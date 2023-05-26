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

using CustomModels.Models.BhoomiMapping;
using ECDataAPI.Areas.BhoomiMapping.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Security;
using System.Text;
using System.IO;
using ECDataUI.Session;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Transactions;
using System.Data;

namespace ECDataAPI.Areas.BhoomiMapping.DAL
{
    public class BhoomiMappingDAL : IBhoomiMapping
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public BhoomiMappingViewModel BhoomiMappingView(int officeID, int LevelID, long UserID)
        {
            try
            {
                BhoomiMappingViewModel BhoomiMappingViewModel = new BhoomiMappingViewModel();
                BhoomiMappingViewModel.SROfficeOrderList = new List<SelectListItem>();
                dbContext = new KaveriEntities();
                var SroList = dbContext.SROMaster.OrderBy(t => t.SRONameE).Select(x => new { x.SRONameE, x.SROCode }).ToList();
                BhoomiMappingViewModel.SROfficeOrderList.Add(new SelectListItem { Text = "Select SRO", Value = "" });

                foreach (var item in SroList)
                {
                    BhoomiMappingViewModel.SROfficeOrderList.Add(new SelectListItem { Text = item.SRONameE, Value = Convert.ToString(item.SROCode) });
                }
                return BhoomiMappingViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Upload(BhoomiMappingUpdateModel model)
        {
            try
            {
                BhoomiMappingUpdateModel bhoomiMappingUpdateModel = new BhoomiMappingUpdateModel();
                List<double> errIdList= new List<double>();
                List<double> SuccessIdList= new List<double>();
                List<double> UnavailableIdList= new List<double>();
                int err = 0;
                int succ = 0;
                int unavailable = 0;
                dbContext = new KaveriEntities();
                DataTable dt = new DataTable();
                dt = model.ExcelTable;
                double KVC;
                double KSC;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["KaveriVillageCode"].ToString() != "")
                        {
                            KVC = Convert.ToDouble(dr["KaveriVillageCode"]);
                            KSC = Convert.ToDouble(dr["KaveriSROCode"]);

                            var query = dbContext.BhoomiMappingDetails.Where(x => x.KaveriVillageCode == KVC && x.KaveriSROCode == KSC).AsEnumerable().FirstOrDefault();
                            if (query != null)
                            {

                                query.BhoomiDistrictCode = Convert.ToDouble(dr["BhoomiDistrictCode"]);
                                query.BhoomiTalukCode = Convert.ToDouble(dr["BhoomiTalukCode"]);
                                query.BhoomiHobiCode = Convert.ToDouble(dr["BhoomiHobiCode"]);
                                //if (Convert.ToDouble(dr["KaveriVillageCode"]) == 20503)
                                //{
                                //    throw new NullReferenceException("object is null.");
                                //}
                                query.BhoomiVillageCode = Convert.ToDouble(dr["BhoomiVillageCode"]);
                                dbContext.Entry(query).State = System.Data.Entity.EntityState.Modified;
                                SuccessIdList.Add(Convert.ToDouble(dr["KaveriVillageCode"]));
                                succ++;
                                query.IsUpdated = true;
                            }
                            else
                            {
                                unavailable++;
                                UnavailableIdList.Add(Convert.ToDouble(dr["KaveriVillageCode"]));
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        err++;
                        errIdList.Add(Convert.ToDouble(dr["KaveriVillageCode"]));
                        continue;
                    }
                }
                dbContext.SaveChanges();

                if (err > 0 && succ > 0 && unavailable > 0)
                {
                    return "<span style='font-weight:600;'>Update Completed with error in following Kaveri Village Code:</span><br><br>" + string.Join(", ", errIdList) + "<br><br><span style='font-weight:600;'>Update Completed with Success for following Kaveri Village Code:</span><br><br>" + string.Join(", ", SuccessIdList) + "<br><br><span style='font-weight:600;'>Following Village Code in Excel sheet are missing from database:</span><br><br>" + string.Join(", ", UnavailableIdList);
                }
                if (succ > 0 && unavailable > 0)
                {
                    return "<span style='font-weight:600;'>Update Completed with Success for following Kaveri Village Code:</span><br><br>" + string.Join(", ", SuccessIdList) + "<br><br><span style='font-weight:600;'>Following Village Code in Excel sheet are missing from database:</span><br><br>" + string.Join(", ", UnavailableIdList);
                }
                else if (err > 0 && succ > 0)
                {
                    return "<span style='font-weight:600;'>Update Completed with error in following Kaveri Village Code:</span><br><br>" + string.Join(", ", errIdList) + "<br><br><span style='font-weight:600;'>Update Completed with Success for following Kaveri Village Code:</span><br><br>" + string.Join(", ", SuccessIdList);
                }
                else if (err > 0 && unavailable > 0)
                {
                    return "<span style='font-weight:600;'>Update Completed with error in following Kaveri Village Code:</span><br><br>" + string.Join(", ", errIdList) + "<br><br><span style='font-weight:600;'>Following Village Code in Excel sheet are missing from database:</span><br><br>" + string.Join(", ", UnavailableIdList);
                }
                else if (err > 0)
                {
                    return "<span style='font-weight:600;'>Update Completed with error in following Kaveri Village Code:</span><br><br>" + string.Join(", ", errIdList);
                }
                else if(succ>0)
                {
                    return "<span style='font-weight:600;'>Update Completed with Success for following Kaveri Village Code:</span><br><br>" + string.Join(", ", SuccessIdList);
                }
                else 
                {
                    return "<span style='font-weight:600;'>No Village Code is available in database from excel sheet.</span>" ;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        

    }
}