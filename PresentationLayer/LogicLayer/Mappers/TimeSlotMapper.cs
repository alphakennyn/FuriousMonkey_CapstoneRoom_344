using System;
using System.Collections.Generic;
using TDG;
using LogicLayer;
using CapstoneRoomScheduler.LogicLayer.IdentityMaps;
using System.Threading;

namespace Mappers
{
    class TimeSlotMapper
    {

        //Instance of this mapper object
        private static TimeSlotMapper instance = new TimeSlotMapper();

        private TDGTimeSlot tdgTimeSlot = TDGTimeSlot.getInstance();
        private TimeSlotIdentityMap timeSlotIdentityMap = TimeSlotIdentityMap.getInstance();
        private WaitsForMapper waitsForMapper = WaitsForMapper.getInstance();

        // The last ID that is used
        private int lastID;

        // Lock to modify last ID
        private readonly Object lockLastID = new Object();

        //default constructor
        private TimeSlotMapper()
        {
            this.lastID = tdgTimeSlot.getLastID();

        }

        // Get instance
        public static TimeSlotMapper getInstance()
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

            lock(this.lockLastID)
            {
                this.lastID++;
                nextID = this.lastID;
            }
            return nextID;
        }

        /**
         * Handles the creation of a new object of type TimeSlot.
         **/
        public TimeSlot makeNew(int reservationID, int hour)
        {
            // TimeSlot ID
            int timeslotID = getNextID();

            // Make a new time slot
            TimeSlot timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot(timeslotID, reservationID, hour);

            //Add new TimeSlot object to the identity map
            timeSlotIdentityMap.addTo(timeSlot);

            //Add TimeSlot object to UoW
            UnitOfWork.getInstance().registerNew(timeSlot);

            return timeSlot;
        }

        /**
         * Retrieve a TimeSlot given its TimeSlot ID.
         */

        public TimeSlot getTimeSlot(int timeSlotID)
        {

            //Try to obtain the TimeSlot from the TimeSlot identity map
            TimeSlot timeslot = timeSlotIdentityMap.find(timeSlotID);
            Object[] result = null;

            if (timeslot == null)
            {
                //If not found in TimeSlot identity map then, it uses TDG to try to retrieve from DB.
                result = tdgTimeSlot.get(timeSlotID);

                if (result != null)
                {
                    //The TimeSlot object was obtained from the TDG (and from the DB)
                    //Instantiate the object by passing values to parameters
                    DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)result[0], (int)result[1], (int)result[2]);
                    
                    //Add TimeSlot to the TimeSlot IdentityMap
                    timeSlotIdentityMap.addTo(timeslot);
                }
            }
            //Null is returned if it is not found in the TimeSlot identity map NOR in the DB
            return timeslot;
        }

        /**
         * Retrieve all timeslots
         * */
        public Dictionary<int, TimeSlot> getAllTimeSlot()
        {
            //Get all timeslots from the Time Slot Identity Map.
            Dictionary<int, TimeSlot> timeslots = timeSlotIdentityMap.findAll();

            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgTimeSlot.getAllTimeSlot();

            // If it's empty, simply return those from the identity map
            if(result == null)
            {
                return timeslots;
            }

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                //The timeSlot is not in the Time Slot identity map.
                //Create an instance, add it to the Time Slot indentity map and to the return variable
                if (!timeslots.ContainsKey(record.Key))
                {
                    TimeSlot timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);

                    // Add to IdentityMap
                    timeSlotIdentityMap.addTo(timeSlot);

                    timeslots.Add(timeSlot.timeSlotID, timeSlot);
                }
            }
            return timeslots;
        }

        /**
        * Initialize the list of time slots, used for instantiating console
        * */
        public void initializeDirectory()
        {
            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgTimeSlot.getAllTimeSlot();

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                TimeSlot timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);

                // Add to IdentityMap
                timeSlotIdentityMap.addTo(timeSlot);
            }
        }

        /**
         * Retrieve all timeslots that have the same Reservation ID
         * */
        public Dictionary<int, TimeSlot> getAllTimeSlot(int reservationID)
        {
            //Get all timeslots from the Time Slot Identity Map.
            Dictionary<int, TimeSlot> timeslots = timeSlotIdentityMap.findAll(reservationID);

            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgTimeSlot.getAllTimeSlot(reservationID);

            // If it's empty, simply return those from the identity map
            if (result == null)
            {
                return timeslots;
            }

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                //The timeslot is not in the Time Slot identity map.
                //Create an instance, add it to the Time Slot identity map and to the return variable
                if (!timeslots.ContainsKey(record.Key))
                {
                    TimeSlot timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);
                    timeSlotIdentityMap.addTo(timeSlot);
                    timeslots.Add(timeSlot.timeSlotID, timeSlot);
                }
            }
            return timeslots;
        }

        /**
         * Return all users waiting for a TimeSlot given the
         * TimeSlot ID.
         */
        public List<int> getAllUsers(int timeSlotID)
        {
            return waitsForMapper.getAllUsers(timeSlotID);
        }

        /**
         * Set time slot attributes
         */
        public void setTimeSlot(int timeSlotID, int reservationID, Queue<int> waitList)
        {
            // Update the timeslot
            TimeSlot timeSlot = DirectoryOfTimeSlots.getInstance().modifyTimeSlot(timeSlotID, reservationID, waitList);

            // Register it to the unit of work
            UnitOfWork.getInstance().registerDirty(timeSlot); 
        }

        /**
         * Delete timeslot
         * */
        public void delete(int timeSlotID)
        {
            //Get the timeslot to be deleted by checking the identity map
            TimeSlot timeSlot = timeSlotIdentityMap.find(timeSlotID);

            //If TimeSlot IdentityMap returned the object, remove it from identity map
            if (timeSlot != null)
            {
                timeSlotIdentityMap.removeFrom(timeSlot);
            }
            else
            {
                tdgTimeSlot.get(timeSlotID);
            }

            DirectoryOfTimeSlots.getInstance().deleteTimeSlot(timeSlotID);

            //Register as deleted in the Unit Of Work
            UnitOfWork.getInstance().registerDeleted(timeSlot);

        }

        /**
         * Done: commit
         * When it is time to commit, UoW writes changes to the DB
         * */

        public void done()
        {
            UnitOfWork.getInstance().commit();
        }

        //For Unit of Work: A list of timeslots to be added to the DB is passed to the TDG. 
        public void addTimeSlot(List<TimeSlot> newList)
        {
            tdgTimeSlot.addTimeSlot(newList);
            waitsForMapper.refreshWaitsFor(newList);
        }

        // For Unit of Work: A list of timeslots to be updated to the DB is passed to the TDG. 
        public void updateTimeSlot(List<TimeSlot> updateList)
        {
            tdgTimeSlot.updateTimeSlot(updateList);
            waitsForMapper.refreshWaitsFor(updateList);
        }

        //For Unit of Work : A list of timeslots to be deleted in the DB is passes to the TDG.
        public void deleteTimeSlot(List<TimeSlot> deleteList)
        {
            tdgTimeSlot.deleteTimeSlot(deleteList);
        }


        /**
         * Retrieve the total number of hours associated with given reservation IDs
         */

        public int findHoursByReservationID(List<int> IDlist)
        {
            int hours = 0;
            hours = TimeSlotIdentityMap.getInstance().findTotalHours(IDlist);

            if (hours == 0)
            {
                hours = tdgTimeSlot.getTotalHoursforReservationID(IDlist);
            }

            return hours;
        }

        public List<TimeSlot> getListOfTimeSlots()
        {
            return (DirectoryOfTimeSlots.getInstance().timeSlotList);
        }

    }
}