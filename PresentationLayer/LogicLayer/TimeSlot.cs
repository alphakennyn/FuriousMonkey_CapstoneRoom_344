using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class TimeSlot
    {
        public int timeSlotID { get; set; }
        public int reservationID { get; set; }
        public int hour { get; set; }
        public Queue<int> waitlist { get; set; }

        public TimeSlot()
        {
            timeSlotID = 0;
            reservationID = 0;
            hour = 0;
            waitlist = new Queue<int>();
        }

        public TimeSlot(int timeSlotID, int resID, int hour)
        {
            this.timeSlotID = timeSlotID;
            this.reservationID = resID;
            this.hour = hour;
            this.waitlist = new Queue<int>();
        }
    }
}
