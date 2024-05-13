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
        public string PrepareOrderSelection(List<int> selectedItemIds)
        {
            var allItems = ListAvailableItems();
            List<string> selectedItems = new List<string>();

            foreach (int id in selectedItemIds)
            {
                var item = allItems.FirstOrDefault(it => it.StartsWith(id.ToString() + ","));
                if (item != null)
                {
                    selectedItems.Add(item);
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
