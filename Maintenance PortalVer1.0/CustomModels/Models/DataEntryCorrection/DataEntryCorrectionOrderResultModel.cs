using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CustomModels.Models.DataEntryCorrection
{
    public class DataEntryCorrectionOrderResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }

        //Added by Madhusoodan on 02/08/2021 to maintain OrderID in session
        public int OrderID { get; set; }

    }

    public class SelectDocumentResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }

        //Added by Madhusoodan on 02/08/2021 to maintain DocumentID in session
        public long DocumentID { get; set; }

        public long? DocumentNumber { get; set; }

        public int BookTypeID { get; set; }

        public string FinancialYearStr { get; set; }

        //public int SROCode { get; set; }    //for session

    }

    //Added by Madhusoodan on 06/08/2021
    public class Section68NoteResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }

        public string previousSection68Note { get; set; }

        //Added by Madhusoodan on 17/08/2021
        public string Sec68NotePreparedPart { get; set; }
    }

    //Added by Madhusoodan on 11/08/2021 (To take Order ID to save file)

    public class DROrderFilePathResultModel
    {
        public int NewOrderID { get; set; }  //To take OrderID from sequence and save file using this OrderID

        public string RelativeFilePath { get; set; }

        public string FileName { get; set; }

        //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry
        public string rootPath { get; set; }
        public byte[] DataEntryCorrectionFileBytes { get; set; }
        //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry
        
    }
}
