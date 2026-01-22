import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(username, password);
      navigate('/timetable');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Invalid username or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen p-4" style={{
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%)',
      minHeight: '100vh',
      position: 'relative',
      overflow: 'hidden'
    }}>
      <style>{`
        @keyframes rotate {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
        @keyframes float {
          0%, 100% { transform: translateY(0px); }
          50% { transform: translateY(-20px); }
        }
        .glass-morphism {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border: 2px solid rgba(255, 255, 255, 0.3);
        }
        .floating {
          animation: float 3s ease-in-out infinite;
        }
        .input-glow:focus {
          box-shadow: 0 0 30px rgba(102,126,234,0.6);
        }
        .pulse-glow {
          animation: pulse-glow 2s ease-in-out infinite;
        }
        @keyframes pulse-glow {
          0%, 100% { box-shadow: 0 0 20px rgba(102,126,234,0.5); }
          50% { box-shadow: 0 0 40px rgba(102,126,234,0.8), 0 0 60px rgba(102,126,234,0.6); }
        }
      `}</style>
      
      <div className="glass-morphism rounded-3xl shadow-2xl p-8 md:p-12 w-full max-w-md floating pulse-glow relative z-10">
        <div className="text-center mb-8">
          <div className="inline-block text-7xl mb-4">📅</div>
          <h1 className="text-4xl md:text-5xl font-bold bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600 bg-clip-text text-transparent mb-2">
            Mystical Portal
          </h1>
          <p className="text-purple-700 font-medium">Enter the realm of time management</p>
        </div>

        {error && (
          <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded-lg mb-6 shadow-md">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="font-bold text-purple-800 block mb-2">🧙 Wizard Name</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl input-glow focus:outline-none"
              placeholder="Enter your mystical name"
              required
            />
          </div>

          <div>
            <label className="font-bold text-purple-800 block mb-2">🔐 Secret Spell</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-4 py-3 border-2 border-purple-300 rounded-xl input-glow focus:outline-none"
              placeholder="Enter your secret incantation"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-gradient-to-r from-purple-600 via-pink-600 to-blue-600 text-white font-bold py-4 px-6 rounded-full text-lg shadow-xl hover:shadow-2xl transition-all duration-300 disabled:opacity-50"
          >
            {loading ? '✨ Entering... ✨' : '✨ Enter the Portal ✨'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;
