using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;

namespace HotelBooking.UnitTests.BookingManagerTest;

public class BookingManagerCreateBookingTest
{
    private IBookingManager bookingManager;
    private IRepository<Booking> bookingRepository;

    public BookingManagerCreateBookingTest()
    {
        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(20);
        bookingRepository = new FakeBookingRepository(start, end);
        IRepository<Room> roomRepository = new FakeRoomRepository();
        bookingManager = new BookingManager(bookingRepository, roomRepository);
    }

    [Fact]
    public async void Create_Booking_FullyOccupied()
    {
        // act
        var booking = new Booking()
        {
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(20),
        };
        var expectedResult = false;
        
        // arrange
        var result = await bookingManager.CreateBooking(booking);
        
        // assert
        Assert.Equal(expectedResult, result);
    }
    
    [Fact]
    public async void Create_Booking_NotFullyOccupied()
    {
        // act
        var booking = new Booking()
        {
            StartDate = DateTime.Today.AddDays(21),
            EndDate = DateTime.Today.AddDays(30),
        };
        var expectedResult = true;
        
        // arrange
        var result = await bookingManager.CreateBooking(booking);
        
        // assert
        Assert.Equal(expectedResult, result);
    }
}