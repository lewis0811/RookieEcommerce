import React from 'react';
import {
  Container,
  Box,
  Typography,
  Button,
  Paper,
} from '@mui/material';
import { authService } from '../auth/AuthConfig';

const LoginPage: React.FC = () => {

  const handleLoginClick = async () => {
    try {
      await authService.login();
    } catch (error) {
      console.error("Lỗi khi bắt đầu đăng nhập OIDC:", error);
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Paper elevation={3} sx={{ mt: 8, p: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography component="h1" variant="h5">
          NashLux Admin
        </Typography>
        <Box sx={{ mt: 1, width: '100%' }}>
          <Button
            type="button"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            onClick={handleLoginClick}
          >
            Đăng Nhập
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default LoginPage;