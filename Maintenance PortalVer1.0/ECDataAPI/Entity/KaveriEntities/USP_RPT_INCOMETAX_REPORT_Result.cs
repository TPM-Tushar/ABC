//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.KaveriEntities
{
    using System;
    
    public partial class USP_RPT_INCOMETAX_REPORT_Result
    {
        public Nullable<long> REPORT_SNO { get; set; }
        public Nullable<long> ORIGINAL_REPORT_SNO { get; set; }
        public long CUSTOMERID { get; set; }
        public string PERSON_NAME { get; set; }
        public string DOB { get; set; }
        public string FATHERS_NAME { get; set; }
        public string PAN_ACK_NO { get; set; }
        public string AADHAR_NO { get; set; }
        public string IDENTIFICATION_TYPE { get; set; }
        public string Identification_Number { get; set; }
        public string FLAT_DOOR_BUILDING { get; set; }
        public string NAME_OF_PREM { get; set; }
        public string ROAD_STREET { get; set; }
        public string AREA_LOCALITY { get; set; }
        public string CITY_TOWN { get; set; }
        public string POSTAL_CODE { get; set; }
        public string STATE_CODE { get; set; }
        public int COUNTRY_CODE { get; set; }
        public string MOBILE_NO { get; set; }
        public string STD_CODE { get; set; }
        public string TELEPHONE_NO { get; set; }
        public int AGRI_INCOME { get; set; }
        public int NON_AGRI_INCOME { get; set; }
        public string REMARKS { get; set; }
        public string FORM60_ACK_NO { get; set; }
        public Nullable<System.DateTime> TXN_DATE { get; set; }
        public string TXN_ID { get; set; }
        public string TXN_TYPE { get; set; }
        public Nullable<decimal> TXN_AMOUNT { get; set; }
        public string TXN_MODE { get; set; }
    }
}