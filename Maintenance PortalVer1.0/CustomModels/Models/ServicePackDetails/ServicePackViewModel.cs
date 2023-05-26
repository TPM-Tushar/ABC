/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: ServicePackViewModel.cs
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : Model for Service PAck Details Module
 * ECR No : 300
*/

#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
#endregion

namespace CustomModels.Models.ServicePackDetails
{
    public class ServicePackViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedID { get; set; }
        public ServicePackChangesDetailsModel ServicePackChangesDetails { get; set; }
        public SoftwareReleaseTypes SoftwareReleaseType { get; set; }
        public ServicePackDetail ServicePackDetails { get; set; }
        public List<SelectListItem> ReleaseTypeDropDown { get; set; }
        public List<SelectListItem> ChangeTypeDropDown { get; set; }
        public List<ServicePackChangesDetailsModel> servicePackChagesDetails { get; set; }
        public string BugsListValues { get; set; }
        public string EnhancementListValues { get; set; }
		//Added and Changed by mayank on 14/09/2021 for Support Exe Release
        public string SupportAnalysisListValues { get; set; }
        public string ReleaseMode { get; set; }
        public int TotalServicePackDetailsCountForList { get; set; }
        public string ChangesTable { get; set; }
        public string SelectedValueReleaseType { get; set; }
        public string SelectedValueChangeType { get; set; }
        public Dictionary<string, int> WebGridListValues { get; set; }
        public string FilePath { get; set; }
        public string ReleaseDetails { get; set; }

        //Added by shubham bhagat on 18-09-2019
        // below variable will be update in Javascript and will check in DAL
        public bool IsFileToUpdate { get; set; }

        //Added by shubham bhagat on 18-09-2019
        // below variable will be update in MVC Controller and will check in DAL
        public bool IsFileUploadedSuccessfully { get; set; }

        //public List<ModificationTypeUpdateIdModel> ModificationTypeUpdateIds { get; set; }

        //Added by shubham bhagat on 18-09-2019
        public byte[] FileData { get; set; }

        //Added by shubham bhagat on 18-09-2019
        public string FileExtension { get; set; }

        //Added by shubham bhagat on 18-09-2019
        public string ServicePackReleaseDate { get; set; }

        //Added by shubham bhagat on 18-09-2019
        public int SerialNo { get; set; }

        //Added By Tushar on 2 Jan 2023 for Upload DateTime
        public string ServicePackAddedDateTime { get; set; }
        //End By Tushar on 2 Jan 2023
    }

    public class SoftwareReleaseTypes
    {
        //Added and Changed by mayank on 14/09/2021 for Support Exe Release
        //[Range(1, 2, ErrorMessage = "Please Select Release Type")]
        [Range(1, 3, ErrorMessage = "Please Select Release Type")]
        public short TypeID { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(50)]
        [Display(Name = "Service Pack Release Type")]
        public string TypeName { get; set; }

        public bool IsActive { get; set; }
    }


    public class ServicePackDetail
    {
        public ServicePackDetail()
        {
            Files = new List<HttpPostedFileBase>();

        }

        [Required(ErrorMessage = "Service Pack File is Required")]
        public List<HttpPostedFileBase> Files { get; set; }

        public int SpID { get; set; }

        public string IsTestFinal { get; set; }
        public string strIsActive { get; set; }

        [Display(Name = "Release Type Mandatory")]
        [Required(ErrorMessage = "{0} is required.")]
        public bool IsTestOrFinal { get; set; }

        [Display(Name = "Mandatory")]
        [Required(ErrorMessage = "{0} is required.")]
        public bool IsActive { get; set; }


        [Required(ErrorMessage = "{0} is Required")]
        [Display(Name = "Major")]
        [RegularExpression(@"^\s*(\d{1,2}|-\d+)(\.\d{1,4})?\s*$", ErrorMessage = "2 Digit Input Only")]
        public int Major { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [Display(Name = "Minor")]
        //Added by shubham bhagat on 18-09-2019
        [RegularExpression(@"^\s*(\d{1}|-\d+)(\.\d{1,4})?\s*$", ErrorMessage = "1 Digit Input Only")]
        //[RegularExpression(@"^\s*(\d{1,2}|-\d+)(\.\d{1,4})?\s*$", ErrorMessage = "2 Digit Input Only")]
        public int Minor { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(500)]
        [Display(Name = "Description")]
        //Commented by mayank on 06/12/2021 to remove validtions
        //[RegularExpression(@"^[a-zA-Z0-9-/., ]+$", ErrorMessage = "Invalid Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(500)]
        [Display(Name = "Installation Procedure Details")]
        //Commented by mayank on 06/12/2021 to remove validtions
        //[RegularExpression(@"^[a-zA-Z0-9-/., ]+$", ErrorMessage = "Invalid Input")]
        public string InstallationProcedure { get; set; }

        public string FileServerFPath { get; set; }
        public string FileServerVPath { get; set; }

        //Added by shubham bhagat on 18-09-2019
        // commented on 25-09-2019 because it is not used any where 
        //public DateTime ServicePackReleaseDateTime { get; set; }
        public Int64 ServicePackUploadedBy { get; set; }

        public bool IsReleased { get; set; }
    }


    public class ServicePackChangesDetailsModel
    {

        public string SpFixedID { get; set; }
        public int SpID { get; set; }
        public int ChangeType { get; set; }
        public int RowID { get; set; }
        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        //Added by shubham bhagat on 18-09-2019
        public List<SelectListItem> ChangeTypeDropDownNew { get; set; }

        public string SelectedValueChangeTypeDesc { get; set; }
    }

    public class ReleaseDetails
    {
        public int ReleaseID { get; set; }
        public int ServicePackID { get; set; }

        [StringLength(500)]
        [Display(Name = "Release Note:")]
        [RegularExpression(@"^[a-zA-Z0-9-/., ]+$", ErrorMessage = "Invalid Input")]
        public string ReleaseNotes { get; set; }

        [Display(Name = "Release Date")]
        public DateTime ReleaseDateTime { get; set; }
        public Int64 ServicePackReleasedBy { get; set; }

        public bool IsApproved { get; set; }
    }

    //public class ModificationTypeUpdateIdModel
    //{
    //    public String  Id { get; set; }

    //    public String Value { get; set; }
    //}

    public class DownloadResponseModel
    {
        public Byte[] FileByte { get; set; }

        public String Filename { get; set; }

        public String errorMsg { get; set; }

    }
}
