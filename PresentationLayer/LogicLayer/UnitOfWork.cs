using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using StorageLayer;
using Mappers;

namespace LogicLayer
{
    public class UnitOfWork
    {

        private static UnitOfWork instance = new UnitOfWork();

        private List<User> userNewList = new List<User>();
        private List<User> userChangedList = new List<User>();
        private List<User> userDeletedList = new List<User>();

        private List<Reservation> reservationNewList = new List<Reservation>();
        private List<Reservation> reservationChangedList = new List<Reservation>();
        private List<Reservation> reservationDeletedList = new List<Reservation>();

        private List<Room> roomNewList = new List<Room>();
        private List<Room> roomChangedList = new List<Room>();
        private List<Room> roomDeletedList = new List<Room>();

        private List<TimeSlot> timeSlotNewList = new List<TimeSlot>();
        private List<TimeSlot> timeSlotDeletedList = new List<TimeSlot>();
        private List<TimeSlot> timeSlotChangedList = new List<TimeSlot>();

        private List<Equipment> equipmentNewList = new List<Equipment>();
        private List<Equipment> equipmentDeletedList = new List<Equipment>();
        private List<Equipment> equipmentChangedList = new List<Equipment>();

        UserMapper userMapper = UserMapper.getInstance();
        RoomMapper roomMapper = RoomMapper.getInstance();
        ReservationMapper reservationMapper = ReservationMapper.getInstance();
        TimeSlotMapper timeSlotMapper = TimeSlotMapper.getInstance();
        EquipmentMapper equipmentMapper = EquipmentMapper.getInstance();

        private UnitOfWork() { }

        public static UnitOfWork getInstance()
        {
            return instance;
        }

        public void registerDirty(User user)
        {
            userChangedList.Add(user);
        }

        public void registerNew(Reservation reservation)
        {
            reservationNewList.Add(reservation);
        }

        public void registerDirty(Reservation reservation)
        {
            reservationChangedList.Add(reservation);
        }

        public void registerDeleted(Reservation reservation)
        {
            reservationDeletedList.Add(reservation);
        }

        public void registerNew(Room room)
        {
            roomNewList.Add(room);
        }

        public void registerDirty(Room room)
        {
            roomChangedList.Add(room);
        }

        public void registerDeleted(Room room)
        {
            roomDeletedList.Add(room);
        }

        public void registerNew(TimeSlot timeslot)
        {
            timeSlotNewList.Add(timeslot);
        }

        public void registerDeleted(TimeSlot timeslot)
        {
            timeSlotDeletedList.Add(timeslot);
        }

        public void registerDirty(TimeSlot timeslot)
        {
            timeSlotChangedList.Add(timeslot);
        }

        public void registerNew(Equipment equipment)
        {
            equipmentNewList.Add(equipment);
        }

        public void registerDeleted(Equipment equipment)
        {
            equipmentDeletedList.Add(equipment);
        }

        public void registerDirty(Equipment equipment)
        {
            equipmentChangedList.Add(equipment);
        }
        public void commit()
        {

            // To be verified with respective mappers
            //if (userNewList.Count() != 0)
            //userMapper.AddUser(userNewList); 
            //prof doesn't want add users in our case
            //if (userChangedList.Count() != 0)
            //    userMapper.updateUser(userChangedList);
            //if (userDeletedList.Count() != 0)
            //    userMapper.deleteUser(userDeletedList);

            if (reservationNewList.Count() != 0)
                reservationMapper.addReservation(reservationNewList);
            if (reservationChangedList.Count() != 0)
                reservationMapper.updateReservation(reservationChangedList);
            if (reservationDeletedList.Count() != 0)
                reservationMapper.deleteReservation(reservationDeletedList);

            if (roomNewList.Count() != 0)
                roomMapper.addRoom(roomNewList);
            if (roomChangedList.Count() != 0)
                roomMapper.updateRoom(roomChangedList);
            if (roomDeletedList.Count() != 0)
                roomMapper.deleteRoom(roomDeletedList);

            if (timeSlotNewList.Count() != 0)
                timeSlotMapper.addTimeSlot(timeSlotNewList);
            if (timeSlotChangedList.Count() != 0)
                timeSlotMapper.updateTimeSlot(timeSlotChangedList);
            if (timeSlotDeletedList.Count() != 0)
                timeSlotMapper.deleteTimeSlot(timeSlotDeletedList);

            if (equipmentNewList.Count() != 0)
                equipmentMapper.addequipment(equipmentNewList);
            if (timeSlotChangedList.Count() != 0)
                equipmentMapper.updateequipment(equipmentChangedList);
            if (timeSlotDeletedList.Count() != 0)
                equipmentMapper.deleteequipment(equipmentDeletedList);

            //Empty the lists after the Commit.
            userDeletedList.Clear();
            userChangedList.Clear();
            userNewList.Clear();
            reservationDeletedList.Clear();
            reservationChangedList.Clear();
            reservationNewList.Clear();
            roomDeletedList.Clear();
            roomChangedList.Clear();
            roomNewList.Clear();
            timeSlotNewList.Clear();
            timeSlotChangedList.Clear();
            timeSlotDeletedList.Clear();
            equipmentNewList.Clear();
            equipmentChangedList.Clear();
            equipmentDeletedList.Clear();
        }
    }
}