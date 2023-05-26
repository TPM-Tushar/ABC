using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ECDataAPI.Common
{
    public class DownloadCCModel
    {

        public string RemoteFileName;
        protected AnywhereCCService.PreRegCCService objccservice;
        public bool AutoSetChunkSize;
        public int ChunkSize;
        public int MaxRetries;
        protected int NumRetries;
        public int ChunkSizeSampleInterval;
        public int PreferredTransferDuration;
        protected long Offset;
        protected DateTime StartTime;
        public string LocalFilePath;			// this variable must be set prior to starting an Upload.  
        public long MaxRequestLength;
        public string Guid;
        public string LocalFileName;

        public event EventHandler ChunkSizeChanged;

        public DownloadCCModel(string RemoteFileName)
        {
            this.objccservice = new AnywhereCCService.PreRegCCService();// the web service object      
            this.RemoteFileName = RemoteFileName;
            this.AutoSetChunkSize = true; // take a sample of 5 small chunks and then change the chunk size to suit the bandwidth capacity.  bigger capacity = bigger chunks = more efficient.
            this.ChunkSize = 16 * 1024; // 16k by default
            this.MaxRetries = 50; // max number of corrupted chunks or failed transfers to allow before giving up
            this.NumRetries = 0;
            this.ChunkSizeSampleInterval = 15;// interval to update the chunk size, used in conjunction with AutoSetChunkSize. 
            this.PreferredTransferDuration = 800;// milliseconds, the timespan the class will attempt to achieve for each chunk, to give responsive feedback on the progress bar.
            this.Offset = 0;// used in persisting the transfer position 
            this.MaxRequestLength = 4096;   // default, this is updated so that the transfer class knows how much the server will accept
            string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
            sPath = sPath + @"\TempDocs\";
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);

            this.LocalFilePath = (Path.Combine(sPath, this.RemoteFileName)); //Path for create new file 
        }


        public bool DownloadCCChunkFormat(ref string errorMessage)
        {
            try
            {
                long FileSize = this.objccservice.GetFileSize(this.RemoteFileName);
                string FileSizeDescription = CalcFileSize(FileSize);
                int numIterations = 0;

                using (FileStream fs = new FileStream(LocalFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Seek(Offset, SeekOrigin.Begin);

                    // download the chunks from the web service one by one, until all the bytes have been read, meaning the entire file has been downloaded.
                    while (Offset < FileSize)
                    {
                        if (this.AutoSetChunkSize)
                        {
                            int currentIntervalMod = numIterations % this.ChunkSizeSampleInterval;
                            if (currentIntervalMod == 0)
                                this.StartTime = DateTime.Now;	// used to calculate the time taken to transfer the first 5 chunks
                            else if (currentIntervalMod == 1)
                                this.CalcAndSetChunkSize();
                        }
                        try
                        {
                            // although the DownloadChunk returns a byte[], it is actually sent using MTOM because of the configuration settings. 
                            byte[] Buffer = objccservice.DownloadCCChunk(this.RemoteFileName, this.Offset, ChunkSize, ref errorMessage);
                            fs.Write(Buffer, 0, Buffer.Length);
                            this.Offset += Buffer.Length;	// save the offset position for resume
                        }
                        catch (Exception ex)
                        {
                            // swallow the exception and try again
                            //Debug.WriteLine("Exception: " + ex.ToString());

                            if (NumRetries++ >= MaxRetries)	// too many retries, bail out
                            {
                                fs.Close();
                                throw new Exception("Error occurred during upload, too many retries.\r\n" + ex.Message);
                            }
                        }
                        // update the user interface by reporting progress.
                        string SummaryText = String.Format("Transferred {0} / {1}", CalcFileSize(Offset), FileSizeDescription);
                        //this.ReportProgress((int)(((decimal)Offset / (decimal)FileSize) * 100), SummaryText);
                        numIterations++;
                    }
                }



                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            finally
            {
                //this.objccservice.DeleteFile(this.RemoteFileName);
            }

        }

        protected void CalcAndSetChunkSize()
        {
            /* chunk size calculation is defined as follows 
             *		in the examples below, the preferred transfer time is 1500ms, taking one sample.
             *		
             *									  Example 1									Example 2
             *		Initial size				= 16384 bytes	(16k)						16384
             *		Transfer time for 1 chunk	= 800ms										2000 ms
             *		Average throughput / ms		= 16384b / 800ms = 20.48 b/ms				16384 / 2000 = 8.192 b/ms
             *		How many bytes in 1500ms?	= 20.48 * 1500 = 30720 bytes				8.192 * 1500 = 12228 bytes
             *		New chunksize				= 30720 bytes (speed up)					12228 bytes (slow down from original chunk size)
             */
            double transferTime = DateTime.Now.Subtract(this.StartTime).TotalMilliseconds;
            double averageBytesPerMilliSec = this.ChunkSize / transferTime;
            double preferredChunkSize = averageBytesPerMilliSec * this.PreferredTransferDuration;
            this.ChunkSize = (int)Math.Min(this.MaxRequestLength, Math.Max(4 * 1024, preferredChunkSize));	// set the chunk size so that it takes 1500ms per chunk (estimate), not less than 4Kb and not greater than 4mb // (note 4096Kb sometimes causes problems, probably due to the IIS max request size limit, choosing a slightly smaller max size of 4 million bytes seems to work nicely)			

            string statusMessage = String.Format("Chunk size: {0}{1}", CalcFileSize(this.ChunkSize), (this.ChunkSize == this.MaxRequestLength) ? " (max)" : "");
            if (this.ChunkSizeChanged != null)
                this.ChunkSizeChanged(statusMessage, EventArgs.Empty);
        }

        public static string CalcFileSize(long numBytes)
        {
            string fileSize = "";

            if (numBytes > 1073741824)
                fileSize = String.Format("{0:0.00} Gb", (double)numBytes / 1073741824);
            else if (numBytes > 1048576)
                fileSize = String.Format("{0:0.00} Mb", (double)numBytes / 1048576);
            else
                fileSize = String.Format("{0:0} Kb", (double)numBytes / 1024);

            if (fileSize == "0 Kb")
                fileSize = "1 Kb";	// min.							
            return fileSize;
        }

        public static bool CheckFileExists(string fileName)
        {
            //string filePath = HttpContext.Current.Server.MapPath(Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ECFileProcessingPath"], fileName));
            //if (File.Exists(filePath))
            //    return true;
            //else
            //    return false;

            throw new Exception("code commented by rafe");

        }
    }
}