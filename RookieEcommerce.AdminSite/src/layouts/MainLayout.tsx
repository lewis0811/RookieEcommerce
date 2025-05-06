import React, { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Box,
  CssBaseline,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  Divider,
  Collapse, // Thêm Divider
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import HomeIcon from '@mui/icons-material/Home';
import LogoutIcon from '@mui/icons-material/Logout';
import ListAltIcon from '@mui/icons-material/ListAlt';
import FiberManualRecordIcon from '@mui/icons-material/FiberManualRecord';
import { ExpandLess, ExpandMore } from '@mui/icons-material';
import { authService } from '../auth/AuthConfig';

const drawerWidth = 240;

const MainLayout: React.FC = () => {
  // State mới để quản lý menu Quản lý
  const [managementOpen, setManagementOpen] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleNavigate = (path: string) => {
    navigate(path);
  };

  const handleManagementClick = () => {
    setManagementOpen(!managementOpen);
  };

  const handleLogout = async () => {
    await authService.logout();
    // logout();
  };

  const drawer = (
    <div>
      <Toolbar />
      <Divider />
      <List>
        {/* --- HomePage --- */}
        <ListItem disablePadding>
          <ListItemButton onClick={() => handleNavigate('/')}>
            <ListItemIcon>
              <HomeIcon />
            </ListItemIcon>
            <ListItemText primary="Trang Chủ" />
          </ListItemButton>
        </ListItem>

        {/* --- Management --- */}
        <ListItemButton onClick={handleManagementClick}>
          <ListItemIcon>
            <ListAltIcon />
          </ListItemIcon>
          <ListItemText primary="Quản Lý" />
          {managementOpen ? <ExpandLess /> : <ExpandMore />}
        </ListItemButton>
        <Collapse in={managementOpen} timeout="auto" unmountOnExit>
          <List component="div" disablePadding>

            {/* Category */}
            <ListItemButton sx={{ pl: 4 }} onClick={() => handleNavigate('/manage/categories')}>
              <ListItemIcon>
                <FiberManualRecordIcon sx={{ fontSize: 'small' }} />
              </ListItemIcon>
              <ListItemText primary="Quản lý Category" />
            </ListItemButton>

            {/* Product */}
            <ListItemButton sx={{ pl: 4 }} onClick={() => handleNavigate('/manage/products')}>
              <ListItemIcon>
                <FiberManualRecordIcon sx={{ fontSize: 'small' }} />
              </ListItemIcon>
              <ListItemText primary="Quản lý Product" />
            </ListItemButton>
          </List>
        </Collapse>

        <Divider />
        <ListItem disablePadding>
          <ListItemButton onClick={handleLogout}>
            <ListItemIcon>
              <LogoutIcon />
            </ListItemIcon>
            <ListItemText primary="Đăng Xuất" />
          </ListItemButton>
        </ListItem>
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar
        position="fixed"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div">
            NashLux Admin
          </Typography>
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
        aria-label="mailbox folders"
      >
        {/* Mobile drawer */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>

        {/* Web drawer */}
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>

      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
        }}
      >
        <Toolbar />
        <Outlet />
      </Box>

    </Box>
  );
};

export default MainLayout;