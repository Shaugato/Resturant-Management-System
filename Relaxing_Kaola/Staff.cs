using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class Staff : User
    {
        public string Role { get; set; }
        public string Name { get; set; }

        public Staff(DatabaseManager dbManager) : base(dbManager)
        {
        }

        public override bool Login(string email, string password)
        {
            var records = DbManager.FindRecords("Users", $"{email},{password}");
            return records.Count > 0;
        }

        public void GetStaffDetails(int staffId)
        {
            var records = DbManager.FindRecords("Staff", $"{staffId},");
            if (records.Count > 0)
            {
                var fields = records[0].Split(',');
                UserId = int.Parse(fields[0]);
                Role = fields[2];
                Name = fields[3];
            }
        }
    }

}
