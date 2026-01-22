# Session Cookie Debugging Guide

## The Problem
You're getting 401 Unauthorized errors because the session cookie isn't being set or sent properly.

## Steps to Debug

### 1. Check if Cookie is Set After Login
1. Open browser DevTools (F12)
2. Go to Application/Storage tab → Cookies
3. Look for `http://localhost:5173` or `http://localhost:5089`
4. After logging in, check if `.TimetableSystem.Session` cookie exists

### 2. Test Session Endpoint
Visit: `http://localhost:5173/api/account/testsession`

This will show:
- If session exists
- Session ID
- Current session values
- Cookie header received

### 3. Check Network Tab
1. Open DevTools → Network tab
2. Try to login
3. Check the login response - look for `Set-Cookie` header
4. Check subsequent API requests - look for `Cookie` header in request

### 4. Common Issues and Solutions

#### Issue: Cookie not being set
**Solution**: The backend might be setting the cookie, but the proxy isn't forwarding it correctly.

#### Issue: Cookie domain mismatch
**Solution**: Make sure cookie domain is `localhost` (not `localhost:5089`)

#### Issue: SameSite policy blocking
**Solution**: We're using `SameSite=Lax` which should work for same-site requests

## Quick Fix to Try

1. **Clear all cookies** for localhost in your browser
2. **Restart both servers**:
   - Backend: `dotnet run`
   - Frontend: `cd frontend && npm run dev`
3. **Try logging in again**
4. **Check cookies** in DevTools after login

## Alternative: Use JWT Tokens Instead
If session cookies continue to be problematic, we can switch to JWT token-based authentication which is more reliable for API-based architectures.
