// src/auth/LoginCallback.tsx
import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from './AuthConfig';
import { useAuth } from '../contexts/AuthContext';

export const LoginCallback = () => {
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const processed = useRef(false);
  const { login } = useAuth();

  useEffect(() => {
    if (!processed.current) {
      processed.current = true;
      const processLoginCallback = async () => {
        try {
          const user = await authService.handleLoginCallback();
          if (Array.isArray(user?.profile?.role) && user.profile.role[0] === "ADMIN") {
            login();
            navigate('/');
          } else {
            setError('Login failed or user data not available.');
            await authService.handleLogoutCallback();
            localStorage.clear();
            
            navigate('/login');
          }
        } catch (err) {
          console.error('Login callback error:', err);
          setError(err instanceof Error ? err.message : 'Unknown error during login');
          navigate('/login');
        }
      };

      processLoginCallback();
    }
  }, [navigate, login]);

  if (error) {
    return <div>Login failed: {error}</div>;
  }

  return <div>Processing login...</div>;
};