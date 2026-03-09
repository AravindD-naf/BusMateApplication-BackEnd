# Quick Start Guide - Bus Ticketing System

## 🚀 Project Setup (5 minutes)

### 1. Prerequisites
```bash
# .NET 10 SDK installed
dotnet --version

# SQL Server running
# Update connection string in appsettings.json
```

### 2. Database Setup
```bash
# Navigate to project
cd BusTicketingSystem

# Apply migrations
dotnet ef database update

# This will:
# - Create all tables
# - Create Source and Destination master tables
# - Create stored procedure for seat generation
```

### 3. Run Application
```bash
# Start the server
dotnet run

# Server runs on http://localhost:5000
```

### 4. Run Tests
```bash
# Run all tests
dotnet test BusTicketingSystem.Tests

# Expected: All tests pass
```

---

## 📚 Key Files to Know

| File | Purpose |
|------|---------|
| `Models/Bus.cs` | Bus entity (max 40 seats, InActive on creation) |
| `Models/Source.cs` | Source master table |
| `Models/Destination.cs` | Destination master table |
| `Services/ScheduleService.cs` | Calls stored procedure for seat generation |
| `Controllers/BusesController.cs` | Bus API endpoints (POST) |
| `Controllers/SourceController.cs` | Source API endpoints (POST) |
| `Controllers/DestinationController.cs` | Destination API endpoints (POST) |
| `Program.cs` | Dependency injection setup |

---

## 🔑 Critical Implementation Details

### 1. Bus Creation
**Result**: Bus created with `IsActive = false`
```bash
POST /api/v1/buses
{
  "busNumber": "BUS001",
  "busType": "AC",
  "totalSeats": 40,
  "operatorName": "TravelCorp"
}
```

### 2. Schedule Creation
**Result**: Seats auto-generated + Bus becomes Active
```bash
POST /api/v1/schedules
{
  "routeId": 1,
  "busId": 1,
  "travelDate": "2025-04-01",
  "departureTime": "10:00:00",
  "arrivalTime": "16:00:00"
}
```

### 3. List Endpoints (All POST)
**Pattern**: POST with pagination in body
```bash
POST /api/v1/buses/get-all
{
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## 🧪 Quick Test

### Login
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@system.com",
    "password": "Admin@123"
  }'
```

### Create Bus
```bash
curl -X POST http://localhost:5000/api/v1/buses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "busNumber": "BUS001",
    "busType": "AC",
    "totalSeats": 40,
    "operatorName": "TestOperator"
  }'
```

### Get All Buses
```bash
curl -X POST http://localhost:5000/api/v1/buses/get-all \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }'
```

---

## 🧩 Project Structure

```
BusTicketingSystem/
├── Models/                    # Entities
│   ├── Bus.cs
│   ├── Route.cs
│   ├── Schedule.cs
│   ├── Source.cs (NEW)
│   └── Destination.cs (NEW)
├── Services/                  # Business logic
│   ├── BusService.cs
│   ├── ScheduleService.cs
│   ├── SourceService.cs (NEW)
│   └── DestinationService.cs (NEW)
├── Controllers/               # API endpoints
│   ├── BusesController.cs
│   ├── SourceController.cs (NEW)
│   ├── DestinationController.cs (NEW)
│   └── ...
├── Repositories/              # Data access
│   ├── BusRepository.cs
│   ├── SourceRepository.cs (NEW)
│   └── DestinationRepository.cs (NEW)
├── DTOs/                      # Data transfer
│   ├── Requests/
│   │   ├── PaginationRequest.cs (NEW)
│   │   ├── OperatorSearchRequest.cs (NEW)
│   │   └── ...
│   └── Responses/
├── Migrations/                # Database
│   ├── 20250306_AddSourceAndDestinationTables.cs (NEW)
│   ├── 20250306_CreateStoredProcedureForSeats.cs (NEW)
│   └── ...
└── Data/
    └── ApplicationDbContext.cs

BusTicketingSystem.Tests/
├── Services/                  # Service tests
├── Models/                    # Model tests
├── DTOs/                      # DTO tests
├── Controllers/               # Controller tests
└── Integration/               # Integration tests
```

---

## 📋 API Conversion Summary

| Old Method | New Method | Body Parameter |
|-----------|-----------|-----------------|
| GET /buses | POST /buses/get-all | PaginationRequest |
| GET /routes | POST /routes/get-all | PaginationRequest |
| GET /schedules | POST /schedules/get-all | PaginationRequest |
| GET /booking/my | POST /booking/my-bookings | PaginationRequest |
| GET /auditlogs | POST /auditlogs/get-all | AuditLogSearchRequest |

---

## ⚠️ Important Notes

1. **Bus State**: Bus is **InActive** when created, becomes **Active** when scheduled
2. **Seat Limit**: Maximum 40 seats per bus (enforced via validation)
3. **Seats Generation**: Automatic via stored procedure when schedule is created
4. **Pagination**: All list endpoints use POST with pagination in request body
5. **Status Codes**: Proper HTTP status codes returned (200, 400, 401, 404, 409, 500)

---

## 🔍 Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| "Bus not found" (404) | Verify bus ID exists |
| "Total seats must be between 1 and 40" (400) | Check seat count |
| "Unauthorized" (401) | Include JWT token in Authorization header |
| "Bus number already exists" (409) | Use unique bus number |
| Migration fails | Ensure connection string is correct |

---

## 📖 Documentation

- **README.md** - Complete API documentation
- **TESTING_GUIDE.md** - Detailed testing guide
- **API_USAGE_EXAMPLES.md** - cURL examples for all endpoints
- **IMPLEMENTATION_SUMMARY.md** - Implementation details
- **PROJECT_VERIFICATION_REPORT.md** - Verification checklist

---

## 🎯 Important Classes to Know

### Request Models (Pagination)
```csharp
public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

### Response Format
```csharp
{
    "success": true,
    "message": "Operation successful",
    "data": { /* actual data */ }
}
```

### Search Requests
- `OperatorSearchRequest` - Search buses by operator
- `RouteSourceSearchRequest` - Search routes by source
- `ScheduleSearchRequest` - Advanced schedule search
- `AuditLogSearchRequest` - Filter audit logs

---

## 🚀 Deployment Steps

1. **Backup Database** (if production)
2. **Run Migrations**: `dotnet ef database update`
3. **Build Project**: `dotnet build --configuration Release`
4. **Run Tests**: `dotnet test` (ensure all pass)
5. **Start Application**: `dotnet run`
6. **Verify Endpoints** using API examples

---

## 📞 Support

For detailed information:
- Check README.md for full API documentation
- Review TESTING_GUIDE.md for test scenarios
- See API_USAGE_EXAMPLES.md for cURL commands
- Look at test files for implementation examples

---

## ✅ Verification Checklist

Before going live:
- [ ] Database migration completed
- [ ] Application builds successfully
- [ ] Tests pass
- [ ] Can create bus (becomes InActive)
- [ ] Can create schedule (auto-generates seats)
- [ ] Bus becomes Active after schedule creation
- [ ] POST endpoints work with pagination
- [ ] Error messages are clear

---

**Last Updated**: March 6, 2025  
**Status**: Ready for Production ✅
