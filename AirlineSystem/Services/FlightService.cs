using AirlineSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AirlineSystem.Services
{
    public class FlightService
    {
        private readonly string _connectionString;

        public FlightService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Flight> GetAllFlights()
        {
            var flights = new List<Flight>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT f.*, r.DeparturePoint, r.DestinationPoint 
                    FROM Flights f
                    INNER JOIN Routes r ON f.RouteID = r.RouteID
                    ORDER BY f.DepartureTime";

                var command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        flights.Add(new Flight
                        {
                            FlightID = Convert.ToInt32(reader["FlightID"]),
                            FlightNumber = reader["FlightNumber"].ToString(),
                            DeparturePoint = reader["DeparturePoint"].ToString(),
                            DestinationPoint = reader["DestinationPoint"].ToString(),
                            DepartureTime = Convert.ToDateTime(reader["DepartureTime"]),
                            ArrivalTime = Convert.ToDateTime(reader["ArrivalTime"]),
                            Status = reader["Status"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            AvailableSeats = Convert.ToInt32(reader["AvailableSeats"]),
                            IsTouristSeason = Convert.ToBoolean(reader["IsTouristSeason"]),
                            RouteID = Convert.ToInt32(reader["RouteID"]),
                            AircraftID = Convert.ToInt32(reader["AircraftID"])
                        });
                    }
                }
            }

            return flights;
        }

        public bool AddFlight(Flight flight)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Flights (FlightNumber, RouteID, AircraftID, DepartureTime, 
                                        ArrivalTime, Status, Price, AvailableSeats, IsTouristSeason)
                    VALUES (@FlightNumber, @RouteID, @AircraftID, @DepartureTime, 
                            @ArrivalTime, @Status, @Price, @AvailableSeats, @IsTouristSeason)";

                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@FlightNumber", flight.FlightNumber);
                command.Parameters.AddWithValue("@RouteID", flight.RouteID);
                command.Parameters.AddWithValue("@AircraftID", flight.AircraftID);
                command.Parameters.AddWithValue("@DepartureTime", flight.DepartureTime);
                command.Parameters.AddWithValue("@ArrivalTime", flight.ArrivalTime);
                command.Parameters.AddWithValue("@Status", flight.Status);
                command.Parameters.AddWithValue("@Price", flight.Price);
                command.Parameters.AddWithValue("@AvailableSeats", flight.AvailableSeats);
                command.Parameters.AddWithValue("@IsTouristSeason", flight.IsTouristSeason);

                connection.Open();
                int result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public bool UpdateFlight(Flight flight)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Flights 
                    SET FlightNumber = @FlightNumber,
                        Status = @Status,
                        Price = @Price,
                        AvailableSeats = @AvailableSeats,
                        IsTouristSeason = @IsTouristSeason
                    WHERE FlightID = @FlightID";

                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@FlightID", flight.FlightID);
                command.Parameters.AddWithValue("@FlightNumber", flight.FlightNumber);
                command.Parameters.AddWithValue("@Status", flight.Status);
                command.Parameters.AddWithValue("@Price", flight.Price);
                command.Parameters.AddWithValue("@AvailableSeats", flight.AvailableSeats);
                command.Parameters.AddWithValue("@IsTouristSeason", flight.IsTouristSeason);

                connection.Open();
                int result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public bool DeleteFlight(int flightId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Flights WHERE FlightID = @FlightID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FlightID", flightId);

                connection.Open();
                int result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public List<Flight> SearchFlights(string searchText, string statusFilter, DateTime? dateFilter)
        {
            var allFlights = GetAllFlights();
            var filtered = allFlights.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string searchLower = searchText.ToLower();
                filtered = filtered.Where(f =>
                    f.FlightNumber.ToLower().Contains(searchLower) ||
                    f.DeparturePoint.ToLower().Contains(searchLower) ||
                    f.DestinationPoint.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "Все статусы")
            {
                filtered = filtered.Where(f => f.Status == statusFilter);
            }

            if (dateFilter.HasValue)
            {
                filtered = filtered.Where(f => f.DepartureTime.Date == dateFilter.Value.Date);
            }

            return filtered.ToList();
        }
    }
}