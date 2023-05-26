
namespace ECDataAPI.Common
{
    using ECDataAPI.FileDownloadUploadService;


    #region References

   // using ECDataAPI.CertifiedCopy;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
   // using ECDataAPI.Co  DownloadUploadFiles;
    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    public class DownloadUploadFiles
    {
        #region Properties

        public int ChunkSize;
        public string FilePath;// = string.Empty;
        DownloadUploadFilesClient client;
        public string VirtualPath;

        #endregion

        #region Constructor

        /// <summary>
        /// DownloadUploadFiles
        /// </summary>
        public DownloadUploadFiles()
        {
            ChunkSize = 16 * 1024; //set the chunk size to retrive or upload bytes.
            FilePath = string.Empty;
            client = new DownloadUploadFilesClient();
            VirtualPath = string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use Upload file to server and return server file path
        /// required to send file data in byte[].
        /// </summary>
        /// <param name="filedata"></param>
        /// <param name="fileName"></param>
        /// <param name="directoryStructure"></param>
        /// <param name="serviceType"></param>
        /// <returns>server file path</returns>
        public string UploadByteArrayFile(byte[] filedata, string fileName, string directoryStructure)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[ChunkSize];
            while (bytesRead < filedata.Length)
            {
                int length = Math.Min(buffer.Length, filedata.Length - bytesRead);
                if (length != ChunkSize)
                {
                    buffer = null;
                    buffer = new byte[length];
                }
                Buffer.BlockCopy(filedata, bytesRead, buffer, 0, length);
                FilePath = client.Upload(buffer, bytesRead, fileName, directoryStructure);
                // FilePath = client.UploadWithVirtualPath(buffer, bytesRead, fileName, directoryStructure,out VirtualPath);
                bytesRead += length;
            }
            // bool check = client.CreatePasswordProtedtedScannedDocument(filePath);
            return FilePath;
        }


        public string UploadByteArrayFileVirtual(byte[] filedata, string fileName, string directoryStructure, out string virtualFilePath)
        {
            virtualFilePath = string.Empty;
            int bytesRead = 0;
            byte[] buffer = new byte[ChunkSize];
            try
            {
                while (bytesRead < filedata.Length)
                {
                    int length = Math.Min(buffer.Length, filedata.Length - bytesRead);
                    if (length != ChunkSize)
                    {
                        buffer = null;
                        buffer = new byte[length];
                    }
                    Buffer.BlockCopy(filedata, bytesRead, buffer, 0, length);
                    // FilePath = client.Upload(buffer, bytesRead, fileName, directoryStructure);
                    FilePath = client.UploadWithVirtualPath(buffer, bytesRead, fileName, directoryStructure, out virtualFilePath);
                    bytesRead += length;
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(@"E:\web\templog.txt"))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("INSIDE DownloadUploadFiles UploadByteArrayFileVirtual  Catch Block :" + ex);

                //    sw.Close();
                //}
                ApiExceptionLogs.LogError(ex);

                throw;
            }
            // bool check = client.CreatePasswordProtedtedScannedDocument(filePath);
            return FilePath;
        }

        /// <summary>
        /// Download file from server in byte[] format and retuen
        /// </summary>
        /// <param name="serverFilePath"></param>
        /// <returns>return byte[]</returns>
        public byte[] DownloadInByteArray(string serverFilePath)
        {
            ApiCommonFunctions.WriteProductionLog("Download function ");
            FilePath = serverFilePath;
            int offset = 0;
            long fileSize = this.client.GetFileSize(FilePath);
            ApiCommonFunctions.WriteProductionLog("filesize: " + fileSize);
            byte[] fileBuffer = new byte[fileSize];
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (offset < fileSize)
                    {
                        byte[] buffer = client.Download(this.FilePath, offset, ChunkSize);
                        ms.Write(buffer, 0, buffer.Length);
                        offset += ChunkSize;
                        if (offset > fileSize)
                            offset = offset - (int)(offset - fileSize);
                    }
                    fileBuffer = ms.ToArray();
                }
                ApiCommonFunctions.WriteProductionLog("download complete");
                return fileBuffer;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteProductionLog("exception donload: " + ex.InnerException);
                throw;
            }
        }


        public byte[] DownloadInByteArrayVirtualPath(string serverVirtualPath)
        {
            VirtualPath = serverVirtualPath;
            int offset = 0;
            long fileSize = this.client.GetFileSizeWithVirtualPath(VirtualPath);
            byte[] fileBuffer = new byte[fileSize];
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (offset < fileSize)
                    {
                        byte[] buffer = client.DownloadWithVirtualPath(this.VirtualPath, offset, ChunkSize);
                        ms.Write(buffer, 0, buffer.Length);
                        offset += ChunkSize;
                        if (offset > fileSize)
                            offset = offset - (int)(offset - fileSize);
                    }
                    fileBuffer = ms.ToArray();
                }
                return fileBuffer;
            }
            catch(Exception e)
            {
                ApiExceptionLogs.LogError(e);

                throw;
            }
        }

        /// <summary>
        /// Download file from server in byte[] format and retuen
        /// </summary>
        /// <param name="serverFilePath"></param>
        /// <returns>return byte[]</returns>
        public byte[] DownloadInByteArrayTempFile(string tempFilePath)
        {
            FilePath = tempFilePath;
            int offset = 0;
            //long fileSize = this.client.GetFileSize(FilePath);

            long fileSize = (new FileInfo(tempFilePath)).Length;
            byte[] fileBuffer = new byte[fileSize];
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (offset < fileSize)
                    {
                        byte[] buffer = DownloadTempFile(tempFilePath, offset, ChunkSize);
                        ms.Write(buffer, 0, buffer.Length);
                        offset += ChunkSize;
                        if (offset > fileSize)
                            offset = offset - (int)(offset - fileSize);
                    }
                    fileBuffer = ms.ToArray();
                }
                return fileBuffer;
            }
            catch
            {
                throw;
            }
        }

        public byte[] DownloadTempFile(string sFilePath, long lOffset, int iBufferSize)
        {
            try
            {
                // check that requested file exists
                if (!File.Exists(sFilePath))
                    throw new Exception(sFilePath + " does not exist");

                long fileSize = new FileInfo(sFilePath).Length;

                // if the requested Offset is larger than the file, quit.
                if (lOffset > fileSize)
                    throw new Exception("Invalid Download Offset" + sFilePath + " The file size is {0}, received request for offset {1}");

                // open the file to return the requested chunk as a byte[]
                byte[] tmpBuffer;
                int bytesRead;

                try
                {
                    using (FileStream fs = new FileStream(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        fs.Seek(lOffset, SeekOrigin.Begin);	// this is relevent during a retry. otherwise, it just seeks to the start
                        tmpBuffer = new byte[iBufferSize];
                        bytesRead = fs.Read(tmpBuffer, 0, iBufferSize);	// read the first chunk in the buffer (which is re-used for every chunk)
                    }
                    if (bytesRead != iBufferSize)
                    {
                        // the last chunk will almost certainly not fill the buffer, so it must be trimmed before returning
                        byte[] TrimmedBuffer = new byte[bytesRead];
                        Array.Copy(tmpBuffer, TrimmedBuffer, bytesRead);
                        return TrimmedBuffer;
                    }
                    else
                        return tmpBuffer;
                }
                catch
                {
                    throw new Exception("Error while reading file");

                }
            }
            catch
            {
                throw new Exception("Error while reading file ");
            }
        }


        /// <summary>
        /// Downlad file in byte[] and write in downladfile where it should write the file
        /// </summary>
        /// <param name="serverFilePath"></param>
        /// <param name="downloadFilePath"></param>
        /// <returns></returns>
        public bool DownloadFile(string serverFilePath, string downloadFilePath)
        {
            try
            {
                byte[] buffer = DownloadInByteArray(serverFilePath);
                File.WriteAllBytes(downloadFilePath, buffer);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool DownloadFileWithVirtualPath(string serverVirtualPath, string downloadFilePath)
        {
            try
            {
                byte[] buffer = DownloadInByteArrayVirtualPath(serverVirtualPath);
                File.WriteAllBytes(downloadFilePath, buffer);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// upload file the file convert into byte[] array and upload.
        /// </summary>
        /// <param name="uploadFilePath"></param>
        /// <param name="fileName"></param>
        /// <param name="directoryStructure"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public string UploadFile(string uploadFilePath, string fileName, string directoryStructure, short serviceType)
        {
            try
            {
                if (File.Exists(uploadFilePath))
                {
                    byte[] inputDate = File.ReadAllBytes(uploadFilePath);
                    return UploadByteArrayFile(inputDate, fileName, directoryStructure); ;
                }
                else
                    throw new Exception("File Not Found Exception");
            }
            catch
            {
                throw;
            }
        }

        public string UploadFileWithVirtualPath(string uploadFilePath, string fileName, string directoryStructure, short serviceType, out string virtualFilePath)
        {
            virtualFilePath = string.Empty;
            try
            {
                if (File.Exists(uploadFilePath))
                {
                    byte[] inputDate = File.ReadAllBytes(uploadFilePath);
                    return UploadByteArrayFileVirtual(inputDate, fileName, directoryStructure, out virtualFilePath);
                }
                else
                    throw new Exception("File Not Found Exception");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get scanned document file page count.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int GetPageCount(string filePath)
        {
            try
            {
                return client.PageCount(filePath);
            }
            catch
            {
                throw;
            }
        }

        public int GetPageCountVirtual(string virtualPath)
        {
            try
            {
                return client.PageCountWithVirtualPath(virtualPath);
            }
            catch
            {
                throw;
            }
        }

        public string GetFullPath(string virtualPath)
        {
            try
            {
                return client.GetFullPath(virtualPath);
            }
            catch
            {
                throw;
            }
        }

        public string GetURIPathofVirtual(string virtualPath)
        {
            try
            {
                return client.GetURIPathofVirtual(virtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Check the file is exist on server
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsFileReadable(string filePath)
        {
            try
            {
                return client.IsFileReadable(filePath);
            }
            catch
            {
                throw;
            }
        }

        public bool IsFileReadableWithVirtual(string virtualPath)
        {
            try
            {
                return client.IsFileReadableWithVirtualPath(virtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Upload thumb image to server 
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        /// <param name="imageName"></param>
        /// <param name="directoryStructure"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public string UploadImage(byte[] imageData, int imgWidth, int imgHeight, string imageName, string directoryStructure)
        {
            try
            {
                return client.UploadImage(imageData, imgWidth, imgHeight, imageName, directoryStructure);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Upload thumb image to server 
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        /// <param name="imageName"></param>
        /// <param name="directoryStructure"></param>
        /// <param name="virtualFilePath"></param>
        /// <returns></returns>
        public string UploadImageWithVirtual(byte[] imageData, int imgWidth, int imgHeight, string imageName, string directoryStructure, out string virtualFilePath)
        {
            try
            {
                return client.UploadImageWithVirtualPaths(imageData, imgWidth, imgHeight, imageName, directoryStructure, out virtualFilePath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// CopyExistingFile
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFilePath"></param>
        /// <returns></returns>
        public bool CopyExistingFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                return client.CopyExistingFile(sourceFilePath, destinationFilePath);
            }
            catch
            {
                throw;
            }
        }

        public bool CopyExistingFileWithVirtual(string sourceVirtualPath, string destinationVirtualPath)
        {
            try
            {
                return client.CopyExistingFileWithVirtualPaths(sourceVirtualPath, destinationVirtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GetFileLenghtChecksum
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        public string GetFileLenghtChecksum(string sFilePath)
        {
            try
            {
                return client.GetFileLenghtChecksum(sFilePath);
            }
            catch
            {
                throw;
            }
        }

        public string GetFileLenghtChecksumWithVirtual(string sVirtualPath)
        {
            try
            {
                return client.GetFileLenghtChecksumWithVirtual(sVirtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GetCertifiedCopy
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        //public byte[] GetCertifiedCopy(DocumentDetails details)
        //{
        //    string tempFile = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["TempFileProcess"], "TempFile_" + DateTime.Now.Millisecond + ".pdf");
        //    byte[] fileData = DownloadInByteArray(details.FilePath);
        //    PdfReader readFile = new PdfReader(fileData);
        //    FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
        //    try
        //    {
        //        using (PdfStamper stamper = new PdfStamper(readFile, fs))
        //        {
        //            int pageNumber = readFile.NumberOfPages;
        //            iTextSharp.text.Rectangle prect = readFile.GetPageSize(1);
        //            int lastPage = pageNumber + 1;
        //            stamper.InsertPage(lastPage, prect);
        //            PdfContentByte pcb = stamper.GetUnderContent(lastPage);
        //            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        //            pcb.SetFontAndSize(bf, 10);
        //            pcb.BeginText();
        //            string text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf1; //"Departmment Of Stamps And Registration";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 50, 0);
        //            //pcb.NewlineText();
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf2; //"Certificate";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 80, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf3; //"Certificate Under Section 10A of Karnataka Stamp Act,1957 for the purpose of section 10 of ";
        //            //pcb.NewlineShowText(text);
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 110, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf4; //"Karnataka Stamp Act,1957 section 10 of Karnataka Stamp Act,1957";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 140, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf5; //"Certificate that sum of Rs 10 being proper stamp duty been remitted as follows:";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 170, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf6; //"Digitally Signed by Departmment Of Stamps And Registration";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 200, prect.Height - 300, 0);

        //            //text = "Place:" + details.OfficeName;
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf7 + details.OfficeName;

        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 330, 0);

        //            //text = "Date:" + DateTime.Now.ToString("dd/MMM/yyyy");
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf8 + DateTime.Now.ToString("dd/MMM/yyyy");
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 360, 0);

        //            PdfPTable table = new PdfPTable(2);
        //            table.HorizontalAlignment = PdfContentByte.ALIGN_CENTER;
        //            //var firstRowCell1 = new PdfPCell(new Phrase("Type"));
        //            //var firstRowCell2 = new PdfPCell(new Phrase("Amount (Rs.)"));
        //            var firstRowCell1 = new PdfPCell(new Phrase(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf9));
        //            var firstRowCell2 = new PdfPCell(new Phrase(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf10));
        //            PdfPCell[] row1Cells = { firstRowCell1, firstRowCell2 };
        //            var row1 = new PdfPRow(row1Cells);
        //            table.Rows.Add(row1);

        //            //Repeat Table data
        //            foreach (var feesDetails in details.FeesRuleAmount)
        //            {
        //                var nextRowCell1 = new PdfPCell(new Phrase(feesDetails.FeesRule));
        //                var nextRowCell2 = new PdfPCell(new Phrase(feesDetails.Amount.ToString()));
        //                PdfPCell[] row2Cells = { nextRowCell1, nextRowCell2 };
        //                var row2 = new PdfPRow(row2Cells);
        //                table.Rows.Add(row2);
        //            }
        //            table.TotalWidth = 300f;
        //            table.WriteSelectedRows(0, -1, 100, prect.Height - 190, pcb);
        //            //text = "By " + details.partyName + ",residing at " + details.Address;
        //            text = string.Format(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf11, details.partyName, details.Address);

        //            Phrase p = new Phrase(text, FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED, 10));
        //            ColumnText ct = new ColumnText(pcb);
        //            ct.SetSimpleColumn(p, 100, prect.Height - 240, 530, 36, 25, Element.ALIGN_LEFT);
        //            ct.Go();

        //            pcb.EndText();
        //        }
        //        byte[] finalData = File.ReadAllBytes(tempFile);
        //        return finalData;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (File.Exists(tempFile))
        //            File.Delete(tempFile);
        //    }
        //}

        /// <summary>
        /// GetCertifiedCopyWithVirtualPath
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        //public byte[] GetCertifiedCopyWithVirtualPath(DocumentDetails details)
        //{
        //    string tempFile = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["TempFileProcess"], "TempFile_" + DateTime.Now.Millisecond + ".pdf");
        //    byte[] fileData = DownloadInByteArrayVirtualPath(details.VirtualFilePath);
        //    PdfReader readFile = new PdfReader(fileData);
        //    FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
        //    try
        //    {
        //        using (PdfStamper stamper = new PdfStamper(readFile, fs))
        //        {
        //            int pageNumber = readFile.NumberOfPages;
        //            iTextSharp.text.Rectangle prect = readFile.GetPageSize(1);
        //            int lastPage = pageNumber + 1;
        //            stamper.InsertPage(lastPage, prect);
        //            PdfContentByte pcb = stamper.GetUnderContent(lastPage);
        //            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        //            pcb.SetFontAndSize(bf, 10);
        //            pcb.BeginText();
        //            string text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf1; //"Departmment Of Stamps And Registration";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 50, 0);
        //            //pcb.NewlineText();
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf2; //"Certificate";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 80, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf3; //"Certificate Under Section 10A of Karnataka Stamp Act,1957 for the purpose of section 10 of ";
        //            //pcb.NewlineShowText(text);
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 110, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf4; //"Karnataka Stamp Act,1957 section 10 of Karnataka Stamp Act,1957";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 140, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf5; //"Certificate that sum of Rs 10 being proper stamp duty been remitted as follows:";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, 300, prect.Height - 170, 0);

        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf6; //"Digitally Signed by Departmment Of Stamps And Registration";
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 200, prect.Height - 300, 0);

        //            //text = "Place:" + details.OfficeName;
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf7 + details.OfficeName;

        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 330, 0);

        //            //text = "Date:" + DateTime.Now.ToString("dd/MMM/yyyy");
        //            text = Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf8 + DateTime.Now.ToString("dd/MMM/yyyy");
        //            pcb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, 100, prect.Height - 360, 0);

        //            PdfPTable table = new PdfPTable(2);
        //            table.HorizontalAlignment = PdfContentByte.ALIGN_CENTER;
        //            //var firstRowCell1 = new PdfPCell(new Phrase("Type"));
        //            //var firstRowCell2 = new PdfPCell(new Phrase("Amount (Rs.)"));
        //            var firstRowCell1 = new PdfPCell(new Phrase(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf9));
        //            var firstRowCell2 = new PdfPCell(new Phrase(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf10));
        //            PdfPCell[] row1Cells = { firstRowCell1, firstRowCell2 };
        //            var row1 = new PdfPRow(row1Cells);
        //            table.Rows.Add(row1);

        //            //Repeat Table data
        //            foreach (var feesDetails in details.FeesRuleAmount)
        //            {
        //                var nextRowCell1 = new PdfPCell(new Phrase(feesDetails.FeesRule));
        //                var nextRowCell2 = new PdfPCell(new Phrase(feesDetails.Amount.ToString()));
        //                PdfPCell[] row2Cells = { nextRowCell1, nextRowCell2 };
        //                var row2 = new PdfPRow(row2Cells);
        //                table.Rows.Add(row2);
        //            }
        //            table.TotalWidth = 300f;
        //            table.WriteSelectedRows(0, -1, 100, prect.Height - 190, pcb);
        //            //text = "By " + details.partyName + ",residing at " + details.Address;
        //            text = string.Format(Localization.Resources.DownloadUploadFiles.DownloadUploadFilesResource.CCPdf11, details.partyName, details.Address);

        //            Phrase p = new Phrase(text, FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED, 10));
        //            ColumnText ct = new ColumnText(pcb);
        //            ct.SetSimpleColumn(p, 100, prect.Height - 240, 530, 36, 25, Element.ALIGN_LEFT);
        //            ct.Go();

        //            pcb.EndText();
        //        }
        //        byte[] finalData = File.ReadAllBytes(tempFile);
        //        return finalData;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (File.Exists(tempFile))
        //            File.Delete(tempFile);
        //    }
        //}

        public long GetFileSize(string sFilePath)
        {
            try
            {
                return client.GetFileSize(sFilePath);
            }
            catch
            {
                throw;
            }
        }

        public long GetFileSizeWithVirtualPath(string sVirtualPath)
        {
            try
            {
                return client.GetFileSizeWithVirtualPath(sVirtualPath);
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteExistingithVirtualPath(string sVirtualPath)
        {
            try
            {
                return client.DeleteExistingFileWithVirtualPath(sVirtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Added by Shrikant J
        /// </summary>
        /// <param name="sVirtualPath"></param>
        /// <returns></returns>
        public string GetRDPRXMLInnerXmlDetails(string sVirtualPath)
        {
            try
            {
                return client.GetRDPRXMLInnerXmlDetails(sVirtualPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Added by Shrikant Jadhav
        /// </summary>
        /// <param name="supportDocumentList"></param>
        /// <param name="supportingDocDescs"></param>
        /// <param name="sFileName"></param>
        /// <param name="sdirectoryStructure"></param>
        /// <param name="virtualfilepath"></param>
        /// <returns></returns>
        public string UploadFirmSupportingDocumentWithVirtualPath(List<string> supportDocumentList, string[] supportingDocDescs, string sFileName, string sdirectoryStructure, out string virtualfilepath)
        {
            virtualfilepath = string.Empty;
            try
            {
                return client.UploadFirmSupportingDocumentWithVirtualPath(supportDocumentList.ToArray(), supportingDocDescs, sFileName, sdirectoryStructure, out virtualfilepath);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}