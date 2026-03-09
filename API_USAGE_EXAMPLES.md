# API Usage Examples

## Authentication

### Login
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@system.com",
    "password": "Admin@123"
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 1,
    "email": "admin@system.com",
    "role": "Admin"
  }
}
```

---

## Bus Management

### 1. Create Bus (Admin Only)
```bash
curl -X POST http://localhost:5000/api/v1/buses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "busNumber": "BUS001",
    "busType": "AC Sleeper",
    "totalSeats": 40,
    "operatorName": "TravelCorp Express"
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "Bus created successfully",
  "data": {
    "busId": 1,
    "busNumber": "BUS001",
    "busType": "AC Sleeper",
    "totalSeats": 40,
    "operatorName": "TravelCorp Express",
    "isActive": false
  }
}
```

**Note**: Bus is created in **InActive** state. It becomes Active when first scheduled.

### 2. Get All Buses (Pagination)
```bash
curl -X POST http://localhost:5000/api/v1/buses/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "Buses retrieved successfully",
  "data": [
    {
      "busId": 1,
      "busNumber": "BUS001",
      "busType": "AC Sleeper",
      "totalSeats": 40,
      "operatorName": "TravelCorp Express",
      "isActive": false
    }
  ]
}
```

### 3. Get Single Bus
```bash
curl -X POST http://localhost:5000/api/v1/buses/1 \
  -H "Content-Type: application/json"
```

### 4. Update Bus (Admin Only)
```bash
curl -X PUT http://localhost:5000/api/v1/buses/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "busType": "AC Sleeper",
    "totalSeats": 40,
    "operatorName": "TravelCorp Express",
    "isActive": false
  }'
```

### 5. Delete Bus (Admin Only)
```bash
curl -X DELETE http://localhost:5000/api/v1/buses/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 6. Search Buses by Operator
```bash
curl -X POST http://localhost:5000/api/v1/buses/search-by-operator \
  -H "Content-Type: application/json" \
  -d '{
    "operatorName": "TravelCorp Express",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

---

## Source & Destination Management

### Create Source (Admin Only)
```bash
curl -X POST http://localhost:5000/api/v1/sources \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "sourceName": "Delhi",
    "description": "Capital of India"
  }'
```

### Get All Sources
```bash
curl -X POST http://localhost:5000/api/v1/sources/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Create Destination (Admin Only)
```bash
curl -X POST http://localhost:5000/api/v1/destinations \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "destinationName": "Mumbai",
    "description": "Financial Hub"
  }'
```

### Get All Destinations
```bash
curl -X POST http://localhost:5000/api/v1/destinations/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

---

## Route Management

### Create Route (Admin Only)
```bash
curl -X POST http://localhost:5000/api/v1/routes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "source": "Delhi",
    "destination": "Mumbai",
    "distance": 1400,
    "estimatedTravelTimeMinutes": 600,
    "baseFare": 1500
  }'
```

### Get All Routes
```bash
curl -X POST http://localhost:5000/api/v1/routes/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Search Routes by Source
```bash
curl -X POST http://localhost:5000/api/v1/routes/search-by-source \
  -H "Content-Type: application/json" \
  -d '{
    "source": "Delhi",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Search Routes by Destination
```bash
curl -X POST http://localhost:5000/api/v1/routes/search-by-destination \
  -H "Content-Type: application/json" \
  -d '{
    "destination": "Mumbai",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Advanced Route Search
```bash
curl -X POST http://localhost:5000/api/v1/routes/search \
  -H "Content-Type: application/json" \
  -d '{
    "source": "Delhi",
    "destination": "Mumbai",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

---

## Schedule Management

### Create Schedule (Admin Only)
**Important**: This endpoint automatically:
1. Generates seats via stored procedure
2. Activates the bus (sets IsActive = true)

```bash
curl -X POST http://localhost:5000/api/v1/schedules \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "routeId": 1,
    "busId": 1,
    "travelDate": "2025-04-01",
    "departureTime": "10:00:00",
    "arrivalTime": "16:00:00"
  }'
```

### Get All Schedules
```bash
curl -X POST http://localhost:5000/api/v1/schedules/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Search Schedules by From City
```bash
curl -X POST http://localhost:5000/api/v1/schedules/search-by-from-city \
  -H "Content-Type: application/json" \
  -d '{
    "city": "Delhi",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Search Schedules by To City
```bash
curl -X POST http://localhost:5000/api/v1/schedules/search-by-to-city \
  -H "Content-Type: application/json" \
  -d '{
    "city": "Mumbai",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Advanced Schedule Search
```bash
curl -X POST http://localhost:5000/api/v1/schedules/search \
  -H "Content-Type: application/json" \
  -d '{
    "fromCity": "Delhi",
    "toCity": "Mumbai",
    "travelDate": "2025-04-01",
    "pageNumber": 1,
    "pageSize": 10
  }'
```

---

## Booking Management

### Get Seat Layout for Schedule
```bash
curl -X POST http://localhost:5000/api/v1/booking/seats/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Lock Seats (Customer)
```bash
curl -X POST http://localhost:5000/api/v1/booking/seats/lock \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1", "A2", "A3"]
  }'
```

### Create Booking (Customer)
```bash
curl -X POST http://localhost:5000/api/v1/booking \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1", "A2"],
    "passengers": [
      {
        "fullName": "John Doe",
        "email": "john@example.com",
        "phoneNumber": "9876543210",
        "age": 30
      }
    ],
    "totalFare": 3000
  }'
```

### Get My Bookings (Customer)
```bash
curl -X POST http://localhost:5000/api/v1/booking/my-bookings \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Get All Bookings (Admin Only)
```bash
curl -X POST http://localhost:5000/api/v1/booking/get-all \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

### Get Booking by ID
```bash
curl -X POST http://localhost:5000/api/v1/booking/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Cancel Booking (Customer/Admin)
```bash
curl -X PUT http://localhost:5000/api/v1/booking/cancel/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Audit Logs (Admin Only)

### Get Audit Logs with Filters
```bash
curl -X POST http://localhost:5000/api/v1/auditlogs/get-all \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10,
    "entityName": "Bus",
    "userId": 1,
    "fromDate": "2025-03-01",
    "toDate": "2025-04-01"
  }'
```

---

## Common Errors & Solutions

### 400 Bad Request
```json
{
  "success": false,
  "message": "Total seats must be between 1 and 40"
}
```
**Solution**: Ensure seats are between 1 and 40

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Invalid token or not authenticated"
}
```
**Solution**: Include valid JWT token in Authorization header

### 404 Not Found
```json
{
  "success": false,
  "message": "Bus not found"
}
```
**Solution**: Verify the resource ID exists

### 409 Conflict
```json
{
  "success": false,
  "message": "Bus number BUS001 already exists"
}
```
**Solution**: Use a different bus number

---

## Response Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | OK | Bus retrieved successfully |
| 201 | Created | Bus created successfully |
| 400 | Bad Request | Invalid seats (> 40) |
| 401 | Unauthorized | Missing/invalid token |
| 404 | Not Found | Bus ID doesn't exist |
| 409 | Conflict | Bus number already exists |
| 500 | Server Error | Database connection error |

---

## Testing Workflow

1. **Create Bus** (Admin)
2. **Create Source** (Admin)
3. **Create Destination** (Admin)
4. **Create Route** (Admin)
5. **Create Schedule** (Admin) - Auto-generates seats, activates bus
6. **View Seats** (Customer) - See available seats
7. **Lock Seats** (Customer) - Reserve seats for 5 minutes
8. **Create Booking** (Customer) - Complete booking
9. **View My Bookings** (Customer) - See booking history
