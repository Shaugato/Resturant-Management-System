using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class OrderManager
    {
        private DatabaseManager DbManager;

        public OrderManager(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public bool CreateOrder(int customerId, string orderDetails)
        {
            string newOrder = $"{DbManager.GetAllRecords("Orders").Count + 1},{customerId},{orderDetails},Pending";
            return DbManager.CreateRecord("Orders", newOrder);
        }

        public bool UpdateOrder(int orderId, string newDetails)
        {
            return DbManager.UpdateRecord("Orders", $"{orderId},", $"{orderId},{newDetails}");
        }

        public bool FinalizeOrder(int orderId)
        {
            string update = $"{orderId},Completed";
            return DbManager.UpdateRecord("Orders", orderId + ",", update);
        }
    }

}
