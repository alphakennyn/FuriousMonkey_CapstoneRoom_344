using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class DirectoryOfTimeSlots
    {

        private static DirectoryOfTimeSlots instance = new DirectoryOfTimeSlots();

        public List<TimeSlot> timeSlotList { get; set; }

        // Constructor
        private DirectoryOfTimeSlots()
        {
            timeSlotList = new List<TimeSlot>();
        }

        // Get instance
        public static DirectoryOfTimeSlots getInstance()
        {
            return instance;
        }

        // Method to make a new time slot
        public TimeSlot makeNewTimeSlot(int timeslotID, int reservationID, int hour)
        {
            TimeSlot timeSlot = new TimeSlot(timeslotID, reservationID, hour);
            if (!timeSlotList.Contains(timeSlot))
                timeSlotList.Add(timeSlot);
            return timeSlot;
        }

        // Method to modify a time slot
        public TimeSlot modifyTimeSlot(int timeSlotID, int resID, Queue<int> wlist)
        {
            for (int i = 0; i < timeSlotList.Count; i++ )
            {
                if (timeSlotList[i].timeSlotID == timeSlotID)
                {
                    timeSlotList[i].reservationID = resID;
                    timeSlotList[i].timeSlotID = timeSlotID;
                    timeSlotList[i].waitlist = wlist;
                    return timeSlotList[i];
                }
            }
            return null;
        }

        // Method to delete a timeslot
        public void deleteTimeSlot(int timeSlotID)
        {
            foreach (TimeSlot timeSlot in timeSlotList)
                if (timeSlot.timeSlotID == timeSlotID)
                {
                    timeSlotList.Remove(timeSlot);
                    return;
                }
        }

    }
}
