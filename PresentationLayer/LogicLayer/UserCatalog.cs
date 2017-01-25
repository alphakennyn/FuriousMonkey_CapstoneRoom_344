using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LogicLayer
{
    public class UserCatalog
    {
        // Singleton
        private static UserCatalog instance = new UserCatalog();

        // List of users with get and set
        public List<User> registeredUsers { get; set; }

        // Constructor
        private UserCatalog()
        {
            registeredUsers = new List<User>();
        }

        // Method to get instance
        public static UserCatalog getInstance()
        {
            return instance;
        }

        // Method to make a new user
        public User makeNewUser(int userID, String username, String password, String name)
        {
            User user = new User(userID, username, password, name);
            registeredUsers.Add(user);
            return user;
        }

        // Method to modify a user
        public User modifyUser(int userID, String name)
        {
            for(int i = 0; i < registeredUsers.Count; i++)
            {
                if (registeredUsers[i].userID == userID)
                {
                    registeredUsers[i].userID = userID;
                    registeredUsers[i].name = name;
                    return registeredUsers[i];
                }
            }
            return null;
        }
    }
}
