using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class InventoryManager
    {
        private DatabaseManager DbManager;

        public InventoryManager(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public bool AdjustInventory(string itemName, int adjustment)
        {
            var itemDetails = DbManager.FindRecords("Inventory", itemName).FirstOrDefault();
            if (itemDetails != null)
            {
                var fields = itemDetails.Split(',');
                int currentQuantity = int.Parse(fields[2]);
                int newQuantity = currentQuantity + adjustment;
                if (newQuantity < 0)
                {
                    Console.WriteLine("Adjustment leads to negative inventory, operation aborted.");
                    return false;
                }

                string updatedRecord = $"{fields[0]},{fields[1]},{newQuantity}";
                return DbManager.UpdateRecord("Inventory", fields[0] + ",", updatedRecord);
            }
            return false;
        }

        public bool CheckStock(string itemName)
        {
            var itemDetails = DbManager.FindRecords("Inventory", itemName).FirstOrDefault();
            if (itemDetails != null)
            {
                Console.WriteLine($"Current stock for {itemName}: {itemDetails.Split(',')[2]}");
                return true;
            }
            Console.WriteLine($"Item {itemName} not found in inventory.");
            return false;
        }
    }

}
