using System.Collections.Generic;
using LogicLayer;

namespace CapstoneRoomScheduler.LogicLayer.IdentityMaps
{
    public class RoomIdentityMap
    {
        /**
         * Room Identity Map instance (Singleton)
         */ 
        private static RoomIdentityMap instance = new RoomIdentityMap();
        
        /**
         * Dictionary representing the rooms that are currently used in the active memory
         */ 
        private Dictionary<int, Room> roomList_ActiveMemory = new Dictionary<int, Room>();

        /**
         * Private constructor (Singleton)
         */ 
        private RoomIdentityMap() { }

        /**
         * Returns the instance of RoomIdentityMap
         */ 
        public static RoomIdentityMap getInstance()
        {
            return instance;
        }

        /**
         * Adds a room object to the dictionary representing all rooms in the active memory
         */ 
        public void addTo(Room room)
        {
            roomList_ActiveMemory.Add(room.roomID, room);
        }

        /**
         * Removes a room object from the active memory dictionary
         */ 
        public void removeFrom(Room room)
        {
            roomList_ActiveMemory.Remove(room.roomID);
        }

        /**
         * Finds and return a room based on its id from the active memory
         */ 
        public Room find(int roomID)
        {
            Room room;
            if (roomList_ActiveMemory.TryGetValue(roomID, out room))
            {
                return room;
            }
            return null;
        }

        /**
         * Finds all rooms that are currently in the active memory
         */
         public Dictionary<int, Room> findAll()
        {
            // Create a new dictionary to be returned
            Dictionary<int, Room> newDictionary = new Dictionary<int, Room>();

            // Copy each key value pairs (do not need to deep copy the value, room).
            // We simply want to not return the reference to the dictionary used here.
            foreach(KeyValuePair<int, Room> pair in this.roomList_ActiveMemory)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }

            return newDictionary;
        }
          
    }
}
