import React, { createContext, useState, useContext, ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';

interface AuthContextType {
  isAuthenticated: boolean;
  login: (callback?: () => void) => void;
  logout: (callback?: () => void) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const navigate = useNavigate();

  const login = (callback?: () => void) => {
    setIsAuthenticated(true);
    if (callback) callback();
  };

  const logout = (callback?: () => void) => {
    console.log("Logging out...");
    setIsAuthenticated(false);
    if (callback) callback();
    else navigate('/login');
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};