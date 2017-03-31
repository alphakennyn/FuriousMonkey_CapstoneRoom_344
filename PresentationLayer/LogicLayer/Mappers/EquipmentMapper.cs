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
                Equipment equipment = DirectoryOfEquipment.getInstance().makeNewEquipment((int)record.Key, (int)record.Value[1], (string)record.Value[2]);

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

    }
}