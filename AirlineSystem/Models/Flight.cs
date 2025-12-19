using System;

namespace AirlineSystem.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public string FlightNumber { get; set; }
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }
        public string RouteInfo => $"{DeparturePoint} → {DestinationPoint}";
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsTouristSeason { get; set; }
        public int RouteID { get; set; }
        public int AircraftID { get; set; }

        public Flight()
        {
            FlightNumber = "";
            DeparturePoint = "";
            DestinationPoint = "";
            Status = "Scheduled";
        }

        public Flight(string flightNumber, string departure, string destination,
                     DateTime departureTime, DateTime arrivalTime, decimal price, int seats)
        {
            FlightNumber = flightNumber;
            DeparturePoint = departure;
            DestinationPoint = destination;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Price = price;
            AvailableSeats = seats;
            Status = "Scheduled";
            IsTouristSeason = false;
        }

        public bool CanBeBooked()
        {
            return Status == "Scheduled" && AvailableSeats > 0;
        }

        public void BookSeat()
        {
            if (AvailableSeats > 0)
                AvailableSeats--;
        }

        public void CancelSeat()
        {
            AvailableSeats++;
        }

        public decimal CalculateRevenue(int bookedSeats)
        {
            return Price * bookedSeats;
        }

        public bool IsDelayed()
        {
            return Status == "Delayed";
        }

        public bool IsCompleted()
        {
            return Status == "Completed";
        }
    }
}