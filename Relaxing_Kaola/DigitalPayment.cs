using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public class DigitalPayment : PaymentProcessor
    {
        private DatabaseManager DbManager;

        public DigitalPayment(DatabaseManager dbManager)
        {
            DbManager = dbManager;
        }

        public bool ProcessPayment(int orderId, double amount)
        {
            // Retrieve the order to calculate the total amount
            var orderDetails = DbManager.FindRecords("Orders", orderId.ToString()).FirstOrDefault();
            if (orderDetails == null)
            {
                Console.WriteLine("Order not found.");
                return false;
            }

            // Calculate the total amount from order details
            double calculatedAmount = CalculateTotalAmount(orderDetails);
            if (calculatedAmount != amount)
            {
                Console.WriteLine("Payment amount does not match the order total.");
                return false;
            }

            // Simulate payment processing
            string paymentRecord = $"{DbManager.GetAllRecords("Payments").Count + 1},{orderId},{amount},Digital,Completed";
            DbManager.CreateRecord("Payments", paymentRecord);
            return true;
        }

        public bool RefundPayment(int paymentId)
        {
            // Simply mark the payment as refunded for simplicity
            var paymentDetails = DbManager.FindRecords("Payments", paymentId.ToString()).FirstOrDefault();
            if (paymentDetails == null)
            {
                Console.WriteLine("Payment not found.");
                return false;
            }

            string updatedPayment = paymentDetails.Replace("Completed", "Refunded");
            return DbManager.UpdateRecord("Payments", paymentId + ",", updatedPayment);
        }

        private double CalculateTotalAmount(string orderDetails)
        {
            // Assuming the order details format is "OrderId,CustomerId,Details,Status"
            var details = orderDetails.Split(',');
            var itemsDetails = details[2].Split(';'); // Assuming items details are separated by semicolon
            double totalAmount = 0.0;

            foreach (var itemDetail in itemsDetails)
            {
                var item = itemDetail.Trim().Split('x');
                var menuItem = DbManager.FindRecords("MenuItems", item[1].Trim()).FirstOrDefault();
                if (menuItem != null)
                {
                    var menuItemDetails = menuItem.Split(',');
                    totalAmount += double.Parse(menuItemDetails[2]) * int.Parse(item[0]);
                }
            }

            return totalAmount;
        }
    }

}
