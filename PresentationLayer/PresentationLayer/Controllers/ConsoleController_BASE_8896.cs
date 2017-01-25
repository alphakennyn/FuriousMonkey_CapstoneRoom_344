using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapstoneRoomScheduler.Controllers
{
    public class ConsoleController : Controller
    {
        public ActionResult Calendar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult acceptTimeslots(string inputCourseName,int firstTimeslot, int lastTimeslot, int room, string date)
        {
           
         return Content("wow");
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is Harambook";

            return View();
        }
        public ActionResult Reservations()
        {
           

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Harambe's Home :'(";

            return View();
        }
    }
}