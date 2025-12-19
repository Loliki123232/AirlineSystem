using Xunit;
using AirlineSystem.Models;
using System;

namespace AirlineSystem.Tests
{
    public class FlightTests
    {
        [Fact]
        public void Flight_CanBeCreated_WithValidParameters()
        {
            // Arrange
            DateTime departureTime = new DateTime(2024, 6, 1, 8, 0, 0);
            DateTime arrivalTime = new DateTime(2024, 6, 1, 9, 30, 0);

            // Act
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   departureTime, arrivalTime, 5500.00m, 189);

            // Assert
            Assert.Equal("SU 1001", flight.FlightNumber);
            Assert.Equal("Москва", flight.DeparturePoint);
            Assert.Equal("Санкт-Петербург", flight.DestinationPoint);
            Assert.Equal("Москва → Санкт-Петербург", flight.RouteInfo);
        }

        [Fact]
        public void Flight_CanBeBooked_WhenAvailableSeatsExist()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 10);

            // Act
            bool canBeBooked = flight.CanBeBooked();

            // Assert
            Assert.True(canBeBooked);
        }

        [Fact]
        public void Flight_CannotBeBooked_WhenNoSeatsAvailable()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 0);

            // Act
            bool canBeBooked = flight.CanBeBooked();

            // Assert
            Assert.False(canBeBooked);
        }

        [Fact]
        public void BookSeat_DecreasesAvailableSeats()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 10);
            int initialSeats = flight.AvailableSeats;

            // Act
            flight.BookSeat();

            // Assert
            Assert.Equal(initialSeats - 1, flight.AvailableSeats);
        }

        [Fact]
        public void CancelSeat_IncreasesAvailableSeats()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 5);
            int initialSeats = flight.AvailableSeats;

            // Act
            flight.CancelSeat();

            // Assert
            Assert.Equal(initialSeats + 1, flight.AvailableSeats);
        }

        [Fact]
        public void CalculateRevenue_ReturnsCorrectAmount()
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 100);
            int bookedSeats = 50;
            decimal expectedRevenue = 5500.00m * bookedSeats;

            // Act
            decimal actualRevenue = flight.CalculateRevenue(bookedSeats);

            // Assert
            Assert.Equal(expectedRevenue, actualRevenue);
        }

        [Theory]
        [InlineData("Delayed", true)]
        [InlineData("Scheduled", false)]
        [InlineData("Completed", false)]
        public void IsDelayed_ReturnsCorrectValue(string status, bool expected)
        {
            // Arrange
            var flight = new Flight("SU 1001", "Москва", "Санкт-Петербург",
                                   DateTime.Now.AddHours(1), DateTime.Now.AddHours(3),
                                   5500.00m, 100);
            flight.Status = status;

            // Act
            bool isDelayed = flight.IsDelayed();

            // Assert
            Assert.Equal(expected, isDelayed);
        }

        [Fact]
        public void TouristSeasonFlight_HasHigherPriority()
        {
            // Arrange
            var flight = new Flight("SU 2001", "Москва", "Сочи",
                                   new DateTime(2024, 7, 1, 10, 0, 0),
                                   new DateTime(2024, 7, 1, 12, 30, 0),
                                   8500.00m, 180);

            // Act
            flight.IsTouristSeason = true;

            // Assert
            Assert.True(flight.IsTouristSeason);
            Assert.True(flight.Price > 8000.00m); // Туристические рейсы дороже
        }
    }
}