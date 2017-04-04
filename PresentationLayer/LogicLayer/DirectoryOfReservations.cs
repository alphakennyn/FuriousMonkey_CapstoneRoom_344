using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class DirectoryOfReservations
    {
        private static DirectoryOfReservations instance = new DirectoryOfReservations();
        
        public List<Reservation> reservationList { get; set; }
       

        // Constructor
        private DirectoryOfReservations()
        {
            reservationList = new List<Reservation>();
        }

        // Get the instance object
        public static DirectoryOfReservations getInstance()
        {
            return instance;
        }

        // Method to make a new reservation
        public Reservation makeNewReservation(int reservationID, int userID, int roomID, string desc, DateTime date, List<Equipment> equipmentList)
        {

            Reservation reservation = new Reservation(reservationID, userID, roomID, desc, date, equipmentList);

            reservationList.Add(reservation);

            return reservation;
        }

        // Method to modify a reservation
        public void modifyReservation(int reservationID, int roomID, string desc, DateTime date)
        {
            foreach (Reservation reservation in reservationList)
            {
                if (reservation.reservationID == reservationID)
                {
                    reservation.roomID = roomID;
                    reservation.description = desc;
                    reservation.date = date;
                }
            }
        }

        // Method to cancel a reservation
        public void cancelReservation(int reservationID)
        {
            foreach (Reservation reservation in this.reservationList)
            {
                if (reservation.reservationID == reservationID)
                {
                    reservationList.Remove(reservation);
                    return;
                }
            }
        }

    }
}