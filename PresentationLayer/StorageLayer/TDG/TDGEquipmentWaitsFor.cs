using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LogicLayer;

namespace TDG
{

    //need to modify equipment

    //table IMPORTANT

    class TDGEquipmentWaitsFor
    {
        // This instance
        private const String TABLE_NAME = "equipmentwaitsfor";

        // Table name
        private static TDGEquipmentWaitsFor instance = new TDGEquipmentWaitsFor();

        // Field names of the table
        private static readonly String[] FIELDS = { "equipmentName", "userID", "dateTime", "firstHour", "lastHour","roomID" };

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
        public static TDGEquipmentWaitsFor getInstance()
        {
            return instance;
        }

        /**
         * Default constructor, private for Singleton
         */
        private TDGEquipmentWaitsFor()
        {
        }

        /**
         * Select all data from the table
         * Returns it as a List<int>
         * Where int is the ID of the object and Object[] contains the record of the row
         */
        public List<Object[]> getAll()
        {
            List<Object[]> records = new List<Object[]>();
            String commandLine = "SELECT * FROM " + TABLE_NAME + " WHERE 1;";
            MySqlDataReader reader = null;
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

            // Open connection
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                reader = cmd.ExecuteReader();

                // If no record is found, return empty list
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return records;
                }

                // For each reader, add it to the list
                while (reader.Read())
                {
                    if (reader[0].GetType() == typeof(System.DBNull))
                    {
                        reader.Close();
                        conn.Close();
                        return records;
                    }
                    Object[] attributes = new Object[FIELDS.Length];
                    attributes[0] = reader[0]; //equipmentName
                    attributes[1] = reader[1]; //userID
                    attributes[2] = reader[2]; //date
                    attributes[3] = reader[3]; //firstHour
                    attributes[4] = reader[4]; //lastHour
                    attributes[5] = reader[5]; //roomID

                    records.Add(attributes);
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

            // If successful, return the list
            return records;
        }

        public void refreshEquipmentWaitsFor(List<Equipment> listOfEquipment)
        {

            foreach (Equipment equipment in listOfEquipment)
            {
                // The list is not empty, refresh the content of the database
                if (equipment.equipmentWaitList.Count() != 0)
                {
                    // Obtain all queuery for that equipment from the database
                    String commandLine = "SELECT " + FIELDS[1] + " FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + "=" + equipment.equipmentName;
                    MySqlDataReader reader = null;
                    MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);

                    // Try to exeucte the command and read
                    try
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                        reader = cmd.ExecuteReader();

                        // Store the results
                        List<int> results = new List<int>(); // Will store the results found after querying the database
                        while (reader.Read())
                        {
                            results.Add((int)reader[0]); // Selecting only the userID
                        }
                        reader.Close();

                        // Get the waitlist of the equipment that is refreshed
                        Queue<int>  equipmentWaitlist = equipment.equipmentWaitList;

                        // If a userID is found in the DB but not in the waitlist: remove from the DB
                        foreach (int userID in results)
                        {
                            if (!equipmentWaitlist.Contains(userID))
                            {
                                deleteWaitsFor(conn, equipment.equipmentID, userID);
                            }
                        }

                        // If a userID is found in the waitlist but not in the DB: add it to the DB
                        foreach (int userID in equipmentWaitlist)
                        {
                            if (!results.Contains(userID))
                            {
                                String currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                createEquipmentWaitsFor(conn, equipment.equipmentName, userID, currentDateTime, 0, 1,1);
                            }
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

                }
                // If the queue is empty, ensure it is empty by deleting all rows that have that equipment id
                else
                {
                    String commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + " = " + equipment.equipmentID;
                    MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
                    MySqlDataReader reader = null;

                    try
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(commandLine, conn);
                        reader = cmd.ExecuteReader();
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
                }
            }
        }

        public void addEquipmentWaitsFor(string equipmentName, int userID, DateTime currentDateTime, int firstHour, int lastHour,int roomID)
        {

            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            String stringDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            // Attempt to open the connection and create many reservations
            try
            {
                conn.Open();
                    createEquipmentWaitsFor(conn, equipmentName, userID, stringDateTime, firstHour, lastHour,roomID);
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

        private void createEquipmentWaitsFor(MySqlConnection conn, string equipmentName, int userID, string currentDateTime, int firstHour, int lastHour, int roomID)
        {
            String commandLine = "INSERT INTO " + TABLE_NAME + " VALUES ( '" + equipmentName + "'," + userID + ", '" + currentDateTime + "'," + firstHour + "," + lastHour +","+roomID +");";
            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
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
                if(reader != null)
                    reader.Close();
            }
        }
        private void deleteWaitsFor(MySqlConnection conn, int equipmentName, int userID)
        {
            String commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[0] + "=" + equipmentName + " AND " + FIELDS[1] + " = " + userID;
            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if(reader!=null)
                    reader.Close();
            }
        }
        public void deleteEquipmentWaitsFor(int userID)
        {
            MySqlConnection conn = new MySqlConnection(DATABASE_CONNECTION_STRING);
            string commandLine = "DELETE FROM " + TABLE_NAME + " WHERE " + FIELDS[1] + "=" + userID;
            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand(commandLine, conn);
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
