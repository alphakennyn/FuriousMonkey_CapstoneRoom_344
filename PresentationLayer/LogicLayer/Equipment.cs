using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Equipment
    {
        public int equipmentID { get; set; }
        public string equipmentName { get; set; }
        public List<Reservation> equipmentReservations { get; set; }
        public List<int> equipmentWaitList { get; set; }

        public Equipment()
        {
            equipmentID = 0;
            equipmentName = "";
            equipmentReservations = new List<Reservation>();
        }

        public Equipment(int equipmentID, string equipmentName)
        {
            this.equipmentID = equipmentID;
            this.equipmentName = equipmentName;
            this.equipmentReservations = new List<Reservation>();
        }
    }
}