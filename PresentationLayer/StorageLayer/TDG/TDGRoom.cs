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
     * Class: TDGRoom
     * 
     * Table data gateway of Room table
     * 
     * This class acts as a bridge from the software application to the database.
     * It allows to create, update, delete and find data from the room database table.
     */

    public class TDGRoom
    {
        // This instance
        private static TDGRoom instance = new TDGRoom();

        // Table name
        private const String TABLE_NAME = "room";

        // Field names of the table
        private readonly String[] FIELDS = { "RoomID", "RoomNum" };

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


        /**
         * Returns the instance
         */
        public static TDGRoom getInstance()
        {
            return instance;
        }

        /**
         * Default constructor
         */
        private TDGRoom()
        {
        }


        /**
         * Add new rooms to the database
         */
        public void addRoom(List<Room> newList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < newList.Count; i++)
                {
                    createRoom(conn, newList[i]);
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
         * Update rooms of the database
         */
        public void updateRoom(List<Room> updateList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < updateList.Count; i++)
                {
                    updateRoom(conn, updateList[i]);
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
         * Delete room(s) from the database
         */
        public void deleteRoom(List<Room> deleteList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many reservations
            try
            {
                conn.Open();
                for (int i = 0; i < deleteList.Count; i++)
                {
                    removeRoom(conn, deleteList[i]);
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
         * Returns a record for the room given its roomID
         */
        public Object[] get(int roomID)
        {

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + " = " + roomID;
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
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
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
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE 1;";
            MySqlDataReader reader = null;

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
                    attributes[0] = reader[0]; // roomID
                    attributes[1] = reader[1]; // roomNum
                    records.Add((int)reader[0], attributes);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                conn.Close();
            }
            
            return records;
        }

        /**
         * Adds one room to the database
         */
        private void createRoom(MySqlConnection conn, Room room)
        {
            if (room == null)
                return;

            String commandLine = "INSERT INTO " + TABLE_NAME + " VALUES (" + room.roomID + ",'" + room.roomNum + "');";
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
                reader.Close();
            }
        }

        /**
         * Updates one room of the database
         */
        private void updateRoom(MySqlConnection conn, Room room)
        {
            if (room == null)
                return;

            String commandLine = "UPDATE " + TABLE_NAME + " SET " + FIELDS[1] + "= '" + room.roomNum + "' WHERE " + FIELDS[0] + " = " + room.roomID + ";";
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
                reader.Close();
            }
        }

        /**
         * Removes one room from the database
         */
        private void removeRoom(MySqlConnection conn, Room room)
        {
            if (room == null)
                return;

            String commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + "=" + room.roomID + ";";
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

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT MAX(" + FIELDS[0] + ") FROM " + TABLE_NAME;
            MySqlDataReader reader = null;

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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                success = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                conn.Close();
            }

            // return the last id
            if (success)
                return lastID;
            else
                return -2;
        }
    }
}
