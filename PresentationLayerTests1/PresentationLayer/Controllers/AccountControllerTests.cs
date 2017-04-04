using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PresentationLayer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers.SchedulerTests
{
    [TestClass()]
    public class AccountControllerTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            string[] username = new String[] { "", "Jack", "Holly", "" };
            string[] password = new String[] { "", "Smith", "Doe", "Doe" };

            for (int i = 0; i < 4; i++)
            {
                if (username[i] == null || password[i] == null)
                {
                    Assert.Fail();
                }
            }

        }

        [TestMethod()]
        public void LoginIsValidTest()
        {
            string username = "jack";
            string password = "reacher";

            foreach (User user in ReservationConsole.getInstance().getUserCatalog())
            {
                if (user.username != username && user.password != password)
                {
                    Assert.Fail();
                }
            }

        }
    }
}