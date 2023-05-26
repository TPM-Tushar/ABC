using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Common
{
   public class KioskOnlineUserInfo
    {
        [Display(Name = "ApplicationNumber")]
        public string ApplicationNumber { get; set; }

// [Required(ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "ServiceIdReq")]
        [Display(Name = "ServiceId")]
        //[Range(1, 7, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "ServiceIdRange")]
        public Int64 ServiceId { get; set; }
        public List<SelectListItem> lstServices { get; set; }

         
// [Required(ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "OfficeIdReq")]
        [Display(Name = "OfficeId")]
        //[Range(1, 7, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "OfficeIdRange")]
        public short OfficeID { get; set; }

        [Display(Name = "PartyType")]
        //[Range(1, 20, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "PartyTypeRange")]
        public Nullable<int> PartyType { get; set; }

        [Display(Name = "IsOnline")]
        public bool IsOnline { get; set; }
        
        [Display(Name = "FirstName")]
// [Required(ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "FirstNameRequired")]
        //[RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessageResourceName = "FirstNameRegEx", ErrorMessageResourceType = typeof(KioskDetailsResource))]
        //[StringLength(75, MinimumLength = 2, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "FirstNameLen")]
        public string FirstName { get; set; }

        [Display(Name = "MiddleName")]
        //[RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessageResourceName = "MiddleNameRegEx", ErrorMessageResourceType = typeof(KioskDetailsResource))]
        //[StringLength(75, MinimumLength = 2, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "MiddleNameLen")]
        public string MiddleName { get; set; }

        [Display(Name = "LastName")]
// [Required(ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "LastNameRequired")]
        //[RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessageResourceName = "LastNameRegEx", ErrorMessageResourceType = typeof(KioskDetailsResource))]
        //[StringLength(75, MinimumLength = 2, ErrorMessageResourceType = typeof(KioskDetailsResource), ErrorMessageResourceName = "LastNameLen")]
        public string LastName { get; set; }

        public string ApplicantName { get; set; }

        public long? UserID { get; set; }
        public bool IsOnlineUserGenerateToken { get; set; }

        public long ApplicationID { get; set; }

    }
}
