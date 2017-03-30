using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class DirectoryOfEquipment
    {
        private static DirectoryOfEquipment instance = new DirectoryOfEquipment();

        public List<Equipment> equipmentList { get; set; }

        // Constructor
        private DirectoryOfEquipment()
        {
            equipmentList = new List<Equipment>();
        }

        // Get instance
        public static DirectoryOfEquipment getInstance()
        {
            return instance;
        }

        public void getEquipment(DateTime date, int firstHour, int lastHour, List<String> equipmentNameList)
        {

        }

        

    }
}
