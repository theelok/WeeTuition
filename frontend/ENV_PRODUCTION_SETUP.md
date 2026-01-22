# Production Environment Setup

## Create `.env.production` file

Create a file named `.env.production` in the `frontend` directory with the following content:

```
VITE_API_URL=/api
```

This tells the frontend to use `/api` for API calls since both frontend and backend are on the same domain (https://weetuition.onrender.com).

## For Render Deployment

If you're deploying frontend and backend separately, use:

```
VITE_API_URL=https://your-backend-service.onrender.com/api
```

But since you're using https://weetuition.onrender.com, use `/api` as shown above.
