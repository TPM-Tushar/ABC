#region File Header
/*
        * Project Id    :
        * Project Name  :   Kaveri
        * Name          :   IDProofValidation.cs        
        * Description   :   Validation attribute to define properties and method for ID proof Details.
        * Author        :   Akash Patil
        * Creation Date :   
 **/
#endregion

namespace CustomModels.CustomValidations
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    #endregion

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IDProofValidation : ValidationAttribute, IClientValidatable
    {
        #region Properties
        private string BaseValue;
        #endregion

        #region Constructor

        /// <summary>
        /// Contract to init baseValue
        /// </summary>
        /// <param name="value"></param>
        public IDProofValidation(string value)
        {
            BaseValue = value;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Format error message
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, BaseValue);
        }

        /// <summary>
        /// Validate model
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = validationContext.ObjectInstance as dynamic;


                if (model != null && model.IDProofID != 0)
                {


                    if (model.IsForIDProofForPartner)
                    {

                        if (!string.IsNullOrEmpty(model.EPIC))
                        {
                            if (ElectionIDValidationAlgorithm.ValidateElectionID(model.EPIC))
                                return ValidationResult.Success;
                            else
                                return new ValidationResult("Invalid EPIC number format.");
                        }
                    }


                    if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.PAN))
                    {
                        //added by m rafe on 24-12-19
                        if(string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid PAN number format.");


                        if (PANNumberValidationAlgorithm.ValidatePANNumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid PAN number format.");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.AadharNumber))
                    {
                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid aadhar number format.");



                        if (AadharNumberValidationAlgorithm.ValidateAadharNumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid aadhar number format.");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.Passport))
                    {

                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid passport number format.");

                        if (PassportNumberValidationAlgorithm.ValidatePassportNumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid passport number format.");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.VotersIdentityCard))
                    {

                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid voter ID number format.");



                        if (ElectionIDValidationAlgorithm.ValidateElectionID(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid voter ID number format.");
                        // return ValidationResult.Success;
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.MatriculationCertificate))
                    {
                        return ValidationResult.Success;
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.DrivingLicense))
                    {


                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid driving license number format.");


                        if (DrivingLicenceNumberValidationAlgorithm.ValidateDrivingLicenceNumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid driving license number format.");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.CreditCardStatement))
                    {


                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid credit card statement");


                        if (CreditCardNumberValidationAlgorithm.ValidateCreditCardNumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid credit card statement");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.BankPassBook))
                    {
                        return ValidationResult.Success;
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.DegreeofARecognisedEducationalInstitution))
                    {
                        return ValidationResult.Success;
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.PIO))
                    {


                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))
                            return new ValidationResult("Invalid PIO card number");


                        if (PIOValidationAlgorithm.ValidatePIONumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid PIO card number");
                    }
                    else if (model.IDProofID == Convert.ToInt16(Common.CommonEnum.IDProofTypes.OCI))
                    {

                        //added by m rafe on 24-12-19
                        if (string.IsNullOrEmpty(model.IDProofNumber))

                            return new ValidationResult("Invalid OCI card number");


                        if (OCIValidationAlgorithm.ValidateOCINumber(model.IDProofNumber))
                            return ValidationResult.Success;
                        else
                            return new ValidationResult("Invalid OCI card number");
                    }
                    else
                        return ValidationResult.Success;
                }
                else
                    return ValidationResult.Success;
            }
            catch
            {
                //return ValidationResult.Success;
                throw;
            }
        }

        /// <summary>
        /// Get client validation rules
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
            //    //This is the name of the method aaded to the jQuery validator method (must be lower case)
            //    ValidationType = "dddetailsvalidator"
            //};

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("value", BaseValue);
            rule.ValidationType = "dddetailsvalidator";
            yield return rule;
        }

        #endregion
    }

    /// <summary>
    /// PAN number validation algorithm
    /// </summary>
    public class PANNumberValidationAlgorithm
    {
        public static bool ValidatePANNumber(String num)
        {
            //var regex = @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$";

            //Please refere this site = https://en.wikipedia.org/wiki/Permanent_account_number

            var regex = @"^[(A-Z)]{3}[(P|C|H|F|A|T|B|L|J|G)]{1}[(A-Z)]{1}[\d]{4}[(A-Z)]{1}$";
            var match = Regex.Match(num, regex, RegexOptions.IgnoreCase);
            return match.Success;
        }
    }

    /// <summary>
    /// Passport number validation algorithm
    /// </summary>
    public class PassportNumberValidationAlgorithm
    {
        public static bool ValidatePassportNumber(String num)
        {
            //var regex = @"/^[A-PR-WY]{1}[0-9]\d\s?\d{6}[1-9]{1}$/ig";
            var regex = @"^[A-PR-WY]{1}[0-9]{6}[1-9]{1}$";
            var match = Regex.Match(num, regex, RegexOptions.IgnoreCase);
            return match.Success;
        }
    }

    /// <summary>
    /// Adhar number validation algorithm
    /// </summary>
    public class AadharNumberValidationAlgorithm
    {
        #region Init

        static int[,] d = new int[,]
                {
                        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                        {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
                        {2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
                        {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
                        {4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
                        {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
                        {6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
                        {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
                        {8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
                        {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
                };
        static int[,] p = new int[,]
                {
                        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                        {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
                        {5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
                        {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
                        {9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
                        {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
                        {2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
                        {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
                };
        static int[] inv = { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };

        #endregion

        #region Methods

        /// <summary>
        /// Validation aadhar number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool ValidateAadharNumber(String num)
        {

            //var matchNumberRegex = "^[a-zA-Z0-9]{12}$";
            var matchNumberRegex = "^[0-9]{12}$";
            var match = Regex.Match(num, matchNumberRegex, RegexOptions.IgnoreCase);
            int c = 0;
            if (match.Success)
            {

                int[] myArray = StringToReversedIntArray(num);
                for (int i = 0; i < myArray.Length; i++)
                {
                    c = d[c, p[(i % 8), myArray[i]]];
                }
                return (c == 0);
            }
            else
            {
                return (c > 0);
            }
        }

        /// <summary>
        /// String to reversed int array
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static int[] StringToReversedIntArray(String num)
        {
            int[] myArray = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                myArray[i] = int.Parse(num.Substring(i, 1));
            }
            myArray = Reverse(myArray);
            return myArray;
        }

        /// <summary>
        /// Reverse array
        /// </summary>
        /// <param name="myArray"></param>
        /// <returns></returns>
        private static int[] Reverse(int[] myArray)
        {
            int[] reversed = new int[myArray.Length];
            for (int i = 0; i < myArray.Length; i++)
            {
                reversed[i] = myArray[myArray.Length - (i + 1)];
            }
            return reversed;
        }

        #endregion
    }

    /// <summary>
    /// Driving licence number validation algorithm
    /// </summary>
    public class DrivingLicenceNumberValidationAlgorithm
    {
        #region Properties
        private const int MinYears = 16;
        private const int MaxYears = 90;
        #endregion

        #region Methods

        /// <summary>
        /// Validate is alpha string or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllAlpha(string test)
        {
            return !test.Any(ch => !Char.IsLetter(ch));
        }

        /// <summary>
        /// Validate is numeric or not 
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllNumeric(string test)
        {
            return !test.Any(ch => !Char.IsDigit(ch));
        }

        /// <summary>
        /// Validate is financial year or not
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static bool IsYear(string year)
        {
            DateTime birthyeardate;
            if (DateTime.TryParse(@"1/1/" + year, out birthyeardate))
            {
                int yearsold = DateTime.Now.Year - birthyeardate.Year;
                if (yearsold >= MinYears && yearsold <= MaxYears) return true;
            }
            return false;
        }

        /// <summary>
        /// Validate driving licence number
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ValidateDrivingLicenceNumber(string source)
        {
            if (source.Length != 15)
            {
                return false;
            }

            if (AllAlpha(source.Substring(0, 2)))
            {
                if (AllNumeric(source.Substring(2, 13)))
                {
                    //if (IsYear(source.Substring(4, 4)))
                    //{
                    //    if (AllNumeric(source.Substring(8)))
                    //    {
                    //        return true;
                    //    }
                    //}
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Credit card number validation algorithm
    /// </summary>
    public class CreditCardNumberValidationAlgorithm
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool ValidateCreditCardNumber(string creditCardNumber)
        {
            //// check whether input string is null or empty
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return false;
            }

            //// 1.	Starting with the check digit double the value of every other digit 
            //// 2.	If doubling of a number results in a two digits number, add up
            ///   the digits to get a single digit number. This will results in eight single digit numbers                    
            //// 3. Get the sum of the digits
            int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                            .Reverse()
                            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                            .Sum((e) => e / 10 + e % 10);
            //// If the final sum is divisible by 10, then the credit card number
            //   is valid. If it is not divisible by 10, the number is invalid.            
            return sumOfDigits % 10 == 0;
        }
        #endregion
    }

    /// <summary>
    /// Election ID validation algorithm
    /// </summary>
    public class ElectionIDValidationAlgorithm
    {
        #region Methods

        /// <summary>
        /// Validate is aplha or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllAlpha(string test)
        {
            return !test.Any(ch => !Char.IsLetter(ch));
        }

        /// <summary>
        /// Validate is numeric or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllNumeric(string test)
        {
            return !test.Any(ch => !Char.IsDigit(ch));
        }

        /// <summary>
        /// Validate election id
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ValidateElectionID(string source)
        {
            if (source.Length != 10)
            {
                return false;
            }
            if (AllAlpha(source.Substring(0, 3)))
            {
                if (AllNumeric(source.Substring(3, 7)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Relation name validation
    /// </summary>
    public class RelationNameValidation
    {
        #region Methods

        /// <summary>
        /// Validate relation name
        /// </summary>
        /// <param name="relationID"></param>
        /// <param name="relationName"></param>
        /// <returns></returns>
        public static bool ValidateRelationName(string relationID, string relationName)
        {
            if (string.IsNullOrEmpty(relationID) && string.IsNullOrEmpty(relationName))
            {
                return true;
            }
            else
            {
                if (relationID == "0")
                    return false;
                if (string.IsNullOrEmpty(relationName))
                    return false;
                return true;
            }
        }
        #endregion
    }

    /// <summary>
    /// Mobile number validation
    /// </summary>
    public class MobileNumberValidationAlgorithm
    {
        #region Methods

        /// <summary>
        /// Validate mobile no
        /// </summary>
        /// <param name="num"></param> 
        /// <returns></returns>
        public static bool ValidateMobileNo(string num)
        {
            //Please refer this link https://en.wikipedia.org/wiki/Mobile_telephone_numbering_in_India
            var regex = @"(\s*\s*)^[7-9]{1}[0-9]{9}$[^<>]*";
            var match = Regex.Match(num, regex, RegexOptions.IgnoreCase);
            return match.Success;
        }
        #endregion
    }

    /// <summary>
    /// </summary>
    public class MobileNumberValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = validationContext.ObjectInstance as dynamic;
                if (null == model || string.IsNullOrEmpty(model.Mobile))
                    return ValidationResult.Success;

                string mobileNo = Convert.ToString(model.Mobile);
                if (MobileNumberValidationAlgorithm.ValidateMobileNo(mobileNo))
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Mobile is invalid format.");
            }
            catch
            {
                return new ValidationResult("Mobile is invalid format.");
            }
        }
    }

    /// <summary>
    /// </summary>
    public class UIDNumberValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = validationContext.ObjectInstance as dynamic;
                if (null == model || string.IsNullOrEmpty(model.UID))
                    return ValidationResult.Success;

                string UIDNo = Convert.ToString(model.UID);
                if (AadharNumberValidationAlgorithm.ValidateAadharNumber(UIDNo))
                    return ValidationResult.Success;
                else
                    return new ValidationResult("UID is invalid format.");
            }
            catch
            {
                return new ValidationResult("UID is invalid format.");
            }
        }
    }


    public class CustomAttribute : ValidationAttribute
    {
        private readonly string _other;
        private readonly string _displayName;
        public CustomAttribute(string propertyName, string displayName)
        {
            _other = propertyName;
            _displayName = displayName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            string v = value as string;
            if (v == null)
                return ValidationResult.Success;

            else
            {
                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)value);
                if (mtch.Success)
                    return new ValidationResult(_displayName + " field should not contain (#,$,<,>) special character.", new string[] { _other });
                else
                {
                    return ValidationResult.Success;

                }
            }

        }

    }

    // Added By Shubham Bhagat on 29/11/2018
    /// <summary>
    /// PIO validation algorithm
    /// </summary>
    public class PIOValidationAlgorithm
    {
        #region Methods

        /// <summary>
        /// Validate is aplha or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllAlpha(string test)
        {
            return !test.Any(ch => !Char.IsLetter(ch));
        }

        /// <summary>
        /// Validate is numeric or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllNumeric(string test)
        {
            return !test.Any(ch => !Char.IsDigit(ch));
        }

        /// <summary>
        /// Validate election id
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ValidatePIONumber(string source)
        {
            // Added By Shubham Bhagat on 29/11/2018
            if (source.Length != 8)
            {
                return false;
            }
            if (AllAlpha(source.Substring(0, 1)))
            {
                if (AllNumeric(source.Substring(1, 7)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }


    // Added By Shubham Bhagat on 29/11/2018
    /// <summary>
    /// OCI validation algorithm
    /// </summary>
    public class OCIValidationAlgorithm
    {
        #region Methods

        /// <summary>
        /// Validate is aplha or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllAlpha(string test)
        {
            return !test.Any(ch => !Char.IsLetter(ch));
        }

        /// <summary>
        /// Validate is numeric or not
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool AllNumeric(string test)
        {
            return !test.Any(ch => !Char.IsDigit(ch));
        }

        /// <summary>
        /// Validate election id
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ValidateOCINumber(string source)
        {
            // Added By Shubham Bhagat on 29/11/2018
            if (source.Length != 7)
            {
                return false;
            }
            if (AllAlpha(source.Substring(0, 1)))
            {
                if (AllNumeric(source.Substring(1, 6)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }




}


