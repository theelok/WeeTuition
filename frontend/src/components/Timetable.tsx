import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { timetableApi } from '../services/api';
import type { TimetableViewModel, TimetableEntryViewModel } from '../types';

const Timetable: React.FC = () => {
  const { user, loading: authLoading, logout } = useAuth();
  const navigate = useNavigate();
  const [timetable, setTimetable] = useState<TimetableViewModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [year, setYear] = useState(new Date().getFullYear());
  const [month, setMonth] = useState(new Date().getMonth() + 1);

  useEffect(() => {
    if (authLoading) {
      return;
    }
    
    if (!user) {
      navigate('/login');
      return;
    }
    
    loadTimetable();
  }, [user, authLoading, year, month]);

  const loadTimetable = async () => {
    try {
      setLoading(true);
      const data = await timetableApi.getTimetable(year, month);
      setTimetable(data);
    } catch (error) {
      console.error('Failed to load timetable:', error);
    } finally {
      setLoading(false);
    }
  };

  const navigateMonth = (direction: number) => {
    let newMonth = month + direction;
    let newYear = year;

    if (newMonth > 12) {
      newMonth = 1;
      newYear++;
    } else if (newMonth < 1) {
      newMonth = 12;
      newYear--;
    }

    setMonth(newMonth);
    setYear(newYear);
  };

  const handleEditEntry = (entryId: number) => {
    if (user?.isTeacher) {
      navigate(`/timetable/edit/${entryId}`);
    } else {
      alert('🔒 Only wizards can edit timetable entries!');
    }
  };

  const handleAddEntry = (date: string) => {
    // Format date to yyyy-MM-dd for the query parameter
    const dateOnly = new Date(date).toISOString().split('T')[0];
    navigate(`/timetable/create?date=${dateOnly}`);
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  if (authLoading || loading || !timetable) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-2xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="p-4 md:p-8" style={{
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%)',
      minHeight: '100vh'
    }}>
      <style>{`
        .glass-morphism {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(10px);
          border: 1px solid rgba(255, 255, 255, 0.3);
        }
        .entry {
          transition: all 0.3s ease;
          position: relative;
          overflow: hidden;
        }
        .entry::before {
          content: '';
          position: absolute;
          top: 0;
          left: -100%;
          width: 100%;
          height: 100%;
          background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
          transition: left 0.5s;
        }
        .entry:hover::before {
          left: 100%;
        }
        .floating {
          animation: float 3s ease-in-out infinite;
        }
        @keyframes float {
          0%, 100% { transform: translateY(0px); }
          50% { transform: translateY(-10px); }
        }
        .sparkle {
          animation: sparkle 2s infinite;
        }
        @keyframes sparkle {
          0%, 100% { opacity: 1; transform: scale(1); }
          50% { opacity: 0.5; transform: scale(1.1); }
        }
      `}</style>

      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="glass-morphism rounded-3xl shadow-2xl p-6 md:p-8 mb-8 floating">
          <div className="flex flex-col md:flex-row justify-between items-center gap-4">
            <div className="flex items-center gap-3">
              <span className="text-5xl sparkle">📅</span>
              <div>
                <h1 className="text-3xl md:text-4xl font-bold bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600 bg-clip-text text-transparent">
                  Mystical Timetable
                </h1>
                <p className="text-sm text-purple-600 font-medium">
                  {user?.isTeacher ? '✨ Wizard\'s View' : '🎓 Apprentice\'s View'}
                </p>
              </div>
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={() => navigateMonth(-1)}
                className="bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white px-6 py-3 rounded-full font-semibold shadow-lg transform hover:scale-105 transition-all duration-300"
              >
                ◀ Previous
              </button>
              <span className="text-xl md:text-2xl font-bold text-purple-800 px-4">
                {timetable.monthName}
              </span>
              <button
                onClick={() => navigateMonth(1)}
                className="bg-gradient-to-r from-blue-500 to-purple-500 hover:from-blue-600 hover:to-purple-600 text-white px-6 py-3 rounded-full font-semibold shadow-lg transform hover:scale-105 transition-all duration-300"
              >
                Next ▶
              </button>
              <button
                onClick={handleLogout}
                className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-full font-semibold shadow-lg"
              >
                Logout
              </button>
            </div>
          </div>
        </div>

        {/* Legend */}
        <div className="glass-morphism rounded-2xl shadow-xl p-6 mb-8">
          <h2 className="text-xl font-bold text-purple-800 mb-4 flex items-center gap-2">
            <span className="text-2xl">🔮</span> Magic Status Crystal
          </h2>
          <div className="flex flex-wrap gap-4">
            {timetable.statusLegend.map((status) => (
              <div
                key={status.statusId}
                className="flex items-center gap-3 bg-white/50 rounded-full px-4 py-2 shadow-md hover:shadow-lg transition-all duration-300 hover:scale-105"
              >
                <div
                  className="w-8 h-8 rounded-full shadow-inner"
                  style={{ background: status.colorCode }}
                ></div>
                <div>
                  <span className="font-bold text-gray-800">{status.statusName}</span>
                  {status.description && (
                    <span className="text-sm text-gray-600 ml-2">{status.description}</span>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Calendar */}
        <div className="glass-morphism rounded-2xl shadow-2xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600">
                  {['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'].map((day) => (
                    <th
                      key={day}
                      className="text-white font-bold py-4 px-2 text-center border border-purple-700"
                    >
                      {day}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {timetable.weeks.map((week, weekIndex) => (
                  <tr key={weekIndex}>
                    {week.days.map((day) => (
                      <td
                        key={day.date}
                        className={`border border-gray-200 p-2 align-top ${
                          day.isCurrentMonth ? 'bg-white/80' : 'bg-gray-100/50 opacity-60'
                        }`}
                        style={{ minHeight: '120px', height: '150px' }}
                      >
                        <div className="font-bold text-gray-800 mb-2 text-sm">{day.dayOfMonth}</div>
                        {day.entries.map((entry) => (
                          <div
                            key={entry.entryId}
                            className="entry mb-2 p-2 rounded-lg shadow-md cursor-pointer hover:shadow-xl hover:scale-105 text-gray-800"
                            style={{ backgroundColor: entry.colorCode }}
                            onClick={() => handleEditEntry(entry.entryId)}
                          >
                            <div className="font-bold text-xs">⏰ {entry.timeSlot}</div>
                            {user?.isTeacher && (
                              <div className="font-semibold text-sm mt-1">👤 {entry.studentCode}</div>
                            )}
                            <div className="text-xs opacity-90">📚 {entry.subjectCode}</div>
                          </div>
                        ))}
                        {user?.isTeacher && day.isCurrentMonth && (
                          <button
                            className="mt-2 w-full bg-gradient-to-r from-green-400 to-emerald-500 hover:from-green-500 hover:to-emerald-600 text-white font-bold py-2 px-3 rounded-lg text-sm shadow-md hover:shadow-lg transition-all duration-300 transform hover:scale-105"
                            onClick={() => handleAddEntry(day.date)}
                          >
                            ✨ Add
                          </button>
                        )}
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Timetable;
