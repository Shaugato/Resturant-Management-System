using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class TableManager
    {
        private DatabaseManager DbManager;
        public TableManager(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public class Table
        {
            public int TableId { get; set; }
            public int Capacity { get; set; }
            public string Status { get; set; }

            public Table(string record)
            {
                var fields = record.Split(',');
                TableId = int.Parse(fields[0]);
                Capacity = int.Parse(fields[1]);
                Status = fields[2];
            }
        }

        public List<Table> ListAvailableTables()
        { 
            return DbManager.GetAllRecords("Tables")
            .Select(record => new Table(record))
            .Where(table => table.Status == "Available")
            .ToList();
        }

        public bool IsTableAvailable(int tableId, DateTime date)
        {
            // This method checks if the table is available for a specific date
            var reservations = DbManager.FindRecords("Reservations", $"{tableId}");
            foreach (var res in reservations)
            {
                var fields = res.Split(',');
                DateTime reservedDate = DateTime.Parse(fields[3]);
                if (reservedDate.Date == date.Date)
                {
                    return false; // Table is not available on this date
                }
            }
            return true; // Table is available
        }

        public bool UpdateTableStatus(int tableId, string newStatus)
        {
            var record = DbManager.FindRecords("Tables", $"{tableId},").FirstOrDefault();
            if (record != null)
            {
                var fields = record.Split(',');
                string updatedRecord = $"{fields[0]},{fields[1]},{newStatus}";
                return DbManager.UpdateRecord("Tables", fields[0] + ",", updatedRecord);
            }
            return false;
        }
    }

}
