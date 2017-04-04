using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using PresentationLayer.Hubs;
using Mappers;

namespace LogicLayer
{
    public class ReservationConsole
    {
        //Instance of ReservationConsole class
        private static ReservationConsole instance = new ReservationConsole();

        //Get instance
        public static ReservationConsole getInstance()
        {
            return instance;
        }

        //Constructor
        public ReservationConsole()
        {
            TimeSlotMapper.getInstance().initializeDirectory();
            ReservationMapper.getInstance().initializeDirectory();
            UserMapper.getInstance().initializeDirectory();
            RoomMapper.getInstance().initializeDirectory();
            EquipmentMapper.getInstance().initializeDirectory();
            updateDirectories();
        }

        //Method to make a reservation
        public void makeReservation(int userID, int roomID, string desc, DateTime date, int firstHour, int lastHour, List<String> equipmentNameList)
        {
            List<int> hours = new List<int>();
            for (int i = firstHour; i <= lastHour; i++)
                hours.Add(i);

            List<int> availableEquipmentIDList = new List<int>();
            List<Equipment> equipmentList = new List<Equipment>();

            availableEquipmentIDList = getAvailableEquipmentIDList(date, firstHour, lastHour, equipmentNameList);
            
            if (availableEquipmentIDList.Contains(-1))
            {
                int count = 0;
                foreach( int i in availableEquipmentIDList)
                {
                    if (i == -1)
                    {

                        string equipmentName = equipmentNameList[count];//specific equipment that is busy
                            
                        EquipmentWaitsForMapper.getInstance().putOnWaitingList(userID, date, firstHour, lastHour, equipmentName);
                    }
                    count++;
                }
            }
            else
            {
                foreach (int id in availableEquipmentIDList)
                {
                    equipmentList.Add(EquipmentMapper.getInstance().getEquipment(id));
                }

                //foreach (Reservation reservation in directoryOfReservations.reservationList)
                foreach (Reservation reservation in ReservationMapper.getInstance().getListOfReservations())    //Goes through all reservations in listOfReservations
                {
                    // Compare if the date (not the time portion) are the same and the rooms are the same
                    if (reservation.date.Date == date.Date && reservation.roomID == roomID)
                    {
                        foreach (TimeSlot timeSlot in reservation.timeSlots)
                        {
                            for (int i = firstHour; i <= lastHour; i++)
                            {
                                if (timeSlot.hour == i)
                                {
                                    if (!timeSlot.waitlist.Contains(userID) && reservation.userID != userID)
                                    {
                                        timeSlot.waitlist.Enqueue(userID);
                                        TimeSlotMapper.getInstance().setTimeSlot(timeSlot.timeSlotID, timeSlot.reservationID, timeSlot.waitlist); // It's already modifying the waitlist from the line above, do we need this?
                                    }
                                    hours.Remove(i);
                                }
                            }
                        }
                    }
                }
            }

            if (hours.Count > 0 && !availableEquipmentIDList.Contains(-1))
            {
                if (weeklyConstraintCheck(userID, date) && dailyConstraintCheck(userID,date,firstHour,lastHour))
                {
                    Reservation reservation = ReservationMapper.getInstance().makeNew(userID, roomID, desc, date, equipmentList);

                    for (int i = 0; i < hours.Count; i++)
                    {
                        TimeSlot timeSlot = TimeSlotMapper.getInstance().makeNew(reservation.reservationID, hours[i]);
                        updateWaitList(userID, date, i);
                    }
                }             
            }

            ReservationMapper.getInstance().done(); // We don't need to call done() twice. At the end of the method, you're just doing UnitOfWork.commit()
            updateDirectories(); // Need to 'refresh' all the directories to reflect the changes
        }

        // Method used to update the lists inside each directories
        public void updateDirectories()
        {
            // Updating timeSlots of each reservations
            List<TimeSlot> timeSlotList = TimeSlotMapper.getInstance().getAllTimeSlot().Values.ToList();
            timeSlotList.Sort((x, y) => x.hour.CompareTo(y.hour));
            for (int i = 0; i < ReservationMapper.getInstance().getListOfReservations().Count; i++)
            {
                foreach (TimeSlot timeSlot in timeSlotList)
                {
                    if (ReservationMapper.getInstance().getListOfReservations()[i].reservationID == timeSlot.reservationID && !ReservationMapper.getInstance().getListOfReservations()[i].timeSlots.Contains(timeSlot))
                        ReservationMapper.getInstance().getListOfReservations()[i].timeSlots.Add(timeSlot);
                }
            }

            // Updating equipment of each reservations
            
            List<Equipment> equipmentList = EquipmentMapper.getInstance().getAllEquipment().Values.ToList();
            for (int i = 0; i < ReservationMapper.getInstance().getListOfReservations().Count; i++)
            {
                foreach (Equipment equipment in equipmentList)
                {
                    if (equipment.reservationIDList.Contains(ReservationMapper.getInstance().getListOfReservations()[i].reservationID) && !ReservationMapper.getInstance().getListOfReservations()[i].equipmentList.Contains(equipment))
                        ReservationMapper.getInstance().getListOfReservations()[i].equipmentList.Add(equipment);
                }
            }
            

            //UPDATING WAITLIST FOR EACH EQUIPMENT?????????????????????????????????????????????????

            // Updating the waitList of each timeSlot
            for (int i = 0; i < TimeSlotMapper.getInstance().getListOfTimeSlots().Count; i++)
            {
                List<int> waitList = TimeSlotMapper.getInstance().getAllUsers(TimeSlotMapper.getInstance().getListOfTimeSlots()[i].timeSlotID);
                if (waitList != null)
                    for (int j = 0; j < waitList.Count; j++)
                        if (!TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Contains(waitList[j]))
                             TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Enqueue(waitList[j]);
            }

            // Updating the reservations for each room
            for (int i = 0; i < RoomMapper.getInstance().getListOfRooms().Count; i++)
            {
                foreach (KeyValuePair<int, Reservation> reservation in ReservationMapper.getInstance().getAllReservation())
                {
                    if (reservation.Value.roomID == RoomMapper.getInstance().getListOfRooms()[i].roomID)
                        RoomMapper.getInstance().getListOfRooms()[i].roomReservations.Add(reservation.Value);
                }
            }
        }

        // Method to update the waiting list for a timeslot
        public void updateWaitList(int userID, DateTime date, int hour)
        {
            foreach (TimeSlot timeSlot in TimeSlotMapper.getInstance().getListOfTimeSlots())
            {
                // Obtain the date associated with that timeslot for the current reservation
                DateTime timeSlotDate = ReservationMapper.getInstance().getReservation(timeSlot.reservationID).date;
                // DateTime timeSlotDate = directoryOfReservations.getReservation(timeSlot.reservationID).date;

                // We only want to remove the user from the waitlist of timeslots of the same date and hour 
                if (timeSlot.waitlist.Contains(userID) && timeSlotDate.Equals(date) && timeSlot.hour == hour)
                {
                    Queue<int> newQueue = new Queue<int>();
                    int size = timeSlot.waitlist.Count;
                    for (int i = 0; i < size; i++)
                    {
                        if (timeSlot.waitlist.Peek() == userID)
                        {
                            timeSlot.waitlist.Dequeue();
                        }
                        else
                        {
                            newQueue.Enqueue(timeSlot.waitlist.Dequeue());
                        }
                    }
                    TimeSlotMapper.getInstance().setTimeSlot(timeSlot.timeSlotID, timeSlot.reservationID, newQueue);
                }

            }
            TimeSlotMapper.getInstance().done();
        }

        // Method to modify a reservation
        public void modifyReservation(int resID, int roomID, string desc, DateTime date, int firstHour, int lastHour)
        {
            Reservation resToModify = new Reservation();
            for (int i = 0; i < ReservationMapper.getInstance().getListOfReservations().Count; i++)
            {
                if (resID == ReservationMapper.getInstance().getListOfReservations()[i].reservationID)
                    resToModify = ReservationMapper.getInstance().getListOfReservations()[i];
            }

            if (resToModify.date.Date != date.Date || resToModify.roomID != roomID)
            {

                for (int i = 0; i < resToModify.timeSlots.Count; i++)
                {
                    if (resToModify.timeSlots[i].waitlist.Count == 0)
                    {
                        //If waitList for timeSlots is empty, delete from db
                        TimeSlotMapper.getInstance().delete(resToModify.timeSlots[i].timeSlotID);
                        for (int k = 0; k < ReservationMapper.getInstance().getListOfReservations().Count; k++)
                        {
                            if (ReservationMapper.getInstance().getListOfReservations()[k].reservationID == resToModify.reservationID)
                                ReservationMapper.getInstance().getListOfReservations()[k].timeSlots.Remove(resToModify.timeSlots[i]);
                        }
                        i--;
                        TimeSlotMapper.getInstance().done();
                    }
                    else
                    {
                        for (int k = 0; k < TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Count; k++)
                        {
                            //Else give new reservation to the first person in waitlist
                            int userID = resToModify.timeSlots[i].waitlist.Dequeue();
                            if (weeklyConstraintCheck(userID, date) && dailyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(resID).date, 
                                resToModify.timeSlots[i].hour, resToModify.timeSlots[i].hour))
                            {
                                //get list of equipment
                                List<Equipment> equipmentList = ReservationMapper.getInstance().getEquipmentFromTDG(resID);

                                Reservation res = ReservationMapper.getInstance().makeNew(userID, ReservationMapper.getInstance().getReservation(resID).roomID,
                                                                                        "", ReservationMapper.getInstance().getReservation(resID).date, equipmentList);
                                ReservationMapper.getInstance().done();
                                TimeSlotMapper.getInstance().setTimeSlot(resToModify.timeSlots[i].timeSlotID, res.reservationID, resToModify.timeSlots[i].waitlist);
                                TimeSlotMapper.getInstance().done();
                                updateWaitList(userID, ReservationMapper.getInstance().getReservation(resID).date, resToModify.timeSlots[i].hour);
                                break;
                            }
                            updateWaitList(userID, ReservationMapper.getInstance().getReservation(resID).date, resToModify.timeSlots[i].hour);
                        }
                    }
                }
            }

            //Remove timeSlots that are not in common with the new reservation (depending if they have waitlist or not)
            for (int i = 0; i < resToModify.timeSlots.Count; i++)
            {
                int hour = resToModify.timeSlots[i].hour;
                bool foundSlot = false;
                for (int j = firstHour; j <= lastHour; j++)
                {
                    if (hour == j)
                        foundSlot = true;
                }
                if (!foundSlot)
                {
                    //If waitList for timeSlot exist, give to new user
                    //Else delete timeSlot
                    if (resToModify.timeSlots[i].waitlist.Count == 0)
                    {
                        //If waitList for timeSlots is empty, delete from db
                        TimeSlotMapper.getInstance().delete(resToModify.timeSlots[i].timeSlotID);
                        for (int k = 0; k < ReservationMapper.getInstance().getListOfReservations().Count; k++)
                        {
                            if (ReservationMapper.getInstance().getListOfReservations()[k].reservationID == resToModify.reservationID)
                                ReservationMapper.getInstance().getListOfReservations()[k].timeSlots.Remove(resToModify.timeSlots[i]);
                        }
                        i--;
                        TimeSlotMapper.getInstance().done();
                    }
                    else
                    {
                        for (int k = 0; k < TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Count; k++)
                        {
                            //Else give new reservation to the first person in waitlist
                            int userID = resToModify.timeSlots[i].waitlist.Dequeue();
                            if (weeklyConstraintCheck(userID, date) && dailyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(resID).date,
                                TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour))
                            {
                                int myroomid = ReservationMapper.getInstance().getReservation(resID).roomID;
                                DateTime mydate = ReservationMapper.getInstance().getReservation(resID).date;
                                //get equipment from reservationID
                                List<Equipment> equipmentList = ReservationMapper.getInstance().getEquipmentFromTDG(resID);
                                Reservation res = ReservationMapper.getInstance().makeNew(userID, myroomid, "", mydate, equipmentList);
                                //Reservation res = directoryOfReservations.makeNewReservation(directoryOfReservations.getReservation(reservationID).roomID, userID, "",
                                //   directoryOfReservations.getReservation(reservationID).date);
                                ReservationMapper.getInstance().done();
                                TimeSlotMapper.getInstance().setTimeSlot(resToModify.timeSlots[i].timeSlotID, res.reservationID, resToModify.timeSlots[i].waitlist);
                                TimeSlotMapper.getInstance().done();
                                updateWaitList(userID, ReservationMapper.getInstance().getReservation(resID).date, resToModify.timeSlots[i].hour);
                                break;
                            }
                            updateWaitList(userID, ReservationMapper.getInstance().getReservation(resID).date, resToModify.timeSlots[i].hour);
                            k--;
                        }   
                    }
                }
            }

            //Put on waitList if the new timeSlots are already taken, else create new ones
            List<int> hours = new List<int>();
            for (int i = firstHour; i <= lastHour; i++)
                hours.Add(i);

            foreach (Reservation reservation in ReservationMapper.getInstance().getListOfReservations())
            {
                if (reservation.date == date && reservation.roomID == roomID)
                {
                    foreach (TimeSlot timeSlot in reservation.timeSlots)
                    {
                        for (int i = firstHour; i <= lastHour; i++)
                        {
                            if (timeSlot.hour == i)
                            {
                                if (!timeSlot.waitlist.Contains(resToModify.userID) && reservation.userID != resToModify.userID)
                                {
                                    timeSlot.waitlist.Enqueue(resToModify.userID);
                                    TimeSlotMapper.getInstance().setTimeSlot(timeSlot.timeSlotID, timeSlot.reservationID, timeSlot.waitlist);
                                }
                                hours.Remove(i);
                            }
                        }
                    }
                }
            }

            if (hours.Count > 0)
            {
                for (int i = 0; i < hours.Count; i++)
                {
                    TimeSlotMapper.getInstance().makeNew(resToModify.reservationID, hours[i]);
                    updateWaitList(resToModify.userID, resToModify.date, i);
                }
            }

            TimeSlotMapper.getInstance().done();
            ReservationMapper.getInstance().modifyReservation(resToModify.reservationID, roomID, desc, date);
            ReservationMapper.getInstance().done();
            updateDirectories();
        }

        // Method to cancel a reservation
        public void cancelReservation(int reservationID)
        {
            // Loop through each timeslot
            for (int i = 0; i < TimeSlotMapper.getInstance().getListOfTimeSlots().Count; i++)
            {
                // For those who are belonging to the reservation to be cancelled:
                if (TimeSlotMapper.getInstance().getListOfTimeSlots()[i].reservationID == reservationID) //make a method in mappers that returns the respective directory lists
                {
                    TimeSlot timeSlot = TimeSlotMapper.getInstance().getListOfTimeSlots()[i];
                    // If no one is waiting, delete it.
                    if (TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Count == 0)
                    {
                        TimeSlotMapper.getInstance().delete(TimeSlotMapper.getInstance().getListOfTimeSlots()[i].timeSlotID);
                        i--;
                        TimeSlotMapper.getInstance().done();
                    }
                    // Otherwise:
                    // - Obtain the next in line, dequeue.
                    // - Make a new reservation (done - reservation)
                    // - Update the waitlists
                    // - Update the timeslot from old reservation to the new one. (done - timeslot)
                    else
                    {
                        if(findCapstone(timeSlot).Count > 0) //if there are capstone students
                        {
                            for(int j = 0; j < findCapstone(timeSlot).Count; j++)
                            {
                                int userID = findCapstone(timeSlot).Dequeue();
                                //if they meet the constraints
                                if(weeklyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(reservationID).date)
                                   && dailyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(reservationID).date,
                                   TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour))
                                {
                                    //get list of equipment
                                    List<Equipment> equipmentList = ReservationMapper.getInstance().getEquipmentFromTDG(reservationID);
                                    Reservation res = ReservationMapper.getInstance().makeNew(userID, ReservationMapper.getInstance().getReservation(reservationID).roomID,
                                 "", ReservationMapper.getInstance().getReservation(reservationID).date, equipmentList);
                                    ReservationMapper.getInstance().done();
                                    TimeSlotMapper.getInstance().setTimeSlot(TimeSlotMapper.getInstance().getListOfTimeSlots()[i].timeSlotID, res.reservationID,
                                                                             TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist);

                                    TimeSlotMapper.getInstance().done();
                                    updateWaitList(userID, ReservationMapper.getInstance().getReservation(reservationID).date, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour);
                                    break;

                                }
                                updateWaitList(userID, ReservationMapper.getInstance().getReservation(reservationID).date, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour);
                                j--;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Count; k++)
                            {
                                int userID = TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist.Dequeue();
                                if (weeklyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(reservationID).date)
                                    && dailyConstraintCheck(userID, ReservationMapper.getInstance().getReservation(reservationID).date,
                                    TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour))
                                {
                                    //get list of equipment
                                    List<Equipment> equipmentList = ReservationMapper.getInstance().getEquipmentFromTDG(reservationID);
                                    Reservation res = ReservationMapper.getInstance().makeNew(userID, ReservationMapper.getInstance().getReservation(reservationID).roomID,
                                 "", ReservationMapper.getInstance().getReservation(reservationID).date, equipmentList);
                                    ReservationMapper.getInstance().done();
                                    TimeSlotMapper.getInstance().setTimeSlot(TimeSlotMapper.getInstance().getListOfTimeSlots()[i].timeSlotID, res.reservationID,
                                                                             TimeSlotMapper.getInstance().getListOfTimeSlots()[i].waitlist);

                                    TimeSlotMapper.getInstance().done();
                                    updateWaitList(userID, ReservationMapper.getInstance().getReservation(reservationID).date, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour);
                                    break;

                                }
                                updateWaitList(userID, ReservationMapper.getInstance().getReservation(reservationID).date, TimeSlotMapper.getInstance().getListOfTimeSlots()[i].hour);
                                k--;
                            }
                        }  
                    }
                }
            }


            // Completely done with this reservation, delete it.
            ReservationMapper.getInstance().delete(reservationID);
            ReservationMapper.getInstance().done();
            updateDirectories();
        }

        public Queue<int> findCapstone(TimeSlot timeslot)
        {
            Queue<int> tempQueue = timeslot.waitlist;
            Queue<int> newQueue = new Queue<int>();
            for(int i = 0; i < timeslot.waitlist.Count; i++)
            {
                User tempUser = UserMapper.getInstance().getUser(tempQueue.Dequeue());
                if (tempUser != null && tempUser.inCapstone)
                {
                    newQueue.Enqueue(tempUser.userID);
                }
            }
            return newQueue;
        }

        public List<TimeSlot> getAllTimeSlots()
        {
            return TimeSlotMapper.getInstance().getListOfTimeSlots();
        }

        public List<Reservation> getAllReservations()
        {
            return ReservationMapper.getInstance().getListOfReservations(); 
        }

        public List<Room> getAllRooms()
        {
            return RoomMapper.getInstance().getListOfRooms();
        }

        public List<User> getUserCatalog()
        {
            return UserMapper.getInstance().getListOfUsers();
        }


        public List<Reservation> findByDate(DateTime date)
        {
            List<Reservation> listByDate = new List<Reservation>();
            foreach (Reservation reservation in ReservationMapper.getInstance().getListOfReservations())
            {
                if (reservation.date.Date == date.Date)
                {
                    listByDate.Add(reservation);
                }
            }
            return listByDate;
        }

        public List<Reservation> findByUser(int userID)
        {
            List<Reservation> listByuserId = new List<Reservation>();
            foreach (Reservation reservation in ReservationMapper.getInstance().getListOfReservations())
            {
                if (reservation.userID == userID)
                {
                    listByuserId.Add(reservation);
                }
            }
            return listByuserId;
        }


        public List<Reservation> filterByBlock(DateTime date)
        {
            List<Reservation> listByDate = new List<Reservation>();
            foreach (Reservation reservation in ReservationMapper.getInstance().getListOfReservations())
            {
                if (reservation.date == date)
                {
                    listByDate.Add(reservation);
                }
            }
            return listByDate;
        }

        //public UserCatalog getUserCatalog()
        //{
        //    return userCatalog;
        //}

        /**
         * method for the constraint of not exceeding a reservation of 3 hours per day per person
         */
        public Boolean dailyConstraintCheck(int userID, DateTime date, int firstHour, int lastHour)
        {
            //interval of hours for the desired reservation
            int newHours = lastHour - firstHour + 1;
            //number of hours of reservation currently for chosen day
            List<int> result = ReservationMapper.getInstance().findReservationIDs(userID, date);
            // if the user doesn't have any reservations for this day, the constraint satisfied (user can make reservation)
            if(result == null)
            {
                return true;
            }
            int currentHours = TimeSlotMapper.getInstance().findHoursByReservationID(result);
            //checks of reservation is possible according to constraint
            if (currentHours + newHours <= 3)
            {
                return true;
            }
            return false;
        }


        /**
         * method for the constraint of not exceeding 3 reservations per week per person
         */
        public Boolean weeklyConstraintCheck(int userID, DateTime date)
        {
            int counter = 0;
            //numbercial value for day of the week
            int currentDay = (int)date.DayOfWeek;
            //list that will contain all the found reservation IDs
            List<int> IDlist = new List<int>();
            //if the day is sunday
            if (currentDay == 0)
            {
                currentDay = 7;
            }
            //for every day of the week until current day
            for (int i = 0; i < currentDay; i++)
            {
                List<int> result = ReservationMapper.getInstance().findReservationIDs(userID, date.AddDays(-i));
                if (result != null)
                {
                    counter += result.Count;
                }
            } 
            //return true if the user has made less than 3 reservations
            if (counter < 3)
            {
                return true;
            }
            //otherwise return false
            return false;
        }

        public List<int> getAvailableEquipmentIDList(DateTime date, int firstHour, int lastHour, List<string> equipmentNameList)
        {
            updateDirectories();
            List<int> equipmentIDList = new List<int>();
            List<Reservation> reservationList = ReservationMapper.getInstance().getListOfReservations();
            List<Equipment> equipmentList = EquipmentMapper.getInstance().getListOfEquipment();

            foreach (string name in equipmentNameList)   //perform on each string in equipmentNameList *e.g. "computer"
            {
                bool equipmentFound = false;
                foreach (Equipment equipment in equipmentList)   //Goes through all equipment objects
                {
                    if (equipment.equipmentName == name && !equipmentIDList.Contains(equipment.equipmentID))    //Comparing to all objects with matching name *e.g. to all computers
                    {
                        bool isAvailable = true;
                        foreach (int resID in equipment.reservationIDList)  //go through each reservation ID in the matching equipment
                        {
                            foreach (Reservation reservation in reservationList) //go through every reservation
                            {
                                if (reservation.reservationID == resID) //compare to reservations that are borrowing the specified equipment *e.g. reservations with computers
                                {
                                    if (reservation.date == date)   //Compare to reservations that occur on the same date as selected
                                    {
                                        foreach (TimeSlot timeslot in reservation.timeSlots)    //Goes through the timeslots in the above reservations
                                        {
                                            for (int i = firstHour; i <= lastHour; i++)
                                            {
                                                if (timeslot.hour == i) //If the reservation occurs at the same time as the specified time
                                                {
                                                    isAvailable = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (isAvailable)
                        {
                            equipmentIDList.Add(equipment.equipmentID);
                            equipmentFound = true;
                            break;
                        }
                    }
                }
                if (!equipmentFound)
                {
                    equipmentIDList.Add(-1);
                }
            }
            return equipmentIDList;
        }
    }
}