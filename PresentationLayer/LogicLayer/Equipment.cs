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
        public List<int> reservationIDList { get; set; }
        public string equipmentName { get; set; }
        
        public Queue<int> equipmentWaitList { get; set; }

        public Equipment()
        {
            equipmentID = 0;
            equipmentName = "computer";
            reservationIDList = new List<int>();
        }

        public Equipment(int equipmentID, List<int> resIDList,  string equipmentName)
        {
            this.equipmentID = equipmentID;
            this.equipmentName = equipmentName;
            this.reservationIDList = resIDList;
        }

        public void addReservationID(int reservationID)
        {
            this.reservationIDList.Add(reservationID);
        }

        //Add method to remove reservation ID
    }
}