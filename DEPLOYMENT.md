# Deployment Guide

## Prerequisites
- Your domain (e.g., `yourdomain.com`)
- Backend API domain (e.g., `api.yourdomain.com` or same domain)
- SSL certificates (HTTPS required for production)

## Step 1: Update Configuration Files

### Backend Configuration (`appsettings.Production.json`)

Update with your actual domains:

```json
{
  "FrontendUrl": "https://yourdomain.com",
  "BackendUrl": "https://api.yourdomain.com"
}
```

### Frontend Configuration (`frontend/.env.production`)

Update with your actual API URL:

```
VITE_API_URL=https://api.yourdomain.com/api
```

Or if backend and frontend are on the same domain:

```
VITE_API_URL=/api
```

## Step 2: Update CORS in Program.cs

The CORS configuration will automatically use your production domain from `appsettings.Production.json`.

## Step 3: Build Frontend

```bash
cd frontend
npm install
npm run build
```

This creates a `dist` folder with production-ready files.

## Step 4: Deployment Options

### Option A: Same Domain (Recommended)
- Frontend: `https://yourdomain.com`
- Backend API: `https://yourdomain.com/api`

**Setup:**
1. Serve frontend static files from root
2. Serve backend API from `/api` path
3. Update `.env.production`: `VITE_API_URL=/api`

### Option B: Separate Domains
- Frontend: `https://yourdomain.com`
- Backend API: `https://api.yourdomain.com`

**Setup:**
1. Deploy frontend to `yourdomain.com`
2. Deploy backend to `api.yourdomain.com`
3. Update `.env.production`: `VITE_API_URL=https://api.yourdomain.com/api`
4. Update `appsettings.Production.json`: `"FrontendUrl": "https://yourdomain.com"`

## Step 5: Backend Deployment

### Using .NET
```bash
dotnet publish -c Release -o ./publish
```

### Using Docker
```bash
docker build -t timetable-backend .
docker run -p 8080:8080 timetable-backend
```

## Step 6: Frontend Deployment

### Copy built files to your web server:
```bash
# Copy the dist folder contents to your web server
cp -r frontend/dist/* /path/to/web/server/
```

### Nginx Configuration Example:

```nginx
server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com;

    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;

    # Frontend static files
    root /var/www/timetable-frontend;
    index index.html;

    # API proxy (if same domain)
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    # Frontend routes (SPA)
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

## Step 7: Environment Variables

Make sure to set production environment:
- Backend: `ASPNETCORE_ENVIRONMENT=Production`
- Frontend: Uses `.env.production` automatically during build

## Step 8: Verify Deployment

1. Visit `https://yourdomain.com`
2. Check browser console for errors
3. Test login functionality
4. Verify API calls are working

## Important Notes

1. **HTTPS is Required**: Production uses `SecurePolicy.Always` for cookies
2. **CORS**: Make sure your frontend domain is in the CORS allowed origins
3. **Session Cookies**: Will work with `SameSite=None` and `Secure=true` in production
4. **Database**: Ensure your production database connection string is correct

## Troubleshooting

### 401 Unauthorized Errors
- Check CORS configuration matches your frontend domain
- Verify session cookies are being set (check browser DevTools)
- Ensure HTTPS is enabled (required for Secure cookies)

### CORS Errors
- Verify `FrontendUrl` in `appsettings.Production.json` matches your actual domain
- Check that `AllowCredentials()` is enabled in CORS policy

### Cookie Issues
- Ensure both frontend and backend use HTTPS
- Check cookie `SameSite` and `Secure` settings match your setup
