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
     * Returns the instance of ReservationIdentityMap
     * */

    class ReservationIdentityMap
    {


        private static ReservationIdentityMap instance = new ReservationIdentityMap();
        private Dictionary<int, Reservation> reservationList_ActiveMemory = new Dictionary<int, Reservation>();
        private ReservationIdentityMap() { }

        public static ReservationIdentityMap getInstance()
        {
            return instance;
        }



        /**
         * Adds a reservation object to the dictionary representing all reservations in the active memory
         */
        public void addTo(Reservation reservation)
        {
            reservationList_ActiveMemory.Add(reservation.reservationID, reservation);
        }

        public void removeFrom(Reservation reservation)
        {
            reservationList_ActiveMemory.Remove(reservation.reservationID);
        }


        /*
         * Finds and return a reservation based on its id from the ative memory
         * */

        public Reservation find(int reservationID)
        {
            Reservation reservation;
            if (reservationList_ActiveMemory.TryGetValue(reservationID, out reservation))
            {
                return reservation;
            }
            return null;
        }


        /**
       * Finds all rooms that are currently in the active memory
       * */

        public Dictionary<int, Reservation> findAll()
        {

            //Create a new dictionary to be returned

            Dictionary<int, Reservation> newDictionary = new Dictionary<int, Reservation>();

            //Copy each key value pairs (do not need to deep copy the value, reservation).
            //We simply want to not return the reference to the dictionary used here.

            foreach (KeyValuePair<int, Reservation> pair in this.reservationList_ActiveMemory)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }

            return newDictionary;

        }

        /**
        * Finds all reservation IDs associated with the unique combination of user ID and date
        * */

        public List<int> findReservationIDs(int userID, DateTime date)
        {
            //instantiate a new list to be returned
            List<int> IDlist = new List<int>();
            foreach (KeyValuePair<int, Reservation> pair in reservationList_ActiveMemory)
            {
                if (pair.Value.userID.Equals(userID) && pair.Value.date.Equals(date.Date.ToString("yyyy-MM-dd")))
                {
                    //add the found IDs to the list
                    IDlist.Add(pair.Value.reservationID);
                }
            }
            //return list
            return IDlist;
        }



    }
}