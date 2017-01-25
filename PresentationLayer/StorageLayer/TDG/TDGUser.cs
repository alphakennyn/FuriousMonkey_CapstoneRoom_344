using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LogicLayer;

namespace TDG
{
    /**
  * Class: TDGUser
  * 
  * Table data gateway of User table
  * 
  * This class acts as a bridge from the software application to the database.
  * It allows to create, update, delete and find data from the user database table.
  */
    public class TDGUser
    {
        // This instance
        private const String TABLE_NAME = "user";

        // Table name
        private static TDGUser instance = new TDGUser();

        // Field names of the table
        private static readonly String[] FIELDS = { "userID", "username", "password", "name" };
        
        // Database server (localhost)
        private const String DATABASE_SERVER = "127.0.0.1";

        // Database to which we will connect
        private const String DATABASE_NAME = "reservation_system";

        // Credentials to connect to the database
        private const String DATABASE_UID = "root";
        private const String DATABASE_PWD = "";

        // The whole connection string used to connect to the database
        // In our case, we will always connect to the same database, thus it can
        // be defined as a constant. But we can always change it.
        private const String DATABASE_CONNECTION_STRING = "server=" + DATABASE_SERVER + ";uid=" + DATABASE_UID + ";pwd=" + DATABASE_PWD + ";database=" + DATABASE_NAME + ";";

        // Determine after how much time a command (query) should be timed out
        private const int COMMAND_TIMEOUT = 60;

        // Constructor
        public TDGUser()
        {
        }

        public static TDGUser getInstance()
        {
            return instance;
        }

        // Updates the list of users in the DB
        public void updateUser(List<User> updateList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and update many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < updateList.Count; i++)
                {
                    updateUser(conn, updateList[i]);
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // SQL Statement to update an existing User/Row
        private void updateUser(MySqlConnection conn, User user) {
            if (user == null)
                return;

            String commandLine = "UPDATE " + TABLE_NAME + " \n" +
                   "SET " + FIELDS[1] + "='" + user.username + "'," + FIELDS[2] + "='" + user.password + "'," +
                   FIELDS[3] + "='" + user.name + ";\n" +
                   " WHERE " + FIELDS[0] + "=" + user.userID + ";";
            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        /**
       * Returns a record for the user given its userID
       */
        public Object[] get(int userID)
        {
            String commandLine = "SELECT * FROM " + TABLE_NAME + " \n" +
                    " WHERE " + FIELDS[0] + "=" + userID + ";";
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            MySqlDataReader reader = null;
            Object[] record = null; // to be returned

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                // If no record is found, return null
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return null;
                }
                // There is only one result since we find it by id
                record = new Object[FIELDS.Length];
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }
                    record[0] = reader[0];
                    record[1] = reader[1];
                    record[2] = reader[2];
                    record[3] = reader[3];

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Close connection
                if(reader!=null)
                    reader.Close();
                conn.Close();
            }

            // Format and return the result
            return record;
        }

      

        /**
         * Select all data from the table
         * Returns it as a Dictionary<int, Object[]>
         * Where int is the ID of the object and Object[] contains the record of the row
         */
        public Dictionary<int, Object[]> getAll()
        {
            Dictionary<int, Object[]> records = new Dictionary<int, Object[]>();
            MySqlDataReader reader = null;
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE 1;";
            

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                // If no record is found, return empty records
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return records;
                }

                // For each reader, add it to the dictionary
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return records;
                    }
                    Object[] attributes = new Object[FIELDS.Length];
                    attributes[0] = reader[0]; // userID
                    attributes[1] = reader[1]; // userName
                    attributes[2] = reader[2]; // password
                    attributes[3] = reader[3]; // name
                    records.Add((int)reader[0], attributes);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(reader!=null)
                    reader.Close();
                conn.Close();
            }

            // Format and return the result
            return records;
        }
    }
}
