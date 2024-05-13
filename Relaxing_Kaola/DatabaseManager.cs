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
    }
}
