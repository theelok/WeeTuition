import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { timetableApi } from '../services/api';
import type { EditEntryViewModel, Student, ScheduleStatus } from '../types';

const EntryForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const isEdit = !!id;

  // Helper to format date for input field (avoid timezone issues)
  const formatDateForInput = (dateString: string | null): string => {
    if (!dateString) return new Date().toISOString().split('T')[0];
    try {
      // If it's already in YYYY-MM-DD format, return as is
      if (/^\d{4}-\d{2}-\d{2}$/.test(dateString)) {
        return dateString;
      }
      // If it's an ISO date string, extract just the date part before any timezone conversion
      // Extract YYYY-MM-DD from ISO string (e.g., "2026-01-02T00:00:00Z" -> "2026-01-02")
      const dateMatch = dateString.match(/^(\d{4}-\d{2}-\d{2})/);
      if (dateMatch) {
        return dateMatch[1];
      }
      // Fallback: parse as date and use UTC methods to avoid timezone shift
      const date = new Date(dateString);
      const year = date.getUTCFullYear();
      const month = String(date.getUTCMonth() + 1).padStart(2, '0');
      const day = String(date.getUTCDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    } catch {
      return new Date().toISOString().split('T')[0];
    }
  };

  const [formData, setFormData] = useState<EditEntryViewModel>({
    studentId: 0,
    scheduleDate: formatDateForInput(searchParams.get('date')),
    startTime: '09:00',
    endTime: '10:00',
    statusId: 1,
    subject: '',
    location: '',
    notes: '',
  });

  const [students, setStudents] = useState<Student[]>([]);
  const [statuses, setStatuses] = useState<ScheduleStatus[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user?.isTeacher) {
      navigate('/timetable');
      return;
    }

    loadData();
  }, [id, user]);

  const loadData = async () => {
    try {
      const [studentsData, statusesData] = await Promise.all([
        timetableApi.getStudents(),
        timetableApi.getStatuses(),
      ]);
      setStudents(studentsData);
      setStatuses(statusesData);

      if (isEdit && id) {
        const entry = await timetableApi.getEntry(parseInt(id));
        // Format date to yyyy-MM-dd for date input (avoid timezone issues)
        const scheduleDate = formatDateForInput(entry.scheduleDate);
        setFormData({
          ...entry,
          scheduleDate: scheduleDate,
          startTime: entry.startTime,
          endTime: entry.endTime,
        });
      } else if (statusesData.length > 0) {
        setFormData((prev) => ({ ...prev, statusId: statusesData[0].statusId }));
      }
    } catch (err) {
      console.error('Failed to load data:', err);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Create payload without students/statuses arrays (only needed for form display)
      const payload: EditEntryViewModel = {
        studentId: formData.studentId,
        scheduleDate: formData.scheduleDate,
        startTime: formData.startTime,
        endTime: formData.endTime,
        statusId: formData.statusId,
        subject: formData.subject || '',
        location: formData.location || '',
        notes: formData.notes || '',
      };

      if (isEdit && id) {
        payload.entryId = formData.entryId;
        await timetableApi.updateEntry(parseInt(id), payload);
      } else {
        await timetableApi.createEntry(payload);
      }
      const date = new Date(formData.scheduleDate);
      navigate(`/timetable?year=${date.getFullYear()}&month=${date.getMonth() + 1}`);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save entry');
    } finally {
      setLoading(false);
    }
  };

  if (!user?.isTeacher) {
    return null;
  }

  return (
    <div className="min-h-screen p-4" style={{
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%)',
    }}>
      <div className="max-w-2xl mx-auto">
        <div className="glass-morphism rounded-3xl shadow-2xl p-8 mt-8">
          <style>{`
            .glass-morphism {
              background: rgba(255, 255, 255, 0.95);
              backdrop-filter: blur(10px);
              border: 1px solid rgba(255, 255, 255, 0.3);
            }
          `}</style>

          <h2 className="text-3xl font-bold bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600 bg-clip-text text-transparent mb-6">
            {isEdit ? '✨ Edit Entry' : '✨ Create New Entry'}
          </h2>

          {error && (
            <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded-lg mb-6">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="block font-bold text-purple-800 mb-2">Student</label>
              <select
                value={formData.studentId}
                onChange={(e) => setFormData({ ...formData, studentId: parseInt(e.target.value) })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                required
              >
                <option value="">Select a student</option>
                {students.map((student) => (
                  <option key={student.studentId} value={student.studentId}>
                    {student.studentCode} - {student.studentName}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block font-bold text-purple-800 mb-2">Date</label>
              <input
                type="date"
                value={formData.scheduleDate}
                onChange={(e) => setFormData({ ...formData, scheduleDate: e.target.value })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block font-bold text-purple-800 mb-2">Start Time</label>
                <input
                  type="time"
                  value={formData.startTime}
                  onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                  className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                  required
                />
              </div>
              <div>
                <label className="block font-bold text-purple-800 mb-2">End Time</label>
                <input
                  type="time"
                  value={formData.endTime}
                  onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                  className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                  required
                />
              </div>
            </div>

            <div>
              <label className="block font-bold text-purple-800 mb-2">Status</label>
              <select
                value={formData.statusId}
                onChange={(e) => setFormData({ ...formData, statusId: parseInt(e.target.value) })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                required
              >
                {statuses.map((status) => (
                  <option key={status.statusId} value={status.statusId}>
                    {status.statusName}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block font-bold text-purple-800 mb-2">Subject</label>
              <input
                type="text"
                value={formData.subject || ''}
                onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
              />
            </div>

            <div>
              <label className="block font-bold text-purple-800 mb-2">Location</label>
              <input
                type="text"
                value={formData.location || ''}
                onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
              />
            </div>

            <div>
              <label className="block font-bold text-purple-800 mb-2">Notes</label>
              <textarea
                value={formData.notes || ''}
                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500"
                rows={4}
              />
            </div>

            <div className="flex gap-4">
              <button
                type="submit"
                disabled={loading}
                className="flex-1 bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600 text-white font-bold py-4 px-6 rounded-full text-lg shadow-xl hover:shadow-2xl transition-all duration-300 disabled:opacity-50"
              >
                {loading ? 'Saving...' : isEdit ? 'Update Entry' : 'Create Entry'}
              </button>
              <button
                type="button"
                onClick={() => navigate('/timetable')}
                className="px-6 py-4 bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold rounded-full transition-all duration-300"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default EntryForm;
