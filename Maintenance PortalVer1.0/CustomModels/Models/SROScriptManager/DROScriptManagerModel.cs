using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.SROScriptManager
{
    public class DROScriptManagerModel
    {
        public int ScriptID { get; set; }

        [Display(Name = "Enable")]
        public bool IsActive { get; set; }

        [Display(Name = "Script Description")]
        [Required(ErrorMessage = "Ticket Description is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Ticket Description should be min 5 and max 200 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Only alphabets and numbers are allowed.")]
        public string ScriptDescription { get; set; }

        [Display(Name = "Service Pack")]
        [Required(ErrorMessage = "Service Pack is required.")]
        [StringLength(200, MinimumLength = 2,  ErrorMessage = "Ticket Description should be min 5 and max 200 characters long.")]
        [RegularExpression(@"^(?:(\d+)\.)?(?:(\d+)\.)?(\*|\d+)$", ErrorMessage = "Only Number and Dot are allowed.")]
        public string ServicePackNumber { get; set; }

        [Display(Name = "Script FilePath")]
        public string FilePath { get; set; }
        public bool IsInsertedSuccessfully { get; set; }

        public string ErrorMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string ScriptContent { get; set; }



        // Service pack Details 

        // For Child table popup
        public String ColumnName { get; set; }
        public int StartLen { get; set; }
        public int TotalNum { get; set; }

        public int SROCode { get; set; }

        public bool IsForExcelDownload { get; set; }
        public bool IsForSearch { get; set; }
    }


    public class DROScriptManagerDetailWrapperModel
    {
        public List<DROScriptManagerDetailModel> ScriptManagerDetailList { get; set; }

        public int TotalCount { get; set; }
    }

    public class DROScriptManagerDetailModel
    {
        public string SerialNo { get; set; }

        public string ID { get; set; }

        public string Script { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public string ServicePack { get; set; }

        public string DateOfScript { get; set; }

        public string Action { get; set; }
    }

}
