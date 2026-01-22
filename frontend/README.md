# Timetable System - React Frontend

This is the React + TypeScript frontend for the Timetable System, built with Vite.

## Getting Started

### Prerequisites

- Node.js (v18 or higher)
- npm or yarn

### Installation

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

### Development

Start the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

### Building for Production

Build the production bundle:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

## Project Structure

```
frontend/
├── src/
│   ├── components/     # React components
│   │   ├── Login.tsx
│   │   ├── Timetable.tsx
│   │   └── EntryForm.tsx
│   ├── context/        # React context providers
│   │   └── AuthContext.tsx
│   ├── services/       # API service layer
│   │   └── api.ts
│   ├── types/          # TypeScript type definitions
│   │   └── index.ts
│   ├── App.tsx         # Main app component
│   └── main.tsx        # Entry point
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

## API Integration

The frontend communicates with the C# API backend running on `http://localhost:5000`. The Vite dev server is configured to proxy API requests to the backend.

## Features

- User authentication (Teacher/Student)
- Calendar view of timetable entries
- Create, edit, and delete entries (Teachers only)
- Month navigation
- Responsive design with Tailwind CSS
