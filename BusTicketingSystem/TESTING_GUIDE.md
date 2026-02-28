# Testing Guide - Seat Selection System

## ?? Unit Testing Examples

### Test 1: Lock Seats Successfully

```csharp
[Fact]
public async Task LockSeats_ValidSeats_ShouldLockSuccessfully()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1", "A2" };
    var userId = 5;
    var ipAddress = "192.168.1.1";

    // Act
    var result = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, ipAddress);

    // Assert
    Assert.True(result.Data.Success);
    Assert.Equal(2, result.Data.LockedSeatNumbers.Count);
    Assert.Empty(result.Data.FailedSeatNumbers);
    Assert.NotNull(result.Data.LockExpiresAt);
}
```

### Test 2: Cannot Lock Already Booked Seat

```csharp
[Fact]
public async Task LockSeats_BookedSeat_ShouldFail()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" }; // Already booked
    var userId = 5;

    // Act
    var result = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    // Assert
    Assert.False(result.Data.Success);
    Assert.Empty(result.Data.LockedSeatNumbers);
    Assert.Contains("already booked", result.Data.FailedSeatNumbers[0]);
}
```

### Test 3: Lock Renewal by Same User

```csharp
[Fact]
public async Task LockSeats_RenewLock_ShouldUpdateTimestamp()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" };
    var userId = 5;
    
    // Lock once
    var firstLock = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");
    var firstLockTime = firstLock.Data.LockExpiresAt;
    
    // Wait 2 seconds
    await Task.Delay(2000);

    // Act - Lock again (renew)
    var secondLock = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    // Assert - Expiry time should be updated
    Assert.True(secondLock.Data.LockExpiresAt > firstLockTime);
}
```

### Test 4: Cannot Lock Seat Locked by Another User

```csharp
[Fact]
public async Task LockSeats_LockedByOtherUser_ShouldFail()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" };
    var user1Id = 5;
    var user2Id = 6;

    // User 1 locks seat
    await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, user1Id, "192.168.1.1");

    // Act - User 2 tries to lock same seat
    var result = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, user2Id, "192.168.1.1");

    // Assert
    Assert.False(result.Data.Success);
    Assert.Empty(result.Data.LockedSeatNumbers);
    Assert.Contains("locked by another user", result.Data.FailedSeatNumbers[0]);
}
```

### Test 5: Create Booking with Locked Seats

```csharp
[Fact]
public async Task CreateBooking_WithLockedSeats_ShouldSucceed()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1", "A2" };
    var userId = 5;

    // Lock seats first
    await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    var dto = new CreateBookingRequestDto
    {
        ScheduleId = scheduleId,
        SeatNumbers = seatNumbers
    };

    // Act
    var result = await _bookingService.CreateBookingAsync(
        dto, userId, "192.168.1.1");

    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.Data.BookingId);
    Assert.Equal("Confirmed", result.Data.BookingStatus);
    Assert.Equal(2, result.Data.NumberOfSeats);
}
```

### Test 6: Cannot Book Unlocked Seats

```csharp
[Fact]
public async Task CreateBooking_UnlockedSeats_ShouldFail()
{
    // Arrange
    var dto = new CreateBookingRequestDto
    {
        ScheduleId = 1,
        SeatNumbers = new List<string> { "A1" } // Not locked
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(
        () => _bookingService.CreateBookingAsync(dto, 5, "192.168.1.1"));
    
    Assert.Contains("not locked", exception.Message);
}
```

### Test 7: Release Locked Seats

```csharp
[Fact]
public async Task ReleaseSeats_LockedByUser_ShouldSucceed()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" };
    var userId = 5;

    // Lock seat
    await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    // Act
    var result = await _seatService.ReleaseSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    // Assert
    Assert.True(result.Data.Success);
    Assert.Contains("A1", result.Data.ReleasedSeatNumbers);
}
```

### Test 8: Cancel Booking and Release Seats

```csharp
[Fact]
public async Task CancelBooking_ShouldReleaseSeatsAndRestoreSchedule()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1", "A2" };
    var userId = 5;
    
    // Get initial available seats
    var scheduleBefore = await _scheduleRepository.GetByIdAsync(scheduleId);
    var availableSeatsBeforeBooking = scheduleBefore.AvailableSeats;
    
    // Create booking
    var lockResult = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");
    
    var bookingDto = new CreateBookingRequestDto
    {
        ScheduleId = scheduleId,
        SeatNumbers = seatNumbers
    };
    var bookingResult = await _bookingService.CreateBookingAsync(
        bookingDto, userId, "192.168.1.1");
    var bookingId = bookingResult.Data.BookingId;
    
    // Verify seats were deducted
    var scheduleAfter = await _scheduleRepository.GetByIdAsync(scheduleId);
    Assert.Equal(availableSeatsBeforeBooking - 2, scheduleAfter.AvailableSeats);

    // Act - Cancel booking
    var cancelResult = await _bookingService.CancelBookingAsync(
        bookingId, userId, "Customer", "192.168.1.1");

    // Assert
    Assert.True(cancelResult.Data);
    
    var scheduleAfterCancel = await _scheduleRepository.GetByIdAsync(scheduleId);
    Assert.Equal(availableSeatsBeforeBooking, scheduleAfterCancel.AvailableSeats);
}
```

### Test 9: Seat Lock Expiration Cleanup

```csharp
[Fact]
public async Task CleanupExpiredLocks_ShouldReleaseExpiredSeats()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" };
    var userId = 5;

    // Lock seat
    await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId, "192.168.1.1");

    // Simulate 5+ minutes passing (mock DateTime.UtcNow if needed)
    // In real test, use time-travel or manual DB update

    // Act
    var cleanedCount = await _seatService.CleanupExpiredLocksAsync();

    // Assert
    Assert.Equal(1, cleanedCount);
    
    // Verify seat is now available
    var seat = await _seatRepository.GetSeatByScheduleAndNumberAsync(
        scheduleId, "A1");
    Assert.Equal("Available", seat.SeatStatus);
    Assert.Null(seat.LockedByUserId);
}
```

### Test 10: Partial Lock Response

```csharp
[Fact]
public async Task LockSeats_MixedStatus_ShouldReturnPartialSuccess()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1", "A2", "A3" };
    // A1 = Available
    // A2 = Booked
    // A3 = Locked by another user

    // Act
    var result = await _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, 5, "192.168.1.1");

    // Assert
    Assert.False(result.Data.Success);
    Assert.Contains("A1", result.Data.LockedSeatNumbers);
    Assert.Contains("A2", string.Concat(result.Data.FailedSeatNumbers));
    Assert.Contains("A3", string.Concat(result.Data.FailedSeatNumbers));
}
```

---

## ?? Integration Testing Scenarios

### Scenario 1: Complete Booking Flow

```csharp
[Fact]
public async Task CompleteBookingFlow_EndToEnd_ShouldSucceed()
{
    // 1. Load seat layout
    var layoutResult = await _seatService.GetSeatLayoutAsync(1);
    Assert.True(layoutResult.Success);
    Assert.Equal(40, layoutResult.Data.TotalSeats);

    // 2. Lock 3 seats
    var lockResult = await _seatService.LockSeatsAsync(
        1, new List<string> { "A1", "A2", "B1" }, 5, "192.168.1.1");
    Assert.True(lockResult.Data.Success);

    // 3. Verify seats are now locked
    var layoutAfterLock = await _seatService.GetSeatLayoutAsync(1);
    Assert.Equal(3, layoutAfterLock.Data.LockedSeats);

    // 4. Create booking
    var bookingResult = await _bookingService.CreateBookingAsync(
        new CreateBookingRequestDto
        {
            ScheduleId = 1,
            SeatNumbers = new List<string> { "A1", "A2", "B1" }
        },
        5,
        "192.168.1.1");
    Assert.True(bookingResult.Success);

    // 5. Verify seats are now booked
    var layoutAfterBooking = await _seatService.GetSeatLayoutAsync(1);
    Assert.Equal(0, layoutAfterBooking.Data.LockedSeats);
    Assert.Equal(3, layoutAfterBooking.Data.BookedSeats);
    Assert.Equal(37, layoutAfterBooking.Data.AvailableSeats);

    // 6. Get booking details
    var bookingDetails = await _bookingService.GetBookingByIdAsync(
        bookingResult.Data.BookingId);
    Assert.True(bookingDetails.Success);
    Assert.Equal("Confirmed", bookingDetails.Data.BookingStatus);

    // 7. Cancel booking
    var cancelResult = await _bookingService.CancelBookingAsync(
        bookingResult.Data.BookingId, 5, "Customer", "192.168.1.1");
    Assert.True(cancelResult.Data);

    // 8. Verify seats returned to available
    var layoutAfterCancel = await _seatService.GetSeatLayoutAsync(1);
    Assert.Equal(0, layoutAfterCancel.Data.BookedSeats);
    Assert.Equal(40, layoutAfterCancel.Data.AvailableSeats);
}
```

---

## ?? Concurrency Testing

### Scenario: Race Condition - Two Users Lock Same Seat

```csharp
[Fact]
public async Task ConcurrentLock_SameSeats_OnlyOneSucceeds()
{
    // Arrange
    var scheduleId = 1;
    var seatNumbers = new List<string> { "A1" };
    
    // Act - Both users try to lock simultaneously
    var task1 = _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId: 5, "192.168.1.1");
    
    var task2 = _seatService.LockSeatsAsync(
        scheduleId, seatNumbers, userId: 6, "192.168.1.2");

    await Task.WhenAll(task1, task2);

    // Assert - Only one should succeed
    var result1 = task1.Result;
    var result2 = task2.Result;

    var totalSuccessful = 0;
    if (result1.Data.LockedSeatNumbers.Count > 0) totalSuccessful++;
    if (result2.Data.LockedSeatNumbers.Count > 0) totalSuccessful++;

    Assert.Equal(1, totalSuccessful);
}
```

---

## ?? Performance Testing

### Scenario: Load Test - Many Users Locking Seats

```csharp
[Fact]
public async Task LoadTest_Many_Users_LockingSeats()
{
    // Arrange
    var scheduleId = 1;
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    
    // Act - 100 users lock different seats concurrently
    var tasks = new List<Task>();
    for (int i = 0; i < 100; i++)
    {
        int userId = 1000 + i;
        var seatNumber = $"{(char)('A' + (i / 10))}{(i % 10) + 1}"; // A1-J10
        
        tasks.Add(_seatService.LockSeatsAsync(
            scheduleId,
            new List<string> { seatNumber },
            userId,
            "192.168.1.1"));
    }

    await Task.WhenAll(tasks);
    stopwatch.Stop();

    // Assert
    Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should complete in < 5 seconds
}
```

---

## ? Checklist for Manual Testing

### Pre-Booking Phase
- [ ] Display all seats with correct status
- [ ] Lock single seat
- [ ] Lock multiple seats
- [ ] Lock invalid seat number (should fail)
- [ ] Renew lock before expiry
- [ ] Release locked seat
- [ ] Release seat locked by another user (should fail)

### Booking Phase
- [ ] Create booking with locked seats
- [ ] Booking with unlocked seats (should fail)
- [ ] Booking after lock expires (should fail)
- [ ] View booking details
- [ ] Verify seats are marked as "Booked"
- [ ] Verify available seats count decrements

### Cancellation Phase
- [ ] Cancel booking as owner
- [ ] Cancel booking as admin
- [ ] Cancel booking as different user (should fail)
- [ ] Verify seats return to "Available"
- [ ] Verify available seats count increments
- [ ] Try to cancel already-cancelled booking (should fail)
- [ ] Cancel after departure time (should fail)

### Lock Expiration
- [ ] Lock seat (note expiry time)
- [ ] Wait 5 minutes
- [ ] Try to book expired lock (should fail)
- [ ] Verify seat is available again

### Concurrency
- [ ] Open 2 browser tabs for same schedule
- [ ] Tab 1: Lock seat A1
- [ ] Tab 2: Try to lock seat A1 immediately
- [ ] Verify only one succeeds
- [ ] Verify partial lock response shows failures

---

## ?? Test Data Setup

```csharp
private async Task SeedTestData()
{
    // Create schedule
    var schedule = new Schedule
    {
        RouteId = 1,
        BusId = 1,
        TravelDate = DateTime.UtcNow.AddDays(7),
        DepartureTime = new TimeSpan(14, 0, 0),
        ArrivalTime = new TimeSpan(18, 0, 0),
        TotalSeats = 40,
        AvailableSeats = 40,
        IsActive = true
    };
    await _scheduleRepository.AddAsync(schedule);
    await _scheduleRepository.SaveChangesAsync();

    // Create seats A1-A4, B1-B4, ..., J1-J4
    var seats = new List<Seat>();
    for (int row = 0; row < 10; row++)
    {
        for (int col = 1; col <= 4; col++)
        {
            seats.Add(new Seat
            {
                ScheduleId = schedule.ScheduleId,
                SeatNumber = $"{(char)('A' + row)}{col}",
                SeatStatus = "Available"
            });
        }
    }
    
    foreach (var seat in seats)
        await _seatRepository.AddAsync(seat);
    
    await _seatRepository.SaveChangesAsync();
}
```

---

## ?? Debugging Tips

### Common Issues

**Issue**: "Lock expires too quickly"
- **Check**: Ensure `DateTime.UtcNow` is used consistently
- **Solution**: Mock time in tests or use test fixtures

**Issue**: "Can't book after locking"
- **Check**: Verify lock is actually created (check SeatLock table)
- **Check**: Verify seat.LockedByUserId matches userId
- **Check**: Verify lock hasn't expired

**Issue**: "Partial lock not working"
- **Check**: Ensure response includes both succeeded and failed items
- **Check**: Transaction didn't rollback
- **Solution**: Add logging to see which seats fail

**Issue**: "Concurrency errors"
- **Check**: Database connection string and transaction isolation level
- **Check**: Ensure using `DbContext.Database.BeginTransactionAsync()`
- **Solution**: Add retry logic or use optimistic concurrency

---

## ?? Test Coverage Goals

| Component | Target Coverage |
|-----------|-----------------|
| SeatService | 90%+ |
| BookingService | 85%+ |
| SeatRepository | 80%+ |
| Controllers | 70%+ |
| Integration Tests | Full flow coverage |

---

## ?? Key Test Assertions

```csharp
// Lock operations
Assert.Contains(seatNumber, result.LockedSeatNumbers);
Assert.False(result.Success); // Partial success
Assert.NotNull(result.LockExpiresAt);

// Booking operations
Assert.Equal("Confirmed", booking.BookingStatus);
Assert.Equal(expectedAmount, booking.TotalAmount);
Assert.Equal(userId, booking.UserId);

// Seat operations
Assert.Equal("Available", seat.SeatStatus);
Assert.Null(seat.LockedByUserId);
Assert.Equal("Booked", seat.SeatStatus);

// Schedule operations
Assert.Equal(expectedAvailableSeats, schedule.AvailableSeats);
```
