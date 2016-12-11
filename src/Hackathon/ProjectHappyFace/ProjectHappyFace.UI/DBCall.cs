using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ProjectHappyFace.UI
{
    public class DBCall
    {
        public int StoreUSerDataInDatabase(UserData data)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCommand cmd = new SqlCommand("Usp_SaveUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
               // cmd.Parameters.Add("@img", SqlDbType.Image).Value = img;
               return cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {

            }
            return 0;
        }


        public static string GetConnectionString()
        {
            return @"Data Source=5CG4392SWD\AURA;Initial Catalog=hackathon;Integrated Security=True";//Build Connection String and Return
        }

       
    }
}