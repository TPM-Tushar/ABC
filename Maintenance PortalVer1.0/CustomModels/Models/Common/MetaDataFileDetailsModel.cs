using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class MetaDataFileDetailsModel
    {


        public string EncryptedID { get; set; }
        public byte[] FileBytes { get; set; }
        public short ServiceID { get; set; }


    }
}
