using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapstoneRoomScheduler.LogicLayer.AuthorizeManager;
using Microsoft.AspNet.Identity;
using LogicLayer;
using Microsoft.AspNet.SignalR;
using PresentationLayer.Hubs;
using System.Threading;
using System.Diagnostics;

namespace CapstoneRoomScheduler.Controllers
{
    
    public class HomeController : Controller
    {
        //Lock object to lock rooms. Eventually add objects to the lockobject dynamically using the total number of rooms
        //Technically a room should have its own lock object
        private static object[] _lockobject = new object[] {new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        new object(),new object(),new object(),new object(),new object(),
        }
      ;
        public ActionResult Calendar()
        {
            return View();
        }
        [LoggedIn]
        [HttpPost]
        public void makeReservation(int room,string description,int day,int month,int year,int firstTimeSlot, int lastTimeSlot, int numOfComputers, int numOfProjectors, int numOfMarkers)
        {
            if(Monitor.TryEnter(_lockobject[room],TimeSpan.FromSeconds(59))){
                var userID = Int32.Parse(User.Identity.GetUserId());
                var date = new DateTime(year, month, day);
                var weeklyConstraint = ReservationConsole.getInstance().weeklyConstraintCheck(userID, date);
                var dailyConstraint = ReservationConsole.getInstance().dailyConstraintCheck(userID, date, firstTimeSlot, lastTimeSlot);

                //Store a name of equipment for each equipment requested
                List<string> equipmentNameList = new List<string>();
                for (int i = 0; i < numOfComputers; i++)
                {
                    equipmentNameList.Add("computer");
                }
                for (int j = 0; j < numOfProjectors; j++)
                {
                    equipmentNameList.Add("projector");
                }
                for (int k = 0; k < numOfMarkers; k++)
                {
                    equipmentNameList.Add("marker");
                }
                
                //Testing whether this even works
                for (int x = 0; x < equipmentNameList.Count(); x++)
                {
                    Debug.WriteLine(equipmentNameList[x]);
                }

                if (dailyConstraint && weeklyConstraint)
                {
                    Thread.Sleep(4000); //locks room for 4 seconds for mutual exclusion
                    ReservationConsole.getInstance().makeReservation(userID, room, description, date, firstTimeSlot, lastTimeSlot);
                    GlobalHost.ConnectionManager.GetHubContext<CalendarHub>().Clients.Group(User.Identity.GetUserId()).incomingMessage("Reservation has been successfully created");
                    updateCalendar(year, month, day);
                }
                Monitor.Exit(_lockobject[room]);
            }
           
        }
        [HttpPost]
        public void modifyReservation(int roomId,string date,int initialTimeslot,int finalTimeslot,int resid,int day,int month,int year,string description)
        {
            string[] dateArray=date.Split('-');
            var userID = Int32.Parse(User.Identity.GetUserId());
            var dateOfRes = new DateTime(Int32.Parse(dateArray[0]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[2]));
            var weeklyConstraint = ReservationConsole.getInstance().weeklyConstraintCheck(userID, dateOfRes);
            var dailyConstraint = ReservationConsole.getInstance().dailyConstraintCheck(userID, dateOfRes, initialTimeslot, initialTimeslot);
            if (dailyConstraint && weeklyConstraint)
            {
                ReservationConsole.getInstance().modifyReservation(resid, roomId, description, dateOfRes, initialTimeslot, finalTimeslot - 1);
                GlobalHost.ConnectionManager.GetHubContext<CalendarHub>().Clients.Group(User.Identity.GetUserId()).incomingMessage("Reservation has been successfully modifed");
                getReservations();
                updateCalendar(year, month, day);
            }
        }
        [HttpPost]
        public void cancelReservation(string resid,int day, int month, int year)
        {

            ReservationConsole.getInstance().cancelReservation(Int32.Parse(resid));
            GlobalHost.ConnectionManager.GetHubContext<CalendarHub>().Clients.Group(User.Identity.GetUserId()).incomingMessage("Reservation has been successfully cancelled");
            getReservations();
            updateCalendar(year, month, day);
           
        }

        [HttpPost]
        public void updateCalendar(int year, int month, int day)
        {
           
           DateTime date = new DateTime(year, month, day);
           var hubContext = GlobalHost.ConnectionManager.GetHubContext<CalendarHub>();
           hubContext.Clients.All.updateCalendar(convertToJsonObject(ReservationConsole.getInstance().findByDate(date)));
            hubContext.Clients.All.updateWaitlist(ReservationConsole.getInstance().getAllTimeSlots(), convertToJsonObject(ReservationConsole.getInstance().findByDate(date)),ReservationConsole.getInstance().getUserCatalog());
            
            }
        
        [HttpPost]
        public void getReservations() {

            if (User.Identity.IsAuthenticated) {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<CalendarHub>();
                var JsonListofReservations = convertToJsonObject(ReservationConsole.getInstance().findByUser(Int32.Parse(User.Identity.GetUserId())));
                hubContext.Clients.All.populateReservations(JsonListofReservations); //returns a list of reservations in the js function
            }
            else {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<CalendarHub>();
                var JsonListofReservations = convertToJsonObject(ReservationConsole.getInstance().getAllReservations());
                hubContext.Clients.All.populateReservations(JsonListofReservations);

            }
        }

        //Techiincally asp/signarl autmatically converts to json when you pass an object to javascript but here we just convert it into an easy to digest object
        public List<object> convertToJsonObject(List<Reservation> reservationList)
        {
            
            int firstTimeSlot;
            int lastTimeSlot;
            List<object> list = new List<object>();
            for (int i = 0; i < (reservationList.Count()); i++)
            {
                firstTimeSlot = reservationList[i].timeSlots[0].hour;
                
                lastTimeSlot = reservationList[i].timeSlots[reservationList[i].timeSlots.Count()-1].hour;
                list.Add(new
                {
                    initialTimeslot = firstTimeSlot,
                    finalTimeslot = lastTimeSlot,
                    roomId = reservationList[i].roomID,
                    description = reservationList[i].description,
                    userName = ReservationConsole.getInstance().getUserCatalog().First(x => x.userID == reservationList[i].userID).name,
                    reservationId = reservationList[i].reservationID,
                    userId = reservationList[i].userID,
                    date = reservationList[i].date.Date

                });

            }
            return list;
        }




    }
}