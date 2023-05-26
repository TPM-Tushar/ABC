using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.ComponentModel;



namespace ECDataUI.Common
{
    public class PDFConversion
    {
        public PdfDocument srcdoc { get; set; }
       // private string tempPdfFilePath;  
        public string pdfFileName { get; set; }       
        public string srcFile { get; set; }       
        public string dstFile { get; set; }


        public PDFConversion()
        {
        }
       
        public PDFConversion(string srcFile, string dstFile)
        {
            this.srcFile = srcFile;
            this.dstFile = dstFile;
        }
        public bool ConvertTifftoPdfCheckReadablePageCount(ref int pageCount)
        {
            try
            {

                // creation of the document with a certain size and certain margins  
                Document document = new Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0);

                // creation of the different writers  
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new System.IO.FileStream(dstFile, System.IO.FileMode.Create));

                // load the tiff image and count the total pages  
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(srcFile);
                int total = bm.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                pageCount = total;
                document.Open();
                iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
                for (int k = 0; k < total; ++k)
                {
                    bm.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bm, System.Drawing.Imaging.ImageFormat.Bmp);
                    // scale the image to fit in the page  
                    img.ScalePercent(72f / img.DpiX * 100);
                    img.SetAbsolutePosition(0, 0);
                    cb.AddImage(img);
                    document.NewPage();
                }
                document.Close();
                bm.Dispose();
                //PdfReader reader = new PdfReader(tempPdfFileName);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {               
                new CommonFunctions().DeleteFileFromTemporaryFolder(srcFile);
            }

        }
        public bool AddHeaderFooterwaterMark(bool isSigned,string name,string address,ref string errorMessage)
        {      
             string watermark = string.Empty;
             if (isSigned)
             {                
                 watermark = System.Configuration.ConfigurationManager.AppSettings["watermark"];
             }
             else
             {
                 watermark = System.Configuration.ConfigurationManager.AppSettings["informationWatermark"];
             }
            PdfReader reader1 = new PdfReader(srcFile);

            FileStream fs = new FileStream(dstFile, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                using (PdfStamper stamper = new PdfStamper(reader1, fs))
                {
                    //int pageCount1 = reader1.NumberOfPages;
                    int pageNumber = reader1.NumberOfPages;
                    //Create a new layer
                    PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                    for (int i = 1; i <= pageNumber; i++)
                    {
                        iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);
                        //Get the ContentByte object
                        PdfContentByte cb = stamper.GetOverContent(i);

                       


                        //****************************************** Watermark *************************************
                        //Tell the CB that the next commands should be "bound" to this new layer
                        cb.BeginLayer(layer);
                        cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.TIMES_ITALIC, BaseFont.CP1252, false), 50);
                        //cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLDOBLIQUE, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                        PdfGState gState1 = new PdfGState();
                        gState1.FillOpacity = 0.35f;
                        cb.SetGState(gState1);
                        cb.SetColorFill(BaseColor.DARK_GRAY);
                        cb.BeginText();
                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermark, rect.Width / 2, rect.Height / 2, 45f);
                        cb.EndText();
                        //"Close" the layer
                        cb.EndLayer();
                        //******************************************** End******************************************

                       
                        if (isSigned)
                        {
                            string header = System.Configuration.ConfigurationManager.AppSettings["headrInfo"];
                            string footer = System.Configuration.ConfigurationManager.AppSettings["footerInfo"];
                            //************************************* Header *******************************************
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, false), 10);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.35f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.DARK_GRAY);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, header + " " + i + "/" + pageNumber, (rect.Width - 20), (rect.Height - 20), 0);
                            cb.EndText();

                            //************************************* End **********************************************

                            //******************************************** Footer ******************************************
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, false), 10);
                            PdfGState gState2 = new PdfGState();
                            gState2.FillOpacity = 0.35f;
                            cb.SetGState(gState1);
                            cb.SetColorFill(BaseColor.DARK_GRAY);
                            cb.BeginText();
                            //Image objImage = Image.GetInstance(@"D:\NewDevlopments\TestingApp\TestingApp\images.jpg");
                            //cb.AddImage(objImage, 10, 0, 0, 35, 100, 100);
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, footer, 140, 35, 0);
                            cb.EndText();
                            //"Close" the layer
                            cb.EndLayer();
                            //********************************************** End *************************************************


                            string symbolImagePath = System.Configuration.ConfigurationManager.AppSettings["symbolImagePath"];
                            Image objImage = Image.GetInstance(symbolImagePath);
                            cb.AddImage(objImage, 100, 0, 0, 75, rect.Width - 150, 55);
                            cb.EndLayer();
                        }
                    }

                    if (isSigned)
                    {
                        iTextSharp.text.Rectangle prect = reader1.GetPageSize(1);
                        int lastPage = pageNumber + 1;
                        stamper.InsertPage(lastPage, prect);
                        PdfContentByte pcb = stamper.GetUnderContent(lastPage);
                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                        pcb.SetFontAndSize(bf, 10);
                        pcb.BeginText();
                        string text = "Departmment Of Stamps And Registration";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 50, 0);
                        //pcb.NewlineText();
                        text = "Certificate";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 80, 0);

                        text = "Certificate Under Section 10A of Karnataka Stamp Act,1957 for the purpose of section 10 of ";
                        //pcb.NewlineShowText(text);
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 110, 0);

                        text = "Karnataka Stamp Act,1957 section 10 of Karnataka Stamp Act,1957";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 140, 0);

                        text = "Certificate that sum of Rs 10 being proper stamp duty been remitted as follows:";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 170, 0);


                        

                        text = "Digitally Signed by Departmment Of Stamps And Registration";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 200, prect.Height - 280, 0);

                        text = "Place:Bengaluru";
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 310, 0);

                        text = "Date:" + DateTime.Now.ToString("dd/MMM/yyyy");
                        pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 340, 0);



                        PdfPTable table = new PdfPTable(2);
                        var firstRowCell1 = new PdfPCell(new Phrase("Type"));
                        var firstRowCell2 = new PdfPCell(new Phrase("Amount (Rs.)"));
                        PdfPCell[] row1Cells = { firstRowCell1, firstRowCell2 };
                        var row1 = new PdfPRow(row1Cells);
                        table.Rows.Add(row1);

                        var nextRowCell1 = new PdfPCell(new Phrase("Stamp Duty"));
                        var nextRowCell2 = new PdfPCell(new Phrase("10"));
                        PdfPCell[] row2Cells = { nextRowCell1, nextRowCell2 };
                        var row2 = new PdfPRow(row2Cells);
                        table.Rows.Add(row2);

                        table.TotalWidth = 300f;
                        table.WriteSelectedRows(0, -1, 100, prect.Height - 190, pcb);

                       

                        text = "By " + name + ",residing at " + address;
                        //pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 250, 0);

                        Phrase p = new Phrase(text, FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED, 10));
                        ColumnText ct = new ColumnText(pcb);
                        ct.SetSimpleColumn(p, 100, prect.Height - 220, 530, 36, 25, Element.ALIGN_LEFT);
                        ct.Go();

                        pcb.EndText();
                    }

                }
                return true;
            }
            catch
            {
                errorMessage = "Error in adding watermark";
                return false;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
                reader1.Close();
                new CommonFunctions().DeleteFileFromTemporaryFolder(srcFile);

            }
        }

        /// <summary>
        /// Convert Tiff to PDF using Third Party s/w Image Magick 
        /// </summary>
        /// <param name="watermarkToEmbed">Watermark string to embed</param>
        /// <param name="errorMessage">Error generated in conversion</param>
        /// <returns>Status of conversion</returns>
        public bool convertUsingImageMagick(string watermarkToEmbed, ref string errorMessage)
        {
            bool result = false;
            try
            {
                string outputmsg = string.Empty;
                string ImageMagickPath = System.Configuration.ConfigurationManager.AppSettings["ImageMagickPath"];
                if (!ImageMagickPath.EndsWith(@"\"))
                {
                    ImageMagickPath = ImageMagickPath + @"\";
                }
                ExecuteShellCommand(ImageMagickPath + "Convert.exe", "\"" + srcFile + "\"" + " " + "\"" + dstFile + "\"", ref outputmsg, ref errorMessage);
                if (!string.IsNullOrEmpty(errorMessage) && (!File.Exists(dstFile)))
                {
                    throw new Exception(errorMessage);
                }

                if (!string.IsNullOrEmpty(watermarkToEmbed))
                {
                    AddWaterMarkOnPDF(watermarkToEmbed, dstFile, ref errorMessage);
                }
                result = File.Exists(dstFile);
            }
            catch (Exception ex)
            {
                errorMessage = "Type: " + ex.GetType().ToString() + " \n\n Message :" + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage = errorMessage + " Inner Exception: " + ex.InnerException.ToString();
                }
                return false;
            }
            finally
            {
                new CommonFunctions().DeleteFileFromTemporaryFolder(srcFile);
            }
            return result;
        }

        /// <summary>
        /// Add water mark to PDF file
        /// </summary>
        /// <param name="watermarkToEmbed">String Watermark to embed</param>
        /// <param name="filePath">PDF file path</param>
        /// <param name="errorMessage">Error message while adding watermark</param>
        public void AddWaterMarkOnPDF(string watermarkToEmbed, string filePath, ref string errorMessage)
        {

            PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify);// PdfDocumentOpenMode.Modify 

            for (int i = 0; i < doc.PageCount; i++)
            {

                if (watermarkToEmbed.Length > 0)
                {
                    PdfSharp.Pdf.PdfPage page = doc.Pages[i];
                    PdfSharp.Drawing.XGraphics gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page, PdfSharp.Drawing.XGraphicsPdfPageOptions.Append);
                    // Define a rotation transformation at the center of the page
                    gfx.TranslateTransform(page.Width / 2, page.Height / 2);
                    //gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
                    gfx.TranslateTransform(-page.Width / 2, -(page.Height * 0.10));

                    //OLD Code (Not in Use)
                    //Create a string format
                    //XStringFormat format = new XStringFormat();
                    //format.Alignment = XStringAlignment.Near;
                    //format.LineAlignment = XLineAlignment.Near;

                    // Create a graphical path
                    PdfSharp.Drawing.XGraphicsPath path = new PdfSharp.Drawing.XGraphicsPath();

                    // Create a dimmed red brush
                    PdfSharp.Drawing.XBrush brush = new PdfSharp.Drawing.XSolidBrush(PdfSharp.Drawing.XColor.FromArgb(128, 255, 0, 0));
                    PdfSharp.Drawing.XFont font = new PdfSharp.Drawing.XFont("Times New Roman", 12, PdfSharp.Drawing.XFontStyle.Italic);

                    PdfSharp.Drawing.XPen pen = new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColor.FromArgb(128, 255, 0, 0), 0);

                    int intcnt = 0;
                    int substringLength = 100;

                    int position = 0;
                    position = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(watermarkToEmbed.Length) / substringLength) / 2);
                    position = -position * 10;

                    int offset = 0;
                    string watermark = string.Empty;
                    do
                    {
                        offset = watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length;
                        watermark = watermarkToEmbed.Substring(intcnt, offset);


                        PdfSharp.Drawing.XSize size = gfx.MeasureString(watermark, font);

                        if (!string.IsNullOrEmpty(watermark))
                        {
                            // Add the text to the path
                            path.AddString(watermark, font.FontFamily, font.Style, font.Size,
                            new PdfSharp.Drawing.XPoint(((page.Width - size.Width) / 2), ((page.Height - size.Height) / 2) + position), PdfSharp.Drawing.XStringFormat.TopLeft);
                        }
                        position = position + 15;

                        intcnt = intcnt + (watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length);

                    } while (watermark != "");

                    // Stroke the outline of the path
                    gfx.DrawPath(pen, brush, path);

                    #region OLD Commented Code
                    //string watermark = string.Empty;
                    //int intcnt = 0;
                    //int substringLength = 80;
                    //int position = 0;
                    //position = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(watermarkToEmbed.Length) / substringLength) / 2);
                    //position = -position * 10;
                    //do
                    //{

                    //    watermark = watermarkToEmbed.Substring(intcnt, watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length);
                    //    XSize size = gfx.MeasureString(watermark, font);

                    //    // Add the text to the path
                    //    path.AddString(watermarkToEmbed, font.FontFamily, XFontStyle.Italic, 20,
                    //      new XPoint(20,50), XStringFormat.TopLeft);

                    //    // Draw the string
                    //  //  gfx.DrawString(watermark, font, brush,
                    //   // new XPoint(0,0), XStringFormat.TopLeft);

                    //    // Stroke the outline of the path
                    //    gfx.DrawPath(pen, brush, path);

                    //    position = position + 15;

                    //    intcnt = intcnt + (watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length);


                    //} while (watermark != "");
                    #endregion
                }
            }
            doc.Save(filePath);
            doc.Close();
            errorMessage = string.Empty;

        }

        /// <summary>
        /// Executes Shell Command using Process
        /// </summary>
        /// <param name="_FileToExecute">Command line Exe</param>
        /// <param name="_CommandLine">Parameters to command line exe</param>
        /// <param name="_outputMessage">Return out message</param>
        /// <param name="_errorMessage">Error message generated in execution</param>
        public static void ExecuteShellCommand(string _FileToExecute, string _CommandLine, ref string _outputMessage, ref string _errorMessage)
        {
            // Set process variable
            // Provides access to local and remote processes and enables you to start and stop local system processes.
            System.Diagnostics.Process _Process = null;
            try
            {
                _Process = new System.Diagnostics.Process();

                // invokes the cmd process specifying the command to be executed.
                //string _CMDProcess = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\cmd.exe", new object[] { Environment.SystemDirectory });
                string _CMDProcess = string.Format(System.Globalization.CultureInfo.InvariantCulture, _FileToExecute);

                string _Arguments = string.Empty;

                // pass any command line parameters for execution
                if (_CommandLine != null && _CommandLine.Length > 0)
                {
                    _Arguments += string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0}", new object[] { _CommandLine, System.Globalization.CultureInfo.InvariantCulture });
                }

                // Specifies a set of values used when starting a process.
                System.Diagnostics.ProcessStartInfo _ProcessStartInfo = new System.Diagnostics.ProcessStartInfo(_CMDProcess, _Arguments);
                // sets a value indicating not to start the process in a new window. 
                _ProcessStartInfo.CreateNoWindow = true;

                // sets a value indicating not to use the operating system shell to start the process. 
                _ProcessStartInfo.UseShellExecute = false;
                // sets a value that indicates the output/input/error of an application is written to the Process.
                _ProcessStartInfo.RedirectStandardOutput = true;
                _ProcessStartInfo.RedirectStandardInput = true;
                _ProcessStartInfo.RedirectStandardError = true;
                _Process.StartInfo = _ProcessStartInfo;

                // Starts a process resource and associates it with a Process component.
                _Process.Start();

                // Instructs the Process component to wait indefinitely for the associated process to exit.
                _errorMessage = _Process.StandardError.ReadToEnd();
                _Process.WaitForExit();

                // Instructs the Process component to wait indefinitely for the associated process to exit.
                _outputMessage = _Process.StandardOutput.ReadToEnd();
                _Process.WaitForExit();
            }
            catch (Win32Exception _Win32Exception)
            {
                // Error
                _errorMessage = _errorMessage + "Win32 Exception caught in process:" + _Win32Exception.ToString();
            }
            catch (Exception _Exception)
            {
                // Error
                _errorMessage = _errorMessage + "Exception caught in process:" + _Exception.ToString();
            }
            finally
            {
                // close process and do cleanup
                _Process.Close();
                _Process.Dispose();
                _Process = null;
            }
        }



        //Integrated  By Raman Kalegaonkar on 26-11-2020
        //Code Added to detect Black Image Issue
        public bool convertUsingImageMagickLatestVersion(ref string errorMessage)
        {
            bool result = false;
            try
            {
                string outputmsg = string.Empty;
                string ImageMagickPath = System.Configuration.ConfigurationManager.AppSettings["ImageMagickPathLatestVersion"];
                if (!ImageMagickPath.EndsWith(@"\"))
                {
                    ImageMagickPath = ImageMagickPath + @"\";
                }
                //   ExecuteShellCommand(ImageMagickPath + "Convert.exe", "\"" + srcFile + "\"" + " " + "\"" + dstFile + "\"", ref outputmsg, ref errorMessage);
                ExecuteShellCommand(ImageMagickPath + "magick", "\"" + srcFile + "\"" + " " + "\"" + dstFile + "\"", ref outputmsg, ref errorMessage);

                if (!string.IsNullOrEmpty(errorMessage) && (!File.Exists(dstFile)))
                {
                    throw new Exception(errorMessage);
                }

                //if (!string.IsNullOrEmpty(watermarkToEmbed))
                //{
                //    AddWaterMarkOnPDF(watermarkToEmbed, dstFile, ref errorMessage);
                //}
                result = File.Exists(dstFile);
            }
            catch (Exception ex)
            {
                errorMessage = "Type: " + ex.GetType().ToString() + " \n\n Message :" + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage = errorMessage + " Inner Exception: " + ex.InnerException.ToString();
                }
                return false;
            }
            finally
            {
                //Raman
                //CommonFunctions.DeleteFileFromTemporaryFolder(srcFile);
            }
            return result;
        }
    }
}