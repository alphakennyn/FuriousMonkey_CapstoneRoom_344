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

        // Method to make a new time slot
        public Equipment makeNewEquipment(int equipmentID, List<int> reservationIDList, string equipmentName)
        {
            Equipment equipment = new Equipment(equipmentID, reservationIDList, equipmentName);
            if (!equipmentList.Contains(equipment))
                equipmentList.Add(equipment);
            return equipment;
        }

        public Equipment modifyEquipment(int equipmentID, List<int> reservationIDList, Queue<int> wlist)
        {
            for (int i = 0; i < equipmentList.Count; i++)
            {
                if (equipmentList[i].equipmentID == equipmentID)
                {
                    equipmentList[i].reservationIDList = reservationIDList;
                    equipmentList[i].equipmentID = equipmentID;
                    equipmentList[i].equipmentWaitList = wlist;
                    return equipmentList[i];
                }
            }
            return null;
        }
    }
}
