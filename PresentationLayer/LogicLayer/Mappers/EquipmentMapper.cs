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

        public void addEquipment(List<Equipment> newList)
        {
            tdgEquipment.addEquipment(newList);
        }

        // For Unit of Work: A list of timeslots to be updated to the DB is passed to the TDG. 
        public void updateTimeSlot(List<Equipment> updateList)
        {
            tdgEquipment.updateEquipment(updateList);
        }

        //For Unit of Work : A list of timeslots to be deleted in the DB is passes to the TDG.
        public void deleteEquipment(List<Equipment> deleteList)
        {
            tdgEquipment.deleteEquipment(deleteList);
        }

        public Dictionary<int, Equipment> getAllEquipment()
        {
            //Get all equipment from the equipment Identity Map.
            Dictionary<int, Equipment> equipmentDic = equipmentIdentityMap.findAll();

            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgEquipment.getAll();

            // If it's empty, simply return those from the identity map
            if (result == null)
            {
                return equipmentDic;
            }

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                //The timeSlot is not in the Time Slot identity map.
                //Create an instance, add it to the equipment indentity map and to the return variable
                if (!equipmentDic.ContainsKey(record.Key))
                {
                    Equipment equipment = DirectoryOfEquipment.getInstance().makeNewEquipment((int)record.Key, (List<int>)record.Value[1], (string)record.Value[2].ToString());

                    // Add to IdentityMap
                    equipmentIdentityMap.addTo(equipment);

                    equipmentDic.Add(equipment.equipmentID, equipment);
                }
            }
            return equipmentDic;
        }

        /**
         * Retrieve all timeslots
         * */


        /**
        * Initialize the list of time slots, used for instantiating console
        * */
        public void initializeDirectory()
        {
            //Get all timeslots in the DB
            Dictionary<int, Object[]> result = tdgEquipment.getAll();

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                Equipment equipment = DirectoryOfEquipment.getInstance().makeNewEquipment((int)record.Key, (List<int>)record.Value[2], (string)record.Value[1]);

                // Add to IdentityMap
                equipmentIdentityMap.addTo(equipment);
            }
        }

        public Equipment getEquipment(int equipmentID)
        {

            //Try to obtain the TimeSlot from the TimeSlot identity map
            Equipment equipment = equipmentIdentityMap.find(equipmentID);
            Object[] result = null;
            List<int> reservationIDs = null;

            if (equipment == null)
            {
                //If not found in TimeSlot identity map then, it uses TDG to try to retrieve from DB.
                result = tdgEquipment.get(equipmentID);

                reservationIDs = tdgEquipment.getReservationIDs(equipmentID);

                if (result != null)
                {
                    //The TimeSlot object was obtained from the TDG (and from the DB)
                    //Instantiate the object by passing values to parameters
                    equipment=DirectoryOfEquipment.getInstance().makeNewEquipment((int)result[0],reservationIDs,(string)result[1]);

                    //Add TimeSlot to the TimeSlot IdentityMap
                    EquipmentIdentityMap.getInstance().addTo(equipment);
                }
            }
            //Null is returned if it is not found in the TimeSlot identity map NOR in the DB
            return equipment;
        }

        /**
         * Done: commit
         * When it is time to commit, UoW writes changes to the DB
         * */

        public void done()
        {
            UnitOfWork.getInstance().commit();
        }

      

        /**
         * Retrieve the total number of hours associated with given reservation IDs
         */


        public List<Equipment> getListOfEquipment()
        {
            return (DirectoryOfEquipment.getInstance().equipmentList);
        }

        public void setEquipment(int equipmentID, List<int> reservationIDList, Queue<int> equipmentWaitList)
        {
            // Update the timeslot
            Equipment equipment = DirectoryOfEquipment.getInstance().modifyEquipment(equipmentID, reservationIDList, equipmentWaitList);

            // Register it to the unit of work
            UnitOfWork.getInstance().registerDirty(equipment);
        }
    }
}