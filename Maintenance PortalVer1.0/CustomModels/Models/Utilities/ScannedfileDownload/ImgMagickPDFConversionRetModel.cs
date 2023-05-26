using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Utilities.ScannedfileDownload
{
    public class ImgMagickPDFConversionRetModel
    {
        public Boolean Status { get; set; }
        public string CorrectedPDFFilePath { get; set; }
        public string CorrectedPDFFileName { get; set; }
        public Boolean serverError { get; set; }
        public Boolean IsImgMagickConversionError { get; set; }
    }
}
