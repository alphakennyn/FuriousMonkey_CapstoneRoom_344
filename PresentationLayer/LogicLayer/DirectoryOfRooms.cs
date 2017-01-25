using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class DirectoryOfRooms
    {
        private static DirectoryOfRooms instance = new DirectoryOfRooms();

        public List<Room> roomList { get; set; }

        // Constructor
        private DirectoryOfRooms()
        {
            roomList = new List<Room>();   
        }

        // Get instance
        public static DirectoryOfRooms getInstance()
        {
            return instance;
        }

        // Method to make a new room
        public Room makeNewRoom(int roomID, string roomNum)
        {
            Room r = new Room(roomID, roomNum);
            return r;
        }

        // Method to delete a room from the list
        public void deleteRoom(int roomID)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].roomID == roomID)
                {
                    roomList.Remove(roomList[i]);
                    break;
                }
            }
        }

        // Method to modify a room. Commented it out because it is not used right now.
        //The list of reservation is updated upon calling updateDirectories() in ReservationConsole
        public Room modifyRoom(int roomID, string roomNum, List<Reservation> reservations)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].roomID == roomID)
                {
                    roomList[i].roomID = roomID;
                    roomList[i].roomNum = roomNum;
                    roomList[i].roomReservations = reservations;
                    return roomList[i];
                }
            }
            return null;
        }

    }
}
