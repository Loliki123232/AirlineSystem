using Xunit;
using AirlineSystem.Models;
using System;

namespace AirlineSystem.Tests
{
    public class BookingTests
    {
        [Fact]
        public void Booking_CreatedWithCurrentDate()
        {
            // Arrange & Act
            var booking = new Booking(1, 1, "12A", 5500.00m);

            // Assert
            Assert.Equal(DateTime.Now.Date, booking.BookingDate.Date);
            Assert.Equal("Confirmed", booking.Status);
        }

        [Fact]
        public void ConfirmedBooking_CanBeCancelled()
        {
            // Arrange
            var booking = new Booking(1, 1, "12A", 5500.00m);

            // Act
            bool canBeCancelled = booking.CanBeCancelled();

            // Assert
            Assert.True(canBeCancelled);
        }

        [Fact]
        public void Cancel_ChangesStatusToCancelled()
        {
            // Arrange
            var booking = new Booking(1, 1, "12A", 5500.00m);

            // Act
            booking.Cancel();

            // Assert
            Assert.Equal("Cancelled", booking.Status);
            Assert.False(booking.CanBeCancelled());
        }

        [Fact]
        public void CompletedBooking_CannotBeCancelled()
        {
            // Arrange
            var booking = new Booking(1, 1, "12A", 5500.00m)
            {
                Status = "Completed"
            };

            // Act
            bool canBeCancelled = booking.CanBeCancelled();

            // Assert
            Assert.False(canBeCancelled);
        }
    }
}