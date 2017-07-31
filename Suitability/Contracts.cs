using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using System;

namespace Suitability
{
    class Contracts
    {
        //Set up db connection
        private MySqlConnection conn;
        private MySqlCommand cmd = new MySqlCommand();

        //Pass in connection to use
        public Contracts(MySqlConnection conn)
        {
            this.conn = conn;
        }

        /// <summary>
        /// Get POC emails from db
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="storedProcedure"></param>
        /// <returns></returns>
        public string GetPOCEmails(int personID, string storedProcedure)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("pers_id", personID);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = storedProcedure;

                        object POCEMails = cmd.ExecuteScalar();

                        return (POCEMails == DBNull.Value) ? string.Empty : (string)POCEMails;
                    }

                }
            }
            //Catch all errors
            catch (MySqlException)
            {
                //Log exception
                throw;
            }
        }

        //change variable e-mails to names as this returns names not e-mails
        /// <summary>
        /// Get gsa poc names from db
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public string GetGSAPOCNames(int personID)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("pers_id", personID);

                        cmd.CommandText = "AdjudicationGSAPOCNames";
                        cmd.CommandType = CommandType.StoredProcedure;

                        object GSAPOCNames = cmd.ExecuteScalar();

                        return (GSAPOCNames == DBNull.Value) ? string.Empty : (string)GSAPOCNames;
                    }
                }
            }
            ///Catch all errors
            catch (MySqlException)
            {
                //Log exception
                throw;
            }
        }

        /// <summary>
        /// Get sponsorship emails from db
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public string GetSponsorshipEMails(int personID)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("pers_id", personID);

                        cmd.CommandText = "SponsorshipEMails";
                        cmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataReader sponsorshipEMails = cmd.ExecuteReader();

                        StringBuilder emails = new StringBuilder();

                        while (sponsorshipEMails.Read())
                        {
                            if (!sponsorshipEMails.IsDBNull(0))
                            {
                                emails.Append(sponsorshipEMails.GetValue(0));
                                emails.Append(",");
                            }
                        }

                        return emails.ToString().TrimEnd(',');
                    }
                }
            }
            //Catch all errors
            catch (MySqlException)
            {
                //Log exception
                throw;
            }
        }

        /// <summary>
        /// Get contract details from db
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public ContractDetails GetContractDetails(int personID)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;

                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("pers_id", personID);

                        cmd.CommandText = "AdjudicationContractInfo";
                        cmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataReader contractData = cmd.ExecuteReader();

                        while (contractData.Read())
                        {
                            return ContractDetails.Create(contractData);
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