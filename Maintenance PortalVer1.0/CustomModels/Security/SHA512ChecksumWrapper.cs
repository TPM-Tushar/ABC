using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Security
{
    public class SHA512ChecksumWrapper
    {
        public static string GenerateSalt(int lenght)
        {
            return Security.SHA512Checksum.GenerateSalt(lenght);
        }

        public static string ComputeHash(string hash, string salt)
        {
            return Security.SHA512Checksum.ComputeHash(hash, salt);
        }
    }
}