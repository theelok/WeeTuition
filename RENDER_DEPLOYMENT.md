# Render Deployment Guide for WeeTuition

## Domain Configuration
- **Production URL**: https://weetuition.onrender.com
- **Frontend**: https://weetuition.onrender.com
- **Backend API**: https://weetuition.onrender.com/api

## Configuration Files Updated

### Backend (`appsettings.Production.json`)
```json
{
  "FrontendUrl": "https://weetuition.onrender.com",
  "BackendUrl": "https://weetuition.onrender.com"
}
```

### Frontend (`frontend/.env.production`)
```
VITE_API_URL=/api
```

## Render Deployment Setup

### Option 1: Single Service (Recommended)
Deploy both frontend and backend in one Render service:

1. **Build Command**:
   ```bash
   cd frontend && npm install && npm run build
   dotnet publish -c Release -o ./publish
   ```

2. **Start Command**:
   ```bash
   cd publish && dotnet TimetableSystem.dll
   ```

3. **Static Files**: Serve `frontend/dist` as static files
4. **API Proxy**: Configure Render to proxy `/api/*` to the backend

### Option 2: Two Services (Frontend + Backend)

#### Backend Service:
- **Build Command**: `dotnet publish -c Release -o ./publish`
- **Start Command**: `cd publish && dotnet TimetableSystem.dll`
- **Environment**: `ASPNETCORE_ENVIRONMENT=Production`
- **Port**: Render will assign automatically

#### Frontend Service:
- **Build Command**: `cd frontend && npm install && npm run build`
- **Publish Directory**: `frontend/dist`
- **Environment Variable**: `VITE_API_URL=https://your-backend-service.onrender.com/api`

## Environment Variables

### Backend (Render Environment Variables):
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<your-database-connection-string>`

### Frontend (if separate service):
- `VITE_API_URL=/api` (for same domain) or full backend URL

## Important Notes

1. **HTTPS**: Render provides HTTPS automatically
2. **CORS**: Already configured for `https://weetuition.onrender.com`
3. **Session Cookies**: Configured for production with `Secure=true` and `SameSite=None`
4. **Database**: Ensure your PostgreSQL connection string is set in Render environment variables

## Testing After Deployment

1. Visit https://weetuition.onrender.com
2. Test login functionality
3. Verify API calls work (check browser console)
4. Test creating/editing timetable entries

## Troubleshooting

### 401 Unauthorized Errors
- Check CORS configuration matches your domain
- Verify session cookies are being set (check browser DevTools)
- Ensure `ASPNETCORE_ENVIRONMENT=Production` is set

### API Not Found (404)
- Verify API proxy configuration in Render
- Check that `/api` routes are properly configured
- Ensure backend service is running

### CORS Errors
- Verify `FrontendUrl` in `appsettings.Production.json` matches your domain
- Check that `AllowCredentials()` is enabled in CORS policy
