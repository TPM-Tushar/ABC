

//#region File Header
///*
//    * Project Id        :   -
//    * Project Name      :   Kaveri
//    * File Name         :   KaveriSession.cs
//    * Author Name       :   
//    * Creation Date     :   -
//    * Last Modified By  :   -
//    * Last Modified On  :   -
//    * Description       :   Used to Maintain Session in project
//*/
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

using CustomModels.Models.UserManagement;
using System;
using System.Web;
namespace ECDataUI.Session
{
    [Serializable]
    public class KaveriSession
    {

        #region Public Constructor

        /// <summary>
        /// KaveriSession
        /// </summary>
        private KaveriSession()
        {
            //Session[]
        }

        #endregion

        #region Private/Public Properties



        /// <summary>
        /// Static reference of currect class
        /// </summary>
        public static KaveriSession Current
        {
            get
            {
                HttpContext.Current.Session["__KaveriSession__"] = HttpContext.Current.Session["__KaveriSession__"] as KaveriSession ?? new KaveriSession();
                return (KaveriSession)HttpContext.Current.Session["__KaveriSession__"];
            }
            set  
            {
                HttpContext.Current.Session["__KaveriSession__"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// EndSession
        /// </summary>
        public void EndSession()
        {
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// Method to Set Session variables
        /// </summary>
        //public static void SetSession(UserModel userDetails)
        //{
        //    try
        //    {
        //        KaveriSession.Current.UserID = userDetails.UserID;
        //        //KaveriSession.Current.UserProfileID = userDetails.UserProfileID;
        //        KaveriSession.Current.UserName = userDetails.UserName;
        //        KaveriSession.Current.FullName = userDetails.FirstName + " " + userDetails.MiddleName + " " + userDetails.LastName;
        //        KaveriSession.Current.RoleID = userDetails.DefaultRoleID;
        //        KaveriSession.Current.OfficeID = userDetails.OfficeID;
        //        KaveriSession.Current.DocumentID = 0;
        //        KaveriSession.Current.RoleName = userDetails.RoleName;

        //        KaveriSession.Current.HomePgURL = userDetails.HomePgURL;
        //    }
        //    catch
        //    {
        //        KaveriSession.Current.UserID = 0;
        //        throw;
        //    }
        //}

        /// <summary>
        /// IsSessionExpired
        /// </summary>
        /// <returns></returns>
        public bool IsSessionExpired()
        {
            return KaveriSession.Current.UserID != 0 ? false : true;
        }


        #endregion

        #region Get/Set Properties


        public String SessionSalt { get; set; }

        public long UserID { get; set; }
        public string RoleName { get; set; }
        public long DocumentID { get; set; }
        public short OfficeID { get; set; }
        public string UserName { get; set; }
        public string OfficeName { get; set; }
        public long KioskTokenID { get; set; }
        public int FirmApplicationTypeID { get; set; }
        public string TokenNumber { get; set; }
       
        public short RoleID { get; set; }
 
        public byte DocumentStatusID { get; set; }
        public long FirmID { get; set; }


        public string EncryptedID { get; set; }
        public string ModuleName { get; set; }
        public Int16 ServiceID { get; set; }
         
        public String HomePgURL { get; set; }
        public string FullName { get; set; }

        public long ResourceID { get; set; }
        public Int32 ParentMenuId { get; set; }
        public Int32 TopMostParentMenuId { get; set; }
       // public int? ModuleID { get; set; }
        public int ModuleID { get; set; } //added by akash
        public string FirmName { get; set; }


        public long CCApplicationID { get; set; }

        public byte[] FileBytes { get; set; }



        public short SR_Home_NextTab { get; set; }

        public string SessionID { get; set; }

        public bool IsSessionExpiredStatus { get; set; }
        public long DissolutionID { get; set; }
        public long AmendmentID { get; set; }

        public int SubParentMenuToBeActive { get; set; }


        // Added by Shubham Bhagat on 25-1-2019
        public bool IsAmendmentForEdit { get; set; }
        public String AmendmentApplyingPartyID { get; set; }
        public String AmendmentTypeForEditID { get; set; } //Amendment type radio buttonID

        #region 10-04-2019 For Level drop down by shubham bhagat 
        public int LevelID { get; set; } // to edit role we need old levelId for update
        #endregion

        // Added by shubham Bhagat on 20-04-2019 for landing page of department admin flag
        public bool IsLandingPageChanged { get; set; }


        public string SubParentMenuName { get; set; }

        //Added by Madhusoodan on 02/08/2021 for Data Entry Correction
        public int OrderID { get; set; }

        public long? DECDocumentNumber { get; set; }

        public int DECBookTypeID { get; set; }

        public int DECFinancialYear { get; set; }

        public int DECSROCode { get; set; }

        //Added by Madhusoodan on 05/08/2021
        public bool IsEditMode { get; set; }


        #endregion
    }
}

