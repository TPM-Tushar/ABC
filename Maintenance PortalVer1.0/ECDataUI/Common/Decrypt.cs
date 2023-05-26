// ***********************************************************************************
// Author: Prashanth M V
// Revision History
// 1. Prashanth M V 6/01/2011
// Introduction
//(Reference Taken from Digital Sign Utility)
// This file defines  Decrypt class. The class has two members and one member function.
// This class is usefull to decrypt the encrypted file (Encrypted using blow fish)
// ***********************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;

namespace ECDataUI.Common
{
    class Decrypt
    {

        public string encFilePath { get; set; } // string to store the encrypted file path
        public string decFilePath { get; set; } // string to store the decrypted file path


        /// <summary>
        /// This function Decrypts the encrypted file 
        /// </summary>
        /// <param name="keyEncDec"></param> ' key value to decrypt
        public bool DecryptFile(string keyEncDec, ref string errorMessage)
        {
            try
            {

                if (System.IO.Path.GetFileName(encFilePath).ToString().EndsWith("enc"))
                {
                }
                //added by Prashanth M V on 11/1/11
                if (encFilePath == string.Empty || System.IO.Path.GetFileName(encFilePath).ToString().EndsWith("enc") == false)
                {
                    //MessageBox.Show("Invalid Path of the Encrypted file ", "File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // get the exact file name from the path 
                String encFile = System.IO.Path.GetFileName(encFilePath); //string to get the encrypted file name
                FileInfo fInfo = new FileInfo(encFilePath); //string to get the encrypted file info


                long numBytes = fInfo.Length; //string to store length of the file

                FileStream fStream = new FileStream(encFilePath, FileMode.Open, FileAccess.Read); //file stream for encrypte file
                BinaryReader br = new BinaryReader(fStream); //Binary reader to read the file stream

                // convert the file to a byte array 
                Byte[] Message = br.ReadBytes((int)numBytes);
                br.Close();
                fStream.Close();
                fStream.Dispose();

                // *************************
                // Pad the message length to multiple of 8
                // *************************
                if ((Message.Length % 8) != 0) Array.Resize(ref Message, (Message.Length + 8 - 1) / 8 * 8);
                // *************************

                // *************************
                // Act "in place"
                // *************************
                Byte[] Key = BlowFish.ToByteArray(keyEncDec); //byte to store byte array
                BlowFish fish = new BlowFish(Key); //blow fish object to decrypt the message
                fish.Decrypt(Message);
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(decFilePath, FileMode.Create)))
                {

                    binWriter.Write(Message);
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Decrypt Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorMessage = "Type: " + ex.GetType().ToString() + " \n\n Message :" + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage = errorMessage + " Inner Exception: " + ex.InnerException.ToString();
                }
                return false;
            }
            finally
            {
            }

        }


    }
}
