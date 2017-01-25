using System.Collections.Generic;
using LogicLayer;
﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using StorageLayer;

namespace CapstoneRoomScheduler.LogicLayer.IdentityMaps
{
    /*
     * Returns the instance of TimeSlotIdentityMap
     * */
    class TimeSlotIdentityMap
    {
        private static TimeSlotIdentityMap instance = new TimeSlotIdentityMap();
        private Dictionary<int, TimeSlot> timeSlotList_ActiveMemory = new Dictionary<int, TimeSlot>();
        private TimeSlotIdentityMap() { }

        public static TimeSlotIdentityMap getInstance()
        {
            return instance;
        }


        /**
         * Adds a time slot object to the dictionary representing all reservations in the active memory
         */
        public void addTo(TimeSlot timeslot)
        {
            timeSlotList_ActiveMemory.Add(timeslot.timeSlotID, timeslot);
        }

        public void removeFrom(TimeSlot timeslot)
        {
            timeSlotList_ActiveMemory.Remove(timeslot.timeSlotID);
        }


        /*
         * Finds and return a time slot based on its id from the active memory
         * */

        public TimeSlot find(int timeSlotID)
        {
            TimeSlot timeslot;
            if (timeSlotList_ActiveMemory.TryGetValue(timeSlotID, out timeslot))
            {
                return timeslot;
            }
            return null;
        }


        /**
         * Finds all timeslots that are currently in the active memory
         * */

        public Dictionary<int, TimeSlot> findAll()
        {
            //Create a new dictionary to be returned
            Dictionary<int, TimeSlot> newDictionary = new Dictionary<int, TimeSlot>();

            //Copy each key value pairs (do not need to deep copy the value, timeslot).
            //We simply want to not return the reference to the dictionary used here.
            foreach (KeyValuePair<int, TimeSlot> pair in this.timeSlotList_ActiveMemory)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }
            return newDictionary;
        }

        /**
        * Finds all timeslots that are currently in the active memory that has a specific timeslot ID
        * */

        public Dictionary<int, TimeSlot> findAll(int reservationID)
        {

            //Create a new dictionary to be returned
            Dictionary<int, TimeSlot> newDictionary = new Dictionary<int, TimeSlot>();

            //Copy each key value pairs (do not need to deep copy the value, timeslot).
            //We simply want to not return the reference to the dictionary used here.
            foreach (KeyValuePair<int, TimeSlot> pair in this.timeSlotList_ActiveMemory)
            {
                if ((pair.Value).reservationID == reservationID)
                    newDictionary.Add(pair.Key, pair.Value);
            }

            return newDictionary;

        }

        /**
         * Finds all hours associated with reservation IDs provided and sums them up, that are currently in the active memory
         * */

        public int findTotalHours(List<int> IDlist)
        {
            int hours = 0;
            foreach (KeyValuePair<int, TimeSlot> pair in timeSlotList_ActiveMemory)
            {
                foreach (int reservationID in IDlist)
                {
                    if (pair.Value.reservationID.Equals(reservationID))
                    {
                        hours ++;
                    }
                }
            }
            return hours;
        }


    }
}