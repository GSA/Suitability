using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using System;

namespace Suitability
{
    class Contracts
    {        
        private MySqlConnection conn;
        private MySqlCommand cmd = new MySqlCommand();

        public Contracts(MySqlConnection conn)
        {
            this.conn = conn;
        }

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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //Log exception
                throw;
            }
        }       

        //change variable e-mails to names as this returns names not e-mails
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //Log exception
                throw;
            }
        }

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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //Log exception
                throw;
            }
        }
        
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //Log exception
                throw;
            }            
        }
    }
}