using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class OrderManager
    {
        private DatabaseManager dbManager;
        private MenuManager menuManager;

        public OrderManager(DatabaseManager dbManager, MenuManager menuManager)
        {
            this.dbManager = dbManager;
            this.menuManager = menuManager;
        }

        public int CreateOrder(int customerId, Dictionary<int, int> selectedItemQuantities)
        {
            var orderDetails = menuManager.PrepareOrderSelection(selectedItemQuantities);
            double totalAmount = CalculateTotalAmount(selectedItemQuantities);
            int newOrderId = dbManager.GetAllRecords("Orders").Count + 1;

            string newOrder = $"{newOrderId},{customerId},'{orderDetails}','Pending',{totalAmount}";
            bool success = dbManager.CreateRecord("Orders", newOrder);
            return success ? newOrderId : -1;  // Return the new order ID if successful, otherwise -1
        }

        public double CalculateTotalAmount(Dictionary<int, int> selectedItemQuantities)
        {
            double totalAmount = 0.0;
            var allItems = menuManager.ListAvailableItems();
            foreach (var entry in selectedItemQuantities)
            {
                var item = allItems.FirstOrDefault(it => it.StartsWith(entry.Key.ToString() + ","));
                if (item != null)
                {
                    double price = double.Parse(item.Split(',')[2], CultureInfo.InvariantCulture);
                    totalAmount += price * entry.Value;
                }
            }
            return totalAmount;
        }

        public bool UpdateOrder(int orderId, string newDetails)
        {
            return dbManager.UpdateRecord("Orders", $"{orderId},", $"{orderId},{newDetails}");
        }

        public bool FinalizeOrder(int orderId)
        {
            string update = $"{orderId},Completed";
            return dbManager.UpdateRecord("Orders", orderId + ",", update);
        }
    }

}
