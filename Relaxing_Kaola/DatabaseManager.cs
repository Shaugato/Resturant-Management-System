using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Relaxing_Kaola
{
    public class DatabaseManager
    {
        private readonly string basePath;

        public DatabaseManager(string path)
        {
            basePath = path;
        }

        public bool CreateRecord(string tableName, string record)
        {
            try
            {
                string filePath = Path.Combine(basePath, $"{tableName}.txt");
                File.AppendAllText(filePath, record + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool UpdateRecord(string tableName, string identifier, string newData)
        {
            try
            {
                string filePath = Path.Combine(basePath, $"{tableName}.txt");
                var lines = File.ReadAllLines(filePath).ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith(identifier))
                    {
                        lines[i] = newData;
                        File.WriteAllLines(filePath, lines);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public List<string> FindRecords(string tableName, string criteria)
        {
            try
            {
                string filePath = Path.Combine(basePath, $"{tableName}.txt");
                return File.ReadAllLines(filePath).Where(line => line.Contains(criteria)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching for records: {ex.Message}");
                return new List<string>();
            }
        }

        public bool DeleteRecord(string tableName, string identifier)
        {
            try
            {
                string filePath = Path.Combine(basePath, $"{tableName}.txt");
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
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public List<string> GetAllRecords(string tableName)
        {
            try
            {
                string filePath = Path.Combine(basePath, $"{tableName}.txt");
                return File.ReadAllLines(filePath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching all records: {ex.Message}");
                return new List<string>();
            }
        }

        public int GetCustomerIdByName(string name)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Customers.txt");
                var customerRecord = File.ReadAllLines(filePath)
                    .Skip(1)
                    .FirstOrDefault(line => line.Split(',')[2].Trim('"').Equals(name, StringComparison.OrdinalIgnoreCase));

                return customerRecord != null ? int.Parse(customerRecord.Split(',')[0]) : -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching customer ID: {ex.Message}");
                return -1;
            }
        }

        public string GetCustomerNameById(int customerId)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Customers.txt");
                var customerLine = File.ReadAllLines(filePath)
                    .Skip(1)
                    .FirstOrDefault(line => line.StartsWith($"{customerId},"));

                return customerLine?.Split(',')[2].Trim('"');
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve customer name: {ex.Message}");
                return null;
            }
        }

        public string GetLatestOrderForCustomer(int customerId)
        {
            try
            {
                string filePath = Path.Combine(basePath, "Orders.txt");
                var orders = File.ReadAllLines(filePath)
                    .Skip(1)
                    .Where(line => line.Split(',')[1].Trim() == customerId.ToString())
                    .OrderByDescending(line => int.Parse(line.Split(',')[0].Trim()))
                    .FirstOrDefault();

                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching the latest order for customer: {ex.Message}");
                return null;
            }
        }
    }
}
