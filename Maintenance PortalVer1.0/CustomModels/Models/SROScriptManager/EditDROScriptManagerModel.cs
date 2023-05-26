using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.SROScriptManager
{
    public class EditDROScriptManagerModel
    {
        [Display(Name = "Enable")]
        public bool IsActive { get; set; }

        [Display(Name = "Ticket Description")]
        [Required(ErrorMessage = "Ticket Description is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Ticket Description should be min 5 and max 200 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Only alphabets and numbers are allowed.")]
        public string ScriptDescription { get; set; }

        [Display(Name = "Service Pack")]
        [Required(ErrorMessage = "Service Pack is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Ticket Description should be min 5 and max 200 characters long.")]
        [RegularExpression(@"^(?:(\d+)\.)?(?:(\d+)\.)?(\*|\d+)$", ErrorMessage = "Only Number and Dot are allowed.")]
        public string ServicePackNumber { get; set; }

        public string FilePath { get; set; }
        public bool IsInsertedSuccessfully { get; set; }

        public string ErrorMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string ScriptContent { get; set; }

        
    }
}
