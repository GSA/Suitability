using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Suitability
{
    
    public class SendNotification
    {
        private MySqlConnection conn = new MySqlConnection();

        private string defaultEMail = "hspd12.security@gsa.gov";
        private int personID = 0;
        private string connectionString = string.Empty;
        private string smtpServer = "192.168.1.1";
        //private string emailTemplatesLocation = string.Empty;
        //private string regionalXMLLocation = string.Empty;
        private string onboardingLocation = string.Empty;

        private Suitability.EMail message = new Suitability.EMail();        

        /// <summary>
        /// Constructor for adjudication e-mails
        /// </summary>
        /// <param name="defaultEMail"></param>
        /// <param name="personID"></param>
        /// <param name="connectionString"></param>
        /// <param name="smtpServer"></param>
        /// <param name="emailTemplatesLocation"></param>
        public SendNotification(string defaultEMail, int personID, string connectionString, string smtpServer, string onboardingLocation)
        {            
            this.defaultEMail = defaultEMail;
            this.personID = personID;            
            this.connectionString = connectionString;
            this.smtpServer = smtpServer;
            //this.emailTemplatesLocation = emailTemplatesLocation;
            //this.regionalXMLLocation = onboardingLocation;
            this.onboardingLocation = onboardingLocation;
            this.conn.ConnectionString = connectionString;
        }

        private bool IncludeFASEMail(string majorOrg)
        {
            if (majorOrg == "q") return true;
            
            return false;
        }

        private bool IncludeChildCareEMail(string investType, string investTypeRequested)
        {
            if ((investType == "tier 1c") || (investTypeRequested =="tier 1c")) return true;

            return false;
        }

        /// <summary>
        /// Sends adjudication e-mails
        /// </summary>
        public void SendAdjudicationNotification()
        {            
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

            personInfo = personData.GetPersonDetails(personID, "A");

            gsaPOCEMails = contractData.GetPOCEmails(personID, "AdjudicationGovEMails");
            vendorPOCEmails = contractData.GetPOCEmails(personID, "AdjudicationVendorEMails");
            gsaPOCNames = contractData.GetGSAPOCNames(personID);
            contractInfo = contractData.GetContractDetails(personID);

            //If contract info is null then create a new empty one, sets the defaults to string.empty
            if (contractInfo == null)
                contractInfo = new ContractDetails();                        
            
            if (personInfo.Region != "CO" && personInfo.Region != "NCR")
                regionalEMails = regionalData.GetRegionalEMails(personInfo.Region, onboardingLocation + @"\RegionalEMails.xml");

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
                case "tier 2s":
                case "tier 4":
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
            switch (personInfo.InvestigationType.ToLower())
            {
                case "tier 1":
                    investType = "Tier 1 (Previously NACI)";
                    break;
                case "tier 2s":
                    investType = "Tier 2S (Previously MBI)";
                    break;
                case "tier 4":
                    investType = "Tier 4 (Previously BI)";
                    break;
                default:
                    investType = personInfo.InvestigationType;
                    break;
            }

            body = body.Replace("[INVESTIGATIONTYPE]", investType);
            
            //body = body.Replace("[INVESTIGATIONTYPE]", personInfo.InvestigationType);

            //Replace Investigation Date
            body = body.Replace("[INVESTIGATIONDATE]", personInfo.InvestigationDate.Value.ToString("MMMM dd, yyyy"));

            //If GSA POC, Vendor POC and Home emails are empty, then add hspd12.security@gsa.gov to the To email line
            if (string.IsNullOrEmpty(gsaPOCEMails) && string.IsNullOrEmpty(vendorPOCEmails) && string.IsNullOrEmpty(personInfo.HomeEMail))
            {
                message.Send(defaultEMail, defaultEMail, regionalEMails, defaultEMail, subject, body, "", smtpServer, true);
                return;
            }

            if (IncludeFASEMail(personInfo.MajorOrg.ToLower()))
                fasEMail = ConfigurationManager.AppSettings["FASEMAIL"].ToString();

            if (IncludeChildCareEMail(personInfo.InvestigationType.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                childcareEMail = ConfigurationManager.AppSettings["CHILDCAREEMAIL"].ToString();

            string[] emailsToJoin = { vendorPOCEmails, personInfo.HomeEMail, regionalEMails, fasEMail, childcareEMail };

            var emails = string.Join(",", emailsToJoin.Where(s => !string.IsNullOrEmpty(s))).TrimEnd(',');

            message.Send(defaultEMail, gsaPOCEMails, emails, defaultEMail, subject, body, "", smtpServer, true);
            
            return;
        }

        public void SendSponsorshipNotification()
        {
            StringBuilder emailAttachments = new StringBuilder();

            string body = string.Empty;
            string subject = string.Empty;
            string emails = string.Empty;
            string regionalEMail = string.Empty;
            string sponsorshipEmails = string.Empty;
            string investType = string.Empty;
            string phoneNumber = string.Empty;

            Person personData = new Person(conn);
            PersonDetails personInfo = new PersonDetails();
            Contracts contracts = new Contracts(conn);
            Regions regionalData = new Regions();
            Attachments attachments = new Attachments();

            conn = new MySqlConnection();
            conn.ConnectionString = connectionString;            

            personInfo = personData.GetPersonDetails(personID, "S");
            sponsorshipEmails = contracts.GetSponsorshipEMails(personID);

            regionalEMail = regionalData.GetRegionalEMails(personInfo.Region, onboardingLocation + @"\RegionalEMails.xml");
            phoneNumber = regionalData.GetRegionalPhoneNumbers(personInfo.Region, onboardingLocation + @"\RegionalPhoneNumbers.xml");

            subject = "[Name (first, middle, last, suffix)] - Fitness Determination Applicant Instructions [Tier Type]";

            switch (personInfo.InvestigatonRequested.ToLower())
            {
                case "tier 1":
                case "naci":
                    body = File.ReadAllText(onboardingLocation + @"\Tier12S4.html");

                    emailAttachments.Append(onboardingLocation + @"\OF0306.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\GSA3665.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\SF85.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\" + attachments.ApplicaitonInstruction(personInfo.Region));

                    break;
                case "tier 2s":
                case "tier 4":
                case "mbi":
                case "bi":
                    body = File.ReadAllText(onboardingLocation + @"\Tier12S4.html");

                    emailAttachments.Append(onboardingLocation + @"\OF0306.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\GSA3665.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\SF85P.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\" + attachments.ApplicaitonInstruction(personInfo.Region));

                    break;
                case "tier 1c":
                    body = File.ReadAllText(onboardingLocation + @"\Tier1C.html");

                    emailAttachments.Append(onboardingLocation + @"\GSA176-15.pdf" + ";");
                    emailAttachments.Append(onboardingLocation + @"\Tier1CStateForms.pdf");

                    break;
                case "sac":
                case "fingerprint":
                    body = File.ReadAllText(onboardingLocation + @"\SAC.html");

                    //Has a specific subject
                    subject = "[Name (first, middle, last, suffix)] - GSA Special Agreement Check (SAC) Fitness Determination Applicant Instructions (less than 6 month)";

                    break;
                default:
                    break;
            }
                        
            subject = subject.Replace("[Name (first, middle, last, suffix)]", personInfo.FullName);
            subject = subject.Replace("[Tier Type]", personInfo.InvestigatonRequested);
            body = body.Replace("[NAME]", personInfo.FullName);
            body = body.Replace("[REGIONALEMAIL]", regionalEMail);
            body = body.Replace("[PHONENUMBER]", phoneNumber);

            if ((personInfo.Region == "NCR" || personInfo.Region == "CO") && 
                 string.IsNullOrEmpty(personInfo.HomeEMail) && string.IsNullOrEmpty(sponsorshipEmails))
            {
                emails = regionalEMail;
            }
            else
            {
                emails = string.Join(",", sponsorshipEmails, regionalEMail);
            }

            if (IncludeFASEMail(personInfo.MajorOrg.ToLower()))
                emails = string.Join(",", emails, ConfigurationManager.AppSettings["FASEMAIL"].ToString());

            if (IncludeChildCareEMail(personInfo.InvestigatonRequested.ToLower(), personInfo.InvestigatonRequested.ToLower()))
                emails = string.Join(",", emails, ConfigurationManager.AppSettings["CHILDCAREEMAIL"].ToString());

            emails = emails.TrimStart(',').TrimEnd(',');

            message.Send(regionalEMail, personInfo.HomeEMail, emails, defaultEMail, subject, body, emailAttachments.ToString().TrimEnd(';'), smtpServer, true);            
        }
    }
}-