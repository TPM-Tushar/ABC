//----------------------------------------------------------------------------------------
//File Name: IsFocConverter.cs
//Author : Rohit Sanjay Khatale
//Creation Date :  09/Aug/2013
//ECR NO:209
//Desc : RDPR Main window
//      
//----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Com.Cdacindia.Gist.NetISMAPI;
using Com.Cdacindia.Gist.RevConverters;
using Com.Cdacindia.Gist.Converters;
using System.Runtime.InteropServices;

namespace ECDataAPI.Common
{

    public static class IsFocConverter
    {
        static Com.Cdacindia.Gist.NetISMAPI.Converter objNetISMAPIConverter;
        static Com.Cdacindia.Gist.Converters.Converter objConverter;
        static RevConverter objRevConverter;

        /// <summary>
        /// Convert string unicode to isfoc
        /// </summary>
        /// <param name="strUnicode"></param>
        /// <returns></returns>

        public static string GetIsfoc(string strUnicode)
        {
            string toIsFoc;
            string strgISCII;
            try
            {
                objNetISMAPIConverter = new Com.Cdacindia.Gist.NetISMAPI.Converter();
                objConverter = new Com.Cdacindia.Gist.Converters.Converter();
                objRevConverter = new RevConverter();

                if (string.IsNullOrEmpty(strUnicode))
                    strgISCII = string.Empty;
                else
                    strgISCII = objNetISMAPIConverter.UnicodeToIscii(strUnicode);//"ಸದರಿ ಸ್ವತ್ತಿನ ");
                toIsFoc = objConverter.IsciiToIsfoc(strgISCII, "Kannada");
                return toIsFoc;

            }
            catch (Exception e)
            {
                //return "ERROR " + e.Message;
                //commented by Prashanth M V on 19/07/2019 to rectify error string
                return "ISFOCCONVERROR " + e.Message;
            }
        }

        /// <summary>
        /// Check string unicode complience or not
        /// </summary>
        /// <param name="IsFoc"></param>
        /// <returns>if string is unicode complience return true</returns>

        public static bool CheckUnicodeComplience(string isFoc)
        {
            objNetISMAPIConverter = new Com.Cdacindia.Gist.NetISMAPI.Converter();
            objConverter = new Com.Cdacindia.Gist.Converters.Converter();
            objRevConverter = new RevConverter();

            string convString;
            //Step1 
            convString = objRevConverter.IsfocToIscii(isFoc, "Kannada");
            //Step 2
            convString = objNetISMAPIConverter.IsciiToUnicode(convString, "KNB");
            //Step 3 agin assign to strgISCII varriable
            convString = objNetISMAPIConverter.UnicodeToIscii(convString);
            //step 4 //Converting  Unicode to IsFoc 
            convString = objConverter.IsciiToIsfoc(convString, "Kannada");
            // ToIsFoc = conv.UnicodeToIscii(ToUnicode);
            if (isFoc.Equals(convString))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        ///convert string isfoc to unicode
        /// </summary>
        /// <param name="IsFoc"></param>
        /// <returns></returns>
        public static string GetUnicode(string isFoc)
        {
            objNetISMAPIConverter = new Com.Cdacindia.Gist.NetISMAPI.Converter();
            objRevConverter = new RevConverter();
            string convString;
            //Step1 convert isfoc to Iscii
            convString = objRevConverter.IsfocToIscii(isFoc, "Kannada");
            //Step 2 convert Iscii txet to Unicode
            convString = objNetISMAPIConverter.IsciiToUnicode(convString, "KNB");
            //Returning Unicode
            return convString;
        }

    }
}
