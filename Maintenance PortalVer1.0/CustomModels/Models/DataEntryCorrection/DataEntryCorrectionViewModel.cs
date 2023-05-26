using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.DataEntryCorrection
{
    public class DataEntryCorrectionViewModel
    {
        //public string OfficeName { get; set; }
        public int  OfficeID { get; set; }
        
        //public int databaseID;
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }

        //Changed to long by Madhusoodan on 06/08/2021
        //public int UserID { get; set; }
        public long UserID { get; set; }

        [Required(ErrorMessage = "DR OfficeID is required")]
        public int DROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int FeeTypeID { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }
        [Display(Name = "BookType")]
        public List<SelectListItem> BookType { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYear { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Please enter Document Number.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter valid Document Number.")]
        public long? DocumentNumber { get; set; }

        public int BookTypeID { get; set; }

        public string FinancialYearStr { get; set; }

        public int FinancialYearID { get; set; }

        public string EncrytptedDocumentID { get; set; }

        //Below properties are added from DataEntryCorrectionOrderViewModel

        [Display(Name = "Order No.")]
        [Required(ErrorMessage = "Please enter Order Number.")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()\[\]%&*. :;]*$", ErrorMessage = "Please enter valid Order Number.")]
        [StringLength(100)]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Upload DR Order File")]
        public string OrderNoteFile { get; set; }

        public int OrderID { get; set; }
        public int SROCode { get; set; }
        public long DocumentID { get; set; }
        public long PropertyID { get; set; }
        public string IPAddress { get; set; }

        public string EncryptedPropertyID { get; set; }
        public string EncryptedDocumentID { get; set; }
        public string FilePath { get; set; }
        public bool IsInsertedSuccessfully { get; set; }
        //public long UserID { get; set; }

        //Added by Madhusoodan on 06/08/2021
        [Display(Name = "Section 68(2) Note for Property")]
        [Required(ErrorMessage = "Please enter Section 68(2) Note for selected Property")]
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,]) special characters are allowed")]
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()%#&*.,:; ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,],[*],[:],[;],[,],[.]) special characters are allowed")]
        //changed by mayank on 31/01/2022 for Section 68 Note change
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()%#&*.,:; +]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,],[*],[:],[;],[,],[.]) special characters are allowed")]
        //ADDED BY VIJAY ON 05-04-23 TO ACCEPT CHARACTERS LIKE "ತ್‌"
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()%#&*.,:; +\u200C\u0CCD]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,],[*],[:],[;],[,],[.]) special characters are allowed")]

        //[StringLength(300)]
        //changed by mayank on 31/01/2022 for Section 68 Note change
        [StringLength(4000)]
        public string Section68NoteForProperty { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeOrderList { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeOrderList { get; set; }

        public int SROOrderCode { get; set; }
        public long DistrictOrderCode { get; set; }

        public bool isEditMode { get; set; }

        public bool IsDocumentSearched { get; set; }

        //Added by mayank on 30/08/201
        //Added by mayank District enabled for DEC
        public bool IsDRLoginEnabledforDEC { get; set; }
    }

    public class DataEntryCorrectionOrderTableModel
    {
        public string Select { get; set; }
        public int OrderID { get; set; }
        public string SROName { get; set; }
        public string DROrderNumber { get; set; }
        public string OrderDate { get; set; }
        //Commented and changed to string by Madhusoodan on 04/08/2021
        //public Nullable<int> Section68Note { get; set; }
        public string Section68Note { get; set; }
        public string RegistrationNumber { get; set; }
        public string AbsoluteFilePath { get; set; }
        public string Relativepath { get; set; }
        public string FileName { get; set; }
        public string SNo { get; set; }
        public string ViewBtn { get; set; }
        public string Action { get; set; }
        //Added by mayank on 02/09/2021 for Excel
        public string Section68NoteForExcel { get; set; }

        //Added by mayank on 02/09/2021
        public string DistrictName { get; set; }
        public string EnteredBY { get; set; }
        //Added by mayank on 29/11/2021
        public string Status { get; set; }
    }

    public class PropertyNumberDetailsViewModel
    {
        public long UserID { get; set; }
        public PropertyNumberDetailsAddEditModel PropertyNumberDetailsAddEditModel { get; set; }
        public int OfficeID { get; set; }
        public string IPAddress { get; set; }
        public long PropertyID { get; set; }    
        public long DocumentID { get; set; }
        public int OrderID { get; set; }

        //Added by Madhusoodan on 03/08/2021 to save SROCode from Dropdown in Select Document Tab
        public int SROfficeID { get; set; }


    }
    public class PropertyNumberDetailsAddEditModel
    {
        //[Display(Name = "Section 68 Note for Property Details")]
        //[Required(ErrorMessage = "Please enter Section 68 Note for Property")]
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,]) special characters are allowed")]
        //[StringLength(300)]
        //public string Section68NoteForProperty { get; set; }

        public long keyID { get; set; }

        [Display(Name = "Village/Area")]
        public long VillageID { get; set; }

        public List<SelectListItem> VillageList { get; set; }

        [Display(Name = "Hobli")]
        public long HobliID { get; set; }

        public string HobliName { get; set; }

        //[Display(Name = "Current Property Type")]
        //public int CurrentPropertyTypeID { get; set; }

        public List<SelectListItem> CurrentPropertyType { get; set; }

        //[Display(Name = "Old Property Type")]
        //public int OldPropertyTypeID { get; set; }

        public List<SelectListItem> OldPropertyType { get; set; }

        //[Display(Name = "Current Number")]
        //public string CurrentNumber { get; set; }

        //[Display(Name = "Old Number")]
        //public string OldNumber { get; set; }

        //[Display(Name = "Survey Number")]
        //public int CurrentSurveyNumber { get; set; }

        //[Display(Name = "Survey No Character")]
        //public string CurrentSurveyNoChar { get; set; }

        //[Display(Name = "Hissa Number")]
        //public string CurrentHissaNumber { get; set; }

        //[Display(Name = "Survey Number")]
        //public int OldSurveyNumber { get; set; }

        //[Display(Name = "Survey No Character")]
        //public string OldSurveyNoChar { get; set; }

        //[Display(Name = "Hissa Number")]
        //public string OldHissaNumber { get; set; }

        [Display(Name = "Current Property Number Type")]
        //[Required(ErrorMessage = "Please Select Current Property Number Type")]
        [RegularExpression("^[1-9]*[1-9][0-9]*$", ErrorMessage = "Please select valid current property number type")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please select valid current property number type")]
        public int CurrentPropertyTypeID { get; set; }



        [Display(Name = "Current Number")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&* ]*$", ErrorMessage = @"Only numbers, alphabets and ([-],[/],[\],[_],[(],[)],[#],[&],[*]) special characters are allowed in Current Number")]
        [StringLength(100)]
        public string CurrentNumber { get; set; }


        [Display(Name = "Old Property Number Type")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please select valid old property number type")]
        public string OldPropertyTypeID { get; set; }


        [Display(Name = "Old Property Number")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&* ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[*]) special characters are allowed in Old Number")]
        [StringLength(75)]
        public string OldNumber { get; set; }

        [Display(Name = "Survey No")]
        //[RegularExpression("^[1-9]*$", ErrorMessage = "Only numbers are allowed")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Only numbers are allowed in Survey Number")]
        [Required(ErrorMessage = "Please Enter Survey No")]
        //[StringLength(5)]
        public int CurrentSurveyNumber { get; set; }


        [Display(Name = "Surnoc")]
        //[Required(ErrorMessage = "Please Enter Surnoc")]
        //commented by vijay  on 07-02-2023 because + was not accepted 
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&*, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_], [(],[)],[#],[&],[*],[,]) special characters are allowed in Survey No Character")]
        //added by vijay on 06-02-2023 to accept + in CurrentSurveyNoChar

        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&*+, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_], [(],[)],[#],[&],[*],[,]) special characters are allowed in Survey No Character")]

        //[StringLength(20)]
        public string CurrentSurveyNoChar { get; set; }


        [Display(Name = "Hissa No")]
        //[Required(ErrorMessage = "Please Enter Hissa No")]
        //commented by vijay  on 07-02-2023 because + was not accepted 
      //  [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&*, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_], [(],[)],[#],[&],[*],[,]) special characters are allowed in Hissa Number")]
      //added by vijay on 06-02-2023 to accept + in hissaa no
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&*+, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_], [(],[)],[#],[&],[*],[,]) special characters are allowed in Hissa Number")]
        //[StringLength(30)]
        //[Required(AllowEmptyStrings =true)]
        public string CurrentHissaNumber { get; set; }

        //Added by mayank on 16/08/2021 for Data Entry Correction
        [Display(Name = "Sub Registrar")]
        public long SroCode { get; set; }

        public List<SelectListItem> SROfficeList { get; set; }


        //Added by Shivam B on 10/05/2022 for Add New Property Number Search Parameter in Section 68
        [Display(Name = "District Registrar")]
        public long DroCode { get; set; }
        public List<SelectListItem> DROfficeList { get; set; }

        public long HiddenDroCode { get; set; }
        
        public string HidddenDROName { get; set; }

        public long HiddenSROCode { get; set; }

        public string HiddenSROName { get; set; }

        //Added by Shivam B on 10/05/2022 for Add New Property Number Search Parameter in Section 68


    }

    public class AddPropertyNoDetailsResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class PartyDetailsViewModel
    {
        public long UserID { get; set; }
        public PartyDetailsAddEditModel PartyDetailsAddEditModel { get; set; }
        public int OfficeID { get; set; }
        public string IPAddress { get; set; }
        public long PropertyID { get; set; }
        public long DocumentID { get; set; }
        public int OrderID { get; set; }

        //Added by Madhusoodan on 03/08/2021 to save SROCode from Dropdown in Select Document Tab
        public int SROfficeID { get; set; }
    }

    public class PartyDetailsAddEditModel
    {

        [Display(Name = "Party Type")]
        [Required(ErrorMessage = "Please select party type")]
        [RegularExpression("^[1-9]*[1-9][0-9]*$", ErrorMessage = "Please select party type")]
        public int PartyTypeID { get; set; }

        public List<SelectListItem> PartyTypeList { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter first name")]
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z\\-_ 0-9]*$", ErrorMessage = @"Only alphabet and ([-]) special character are allowed")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z\-_ 0-9 \/.&\\]*$", ErrorMessage = @"Only alphabet and ([-,.,&,\,/]) special character are allowed")]
        [StringLength(300)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z-\/.&\\]*$", ErrorMessage = @"Only alphabet and ([-,.,&,\,/]) special character are allowed")]
        [StringLength(100)]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z-\/.&\\]*$", ErrorMessage = @"Only alphabet and ([-,.,&,\,/]) special character are allowed")]
        [StringLength(350)]
        public string LastName { get; set; }

        //[Display(Name = "Section 68 Note for Party")]
        //[Required(ErrorMessage = "Please enter Section 68 Note for Party")]
        //[RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()#&, ]*$", ErrorMessage = @"Only numbers, alphabet and ([-],[/],[\],[_],[(],[)],[#],[&],[,]) special characters are allowed")]
        //[StringLength(300)]
        //public string Section68NoteForParty { get; set; }
    }

    public class AddPartyDetailsResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }
    }
}
