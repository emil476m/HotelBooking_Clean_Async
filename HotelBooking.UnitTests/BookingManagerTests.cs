using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;


namespace HotelBooking.UnitTests
{
    
    //TODO Write multiple test for the different methods
    
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }
        
        public static IEnumerable<object[]> GetPreDates() //Could be mowed to seperate file
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
        
        [Theory]
        [MemberData(nameof(GetPreDates))] //Arange
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime date)
        {
            // Act
            Task result() => bookingManager.FindAvailableRoom(date, date);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);

            var bookingForReturnedRoomId = (await bookingRepository.GetAllAsync()).
                Where(b => b.RoomId == roomId
                           && b.StartDate <= date
                           && b.EndDate >= date
                           && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }

    }
}
