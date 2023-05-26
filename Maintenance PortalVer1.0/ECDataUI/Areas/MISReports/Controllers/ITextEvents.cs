#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ITextEvents.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataUI.Areas.MISReports.Controllers
{
    public class ITextEvents : PdfPageEventHelper
    {

  
        PdfContentByte cb;

     
       public ITextEvents()
        {

        }


        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        /// <summary>
        /// On Open Document
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(5, 50);
            }
            catch (DocumentException)
            {
                //handle exception here
            }
            catch (System.IO.IOException)
            {
                //handle exception here
            }
        }


        //public override void OnOpenDocument(PdfWriter writer, Document document)
        //{
        //    try
        //    {
        //        PrintTime = DateTime.Now;
        //        bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //        cb = writer.DirectContent;
        //        headerTemplate = cb.CreateTemplate(100, 100);
        //        footerTemplate = cb.CreateTemplate(50, 50);
        //    }
        //    catch (DocumentException)
        //    {
        //        //handle exception here
        //    }
        //    catch (System.IO.IOException)
        //    {
        //        //handle exception here
        //    }
        //}

        /// <summary>
        /// On End Page
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {

           
            base.OnEndPage(writer, document);
            BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
         ////   iTextSharp.text.Font fntHead = objCommon.DefineBoldFont("Times New Roman", 11);
            //PdfPTable t = new PdfPTable(12);
         

            base.OnEndPage(writer, document);


         

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            //Create PdfTable object
            //PdfPTable pdfTab = new PdfPTable(3);
            //PdfPCell pdfCell1 = new PdfPCell();
            //PdfPCell pdfCell3 = new PdfPCell();
            //DateTime PrintDateTime = Convert.ToDateTime(PrintTime);
            //string printDateTime = PrintDateTime.ToString("dd/MM/yyyy ,HH:mm:ss");
            String text = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t Page: " + writer.CurrentPageNumber ;
           // pdfTab.TotalWidth = document.PageSize.Width - 80f;
            //pdfTab.WidthPercentage = 70;

            //Add paging to footer
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(520), document.PageSize.GetBottom(20));
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 12);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(80), document.PageSize.GetBottom(20));
            }

         

           
           // pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);

            cb.MoveTo(40, document.PageSize.GetBottom(40));
            cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(40));
            cb.Stroke();
        }

        /// <summary>
        /// On Close Document
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            //headerTemplate.BeginText();
            //headerTemplate.SetFontAndSize(bf, 12);
            //headerTemplate.SetTextMatrix(0, 0);
            //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            //headerTemplate.EndText();
          
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bf, 12);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            //footerTemplate.EndText();


        }
     
    }
}