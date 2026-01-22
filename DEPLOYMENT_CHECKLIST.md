# Pre-Deployment Checklist

## Before Pushing to GitHub

### 1. Update Domain Configuration

- [x] Update `appsettings.Production.json`:
  - ✅ Set to `https://weetuition.onrender.com`
  - ✅ Backend URL set to `https://weetuition.onrender.com`

- [ ] Create `frontend/.env.production`:
  - Set `VITE_API_URL=/api` (for same domain setup)
  - See `frontend/ENV_PRODUCTION_SETUP.md` for details

### 2. Security Check

- [ ] Remove or secure database connection string in `appsettings.json`
- [ ] Use environment variables or secure vault for production secrets
- [ ] Ensure `.env` files with secrets are in `.gitignore`

### 3. Build Test

- [ ] Test production build locally:
  ```bash
  cd frontend
  npm run build
  ```

- [ ] Verify `dist` folder is created and contains built files

### 4. Configuration Files

- [ ] Verify `Program.cs` has production CORS settings
- [ ] Verify session cookie settings are correct for HTTPS
- [ ] Check that environment detection works (`IsDevelopment()` vs Production)

### 5. Git Setup

- [ ] Ensure `.gitignore` includes:
  - `frontend/dist/`
  - `frontend/.env.local`
  - `*.env.local`
  - Build artifacts

- [ ] Commit all changes:
  ```bash
  git add .
  git commit -m "Configure for production deployment"
  ```

### 6. Push to GitHub

```bash
git push origin main
```

## After Deployment

- [ ] Test login functionality
- [ ] Verify API calls work
- [ ] Check browser console for errors
- [ ] Verify HTTPS is working
- [ ] Test session persistence
- [ ] Check CORS headers in network tab
