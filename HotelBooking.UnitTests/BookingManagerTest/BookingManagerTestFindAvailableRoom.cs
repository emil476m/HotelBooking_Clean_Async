using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Core;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests.BookingManagerTest;

public class BookingManagerTestFindAvailableRoom
{
    private IBookingManager bookingManager;

    public BookingManagerTestFindAvailableRoom()
    {
        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(20);
        Mock<IRepository<Booking>> bookingRepository = new Mock<IRepository<Booking>>();
        Mock<IRepository<Room>> roomRepository = new Mock<IRepository<Room>>();

        var bookings = new[] // Mocks booking repository
        {
            new Booking
            {
                Id = 1, StartDate = start, EndDate = end, IsActive = true,
                CustomerId = 1, RoomId = 1
            },
            new Booking
            {
                Id = 2, StartDate = start, EndDate = end, IsActive = true,
                CustomerId = 2, RoomId = 2
            },
        };
        bookingRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(bookings);

        var rooms = new[] //mock data for Rooms repository
        {
            new Room { Id = 1, Description = "A" },
            new Room { Id = 2, Description = "B" }
        };
        roomRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(rooms);

        bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);
    }

    public static IEnumerable<object[]> GetPreDates()
    {
        yield return new object[] { null };
        yield return new object[] { DateTime.Today };
        yield return new object[] { DateTime.Today.AddDays(-100) };
        yield return new object[] { DateTime.Today.AddDays(-50) };
        yield return new object[] { DateTime.Today.AddDays(-56) };
        yield return new object[] { DateTime.Today.AddDays(-20) };
        yield return new object[] { DateTime.Today.AddDays(-7400) };
        yield return new object[] { DateTime.Today.AddDays(-9000) };
        yield return new object[] { DateTime.Today.AddDays(-9300) };
        yield return new object[] { DateTime.Today.AddDays(-14200) };
    }

    public static IEnumerable<object[]> GetFutureDates()
    {
        yield return new object[] { DateTime.Today.AddDays(21), DateTime.Today.AddDays(30) };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(2) };
        yield return new object[] { DateTime.Today.AddDays(37), DateTime.Today.AddDays(44) };
        yield return new object[] { DateTime.Today.AddDays(220), DateTime.Today.AddDays(225) };
        yield return new object[] { DateTime.Today.AddDays(336), DateTime.Today.AddDays(338) };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(5) };
        yield return new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(9) };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(6) };
        yield return new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(7) };
    }

    public static IEnumerable<object[]> GetReservedDates()
    {
        yield return new object[] { DateTime.Today.AddDays(11), DateTime.Today.AddDays(15) };
        yield return new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20) };
        yield return new object[] { DateTime.Today.AddDays(18), DateTime.Today.AddDays(22) };
        yield return new object[] { DateTime.Today.AddDays(8), DateTime.Today.AddDays(14) };
        yield return new object[] { DateTime.Today.AddDays(11), DateTime.Today.AddDays(31) };
    }

    public static IEnumerable<object[]> GetUnrealisticDates()
    {
        yield return new object[] { DateTime.Today.AddDays(32), DateTime.Today.AddDays(21) };
        yield return new object[] { DateTime.Today.AddDays(8), DateTime.Today.AddDays(2) };
        yield return new object[] { DateTime.Today.AddDays(68), DateTime.Today.AddDays(52) };
        yield return new object[] { DateTime.Today.AddDays(70), DateTime.Today.AddDays(69) };
        yield return new object[] { DateTime.Today.AddDays(336), DateTime.Today.AddDays(228) };
        yield return new object[] { DateTime.Today.AddDays(332), DateTime.Today.AddDays(310) };
    }

    [Theory]
    [MemberData(nameof(GetPreDates))] //Arange
    public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime date)
    {
        // Act
        Task result() => bookingManager.FindAvailableRoom(date, date);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }

    [Theory]
    [MemberData(nameof(GetFutureDates))]
    public async Task FindAvailableRoom_DateInTheFuture_ReturnsAvailableRoom(DateTime startDate, DateTime endDate)
    {
        // Act
        int result = await bookingManager.FindAvailableRoom(startDate, endDate);

        // Assert
        Assert.NotEqual(-1, result);
        Assert.IsType<int>(result);
    }

    [Theory]
    [MemberData(nameof(GetReservedDates))]
    public async Task FindAvailebelRoms_FutureDateReserved_NoRoomsAvailable_MinusOne(DateTime startDate,
        DateTime endDate)
    {
        int result = await bookingManager.FindAvailableRoom(startDate, endDate);

        Assert.Equal(-1, result); //Assert result is equal to minus one for no availebel rooms
    }

    [Theory]
    [MemberData(nameof(GetUnrealisticDates))]
    public async Task FindAvailebelRoms_StartDateHigherThanEndDate_ArgumentException(DateTime startDate,
        DateTime endDate)
    {
        // Act
        Task result() => bookingManager.FindAvailableRoom(startDate, endDate);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }
}