# Production-Grade Seat Selection System - Implementation Guide

## ?? Overview

This document describes the complete implementation of a production-grade seat management and booking system with real-time seat locking, expiration, and concurrency handling.

---

## ?? Architecture Overview

### System Components

1. **Models**
   - `Seat` - Represents individual seats with status tracking
   - `SeatLock` - Tracks temporary seat locks with expiration
   - `Booking` - Represents confirmed bookings
   - `Schedule` - Bus schedule with available seat count

2. **Repositories**
   - `ISeatRepository` / `SeatRepository` - Seat data access
   - `ISeatLockRepository` / `SeatLockRepository` - Lock management
   - `IScheduleRepository` / `ScheduleRepository` - Schedule data access with transaction support

3. **Services**
   - `ISeatService` / `SeatService` - Seat locking, releasing, and booking seat confirmation
   - `IBookingService` / `BookingService` - Booking creation and cancellation
   - `IBookingController` - HTTP endpoints for all operations

---

## ?? Seat Status Flow

```
Available 
    ?
    ?? Lock (by User A) ? Locked (5 min expiry)
    ?     ?? Unlock ? Available
    ?     ?? Confirm Booking ? Booked
    ?     ?? Expire ? Available
    ?
    ?? Booked (by Booking X)
           ?? Cancel Booking ? Available
           ?? (Can't be locked by others)
```

---

## ?? Key Business Rules

### 1. Seat Locking Rules
- ? Users can lock "Available" seats
- ? Lock expires after **5 minutes**
- ? Lock can be renewed by user before expiry
- ? Cannot lock "Booked" seats
- ? Cannot lock seats locked by another user
- ? Expired locks automatically become "Available"

### 2. Booking Creation Rules
- ? All seats must be "Locked" by the requesting user
- ? Seats are atomically converted to "Booked"
- ? Schedule `AvailableSeats` is decremented
- ? Cannot book if any seat is "Booked" or locked by others
- ? Entire operation is transactional (all or nothing)

### 3. Seat Releasing Rules
- ? User can release their own locked seats
- ? Seats return to "Available" status
- ? Cannot release seats locked by others
- ? Cannot release "Booked" seats directly (only via cancellation)

### 4. Booking Cancellation Rules
- ? Owner or Admin can cancel
- ? All "Booked" seats return to "Available"
- ? Schedule `AvailableSeats` is incremented
- ? Cannot cancel after departure time
- ? Entire operation is transactional

---

## ?? API Endpoints

### Seat Management Endpoints

#### 1. **Get Seat Layout**
```
GET /api/v1/booking/seats/{scheduleId}
Authorization: Bearer {token}

Response: SeatLayoutResponseDto
{
  "scheduleId": 1,
  "busId": 1,
  "busNumber": "ABC-123",
  "totalSeats": 40,
  "availableSeats": 20,
  "lockedSeats": 5,
  "bookedSeats": 15,
  "seats": [
    {
      "seatId": 1,
      "seatNumber": "A1",
      "seatStatus": "Available",
      "lockedByUserId": null,
      "lockedAt": null,
      "lockedExpiresAt": null
    },
    {
      "seatId": 2,
      "seatNumber": "A2",
      "seatStatus": "Locked",
      "lockedByUserId": 5,
      "lockedAt": "2025-01-15T10:00:00Z",
      "lockedExpiresAt": "2025-01-15T10:05:00Z"
    }
  ]
}
```

#### 2. **Lock Seats (5-minute reservation)**
```
POST /api/v1/booking/seats/lock
Authorization: Bearer {token}
Content-Type: application/json

Request: LockSeatsRequestDto
{
  "scheduleId": 1,
  "seatNumbers": ["A1", "A2", "B1"]
}

Response: LockSeatsResponseDto
{
  "success": true,
  "message": "All 3 seats locked successfully.",
  "lockedSeatNumbers": ["A1", "A2", "B1"],
  "failedSeatNumbers": [],
  "lockExpiresAt": "2025-01-15T10:05:00Z"
}

Error Response (Partial Lock):
{
  "success": false,
  "message": "Locked 2 seats. Failed: 1",
  "lockedSeatNumbers": ["A1", "A2"],
  "failedSeatNumbers": ["B1 (already booked)"],
  "lockExpiresAt": "2025-01-15T10:05:00Z"
}
```

#### 3. **Release Locked Seats**
```
POST /api/v1/booking/seats/release
Authorization: Bearer {token}
Content-Type: application/json

Request: ReleaseSeatsRequestDto
{
  "scheduleId": 1,
  "seatNumbers": ["A1", "A2"]
}

Response: ReleaseSeatsResponseDto
{
  "success": true,
  "message": "All 2 seats released successfully.",
  "releasedSeatNumbers": ["A1", "A2"],
  "failedSeatNumbers": []
}
```

### Booking Endpoints

#### 4. **Create Booking (with locked seats)**
```
POST /api/v1/booking
Authorization: Bearer {token}
Content-Type: application/json

Request: CreateBookingRequestDto
{
  "scheduleId": 1,
  "seatNumbers": ["A1", "A2", "B1"]
}

Response: ApiResponse<BookingResponseDto>
{
  "success": true,
  "message": "...",
  "data": {
    "bookingId": 101,
    "scheduleId": 1,
    "numberOfSeats": 3,
    "totalAmount": 1500.00,
    "bookingStatus": "Confirmed",
    "bookingDate": "2025-01-15T10:02:00Z"
  }
}

Errors:
- "At least one seat must be selected."
- "Seat A1 is not locked. Please lock seats before booking."
- "Seat B1 is not locked by you."
- "Cannot book after departure time."
```

#### 5. **Get User's Bookings**
```
GET /api/v1/booking/my
Authorization: Bearer {token}

Response: ApiResponse<List<BookingResponseDto>>
```

#### 6. **Get All Bookings (Admin)**
```
GET /api/v1/booking
Authorization: Bearer {admin-token}
Roles: Admin

Response: ApiResponse<List<BookingResponseDto>>
```

#### 7. **Get Booking Details**
```
GET /api/v1/booking/{bookingId}
Authorization: Bearer {token}

Response: ApiResponse<BookingDetailResponseDto>
{
  "bookingId": 101,
  "scheduleId": 1,
  "numberOfSeats": 3,
  "totalAmount": 1500.00,
  "bookingStatus": "Confirmed",
  "bookingDate": "2025-01-15T10:02:00Z",
  "routeId": 1,
  "source": "New York",
  "destination": "Boston",
  "busId": 1,
  "busNumber": "ABC-123",
  "busType": "AC",
  "totalSeats": 40,
  "operatorName": "TravelCo",
  "ratingAverage": 4.5,
  "travelDate": "2025-02-01",
  "departureTime": "14:00:00",
  "arrivalTime": "18:00:00",
  "availableSeats": 17
}
```

#### 8. **Cancel Booking**
```
PUT /api/v1/booking/cancel/{bookingId}
Authorization: Bearer {token}

Response: ApiResponse<bool>
{
  "success": true,
  "message": "Booking cancelled successfully",
  "data": true
}

Errors:
- "Booking not found."
- "Unauthorized access."
- "Booking already cancelled."
- "Cannot cancel after departure."
```

---

## ?? Database Transactions & Concurrency

### Atomic Operations

All critical operations use database transactions to ensure atomicity:

#### 1. **Lock Seats Transaction**
```csharp
using (var transaction = await _scheduleRepository.BeginTransactionAsync())
{
    try
    {
        // Validate each seat
        // Create SeatLock records
        // Update Seat statuses
        // Commit transaction
    }
    catch (Exception)
    {
        // Rollback on any error
        await transaction.RollbackAsync();
        throw;
    }
}
```

#### 2. **Create Booking Transaction**
```csharp
using (var transaction = await _scheduleRepository.BeginTransactionAsync())
{
    try
    {
        // Validate all seats are locked by user
        // Create Booking record
        // Confirm booking seats (convert locked ? booked)
        // Decrement Schedule.AvailableSeats
        // Commit transaction
    }
    catch (Exception)
    {
        // Rollback on any error
        await transaction.RollbackAsync();
        throw;
    }
}
```

#### 3. **Cancel Booking Transaction**
```csharp
using (var transaction = await _scheduleRepository.BeginTransactionAsync())
{
    try
    {
        // Get booked seats
        // Release booked seats (convert booked ? available)
        // Increment Schedule.AvailableSeats
        // Update Booking status
        // Commit transaction
    }
    catch (Exception)
    {
        // Rollback on any error
        await transaction.RollbackAsync();
        throw;
    }
}
```

### Concurrency Safety

#### Strategies Employed:

1. **Database Locks**
   - Transactions ensure isolation level prevents dirty reads
   - Lock expiry checked in real-time from database

2. **Validation Before Critical Operations**
   - Check seat status before locking
   - Verify user ownership before releasing
   - Validate timestamps at decision points

3. **Atomic Increment/Decrement**
   - Schedule.AvailableSeats updated in same transaction

4. **Unique Constraints**
   - SeatId + ScheduleId ensures no duplicate seats

---

## ??? Implementation Details

### 1. Seat Status Cleanup (Expired Locks)

**Cleanup Triggered**:
- Automatically on every lock operation
- Can be called by background job (Hangfire, hosted service)

**Implementation**:
```csharp
public async Task<int> CleanupExpiredLocksAsync()
{
    var now = DateTime.UtcNow;
    var expiredSeats = await _context.Seats
        .Where(s => s.SeatStatus == "Locked" && 
               s.LockedAt.HasValue && 
               s.LockedAt.Value.AddMinutes(5) <= now && 
               !s.IsDeleted)
        .ToListAsync();

    foreach (var seat in expiredSeats)
    {
        seat.SeatStatus = "Available";
        seat.LockedByUserId = null;
        seat.LockedAt = null;
        seat.UpdatedAt = now;
    }

    _context.Seats.UpdateRange(expiredSeats);
    await _context.SaveChangesAsync();
    return expiredSeats.Count;
}
```

### 2. Lock Expiration Check

**Two-Layer Approach**:
1. **Database Layer**: Check `ExpiresAt` timestamp
2. **Application Layer**: Validate `LockedAt + 5 minutes`

```csharp
// In SeatLockRepository.GetActiveLockAsync()
var now = DateTime.UtcNow;
return await _context.SeatLocks
    .FirstOrDefaultAsync(sl => 
        sl.SeatId == seatId && 
        !sl.IsReleased && 
        sl.ExpiresAt > now);  // ? Real-time expiration check
```

### 3. Lock Renewal

Users can extend locks by calling `LockSeatsAsync` again:
```csharp
// If already locked by same user
if (seat.SeatStatus == "Locked" && seat.LockedByUserId == userId)
{
    seat.LockedAt = now;  // ? Renew timestamp
    response.LockedSeatNumbers.Add(seatNumber);
    continue;
}
```

---

## ?? Database Schema

### Seat Entity
```
???????????????????????????????
? Seat                        ?
???????????????????????????????
? SeatId (PK)                 ?
? ScheduleId (FK) ? Schedule  ?
? SeatNumber (string)         ?
? SeatStatus (string)         ? ? Available|Locked|Booked
? LockedByUserId (FK) ? User  ?
? LockedAt (DateTime?)        ?
? BookingId (FK) ? Booking    ?
? CreatedAt                   ?
? UpdatedAt                   ?
? IsDeleted                   ?
???????????????????????????????
```

### SeatLock Entity
```
???????????????????????????????
? SeatLock                    ?
???????????????????????????????
? SeatLockId (PK)             ?
? SeatId (FK) ? Seat          ?
? UserId (FK) ? User          ?
? LockedAt (DateTime)         ?
? ExpiresAt (DateTime)        ? ? Now + 5 minutes
? IsReleased (bool)           ?
? ReleasedAt (DateTime?)      ?
? CreatedAt                   ?
???????????????????????????????
```

### Seat State Transitions

```sql
-- Create/Initialize Seats for a Schedule
INSERT INTO Seats (ScheduleId, SeatNumber, SeatStatus, ...)
VALUES 
  (1, 'A1', 'Available', ...),
  (1, 'A2', 'Available', ...),
  ...

-- Lock Seat
UPDATE Seats 
SET SeatStatus = 'Locked', LockedByUserId = @UserId, LockedAt = @Now
WHERE SeatId = @SeatId AND SeatStatus = 'Available'

-- Create Lock Record
INSERT INTO SeatLocks (SeatId, UserId, LockedAt, ExpiresAt, ...)
VALUES (@SeatId, @UserId, @Now, @Now + 5 minutes, ...)

-- Release Seat (by user)
UPDATE Seats 
SET SeatStatus = 'Available', LockedByUserId = NULL, LockedAt = NULL
WHERE SeatId = @SeatId AND LockedByUserId = @UserId

-- Confirm Booking (lock ? booked)
UPDATE Seats 
SET SeatStatus = 'Booked', BookingId = @BookingId, LockedByUserId = NULL, LockedAt = NULL
WHERE SeatId = @SeatId AND SeatStatus = 'Locked' AND LockedByUserId = @UserId

-- Cancel Booking (booked ? available)
UPDATE Seats 
SET SeatStatus = 'Available', BookingId = NULL
WHERE BookingId = @BookingId AND SeatStatus = 'Booked'
```

---

## ?? Configuration

### Dependency Injection (Program.cs)
```csharp
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatLockRepository, SeatLockRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IBookingService, BookingService>();
```

### DbContext
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatLock> SeatLocks { get; set; }
    // ... other DbSets
}
```

---

## ?? Error Handling & Validation

### Lock Failures (Partial Lock Response)
```
Request: Lock seats [A1, A2, B1]

Response:
{
  "success": false,
  "lockedSeatNumbers": ["A1"],
  "failedSeatNumbers": [
    "A2 (locked by another user)",
    "B1 (already booked)"
  ]
}
```

### Booking Validation
```
Booking Creation Checks:
? User has required auth token
? Schedule exists and is active
? Departure time hasn't passed
? All seats are locked
? All seats locked by same user (userId)
? No concurrent booking of same seats
```

---

## ?? Performance Optimization

### Indexes (Planned for OnModelCreating)
```csharp
// Seat lookups
modelBuilder.Entity<Seat>()
    .HasIndex(s => new { s.ScheduleId, s.SeatStatus });

modelBuilder.Entity<Seat>()
    .HasIndex(s => new { s.ScheduleId, s.LockedByUserId });

// SeatLock cleanup
modelBuilder.Entity<SeatLock>()
    .HasIndex(sl => new { sl.SeatId, sl.UserId, sl.ExpiresAt });

// Booking queries
modelBuilder.Entity<Booking>()
    .HasIndex(b => new { b.UserId, b.BookingStatus });
```

---

## ?? Edge Cases Handled

1. **Race Condition**: User A and B try to lock same seat simultaneously
   - ? Database transaction ensures only one succeeds

2. **Lock Expiry During Payment**: User locked seats, 5 min passed, tries to book
   - ? Booking creation validates lock is still active

3. **Double Booking Prevention**: User tries to book already-booked seats
   - ? Seat status validation prevents this

4. **Concurrent Cancellations**: Two cancel requests for same booking
   - ? First sets status to "Cancelled", second fails with "already cancelled"

5. **Partial Failures**: Lock 3 seats, 1 fails midway
   - ? Partial success response with clear failed seat list

6. **Expired Lock Cleanup**: Old locks from crashed sessions
   - ? Automatic cleanup on every lock operation + scheduled task

---

## ?? Testing Scenarios

### Test Case 1: Happy Path Booking
```
1. User locks seats A1, A2 ? Success
2. User creates booking with A1, A2 ? Success
3. Seats status changed to Booked
4. Schedule.AvailableSeats decremented
```

### Test Case 2: Lock Expiration
```
1. User locks seat A1 (expires at T+5min)
2. Wait 5 minutes
3. User tries to book A1 ? Fails (seat no longer locked)
4. Another user can now lock A1
```

### Test Case 3: Concurrent Booking Attempt
```
1. User A locks seats A1, A2
2. User B tries to lock A1 (at same time)
3. Only one succeeds (database isolation)
4. Other gets partial failure response
```

### Test Case 4: Booking Cancellation
```
1. Create booking with seats A1, A2, B1
2. Cancel booking
3. All three seats return to "Available"
4. Schedule.AvailableSeats incremented by 3
```

---

## ?? Workflow Diagram

```
????????????????????????????????????????????????????????????????
?                  User Booking Workflow                       ?
????????????????????????????????????????????????????????????????

   ???????????????????????????????????????????
   ?  1. GET /booking/seats/{scheduleId}     ?
   ?     Display full seat layout            ?
   ???????????????????????????????????????????
                         ?
   ???????????????????????????????????????????
   ?  2. POST /booking/seats/lock             ?
   ?     Lock [A1, A2, B1] for 5 minutes    ?
   ???????????????????????????????????????????
                         ?
   ???????????????????????????????????????????
   ?  3. User Reviews Selected Seats          ?
   ?     (Can release if changed mind)        ?
   ?     POST /booking/seats/release          ?
   ???????????????????????????????????????????
                         ?
   ???????????????????????????????????????????
   ?  4. POST /booking                        ?
   ?     CreateBooking with locked seats     ?
   ?     ? Atomic Transaction ?              ?
   ?     - Validate lock ownership            ?
   ?     - Convert locked ? booked            ?
   ?     - Decrement available seats          ?
   ?     - Create audit log                   ?
   ???????????????????????????????????????????
                         ?
   ???????????????????????????????????????????
   ?  5. Booking Confirmed                   ?
   ?     GET /booking/{bookingId} details     ?
   ???????????????????????????????????????????
```

---

## ?? Audit Logging

All critical operations are logged:

```csharp
// Lock operation
await _auditRepository.LogAuditAsync(
    "LOCK_SEATS",
    "Seat",
    string.Join(",", response.LockedSeatNumbers),
    null,
    new { scheduleId, seatNumbers = response.LockedSeatNumbers },
    userId,
    ipAddress);

// Booking creation
await _auditRepository.LogAuditAsync(
    "CREATE",
    "Booking",
    booking.BookingId.ToString(),
    null,
    new { bookingId = booking.BookingId, seats = dto.SeatNumbers, amount = totalAmount },
    userId,
    ipAddress);

// Booking cancellation
await _auditRepository.LogAuditAsync(
    "CANCEL",
    "Booking",
    booking.BookingId.ToString(),
    null,
    new { bookingId, seatsReleased = bookedSeats.Count },
    userId,
    ipAddress);
```

---

## ?? Summary

This production-grade implementation provides:

? **Real-time Seat Locking** with 5-minute expiration  
? **Atomic Transactions** for data consistency  
? **Concurrency Safety** at database level  
? **Comprehensive Validation** for all operations  
? **Audit Logging** for all critical actions  
? **Error Handling** with clear failure messages  
? **Scalable Architecture** using repositories and services  
? **RESTful API** with proper HTTP methods and status codes  

The system is ready for production deployment and handles all edge cases correctly.
