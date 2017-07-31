using MySql.Data.MySqlClient;
using System.Data;

namespace Suitability
{
    class Person
    {
        //Set up db connection
        private MySqlConnection conn;
        private MySqlCommand cmd = new MySqlCommand();

        //Pass in connection to use
        public Person(MySqlConnection conn)
        {
            this.conn = conn;
        }

        /// <summary>
        /// Return person details from db
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public PersonDetails GetPersonDetails(int personID, string type)
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

                        cmd.Parameters.AddWithValue("person_id", personID);

                        cmd.CommandText = "AdjudicationPersonDetails";
                        cmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataReader personData = cmd.ExecuteReader();

                        //Return details depending on type
                        while (personData.Read())
                        {
                            if (type == "A") return PersonDetails.Adjudication(personData);
                            if (type == "S") return PersonDetails.Sponsorship(personData);
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