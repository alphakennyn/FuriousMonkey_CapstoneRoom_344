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
     * Class: TDGEquipment
     * 
     * Table data gateway of Equipment table
     * 
     * This class acts as a bridge from the software application to the database.
     * It allows to create, update, delete and find data from the room database table.
     */

    public class TDGEquipment
    {
        // This instance
        private static TDGEquipment instance = new TDGEquipment();

        // Table name
        private const String TABLE_NAME = "equipment";

        // Field names of the table
        private readonly String[] FIELDS = { "equipmentID", "equipmentName"};

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
        public static TDGEquipment getInstance()
        {
            return instance;
        }

        /**
         * Default constructor
         */
        private TDGEquipment()
        {
        }


        /**
         * Add new equipments to the database
         */
        public void addEquipment(List<Equipment> newList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many equipmentss
            try
            {
                conn.Open();
                for (int i = 0; i < newList.Count; i++)
                {
                    createEquipment(conn, newList[i]);
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
         * Update equipments of the database
         */
        public void updateEquipment(List<Equipment> updateList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many equipments
            try
            {
                conn.Open();
                for (int i = 0; i < updateList.Count; i++)
                {
                    updateEquipment(conn, updateList[i]);
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
         * Delete equipment(s) from the database
         */
        public void deleteEquipment(List<Equipment> deleteList)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Attempt to open the connection and create many equipments
            try
            {
                conn.Open();
                for (int i = 0; i < deleteList.Count; i++)
                {
                    removeEquipment(conn, deleteList[i]);
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
         * Returns a record for the equipment given its equipmentID
         */
        public Object[] get(int equipmentID)
        {

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + " = " + equipmentID;
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
            catch (Exception ex)
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


        public Object[] getReservationIDs(int equipmentID)
        {

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT reservationID FROM reservationidlist WHERE " + FIELDS[0] + " = " + equipmentID;
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
                record = new Object[1];
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }
                    record[0] = reader[0];
                }
            }
            catch (Exception ex)
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

        public Object[] getEquipmentIDs(int reservationID)
        {

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String commandLine = "SELECT DISTINCT equipmentID FROM reservationidlist WHERE "+reservationID+ " = reservationID";
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
                record = new Object[1];
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }
                    record[0] = reader[0];
                }
            }
            catch (Exception ex)
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
                    attributes[0] = reader[0]; // equipmentID                                               
                    attributes[1] = reader[1]; // equipmentName
                    attributes[2] = reader[2]; //reservationID
                    records.Add((int)reader[0], attributes);
                }
            }
            catch (Exception ex)
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
         * Adds one equipment to the database
         */
        private void createEquipment(MySqlConnection conn, Equipment equipment)
        {
            if (equipment == null)
                return;

            String commandLine = "INSERT INTO " + TABLE_NAME + " VALUES (" + equipment.equipmentID + ",'" + equipment.equipmentName + "');";
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        /**
         * Updates one equipment of the database
         */
        private void updateEquipment(MySqlConnection conn, Equipment equipment)
        {
            if (equipment == null)
                return;

            String commandLine = "UPDATE " + TABLE_NAME + " SET " + FIELDS[1] + "= '" + equipment.equipmentName + "' WHERE " + FIELDS[0] + " = " + equipment.equipmentID + ";";
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            MySqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        /**
         * Removes one equipment from the database
         */
        private void removeEquipment(MySqlConnection conn, Equipment equipment)
        {
            if (equipment == null)
                return;

            String commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + "=" + equipment.equipmentID + ";";
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            MySqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
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
            catch (Exception e)
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

        public Dictionary<int, Object[]> findAvailableEquipment(DateTime date, int firstHour,int lastHour, List<string> equipmentNameList)
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
                    attributes[0] = reader[0]; // equipmentID                                               
                    attributes[1] = reader[1]; // equipmentName
                    attributes[2] = reader[2]; //reservationID
                    records.Add((int)reader[0], attributes);
                }
            }
            catch (Exception ex)
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
    }
}