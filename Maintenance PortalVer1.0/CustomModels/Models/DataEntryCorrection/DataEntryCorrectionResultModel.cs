using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.DataEntryCorrection
{
    public class DataEntryCorrectionResultModel
    {
        public List<DataEntryCorrectionPropertyDetailModel> DataEntryCorrectionPropertyDetailList { get; set; }
        public List<IndexIIReportsDetailsModel> IndexIIReportList { get; set; }
        public string IndexIIFinalRegistrationNo { get; set; }
        public string IndexIISroName { get; set; }

        //public string EncrytptedDocumetID{ get; set; }
        //public string btnAddOrder{ get; set; }
    }

    public class DataEntryCorrectionPropertyDetailModel
    {
        //public string Select { get; set; }
        //public int SerialNo { get; set; }
        //public long PropertyID { get; set; }
        //public long DocumentID { get; set; }
        //public string FinalRegistrationNo { get; set; }
        //public string PropertyDescription { get; set; }
        //public string PropertyNumberDetail { get; set; }
        //public string ExecutionDateTime { get; set; }
        //public string NatureofDocument { get; set; }
        //public decimal Marketvalue { get; set; }
        //public decimal Consideration { get; set; }
        //public string  RegistrationDate { get; set; }
        //public string  CDNumber { get; set; }
        //public string  ViewBtn { get; set; }

        //public string EncryptedPropertyID { get; set; }

        //public bool ErrorMessage { get; set; }

        //After adding functions in Select Document Tab
        public int SerialNo { get; set; }
        public string FinalRegistrationNo { get; set; }
        public string RegistrationDate { get; set; }
        public string ExecutionDateTime { get; set; }
        public string NatureOfDocument { get; set; }
        public string PropertyDetails { get; set; }
        public string VillageName { get; set; }
        public string TotalArea { get; set; }
        public string ScheduleDescription { get; set; }  //Not in use
        public decimal? Marketvalue { get; set; }

        public decimal? Consideration { get; set; }
        public string Claimant { get; set; }
        public string Executant { get; set; }

        public string DocumentID { get; set; }
 
 		//Added by Madhur 28/07/2021 for Datatable
 		public string Select { get; set; }

        //Added by Madhusoodan on 06/08/2021
        public string ButtonPropertyNoDetails { get; set; }
        public string ButtonPartyDetails { get; set; }

        //

        public string PropertyID { get; set; }
        public string DROrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string Village { get; set; }
        public string CurrentPropertyType { get; set; }
        public string CurrentNumber { get; set; }
        public string OldPropertyType { get; set; }
        public string OldNumber { get; set; }
        public string Survey_No { get; set; }
        public string Surnoc { get; set; }
        public string Hissa_No { get; set; }
        public string CorrectionNote { get; set; }
        public string Action { get; set; }
        //Addition ends here

        //Added by Madhur 29/07/2021
        public string PartyType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        //Addition ends here
        public bool ErrorMessage { get; set; }

        //Added by Madhusoodan on 08/08/2021 For ECReport Details (New SP instead of Index-II SP)
        public string Description { get; set; }
        public string CDNumber { get; set; }

        public int? PageCount { get; set; }
    }

    public class DataEntryCorrectionPropertyNumberDetailModel
    {
        public int SerialNo { get; set; }

        public int OrderID { get; set; }

        public string DROrderNumber { get; set; }

        public DateTime OrderDate { get; set; }
        public string Village { get; set; }

        public string CurrentPropertyType { get; set; }

        public string CurrentNumber { get; set; }

        public string OldPropertyType { get; set; }

        public int OldNumber { get; set; }

        public int SurveyNumber { get; set; }

        public string SurveyNoChar { get; set; }

        public string HissaNumber { get; set; }

        public string CorrectionNote { get; set; }

        public int IsInUse { get; set; }
    }

	 public class DataEntryCorrectionPreviousPropertyDetailModel
    {

        public int SerialNo { get; set; }
        public string DROrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string Section68Note { get; set; }
        public string ViewDocument { get; set; }

        //Added by Madhusoodan on 11/08/2021 to add Order Upload Date column
        public string OrderUploadDate { get; set; }
        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        public string Section68NoteDeleteBtn { get; set; }
    }

    public class EditbtnResultModel
    {
        public string villageID { get; set; }
        public string CurrentPTID{ get; set; }
        public string OldPTID{ get; set; }
        public string CurrentNo{ get; set; }
        public string CurrentSurveyNo{ get; set; }
        public string CurrentSurveyNoChar { get; set; }
        public string CurrentHissaNumber { get; set; }
        public string OldNo{ get; set; }
        public string Flag{ get; set; }
    }
    public class IndexIIReportsDetailsModel
    {
        public int Sno { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public string ArticleNameE { get; set; }
        public string Stamp5Datetime { get; set; }
        public string TotalArea { get; set; }
        public string Unit { get; set; }
        public string PropertyDetails { get; set; }
        public string Schedule { get; set; }
        public string Executant { get; set; }
        public string Claimant { get; set; }
        public string VillageNameE { get; set; }
        public decimal consideration { get; set; }
        public decimal marketvalue { get; set; }
        public decimal Total { get; set; }
        public string SroName { get; set; }
        public string CDNumber { get; set; }
        public string PageCount { get; set; }
        public string Section68Note { get; set; }

        //Added by mayank on 02/09/2021 for Excel
        public string PropertyDescription { get; set; }

    }

}
