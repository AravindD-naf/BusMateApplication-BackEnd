# 🔧 Swagger JWT Authorization - Setup & Fix Guide

## ✅ **What Was Fixed**

Added JWT Bearer token authentication support to Swagger UI. Now you can:
1. ✅ Click **"Authorize"** button in Swagger
2. ✅ Paste your JWT token
3. ✅ Test all protected API endpoints directly from Swagger
4. ✅ Token automatically included in all requests

---

## 📋 **Files Modified**

### 1. **BusTicketingSystem/Program.cs**
- Added `using Microsoft.OpenApi.Models;`
- Added Swagger security scheme configuration
- Enables JWT Bearer token support in Swagger UI

### 2. **BusTicketingSystem/BusTicketingSystem.csproj**
- Added NuGet package: `Microsoft.OpenApi` (v2.4.1)
- This package provides the OpenAPI model types needed for Swagger configuration

### 3. **BusTicketingSystem/Data/SeedData.cs**
- Fixed namespace: `BusMateApp.Data` → `BusTicketingSystem.Data`
- Added template for seed data initialization

---

## 🚀 **How to Complete the Fix**

### **Option 1: Restart Application (Recommended)**

1. **Stop the current application**
   - Press `Ctrl+C` in the terminal/PowerShell
   - Or click the Stop button in Visual Studio

2. **Clean and rebuild**
   ```bash
   cd BusTicketingSystem
   dotnet clean
   dotnet restore
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open Swagger**
   - Navigate to: `https://localhost:5001/swagger`
   - Look for the green **"Authorize"** button in the top-right corner ✅

### **Option 2: Using Visual Studio**

1. **Stop debugging** (Shift+F5)
2. **Clean Solution** (Build → Clean Solution)
3. **Build Solution** (Ctrl+Shift+B)
4. **Run project** (F5)
5. **Check Swagger** - "Authorize" button should now be visible

### **Option 3: Hot Reload (May Not Work)**

If you're using hot reload:
1. Stop the app
2. Make sure you rebuild completely
3. Start again

---

## 🔑 **Using JWT Authorization in Swagger**

### **Step 1: Get Authentication Token**

1. In Swagger, find the **Auth** section
2. Look for: `POST /api/auth/login`
3. Click **"Try it out"**
4. Enter credentials:
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
5. Click **"Execute"**
6. Copy the `token` value from the response

### **Step 2: Authorize in Swagger**

1. Click the green **"Authorize"** button (top-right corner)
2. In the **Authorization** field, paste your token:
   ```
   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjMiLCJuYW1lIjoiYWRtaW4i...
   ```
3. Click **"Authorize"**
4. Click **"Close"**
5. You'll see a **lock icon** 🔒 - you're now authorized!

### **Step 3: Test Protected Endpoints**

Now test any endpoint with `[Authorize]` attribute:

- `POST /buses/get-all` ✅
- `POST /routes/get-all` ✅
- `POST /schedules/get-all` ✅
- `POST /bookings/create` ✅
- `POST /auditlogs/get-all` ✅

The token is automatically added to all requests!

---

## 📊 **What the Code Does**

```csharp
// This tells Swagger about JWT Bearer authentication
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,          // HTTP authentication
    Scheme = "bearer",                        // Bearer tokens
    BearerFormat = "JWT",                     // JWT format
    In = ParameterLocation.Header,            // Token goes in header
    Description = "JWT Authorization..."
});

// This makes authentication required by default
c.AddSecurityRequirement(new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }
});
```

**Result**: Swagger shows "Authorize" button automatically! 🎉

---

## ✅ **Verification Checklist**

After restarting, verify:

- [ ] Application starts without errors
- [ ] Navigate to `https://localhost:5001/swagger`
- [ ] Green **"Authorize"** button is visible (top-right)
- [ ] Can click "Authorize" button
- [ ] Authorization dialog opens and has a text field
- [ ] Can paste JWT token successfully
- [ ] Protected endpoints show lock icon 🔒
- [ ] Can execute protected endpoints with token

---

## 🆘 **Troubleshooting**

### Problem: "Authorize" Button Not Visible

**Solution 1: Restart Application**
```bash
# Stop current process
Ctrl+C

# Clean, rebuild, run
dotnet clean
dotnet build
dotnet run
```

**Solution 2: Clear Browser Cache**
```bash
# In browser:
- Press Ctrl+Shift+Delete
- Clear Cache
- Reload Swagger page
```

**Solution 3: Check if NuGet Package Installed**
```bash
# Verify Microsoft.OpenApi is installed
dotnet list package
```
Should show: `Microsoft.OpenApi` version `2.4.1`

### Problem: "401 Unauthorized" Error

**Solution**: Token expired or invalid
```bash
1. Get new token by logging in again (POST /api/auth/login)
2. Click "Authorize" again
3. Paste the new token
4. Click "Authorize"
```

### Problem: "403 Forbidden" Error

**Solution**: User doesn't have permission
```bash
- Use an admin account for testing
- Or check endpoint authorization requirements
```

### Problem: Build Still Fails After Changes

**Solution: Full Clean & Rebuild**
```bash
# Delete temporary files
Remove-Item -Recurse bin/
Remove-Item -Recurse obj/

# Restore and build fresh
dotnet restore
dotnet build
```

---

## 📝 **Example Request with Swagger**

### Before (Without Authorization):
❌ Cannot test protected endpoints  
❌ No way to send token  
❌ "401 Unauthorized" errors

### After (With Authorization):
✅ Click "Authorize" button  
✅ Paste your JWT token  
✅ Test all protected endpoints  
✅ Token auto-included in all requests  

---

## 🔐 **Security Notes**

1. **Token Format**
   - ✅ Correct: Just the token
   - ❌ Wrong: "Bearer token..." (Swagger adds Bearer automatically)

2. **Token Expiration**
   - Default: 1 hour (3600 seconds)
   - When expired: Get new token from login endpoint

3. **HTTPS Required**
   - Development: https://localhost:5001 (OK)
   - Production: Must use HTTPS in production

4. **Don't Share Tokens**
   - Tokens are like passwords
   - Never commit to GitHub
   - Never expose in logs

---

## 📚 **Additional Resources**

- **API Documentation**: See `README.md`
- **API Examples**: See `API_USAGE_EXAMPLES.md`
- **Testing Guide**: See `TESTING_GUIDE.md`
- **Quick Start**: See `QUICK_START.md`

---

## 🎯 **Next Steps**

1. ✅ Apply the code changes (already done)
2. ✅ Restart the application
3. ✅ Open Swagger UI
4. ✅ Test the "Authorize" button
5. ✅ Get JWT token from login endpoint
6. ✅ Test protected endpoints

---

## 📞 **Support**

If issues persist:

1. Check application console for error messages
2. Verify all files were updated correctly
3. Make sure NuGet package restore ran successfully
4. Try clearing Visual Studio cache and rebuilding
5. Check that .NET SDK is up to date: `dotnet --version`

---

**Status**: ✅ Ready to Use  
**Date**: March 6, 2025  
**Compatibility**: .NET 10, Swagger 10.1.4  
**Changes Required**: Restart application
