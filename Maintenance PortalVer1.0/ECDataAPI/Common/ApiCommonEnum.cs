using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Common
{
    public class ApiCommonEnum
    {


        public enum RoleDetails
        {
            SR = 1,
            OnlineUser = 2,
            DR = 3,
            //SystemAdmin = 4,
            TechnicalAdmin = 4,
            IGR = 5,
            DepartmentAdmin = 6,
            ApprovalAdmin = 7,
            DIGR=8,
            AIGRComp=9,
            AIGRAdmin=10,
            IGRStaff=11,
            SystemIntegrator = 14
        }


        public enum OfficeTypes   //from Kaveri.MAS_OfficeTypes
        {
            IGR = 1,
            DRO = 2,
            SRO = 3,
            Others = 4,
            LawDepartment = 5,

        }
        public enum MenuDetails
        {
            //Parent Menus
            PM_DocumentRegistration = 1,
            PM_MarriageRegistration = 4,
            PM_EncumbranceSearch = 27,
            PM_FirmRegistration = 57,
            PM_CertifiedCopy = 82,
            PM_Liability = 90,
            PM_UnlockDocuments = 101,
            PM_GenerateToken = 140,
            PM_DocumentScanning = 142,

            //Menus Sequence
            Sequence2 = 2,

            //Sub Menus
            Reports = 60, //Document Report
            DROrderEntry = 69, //DR Order Entry
        }
        //added by akash on(25-05-2018) to identify Kaveri MVC Client(CDAC). 
        public enum APiConsumers
        {
            CdacKaveriMVCClient = 1
        }

        public enum LevelDetails
        {
            // commented by shubham bhagat on 12-04-2019 due to requirement change
            //Admin = 1,
            //StateRegistrar = 2,
            //DistrictRegistrar = 3,
            //SubRegistrar = 4,
            //Online = 5,
            //HeadOffice=6,
            //Others=7

            State = 1,
            DR = 2,
            SR = 3,
            Others = 4

        }

        public enum Countries
        {
            India = 1  //From Kaveri.MAS_Countries
        }

        public enum States
        {
            GOA = 11  //From Kaveri.MAS_States
        }

        //public enum ParentMenuIdEnum
        //{
        //   // DocumentRegistration = 1,
        //   // MarriageRegistration = 4,
        //    UserManagement = 17,
        //    RoleMenu = 18,
        //    WorkFlow = 0,
        //    OfficeUserDetails = 22,
        //    EncumberanceSearch = 27,
        //    //Firm = 9,
        //    //FirmRegistration = 1,
        //   // CertifiedCopy = 82,
        //   // CCApplicationList = 187,
        //  //  GeneralReports = 102,
        //  //  Reports = 60,
        //   // AdditionalFeatures = 15,
        //    UserDetails = 25,
        //   // Dashboard = 28,
        //  //  UserDetails_Dashboard = 30,
        //    UserDetails_UserManager = 34
        //}

        //All working fine Commented by Shubham bhagat on 04-04-2019
        //public enum ParentMenuIdEnum
        //{
        //    // DocumentRegistration = 1,
        //    // MarriageRegistration = 4,
        //    UserManagement = 2,
        //    RoleMenu = 3,
        //    WorkFlow = 0,
        //    OfficeUserDetails = 7,
        //    EncumberanceSearch = 12,
        //    //Firm = 9,
        //    //FirmRegistration = 1,
        //    // CertifiedCopy = 82,
        //    // CCApplicationList = 187,
        //    //  GeneralReports = 102,
        //    //  Reports = 60,
        //    // AdditionalFeatures = 15,
        //    UserDetails = 10,
        //    // Dashboard = 28,
        //    //  UserDetails_Dashboard = 30,
        //    UserDetails_UserManager = 14
        //}

        public enum ParentMenuIdEnum
        {
            //// DocumentRegistration = 1,
            //// MarriageRegistration = 4,
            UserManagement = 2,
            RoleMenu = 35,
            WorkFlow = 0,
            OfficeUserDetails = 7,
            EncumberanceSearch = 12,
            ////Firm = 9,
            ////FirmRegistration = 1,
            //// CertifiedCopy = 82,
            //// CCApplicationList = 187,
            ////  GeneralReports = 102,
            ////  Reports = 60,
            //// AdditionalFeatures = 15,
            UserDetails = 10,
            //// Dashboard = 28,
            ////  UserDetails_Dashboard = 30,
            // UserDetails_UserManager = 14, <-commented on 5-4-2019 
            MenuManagement = 34,
            UpdateyourProfile = 38,
            UserManagementForDeptAdmin = 32


        }
        public enum FirmBusinessTypes
        {

            SoleProprietorship = 1,
            Partnership = 2,
            LimitedLiabilityPartnership = 3,
            PrivateLimitedCompany = 4,
            PublicLimitedCompany = 5

        }
        public enum ReferenceType
        {
            FinalRegistrationNumber = 1,
            SerialNumber = 2,
            OtherReferenceNumber = 3
        }

        public enum PaymentModes
        {
            ByCash = 1,
            ByChallan = 2,
            ByDD = 3,
            ByEStamp = 4,
            ByChequePayOrder = 5,
            ByBankersCheque = 6,
            ByEPayment = 7
        }
        public enum ServicesProcess
        {
            FirmRegistration = 1,
            FirmAmendment = 2,
            FirmDissolution = 3,
            RegularDocument = 4,
            Enclosures = 5,
            CertifiedCopies = 6

        }

        public enum FirmApplicationType
        {
            FirmRegistrationFilling = 1,
            FirmAmendmentFilling = 2,
            FirmDissolutionFilling = 3
        }

        public enum enumServiceTypes
        {
            FirmRegistration = 1,
            CertifiedCopies = 2,
            UserManager = 3,
            Scanning = 4,
            TokenGeneration = 5,
            Alert = 6



        }

        public enum MasterDropDownEnum
        {
            CountryDropDown = 1,
            StatesDropDown = 2,
            DistrictsDropDown = 3,
            TitleDropDown = 4,
            GenderDropDown = 5,
            ProfessionDropDown = 6,
            MaritalStatusDropDown = 7,
            NationalityDropDown = 8,
            IdProofsTypeDropDown = 9,
            RelationTypeDropDown = 10,
            FinancialYearDropDown = 11,
            BookTypeDropDown = 12
        }

        public enum Services
        {
            FR = 1,     //Firm Registration
            CC = 2,     //Certified Copies
            USR = 3,    //User Manager
            SCN = 4,    //Scanning
            TKN = 5,    //Token Generation
            ALM = 6     //Alert
        }

        public enum OfficeIdEnum  // from MAS_OfficeMaster
        {
            OnlinePortal = 18
        }

        public enum FeeRules
        {
            FirmRegistration = 1,
            FirmAmendment = 2,
            FirmDissolution = 3,
            ScanningFee = 4,
            CCRegistrationFee = 5,
            CCStampDuty = 6,
            MakingorGrantingCopy = 7,
            CopyingFee = 8
        }

        public enum ActionPerformedBySR  // Actions performed by SR (for validation) //by akash(08-05-18)
        {
            AcceptFirmApplication = 1,
            RegisterFirmApplication = 2
        }



        public enum DocumentStatusTypes
        {
            //OnlineDataEntryisinProgress = 1,
            //OnlineApplicationSubmittedforApproval = 2,
            //ApplicationApprovedforOnlinePayment = 3,
            //Registered = 4,
            //DigitallySigned = 5
            Pending = 1,
            OnlineDataEntryIsInProgress = 1,
            OnlineApplicationSubmittedforApproval = 2,
            ApplicationApprovedforOnlinePayment = 3,
            Registered = 4,
            ApprovedFirms = 5,
            Correction = 6,
            Rejected = 7,
            OnlinePaymentIsDone = 8,
            CC_OnlineDataEntryIsInProgress = 9,
            CC_ApplicationSubmitted = 10,
            CC_DigitallySigned = 11,
            CC_OnlinePaymentDone = 12,
            CC_ApplicationAccepted = 13,
            // Added by shubham on 31-1-2019
            FirmAmendmentFilinginprogress = 14,
            FirmDissolutionFilling = 19,//==OnlineDataEntryIsInProgress-1
            FirmDissolutioForwardedtoSR = 20,//OnlineApplicationSubmittedforApproval-2
            FirmDissolutionForCorrection = 21,// Correction
            FirmDissolutionAccepted = 22,// ApprovedFirms-5
            FirmDissolutionRejected = 23,//Rejected
            RegisteredDissolutionApplication = 24,//Registered-4
            PaymentDoneForDissolutionApplication = 25,//OnlinePaymentIsDone-4
            DissolutionAcceptedForPayment = 26,// ApplicationApprovedforOnlinePayment-3


            //added by chetan 28-1-2019
            /// FirmAmendmentFilinginprogress = 14, // Commented by Shubham on 31-1-2019 
            FirmAmendmentForwardedtoSR = 15,
            FirmAmendmentApplicationApprovedForPayment = 16,
            FirmAmended = 17,
            FirmAmendmentRejected = 18,
            OnlinePaymentDoneforAmendment = 28,
            //Amendment (Added by Akash)
            FirmAmendmentCertificateHasbeenIssued = 27,
            FirmAmendmentForCorrection = 29

        }

        public enum WorkFlowActions
        {
            ForwardByOnlineUser = 1,
            ForwardToOnlineUser = 2

        }

        public enum ScaningServicesProcess
        {
            FirmRegistration = 1,
            FirmAmendment = 2,
            FirmDissolution = 3,
            RegularDocument = 4,
            Enclosures = 5
        }


        public enum Modules
        {
            Home = 1,
            SavedApplications = 2,
            UserManager = 3,
            FirmRegistration = 4,
            CertifiedCopy = 5,
            EncumbranceSearch = 6,
            Dashboard = 7,
            SupportEnclosure = 11

        }

        public enum UploadDocumentTypes
        {
            ScannedDocument = 1,
            Receipt = 2
        }

        public enum PG_ErrorFlag
        {
            Y,
            N
        }
        public enum PG_DoubleVerificationTransStatus
        {
            S = 1,  //Success
            F = 2,  //Failure
            P = 3 //Pending
        }

        public enum Professions
        {

            Doctor = 1,
            Engineer = 2,
            Actor = 3,
            Sports = 4,
            GovtEmployee = 5,
            Lawyer = 6,
            Business = 7,
            Other = 8,
            Farmer = 9
        }

        public enum GoaDistricts
        {
            NorthGoa = 128,
            SouthGoa = 129,
            Other = 1043

        }


        public enum OnlineDataEntryStatus
        {
            FirmDetails = 1,
            BranchDetails = 2,
            SupportedDocument = 3,
            PartnerDetails = 4,
            Summary = 5
        }
        public enum FirmCertificateDescription
        {
            FirmRegistrationCertificate = 13

        }


        public enum FirstPageOfFirmRegistrationCertificate
        {
            FirstPage = 1,
            NumberOfPartnersOnFirstPage = 0
        }

        public enum OTPTypeId
        {
            MobileVerification = 1,
            ChangePassword = 2

        }

        public enum IsOTPSent
        {
            OTPYetToBeSend = 0,
            OTPAlreadySent = 1,
            OTPNotSent = 2,

        }

        public enum IdProofType
        {
            PAN = 1,
            VoterIDCard = 12
        }
        public enum AdminActivityType
        {
            RoleMenuDetails = 1,
            MenuActionDetails = 2,
            ControllerActionDetails = 3,
            OfficeDetail = 4,
            UserDetail = 5
        }


        // Added by Shubham Bhagat on 17-1-2019 and 18-1-2019
        public enum AmendmentType
        {
            ALLSubType = 0,
            AmendmentInFirmNameOrAddress = 1,
            AmendmentInBranchOpeningOrClosing = 2,
            AmendmentInPartnerNameOrAddress = 3,
            AmendmentInConstitutionChange = 4,
            AmendmentInMinorPartner = 5,
            AlterationInFirmName = 6,
            AlterationInPrinciplePlaceOfBusiness = 7,
            OpeningOfBranches = 8,
            ClosingOfBranches = 9,
            AlterationInNameOfPartner = 10,
            AlterationInAddressOfPartner = 11,
            OutgoingPartner = 12,
            JoiningOfPartner = 13,
            MinorPartnerAttainingMajority = 14
        }


        public enum UploadedDocumnetType //MAS_UploadDocumentTypes
        {
            ScannedDocument = 1,
            Receipt = 2,
            FirmCertificate = 3,
            FirmAmendmentCertificate = 4,
            FirmDissolutionCertificate = 5
        }

        //Added by Raman Kalegaonkar on 05-04-2019 For Remittance Diagnostic Details
        public enum TransactionStatus
        {
            All = 0,
            ReceiptsNotSubmittedForRemittance = 1,
            ReceiptsNotRemitted = 2,
            ChallanNotGenerated = 3,
            BankReconcealationPending = 4

        }

        //Added By Raman Kalegaonkar on 30-05-2019
        //For HighValueProperties in MIS Reports
        public enum RangeList
        {
            UptoTenLackhs = 1,
            TenLackhsToOneCrore = 2,
            OneCroreToFiveCrores =3,
            FiveCroresToTenCrores =4,
            MoreThanTenCrores=5

        }

        // ADDED BY SHUBHAM BHAGAT TO WRITE DEBUG LOG IN DATABASE ON 1-07-2019
        public enum FunctionalityMaster
        {
            Login = 1,
            LogAnalysis = 2,
            RemittanceDiagnostics = 3,
            MISReports = 4,
            UserManager = 5,
            ForgotPassword=6
        }
        //Added by Raman Kalegaonkar on 11-07-2019
        public enum ModuleNames   //
        {
            All = 0,
            DocumentReg = 1,
            MarrriageReg = 2,
            MarriageNotice =3,
            StampDuty=4,
            Others=5
        }

        //Added by Raman Kalegaonkar on 25-07-2019
        public enum propertyTypes   
        {
            All = 0,
            Agricultural = 1,
            NonAgricultural = 2,
            Flat = 3,
            Apartment = 4,
            Lease = 5

        }

        ////Added by Raman Kalegaonkar on 25-07-2019
        //public enum BuildTypes
        //{
        //    All = 0,
        //    Flat = 1,
        //    Apartment = 2,
        //    Other =3

        //}

        //Added by Raman Kalegaonkar on 30-07-2019
        public enum PropertyValues  //For Sale Deed
        {
            All = 0,
            UptoTenLakhs = 1,
            AboveTenLakhs = 2
        }

        // ADDED BY SHUBHAM BHAGAT ON 30-09-2019
        public enum ServicePackSoftwareReleaseType
        {
            EXE_DLL = 1,
            ServicePack = 2
        }

        public enum ReceiptType
        {
            Document = 1,
            Marriage = 5,
            Notice = 8,
            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021
            OtherReceipts = 9
            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021
        }
        public enum PaymentMode
        {
            Cash = 1,
            Challen = 2,
            DD = 3,
            ChequeDDPayOrder = 4,
            Others = 5
        }
        public enum DocumentType
        {
            Document = 1,
            Marriage = 2,
            Notice = 3,
            //Added by mayank on 13/09/2021 for Firm registered report
            Firm = 4,
        }

        #region ADDED BY SHUBHAM BHAGAT ON 30-06-2020
        public enum DB_RES_KeyType
        {
            Key = 1,
            OTP = 2
        }

        #endregion

        #region ADDED BY SHUBHAM BHAGAT ON 30-06-2020
        public enum DB_RES_STATUSMASTER
        {
            DatabaseRestored = 1,
            DataInsertedAndPendingForVerification = 2,
            ErrorInDataInsertion = 3,
            DataInsertedAndVerified = 4,
            ErrorInScriptGeneration = 5,
            DataRestorationInitiated = 6,
            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
            DataRestorationAborted = 7
            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	
        }

        public enum DB_RES_ACTIONSMASTER
        {
            KAIGR_REGDatabaseRestore = 1,
            DatabaseConsistencyCheck = 2,
            VerifyDatabaseOfficeCode = 3,
            CreateKAVERIUser = 4,
            EnableDatabaseAuditLog = 5,
            SendLastID = 6,
            GenerateScript = 7,
            ApproveScript = 8,
            ExecuteScript = 9,
            GenerateRecitifiedScript = 10,
            ApproveRecitifiedScript = 11,
            ExecuteRecitifiedScript = 12,
            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 27-11-2020
            SendDatabaseSchema=13
            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 27-11-2020

            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021
            ,
            KAIGR_VALDatabaseRestore = 14,
            MapKaveriUserToKAIGR_REG = 15,
            MapKaveriUserToKAIGR_VAL = 16
            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-01-2021



        }

        public enum DB_RES_SERVICE_COMM_DETAILS_MASTER
        {
            DataBaseRestored = '1',
            DataInsertUsingScript = '2'
        }




        #endregion

        #region ADDED BY PANKAJ SAKHARE ON 26-10-2020
        public enum DiagnosticActionMaster
        {
            CheckPrimaryFilegroup = 1,
            CheckCatalogConsistency = 2,
            CheckConstraintsIntegrity = 3,
            CheckDatabaseConsistency = 4,
            CheckAuditEventStarted = 5,
            CheckOptimizer1 = 6,
            CheckOptimizer2 = 7,
            LastFullBackupVerification = 8,
            LastDifferentialBackupVerification = 9,
            GetDataFileSize = 10,
            GetLogFileSize = 11,
            CheckDatabaseDiskSpace = 12,
            CheckCDDriveBackup = 13,
            CheckTimeZone = 14
        }
        #endregion


        #region Added by mayank on 05-07-2021 for FRUITS report
        public enum FruitsResponseCodes
        {

            //following code are refered in FRUITS_DATA_RECV_DETAILS column name 
            Filed = 1,
            Registered = 2,
            Withdrawn = 3,
            Rejected = 4,
            NotImported = 5,
            Other = 6,
            Accepted = 7,
            Pending = 8,
            AutoFiled = 9,
            //Added By ShivamB on 30-09-2022 for DocumentStatusCode == 99
            SystemRejected = 99
            //Added By ShivamB on 30-09-2022 for DocumentStatusCode == 99
        };
        #endregion

        #region Added by mayank on 20/05/2022 for Diagnostic Details
        public enum DiagnosticsTile
        {
            AllOffice = 0,
            AllCheckSuccessful = 1,
            IssueFound = 2,
            StatusNotAvailabel = 3,
            StatusAvailabel = 4,
        }
        #endregion
       
        #region Added by Vijay on 11-01-2023
        public enum TableType
        {
            villageMaster = 1,
            HobliMaster = 2,
            Bhoomi = 3,
            DistrictMaster = 4,
            VillageMasterVillagesMergingMapping = 5,
            SROMaster = 6,
            MAS_OfficeMaster=7,
            MAS_Villages=8,
            MAS_Hoblis=9,


        }
        #endregion

        #region Added by Vijay on 11-01-2023

        public enum DBType
        {
            ECData = 1,
            KAIGR_ONLINE = 2,
        }
        #endregion


    }
}