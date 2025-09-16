using System;
using System.Threading.Tasks;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests.BookingManagerTest;

public class BookingManagerCreateBookingTest
{
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> bookingRepository;

    public BookingManagerCreateBookingTest()
    {
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
    
    [Fact]
    public async void Create_Booking_StartDate_After_EndDate()
    {
        // act
        var booking = new Booking()
        {
            StartDate = DateTime.Today.AddDays(30),
            EndDate = DateTime.Today.AddDays(20),
        };
        
        // arrange
        Task result() =>  bookingManager.CreateBooking(booking);
        
        // assert
        await Assert.ThrowsAnyAsync<ArgumentException>(result);
    }
    
    [Fact]
    public async void Create_Booking_StartDate_In_ThePast()
    {
        // act
        var booking = new Booking()
        {
            StartDate = DateTime.Today.AddDays(-10),
            EndDate = DateTime.Today.AddDays(20),
        };
        
        // arrange
        Task result() =>  bookingManager.CreateBooking(booking);
        
        // assert
        await Assert.ThrowsAnyAsync<ArgumentException>(result);
    }
}