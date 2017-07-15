using MySql.Data.MySqlClient;
using System.Data;

namespace Suitability
{
    class Person
    {        
        private MySqlConnection conn;
        private MySqlCommand cmd = new MySqlCommand();

        public Person(MySqlConnection conn)
        {
            this.conn = conn;
        }

        public PersonDetails GetPersonDetails(int personID, string type)
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

                        cmd.Parameters.AddWithValue("person_id", personID);

                        cmd.CommandText = "AdjudicationPersonDetails";
                        cmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataReader personData = cmd.ExecuteReader();

                        while (personData.Read())
                        {
                            if (type == "A") return PersonDetails.Adjudication(personData);
                            if (type == "S") return PersonDetails.Sponsorship(personData);
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