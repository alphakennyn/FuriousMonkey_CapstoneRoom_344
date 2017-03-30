using System;
using System.Collections.Generic;
using TDG;
using LogicLayer;
using CapstoneRoomScheduler.LogicLayer.IdentityMaps;
using System.Threading;

namespace Mappers
{
    class EquipmentMapper
    {

        //Instance of this mapper object
        private static EquipmentMapper instance = new EquipmentMapper();

        private TDGEquipment tdgEquipment = TDGEquipment.getInstance();
        private EquipmentIdentityMap equipmentIdentityMap = EquipmentIdentityMap.getInstance();
        private WaitsForMapper equipmentWaitsForMapper = WaitsForMapper.getInstance();

        // The last ID that is used
        private int lastID;

        // Lock to modify last ID
        private readonly Object lockLastID = new Object();

        //default constructor
        private EquipmentMapper()
        {
            this.lastID = tdgEquipment.getLastID();

        }

    // Get instance
    public static EquipmentMapper getInstance()
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
        public Equipment makeNew(int reservationID, int hour)
        {
            // TimeSlot ID
            int timeslotID = getNextID();

            // Make a new time slot
            Equipment timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot(timeslotID, reservationID, hour);

            //Add new TimeSlot object to the identity map
            timeSlotIdentityMap.addTo(timeSlot);

            //Add TimeSlot object to UoW
            UnitOfWork.getInstance().registerNew(timeSlot);

            return timeSlot;
        }

        /**
         * Retrieve a TimeSlot given its TimeSlot ID.
         */

        public Equipment getEquipment(int equipmentID)
        {

            //Try to obtain the TimeSlot from the TimeSlot identity map
            Equipment equipment = equipmentIdentityMap.find(equipmentID);
            Object[] result = null;

            if (equipment == null)
            {
                //If not found in TimeSlot identity map then, it uses TDG to try to retrieve from DB.
                result = tdgEquipment.get(equipmentID);

                if (result != null)
                {
                    //The TimeSlot object was obtained from the TDG (and from the DB)
                    //Instantiate the object by passing values to parameters
                    DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)result[0], (int)result[1], (int)result[2]);

                    //Add TimeSlot to the TimeSlot IdentityMap
                    equipmentIdentityMap.addTo(equipment);
                }
            }
            //Null is returned if it is not found in the TimeSlot identity map NOR in the DB
            return equipment;
        }

        /**
         * Retrieve all timeslots
         * */
        public Dictionary<int, Equipment> getAllTimeSlot()
        {
            //Get all timeslots from the Time Slot Identity Map.
            Dictionary<int, Equipment> timeslots = equipmentIdentityMap.findAll();

            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgEquipment.getAllTimeSlot();

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
                    Equipment timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);

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
                Equipment timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);

                // Add to IdentityMap
                timeSlotIdentityMap.addTo(timeSlot);
            }
        }

        /**
         * Retrieve all timeslots that have the same Reservation ID
         * */
        public Dictionary<int, Equipment> getAllTimeSlot(int reservationID)
        {
            //Get all timeslots from the Time Slot Identity Map.
            Dictionary<int, Equipment> timeslots = timeSlotIdentityMap.findAll(reservationID);

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
                    Equipment timeSlot = DirectoryOfTimeSlots.getInstance().makeNewTimeSlot((int)record.Key, (int)record.Value[1], (int)record.Value[2]);
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
            return equipmentWaitsForMapper.getAllUsers(timeSlotID);
        }

        /**
         * Set time slot attributes
         */
        public void setTimeSlot(int timeSlotID, int reservationID, Queue<int> waitList)
        {
            // Update the timeslot
            Equipment timeSlot = DirectoryOfTimeSlots.getInstance().modifyTimeSlot(timeSlotID, reservationID, waitList);

            // Register it to the unit of work
            UnitOfWork.getInstance().registerDirty(timeSlot); 
        }

        /**
         * Delete timeslot
         * */
        public void delete(int timeSlotID)
        {
            //Get the timeslot to be deleted by checking the identity map
            Equipment timeSlot = equipmentIdentityMap.find(timeSlotID);

            //If TimeSlot IdentityMap returned the object, remove it from identity map
            if (timeSlot != null)
            {
                equipmentIdentityMap.removeFrom(timeSlot);
            }
            else
            {
                tdgEquipment.get(timeSlotID);
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
        public void addTimeSlot(List<Equipment> newList)
        {
            tdgEquipment.addEquipment(newList);
            equipmentWaitsForMapper.refreshWaitsFor(newList);
        }

        // For Unit of Work: A list of timeslots to be updated to the DB is passed to the TDG. 
        public void updateTimeSlot(List<Equipment> updateList)
        {
            tdgEquipment.updateEquipment(updateList);
            equipmentWaitsForMapper.refreshWaitsFor(updateList);
        }

        //For Unit of Work : A list of timeslots to be deleted in the DB is passes to the TDG.
        public void deleteTimeSlot(List<Equipment> deleteList)
        {
            tdgEquipment.deleteEquipment(deleteList);
        }


        /**
         * Retrieve the total number of hours associated with given reservation IDs
         */

        

        public List<Equipment> getListOfEquipment()
        {
            return (DirectoryOfEquipment.getInstance().equipmentList);
        }

    }
}