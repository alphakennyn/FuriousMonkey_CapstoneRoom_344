using System;
using System.Collections.Generic;
using TDG;
using LogicLayer;
using CapstoneRoomScheduler.LogicLayer.IdentityMaps;
using System.Threading;

namespace Mappers
{
    class ReservationMapper
    {

        //Instance of this mapper object
        private static ReservationMapper instance = new ReservationMapper();

        private TDGReservation tdgReservation = TDGReservation.getInstance();
        private ReservationIdentityMap reservationIdentityMap = ReservationIdentityMap.getInstance();

        // The last ID that is used
        private int lastID;

        // Lock to modify last ID
        private readonly Object lockLastID = new Object();

        // Default constructor
        private ReservationMapper()
        {
            this.lastID = tdgReservation.getLastID();
        }

        public static ReservationMapper getInstance()
        {
            return instance;
        }

        /**
         *  Obtain the next ID available
         **/
        private int getNextID()
        {
            // Increments the last ID atomically, return the increment value
            int nextID;

            lock (this.lockLastID)
            {
                this.lastID++;
                nextID = this.lastID;
            }
            return nextID;
        }

        /**
         * Handles the creation of a new object of type Reservation.
         **/

        public Reservation makeNew(int userID, int roomID, string desc, DateTime date, List<int> equipmentIDList)
        {
            //Get the next reservation ID
            int reservationID = getNextID();

            //Make a new reservation object
            Reservation reservation = DirectoryOfReservations.getInstance().makeNewReservation(reservationID, userID, roomID, desc, date, equipmentIDList);

            //Add new reservation to identity map
            reservationIdentityMap.addTo(reservation);

            //Add reservation object to UoW
            UnitOfWork.getInstance().registerNew(reservation);
            return reservation; // Must return the reservation to create the time slot with the reservation ID
        }

        /**
         * Retrieve a reservation given its reservationID.
         */

        public Reservation getReservation(int reservationID)
        {
            //Try to obtain the reservation from the Reservation indentity map
            Reservation reservation = reservationIdentityMap.find(reservationID);
            Object[] result = null;

            if (reservation == null)
            {
                //If not found in Reservation identity map then, it uses TDG to try to retrieve from DB.
                result = tdgReservation.get(reservationID);

                if (result != null)
                {
                    //Reservation object was retrieved from the TDG and values obtained are passed as parameters to instantiate it
                    reservation = DirectoryOfReservations.getInstance().makeNewReservation((int)result[0], (int)result[1], (int)result[2], (String)result[3], Convert.ToDateTime(result[4]));
                    // Add it to identity map
                    reservationIdentityMap.addTo(reservation);
                }
            }
            //Null is returned if it is not found in the reservation identity map NOR in the DB
            return reservation;
        }


        /**
         * Retrieve all reservations
         * */
        public Dictionary<int, Reservation> getAllReservation()
        {
            //Get all reservations from the reservation Identity Map.
            Dictionary<int, Reservation> reservations = reservationIdentityMap.findAll();

            //Get all reservations in the DB
            Dictionary<int, Object[]> result = tdgReservation.getAll();

            // If it's empty, simply return those from the identity map
            if (result == null)
            {
                return reservations;
            }

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                //The reservation is not in the reservation identity map.
                //Create an instance, add it to the reservation identity map and to the return variable
                if (!reservations.ContainsKey(record.Key))
                {

                    Reservation reservation = DirectoryOfReservations.getInstance().makeNewReservation((int)record.Key, (int)record.Value[1], (int)record.Value[2], (string)record.Value[3], (DateTime)record.Value[4]);

                    reservationIdentityMap.addTo(reservation);
                    reservations.Add(reservation.reservationID, reservation);
                }
            }
            return reservations;
        }

        /**
         * Initialize the list of reservation, used for instantiating console
         * */
        public void initializeDirectory()
        {
            //Get all reservations in the DB
            Dictionary<int, Object[]> result = tdgReservation.getAll();

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
               //Create an instance, add it to the reservation identity map and to the return variable
               Reservation reservation = DirectoryOfReservations.getInstance().makeNewReservation((int)record.Key, (int)record.Value[1], (int)record.Value[2], (string)record.Value[3], (DateTime)record.Value[4]);
               reservationIdentityMap.addTo(reservation);
            }
        }


        /**
         * Set reservation attributes
         **/

        public void modifyReservation(int reservationID, int roomID, string desc, DateTime date)
        {
            //Get the reservation that needs to be updated
            Reservation reservation = getReservation(reservationID);

            //Update the reservation
            DirectoryOfReservations.getInstance().modifyReservation(reservationID, roomID, desc, date);

            //Register instances as Dirty in the Unit Of Work since the object has been modified.
            UnitOfWork.getInstance().registerDirty(reservation);
        }

        /**
         * Delete reservation
         * */

        public void delete(int reservationID)
        {
            //Get the reservation to be deleted by checking the identity map
            Reservation reservation = reservationIdentityMap.find(reservationID);

            //If resrvation IdentityMap returned the object, remove it from identity map
            if (reservation != null)
            {
                reservationIdentityMap.removeFrom(reservation);
            }
            else
            {
                reservation = getReservation(reservationID);
            }

            DirectoryOfReservations.getInstance().cancelReservation(reservationID);
            //Register as deleted in the Unit Of Work. 
            UnitOfWork.getInstance().registerDeleted(reservation);
        }
        /**
         * Done: commit
         * When it is time to commit, UoW writes changes to the DB
         * */
        public void done()
        {
            UnitOfWork.getInstance().commit();
        }


        //For Unit of Work: A list of reservations to be added to the DB is passed to the TDG. 
        public void addReservation(List<Reservation> newList)
        {
            tdgReservation.addReservation(newList);

        }

        //For Unit of Work: A list of reservations to be updated in the DB is passed to the TDG.
        public void updateReservation(List<Reservation> updateList)
        {
            tdgReservation.updateReservation(updateList);

        }

        //For Unit of Work : A list of reservation to be deleted in the DB is passes to the TDG.
        public void deleteReservation(List<Reservation> deleteList)
        {
            tdgReservation.deleteReservation(deleteList);
        }


        /**
       * Retrieve all resevation IDs associated with the unique userID & date
       * */
        public List<int> findReservationIDs(int userID, DateTime date)
        {
            List<int> IDlist = new List<int>();
            IDlist = ReservationIdentityMap.getInstance().findReservationIDs(userID, date);

            if (IDlist == null || IDlist.Count == 0)
            {
                IDlist = tdgReservation.getReservationIDs(userID, date);
                if (IDlist == null)
                {
                    return null;
                }
            }
            return IDlist;
        }


        public List<Reservation> getListOfReservations()
        {
            return DirectoryOfReservations.getInstance().reservationList;
        }

    }

}