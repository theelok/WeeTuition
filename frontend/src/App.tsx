import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Login from './components/Login';
import Timetable from './components/Timetable';
import EntryForm from './components/EntryForm';

const PrivateRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return <div className="flex items-center justify-center min-h-screen">Loading...</div>;
  }

  return user ? <>{children}</> : <Navigate to="/login" />;
};

const AppRoutes: React.FC = () => {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        path="/timetable"
        element={
          <PrivateRoute>
            <Timetable />
          </PrivateRoute>
        }
      />
      <Route
        path="/timetable/create"
        element={
          <PrivateRoute>
            <EntryForm />
          </PrivateRoute>
        }
      />
      <Route
        path="/timetable/edit/:id"
        element={
          <PrivateRoute>
            <EntryForm />
          </PrivateRoute>
        }
      />
      <Route path="/" element={<Navigate to="/timetable" />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;
