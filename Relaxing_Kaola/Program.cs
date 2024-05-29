using Relaxing_Kaola;
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    static DatabaseManager dbManager = new DatabaseManager(@"C:\Users\Shaugato\source\repos\Software Acritectue\Relaxing_Kaola\Relaxing_Kaola\files");
    static NotificationService notificationService = new NotificationService(dbManager);
    static TableManager tableManager = new TableManager(dbManager);
    static MenuManager menuManager = new MenuManager(dbManager);
    static OrderManager orderManager = new OrderManager(dbManager, menuManager);
    static ReservationSystem reservationSystem = new ReservationSystem(dbManager, notificationService, tableManager);
    static DigitalPayment paymentProcessor = new DigitalPayment(dbManager, notificationService);

    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Restaurant Reservation and Ordering System");
        while (true)
        {
            Console.WriteLine("Are you a customer or staff? (customer/staff)");
            string userType = Console.ReadLine().ToLower();

            if (userType == "customer")
            {
                CustomerInterface();
                break;
            }
            else if (userType == "staff")
            {
                StaffInterface();
                break;
            }
            else
            {
                Console.WriteLine("Invalid type entered. Please enter 'customer' or 'staff'.");
            }
        }
    }

    static void CustomerInterface()
    {
        while (true)
        {
            Console.WriteLine("Enter your name:");
            string name = Console.ReadLine();
            var customerRecord = dbManager.FindRecords("Customers", name).FirstOrDefault();
            if (customerRecord != null)
            {
                int customerId = int.Parse(customerRecord.Split(',')[0]);

                while (true)
                {
                    Console.WriteLine("Select an option:");
                    Console.WriteLine("1. Make an Order");
                    Console.WriteLine("2. Make a Reservation");
                    Console.WriteLine("3. Process Payment");
                    Console.WriteLine("4. Generate Statistics");
                    Console.WriteLine("5. Exit");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            MakeAnOrder(customerId);
                            break;
                        case "2":
                            MakeAReservation(customerId);
                            break;
                        case "3":
                            // Implement process payment functionality if needed
                            break;
                        case "4":
                            // Implement generate statistics functionality if needed
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Customer not found. Please re-enter your name.");
            }
        }
    }

    static void StaffInterface()
    {
        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Make an Order For the Customer");
            Console.WriteLine("2. Make a Reservation for the Customer");
            Console.WriteLine("3. Cancel a Reservation for the Customer");
            Console.WriteLine("4. Generate Statistics");
            Console.WriteLine("5. Process a Refund");
            Console.WriteLine("6. Exit");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    // Implement make an order for customer functionality if needed
                    break;
                case "2":
                    while (true)
                    {
                        Console.WriteLine("Enter the customer name:");
                        string name = Console.ReadLine();
                        var customerRecord = dbManager.FindRecords("Customers", name).FirstOrDefault();
                        if (customerRecord != null)
                        {
                            int customerId = int.Parse(customerRecord.Split(',')[0]);
                            MakeAReservation(customerId);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Customer not found. Please re-enter the customer name.");
                        }
                    }
                    break;
                case "3":
                    while (true)
                    {
                        Console.WriteLine("Enter reservation ID to cancel:");
                        if (int.TryParse(Console.ReadLine(), out int reservationId))
                        {
                            if (reservationSystem.CancelReservation(reservationId))
                            {
                                Console.WriteLine("Reservation cancelled successfully.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Failed to cancel reservation. Please re-enter the reservation ID.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid reservation ID. Please enter a valid number.");
                        }
                    }
                    break;
                case "4":
                    Statistics stats = new Statistics(dbManager);
                    stats.DisplayItemSalesStatistics();
                    break;
                case "5":
                    ProcessRefund();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }
    }

    static void MakeAnOrder(int customerId)
    {
        string customerName = dbManager.GetCustomerNameById(customerId);
        if (string.IsNullOrEmpty(customerName))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        Console.WriteLine($"Hello, {customerName}. Available Menu Items:");
        var items = menuManager.ListAvailableItems();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }

        Dictionary<int, int> itemQuantities = new Dictionary<int, int>();
        while (true)
        {
            Console.WriteLine("Enter the item numbers you want to order (comma separated):");
            var input = Console.ReadLine();
            var ids = input.Split(',');

            bool valid = true;
            foreach (var id in ids)
            {
                Console.WriteLine($"Enter quantity for item ID {id.Trim()}:");
                if (int.TryParse(Console.ReadLine(), out int quantity) && int.TryParse(id.Trim(), out int itemId))
                {
                    itemQuantities[itemId] = quantity;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please re-enter the item numbers and quantities.");
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                break;
            }
        }

        int orderId = orderManager.CreateOrder(customerId, itemQuantities);
        if (orderId != -1)
        {
            double totalAmount = orderManager.CalculateTotalAmount(itemQuantities);
            Console.WriteLine($"Order {orderId} placed successfully.");
            Console.WriteLine($"Total amount due: ${totalAmount}");

            while (true)
            {
                Console.WriteLine("Please confirm the payment amount to proceed:");
                if (double.TryParse(Console.ReadLine(), out double enteredAmount))
                {
                    ProcessPayment(customerName, enteredAmount, orderId, totalAmount);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid amount entered. Please re-enter the amount.");
                }
            }
        }
        else
        {
            Console.WriteLine("Failed to create the order.");
        }
    }

    static void ProcessPayment(string customerName, double enteredAmount, int orderId, double totalAmount)
    {
        while (true)
        {
            Console.WriteLine("Please enter your debit card number:");
            string cardNumber = Console.ReadLine();

            if (Math.Abs(enteredAmount - totalAmount) < 0.01)
            {
                if (paymentProcessor.ValidateCardNumber(cardNumber))
                {
                    if (paymentProcessor.ProcessPayment(customerName, enteredAmount, cardNumber))
                    {
                        Console.WriteLine("Payment processed successfully.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Payment failed. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid debit card number. Please re-enter the card number.");
                }
            }
            else
            {
                Console.WriteLine("Entered amount does not match the total order amount. Please re-enter the amount.");
            }
        }
    }

    



    static void MakeAReservation(int customerId)
    {
        Console.WriteLine("Checking for available tables...");
        var availableTables = tableManager.ListAvailableTables();

        if (availableTables.Any())
        {
            Console.WriteLine("Available Tables:");
            foreach (var table in availableTables)
            {
                Console.WriteLine($"Table ID: {table.TableId}, Capacity: {table.Capacity}");
            }

            while (true)
            {
                Console.WriteLine("Enter the Table ID you would like to reserve:");
                if (int.TryParse(Console.ReadLine(), out int tableId))
                {
                    Console.WriteLine("Enter Date of Reservation (yyyy-mm-dd):");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime reservationDate))
                    {
                        if (reservationSystem.MakeReservation(customerId, tableId, reservationDate))
                        {
                            Console.WriteLine("Your reservation has been made successfully.");
                            notificationService.SendAlert(customerId, "Your reservation is confirmed.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Failed to make the reservation. The table might not be available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please enter the date in yyyy-mm-dd format.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Table ID. Please enter a valid number.");
                }
            }
        }
        else
        {
            Console.WriteLine("No available tables at the moment.");
        }
    }

    static void ProcessRefund()
    {
        while (true)
        {
            Console.WriteLine("Enter the order ID to refund:");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                if (paymentProcessor.RefundPayment(orderId))
                {
                    Console.WriteLine("Refund processed successfully.");
                    if (orderManager.RemoveOrder(orderId))
                    {
                        Console.WriteLine("Order removed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove the order.");
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Refund failed. Please re-enter the order ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid order ID. Please enter a valid number.");
            }
        }
    }
}
