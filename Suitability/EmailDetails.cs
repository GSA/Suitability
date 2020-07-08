using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Suitability
{
    class EmailDetails
    {
        private MySqlCommand cmd = new MySqlCommand();
        /*
        public string Pers_FullName { get; set; }
        public string Pers_SubjectName { get; set; }
        public string Pers_Position { get; set; }
        public string Pers_Region { get; set; }
        public string Pers_MajorOrg { get; set; }
        public string Pers_IsCitizen { get; set; }
        public string Pers_InvestigationDate { get; set; }
        public string Pers_InvestigationRequested { get; set; }
        public string Pers_InvestigationType { get; set; }
        public string Pers_SponsorDate { get; set; }
        public string Pers_PortofEntry { get; set; }
        public string boolTemplate { get; set; }
        public string boolAttachement { get; set; }
        public string boolSummary { get; set; }
        public string RegionalEMail { get; set; }
        public string SponsorshipEmails { get; set; }
        public string GSAPOCEMails { get; set; }
        public string VendorPOCEmails { get; set; }
        public string FASEMail { get; set; }
        public string ChildcareEMail { get; set; }
        public string GSAPOCNames { get; set; }
        public string PhoneNumber { get; set; }
        public string ZoneLetter { get; set; }
    */
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailFromAdd { get; set; }
        public string EmailToAdd { get; set; }
        public string EmailCCAdd { get; set; }
        public string EmailBCCAdd { get; set; }
        public string EmailAttachment { get; set; }

        private static EmailDetails GetEmailDetails(IDataRecord record)
        {
            try
            {
                var emailData = new EmailDetails
                {
                    /*
                    Pers_FullName = record["fullname"].ToString(),
                    Pers_SubjectName = record["subjectname"].ToString(),
                    Pers_Position = record["position"].ToString(),
                    Pers_Region = record["region"].ToString(),
                    Pers_MajorOrg = record["majororg"].ToString(),
                    Pers_IsCitizen = record["iscitizen"].ToString(),
                    Pers_InvestigationDate = record["investigationdate"].ToString(),
                    Pers_InvestigationRequested = record["investigationrequested"].ToString(),
                    Pers_InvestigationType = record["investigationtype"].ToString(),
                    Pers_SponsorDate = record["sponsoreddate"].ToString(),
                    Pers_PortofEntry = record["portofentry"].ToString(),
                    boolTemplate = record["Templates"].ToString(),
                    boolAttachement = record["Attachments"].ToString(),
                    boolSummary = record["Summary"].ToString(),
                    GSAPOCEMails = record["gsaPOCEMails"].ToString(),
                    VendorPOCEmails = record["vendorPOCEmails"].ToString(),
                    FASEMail = record["fasEMail"].ToString(),
                    ChildcareEMail = record["childcareEMail"].ToString(),
                    RegionalEMail = record["zone_email"].ToString(),
                    SponsorshipEmails = record["sponsorshipEmails"].ToString(),
                    PhoneNumber = record["zone_phone"].ToString()
                     */

                    EmailBody = record["eMailBody"].ToString(),
                    EmailSubject = record["eMailSubject"].ToString(),
                    EmailFromAdd = record["eMailFromAdd"].ToString(),
                    EmailToAdd = record["eMailToAdd"].ToString(),
                    EmailCCAdd = record["eMailCCAdd"].ToString(),
                    EmailBCCAdd = record["eMailBCCAdd"].ToString(),
                    EmailAttachment = record["eMailAttachment"].ToString()

                };
                return emailData;
                //return null;

            }
            catch (Exception)
            {
                //Log exception
                throw;
            }
        }


        public EmailDetails GetEmailDetails(string AppCode, int PersID, MySqlConnection conn, string ContractNumber=null)
        {
            try
            {
                using (conn)
                {
                    //Open connection if not open
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("AppCode", AppCode);
                        cmd.Parameters.AddWithValue("PersonID", PersID);
                        cmd.Parameters.AddWithValue("ContractNumber", ContractNumber);

                        cmd.CommandText = "HSPD12Email_Main";
                        cmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataReader emailData = cmd.ExecuteReader();

                        //Return details depending on type
                        while (emailData.Read())
                        {
                            return EmailDetails.GetEmailDetails(emailData);
                        }

                        return null;
                    }
                }
            }
            catch (MySqlException)
            {
                //Log exception
                throw;
            }
        }
    }


}
