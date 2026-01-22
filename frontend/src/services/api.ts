import axios from 'axios';
import type { 
  LoginViewModel, 
  User, 
  TimetableViewModel, 
  EditEntryViewModel,
  Student,
  ScheduleStatus
} from '../types';

// Use environment variable for API URL, fallback to /api for development
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authApi = {
  login: async (credentials: LoginViewModel): Promise<User> => {
    const response = await api.post<{ success: boolean; isTeacher: boolean; username: string; studentId?: number }>('/account/login', credentials);
    return {
      isTeacher: response.data.isTeacher,
      username: response.data.username,
      studentId: response.data.studentId,
    };
  },

  logout: async (): Promise<void> => {
    await api.post('/account/logout');
  },

  getCurrentUser: async (): Promise<User> => {
    const response = await api.get<{ isTeacher: boolean; username: string; studentId?: number }>('/account/current');
    return {
      isTeacher: response.data.isTeacher,
      username: response.data.username,
      studentId: response.data.studentId,
    };
  },
};

export const timetableApi = {
  getTimetable: async (year?: number, month?: number): Promise<TimetableViewModel> => {
    const params = new URLSearchParams();
    if (year) params.append('year', year.toString());
    if (month) params.append('month', month.toString());
    const response = await api.get<TimetableViewModel>(`/timetable?${params.toString()}`);
    return response.data;
  },

  getEntry: async (id: number): Promise<EditEntryViewModel> => {
    const response = await api.get<EditEntryViewModel>(`/timetable/${id}`);
    return response.data;
  },

  createEntry: async (entry: EditEntryViewModel): Promise<{ success: boolean; message: string; entryId?: number }> => {
    const response = await api.post<{ success: boolean; message: string; entryId?: number }>('/timetable', entry);
    return response.data;
  },

  updateEntry: async (id: number, entry: EditEntryViewModel): Promise<{ success: boolean; message: string }> => {
    const response = await api.put<{ success: boolean; message: string }>(`/timetable/${id}`, entry);
    return response.data;
  },

  deleteEntry: async (id: number): Promise<{ success: boolean; message: string }> => {
    const response = await api.delete<{ success: boolean; message: string }>(`/timetable/${id}`);
    return response.data;
  },

  getStudents: async (): Promise<Student[]> => {
    const response = await api.get<Student[]>('/timetable/students');
    return response.data;
  },

  getStatuses: async (): Promise<ScheduleStatus[]> => {
    const response = await api.get<ScheduleStatus[]>('/timetable/statuses');
    return response.data;
  },
};
