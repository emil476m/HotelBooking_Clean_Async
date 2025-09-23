using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests.BookingManagerTest;

public class BookingManagerTestOccupiedDates
{
    
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> bookingRepository;

    public BookingManagerTestOccupiedDates(){
        // set up bookingRepository
        DateTime fullyOccupiedStartDate = DateTime.Today.AddDays(10);
        DateTime fullyOccupiedEndDate = DateTime.Today.AddDays(20);
        bookingRepository = new Mock<IRepository<Booking>>();
        var bookings = new[]
        {
            new Booking
            {
                Id = 1, StartDate = fullyOccupiedStartDate, EndDate = fullyOccupiedEndDate, IsActive = true,
                CustomerId = 1, RoomId = 1
            },
            new Booking
            {
                Id = 2, StartDate = fullyOccupiedStartDate, EndDate = fullyOccupiedEndDate, IsActive = true,
                CustomerId = 2, RoomId = 2
            },
        };
        bookingRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(bookings);
        
        // set up roomRepository
        Mock<IRepository<Room>> roomRepository = new Mock<IRepository<Room>>();
        var rooms = new[]
        {
            new Room { Id = 1, Description = "A"},
            new Room { Id = 2, Description = "B"}
        };
        roomRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(rooms);
        
        // create bookingManager
        bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);
    }

    [Fact]
    public async Task TestOccupiedDates_GetAllOccupiedDatesInGivenDateInterval_ExpectedResult()
    {
        //Arrange
        
        List<DateTime> fullyOccupiedDates = Enumerable.Range(10,  11).Select(x => DateTime.Today.AddDays(+x)).ToList();
        
        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(20);
        
        //Act
        List<DateTime> result = await bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        Assert.Equal(fullyOccupiedDates, result);
    }

    [Fact]
    public async Task TestOccupiedDates_StartDateHigherThanEndDate_GetArgumentException()
    {
        //Arrange
        DateTime start = DateTime.Today.AddDays(30);
        DateTime end = DateTime.Today.AddDays(10);
        
        //Act
        Task result() => bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }
    
    [Fact]
    public async Task TestOccupiedDates_StartAndEndDateSame_OneDayReturned()
    {
        //Arrange

        List<DateTime> fullyOccupiedDates = new List<DateTime>()
        {
            DateTime.Today.AddDays(10)
        };

        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(10);
        
        //Act
        List<DateTime> result = await bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        Assert.Equal(fullyOccupiedDates, result);
    }
    
    [Fact]
    public async Task TestOccupiedDates_InsideAndOutsideOccupiedDateRange_ExpectedOnlyTwoDaysReturned()
    {
        //Arrange

        List<DateTime> fullyOccupiedDates = new List<DateTime>()
        {
            DateTime.Today.AddDays(19),
            DateTime.Today.AddDays(20)
        };

        DateTime start = DateTime.Today.AddDays(19);
        DateTime end = DateTime.Today.AddDays(30);
        
        //Act
        List<DateTime> result = await bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        Assert.Equal(fullyOccupiedDates, result);
    }
    
    [Fact]
    public async Task TestOccupiedDates_NoOccupiedDates_EmptyList()
    {
        //Arrange

        List<DateTime> fullyOccupiedDates = new List<DateTime>();

        DateTime start = DateTime.Today.AddDays(21);
        DateTime end = DateTime.Today.AddDays(30);
        
        //Act
        List<DateTime> result = await bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        Assert.Equal(fullyOccupiedDates, result);
    }
}