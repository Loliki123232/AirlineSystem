using System;

namespace AirlineSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int PassengerID { get; set; }
        public int FlightID { get; set; }
        public DateTime BookingDate { get; set; }
        public string SeatNumber { get; set; } = "";
        public decimal Price { get; set; }
        public string Status { get; set; } = "Confirmed";

        public Booking()
        {
            BookingDate = DateTime.Now;
            Status = "Confirmed";
        }

        public Booking(int passengerId, int flightId, string seatNumber, decimal price)
        {
            PassengerID = passengerId;
            FlightID = flightId;
            SeatNumber = seatNumber ?? "";
            Price = price;
            BookingDate = DateTime.Now;
            Status = "Confirmed";
        }

        public bool CanBeCancelled()
        {
            return Status == "Confirmed";
        }

        public void Cancel()
        {
            Status = "Cancelled";
        }

        public bool IsCompleted()
        {
            return Status == "Completed";
        }
    }
}