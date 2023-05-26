using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CustomModels.Security
{
    public class SHA512Checksum
    {
        public static string CalculateSHA512Hash(string input)
        {
            // step 1, calculate SHA512 hash from input
            SHA512 md5 = System.Security.Cryptography.SHA512.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Random number generation required for encryption
        /// </summary>
        /// <param name="length">Length of the key</param>
        /// <returns>Random Number</returns>
        public static string GenerateSalt(int length)
        {
            byte[] _randomArray = new byte[length];
            string _salt;
            //Create random salt and convert to string
            RNGCryptoServiceProvider _randomNumberGenerator = new RNGCryptoServiceProvider();
            _randomNumberGenerator.GetBytes(_randomArray);
            _salt = Convert.ToBase64String(_randomArray);
            //$ and / characters are used in URL encryption, so required to be removed from the key
            _salt = _salt.Replace('$', 'a');
            _salt = _salt.Replace('/', 'b');
            return _salt;
        }

        /// <summary>
        /// Generate MD5 hash of the string provided
        /// </summary>
        /// <param name="hash">String to be encrypted</param>
        /// <param name="salt">Random Number for encryption</param>
        /// <returns></returns>
        public static string ComputeHash(String hash, string salt)
        {
            //convert the salt into hex
            string _hexSalt = StringToHexEncoding(salt);
            string _concatHash = hash + _hexSalt;
            SHA512CryptoServiceProvider md5 = new SHA512CryptoServiceProvider();
            byte[] _doubleSaltedPassword = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(_concatHash));
            string _md5Password = BitConverter.ToString(_doubleSaltedPassword);
            _md5Password = _md5Password.Replace("-", "");

            return _md5Password;
        }

        /// <summary>
        /// convert simple string to hex string, required for computing Hash
        /// </summary>
        /// <param name="input">String</param>
        /// <returns>Hex String</returns>
        public static string StringToHexEncoding(string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder builder = new StringBuilder();
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = String.Format("{0:X}", value);
                builder.Append(hexOutput);
            }
            return builder.ToString();
        }
    }
}