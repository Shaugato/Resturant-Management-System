using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Relaxing_Kaola
{
    public class DigitalPayment : PaymentProcessor
    {
        private DatabaseManager DbManager;
        private NotificationService NotificationService;

        public DigitalPayment(DatabaseManager dbManager, NotificationService notificationService)
        {
            DbManager = dbManager;
            NotificationService = notificationService;
        }


        public bool ProcessPayment(string customerName, double enteredAmount, string cardNumber)
        {
            int customerId = DbManager.GetCustomerIdByName(customerName);
            if (customerId == -1)
            {
                Console.WriteLine("Invalid customer name.");
                return false;
            }

            string orderDetails = DbManager.GetLatestOrderForCustomer(customerId);
            if (orderDetails == null)
            {
                Console.WriteLine("Order not found.");
                return false;
            }

            string[] details = orderDetails.Split(',');
            double calculatedAmount = double.Parse(details[4], CultureInfo.InvariantCulture); // Assuming index 4 is TotalAmount

            if (Math.Abs(calculatedAmount - enteredAmount) > 0.01)
            {
                Console.WriteLine($"Entered amount does not match the total order amount.");
                return false;
            }

            if (ValidateCardNumber(cardNumber))
            {
                string paymentRecord = $"{DbManager.GetAllRecords("Payments").Count + 1},{details[0]},{enteredAmount.ToString(CultureInfo.InvariantCulture)},Digital,Completed";
                DbManager.CreateRecord("Payments", paymentRecord);
                NotificationService.SendAlert(customerId, $"Payment of ${enteredAmount} has been processed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid debit card number.");
                return false;
            }
        }





        public bool ValidateCardNumber(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                var digit = (int)char.GetNumericValue(cardNumber[i]);
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit = (digit % 10) + 1;
                    }
                }
                sum += digit;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }



        public bool RefundPayment(int paymentId)
        {
            var paymentDetails = DbManager.FindRecords("Payments", $"{paymentId},").FirstOrDefault();
            if (paymentDetails != null)
            {
                var fields = paymentDetails.Split(',');
                int orderId = int.Parse(fields[1]);
                var orderDetails = DbManager.FindRecords("Orders", $"{orderId},").FirstOrDefault();
                int customerId = int.Parse(orderDetails.Split(',')[1]);
                string updatedPayment = paymentDetails.Replace("Completed", "Refunded");
                if (DbManager.UpdateRecord("Payments", paymentId + ",", updatedPayment))
                {
                    NotificationService.SendAlert(customerId, $"Your payment for order {orderId} has been refunded.");
                    return true;
                }
            }
            return false;
        }

        /*private double CalculateTotalAmount(string orderDetails)
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
        }*/
    }

}
