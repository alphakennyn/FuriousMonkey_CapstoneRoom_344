using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Room
    {
        public int roomID { get; set; }
        public string roomNum { get; set; }
        public List<Reservation> roomReservations { get; set; }

        public Room()
        {
            roomID = 0;
            roomNum = "";
            roomReservations = new List<Reservation>();
        }

        public Room(int roomID, string roomNum)
        {
            this.roomID = roomID;
            this.roomNum = roomNum;
            this.roomReservations = new List<Reservation>();
        }

    }
}
