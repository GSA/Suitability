using System;
using System.Data;

namespace Suitability
{
    class PersonDetails
    {
        public const string AdjudicationPersonDetails = "A";
        public const string SponsorshipPersonDetails = "S";

        //Class properties
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
        /// <summary>
        /// Returns person details object with data from IDataRecord
        /// </summary>
        /// <param name="record"></param>
        /// <returns>A <see cref="PersonDetails"/> object</returns>
        public static PersonDetails Adjudication(IDataRecord record)
        {
            return GetPersonDetails(record, AdjudicationPersonDetails);
        }

        /// <summary>
        /// Returns person details object with data from IDataRecord
        /// </summary>
        /// <param name="record"></param>
        /// <returns>A <see cref="PersonDetails"/> object</returns>
        public static PersonDetails Sponsorship(IDataRecord record)
        {
            return GetPersonDetails(record, SponsorshipPersonDetails);
        }

        /// <summary>
        /// Returns person details object with data from IDataRecord
        /// </summary>
        /// <param name="record"></param>
        /// <param name="type">A = Adjudication; S = Sponsorship</param>
        /// <returns></returns>
        public static PersonDetails GetPersonDetails(IDataRecord record, string type)
        {
            try
            {
                var personDetails = new PersonDetails
                {
                    FullName = record["Full Name"].ToString(),
                    SubjectName = record["Subject Name"].ToString(),
                    Position = record["Position"].ToString(),
                    HomeEMail = record["Home E-Mail"].ToString(),
                    IsCitizen = (bool)record["Is Citizen"],
                    InvestigationDate = (type.ToUpper() == AdjudicationPersonDetails 
                        ? (DateTime)record["Investigation Date"] as DateTime?
                        : null),
                    InvestigatonRequested = record["Investigation Requested"].ToString(),
                    InvestigationType = record["Investigation Type"].ToString(),
                    PortOfEntryDate = record["Port of Entry"].ToString(),
                    Region = record["Region"].ToString(),
                    MajorOrg = record["Major Org"].ToString()
                };

                return personDetails;
            }
            catch (Exception)
            {
                //log
                throw;
            }
        }
    }
}