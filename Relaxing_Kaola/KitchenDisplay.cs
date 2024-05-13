using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class KitchenDisplay : Display
    {
        public override void DisplayMessage(string message)
        {
            Console.WriteLine("Kitchen Display: " + message);
        }

        public override void ClearDisplay()
        {
            Console.WriteLine("Kitchen Display Cleared");
        }

        public void ShowOrderDetails(string orderDetails)
        {
            Console.WriteLine("Order Details: " + orderDetails);
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            DisplayMessage($"Order {orderId} is now {status}.");
        }
    }

}
