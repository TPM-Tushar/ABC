using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataUI.Common
{
    public class CommonEnum
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
            DIGR = 8,
            AIGRComp = 9,
            AIGRAdmin = 10,
            IGRStaff = 11,
            CDACSupportTeam = 12,
            SystemIntegrator = 14
        }

        public enum LevelDetails
        {
            //Admin = 1,
            //StateRegistrar = 2,
            //DistrictRegistrar = 3,
            //SubRegistrar = 4,
            //Online = 5
            State = 1,
            DR = 2,
            SR = 3,
            Others = 4
        }

        public enum Countries
        {
            India = 1  //From Kaveri.MAS_Countries
        }

        public enum OfficeTypes   //from Kaveri.MAS_OfficeTypes
        {
            StateRegistrarOffice = 1,
            DRO = 2,
            SRO = 3,
            Internet = 4,
            LawDepartment = 5
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

        public enum FirmApplicationType
        {
            FirmRegistrationFilling = 1,
            FirmAmendmentFilling = 2,
            FirmDissolutionFilling = 3
        }

        public enum enumServiceTypes
        {
            FirmRegistration = 6,
            CertifiedCopies = 2,
            UserManager = 3,
            Scanning = 4,
            TokenGeneration = 5,
            Alert = 1
        }

        //public enum DocumentStatusTypes
        //{
        //    // Pending = 1,
        //    OnlineDataEntryIsInProgress = 1,
        //    OnlineApplicationSubmittedforApproval = 2,
        //    ApplicationApprovedforOnlinePayment = 3,
        //    Registered = 4,
        //    ApprovedFirms = 5,
        //    // DigitallySigned = 5,
        //    Correction = 6,
        //    Rejected = 7,
        //    OnlinePaymentIsDone = 8,
        //    CC_OnlineDataEntryIsInProgress = 9,
        //    CC_ApplicationSubmitted = 10,
        //    CC_DigitallySigned = 11,
        //    CC_OnlinePaymentDone = 12,
        //    CC_ApplicationAccepted = 13,

        //    FirmDissolutionFilling = 19,//==OnlineDataEntryIsInProgress-1
        //    FirmDissolutioForwardedtoSR = 20,//OnlineApplicationSubmittedforApproval-2
        //    FirmDissolutionForCorrection = 21,// Correction
        //    FirmDissolutionAccepted = 22,// ApprovedFirms-5
        //    FirmDissolutionRejected = 23,//Rejected
        //    RegisteredDissolutionApplication = 24,//Registered-4
        //    PaymentDoneForDissolutionApplication = 25,//OnlinePaymentIsDone-4
        //    DissolutionAcceptedForPayment = 26,// ApplicationApprovedforOnlinePayment-3

        //    //added by chetan 28-1-2019
        //    FirmAmendmentFilinginprogress = 14,
        //    FirmAmendmentForwardedtoSR = 15,
        //    FirmAmendmentApplicationApprovedForPayment = 16,
        //    FirmAmended = 17,
        //    FirmAmendmentRejected = 18,
        //    OnlinePaymentDoneforAmendment = 28,
        //    //Amendment (Added by Akash)
        //    FirmAmendmentCertificateHasbeenIssued = 27,
        //    FirmAmendmentForCorrection = 29
        //}



        public enum IDProofTypes
        {
            PAN = 1,
            Passport = 2,
            DrivingLicense = 3,
            BankPassBook = 4,
            SchoolLeavingCertificate = 5,
            MatriculationCertificate = 6,
            DegreeofARecognisedEducationalInstitution = 7,
            CreditCardStatement = 8,
            WaterBill = 9,
            RationCard = 10,
            PropertyTaxAssessmentOrder = 11,
            VotersIdentityCard = 12,
            AadharNumber = 13,
            Other = 14
        }

        public enum ScaningServicesProcess
        {
            FirmRegistration = 1,
            FirmAmendment = 2,
            FirmDissolution = 3,
            RegularDocument = 4,
            Enclosures = 5
        }

        //public enum MenuDetails
        //{
        //    //Parent Menus
        //    PM_DocumentRegistration = 1,
        //    PM_MarriageRegistration = 4,
        //    PM_EncumbranceSearch = 27,
        //    PM_FirmRegistration = 57,
        //    PM_CertifiedCopy = 82,
        //    PM_Liability = 90,
        //    PM_UnlockDocuments = 101,
        //    PM_GenerateToken = 140,
        //    PM_DocumentScanning = 142,

        //    //Menus Sequence
        //    Sequence2 = 2,

        //    //Sub Menus
        //    Reports = 60, //Document Report
        //    DROrderEntry = 69, //DR Order Entry
        //}

        //public enum ParentMenuIdEnum
        //{
        //    //DocumentRegistration = 1,
        //    //MarriageRegistration = 4,
        //    UserManagement=17,
        //    RoleMenu = 18,
        //    WorkFlow = 0,
        //    OfficeUserDetails = 22,
        //    EncumberanceSearch = 27,//cHANGEpASSWORD
        //    //Firm= 9,
        //    //FirmRegistration=1,
        //    //CertifiedCopy = 82,
        //    //CCApplicationList=187,
        //    //GeneralReports = 102,
        //    //Reports = 60,
        //   // AdditionalFeatures=15,
        //    UserDetails=25,
        //   // Dashboard=28,
        //   // UserDetails_Dashboard=30,
        //    UserDetails_UserManager=34,
        //   // Amendment=37,
        //   // Dissolution=38


        //}



        //All working fine Commented by Shubham bhagat on 04-04-2019
        //public enum ParentMenuIdEnum
        //{
        //    //DocumentRegistration = 1,
        //    //MarriageRegistration = 4,
        //    UserManagement = 2,
        //    RoleMenu = 3,
        //    WorkFlow = 0,
        //    OfficeUserDetails = 7,
        //    EncumberanceSearch = 12,//cHANGEpASSWORD
        //    //Firm= 9,
        //    //FirmRegistration=1,
        //    //CertifiedCopy = 82,
        //    //CCApplicationList=187,
        //    //GeneralReports = 102,
        //    //Reports = 60,
        //    // AdditionalFeatures=15,
        //    UserDetails = 10,
        //    // Dashboard=28,
        //    // UserDetails_Dashboard=30,
        //    UserDetails_UserManager = 14,
        //    // Amendment=37,
        //    // Dissolution=38


        //}

        public enum ParentMenuIdEnum
        {
            ////DocumentRegistration = 1,
            ////MarriageRegistration = 4,
            UserManagement = 2,
            RoleMenu = 3,
            WorkFlow = 0,
            OfficeUserDetails = 7,
            EncumberanceSearch = 12,//cHANGEpASSWORD
            ////Firm= 9,
            ////FirmRegistration=1,
            ////CertifiedCopy = 82,
            ////CCApplicationList=187,
            ////GeneralReports = 102,
            ////Reports = 60,
            //// AdditionalFeatures=15,
            UserDetails = 10,
            //// Dashboard=28,
            //// UserDetails_Dashboard=30,
            // UserDetails_UserManager = 14, <-commented on 5-4-2019
            //// Amendment=37,
            //// Dissolution=38
            MenuManagement = 34,
            UpdateyourProfile = 38,
            UserManagementForDeptAdmin = 32,
            UserManager = 43,
            RoleMenuMapping = 35,
            LogAnalysis = 17,
            MISReports = 25,
            RemittanceDiagnostics = 19,
            SupportEnclosure = 72,
            XELFiles = 74
        }


        public enum Modules
        {
            Home = 1,
            SavedApplications = 2,
            UserRegistration = 3,
            FirmRegistration = 4,
            CertifiedCopy = 5,
            EncumbranceSearch = 6

        }

        public enum UploadDocumentTypes
        {
            ScannedDocument = 1,
            Receipt = 2

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

        public enum OnlineDataEntryStatus
        {
            FirmDetails = 1,
            BranchDetails = 2,
            SupportedDocument = 3,
            PartnerDetails = 4,
            Summary = 5
        }


        public enum SR_Home_NextTab
        {
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
            FirmDissolutionFilling = 19,//==OnlineDataEntryIsInProgress-1
            FirmDissolutionForwardedtoSR = 20,//OnlineApplicationSubmittedforApproval-2
            FirmDissolutionForCorrection = 21,// Correction
            FirmDissolutionAccepted = 22,// ApprovedFirms-5
            FirmDissolutionRejected = 23,//Rejected
            RegisteredDissolutionApplication = 24,//Registered-4
            PaymentDoneForDissolutionApplication = 25,//OnlinePaymentIsDone-4
            DissolutionAcceptedForPayment = 26,// ApplicationApprovedforOnlinePayment-3


            //added by chetan 28-1-2019
            FirmAmendmentFilinginprogress = 14,
            FirmAmendmentForwardedtoSR = 15,
            FirmAmendmentApplicationApprovedForPayment = 16,
            FirmAmended = 17,
            FirmAmendmentRejected = 18,
            OnlinePaymentDoneforAmendment = 28,
            //Amendment (Added by Akash)
            FirmAmendmentCertificateHasbeenIssued = 27,
            FirmAmendmentForCorrection = 29

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
        public enum AdminActivityType
        {
            RoleMenuDetails = 1,
            MenuActionDetails = 2,
            ControllerActionDetails = 3,
            OfficeDetail = 4,
            UserDetail = 5
        }

        public enum SubParentMenuToBeActive
        {
            // Commented  below lines by Shubham bhagat on 6-4-2019
            //Registration = 1,
            //Amendment = 37,
            //Dissolution = 38,
            //RoleMenuActionDetails = 3,
            //OfficeUserDetails = 7,
            //UserProfile = 15,
            //ChangePassword = 16,
            // Commented  above lines by Shubham bhagat on 6-4-2019
            EcAuditDetails = 18,
            DiagnosticDetails = 20,
            DiagnosticSummary = 31,
            UserDetail = 33,
            RoleMenuMapping = 35,
            OfficeDetail = 37,
            UpdateProfile = 39,
            ChangePassword = 40,
            MenuActionDetails = 41,
            ControllerActionDetails = 42,
            IntegrationCallExceptions = 44,
            ErrorLogFiles = 51,
            IndexIIReports = 29,
            RemittanceXMLLog = 54,
            ChallanMatrixXMLLog = 55,
            DoubleVeriXMLLog = 56,
            TotalDocumentsRegistered = 53,
            HighValueProperties = 58,
            DailyRevenueArticleWise = 59,
            SRODocumentsCashCollectionDetails = 61,
            SaleDeedRegisteredandRevenueCollected = 62,
            SRODD_POCollection = 60,
            MISReports = 25,

            KaveriSupport = 45,
            TicketRegistration = 46,
            DecryptEnclosure = 47,
            UploadPatchFile = 48,
            ServicePackStatus = 67,
            DailyReceiptsDetails = 68,
            SurchargeandcessDetails = 69,
            JurisdictionalWiseReport = 70,
            ReScanningDetails = 71,
            SupportEnclosure = 72,
            DownloadEnclosure = 73,
            ExemptedDocument = 77,
            ECDailyReceiptDetails = 78,
            ScannedFileUploadStatus = 80,
            AnyWhereECLog = 79,
            DocumentCentralizationStatus=83,
            CourtOrderDetails=84,
                  // Added by shubham bhagat on 30-09-2019
            ServicePacksForCDAC_SYSIntegrator = 49,
            AddServicePackDetails = 50,
            ServicePackDetails = 57,
            ServicePacks = 63,
            ServicePacksDetails = 64,
            ReleasedServicePacks = 65,
            ScannedFileDownload = 87,
            XELFiles = 74,
            AuditSpecificationDetails = 76,
            AnywhereRegistrationStatistics =81,
            XELLog=90,
            KaveriIntegrationReport = 89,
            PendingDocumentsSummary = 92,
            DataTransmissionReport=93,
            SROScriptManager = 94,
            InsertScriptManagerDetails = 95,
            ScriptManagerDetails = 96,
            AppVersionDetails = 97,
            DROScriptsDetails = 102,
            CDWrittenReport =99,
            BhoomiFileUploadReport=101,
            JSlipUploadReport=103, DocumentReferences=106, DocumentScanandDelivery=98,
            SAKALAUploadAndPendencyReport=105,
            OtherDepartmentImport = 108,
            DiskUtilizationReport = 109,
            Dashboard = 27,
            PropertyWithoutImportBypassRDPR = 110,
            ValuationAnalysis = 119

        }

        // Added by Shubham Bhagat on 17-1-2019 and 18-1-2019
        public enum AmendmentType
        {
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

        // ADDED BY SHUBHAM BHAGAT TO WRITE DEBUG LOG IN DATABASE ON 1-07-2019
        public enum FunctionalityMaster
        {
            Login = 1,
            LogAnalysis = 2,
            RemittanceDiagnostics = 3,
            MISReports = 4,
            UserManager = 5,
            ForgotPassword = 6
        }

        //Added by Raman Kalegaonkar on 11-07-2019
        public enum ModuleNames   //
        {
            All = 0,
            DocumentReg = 1,
            MarrriageReg = 2,
            MarriageNotice = 3,
            StampDuty = 4,
            Others = 5
        }
    }
}