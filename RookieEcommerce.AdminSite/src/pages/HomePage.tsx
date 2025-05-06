import {
  Alert,
  Box,
  CircularProgress,
  Container,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { useCustomerManagement } from '../hooks/useCustomerManagement';

const Homepage: React.FC = () => {
  const {
    customers,
    loading,
    error,
    page,
    rowsPerPage,
    totalCount,
    searchTerm,
    sortBy,
    handleSearchChange,
    handleSearchSubmit,
    handleSortChange,
    handleChangePage,
    handleChangeRowsPerPage,
    setError,
  } = useCustomerManagement();

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom sx={{ textAlign: 'center', fontWeight: 'bold', color: 'primary.main', mb: 3 }}>
        Danh Sách Khách Hàng
      </Typography>

      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3,
          flexWrap: 'wrap',
          gap: 2,
          p: 2,
          borderRadius: 1,
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        }}
      >
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center', flexGrow: 1 }}>
          <TextField
            label="Tìm kiếm khách hàng"
            variant="outlined"
            size="small"
            value={searchTerm}
            onChange={handleSearchChange}
            onKeyPress={(e) => e.key === 'Enter' && handleSearchSubmit()}
            sx={{ minWidth: '250px', flexGrow: 1 }}
          />
          <FormControl size="small" sx={{ minWidth: 220 }}>
            <InputLabel id="sort-by-label">Sắp xếp theo</InputLabel>
            <Select
              labelId="sort-by-label"
              value={sortBy}
              label="Sắp xếp theo"
              onChange={handleSortChange}
              disabled={loading}
            >
              <MenuItem value="name">Tên (A-Z)</MenuItem>
              <MenuItem value="name desc">Tên (Z-A)</MenuItem>
              <MenuItem value="email">Email (A-Z)</MenuItem>
              <MenuItem value="email desc">Email (Z-A)</MenuItem>
            </Select>
          </FormControl>
        </Box>
      </Box>

      {loading && <Box sx={{ display: 'flex', justifyContent: 'center', my: 3 }}><CircularProgress size={50} /></Box>}
      {error && !loading && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {!loading && !error && customers.length === 0 && (
         <Typography variant="h6" sx={{ textAlign: 'center', my: 4, color: 'text.secondary' }}>
            {searchTerm ? `Không tìm thấy khách hàng nào với từ khóa "${searchTerm}".` : "Không có khách hàng nào."}
        </Typography>
      )}

      {!loading && customers.length > 0 && (
        <TableContainer component={Paper} sx={{ boxShadow: '0 4px 8px rgba(0,0,0,0.1)' }}>
          <Table stickyHeader aria-label="customer table">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Tên Khách Hàng</TableCell>
                <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Email</TableCell>
                <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Ngày tạo</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {customers.map((customer) => (
                <TableRow hover role="checkbox" tabIndex={-1} key={customer.id} sx={{ '&:hover': { backgroundColor: 'action.hover' } }}>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>{customer.firstName + " " + customer.lastName || '-'}</TableCell>
                  <TableCell>{customer.email || '-'}</TableCell>
                  <TableCell>{customer.createdDate?.toLocaleDateString('vi-VN') ?? 'N/A'}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {totalCount > 0 && !loading && (
        <TablePagination
          rowsPerPageOptions={[5, 10, 25, 50, 100]}
          component="div"
          count={totalCount}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Số dòng mỗi trang:"
          labelDisplayedRows={({ from, to, count }) => `${from}–${to} của ${count !== -1 ? count : `hơn ${to}`}`}
          sx={{ mt: 2, boxShadow: '0 -2px 4px rgba(0,0,0,0.05)', borderTop: '1px solid #eee' }}
          SelectProps={{ disabled: loading }}
          backIconButtonProps={{ disabled: loading || page === 0, title: "Trang trước" }}
          nextIconButtonProps={{ disabled: loading || page >= Math.ceil(totalCount / rowsPerPage) - 1, title: "Trang sau" }}
        />
      )}
    </Container>
  );
};

export default Homepage;