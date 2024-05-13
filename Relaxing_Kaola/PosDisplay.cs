using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class PosDisplay : Display
    {
        public override void DisplayMessage(string message)
        {
            Console.WriteLine("POS Display: " + message);
        }

        public override void ClearDisplay()
        {
            Console.WriteLine("POS Display Cleared");
        }

        public void ShowTransactionDetails(string transactionDetails)
        {
            DisplayMessage("Transaction Details: " + transactionDetails);
        }

        public void PromptForAction(string message)
        {
            DisplayMessage(message);
        }

        public void DisplayMenuItems(List<string> menuItems)
        {
            Console.WriteLine("Available Menu Items:");
            foreach (var item in menuItems)
            {
                Console.WriteLine(item);
            }
        }

        public void ShowOrderSummary(string orderSummary)
        {
            DisplayMessage("Order Summary: " + orderSummary);
        }
    }

}
