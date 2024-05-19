using Relaxing_Kaola;
using System;
using System.Collections.Generic;

public class Program
{
    static DatabaseManager dbManager = new DatabaseManager();
    static NotificationService notificationService = new NotificationService(dbManager);
    static TableManager tableManager = new TableManager(dbManager);
    static MenuManager menuManager = new MenuManager(dbManager);
    static OrderManager orderManager = new OrderManager(dbManager, menuManager);
    static ReservationSystem reservationSystem = new ReservationSystem(dbManager, notificationService, tableManager);
    static DigitalPayment paymentProcessor = new DigitalPayment(dbManager, notificationService);


    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Restaurant Reservation and Ordering System");
        Console.WriteLine("Are you a customer or staff? (customer/staff)");
        string userType = Console.ReadLine().ToLower();

        if (userType == "customer")
        {
            CustomerInterface();
        }
        else if (userType == "staff")
        {
            StaffInterface();
        }
        else
        {
            Console.WriteLine("Invalid type entered. Exiting...");
        }
    }

    static void CustomerInterface()
    {
        Console.WriteLine("Enter your name:");
        string name = Console.ReadLine();
        var customerRecord = dbManager.FindRecords("Customers", name).FirstOrDefault();
        if (customerRecord != null)
        {
            int customerId = int.Parse(customerRecord.Split(',')[0]);

            bool exit = false;
            while (!exit)
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
                        //ProcessPayment();
                        break;
                    case "4":
                        //GenerateStatistics();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
            
        }
        else
        {
            Console.WriteLine("Customer not found.");
        }
    }

    static void StaffInterface()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Make an Order For the Customer");
            Console.WriteLine("2. Make a Reservation for the Customer");
            Console.WriteLine("3. Cancel a Reservation for the Customer");
            Console.WriteLine("4. Generate Statistics");
            Console.WriteLine("5. Exit");
         
           

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                   // MakeAnOrder(customerId);
                    break;
                case "2":
                    Console.WriteLine("Enter the customer name:");
                    string name = Console.ReadLine();
                    var customerRecord = dbManager.FindRecords("Customers", name).FirstOrDefault();
                    if (customerRecord != null)
                    {
                        int customerId = int.Parse(customerRecord.Split(',')[0]);
                        MakeAReservation(customerId);
                    }
                    else
                    {
                        Console.WriteLine("Customer not found.");
                    }
                    break;
                case "3":
                    Console.WriteLine("Enter reservation ID to cancel:");
                    int reservationId = int.Parse(Console.ReadLine());
                    if (reservationSystem.CancelReservation(reservationId))
                    {
                        Console.WriteLine("Reservation cancelled successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to cancel reservation.");
                    }
                    break;
                case "4":
                    //ProcessPayment();
                    Statistics stats = new Statistics(dbManager);
                    stats.DisplayItemSalesStatistics();
                    break;
                case "5":
                    exit = true;
                    break;
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

        Console.WriteLine("Enter the item numbers you want to order (comma separated):");
        var input = Console.ReadLine();
        var ids = input.Split(',');
        Dictionary<int, int> itemQuantities = new Dictionary<int, int>();

        foreach (var id in ids)
        {
            Console.WriteLine($"Enter quantity for item ID {id.Trim()}:");
            int quantity = int.Parse(Console.ReadLine());
            if (int.TryParse(id.Trim(), out int itemId))
            {
                itemQuantities.Add(itemId, quantity);
            }
        }

        int orderId = orderManager.CreateOrder(customerId, itemQuantities);
        if (orderId != -1)
        {
            double totalAmount = orderManager.CalculateTotalAmount(itemQuantities);
            Console.WriteLine($"Order {orderId} placed successfully.");
            Console.WriteLine($"Total amount due: ${totalAmount}");

            // Automatically initiate the payment process using the customer's name
            Console.WriteLine("Please confirm the payment amount to proceed:");
            double enteredAmount = double.Parse(Console.ReadLine());

            // Use the customer's name to initiate the payment process
            ProcessPayment(customerName, enteredAmount, orderId, totalAmount);
        }
        else
        {
            Console.WriteLine("Failed to create the order.");
        }
    }

    static void ProcessPayment(string customerName, double enteredAmount, int orderId, double totalAmount)
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
                }
                else
                {
                    Console.WriteLine("Payment failed.");
                }
            }
            else
            {
                Console.WriteLine("Invalid debit card number.");
            }
        }
        else
        {
            Console.WriteLine("Entered amount does not match the total order amount.");
        }
    }




    static void MakeAReservation(int customerId)
    {
        Console.WriteLine("Checking for available tables...");
        var availableTables = tableManager.ListAvailableTables();  // This method needs to be implemented in TableManager

        if (availableTables.Any())
        {
            Console.WriteLine("Available Tables:");
            foreach (var table in availableTables)
            {
                Console.WriteLine($"Table ID: {table.TableId}, Capacity: {table.Capacity}");
            }

            Console.WriteLine("Enter the Table ID you would like to reserve:");
            int tableId = int.Parse(Console.ReadLine());

           // Console.WriteLine("Enter your Customer ID:");
           // int customerId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Date of Reservation (yyyy-mm-dd):");
            DateTime reservationDate = DateTime.Parse(Console.ReadLine());

            if (reservationSystem.MakeReservation(customerId, tableId, reservationDate))
            {
                Console.WriteLine("Your reservation has been made successfully.");
                notificationService.SendAlert(customerId, "Your reservation is confirmed.");
            }
            else
            {
                Console.WriteLine("Failed to make the reservation. The table might not be available.");
            }
        }
        else
        {
            Console.WriteLine("No available tables at the moment.");
        }
    }


    /*static void GenerateStatistics()
    {
        Statistics stats = new Statistics(dbManager);
        stats.GenerateSalesReport();
        stats.AnalyzeCustomerTrends();
        stats.AnalyzeMostSellingItems();
    }*/
}
