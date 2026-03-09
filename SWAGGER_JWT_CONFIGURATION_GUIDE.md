# Swagger JWT Authorization Configuration - Fixed ✅

## Problem
The Swagger UI in the API was not showing the "Authorize" button to authenticate users with JWT Bearer tokens. This prevented API testing with authentication from the Swagger interface.

## Solution
Added JWT Bearer security scheme configuration to the Swagger/OpenAPI setup in `Program.cs`.

### Changes Made

#### 1. **Updated Program.cs** - Added Swagger Security Configuration
```csharp
using Microsoft.OpenApi.Models;

// In service registration section:
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\nExample: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
```

#### 2. **Updated BusTicketingSystem.csproj** - Added Missing NuGet Package
Added the `Microsoft.OpenApi` package (v2.4.1) which provides the OpenAPI model types:
```xml
<PackageReference Include="Microsoft.OpenApi" Version="2.4.1" />
```

#### 3. **Updated SeedData.cs** - Fixed Namespace
Changed namespace from `BusMateApp.Data` to `BusTicketingSystem.Data` to match the project structure.

## How to Use

### In Swagger UI:
1. Navigate to `https://localhost:5001/swagger` (or your API URL + `/swagger`)
2. Click the **"Authorize"** button (now visible in top-right corner)
3. Enter your JWT token in the format: `Bearer {token}`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
4. Click **"Authorize"** to apply the token to all requests
5. Test any protected endpoint - the token will be automatically included in the `Authorization` header

### Getting a JWT Token:
1. Call the `/api/auth/login` endpoint with admin/customer credentials:
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'
```

2. Extract the `token` value from the response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresIn": 3600
}
```

3. Use this token in the Swagger Authorize dialog

## What Was Fixed

✅ **Added Security Scheme Definition** - Tells Swagger about JWT Bearer authentication  
✅ **Added Security Requirement** - Makes authentication required for endpoints with `[Authorize]`  
✅ **Added Missing NuGet Package** - `Microsoft.OpenApi` provides OpenAPI model types  
✅ **Updated SeedData Namespace** - Fixed namespace mismatch

## Technical Details

### OpenAPI Security Configuration Breakdown:

| Component | Purpose |
|-----------|---------|
| `AddSecurityDefinition` | Defines the JWT Bearer scheme for Swagger |
| `SecuritySchemeType.Http` | Specifies HTTP-based authentication |
| `Scheme = "bearer"` | Uses HTTP Bearer authentication |
| `BearerFormat = "JWT"` | Indicates JWT format for better UX |
| `ParameterLocation.Header` | Token goes in Authorization header |
| `AddSecurityRequirement` | Makes auth required by default |

### Benefits:

1. **Better Developer Experience** - Can test APIs directly in Swagger
2. **Clear Documentation** - Shows which endpoints require authentication
3. **Quick Testing** - No need to manually add auth headers with cURL
4. **Standards Compliant** - Follows OpenAPI 3.0 specification

## Testing the Fix

### Before (Without Authorization Button):
- Swagger UI had no way to provide JWT token
- Only non-protected endpoints were testable

### After (With Authorization Button):
- Click "Authorize" button in Swagger UI
- Paste your JWT token
- All protected endpoints can be tested with authentication
- Token automatically included in all requests

## Build Status

✅ **Build Successful**
- 0 Compilation Errors
- 0 Warnings (except Edit-and-Continue namespace warning, requires app restart)

## Related Files Modified

1. `BusTicketingSystem/Program.cs` - Swagger configuration
2. `BusTicketingSystem/BusTicketingSystem.csproj` - NuGet package
3. `BusTicketingSystem/Data/SeedData.cs` - Namespace correction

## API Endpoints Protected by JWT

All endpoints with `[Authorize]` attribute now show in Swagger as requiring authentication:
- Bus Management endpoints
- Route Management endpoints
- Schedule Management endpoints
- Booking Management endpoints
- Admin-specific operations

## References

- [OpenAPI Security Schemes](https://spec.openapis.org/oas/v3.0.3#security-scheme-object)
- [Swashbuckle JWT Configuration](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Microsoft.OpenApi NuGet Package](https://www.nuget.org/packages/Microsoft.OpenApi/)

---

**Status**: ✅ **COMPLETE**  
**Date**: March 6, 2025  
**Impact**: Swagger UI now fully functional for testing protected endpoints  
**Breaking Changes**: None
