# Seat Selection System - Quick API Reference

## ?? Quick Start Guide

### 1. Display Available Seats
```bash
curl -X GET "https://localhost:5001/api/v1/booking/seats/1" \
  -H "Authorization: Bearer {token}"
```

**Response**: Full seat layout with status for all seats

---

### 2. Lock Seats (5-minute reservation)
```bash
curl -X POST "https://localhost:5001/api/v1/booking/seats/lock" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1", "A2", "B1"]
  }'
```

**Success (200 OK)**:
```json
{
  "success": true,
  "message": "All 3 seats locked successfully.",
  "lockedSeatNumbers": ["A1", "A2", "B1"],
  "failedSeatNumbers": [],
  "lockExpiresAt": "2025-01-15T10:05:00Z"
}
```

**Partial Success (200 OK)**:
```json
{
  "success": false,
  "message": "Locked 2 seats. Failed: 1",
  "lockedSeatNumbers": ["A1", "A2"],
  "failedSeatNumbers": ["B1 (already booked)"],
  "lockExpiresAt": "2025-01-15T10:05:00Z"
}
```

---

### 3. Release Seats (Before Booking)
```bash
curl -X POST "https://localhost:5001/api/v1/booking/seats/release" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1"]
  }'
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "All 1 seats released successfully.",
  "releasedSeatNumbers": ["A1"],
  "failedSeatNumbers": []
}
```

---

### 4. Create Booking (with locked seats)
```bash
curl -X POST "https://localhost:5001/api/v1/booking" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A2", "B1"]
  }'
```

**Success (200 OK)**:
```json
{
  "success": true,
  "message": "Booking created successfully",
  "data": {
    "bookingId": 101,
    "scheduleId": 1,
    "numberOfSeats": 2,
    "totalAmount": 1000,
    "bookingStatus": "Confirmed",
    "bookingDate": "2025-01-15T10:02:00Z"
  }
}
```

**Error (400 Bad Request)**:
```json
{
  "success": false,
  "message": "Seat A2 is not locked. Please lock seats before booking.",
  "data": null
}
```

---

### 5. View Booking Details
```bash
curl -X GET "https://localhost:5001/api/v1/booking/101" \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "Booking retrieved successfully",
  "data": {
    "bookingId": 101,
    "scheduleId": 1,
    "numberOfSeats": 2,
    "totalAmount": 1000,
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
}
```

---

### 6. View My Bookings
```bash
curl -X GET "https://localhost:5001/api/v1/booking/my" \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "Bookings retrieved successfully",
  "data": [
    {
      "bookingId": 101,
      "scheduleId": 1,
      "numberOfSeats": 2,
      "totalAmount": 1000,
      "bookingStatus": "Confirmed",
      "bookingDate": "2025-01-15T10:02:00Z"
    },
    {
      "bookingId": 102,
      "scheduleId": 2,
      "numberOfSeats": 1,
      "totalAmount": 500,
      "bookingStatus": "Confirmed",
      "bookingDate": "2025-01-14T15:30:00Z"
    }
  ]
}
```

---

### 7. Cancel Booking
```bash
curl -X PUT "https://localhost:5001/api/v1/booking/cancel/101" \
  -H "Authorization: Bearer {token}"
```

**Success (200 OK)**:
```json
{
  "success": true,
  "message": "Booking cancelled successfully",
  "data": true
}
```

**Error (400 Bad Request)**:
```json
{
  "success": false,
  "message": "Cannot cancel after departure.",
  "data": null
}
```

---

## ?? Seat Status Legend

| Status | Description | Can Lock? | Can Book? |
|--------|-------------|-----------|-----------|
| **Available** | Seat is free | ? Yes | ? If locked |
| **Locked** | Reserved for 5 min | ?? Renew only | ? Only by locker |
| **Booked** | Confirmed booking | ? No | ? No |

---

## ?? Typical User Flow

### Scenario: Book 3 seats (A1, A2, B1)

```
Time  ? Action                      ? Lock Status
???????????????????????????????????????????????????????????????
00:00 ? GET /seats/1                ? Show all Available
      ? User selects A1, A2, B1     ?
      ? POST /lock [A1,A2,B1]       ? ? A1, A2, B1 ? Locked
      ?                             ? Expires: 00:05
      ?
00:02 ? User reviews & confirms     ? Still locked
      ? POST /booking               ? ? A1, A2, B1 ? Booked
      ? (Creates Booking #101)      ? Schedule seats -3
      ?
00:06 ? (Locked seats already       ? (Not needed since booked)
      ?  converted to booked)       ?
      ?
01:00 ? User cancels booking #101   ? ? A1, A2, B1 ? Available
      ? PUT /cancel/101             ? Schedule seats +3
```

### Scenario: User locks but changes mind

```
Time  ? Action                      ? Lock Status
???????????????????????????????????????????????????????????????
00:00 ? POST /lock [A1, A2]         ? ? A1, A2 ? Locked
      ? Expires: 00:05              ?
      ?
00:01 ? User changes mind           ?
      ? POST /release [A1, A2]      ? ? A1, A2 ? Available
      ?                             ?
00:03 ? Another user locks A1       ? ? A1 ? Locked (by User B)
      ? POST /lock [A1]             ?
```

### Scenario: Lock expires without booking

```
Time  ? Action                      ? Lock Status
???????????????????????????????????????????????????????????????
00:00 ? POST /lock [A1]             ? ? A1 ? Locked
      ? Expires: 00:05              ? Locker: User A
      ?
00:05 ? (No action from User A)     ?
      ? Lock automatically expired  ? ? A1 ? Available
      ?                             ? (Next GET or POST triggers cleanup)
      ?
00:06 ? Another user can lock A1    ? ? A1 ? Locked (by User B)
      ? POST /lock [A1]             ?
```

---

## ?? Key Points

### Lock Behavior
- ?? Locks expire after **5 minutes**
- ?? User can **renew** by locking again
- ?? Only **user who locked** can release
- ? **Cannot lock** already-booked seats
- ?? **Partial locks** allowed (some succeed, some fail)

### Booking Behavior
- ? Requires **all seats to be locked** by same user
- ?? Converts **Locked ? Booked** atomically
- ?? Decrements **Schedule.AvailableSeats**
- ? Cannot book **after departure time**
- ?? Cannot rebook **same schedule with same seats**

### Cancellation
- ? **Owner or Admin** can cancel
- ? Cannot cancel **after departure**
- ?? Releases **all booked seats**
- ?? Increments **Schedule.AvailableSeats**

---

## ?? Error Codes

| HTTP Status | Error | Meaning |
|-------------|-------|---------|
| 200 | - | Operation successful |
| 400 | Bad Request | Invalid input or business logic failure |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | User doesn't have permission |
| 404 | Not Found | Booking/Schedule/Seat not found |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server error |

---

## ?? Best Practices

### For Frontend Implementation

```javascript
// 1. Load seat layout
async function loadSeatLayout(scheduleId) {
  const response = await fetch(`/api/v1/booking/seats/${scheduleId}`);
  return await response.json();
}

// 2. Lock selected seats
async function lockSeats(scheduleId, seatNumbers) {
  const response = await fetch('/api/v1/booking/seats/lock', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ scheduleId, seatNumbers })
  });
  return await response.json();
}

// 3. Handle partial locks
function handleLockResponse(response) {
  if (response.lockedSeatNumbers.length > 0) {
    showSuccess(`Locked ${response.lockedSeatNumbers.length} seats`);
    highlightSeats(response.lockedSeatNumbers, 'locked');
  }
  
  if (response.failedSeatNumbers.length > 0) {
    showWarning(`Failed to lock: ${response.failedSeatNumbers.join(', ')}`);
  }
  
  // Start countdown timer
  startLockExpiryTimer(response.lockExpiresAt);
}

// 4. Create booking before lock expires
async function createBooking(scheduleId, seatNumbers) {
  const response = await fetch('/api/v1/booking', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ scheduleId, seatNumbers })
  });
  
  if (response.status === 200) {
    const booking = await response.json();
    showSuccess(`Booking ${booking.data.bookingId} confirmed!`);
    return booking.data;
  } else {
    const error = await response.json();
    showError(error.message);
    return null;
  }
}

// 5. Show lock expiry countdown
function startLockExpiryTimer(expiresAt) {
  const expiryTime = new Date(expiresAt);
  const updateInterval = setInterval(() => {
    const remaining = (expiryTime - new Date()) / 1000;
    if (remaining <= 0) {
      showWarning('Lock expired! Please lock seats again.');
      clearInterval(updateInterval);
    } else {
      updateUI(`Locks expire in: ${Math.ceil(remaining)}s`);
    }
  }, 1000);
}
```

---

## ?? Support

For issues or questions:
1. Check error message for specific failure reason
2. Verify all seats are locked before booking
3. Ensure lock hasn't expired (check `lockExpiresAt`)
4. Check user permissions (Customer vs Admin roles)
5. Review audit logs for operation history

---

## ?? Related Files

- **Implementation**: `BusTicketingSystem/SEAT_SELECTION_SYSTEM.md`
- **Services**: `BusTicketingSystem/Services/SeatService.cs`, `BookingService.cs`
- **Repositories**: `BusTicketingSystem/Repositories/SeatRepository.cs`, `SeatLockRepository.cs`
- **Models**: `BusTicketingSystem/Models/Seat.cs`, `SeatLock.cs`
- **DTOs**: `BusTicketingSystem/DTOs/Requests/SeatRequestDtos.cs`, `SeatResponseDtos.cs`
