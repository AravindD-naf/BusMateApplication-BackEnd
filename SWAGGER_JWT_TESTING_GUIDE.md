# Quick Guide: Testing APIs with Swagger JWT Authorization ✅

## Step-by-Step Guide

### 1. **Start the Application**
```bash
dotnet run
```

### 2. **Open Swagger UI**
Navigate to:
- **Local**: `https://localhost:5001/swagger`
- **Or**: `http://localhost:5000/swagger`

### 3. **Get JWT Token** (Step 1 - Login)

Find the **Auth** endpoints section and locate: `POST /api/auth/login`

Click **"Try it out"** and enter credentials:
```json
{
  "username": "admin",
  "password": "your_password"
}
```

Click **"Execute"** and copy the `token` value from the response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI...",
  "refreshToken": "...",
  "expiresIn": 3600
}
```

### 4. **Add Token to Swagger** (Step 2 - Authorize)

1. Click the green **"Authorize"** button in the top-right corner
2. In the **Authorization** field, paste your token:
   ```
   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI...
   ```
   ⚠️ **Important**: Paste ONLY the token, NOT "Bearer" prefix (Swagger adds it automatically)

3. Click **"Authorize"** button
4. Click **"Close"**

You'll see a **lock icon** 🔒 next to your username in the top-right, indicating you're authenticated.

### 5. **Test Protected Endpoints** (Step 3 - Test)

Now you can test any protected endpoint:

**Example: Get All Buses**
- Find: `POST /buses/get-all` in the Buses section
- Click **"Try it out"**
- Enter pagination details:
  ```json
  {
    "pageNumber": 1,
    "pageSize": 10
  }
  ```
- Click **"Execute"**
- The token is automatically included in the `Authorization` header

### 6. **Testing Different Endpoints**

#### 📦 Bus Management
- `POST /buses/create` - Create new bus
- `POST /buses/get-all` - List all buses
- `POST /buses/search-by-operator` - Search buses

#### 🛣️ Route Management  
- `POST /routes/create` - Create route
- `POST /routes/get-all` - List routes
- `POST /routes/search-by-source` - Search by source city

#### 📅 Schedule Management
- `POST /schedules/create` - Create schedule
- `POST /schedules/get-all` - List schedules
- `POST /schedules/search-by-city` - Search by city

#### 🎫 Booking Management
- `POST /bookings/create` - Create booking
- `POST /bookings/user-bookings` - Get user bookings

#### 📊 Audit & Logs
- `POST /auditlogs/get-all` - View audit logs

## Token Formats

### ✅ **Correct**: Just the token
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI...
```

### ❌ **Incorrect**: With "Bearer" prefix
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI...
```
(Swagger automatically adds "Bearer " prefix)

## Troubleshooting

### "401 Unauthorized" Errors
- **Cause**: Token expired or invalid
- **Solution**: Get a new token by logging in again (step 3)

### "403 Forbidden" Errors
- **Cause**: Your user role doesn't have permission for this endpoint
- **Solution**: Use admin account or check endpoint permissions

### Authorize Button Not Visible
- **Cause**: Swagger configuration not applied (old code)
- **Solution**: Rebuild project:
  ```bash
  dotnet clean
  dotnet build
  dotnet run
  ```

### Token Not Being Sent
- **Cause**: Not authorized in Swagger or token field empty
- **Solution**: 
  1. Click "Authorize" button again
  2. Verify token is in the field
  3. Click "Authorize" to confirm

## Testing Without Swagger (cURL)

If you prefer command-line testing:

### 1. Get Token
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "your_password"
  }' \
  --insecure
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

### 2. Test Protected Endpoint
```bash
curl -X POST "https://localhost:5001/api/buses/get-all" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 10
  }' \
  --insecure
```

## Common API Response Examples

### ✅ Success (200 OK)
```json
{
  "success": true,
  "message": "Operation successful",
  "data": [
    {
      "busId": 1,
      "busNumber": "BUS001",
      "totalSeats": 40,
      "isActive": true
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1
}
```

### ❌ Unauthorized (401)
```json
{
  "success": false,
  "message": "Unauthorized: Invalid or expired token"
}
```

### ❌ Validation Error (400)
```json
{
  "success": false,
  "message": "Total seats must be between 1 and 40"
}
```

### ❌ Not Found (404)
```json
{
  "success": false,
  "message": "Bus not found with the provided ID"
}
```

## Token Expiration

- Default token expiration: **1 hour** (3600 seconds)
- When expired: Get a new token by logging in again
- Or use the refresh token endpoint: `POST /api/auth/refresh-token`

## Security Notes

1. **Keep tokens secure** - Don't share or expose in logs
2. **HTTPS only** - Always use HTTPS (not HTTP) in production
3. **Secure storage** - Never store tokens in plain text
4. **Token rotation** - Refresh tokens periodically
5. **Logout** - Call logout endpoint to invalidate tokens

## Help & Support

If you encounter issues:

1. Check the **Response** tab in Swagger for error details
2. Review API logs in the application console
3. Verify token is valid and not expired
4. Ensure your user has required permissions
5. Check CORS settings if calling from different origin

---

**Last Updated**: March 6, 2025  
**Status**: ✅ Working  
**Compatibility**: .NET 10, Swagger UI 10.1.4
