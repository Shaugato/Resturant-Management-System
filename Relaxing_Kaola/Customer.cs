using System;

namespace Relaxing_Kaola
{
    public class Customer : User
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public Customer(DatabaseManager dbManager) : base(dbManager)
        {
        }

        public override bool Login(string email, string password)
        {
            var records = DbManager.FindRecords("Users", $"{email},{password}");
            return records.Count > 0;
        }

        public bool UpdateContactInfo(string newEmail, string newPhone)
        {
            Email = newEmail;
            Phone = newPhone;
            return DbManager.UpdateRecord("Customers", $"{UserId},", $"{UserId},{Email},{Name},{Phone}");
        }

        public void GetCustomerDetails(int customerId)
        {
            var records = DbManager.FindRecords("Customers", $"{customerId},");
            if (records.Count > 0)
            {
                var fields = records[0].Split(',');
                UserId = int.Parse(fields[0]);
                Email = fields[1];
                Name = fields[2];
                Phone = fields[3];
            }
        }
    }
}
