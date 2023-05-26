#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Gauri
    * File Name         :   URLEncrypt.cs
    * Author Name       :   - Mohd Rafe
    * Creation Date     :   - 12-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  This class is used for encrypt/decrypt url querystring.
*/
#endregion
 
namespace CustomModels.Security
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Cryptography;
    #endregion

    public class URLEncrypt
    {
        #region Methods

        /// <summary>
        /// Encrypt the parameters without key value
        /// </summary>
        /// <param name="strParametersToEncrypt"></param>
        /// <returns></returns>
        private static string EncryptParameters_WithoutKeyValue(System.String[] strParametersToEncrypt)
        {
            try
            {
                // Holds the string to calculate digest which will be sent in url along with the parameteres.
                System.String parametersToEncrypt = string.Empty;
                // Random public key for the encryption which will be sent in url for decryption.
                System.String randomKeyForEncryption = CryptographyMethods.createChallenge();
                // Concanated paramters encrypted.
                System.String encryptedParameterString = String.Empty;
                // SHA1 Hash
                System.String digest = String.Empty;
                for (int i = 0; i < strParametersToEncrypt.Length; i++)
                {
                    if (strParametersToEncrypt[i].Contains("/"))
                        throw new Exception("URL is tempered. Please try again or contact to help desk.");
                    if (i == 0)
                        parametersToEncrypt = strParametersToEncrypt[i];
                    else
                        parametersToEncrypt = parametersToEncrypt + "#" + strParametersToEncrypt[i];
                }
                // parametersToEncrypt = "amol#1234#an kit#my&name#file+file#f&+ t";
                // Encrypt parameter string.
                randomKeyForEncryption = randomKeyForEncryption.Replace('/', '$');
                encryptedParameterString = CryptographyMethods.Encrypt(parametersToEncrypt, randomKeyForEncryption);
                encryptedParameterString = encryptedParameterString.Replace('/', '$');
                // Calculate digest for stringToEncrypt + randomKeyForEncryption
                System.Byte[] byteDigest = CryptographyMethods.GenerateHash(parametersToEncrypt + randomKeyForEncryption);
                digest = CryptographyMethods.BytesToHexString(byteDigest);
                //digest = "";
                return System.Web.HttpUtility.UrlPathEncode(encryptedParameterString) + "/" + System.Web.HttpUtility.UrlPathEncode(randomKeyForEncryption) + "/" + System.Web.HttpUtility.UrlPathEncode(digest);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Encrypt the parameters 
        /// </summary>
        /// <param name="strParametersToEncrypt"></param>
        /// <returns></returns>
        public static string EncryptParameters(System.String[] strParametersToEncrypt,string isForLogin="0")
        {
            try
            {
                // Holds the string to calculate digest which will be sent in url along with the parameteres.
                System.String parametersToEncrypt = string.Empty;
                // Random public key for the encryption which will be sent in url for decryption.\
                System.String randomKeyForEncryption = string.Empty;
                if (isForLogin=="1")
                {
                    randomKeyForEncryption = "ABcdef@12345%&17";
                }
                else
                randomKeyForEncryption = GenerateSalt(16);
                // Concanated paramters encrypted.
                System.String encryptedParameterString = String.Empty;
                System.String digest = String.Empty;
                //"/" Characte is not allowed for encryption
                //Concat the parametrs with # value
                for (int i = 0; i < strParametersToEncrypt.Length; i++)
                {
                    if (strParametersToEncrypt[i].Contains("/"))
                        throw new Exception("URL is tempered. Please try again or contact to help desk.");
                    if (i == 0)
                        parametersToEncrypt = strParametersToEncrypt[i];
                    else
                        parametersToEncrypt = parametersToEncrypt + "#" + strParametersToEncrypt[i];
                }
                randomKeyForEncryption = randomKeyForEncryption.Replace('/', '$');
                // Encrypt parameter string.
                encryptedParameterString = CryptographyMethods.Encrypt(parametersToEncrypt, randomKeyForEncryption);
                encryptedParameterString = encryptedParameterString.Replace('/', '$');
                // Calculate digest for stringToEncrypt + randomKeyForEncryption
                System.Byte[] byteDigest = CryptographyMethods.GenerateHash(parametersToEncrypt + randomKeyForEncryption);
                digest = BitConverter.ToString(byteDigest).Replace("-", "");
                //digest = "";
                //Returen Encrypted Encrypted parametrs,encrypted key and digest
                return System.Web.HttpUtility.UrlPathEncode(encryptedParameterString) + "/" + System.Web.HttpUtility.UrlPathEncode(randomKeyForEncryption) + "/" + System.Web.HttpUtility.UrlPathEncode(digest);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Decrypt the parameters without key value
        /// </summary>
        /// <param name="strParametersToDecrypt"></param>
        /// <returns></returns>
        private static string[] DecryptParameters_WithoutKeyValue(string[] strParametersToDecrypt)
        {
            try
            {
                //Holds the string of Encrypted parameters separated by #
                System.String strEncryptedParameters = strParametersToDecrypt[0];
                //Public Key for the decryption
                System.String randomKeyForDecryption = strParametersToDecrypt[1];
                //Digest to check whether URL was tampered
                System.String strDigestFromQueryString = strParametersToDecrypt[2];
                strEncryptedParameters = strEncryptedParameters.Replace('$', '/');
                randomKeyForDecryption = randomKeyForDecryption.Replace('$', '/');
                //Parameters separated
                System.String[] strParameters = strEncryptedParameters.Split('#');
                System.String strComputedDigest = string.Empty;
                for (int i = 0; i < strParameters.Length; ++i)
                {
                    //Parameters Decrypted
                    strParameters[0] = CryptographyMethods.Decrypt(strParameters[0], randomKeyForDecryption);
                    strComputedDigest += strParameters[0];
                }
                strComputedDigest += randomKeyForDecryption;
                //Digest again Computed
                System.Byte[] byteDigest = CryptographyMethods.GenerateHash(strComputedDigest);
                strComputedDigest = CryptographyMethods.BytesToHexString(byteDigest);
                //Check both Digest one Computed and other from URL
                if (!strComputedDigest.Equals(strDigestFromQueryString))
                {
                    throw new Exception("URL is tempered. Please try again or contact to help desk.");
                    //URL Tampered
                }
                return strParameters;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Decrypt the parameters 
        /// </summary>
        /// <param name="strParametersToDecrypt"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DecryptParameters(string[] strParametersToDecrypt)
        {
            try
            {
                System.String strEncryptedParameters = null;
                System.String randomKeyForDecryption = null;
                System.String strDigestFromQueryString = null;
                if (strParametersToDecrypt.Length == 3)
                {
                    //Holds the string of Encrypted parameters separated by #
                    if (!(strParametersToDecrypt[0] == null || strParametersToDecrypt[1] == null || strParametersToDecrypt[2] == null))
                    {
                        strEncryptedParameters = strParametersToDecrypt[0];
                        //Public Key for the decryption
                        randomKeyForDecryption = strParametersToDecrypt[1];
                        //Digest to check whether URL was tampered
                        strDigestFromQueryString = strParametersToDecrypt[2];
                    }
                    else
                        throw new Exception("URL is tempered. Please try again or contact to help desk.");
                }
                strEncryptedParameters = strEncryptedParameters.Replace('$', '/');
                randomKeyForDecryption = randomKeyForDecryption.Replace('$', '/');
                strEncryptedParameters = strEncryptedParameters.Replace(' ', '+');
                randomKeyForDecryption = randomKeyForDecryption.Replace(' ', '+');
                strEncryptedParameters = strEncryptedParameters.Replace("%20", "+");
                randomKeyForDecryption = randomKeyForDecryption.Replace("%20", "+");

                //Parameters separated
                System.String[] strParameters = strEncryptedParameters.Split('#');
                System.String strComputedDigest = string.Empty;
                for (int i = 0; i < strParameters.Length; ++i)
                {
                    //Parameters Decrypted
                    strParameters[0] = CryptographyMethods.Decrypt(strParameters[0], randomKeyForDecryption);
                    strComputedDigest += strParameters[0];
                }
                strComputedDigest += randomKeyForDecryption;
                //Digest again Computed
                System.Byte[] byteDigest = CryptographyMethods.GenerateHash(strComputedDigest);
                strComputedDigest = BitConverter.ToString(byteDigest).Replace("-", "");
                //Check both Digest one Computed and other from URL
                if (!strComputedDigest.Equals(strDigestFromQueryString))
                    throw new Exception("URL is tempered. Please try again or contact to help desk."); //URL Tampered

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string[] str = (strParameters[0].ToString().Split('#'));
                for (int i = 0; i < str.Length; ++i)
                {
                    string[] splitParameter = str[i].Split('=');
                    parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                }
                return parameters;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Random number generation required for encryption
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GenerateSalt(int length)
        {
            try
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
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
