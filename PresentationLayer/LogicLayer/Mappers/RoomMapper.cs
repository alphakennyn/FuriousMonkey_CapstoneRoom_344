using System;
using System.Collections.Generic;
using TDG;
using LogicLayer;
using CapstoneRoomScheduler.LogicLayer.IdentityMaps;

namespace Mappers
{
    class RoomMapper
    {
        // Instance of this mapper object
        private static RoomMapper instance = new RoomMapper();

        private TDGRoom tdgRoom = TDGRoom.getInstance();
        private RoomIdentityMap roomIdentityMap = RoomIdentityMap.getInstance();

        // The last ID that is used
        private int lastID;

        // Lock to modify last ID
        private readonly Object lockLastID = new Object();

        private RoomMapper()
        {
            this.lastID = tdgRoom.getLastID();
        }

        public static RoomMapper getInstance()
        {
            return instance;
        }


        // Method to get rooms from DB and add into the list
        public void initializeDirectoryOfRoom()
        {
            foreach (KeyValuePair<int, Room> room in getAllRooms())
            {
                DirectoryOfRooms.getInstance().roomList.Add(room.Value);
            }
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
         * Handles the creation of a new room
         */
        public Room makeNew (String roomNum)
        {

            int nextID = getNextID();
            Room room = DirectoryOfRooms.getInstance().makeNewRoom(nextID, roomNum);

            // Add it to the identity map
            roomIdentityMap.addTo(room);

            // Register as a new room
            UnitOfWork.getInstance().registerNew(room);

            return room;
        } 

        /**
         * Retrieve a room given its ID
         */
        public Room getRoom (int roomID)
        {
            // Try to obtain the room from the identity map
            Room room = roomIdentityMap.find(roomID);
            Object[] result = null;
            if(room == null)
            {
                // Not found in identity map: try to retrive from DB
                result = tdgRoom.get(roomID);
                if (result != null)
                {
                    room = DirectoryOfRooms.getInstance().makeNewRoom((int)result[0], (String)result[1]);
                    roomIdentityMap.addTo(room);
                }
            }

            // Null is returned if it is not found in the identity map nor in the DB
            return room;
        } 

        /**
         * Retrieve all rooms
         */
        public Dictionary<int, Room> getAllRooms()
        {
            // Get all rooms from the identity map
            Dictionary<int, Room> rooms = roomIdentityMap.findAll();

            // Get all rooms in the database
            Dictionary<int, Object[]> result = tdgRoom.getAll();

            // If it's empty, simply return those from the identity map
            if (result == null)
            {
                return rooms;
            }

            // Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                // The room is not in the identity map. Create an instance, add it to identity map and to the return variable
                if(!rooms.ContainsKey(record.Key))
                {
                    Room room = DirectoryOfRooms.getInstance().makeNewRoom((int)record.Key, (String)record.Value[1]);

                    roomIdentityMap.addTo(room);
                    
                    rooms.Add(room.roomID, room);
                }
            } 

            return rooms;
        }

        /**
         * Initialize the list of rooms, used for instantiating console
         * */
        public void initializeDirectory()
        {
            // Get all rooms in the database
            Dictionary<int, Object[]> result = tdgRoom.getAll();

            //Loop through each of the result:
            foreach (KeyValuePair<int, Object[]> record in result)
            {
                Room room = DirectoryOfRooms.getInstance().makeNewRoom((int)record.Key, (String)record.Value[1]);

                roomIdentityMap.addTo(room);
            }
        }

        // I commented it out because this is not used right now.
        //The list of reservation is updated upon calling updateDirectories()
        //
        ///**
        // * Set room attributes, mainly to update list of reservations
        // */
        //public void setRoom(int roomID, String roomNum, List<Reservation> reservations)
        //{

        //    Room room = DirectoryOfRooms.getInstance().modifyRoom(roomID, roomNum, reservations);

        //    // Register it to the unit of work
        //    UnitOfWork.getInstance().registerDirty(room);
        //}

        /**
         * Delete room
         */
        public void delete(int roomID)
        {
            // Get the room to be deleted
            Room room = roomIdentityMap.find(roomID);

            // If found, remove it from identity map
            if(room != null)
            {
                roomIdentityMap.removeFrom(room);
            }

            // Register as deleted
            UnitOfWork.getInstance().registerDeleted(room);
            DirectoryOfRooms.getInstance().deleteRoom(roomID);

        } 

        /**
         * Done: commit
         */
        public void done()
        {
            UnitOfWork.getInstance().commit();
        } 

        /**
         * For unit of work:
         * Add a list of rooms to DB
         */
        public void addRoom(List<Room> newList)
        {
            tdgRoom.addRoom(newList);
        } 

        /**
         * For unit of work:
         * Update list of rooms on DB
         */
        public void updateRoom(List<Room> updateList)
        {
            tdgRoom.updateRoom(updateList);
        }
        
        /**
         * For unit of work:
         * Remove list of rooms from DB
         */
        public void deleteRoom(List<Room> deleteList)
        {
            tdgRoom.deleteRoom(deleteList);
        }


        public List<Room> getListOfRooms()
        {
            return (DirectoryOfRooms.getInstance().roomList);
        }


    }
}
