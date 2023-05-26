//#region File Header
///*
//    * Project Id        :   -
//    * Project Name      :   Kaveri
//    * File Name         :   Cryptography.cs
//    * Author Name       :   -
//    * Creation Date     :   -
//    * Last Modified By  :   -
//    * Last Modified On  :   -
//    * Description       :   This class is used in URLEncrypt for enrypt the query string.
//*/
//#endregion

//namespace Cryptography
//{
//    #region References
//    using System;
//    using System.Text;
//    using System.Security.Cryptography;
//    using System.IO;
//    #endregion

//    public class CryptographyMethods
//    {
//        #region Properties

//        private static byte[] key = { };
//        private static byte[] IV = { 38, 55, 206, 48, 28, 64, 20, 16 };

//        #endregion

//        #region Public Methods

//        /// <summary>
//        /// Encrypt the string
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="stringKey"></param>
//        /// <returns></returns>
//        public static string Encrypt(string text, string stringKey)
//        {
//            try
//            {
//                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
//                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
//                Byte[] byteArray = Encoding.UTF8.GetBytes(text);
//                MemoryStream memoryStream = new MemoryStream();
//                CryptoStream cryptoStream = new CryptoStream(memoryStream,
//                    des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
//                cryptoStream.Write(byteArray, 0, byteArray.Length);
//                cryptoStream.FlushFinalBlock();
//                return Convert.ToBase64String(memoryStream.ToArray());
//            }
//            catch
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Decrypt the string
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="stringKey"></param>
//        /// <returns></returns>
//        public static string Decrypt(string text, string stringKey)
//        {
//            try
//            {
//                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
//                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
//                Byte[] byteArray = Convert.FromBase64String(text);
//                MemoryStream memoryStream = new MemoryStream();
//                CryptoStream cryptoStream = new CryptoStream(memoryStream,
//                    des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
//                cryptoStream.Write(byteArray, 0, byteArray.Length);
//                cryptoStream.FlushFinalBlock();
//                return Encoding.UTF8.GetString(memoryStream.ToArray());
//            }
//            catch
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Convert string to byte array
//        /// </summary>
//        /// <param name="hex"></param>
//        /// <returns></returns>
//        public static byte[] HexStringToBytes(string hex)
//        {
//            try
//            {
//                if (hex.Length == 0)
//                    return new byte[] { 0 };
//                if (hex.Length % 2 == 1)
//                    hex = "0" + hex;
//                byte[] result = new byte[hex.Length / 2];
//                for (int i = 0; i < hex.Length / 2; i++)
//                {
//                    result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
//                }
//                return result;
//            }
//            catch
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Generate random string
//        /// </summary>
//        /// <returns></returns>
//        public static string createChallenge()
//        {
//            try
//            {
//                System.Random rng = new Random(DateTime.Now.Millisecond);
//                // Create random string
//                byte[] salt = new byte[64];
//                for (int i = 0; i < 64; )
//                {
//                    salt[i++] = (byte)rng.Next(65, 90);
//                }
//                string challenge = string.Empty;
//                challenge = BytesToHexString(salt);
//                return challenge.Substring(0, 8);
//            }
//            catch
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Convert byte array to string
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public static string BytesToHexString(byte[] input)
//        {
//            try
//            {
//                StringBuilder hexString = new StringBuilder(64);
//                for (int i = 0; i < input.Length; i++)
//                {
//                    hexString.Append(String.Format("{0:X2}", input[i]));
//                }
//                return hexString.ToString();
//            }
//            catch
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Generate hash
//        /// </summary>
//        /// <param name="inStr"></param>
//        /// <returns></returns>
//        public static Byte[] GenerateHash(string inStr)
//        {
//            try
//            {
//                System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
//                UTF8Encoding encoder = new UTF8Encoding();
//                return md5Hasher.ComputeHash(encoder.GetBytes(inStr.ToString().Trim()));
//            }
//            catch
//            {
//                throw;
//            }
//        }
//        #endregion
//    }
//}
