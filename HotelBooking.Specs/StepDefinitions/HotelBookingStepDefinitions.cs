using System.Threading.Tasks;
using HotelBooking.Core;
using Moq;
using System;
using Reqnroll;
using Xunit;

namespace HotelBooking.Specs.StepDefinitions;



[Binding]
public class HotelBookingStepDefinitions
{
    private readonly IReqnrollOutputHelper _outputHelper;
    
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> bookingRepository;
    
    private DateTime startDate;
    private DateTime endDate;
    private Task<bool> methodResult;
    
    private readonly DateTime today = DateTime.Today;

    public HotelBookingStepDefinitions(IReqnrollOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        
        
        // set up bookingRepository
        DateTime fullyOccupiedStartDate = today.AddDays(10);
        DateTime fullyOccupiedEndDate = today.AddDays(20);
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

    [Given("a booking starting on {int}")]
    public void GivenABookingStartingOn(int p0)
    {
        startDate = today.AddDays(p0);
    }

    [Given("ending on {int}")]
    public void GivenEndingOn(int p0)
    {
        endDate = today.AddDays(p0);
    }

    [When("the booking is created")]
    public void WhenTheBookingIsCreated()
    {
        var booking = new Booking()
        {
            StartDate = startDate,
            EndDate = endDate,
        };
        methodResult = bookingManager.CreateBooking(booking);
    }

    [Then("the booking should be created sucessfully")]
    public async Task ThenTheBookingShouldBeCreatedSucessfully()
    {
        
        Assert.True(methodResult.Result);
        bookingRepository.Verify(Mock => Mock.AddAsync(It.IsAny<Booking>()), Times.Once);
    }
}