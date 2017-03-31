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
        public int reservationID { get; set; }
        public string equipmentName { get; set; }
        
        public Queue<int> equipmentWaitList { get; set; }

        public Equipment()
        {
            equipmentID = 0;
            equipmentName = "";
            reservationID = 0;
        }

        public Equipment(int equipmentID, int resID,  string equipmentName)
        {
            this.equipmentID = equipmentID;
            this.equipmentName = equipmentName;
            this.reservationID = resID;
        }
    }
}