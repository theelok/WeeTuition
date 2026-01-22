# Quick Production Setup Guide

## Step 1: Update Your Domain Configuration

### Backend - Update `appsettings.Production.json`:

Replace `yourdomain.com` with your actual domain:

```json
{
  "FrontendUrl": "https://yourdomain.com",
  "BackendUrl": "https://api.yourdomain.com"
}
```

**Example:**
- If your domain is `timetable.example.com`, use:
  ```json
  {
    "FrontendUrl": "https://timetable.example.com",
    "BackendUrl": "https://api.timetable.example.com"
  }
  ```

### Frontend - Create `frontend/.env.production`:

Create this file and add your API URL:

**Option A: Same Domain (Recommended)**
```
VITE_API_URL=/api
```

**Option B: Separate API Domain**
```
VITE_API_URL=https://api.yourdomain.com/api
```

## Step 2: What Changed for Production

### Backend (`Program.cs`):
- ✅ Automatically detects production environment
- ✅ Uses HTTPS cookies (`Secure=true`, `SameSite=None`)
- ✅ CORS configured from `appsettings.Production.json`

### Frontend (`src/services/api.ts`):
- ✅ Uses environment variable `VITE_API_URL`
- ✅ Falls back to `/api` if not set (for same-domain setup)

## Step 3: Build and Deploy

### Build Frontend:
```bash
cd frontend
npm install
npm run build
```

### Deploy Backend:
```bash
dotnet publish -c Release
```

## Step 4: Push to GitHub

```bash
git add .
git commit -m "Configure for production with domain support"
git push origin main
```

## Important Notes:

1. **HTTPS Required**: Production uses secure cookies, so HTTPS is mandatory
2. **CORS**: Your frontend domain must match `FrontendUrl` in `appsettings.Production.json`
3. **Environment**: Set `ASPNETCORE_ENVIRONMENT=Production` on your server

## Need Help?

See `DEPLOYMENT.md` for detailed deployment instructions.
