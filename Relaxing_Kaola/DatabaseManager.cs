using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Relaxing_Kaola
{
    public class DatabaseManager
    {
        private string basePath = @"C:\Users\Shaugato\source\repos\Software Acritectue\Relaxing_Kaola\Relaxing_Kaola\files"; // Update this path as necessary

        public bool CreateRecord(string tableName, string record)
        {
            try
            {
                string filePath = Path.Combine(basePath, tableName + ".txt");
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(record);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        public bool UpdateRecord(string tableName, string identifier, string newData)
        {
            try
            {
                string filePath = Path.Combine(basePath, tableName + ".txt");
                var lines = File.ReadAllLines(filePath).ToList();
                bool found = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith(identifier))
                    {
                        lines[i] = newData;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    File.WriteAllLines(filePath, lines);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        public List<string> FindRecords(string tableName, string criteria)
        {
            try
            {
                string filePath = Path.Combine(basePath, tableName + ".txt");
                var lines = File.ReadAllLines(filePath);
                return lines.Where(line => line.Contains(criteria)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while searching for records: " + ex.Message);
                return new List<string>();
            }
        }

        public bool DeleteRecord(string tableName, string identifier)
        {
            try
            {
                string filePath = Path.Combine(basePath, tableName + ".txt");
                var lines = File.ReadAllLines(filePath).ToList();
                int index = lines.FindIndex(line => line.StartsWith(identifier));

                if (index != -1)
                {
                    lines.RemoveAt(index);
                    File.WriteAllLines(filePath, lines);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        public List<string> GetAllRecords(string tableName)
        {
            try
            {
                string filePath = Path.Combine(basePath, tableName + ".txt");
                return File.ReadAllLines(filePath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching all records: " + ex.Message);
                return new List<string>();
            }
        }
        public int GetCustomerIdByName(string name)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Customers.txt");
                var customerRecord = File.ReadAllLines(filePath)
                    .Skip(1)  // Assuming there is a header
                    .Where(line => line.Split(',')[2].Trim('"').Equals(name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (customerRecord != null)
                {
                    return int.Parse(customerRecord.Split(',')[0]);
                }
                Console.WriteLine("Customer not found.");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching customer ID: " + ex.Message);
                return -1;
            }
        }
        public string GetCustomerNameById(int customerId)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Customers.txt");
                var customerLine = File.ReadAllLines(filePath)
                    .Skip(1) // Skip header
                    .FirstOrDefault(line => line.StartsWith(customerId.ToString() + ","));

                if (customerLine != null)
                {
                    return customerLine.Split(',')[2].Trim('"'); // Assuming name is the third column
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to retrieve customer name: " + ex.Message);
                return null;
            }
        }


        public string GetLatestOrderForCustomer(int customerId)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Orders.txt");
                var orders = File.ReadAllLines(filePath)
                    .Skip(1) // Skip the header row
                    .Where(line => line.Split(',')[1].Trim() == customerId.ToString())
                    .Select(line => new
                    {
                        Order = line,
                        OrderId = int.Parse(line.Split(',')[0].Trim())
                    })
                    .OrderByDescending(o => o.OrderId)
                    .FirstOrDefault();

                return orders?.Order;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching the latest order for customer: " + ex.Message);
                return null;
            }
        }
    }
}
