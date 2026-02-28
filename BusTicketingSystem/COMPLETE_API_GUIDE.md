# ?? Bus Ticketing System - Complete End-to-End API Guide

## ?? Table of Contents

1. [Overview & Architecture](#overview--architecture)
2. [Authentication & Authorization](#authentication--authorization)
3. [Complete Workflow](#complete-workflow)
4. [API Endpoints Reference](#api-endpoints-reference)
5. [Detailed Payload Examples](#detailed-payload-examples)
6. [Role-Based Access Control](#role-based-access-control)
7. [Error Handling](#error-handling)
8. [Testing Scenarios](#testing-scenarios)

---

## ??? Overview & Architecture

### Application Structure

```
Bus Ticketing System
??? Authentication (Users, Roles)
??? Route & Schedule Management
??? Seat Selection & Locking
??? Booking Management
??? Payment Processing (Dummy)
??? Refund Management
??? Passenger Details
```

### Available Roles

```
1. Admin
   - Full access to all endpoints
   - Can manage all users' bookings
   - Can process refunds
   - Can view all analytics

2. Customer
   - Can book seats
   - Can view own bookings
   - Can manage own payments
   - Cannot view other users' data
```

---

## ?? Authentication & Authorization

### Step 1: Register a New User (Customer)

**Endpoint:**
```
POST /api/v1/auth/register
```

**Access:** Public (No token required)

**Request Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+919876543210",
  "password": "SecurePassword@123"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": 5,
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "role": "Customer"
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email already exists",
  "data": null
}
```

---

### Step 2: Login (Get Authentication Token)

**Endpoint:**
```
POST /api/v1/auth/login
```

**Access:** Public (No token required)

**Request Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkMWZkYWQwMi1iMDI0LTQwZWQtYjc2Ni00NzE2ZDZkOGFlNjQiLCJlbWFpbCI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwidXNlcklkIjo1LCJyb2xlIjoiQ3VzdG9tZXIiLCJleHAiOjE3MzU2NDEzNDJ9.abc123...",
    "userId": 5,
    "email": "john.doe@example.com",
    "role": "Customer",
    "expiresIn": 1800
  }
}
```

**Important Notes:**
- Token expires in **30 minutes** (default)
- Save this token - you'll need it for all subsequent requests
- Token format: `Bearer {token}`

---

## ?? Complete Workflow

### End-to-End Booking Flow

```
???????????????????????????????????????????????????????????
? COMPLETE BOOKING WORKFLOW                               ?
???????????????????????????????????????????????????????????

Step 1: AUTHENTICATION
   ?? POST /auth/login
      ?? Get Access Token
      ?? Token expires in 30 minutes

Step 2: SEARCH SCHEDULES
   ?? GET /schedules
      ?? Find available buses
      ?? Check departure times
      ?? View available seats

Step 3: VIEW SEAT LAYOUT
   ?? GET /booking/seats/{scheduleId}
      ?? See all seats with status
      ?? Identify available seats
      ?? Plan seat selection

Step 4: LOCK SEATS (5 minutes)
   ?? POST /booking/seats/lock
      ?? Reserve selected seats
      ?? Lock expires in 5 minutes
      ?? Only you can use these seats

Step 5: INITIATE PAYMENT
   ?? POST /booking/payment/initiate
      ?? Create payment record
      ?? Payment expires in 15 minutes
      ?? Status: Pending

Step 6: CREATE BOOKING
   ?? POST /booking
      ?? Convert locked seats to booked
      ?? Booking status: Pending
      ?? Payment required

Step 7: CONFIRM PAYMENT
   ?? POST /booking/payment/confirm
      ?? Dummy payment processing
      ?? Payment status: Success
      ?? Booking status: Confirmed

Step 8: ADD PASSENGER DETAILS
   ?? POST /booking/passengers
      ?? Add names for each seat
      ?? Add ID information
      ?? Booking ready for travel

Step 9: GET CONFIRMATION
   ?? GET /booking/{bookingId}
      ?? View full booking details
      ?? Download e-ticket (future)
      ?? Share confirmation

[OPTIONAL] CANCEL BOOKING
   ?? PUT /booking/cancel/{bookingId}
      ?? Calculate refund
      ?? Release seats
      ?? Update booking status
```

---

## ?? API Endpoints Reference

### A. Authentication Endpoints

#### 1. Register User
```
POST /api/v1/auth/register
Access: Public
Role: None Required
```

#### 2. Login
```
POST /api/v1/auth/login
Access: Public
Role: None Required
```

---

### B. Seat Management Endpoints

#### 3. Get Seat Layout
```
GET /api/v1/booking/seats/{scheduleId}
Access: Authenticated
Role: Customer, Admin
```

#### 4. Lock Seats
```
POST /api/v1/booking/seats/lock
Access: Authenticated
Role: Customer, Admin
Duration: 5 minutes
```

#### 5. Release Seats
```
POST /api/v1/booking/seats/release
Access: Authenticated
Role: Customer, Admin (only own locks)
```

---

### C. Booking Endpoints

#### 6. Create Booking
```
POST /api/v1/booking
Access: Authenticated
Role: Customer, Admin
Requires: Locked seats
```

#### 7. Get Booking by ID
```
GET /api/v1/booking/{bookingId}
Access: Authenticated
Role: Customer (own bookings), Admin (all)
```

#### 8. Get My Bookings
```
GET /api/v1/booking/my
Access: Authenticated
Role: Customer, Admin
```

#### 9. Get All Bookings
```
GET /api/v1/booking
Access: Authenticated
Role: Admin Only
```

#### 10. Cancel Booking
```
PUT /api/v1/booking/cancel/{bookingId}
Access: Authenticated
Role: Customer (own bookings), Admin (all)
```

---

### D. Payment Endpoints

#### 11. Initiate Payment
```
POST /api/v1/booking/payment/initiate
Access: Authenticated
Role: Customer, Admin
Expires: 15 minutes
```

#### 12. Confirm Payment
```
POST /api/v1/booking/payment/confirm
Access: Authenticated
Role: Customer, Admin
Type: Dummy (no real processor)
```

#### 13. Get Payment Details
```
GET /api/v1/booking/payment/{paymentId}
Access: Authenticated
Role: Customer, Admin
```

---

### E. Passenger Endpoints

#### 14. Add Passengers
```
POST /api/v1/booking/passengers
Access: Authenticated
Role: Customer, Admin (own bookings)
Required: One per seat
```

#### 15. Get Passengers
```
GET /api/v1/booking/{bookingId}/passengers
Access: Authenticated
Role: Customer, Admin
```

---

### F. Refund Endpoints

#### 16. Initiate Refund
```
POST /api/v1/booking/refund/initiate
Access: Authenticated
Role: Customer, Admin
Based on: Cancellation policy
```

#### 17. Confirm Refund
```
POST /api/v1/booking/refund/confirm
Access: Authenticated
Role: Admin Only
```

---

## ?? Detailed Payload Examples

### Complete Workflow with Real Payloads

---

## ? STEP 1: AUTHENTICATION

### Login Request

```bash
curl -X POST "http://localhost:5000/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePassword@123"
  }'
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 5,
    "email": "john.doe@example.com",
    "role": "Customer",
    "expiresIn": 1800
  }
}
```

**Save the token:** You'll use it in all future requests as `Authorization: Bearer {token}`

---

## ? STEP 2: VIEW SEAT LAYOUT

### Get Available Seats

```bash
curl -X GET "http://localhost:5000/api/v1/booking/seats/1" \
  -H "Authorization: Bearer {YOUR_TOKEN}"
```

**Response:**
```json
{
  "success": true,
  "message": "Seat layout retrieved successfully",
  "data": {
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
        "lockedAt": null
      },
      {
        "seatId": 2,
        "seatNumber": "A2",
        "seatStatus": "Locked",
        "lockedByUserId": 5,
        "lockedAt": "2025-01-15T10:00:00Z",
        "lockedExpiresAt": "2025-01-15T10:05:00Z"
      },
      {
        "seatId": 3,
        "seatNumber": "A3",
        "seatStatus": "Booked",
        "lockedByUserId": null,
        "lockedAt": null
      }
    ]
  }
}
```

**Key Info:**
- `Available` = Can be locked
- `Locked` = Reserved by someone
- `Booked` = Already purchased

---

## ? STEP 3: LOCK SEATS

### Lock Selected Seats (5-minute reservation)

```bash
curl -X POST "http://localhost:5000/api/v1/booking/seats/lock" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1", "A2", "A3"]
  }'
```

**Request Parameters:**
- `scheduleId` (integer): The schedule you want to book from
- `seatNumbers` (array): Seat numbers like ["A1", "A2"]

**Response (Success):**
```json
{
  "success": true,
  "message": "All 3 seats locked successfully.",
  "data": {
    "success": true,
    "message": "All 3 seats locked successfully.",
    "lockedSeatNumbers": ["A1", "A2", "A3"],
    "failedSeatNumbers": [],
    "lockExpiresAt": "2025-01-15T10:05:00Z"
  }
}
```

**Response (Partial Success):**
```json
{
  "success": true,
  "message": "Partial lock response",
  "data": {
    "success": false,
    "message": "Locked 2 seats. Failed: 1",
    "lockedSeatNumbers": ["A1", "A2"],
    "failedSeatNumbers": [
      "A3 (already booked)"
    ],
    "lockExpiresAt": "2025-01-15T10:05:00Z"
  }
}
```

**Important:**
- Lock expires in **5 minutes**
- You must book before lock expires
- You can lock again to renew
- Only you can release your locks

---

## ? STEP 4: CREATE BOOKING

### Create Booking with Locked Seats

```bash
curl -X POST "http://localhost:5000/api/v1/booking" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "scheduleId": 1,
    "seatNumbers": ["A1", "A2", "A3"]
  }'
```

**Request Parameters:**
- `scheduleId` (integer): Must match the schedule from Step 2
- `seatNumbers` (array): Exact seats you locked in Step 3

**Response:**
```json
{
  "success": true,
  "message": "Booking created successfully",
  "data": {
    "bookingId": 101,
    "scheduleId": 1,
    "numberOfSeats": 3,
    "totalAmount": 1500.00,
    "bookingStatus": "Pending",
    "bookingDate": "2025-01-15T10:02:00Z"
  }
}
```

**Important:**
- Booking status is `Pending` (waiting for payment)
- Total amount = Number of seats × Price per seat (?500)
- Seats are now `Booked` (locked status removed)
- Save `bookingId` for next steps

---

## ? STEP 5: INITIATE PAYMENT

### Create Payment Record

```bash
curl -X POST "http://localhost:5000/api/v1/booking/payment/initiate" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": 101,
    "amount": 1500.00
  }'
```

**Request Parameters:**
- `bookingId` (integer): From Step 4
- `amount` (decimal): Total amount to pay (match booking total)

**Response:**
```json
{
  "success": true,
  "message": "Payment initiated successfully",
  "data": {
    "paymentId": 1,
    "bookingId": 101,
    "amount": 1500.00,
    "status": "Pending",
    "transactionId": "",
    "paymentMethod": "",
    "createdAt": "2025-01-15T10:02:30Z",
    "expiresAt": "2025-01-15T10:17:30Z"
  }
}
```

**Important:**
- Payment status is `Pending`
- Payment expires in **15 minutes**
- Save `paymentId` for Step 6
- Amount must match booking total

---

## ? STEP 6: CONFIRM PAYMENT (DUMMY)

### Simulate Payment Processing

```bash
curl -X POST "http://localhost:5000/api/v1/booking/payment/confirm" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "paymentId": 1,
    "transactionId": "TXN123456789",
    "isSuccess": true,
    "failureReason": ""
  }'
```

**Request Parameters:**
- `paymentId` (integer): From Step 5
- `transactionId` (string): Any unique ID (e.g., "TXN123456789")
- `isSuccess` (boolean): True = success, False = failed
- `failureReason` (string): If failed, why? (e.g., "Card declined")

**Response (Success):**
```json
{
  "success": true,
  "message": "Payment confirmed successfully",
  "data": {
    "paymentId": 1,
    "bookingId": 101,
    "amount": 1500.00,
    "status": "Success",
    "transactionId": "TXN123456789",
    "paymentMethod": "Card",
    "processedAt": "2025-01-15T10:03:00Z"
  }
}
```

**Response (Failure):**
```json
{
  "success": true,
  "message": "Payment confirmed",
  "data": {
    "paymentId": 1,
    "bookingId": 101,
    "amount": 1500.00,
    "status": "Failed",
    "transactionId": "TXN123456789",
    "failureReason": "Card declined"
  }
}
```

**Important:**
- If success: Booking status changes to `Confirmed`
- If failed: Booking status becomes `PaymentFailed`
- Payment record is created regardless of outcome
- Save transaction ID for records

---

## ? STEP 7: ADD PASSENGER DETAILS

### Add Names and IDs for Each Seat

```bash
curl -X POST "http://localhost:5000/api/v1/booking/passengers" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": 101,
    "passengers": [
      {
        "seatNumber": "A1",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com",
        "phoneNumber": "+919876543210",
        "idType": "Passport",
        "idNumber": "A12345678",
        "age": 30,
        "specialRequirements": ""
      },
      {
        "seatNumber": "A2",
        "firstName": "Jane",
        "lastName": "Doe",
        "email": "jane@example.com",
        "phoneNumber": "+919876543211",
        "idType": "Aadhaar",
        "idNumber": "123456789012",
        "age": 28,
        "specialRequirements": "Wheelchair accessibility"
      },
      {
        "seatNumber": "A3",
        "firstName": "Jack",
        "lastName": "Smith",
        "email": "jack@example.com",
        "phoneNumber": "+919876543212",
        "idType": "DrivingLicense",
        "idNumber": "DL12345",
        "age": 35,
        "specialRequirements": ""
      }
    ]
  }'
```

**Request Parameters:**
- `bookingId` (integer): From Step 4
- `passengers` (array): One entry per seat

**Passenger Details Required:**
- `seatNumber` (string): Must match booked seat
- `firstName` (string): Passenger's first name
- `lastName` (string): Passenger's last name
- `email` (string): Valid email address
- `phoneNumber` (string): Contact number
- `idType` (string): Passport, Aadhaar, DrivingLicense, etc.
- `idNumber` (string): ID number
- `age` (integer): Age in years
- `specialRequirements` (string): Wheelchair, dietary, etc. (optional)

**Response:**
```json
{
  "success": true,
  "message": "Passengers added successfully",
  "data": [
    {
      "passengerId": 1,
      "seatNumber": "A1",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "phoneNumber": "+919876543210",
      "idType": "Passport",
      "idNumber": "A12345678",
      "age": 30,
      "specialRequirements": ""
    },
    {
      "passengerId": 2,
      "seatNumber": "A2",
      "firstName": "Jane",
      "lastName": "Doe",
      "email": "jane@example.com",
      "phoneNumber": "+919876543211",
      "idType": "Aadhaar",
      "idNumber": "123456789012",
      "age": 28,
      "specialRequirements": "Wheelchair accessibility"
    },
    {
      "passengerId": 3,
      "seatNumber": "A3",
      "firstName": "Jack",
      "lastName": "Smith",
      "email": "jack@example.com",
      "phoneNumber": "+919876543212",
      "idType": "DrivingLicense",
      "idNumber": "DL12345",
      "age": 35,
      "specialRequirements": ""
    }
  ]
}
```

**Important:**
- One passenger per seat required
- Seat numbers must match booked seats
- ID details are for travel verification

---

## ? STEP 8: GET BOOKING CONFIRMATION

### View Complete Booking Details

```bash
curl -X GET "http://localhost:5000/api/v1/booking/101" \
  -H "Authorization: Bearer {YOUR_TOKEN}"
```

**Response:**
```json
{
  "success": true,
  "message": "Booking details retrieved successfully",
  "data": {
    "bookingId": 101,
    "scheduleId": 1,
    "numberOfSeats": 3,
    "totalAmount": 1500.00,
    "bookingStatus": "Confirmed",
    "bookingDate": "2025-01-15T10:02:00Z",
    "lastStatusChangeAt": "2025-01-15T10:03:00Z",
    "routeId": 1,
    "source": "New York",
    "destination": "Boston",
    "busId": 1,
    "busNumber": "ABC-123",
    "busType": "AC Sleeper",
    "totalSeats": 40,
    "operatorName": "TravelCo Express",
    "ratingAverage": 4.5,
    "travelDate": "2025-02-01",
    "departureTime": "14:00:00",
    "arrivalTime": "18:00:00",
    "availableSeats": 17
  }
}
```

**Confirmation Details:**
- ? Booking status: `Confirmed`
- ? Payment processed
- ? Seats booked
- ? Passengers registered
- ? Ready to travel!

---

## ?? OPTIONAL: CANCEL BOOKING

### Cancel Booking and Get Refund

```bash
curl -X PUT "http://localhost:5000/api/v1/booking/cancel/101" \
  -H "Authorization: Bearer {YOUR_TOKEN}" \
  -H "Content-Type: application/json"
```

**Response:**
```json
{
  "success": true,
  "message": "Booking cancelled successfully",
  "data": {
    "refundId": 1,
    "bookingId": 101,
    "refundAmount": 1125.00,
    "cancellationFee": 375.00,
    "refundPercentage": 75,
    "status": "Pending",
    "requestedAt": "2025-01-15T10:30:00Z"
  }
}
```

**Refund Calculation (Based on Time):**

| Time Before Departure | Refund % | Fee % | Example |
|-|-|-|-|
| > 48 hours | 100% | 0% | ?1500 refund |
| 24-48 hours | 75% | 25% | ?1125 refund |
| < 24 hours | 50% | 50% | ?750 refund |
| After departure | 0% | 100% | ?0 refund |

---

## ?? Role-Based Access Control

### Admin User Capabilities

**Login as Admin:**
```json
{
  "email": "admin@system.com",
  "password": "Admin@123"
}
```

**Admin Can:**
- ? View all bookings (not just own)
- ? Cancel any user's booking
- ? Approve refunds
- ? View payment details
- ? Access analytics

**Endpoints (Admin Only):**
```
GET /api/v1/booking
  ? Get ALL bookings

POST /api/v1/booking/refund/confirm
  ? Approve refunds

GET /api/v1/booking/{bookingId}
  ? Get any booking
```

---

### Customer User Capabilities

**Login as Customer:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword@123"
}
```

**Customer Can:**
- ? Book own seats
- ? View own bookings
- ? Cancel own booking
- ? Add passenger details
- ? View other users' bookings
- ? Approve refunds

**Endpoints (Customer Only):**
```
GET /api/v1/booking/my
  ? Get own bookings only

POST /api/v1/booking
  ? Create booking

GET /api/v1/booking/{own-bookingId}
  ? View own booking
```

---

### Access Control Matrix

| Endpoint | Public | Customer | Admin | Notes |
|----------|--------|----------|-------|-------|
| POST /auth/login | ? | ? | ? | All users |
| POST /auth/register | ? | ? | ? | All users |
| GET /booking/seats/{id} | ? | ? | ? | Authenticated |
| POST /booking/seats/lock | ? | ? | ? | Authenticated |
| POST /booking/seats/release | ? | ? | ? | Own locks only |
| POST /booking | ? | ? | ? | Authenticated |
| GET /booking/my | ? | ? | ? | Own bookings |
| GET /booking | ? | ? | ? | Admin only |
| GET /booking/{id} | ? | ? | ? | Own or all |
| PUT /booking/cancel/{id} | ? | ? | ? | Own or all |
| POST /booking/payment/initiate | ? | ? | ? | Own booking |
| POST /booking/payment/confirm | ? | ? | ? | Own payment |
| POST /booking/passengers | ? | ? | ? | Own booking |
| POST /booking/refund/confirm | ? | ? | ? | Admin only |

---

## ?? Error Handling

### Common Error Responses

#### 400 Bad Request - Invalid Input

```json
{
  "success": false,
  "message": "At least one seat must be selected.",
  "data": null
}
```

#### 401 Unauthorized - Missing Token

```json
{
  "success": false,
  "message": "Invalid token. Authorization header missing.",
  "data": null
}
```

#### 403 Forbidden - Insufficient Permission

```json
{
  "success": false,
  "message": "Access denied. Admin role required.",
  "data": null
}
```

#### 404 Not Found

```json
{
  "success": false,
  "message": "Booking not found.",
  "data": null
}
```

#### 429 Too Many Requests - Rate Limited

```json
{
  "success": false,
  "message": "Too many requests. Please try again later.",
  "data": null
}
```

#### 500 Server Error

```json
{
  "success": false,
  "message": "An internal server error occurred.",
  "data": null
}
```

---

## ?? Testing Scenarios

### Scenario 1: Complete Successful Booking

**Time Required:** ~5 minutes

```bash
# 1. Login
POST /auth/login
? Get token

# 2. View seats
GET /booking/seats/1
? See available seats

# 3. Lock seats
POST /booking/seats/lock
? A1, A2 locked for 5 min

# 4. Create booking
POST /booking
? Booking #101 created (Pending)

# 5. Initiate payment
POST /booking/payment/initiate
? Payment #1 created (Pending)

# 6. Confirm payment
POST /booking/payment/confirm
? Payment approved (Success)
? Booking status: Confirmed

# 7. Add passengers
POST /booking/passengers
? 2 passengers registered

# 8. Get confirmation
GET /booking/101
? Booking confirmed and ready!
```

---

### Scenario 2: Partial Lock Failure

```bash
# Try to lock 3 seats, 1 already booked
POST /booking/seats/lock
Body: ["A1", "A2", "B1"]

Response:
{
  "success": true,
  "lockedSeatNumbers": ["A1", "A2"],
  "failedSeatNumbers": ["B1 (already booked)"]
}

# Choose different seat
POST /booking/seats/lock
Body: ["B2"]

Response:
{
  "success": true,
  "lockedSeatNumbers": ["B2"],
  "failedSeatNumbers": []
}

# Now book with all available locked seats
POST /booking
Body: ["A1", "A2", "B2"]
? Booking successful!
```

---

### Scenario 3: Lock Expiration

```bash
# Lock seat
POST /booking/seats/lock
Body: ["A1"]
? Lock expires at 10:05

# Wait 5 minutes... do nothing

# Try to book at 10:06
POST /booking
Body: ["A1"]
? Error: "Seat A1 is not locked"

# Lock again
POST /booking/seats/lock
Body: ["A1"]
? Now locked again, expires at 10:11

# Book immediately
POST /booking
Body: ["A1"]
? Booking successful!
```

---

### Scenario 4: Payment Failure and Retry

```bash
# Create booking (Pending)
POST /booking
? Booking #102 created

# Initiate payment
POST /booking/payment/initiate
? Payment #2 created

# First payment attempt fails
POST /booking/payment/confirm
Body: { "isSuccess": false, "failureReason": "Card declined" }
? Payment status: Failed
? Booking status: PaymentFailed

# Retry payment (create new payment)
POST /booking/payment/initiate
? Payment #3 created

# Second payment attempt succeeds
POST /booking/payment/confirm
Body: { "isSuccess": true }
? Payment status: Success
? Booking status: Confirmed
```

---

### Scenario 5: Cancellation with Refund

```bash
# Cancel booking (within 24 hours)
PUT /booking/cancel/101
? Refund initiated
? Refund percentage: 50%
? Refund amount: ?750
? Cancellation fee: ?750

# Admin confirms refund
POST /booking/refund/confirm
Body: { "refundId": 1, "isApproved": true }
? Refund status: Completed
? Money transferred (simulated)

# Seats released back to available
GET /booking/seats/1
? A1, A2 now Available
```

---

## ?? Quick Reference Checklist

### Starting a Booking
- [ ] Login and get token
- [ ] View seat layout (GET /booking/seats/{id})
- [ ] Lock seats (POST /booking/seats/lock) - 5 min timer starts
- [ ] Create booking (POST /booking) within 5 minutes

### Completing a Booking
- [ ] Initiate payment (POST /booking/payment/initiate)
- [ ] Confirm payment (POST /booking/payment/confirm)
- [ ] Add passengers (POST /booking/passengers)
- [ ] Get confirmation (GET /booking/{id})

### Cancelling a Booking
- [ ] Cancel booking (PUT /booking/cancel/{id})
- [ ] Refund calculated automatically
- [ ] Admin approves refund (POST /booking/refund/confirm)
- [ ] Seats released to available
- [ ] Refund processed

---

## ?? Important Notes

### Token Management
- ? Token expires in **30 minutes**
- ? Include in every request: `Authorization: Bearer {token}`
- ? Renew by logging in again if expired

### Seat Locking
- ? Locks expire in **5 minutes**
- ? Only locker can release locks
- ? Partial locks allowed (some succeed, some fail)
- ? Renew by locking again before expiration

### Payment Processing
- ? This is **DUMMY payment** (no real processor)
- ? Payments expire in **15 minutes**
- ? Payment required to confirm booking
- ? Can retry if payment fails

### Refunds
- ? Based on **cancellation policy**
- ? Time-dependent (48h, 24h, 0h rules)
- ? Requires **admin approval**
- ? Seats automatically released

### Rate Limiting
- ? Global limit: **100 requests per minute**
- ? Login limit: **5 attempts per minute**
- ? Check response headers for remaining quota

---

## ?? Related Documentation

- [Seat Selection System](./SEAT_SELECTION_SYSTEM.md)
- [Payment System Guide](./PAYMENT_SYSTEM_GUIDE.md)
- [API Quick Reference](./API_QUICK_REFERENCE.md)
- [Testing Guide](./TESTING_GUIDE.md)
- [Developer Checklist](./DEVELOPER_CHECKLIST.md)

---

**Documentation Version:** 1.0.0  
**Last Updated:** January 2025  
**Status:** Production Ready ?
