using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public abstract class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Consider hashing in a real application

        protected DatabaseManager DbManager;

        public User(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public abstract bool Login(string email, string password);
    }



}
