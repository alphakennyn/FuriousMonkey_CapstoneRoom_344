using System.Collections.Generic;
using TDG;
using LogicLayer;

namespace Mappers
{
    //need modification because of table equipmentwaitsfor
    class EquipmentWaitsForMapper
    {
        // Instance of this mapper object
        private static EquipmentWaitsForMapper instance = new EquipmentWaitsForMapper();

        private TDGWaitsFor tdgWaitsFor = TDGWaitsFor.getInstance();

        private EquipmentWaitsForMapper() { }

        public static EquipmentWaitsForMapper getInstance()
        {
            return instance;
        }

        public List<int> getAllUsers(int timeSlotID)
        {
            return tdgWaitsFor.getAllUsers(timeSlotID);
        }

        public void refreshWaitsFor(List<Equipment> refreshList)
        {
            TDGEquipmentWaitsFor.refreshWaitsFor(refreshList);
        }
    }
}
