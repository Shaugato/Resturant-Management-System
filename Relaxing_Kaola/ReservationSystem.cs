using System;

namespace Relaxing_Kaola
{
    public class ReservationSystem
    {
        private DatabaseManager DbManager;
        private NotificationService NotificationService;
        private TableManager TableManager;

        public ReservationSystem(DatabaseManager dbManager, NotificationService notificationService, TableManager tableManager)
        {
            DbManager = dbManager;
            NotificationService = notificationService;
            TableManager = tableManager;
        }

        public bool MakeReservation(int customerId, int tableId, DateTime reservationDate)
        {
            while (!TableManager.UpdateTableStatus(tableId, "Reserved"))
            {
                Console.WriteLine("Failed to reserve the table. Please re-enter the table ID:");
                tableId = int.Parse(Console.ReadLine());
            }

            string reservationRecord = $"{DbManager.GetAllRecords("Reservations").Count + 1},{customerId},{tableId},{reservationDate:yyyy-MM-dd},Confirmed";
            bool success = DbManager.CreateRecord("Reservations", reservationRecord);

            while (!success)
            {
                Console.WriteLine("Failed to create reservation. Please re-enter the reservation details.");
                Console.WriteLine("Please re-enter the table ID:");
                tableId = int.Parse(Console.ReadLine());
                Console.WriteLine("Please re-enter the reservation date (format: yyyy-MM-dd):");
                reservationDate = DateTime.Parse(Console.ReadLine());

                reservationRecord = $"{DbManager.GetAllRecords("Reservations").Count + 1},{customerId},{tableId},{reservationDate:yyyy-MM-dd},Confirmed";
                success = DbManager.CreateRecord("Reservations", reservationRecord);
            }

            NotificationService.SendAlert(customerId, $"Reservation ID: {DbManager.GetAllRecords("Reservations").Count} confirmed.");
            return true;
        }

        public bool CancelReservation(int reservationId)
        {
            var reservation = DbManager.FindRecords("Reservations", $"{reservationId},").FirstOrDefault();
            if (reservation != null)
            {
                var fields = reservation.Split(',');
                int tableId = int.Parse(fields[2]);
                if (DbManager.DeleteRecord("Reservations", $"{reservationId},"))
                {
                    TableManager.UpdateTableStatus(tableId, "Available");
                    int customerId = int.Parse(fields[1]);
                    NotificationService.SendAlert(customerId, "Your reservation has been canceled.");
                    return true;
                }
            }
            return false;
        }
    }
}
