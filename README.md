# Timetable System

A modern timetable management system built with React (TypeScript + Vite) frontend and C# ASP.NET Core Web API backend.

## Architecture

- **Frontend**: React 18 + TypeScript + Vite
- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL (via Entity Framework Core)

## Features

- User authentication (Teacher/Student roles)
- Calendar view of timetable entries
- Create, edit, and delete entries (Teachers only)
- Month navigation
- Beautiful, responsive UI with Tailwind CSS

## Project Structure

```
WeeTuition/
├── Controllers/          # API Controllers
│   ├── AccountController.cs
│   └── TimetableController.cs
├── Models/               # Data models and DbContext
│   ├── AppDbContext.cs
│   ├── TimetableViewModel.cs
│   └── EditEntryViewModel.cs
├── frontend/             # React frontend application
│   ├── src/
│   │   ├── components/   # React components
│   │   ├── context/      # React context providers
│   │   ├── services/     # API service layer
│   │   └── types/        # TypeScript types
│   └── package.json
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js (v18 or higher)
- PostgreSQL database

### Backend Setup

1. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-postgresql-connection-string"
  }
}
```

2. Run the backend:
```bash
dotnet run
```

The API will be available at `http://localhost:5000` (or the port configured in `launchSettings.json`).

### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

### Default Login Credentials

- **Teacher**: 
  - Username: `teacher`
  - Password: `teacher123`

- **Student**: Use credentials from your database

## API Endpoints

### Authentication
- `POST /api/account/login` - Login
- `POST /api/account/logout` - Logout
- `GET /api/account/current` - Get current user

### Timetable
- `GET /api/timetable?year={year}&month={month}` - Get timetable
- `GET /api/timetable/{id}` - Get entry by ID
- `POST /api/timetable` - Create entry
- `PUT /api/timetable/{id}` - Update entry
- `DELETE /api/timetable/{id}` - Delete entry
- `GET /api/timetable/students` - Get all students
- `GET /api/timetable/statuses` - Get all statuses

## Development

### Building for Production

**Frontend:**
```bash
cd frontend
npm run build
```

**Backend:**
```bash
dotnet publish -c Release
```

## Technologies Used

- **Frontend**: React, TypeScript, Vite, Tailwind CSS, Axios, React Router
- **Backend**: ASP.NET Core 8.0, Entity Framework Core, PostgreSQL
- **Authentication**: Session-based authentication

## Notes

- The frontend uses Vite's proxy to forward API requests to the backend during development
- CORS is configured to allow requests from `http://localhost:5173` and `http://localhost:3000`
- Session cookies are configured for cross-origin requests
