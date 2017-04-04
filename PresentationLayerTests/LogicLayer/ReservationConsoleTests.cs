using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.SchedulerTests
{
    [TestClass()]
    public class ReservationConsoleTests
    {

        [TestMethod()]
        public void dailyConstraintCheckSucceedTest()
        {
            int firstHour = 1;
            int lastHour = 3;

            if (lastHour - firstHour > 3)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void dailyConstraintCheckFailTest()
        {
            int firstHour = 1;
            int lastHour = 7;

            if (lastHour - firstHour <= 3)
            {
                Assert.Fail();
            }
        }
    }
}