using System;

namespace StreamWork.DataModels {
    public class PayPalNotification {
        public string Txn_Id { get; set; }
        public string Subcr_Id { get; set; }
        public string Txn_Type { get; set; }

        public uint Item_Number { get; set; }
        public string Item_Name { get; set; }
        public string Payment_Status { get; set; }
        public string Payment_Type { get; set; }
        public DateTime Payment_Date { get; set; }
        public uint Quantity { get; set; }
        public string Mc_Currency { get; set; }
        public decimal Payment_Gross { get; set; }
        public decimal Payment_Fee { get; set; }
        public decimal Mc_Gross { get; set; }
        public decimal Mc_Fee { get; set; }
        public decimal Shipping { get; set; }
        public decimal Handling_Amount { get; set; }
        public decimal Tax { get; set; }

        public string Payer_Id { get; set; }
        public string Payer_Status { get; set; }
        public string Payer_Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }

        public string Address_Status { get; set; }
        public string Address_Name { get; set; }
        public string Address_Country { get; set; }
        public string Address_Country_Code { get; set; }
        public string Address_State { get; set; }
        public string Address_City { get; set; }
        public string Address_Street { get; set; }
        public uint Address_Zip { get; set; }

        public string Receiver_Id { get; set; }
        public string Receiver_Email { get; set; }
        public string Residence_Country { get; set; }

        public string Transaction_Subject { get; set; }
        public string Protection_Eligibility { get; set; }
        public string Charset { get; set; }
        public decimal Notify_Version { get; set; }
        public string Verify_Sign { get; set; }
        public bool Test_Ipn { get; set; }
        public string Custom { get; set; } // Username OR StudentUserName+TutorUserName
    }
}
