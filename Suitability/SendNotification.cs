using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Suitability
{

    public class SendNotification
    {
        //Create connection reference
        private MySqlConnection conn = new MySqlConnection();

        //Create message object
        private EMail message = new EMail();

        //Declare class variables and default values
        private string defaultEMail = "hspd12.security@gsa.gov";
        private int personID = 0;
        private int ContractNumber = 0;
        private string connectionString = string.Empty;
        private string smtpServer = "192.168.1.1";
        //private string emailTemplatesLocation = string.Empty;
        //private string regionalXMLLocation = string.Empty;
        private string onboardingLocation = string.Empty;

        /// <summary>
        /// Constructor for adjudication e-mails
        /// </summary>
        /// <param name="defaultEMail"></param>
        /// <param name="personID"></param>
        /// <param name="connectionString"></param>
        /// <param name="smtpServer"></param>
        /// <param name="emailTemplatesLocation"></param>
        public SendNotification(string defaultEMail, int personID, string connectionString, string smtpServer, string onboardingLocation, int ContractNumber = 0)
        {
            this.defaultEMail = defaultEMail;
            this.personID = personID;
            this.ContractNumber = ContractNumber;
            this.connectionString = connectionString;
            this.smtpServer = smtpServer;
            //this.emailTemplatesLocation = emailTemplatesLocation;
            //this.regionalXMLLocation = onboardingLocation;
            this.onboardingLocation = onboardingLocation;
            conn.ConnectionString = connectionString;
        }

        /// <summary>
        /// Returns if major org is q
        /// </summary>
        /// <param name="majorOrg"></param>
        /// <returns></returns>
        private bool IncludeFASEMail(string majorOrg)
        {
            //this can be simplified
            //return (majorOrg == "q");
            if (majorOrg == "q") return true;

            return false;
        }

        /// <summary>
        /// Returns if tier 1 c
        /// </summary>
        /// <param name="investType"></param>
        /// <param name="investTypeRequested"></param>
        /// <returns></returns>
        private bool IncludeChildCareEMail(string investType, string investTypeRequested)
        {
            //This can be simplified
            //return ((investType == "tier 1c") || (investTypeRequested =="tier 1c"));
            if ((investType == "tier 1c") || (investTypeRequested == "tier 1c")) return true;

            return false;
        }

        /// <summary>
        /// Sends adjudication e-mails
        /// Original method: Process data here.
        /// </summary>
        public void SendAdjudicationNotificationV1()
        {
            //Declare function variables
            //string emails = string.Empty;
            string body = string.Empty;
            string subject = string.Empty;
            string gsaPOCEMails = string.Empty;
            string vendorPOCEmails = string.Empty;
            string regionalEMails = string.Empty;
            string fasEMail = string.Empty;
            string childcareEMail = string.Empty;
            string gsaPOCNames = string.Empty;
            string investType = string.Empty;

            Person personData = new Person(conn); //connectionString);
            Contracts contractData = new Contracts(conn); //connectionString);

            PersonDetails personInfo = new PersonDetails();
            ContractDetails contractInfo = new ContractDetails();
            Regions regionalData = new Regions();

            //Get info from db
            personInfo = personData.GetPersonDetails(personID, "A");
            gsaPOCEMails = contractData.GetPOCEmails(personID, "AdjudicationGovEMails");
            vendorPOCEmails = contractData.GetPOCEmails(personID, "AdjudicationVendorEMails");
            gsaPOCNames = contractData.GetGSAPOCNames(personID);
            contractInfo = contractData.GetContractDetails(personID);

            //If contract info is null then create a new empty one, sets the defaults to string.empty
            if (contractInfo == null)
                contractInfo = new ContractDetails();

            //If not CO or NCR then use regional email xml
            if (personInfo.Region != "CO" && personInfo.Region != "NCR")
                regionalEMails = regionalData.GetRegionalEMails(personInfo.Region, onboardingLocation + @"\RegionalEMails.xml");

            //if not unfavorable and not a citizen check POE < 3 years and assign investigation type to 3Years
            if (personInfo.InvestigationType.ToLower() != "unfavorable")
            {
                if (!personInfo.IsCitizen)
                {
                    DateTime poeDate;
                    DateTime investigationDate = (DateTime)personInfo.InvestigationDate;

                    if (DateTime.TryParse(personInfo.PortOfEntryDate, out poeDate))
                    {
                        //This should also handle leap years
                        if (((investigationDate - poeDate).TotalDays / 365.2425) <= 3)
                        {
                            personInfo.InvestigationType = "3Years";
                        }
                    }
                }
            }

            //Switch on investigation type to set email body
            switch (personInfo.InvestigationType.ToLower())
            {
                case "naci":
                case "anaci":
                case "nacic":
                case "mbi":
                case "lbi":
                case "sbi":
                case "pri":
                case "bi":
                case "ssbi":
                case "ssbi-pr":
                case "ppr":
                case "tier 1":
                case "tier 1c":
                case "tier 2":
                case "tier 2s":
                case "tier 2rs":
                case "tier 4":
                case "tier 4r":
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Final Fit Determination";

                    body = File.ReadAllText(onboardingLocation + @"/GSA Final Fit.html");

                    break;
                case "fingerprint": //Initial
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Enter on Duty Determination";

                    body = File.ReadAllText(onboardingLocation + @"/GSA EOD Fit.html");

                    break;
                case "sac":
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Special Agreement Check (SAC) Determination";

                    body = File.ReadAllText(onboardingLocation + @"/GSA SAC Fit.html");

                    break;
                case "waiting for final":
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Will Need to Wait for Final Fitness Determination";

                    body = File.ReadAllText(onboardingLocation + @"/GSA Waiting.html");

                    break;
                case "unfavorable":
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Unfit Determination";

                    body = File.ReadAllText(onboardingLocation + @"/GSA Unfit.html");

                    break;
                case "3years":
                    subject = personInfo.SubjectName + " - " + "GSA Contractor Enter on Duty Determination (Less Than 3 Years)";

                    body = File.ReadAllText(onboardingLocation + @"/GSA Less Than 3 Years.html");

                    break;
                default:
                    //TODO: Need to handle this
                    return;
            }

            //Replace Name
            body = body.Replace("[NAME]", personInfo.FullName);

            //Replace Position
            body = body.Replace("[POSITION]", personInfo.Position);

            //Replace GSA POC Names
            body = body.Replace("[GSAPOCNAMES]", gsaPOCNames);

            //Replace contract Info
            body = body.Replace("[CONTRACTORCOMPANY]", contractInfo.ContractorCompany);
            body = body.Replace("[CONTRACTNUMBER]", contractInfo.ContractNumber);

            //Replace Investigation Type
            body = body.Replace("[INVESTIGATIONTYPE]", personInfo.InvestigationType);

            //Replace Investigation Date
            body = body.Replace("[INVESTIGATIONDATE]", personInfo.InvestigationDate.Value.ToString("MMMM dd, yyyy"));

            //If GSA POC, Vendor POC and Home emails are empty, then add hspd12.security@gsa.gov to the To email line
            if (string.IsNullOrEmpty(gsaPOCEMails) && string.IsNullOrEmpty(vendorPOCEmails) && string.IsNullOrEmpty(personInfo.HomeEMail))
            {
                message.Send(defaultEMail, defaultEMail, regionalEMails, defaultEMail, subject, body, "", smtpServer, true);
                return;
            }

            //if fas
            if (IncludeFASEMail(personInfo.MajorOrg.ToLower()))
                fasEMail = ConfigurationManager.AppSettings["FASEMAIL"].ToString();

            //if child care
            if (IncludeChildCareEMail(personInfo.InvestigationType.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                childcareEMail = ConfigurationManager.AppSettings["CHILDCAREEMAIL"].ToString();

            //Assemble email address variables into an array
            string[] emailsToJoin = { vendorPOCEmails, personInfo.HomeEMail, regionalEMails, fasEMail, childcareEMail };

            //Join the variables into a string and assign to a new variable
            var emails = string.Join(",", emailsToJoin.Where(s => !string.IsNullOrEmpty(s))).TrimEnd(',');

            //If child care call t1c function
            if (IncludeChildCareEMail(personInfo.InvestigationType.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                SendT1CAdjudication(gsaPOCEMails, emails, subject, body, "", regionalEMails);
            else
                message.Send(defaultEMail, gsaPOCEMails, emails, defaultEMail, subject, body, "", smtpServer, true);

            return;
        }

        /// <summary>
        /// Send sponsorship emails
        /// /// Original method: Process data here.
        /// </summary>
        public void SendSponsorshipNotificationV1()
        {
            //Declare string builder to build email attachments list
            StringBuilder emailAttachments = new StringBuilder();

            //Declare function variables
            string body = string.Empty;
            string subject = string.Empty;
            string emails = string.Empty;
            string regionalEMail = string.Empty;
            string sponsorshipEmails = string.Empty;
            string investType = string.Empty;
            string phoneNumber = string.Empty;

            //Instantiate objects
            Person personData = new Person(conn);
            PersonDetails personInfo = new PersonDetails();
            Contracts contracts = new Contracts(conn);
            Regions regionalData = new Regions();
            Attachments attachments = new Attachments();

            //Set up db connection
            conn = new MySqlConnection();
            conn.ConnectionString = connectionString;

            //Get data from db
            personInfo = personData.GetPersonDetails(personID, "S");
            sponsorshipEmails = contracts.GetSponsorshipEMails(personID);
            regionalEMail = regionalData.GetRegionalEMails(personInfo.Region, onboardingLocation + @"\RegionalEMails.xml");
            phoneNumber = regionalData.GetRegionalPhoneNumbers(personInfo.Region, onboardingLocation + @"\RegionalPhoneNumbers.xml");

            //Set default subject line
            subject = "[Name (first, middle, last, suffix)] - Fitness Determination Applicant Instructions [Tier Type]";

            var OF_0306 = "OF0306.pdf";
            var GSA_3665 = "GSA3665.pdf";
            var SF_85 = "SF85.pdf";
            var SF_85P = "SF85P.pdf";
            var appInstructions = attachments.ApplicaitonInstruction(personInfo.Region, personInfo.InvestigatonRequested);
            var additionalQuestionsForModerateRiskPositionsForm = "AdditionalQuestionsForModerateRiskPositionsForm.pdf";

            //Switch on investigation requested to get needed attachments
            switch (personInfo.InvestigatonRequested.ToLower())
            {
                case "tier 1":
                case "naci":
                    body = File.ReadAllText(GetFilePath("Tier1.html"));
                    emailAttachments.Append(GetFilePath(appInstructions, true));
                    emailAttachments.Append(GetFilePath(OF_0306, true));
                    emailAttachments.Append(GetFilePath(GSA_3665, true));
                    emailAttachments.Append(GetFilePath(SF_85, true));
                    
                    break;

                case "tier 2s":
                case "mbi":
                case "tier 2":
                    body = File.ReadAllText(GetFilePath("Tier2.html"));
                    emailAttachments.Append(GetFilePath(appInstructions, true));
                    emailAttachments.Append(GetFilePath(OF_0306, true));
                    emailAttachments.Append(GetFilePath(GSA_3665, true));
                    emailAttachments.Append(GetFilePath(SF_85P, true));                    
                    break;

                case "tier 2rs":
                    body = File.ReadAllText(GetFilePath("Tier2RS.html"));
                    emailAttachments.Append(GetFilePath(GSA_3665, true));
                    emailAttachments.Append(GetFilePath(SF_85P, true));
                    emailAttachments.Append(GetFilePath(additionalQuestionsForModerateRiskPositionsForm, true));
                    break;

                case "tier 4":
                case "bi":
                    body = File.ReadAllText(GetFilePath("Tier4.html"));
                    emailAttachments.Append(GetFilePath(appInstructions, true));
                    emailAttachments.Append(GetFilePath(OF_0306, true));
                    emailAttachments.Append(GetFilePath(GSA_3665, true));
                    emailAttachments.Append(GetFilePath(SF_85P, true));                    
                    break;

                case "tier 4r":
                    body = File.ReadAllText(GetFilePath("Tier4R.html"));
                    emailAttachments.Append(GetFilePath(GSA_3665, true));
                    emailAttachments.Append(GetFilePath(SF_85P, true));
                    break;

                case "tier 1c":
                    body = File.ReadAllText(GetFilePath("Tier1C.html"));

                    emailAttachments.Append(GetFilePath("GSA176-15.pdf", true));
                    emailAttachments.Append(GetFilePath("Tier1CStateForms.pdf", true));
                    emailAttachments.Append(GetFilePath(SF_85, true));
                    break;

                case "sac":
                case "fingerprint":

                    if ((new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" }.Contains(personInfo.Region) && personInfo.MajorOrg.ToLower() != "p") || (new[] { "CO", "NCR" }.Contains(personInfo.Region.ToUpper())))
                    {
                        UseSacPBSEmail(emailAttachments, ref body);
                    }
                    else
                    {
                        UseSacEmail(ref body);
                    }

                    ////Has a specific subject
                    subject = "[Name (first, middle, last, suffix)] - GSA Special Agreement Check (SAC) Fitness Determination Applicant Instructions (less than 6 month)";

                    break;
                default:
                    break;
            }

            //Replace placeholder name in subject
            subject = subject.Replace("[Name (first, middle, last, suffix)]", personInfo.FullName);
            subject = subject.Replace("[Tier Type]", personInfo.InvestigatonRequested);

            //Replace placeholder fields in body
            body = body.Replace("[NAME]", personInfo.FullName);
            body = body.Replace("[REGIONALEMAIL]", regionalEMail);
            body = body.Replace("[PHONENUMBER]", phoneNumber);

            //If NCR or CO and home email/sponsor email is null
            if ((personInfo.Region == "NCR" || personInfo.Region == "CO") && string.IsNullOrEmpty(personInfo.HomeEMail) && string.IsNullOrEmpty(sponsorshipEmails))
            {
                emails = regionalEMail;
            }
            else
            {
                emails = string.Join(",", sponsorshipEmails, regionalEMail);
            }

            //If major org is q
            if (IncludeFASEMail(personInfo.MajorOrg.ToLower()))
                emails = string.Join(",", emails, ConfigurationManager.AppSettings["FASEMAIL"].ToString());

            //Adds CHILDCAREEMAIL to email CC
            if (IncludeChildCareEMail(personInfo.InvestigationType.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                emails = string.Join(",", emails, ConfigurationManager.AppSettings["CHILDCAREEMAIL"].ToString());

            //Removes leading and trailing commas
            emails = emails.TrimStart(',').TrimEnd(',');

            //If childcare send to default email (hspd12.security@gsa.gov) 
            //Remove defaultEMail from email BCC - UPDATE 09/20/2018 - Client wants to add HSPD-12 Security to BCC
            //Remove zonal email from sender and recipient
            if (IncludeChildCareEMail(personInfo.InvestigationType.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                SendT1CSponsorship(personInfo, emails, subject, body, emailAttachments.ToString(), regionalEMail);
            else
                message.Send(regionalEMail, personInfo.HomeEMail, emails, defaultEMail, subject, body, emailAttachments.ToString().TrimEnd(';'), smtpServer, true);
        }

        /// <summary>
        ///     Configures the attachment path by combining the onboarding loaction
        ///     with the file name.
        /// </summary>
        /// <param name="fileName">The file name of the attachment</param>
        /// <param name="isAttachement">Flag determining if the file is an attachment</param>
        /// <returns>Returns a <see cref="string"/> containing the onboarding location and the file name.</returns>
        private string GetFilePath(string fileName, bool isAttachement = false)
        {
            return $@"{onboardingLocation}\{fileName}{(isAttachement ? ";" : "")}";
        }

        /// <summary>
        /// Called if child care for sponsorship
        /// Send to default email (hspd12.security@gsa.gov) 
        /// Remove defaultEMail from email BCC
        /// Remove regional email from email var
        /// </summary>
        /// <param name="personInfo"></param>
        /// <param name="emails"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="emailAttachments"></param>
        /// <param name="regionalEmail"></param>
        private void SendT1CSponsorship(PersonDetails personInfo, string emails, string subject, string body, string emailAttachments, string regionalEmail)
        {
            //split list to make it easier to remove elements regardless of position
            var emailList = emails.Split(',').ToList();
            //remove regionEmail from list
            if (emailList.Exists(e => e == regionalEmail))
                emailList.RemoveAt(emailList.FindIndex(e => e == regionalEmail));
            //call send after converting back to comma separated string
            message.Send(defaultEMail, personInfo.HomeEMail, string.Join(",", emailList), defaultEMail, subject, body, emailAttachments.ToString().TrimEnd(';'), smtpServer, true);
        }

        /// <summary>
        /// Called if child care for adjudication
        /// send to gsapoc's
        /// Remove regional email from list
        /// </summary>
        /// <param name="gsaPOCEmails"></param>
        /// <param name="emails"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="emailAttachments"></param>
        /// <param name="regionalEmail"></param>
        private void SendT1CAdjudication(string gsaPOCEmails, string emails, string subject, string body, string emailAttachments, string regionalEmail)
        {
            //split list to make it easier to remove elements regardless of position
            var emailList = emails.Split(',').ToList();
            //remove regionEmail from list
            if (emailList.Exists(e => e == regionalEmail))
                emailList.RemoveAt(emailList.FindIndex(e => e == regionalEmail));
            //call send after converting back to comma separated string
            message.Send(defaultEMail, gsaPOCEmails, string.Join(",", emailList), defaultEMail, subject, body, emailAttachments.ToString().TrimEnd(';'), smtpServer, true);
        }

        /// <summary>
        /// Set body to SAC_PBS and attaches of0306
        /// </summary>
        /// <param name="emailAttachments"></param>
        /// <param name="body"></param>
        private void UseSacPBSEmail(StringBuilder emailAttachments, ref string body)
        {
            //use SAC_PBS email
            body = File.ReadAllText(onboardingLocation + @"\SAC_PBS.html");
            //include OF0306.pdf attachment
            emailAttachments.Append(onboardingLocation + @"\OF0306.pdf");
        }

        /// <summary>
        /// Set body to SAC with no attachment
        /// </summary>
        /// <param name="body"></param>
        private void UseSacEmail(ref string body)
        {
            //old sac email
            body = File.ReadAllText(onboardingLocation + @"\SAC.html");
        }


        /// <summary>
        /// New SendAdjudicationNotification method developed ss part of email consolidation.
        /// All the business logic to process the person information is moved to DB.
        /// <summary>
        public void SendAdjudicationNotification()
        {
            HSPD12Email("ADJC");
        }

        /// <summary>
        /// New SendSponsorshipNotification method developed as part of email consolidation.
        /// All the business logic to process the person information is moved to DB.
        /// <summary>
        public void SendSponsorshipNotification()
        {
            HSPD12Email("SPSH");
        }


        /// <summary>
        /// New SendSRSNotification method developed ss part of email consolidation.
        /// All the business logic to process the person information is moved to DB.
        /// <summary>
        public void SendSRSNotification()
        {
            HSPD12Email("SRS");
        }


        /// <summary>
        /// New SendExpiringContractReminder method developed ss part of email consolidation. For CORS.
        /// All the business logic to process the person information is moved to DB.
        /// <summary>
        public void SendExpiringContractReminder()
        {
            HSPD12Email("CORS");
        }


        /// <summary>
        /// New method that access the DB and gets the email details.
        /// Developed as part of email consolidation.   
        /// <summary>
        /// <param name="AppCode"></param>
        private void HSPD12Email(string AppCode)
        {
            //Declare function variables
            EmailDetails emailData = new EmailDetails();
            emailData = emailData.GetEmailDetails(AppCode, personID, conn);
            string strEmailAttachment = string.Empty;
            if (emailData.EmailAttachment.IndexOf(";") > 0) {
                strEmailAttachment = onboardingLocation + emailData.EmailAttachment.Replace(";", string.Concat(";", onboardingLocation));
            }
            message.Send(emailData.EmailFromAdd, emailData.EmailToAdd, emailData.EmailCCAdd, emailData.EmailBCCAdd, emailData.EmailSubject, emailData.EmailBody, strEmailAttachment, smtpServer, true);
        }

        
        /// <summary>
        /// The following are 3 overloaded methods for SendAdjudicationNotification, SendSponsorshipNotification and SendSRSNotification developed as part of email consolidation.
        /// This overloaded method is to handle new types of unique requests that may come in the future.
        /// <summary>
        /// <param name="AppCode"></param>
        public void SendAdjudicationNotification(string sAppCode)
        {
            HSPD12Email(sAppCode);
        }

        public void SendSponsorshipNotification(string sAppCode)
        {
            HSPD12Email(sAppCode);
        }
        public void SendSRSNotification(string sAppCode)
        {
            HSPD12Email(sAppCode);
        }
        public void SendExpiringContractReminder(string sAppCode)
        {
            HSPD12Email(sAppCode);
        }


    }
}