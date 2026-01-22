export interface Student {
  studentId: number;
  studentName: string;
  studentCode: string;
  username: string;
  password: string;
  createdDate: string;
}

export interface ScheduleStatus {
  statusId: number;
  statusName: string;
  colorCode: string;
  description?: string;
}

export interface TimetableEntry {
  entryId: number;
  studentId: number;
  scheduleDate: string;
  startTime: string;
  endTime: string;
  statusId: number;
  subject?: string;
  location?: string;
  notes?: string;
  createdDate: string;
  student?: Student;
  status?: ScheduleStatus;
}

export interface TimetableEntryViewModel {
  entryId: number;
  scheduleDate: string;
  startTime: string;
  endTime: string;
  timeSlot: string;
  studentName: string;
  studentCode: string;
  subjectCode: string;
  subject: string;
  statusName: string;
  colorCode: string;
  location?: string;
  notes?: string;
}

export interface DayViewModel {
  date: string;
  dayOfMonth: number;
  dayOfWeek: string;
  isCurrentMonth: boolean;
  entries: TimetableEntryViewModel[];
}

export interface WeekViewModel {
  days: DayViewModel[];
}

export interface TimetableViewModel {
  year: number;
  month: number;
  monthName: string;
  weeks: WeekViewModel[];
  statusLegend: ScheduleStatus[];
  isTeacher: boolean;
  currentStudentId?: number;
}

export interface EditEntryViewModel {
  entryId?: number;
  studentId: number;
  scheduleDate: string;
  startTime: string;
  endTime: string;
  statusId: number;
  subject?: string;
  location?: string;
  notes?: string;
  students?: Student[];
  statuses?: ScheduleStatus[];
}

export interface LoginViewModel {
  username: string;
  password: string;
}

export interface User {
  isTeacher: boolean;
  username: string;
  studentId?: number;
}
