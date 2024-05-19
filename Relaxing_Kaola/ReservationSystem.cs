using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool StartReservation(int customerId, DateTime date, int partySize)
        {
            if (TableManager.CheckTableAvailability(date, partySize))
            {
                string record = $"{DbManager.GetAllRecords("Reservations").Count + 1},{customerId},{date},{partySize},Pending";
                DbManager.CreateRecord("Reservations", record);
                NotificationService.SendAlert(customerId, "Reservation started and pending confirmation.");
                return true;
            }
            else
            {
                NotificationService.SendAlert(customerId, "No table available for the selected date and party size.");
                return false;
            }
        }

        public bool MakeReservation(int customerId, int tableId, DateTime reservationDate)
        {
            /*if (TableManager.IsTableAvailable(tableId, reservationDate))
            {
                string reservationRecord = $"{DbManager.GetAllRecords("Reservations").Count + 1},{customerId},{tableId},{reservationDate.ToString("yyyy-MM-dd")},{DateTime.Now.ToString("HH:mm")},Confirmed";
                if (DbManager.CreateRecord("Reservations", reservationRecord))
            {
                    NotificationService.SendAlert(customerId, $"Reservation confirmed for table {tableId} on {reservationDate.ToShortDateString()}.");
                    return true;
                }
            }
            else
            {
                NotificationService.SendAlert(customerId, "Selected table is not available on the chosen date.");
            }
            return false;*/

            if (TableManager.UpdateTableStatus(tableId, "Reserved"))
            {
                string reservationRecord = $"{DbManager.GetAllRecords("Reservations").Count + 1},{customerId},{tableId},{reservationDate:yyyy-MM-dd},Confirmed";
                if (DbManager.CreateRecord("Reservations", reservationRecord))
                {
                    NotificationService.SendAlert(customerId, $"Reservation ID : {DbManager.GetAllRecords("Reservations").Count } And Reservation confirmed.");
                    return true;

                }
                else
                {
                    // Roll back table status update if reservation fails
                    TableManager.UpdateTableStatus(tableId, "Available");
                }
            }
            return false;
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

        public bool FinalizeReservation(int reservationId)
        {
            string update = $"{reservationId},Confirmed";
            return DbManager.UpdateRecord("Reservations", reservationId + ",", update);
        }
    }

}
