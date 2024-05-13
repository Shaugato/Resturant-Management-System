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

        public void GenerateSalesReport()
        {
            var orders = DbManager.GetAllRecords("Orders");
            double totalSales = 0;
            foreach (var order in orders)
            {
                var fields = order.Split(',');
                totalSales += double.Parse(fields[2]); // Assuming the order total is in the third position
            }
            Console.WriteLine($"Total sales amount: ${totalSales}");
        }

        public void AnalyzeCustomerTrends()
        {
            var orders = DbManager.GetAllRecords("Orders");
            var customerOrders = orders.GroupBy(
                o => o.Split(',')[1], // Group by CustomerId
                o => o.Split(',')[2], // Sum the totals
                (key, g) => new { CustomerId = key, TotalSpent = g.Sum(s => double.Parse(s)) });

            Console.WriteLine("Customer Spending Trends:");
            foreach (var item in customerOrders)
            {
                Console.WriteLine($"Customer {item.CustomerId} spent a total of ${item.TotalSpent}");
            }
        }

        // Method to analyze the most selling items
        public void AnalyzeMostSellingItems()
        {
            var orders = DbManager.GetAllRecords("Orders");
            Dictionary<string, int> itemCounts = new Dictionary<string, int>();

            foreach (var order in orders)
            {
                var details = order.Split(',')[2]; // Assuming details are in the third column
                var items = details.Split(';'); // Assuming items in an order are separated by semicolon

                foreach (var item in items)
                {
                    var itemDetails = item.Trim().Split('x'); // Assuming format "2x Steak"
                    var itemName = itemDetails[1].Trim();
                    var quantity = int.Parse(itemDetails[0]);

                    if (itemCounts.ContainsKey(itemName))
                        itemCounts[itemName] += quantity;
                    else
                        itemCounts[itemName] = quantity;
                }
            }

            var mostSellingItems = itemCounts.OrderByDescending(kvp => kvp.Value).ToList();
            Console.WriteLine("Most Selling Items:");
            foreach (var item in mostSellingItems)
            {
                Console.WriteLine($"{item.Key}: {item.Value} sold");
            }
        }
    }

}
