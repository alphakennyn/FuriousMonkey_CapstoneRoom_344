using System.Collections.Generic;
using TDG;
using LogicLayer;
using System;

namespace Mappers
{
    //need modification because of table equipmentwaitsfor
    class EquipmentWaitsForMapper
    {
        // Instance of this mapper object
        private static EquipmentWaitsForMapper instance = new EquipmentWaitsForMapper();

        private TDGEquipmentWaitsFor tdgEquipmentWaitsFor = TDGEquipmentWaitsFor.getInstance();

        private EquipmentWaitsForMapper() { }

        public static EquipmentWaitsForMapper getInstance()
        {
            return instance;
        }

        public List<Object[]> getAll()
        {
            return tdgEquipmentWaitsFor.getAll();
        }

        public void refreshWaitsFor(List<Equipment> refreshList)
        {
            tdgEquipmentWaitsFor.refreshEquipmentWaitsFor(refreshList);
        }
        public void putOnWaitingList(int userID, DateTime date, int firstHour, int lastHour, string equipmentName, int roomID)
        {
            tdgEquipmentWaitsFor.addEquipmentWaitsFor(equipmentName, userID, date, firstHour, lastHour, roomID);
        }
        public void removeEquipmentWaitsFor(int userID)
        {
            tdgEquipmentWaitsFor.deleteEquipmentWaitsFor(userID);
        }
    }
}
