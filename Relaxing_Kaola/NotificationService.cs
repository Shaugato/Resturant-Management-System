using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class NotificationService
    {
        private DatabaseManager DbManager;

        public NotificationService(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public void SendAlert(int userId, string message)
        {
            // Retrieve user details to send an alert (assuming users have an associated email or phone)
            var userDetails = DbManager.FindRecords("Users", userId.ToString()).FirstOrDefault();
            if (userDetails != null)
            {
                var fields = userDetails.Split(',');
                string email = fields[1]; // Assuming the email is in the second position
                Console.WriteLine($"Sending alert to {email}: {message}");
            }
            else
            {
                Console.WriteLine("User not found for sending an alert.");
            }
        }
    }

}
