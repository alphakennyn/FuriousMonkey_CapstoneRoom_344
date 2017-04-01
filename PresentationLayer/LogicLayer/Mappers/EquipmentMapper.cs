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

        /**
         * Retrieve a TimeSlot given its TimeSlot ID.
         */

        public void addEquipment(List<Equipment> newList)
        {
            tdgEquipment.addEquipment(newList);
            EquipmentWaitsForMapper.refreshWaitsFor(newList);
        }

        // For Unit of Work: A list of timeslots to be updated to the DB is passed to the TDG. 
        public void updateTimeSlot(List<Equipment> updateList)
        {
            tdgEquipment.updateEquipment(updateList);
            EquipmentWaitsForMapper.refreshWaitsFor(updateList);
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
                    Equipment equipment = DirectoryOfEquipment.getInstance().makeNewEquipment((int)record.Key, (int)record.Value[1], (string)record.Value[2].ToString());

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
                Equipment equipment = DirectoryOfEquipment.getInstance().makeNewEquipment((int)record.Key, (int)record.Value[2], (string)record.Value[1]);

                // Add to IdentityMap
                equipmentIdentityMap.addTo(equipment);
            }
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

        //should return list of ids
        public void find(DateTime date, int firstHour, int lastHour, List<string> equipmentNameList)
        {
            //EquipmentIdentityMap.getInstance().

            TDGEquipment.getInstance().findAvailableEquipment(date, firstHour,lastHour, equipmentNameList);
        }

        public void setEquipment(int equipmentID, int reservationID, Queue<int> equipmentWaitList)
        {
            // Update the timeslot
            Equipment equipment = DirectoryOfEquipment.getInstance().modifyEquipment(equipmentID, reservationID, equipmentWaitList);

            // Register it to the unit of work
            UnitOfWork.getInstance().registerDirty(equipment);
        }

    }
}