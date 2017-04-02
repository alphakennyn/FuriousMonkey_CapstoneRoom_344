using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using LogicLayer;
using MySql.Data.MySqlClient;


namespace TDG
{

    public class TDGReservation
    {
        //This instance

        private static TDGReservation instance = new TDGReservation();

        //Table name
        private const String TABLE_NAME = "reservation";

        //Fields names of the table
        private readonly String[] FIELDS = { "reservationID", "userID", "roomID", "description", "date" }; 

        //Database server (localhost)
        private const String DATABASE_SERVER = "127.0.0.1";

        //Database to which we will connect
        private const String DATABASE_NAME = "reservation_system";

        //Credentials to connect to the databas
        private const String DATABASE_UID = "root";
        private const String DATABASE_PWD = " ";

        //The whole connection string used to connect to the database
        //In our case, we will always connect to the same database, this it can
        //be defined as a constant. But we can always change it.
        private const String DATABASE_CONNECTION_STRING = "server=" + DATABASE_SERVER + ";uid=" + DATABASE_UID + ";pwd=" + DATABASE_PWD + ";database=" + DATABASE_NAME + ";";

        //Determine after how much time a command (query) should be timed out
        private const int COMMAND_TIMEOUT = 60;

        /**
         * Returns the instance
         * */

        public static TDGReservation getInstance()
        {
            return instance;
        }

        /**
         * Default Constructor
         */

        private TDGReservation()
        {

        }


        /**
         * Add new reservations to the database
         * */


        public void addReservation(List<Reservation> newList)
        {
            
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < newList.Count; i++)
                {
                    createReservation(conn, newList[i]);
                }
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        /**
         * Update reservations of the database
         * */

        public void updateReservation(List<Reservation> updateList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and update many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < updateList.Count; i++)
                {
                    updateReservation(conn, updateList[i]);
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


        /**
         * Delete reservation(s) from the databas
         *
         * */

        public void deleteReservation(List<Reservation> deleteList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and update many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < deleteList.Count; i++)
                {
                    removeReservation(conn, deleteList[i]);
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


        /**
         * Returns a record for the reservation given its reservationID
         * */

        public Object[] get(int reservationID)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + " = " + reservationID;
            Object[] record = null; // to be returned
            MySqlDataReader reader = null;

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();
                
                //If no record is found, return null
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return null;
                }

                //There is only one result since we find it by id
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
                    record[4] = reader[4];

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                conn.Close();
            }
            //Format and return the result
            return record;
        }


        /**
         * Select all data from the table
         * Returns it as a Dictionary <int, Object[]>
         * Where int is the ID of the object and Object[] contains the record of the row
         * */

        public Dictionary<int, Object[]> getAll()
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE 1;";
            Dictionary<int, Object[]> records = new Dictionary<int, Object[]>();
            MySqlDataReader reader = null;

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                //If no record is found, return empty records
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return records;

                }

                //For each reader, add it to the dictionary
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return records;
                    }

                    Object[] attributes = new Object[FIELDS.Length];
                    attributes[0] = reader[0]; //reservationID
                    attributes[1] = reader[1]; // userID
                    attributes[2] = reader[2]; //roomID
                    attributes[3] = reader[3]; //desc
                    attributes[4] = reader[4]; //date
                    
                    records.Add((int)reader[0], attributes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                conn.Close();
            }

            //Format and return the result
            return records;
        }

        /**
         * Adds one reservation to the database
         * */
        private void createReservation(MySqlConnection conn, Reservation reservation)
        {
            if(reservation == null)
                return;

            String mySqlDate = reservation.date.Date.ToString("yyyy-MM-dd");
            String commandLine = "INSERT INTO " + TABLE_NAME + " VALUES (" + reservation.reservationID + "," +
                reservation.userID + "," + reservation.roomID + ",'" + reservation.description + "', '" +
                mySqlDate + " ');";

            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            List<MySqlCommand> cmdEquipmentList = new List<MySqlCommand>();
            foreach (Equipment equipment in reservation.equipmentList)
            {
                String commandLineEquipment = "INSERT INTO " + "reservationidlist" + " VALUES (" + equipment.equipmentID + "," + reservation.reservationID + " ');";
                cmdEquipmentList.Add(new MySqlCommand(commandLineEquipment, conn));
            }

            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                foreach(MySqlCommand ecmd in cmdEquipmentList)
                {
                    reader = ecmd.ExecuteReader();
                }
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if(reader != null)
                    reader.Close();
            }
        }

        /**
         * Updates one reservation of the database
         * */

        private void updateReservation(MySqlConnection conn, Reservation reservation)
        {
            if (reservation == null)
                return;

            String mySqlDate = reservation.date.Date.ToString("yyyy-MM-dd");
            String commandLine = "UPDATE " + TABLE_NAME + " SET " +
                FIELDS[4] + " = '" + mySqlDate + "', " + FIELDS[3] + " = '" + reservation.description + "', " +
                FIELDS[2] + " = " + reservation.roomID + ", " + FIELDS[1] + " = " + reservation.userID + " WHERE " +
                FIELDS[0] + " = " + reservation.reservationID + ";";
            
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            MySqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if(reader != null)
                    reader.Close();
            }
        }


        /**
         * Removes one reservation from the database
         * */

        private void removeReservation(MySqlConnection conn, Reservation reservation)
        {
            if (reservation == null)
                return;

            String commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + "=" + reservation.reservationID + ";";
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            MySqlDataReader reader = null;

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
                if(reader != null)
                    reader.Close();
            }
        }

        /**
         * Get the last ID that was entered
         */
        public int getLastID()
        {
            // lastID to be returned
            int lastID = 0;
            bool success = true;

            String commandLine = "SELECT MAX(" + FIELDS[0] + ") FROM " + TABLE_NAME;
            MySqlDataReader reader = null;
            

            // Attempt to open connection
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                // read it, there should only be one
                while (reader.Read())
                {
                    if (reader[0].GetType() != typeof(System.DBNull))
                    {
                        lastID = (int)reader[0];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                success = false;
            }
            finally
            {
                if(reader!=null)
                    reader.Close();
                conn.Close();
            }

            // return the last id
            if (success)
                return lastID;
            else
                return -2;
        }

        /**
         * Get the list of reservation IDs associated with the userID at a specific day
         */
        public List<int> getReservationIDs(int userID, DateTime date)
        {
            List<int> IDlist = new List<int>();
            String mySqlDate = date.Date.ToString("yyyy-MM-dd");
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE " + FIELDS[1] + " = " + userID + " AND " + FIELDS[4] + " = '" + mySqlDate + "';";
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            MySqlDataReader reader = null;

            //Open connection and execute query
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return null;
                }

                //For each reader, add it to the dictionary
                while (reader.Read())
                {
                    IDlist.Add(Convert.ToInt32(reader[0]));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
                if (reader != null)
                    reader.Close();
            }

            //Format and return the result
            return IDlist;
        }
    }
}
