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
    /*private IBookingManager bookingManager;
    Mock<IRepository<Booking>> mockBookingRepository;
    

    public BookingManagerTestOccupiedDates(){
        mockBookingRepository = new Mock<IRepository<Booking>>();
        var mockRoomRepository = new Mock<IRepository<Room>>();
        bookingManager = new BookingManager(mockBookingRepository.Object,  mockRoomRepository.Object);
    }*/

    
    private IBookingManager bookingManager;
    IRepository<Booking> bookingRepository;

    public BookingManagerTestOccupiedDates(){
        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(20);
        bookingRepository = new FakeBookingRepository(start, end);
        IRepository<Room> roomRepository = new FakeRoomRepository();
        bookingManager = new BookingManager(bookingRepository, roomRepository);
    }

    [Fact]
    public async void TestOccupiedDates_GetAllOccupiedDates()
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
    public async void TestOccupiedDates_StartDateHigherThanEndDate_GetArgumentException()
    {
        //Arrange
        DateTime start = DateTime.Today.AddDays(30);
        DateTime end = DateTime.Today.AddDays(10);
        
        //Act
        Task result() => bookingManager.GetFullyOccupiedDates(start, end);
        
        //Assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }
}