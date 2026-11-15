using System;

namespace RestaurantDomain.Models
{
    public class TableReservation : EntityBase
    {
        private int _tableNumber;
        private string _customerName = string.Empty;
        private int _guestsCount;
        private DateTime _reservationTime;

        public int TableNumber
        {
            get => _tableNumber;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Номер столика має бути додатним.");
                _tableNumber = value;
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я клієнта не може бути порожнім.");
                _customerName = value.Trim();
            }
        }

        public int GuestsCount
        {
            get => _guestsCount;
            set
            {
                if (value <= 0 || value > 20)
                    throw new ArgumentException("Кількість гостей має бути від 1 до 20.");
                _guestsCount = value;
            }
        }

        public DateTime ReservationTime
        {
            get => _reservationTime;
            set => _reservationTime = value;
        }

        public TableReservation() { }

        public TableReservation(int tableNumber, string customerName, int guestsCount, DateTime reservationTime)
        {
            TableNumber = tableNumber;
            CustomerName = customerName;
            GuestsCount = guestsCount;
            ReservationTime = reservationTime;
        }

        public override string GetSummary()
        {
            return $"Столик №{TableNumber} | Гість: {CustomerName} ({GuestsCount} ос.) | Час: {ReservationTime:dd.MM.yyyy HH:mm}";
        }
    }
}