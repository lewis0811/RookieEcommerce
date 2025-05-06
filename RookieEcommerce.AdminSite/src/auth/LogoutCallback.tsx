import { useEffect } from 'react';
import { Navigate } from 'react-router-dom';

export const LogoutCallback = () => {
  useEffect(() => {
    console.log('Logout callback processed');
  }, []);

  return <Navigate to="/" replace />;
};