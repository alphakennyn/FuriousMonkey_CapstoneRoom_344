using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
  
   public class User
    {

        public int userID { get; set; }
        public string name { get; set; }
        
        public string username { get; set; }
        public string password { get; set; }

        public User()
        {
            userID = 0;
            username = "";
            password = "";
            name = "";
        }

        public User (int idnumber, string un, string pw, string n)
        {
            this.userID = idnumber;
            this.username = un;
            this.password = pw; 
            this.name = n;
        }

    }

}
