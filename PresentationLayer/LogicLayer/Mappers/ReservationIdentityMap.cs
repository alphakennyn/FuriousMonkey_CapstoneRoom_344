using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageLayer;


namespace LogicLayer
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
            reservationList_ActiveMemory.Add(reservation.getReservationID(), reservation);
        }

        public void removeFrom(Reservation reservation)
        {
            reservationList_ActiveMemory.Remove(reservation.getReservationID());
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

    }
}
