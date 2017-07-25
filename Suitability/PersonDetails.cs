﻿using System;
using System.Data;

namespace Suitability
{
    class PersonDetails
    {   
        public string FullName { get; set; }
        public string SubjectName { get; set; }
        public string Position { get; set; }
        public string HomeEMail { get; set; }
        public bool IsCitizen { get; set; }
        public string InvestigatonRequested { get; set; } //pers_investigation_date
        public DateTime? InvestigationDate { get; set; } //pers_investigation_type_requested
        //public DateTime? SponsorshipDate { get; set; }
        public string PortOfEntryDate { get; set; } //This is a string because in the DB it's a blob
        public string Region { get; set; }
        public string MajorOrg { get; set; }
        public string InvestigationType { get; set; }

        //Should change to generic
        public static PersonDetails Adjudication(IDataRecord record)
        {
            return new PersonDetails
            {
                FullName = record["Full Name"].ToString(),
                SubjectName = record["Subject Name"].ToString(),
                Position = record["Position"].ToString(),
                HomeEMail = record["Home E-Mail"].ToString(),
                IsCitizen = (sbyte)record["Is Citizen"] == 1,
                //SponsorshipDate = (DateTime)record["Sponsored Date"] as DateTime?,
                InvestigationDate = (DateTime)record["Investigation Date"] as DateTime?,
                InvestigatonRequested = record["Investigation Requested"].ToString(),
                InvestigationType = record["Investigation Type"].ToString(),
                PortOfEntryDate = record["Port of Entry"].ToString(),
                Region = record["Region"].ToString(),
                MajorOrg = record["Major Org"].ToString()
            };
        }

        public static PersonDetails Sponsorship(IDataRecord record)
        {
            try
            {
                return new PersonDetails
                {
                    FullName = record["Full Name"].ToString(),
                    SubjectName = record["Subject Name"].ToString(),
                    Position = record["Position"].ToString(),
                    HomeEMail = record["Home E-Mail"].ToString(),
                    IsCitizen = (sbyte)record["Is Citizen"] == 1,
                    //SponsorshipDate = (DateTime)record["Sponsored Date"] as DateTime?,                
                    InvestigatonRequested = record["Investigation Requested"].ToString(),
                    InvestigationType = record["Investigation Type"].ToString(),
                    PortOfEntryDate = record["Port of Entry"].ToString(),
                    Region = record["Region"].ToString(),
                    MajorOrg = record["Major Org"].ToString()
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
