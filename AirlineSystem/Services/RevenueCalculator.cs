using AirlineSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AirlineSystem.Services
{
    public class RevenueCalculator
    {
        private readonly string _connectionString;

        public RevenueCalculator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public decimal CalculateFlightRevenue(int flightId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ISNULL(SUM(b.Price), 0) as PassengerRevenue,
                        ISNULL(SUM(CASE WHEN c.Status = 'Confirmed' THEN c.Weight * 100 ELSE 0 END), 0) as CargoRevenue
                    FROM Flights f
                    LEFT JOIN Bookings b ON f.FlightID = b.FlightID AND b.Status = 'Confirmed'
                    LEFT JOIN Cargo c ON f.FlightID = c.FlightID
                    WHERE f.FlightID = @FlightID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FlightID", flightId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal passengerRevenue = Convert.ToDecimal(reader["PassengerRevenue"]);
                        decimal cargoRevenue = Convert.ToDecimal(reader["CargoRevenue"]);
                        return passengerRevenue + cargoRevenue;
                    }
                }
            }

            return 0;
        }

        public Dictionary<string, decimal> CalculateDailyRevenue(DateTime date)
        {
            var result = new Dictionary<string, decimal>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        f.FlightNumber,
                        ISNULL(SUM(b.Price), 0) as Revenue
                    FROM Flights f
                    LEFT JOIN Bookings b ON f.FlightID = b.FlightID 
                        AND b.Status = 'Confirmed'
                        AND CONVERT(DATE, b.BookingDate) = @Date
                    WHERE CONVERT(DATE, f.DepartureTime) = @Date
                    GROUP BY f.FlightNumber";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Date", date.Date);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string flightNumber = reader["FlightNumber"].ToString();
                        decimal revenue = Convert.ToDecimal(reader["Revenue"]);
                        result[flightNumber] = revenue;
                    }
                }
            }

            return result;
        }

        public decimal CalculateTotalRevenue(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT ISNULL(SUM(b.Price), 0) as TotalRevenue
                    FROM Bookings b
                    INNER JOIN Flights f ON b.FlightID = f.FlightID
                    WHERE b.Status = 'Confirmed'
                    AND f.DepartureTime BETWEEN @StartDate AND @EndDate";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                connection.Open();
                var result = command.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        public decimal CalculateAverageTicketPrice(int flightId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT AVG(Price) as AvgPrice
                    FROM Bookings
                    WHERE FlightID = @FlightID AND Status = 'Confirmed'";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FlightID", flightId);

                connection.Open();
                var result = command.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }
    }
}