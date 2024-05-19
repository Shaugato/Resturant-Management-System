using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class Statistics
    {
        private DatabaseManager DbManager;

        public Statistics(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        // Method to analyze the most selling items
        public void DisplayItemSalesStatistics()
        {
            var orders = DbManager.GetAllRecords("Orders");
            Dictionary<string, (int quantity, double totalRevenue)> itemSales = new Dictionary<string, (int, double)>();
            double totalRevenue = 0;

            // Parse each order and update the item sales count and revenue
            foreach (var order in orders.Skip(1)) // Skip the header
            {
                var details = order.Split(',')[2].Trim('\'');  // Assuming item details are in the third column
                var totalAmount = double.Parse(order.Split(',')[4]); // Assuming total amount is in the fifth column
                var items = details.Split(';');

                foreach (var item in items)
                {
                    var parts = item.Trim().Split('x');
                    var quantity = int.Parse(parts[0].Trim());
                    var itemName = parts[1].Trim();

                    if (itemSales.ContainsKey(itemName))
                    {
                        itemSales[itemName] = (itemSales[itemName].quantity + quantity, itemSales[itemName].totalRevenue + quantity * totalAmount);
                    }
                    else
                    {
                        itemSales[itemName] = (quantity, quantity * totalAmount);
                    }
                }

                totalRevenue += totalAmount;
            }

            // Display the items sorted by quantity, descending
            Console.WriteLine("Item Sales Statistics:");
            Console.WriteLine("Item | Quantity Sold | Total Revenue");
            foreach (var item in itemSales.OrderByDescending(i => i.Value.quantity))
            {
                Console.WriteLine($"{item.Key}: {item.Value.quantity} units, ${item.Value.totalRevenue:N2}");
            }
            Console.WriteLine($"Total Revenue: ${totalRevenue:N2}");
        }
    }

}
