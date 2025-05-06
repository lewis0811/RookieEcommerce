import { Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import MainLayout from './layouts/MainLayout';
import ProtectedRoute from './components/common/ProtectedRoute'; // Import ProtectedRoute
import NotFoundPage from './pages/NotFoundPage';
import CategoryManagementPage from './pages/management/CategoryManagement';
import ProductManagementPage from './pages/management/ProductManagement';
import { LoginCallback } from './auth/LoginCallback';
import { LogoutCallback } from './auth/LogoutCallback';
import { ProductDetailManagementPage } from './pages/management/ProductDetailManagementPage';

function App() {
  return (
    <>
      <Routes>
        <Route element={<ProtectedRoute />}>
          {/* --- Layout --- */}
          <Route element={<MainLayout />}>

            {/* --- Home --- */}
            <Route path="/" element={<HomePage />} />

            {/* --- Management --- */}
            <Route path="/manage/categories" element={<CategoryManagementPage />} />
            <Route path="/manage/products" element={<ProductManagementPage />} />
            <Route path="/manage/products/details" element={<ProductDetailManagementPage />} />
          </Route>
        </Route>

        {/* --- Login --- */}
        <Route path="/login" element={<LoginPage />} />
        <Route path='/callback/login' element={<LoginCallback />} />

        {/* --- LogOut --- */}
        <Route path='/callback/logout' element={<LogoutCallback />} />

        {/* --- Not Found --- */}
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </>
  );
}

export default App;