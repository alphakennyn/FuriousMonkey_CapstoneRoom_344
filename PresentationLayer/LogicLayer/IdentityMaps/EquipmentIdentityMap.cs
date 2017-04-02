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
     * Returns the instance of EquipmentIdentityMap
     * */
    class EquipmentIdentityMap
    {
        private static EquipmentIdentityMap instance = new EquipmentIdentityMap();
        private Dictionary<int, Equipment> equipmentList_ActiveMemory = new Dictionary<int, Equipment>();
        private EquipmentIdentityMap() { }

        public static EquipmentIdentityMap getInstance()
        {
            return instance;
        }


        /**
         * Adds a equipment object to the dictionary representing all reservations in the active memory
         */
        public void addTo(Equipment equipment)
        {
            equipmentList_ActiveMemory.Add(equipment.equipmentID, equipment);
        }

        public void removeFrom(Equipment equipment)
        {
            equipmentList_ActiveMemory.Remove(equipment.equipmentID);
        }


        /*
         * Finds and return a equipment based on its id from the active memory
         * */

        public Equipment find(int equipmentID)
        {
            Equipment equipment;
            if (equipmentList_ActiveMemory.TryGetValue(equipmentID, out equipment))
            {
                return equipment;
            }
            return null;
        }


        /**
         * Finds all equipment that are currently in the active memory
         * */

        public Dictionary<int, Equipment> findAll()
        {
            //Create a new dictionary to be returned
            Dictionary<int, Equipment> newDictionary = new Dictionary<int, Equipment>();

            //Copy each key value pairs (do not need to deep copy the value, equipment).
            //We simply want to not return the reference to the dictionary used here.
            foreach (KeyValuePair<int, Equipment> pair in this.equipmentList_ActiveMemory)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }
            return newDictionary;
        }

        /**
        * Finds all equipment that are currently in the active memory that has a specific equipment ID
        * */

        public Dictionary<int, Equipment> findAll(int reservationID)
        {

            //Create a new dictionary to be returned
            Dictionary<int, Equipment> newDictionary = new Dictionary<int, Equipment>();

            //Copy each key value pairs (do not need to deep copy the value, equipment).
            //We simply want to not return the reference to the dictionary used here.
            foreach (KeyValuePair<int, Equipment> pair in this.equipmentList_ActiveMemory)
            {
                if ((pair.Value).reservationIDList.Contains(reservationID))
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
            foreach (KeyValuePair<int, Equipment> pair in equipmentList_ActiveMemory)
            {
                foreach (int reservationID in IDlist)
                {
                    if (pair.Value.reservationIDList.Contains(reservationID))
                    {
                        hours ++;
                    }
                }
            }
            return hours;
        }


    }
}