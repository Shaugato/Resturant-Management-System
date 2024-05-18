using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class MenuManager
    {
        private DatabaseManager DbManager;

        public MenuManager(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public List<string> ListAvailableItems()
        {
            return DbManager.GetAllRecords("MenuItems");
        }

        public List<string> FilterItemsByDiet(string dietaryRestrictions)
        {
            return DbManager.FindRecords("MenuItems", dietaryRestrictions);
        }
        public string PrepareOrderSelection(Dictionary<int, int> selectedItemQuantities)
        {
            var allItems = ListAvailableItems();
            List<string> selectedItems = new List<string>();

            foreach (var entry in selectedItemQuantities)
            {
                var itemId = entry.Key;
                var quantity = entry.Value;
                var item = allItems.FirstOrDefault(it => it.StartsWith(itemId.ToString() + ","));
                if (item != null)
                {
                    selectedItems.Add($"{quantity}x {item.Split(',')[1]}");
                }
            }

            return string.Join("; ", selectedItems);
        }

        public bool UpdateMenuItem(string itemId, string newName, string newPrice, string newDietary)
        {
            string newData = $"{itemId},{newName},{newPrice},{newDietary}";
            return DbManager.UpdateRecord("MenuItems", itemId + ",", newData);
        }
    }

}
