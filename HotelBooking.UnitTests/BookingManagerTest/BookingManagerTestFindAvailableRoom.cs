using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using Xunit.Abstractions;

namespace HotelBooking.UnitTests.BookingManagerTest;

public class BookingManagerTestFindAvailableRoom
{
    private readonly ITestOutputHelper testOutputHelper;
    private IBookingManager bookingManager;
    private IRepository<Booking> bookingRepository;

    public BookingManagerTestFindAvailableRoom(ITestOutputHelper testOutputHelper){
        this.testOutputHelper = testOutputHelper;
        DateTime start = DateTime.Today.AddDays(10);
        DateTime end = DateTime.Today.AddDays(20);
        bookingRepository = new FakeBookingRepository(start, end);
        IRepository<Room> roomRepository = new FakeRoomRepository();
        bookingManager = new BookingManager(bookingRepository, roomRepository);
    }
    
    public static IEnumerable<object[]> GetPreDates() 
    {
        yield return new object[] { null };
        yield return new object[] { DateTime.Today };
        yield return new object[] { new DateTime(2025,1,1) };
        yield return new object[] { new DateTime(2025,4,8) };
        yield return new object[] { new DateTime(2025,4,2) };
        yield return new object[] { new DateTime(2025,6,5) };
        yield return new object[] { new DateTime(2005,5,5) };
        yield return new object[] { new DateTime(2001,1,1) };
        yield return new object[] { new DateTime(1999,12,31) };
        yield return new object[] { new DateTime(1986,8,28) };
    }
    
    public static IEnumerable<object[]> GetFutureDates() 
    {
        yield return new object[] { DateTime.Today.AddDays(21), DateTime.Today.AddDays(30)  };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(2)  };
        yield return new object[] { DateTime.Today.AddDays(37), DateTime.Today.AddDays(44)  };
        yield return new object[] { DateTime.Today.AddDays(220), DateTime.Today.AddDays(225)  };
        yield return new object[] { DateTime.Today.AddDays(336), DateTime.Today.AddDays(338)  };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(5)  };
        yield return new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(9)  };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(6)  };
        yield return new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(7)  };
    }

    public static IEnumerable<object[]> GetReservedDates()
    {
        yield return new object[] { DateTime.Today.AddDays(11), DateTime.Today.AddDays(15)  };
        yield return new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)  };
        yield return new object[] { DateTime.Today.AddDays(18), DateTime.Today.AddDays(22)  };
        yield return new object[] { DateTime.Today.AddDays(8), DateTime.Today.AddDays(14)  };
        yield return new object[] { DateTime.Today.AddDays(11), DateTime.Today.AddDays(31)  };

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
        Assert.NotEqual(-1 , result);
        Assert.IsType<int>(result);
    }

    [Theory]
    [MemberData(nameof(GetReservedDates))]
    public async Task FindAvailebelRoms_FutureDateReserved_NoRoomsAvailable_MinusOne(DateTime startDate, DateTime endDate)
    {
        int result = await bookingManager.FindAvailableRoom(startDate,endDate);
        
        Assert.Equal(-1, result); //Assert result is equal to minus one for no availebel rooms

    }

}