using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteFinal.Models;
using RestaurantReservations.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestauranteTestes
{
    public class RestauranteTests
    {
        private ReservationContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ReservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ReservationContext(options);

            context.Reservations.AddRange(new List<Reservation>
            {
                new Reservation { Id = 1, CustomerName = "John Doe", ReservationDate = DateTime.Now.Date, ReservationTime = new TimeSpan(18, 0, 0), TableNumber = 1, NumberOfPeople = 2 },
                new Reservation { Id = 2, CustomerName = "Jane Doe", ReservationDate = DateTime.Now.Date.AddDays(1), ReservationTime = new TimeSpan(20, 0, 0), TableNumber = 2, NumberOfPeople = 4, IsDeleted = true }
            });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetAllReservations_ReturnsAllActiveReservations()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);

            // Act
            var result = await controller.GetAllReservations();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reservations = Assert.IsType<List<Reservation>>(okResult.Value);

            // Assert
            Assert.Single(reservations);
            Assert.Equal("John Doe", reservations[0].CustomerName);
        }

        [Fact]
        public async Task GetReservationById_ReturnsReservation_WhenExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);

            // Act
            var result = await controller.GetReservationById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reservation = Assert.IsType<Reservation>(okResult.Value);

            // Assert
            Assert.Equal("John Doe", reservation.CustomerName);
        }

        [Fact]
        public async Task GetReservationById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);

            // Act
            var result = await controller.GetReservationById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateReservation_AddsReservationSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);
            var newReservation = new Reservation { CustomerName = "Alice", ReservationDate = DateTime.Now.Date.AddDays(2), ReservationTime = new TimeSpan(19, 0, 0), TableNumber = 3, NumberOfPeople = 2 };

            // Act
            var result = await controller.CreateReservation(newReservation);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var reservation = Assert.IsType<Reservation>(createdAtActionResult.Value);

            // Assert
            Assert.Equal("Alice", reservation.CustomerName);
            Assert.Equal(3, context.Reservations.Count());
        }

        [Fact]
        public async Task UpdateReservation_UpdatesReservationSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);
            var updatedReservation = new Reservation { CustomerName = "John Updated", ReservationDate = DateTime.Now.Date, ReservationTime = new TimeSpan(18, 30, 0), TableNumber = 1, NumberOfPeople = 3 };

            // Act
            var result = await controller.UpdateReservation(1, updatedReservation);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var reservation = context.Reservations.First(r => r.Id == 1);
            Assert.Equal("John Updated", reservation.CustomerName);
            Assert.Equal(3, reservation.NumberOfPeople);
        }

        [Fact]
        public async Task UpdateReservation_ReturnsNotFound_WhenReservationDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);
            var updatedReservation = new Reservation { CustomerName = "Nonexistent", ReservationDate = DateTime.Now.Date, ReservationTime = new TimeSpan(18, 30, 0), TableNumber = 1, NumberOfPeople = 3 };

            // Act
            var result = await controller.UpdateReservation(99, updatedReservation);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SoftDeleteReservation_MarksReservationAsDeleted()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);

            // Act
            var result = await controller.SoftDeleteReservation(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var reservation = context.Reservations.First(r => r.Id == 1);
            Assert.True(reservation.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteReservation_ReturnsNotFound_WhenReservationDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);

            // Act
            var result = await controller.SoftDeleteReservation(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetReservationsByDate_ReturnsReservationsOnSpecificDate()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ReservationsController(context);
            var date = DateTime.Now.Date;

            // Act
            var result = await controller.GetReservationsByDate(date);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reservations = Assert.IsType<List<Reservation>>(okResult.Value);

            // Assert
            Assert.Single(reservations);
            Assert.Equal("John Doe", reservations[0].CustomerName);
        }
    }
}
