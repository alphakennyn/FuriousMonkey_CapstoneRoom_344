using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LogicLayer;

namespace CapstoneRoomScheduler.LogicLayer.IdentityMaps
{
    public class WaitsForIdentityMap
    {
        //default constructor
        private WaitsForIdentityMap() { }
        //an instance
        private static WaitsForIdentityMap instance = new WaitsForIdentityMap();

        public static WaitsForIdentityMap getInstance()
        {
            return instance;
        }
    }
}