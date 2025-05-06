import {
    Alert, Box, Button, CircularProgress, Container, Dialog,
    DialogActions, DialogContent, DialogTitle, FormControl, InputLabel,
    IconButton, MenuItem, Paper, Select, /* SelectChangeEvent, */ // SelectChangeEvent dùng trong hook
    Table, TableBody, TableCell, TableContainer, TableHead,
    TablePagination, TableRow, TextField, Typography,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { useProductVariantManagement } from '../../hooks/useProductVariantManagement'; // Đường dẫn tới hook

interface ProductVariantManagementProps {
    productId: string | null;
}

const ProductVariantManagementPage: React.FC<ProductVariantManagementProps> = ({ productId }) => {
    const {
        variants,
        loading,
        error,
        openDialog,
        editingVariant,
        formData,
        page,
        rowsPerPage,
        totalCount,
        openConfirmDialog,
        variantToDelete,
        searchTerm,
        sortBy,
        minPrice: hookMinPrice, // Đổi tên để rõ ràng
        maxPrice: hookMaxPrice, // Đổi tên để rõ ràng
        handleSearchChange,
        handleSortChange,
        handleMinPriceChange,
        handleMaxPriceChange,
        handleApplyFilters,
        handleClearFilters,
        handleChangePage,
        handleChangeRowsPerPage,
        handleOpenDialog,
        handleCloseDialog,
        handleOpenConfirmDialog,
        handleCloseConfirmDialog,
        handleInputChange,
        handleSaveVariant,
        handleDeleteConfirm,
        setError,
    } = useProductVariantManagement({ productId });

    // --- Render Component ---
    if (productId === null) {
        return (
            <Container maxWidth="lg" sx={{ mt: 1, mb: 1, display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
                <CircularProgress />
            </Container>
        );
    }

    return (
        <Container maxWidth="lg">
            <Typography variant="h4" component="h1" gutterBottom sx={{ textAlign: 'center', fontWeight: 'bold', color: 'primary.main', mb: 3 }}>
                Danh sách Variants
            </Typography>

            {/* Toolbar: Search, Sort, Add Button */}
            <Box
                sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2, flexWrap: 'wrap', gap: 2 }}
            >
                <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center' }}>
                    <TextField
                        label="Tìm theo tên/SKU"
                        variant="outlined"
                        size="small"
                        value={searchTerm}
                        onChange={handleSearchChange}
                        sx={{ minWidth: '200px' }}
                        disabled={loading}
                    />
                    <TextField
                        label="Giá thấp nhất"
                        variant="outlined"
                        size="small"
                        type="number"
                        value={hookMinPrice}
                        onChange={handleMinPriceChange}
                        InputProps={{ inputProps: { min: 0 } }}
                        sx={{ width: { xs: 'calc(50% - 8px)', sm: 150 } }} 
                        disabled={loading}
                    />
                    <TextField
                        label="Giá cao nhất"
                        variant="outlined"
                        size="small"
                        type="number"
                        value={hookMaxPrice}
                        onChange={handleMaxPriceChange}
                        InputProps={{ inputProps: { min: 0 } }}
                        sx={{ width: { xs: 'calc(50% - 8px)', sm: 150 } }}
                        disabled={loading}
                    />
                    <FormControl size="small" sx={{ minWidth: 180 }} disabled={loading}>
                        <InputLabel>Sắp xếp theo</InputLabel>
                        <Select value={sortBy} label="Sắp xếp theo" onChange={handleSortChange}>
                            <MenuItem value="name">Tên (A-Z)</MenuItem>
                            <MenuItem value="name desc">Tên (Z-A)</MenuItem>
                            <MenuItem value="sku">SKU (A-Z)</MenuItem>
                            <MenuItem value="sku desc">SKU (Z-A)</MenuItem>
                            <MenuItem value="price">Phụ phí (Thấp-Cao)</MenuItem>
                            <MenuItem value="price desc">Phụ phí (Cao-Thấp)</MenuItem>
                            <MenuItem value="stockQuantity">Tồn kho (Ít-Nhiều)</MenuItem>
                            <MenuItem value="stockQuantity desc">Tồn kho (Nhiều-Ít)</MenuItem>
                        </Select>
                    </FormControl>
                    <Button variant="contained" onClick={handleApplyFilters} disabled={loading} size="medium">Áp dụng</Button>
                    <Button variant="outlined" onClick={handleClearFilters} disabled={loading} size="medium">Xóa lọc</Button>
                </Box>
                <Button variant="contained" startIcon={<AddIcon />} onClick={() => handleOpenDialog()} disabled={loading}>
                    Tạo Biến thể
                </Button>
            </Box>

            {/* Global Loading Indicator and Error */}
            {error && !openDialog && !openConfirmDialog && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            {/* Variants Table */}
            <TableContainer component={Paper}>
                <Table stickyHeader aria-label="variant table">
                    <TableHead>
                        <TableRow>
                            <TableCell>Tên Biến thể</TableCell>
                            <TableCell>Loại</TableCell>
                            <TableCell>SKU</TableCell>
                            <TableCell align="right">Phụ phí</TableCell>
                            <TableCell align="right">Tồn kho</TableCell>
                            <TableCell align="center">Hành động</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {loading && variants.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={6} align="center"><CircularProgress /></TableCell>
                            </TableRow>
                        ) : !loading && variants.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={6} align="center">
                                    {searchTerm ? `Không tìm thấy biến thể nào khớp với "${searchTerm}".` : "Chưa có biến thể nào cho sản phẩm này."}
                                </TableCell>
                            </TableRow>
                        ) : (
                            variants.map((variant) => (
                                <TableRow hover key={variant.id}>
                                    <TableCell>{variant.name}</TableCell>
                                    <TableCell>{variant.variantType ?? 'N/A'}</TableCell>
                                    <TableCell>{variant.sku ?? 'N/A'}</TableCell>
                                    <TableCell align="right">{variant.price?.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }) ?? 'N/A'}</TableCell>
                                    <TableCell align="right">{variant.stockQuantity ?? 'N/A'}</TableCell>
                                    <TableCell align="center">
                                        <IconButton onClick={() => handleOpenDialog(variant)} color="primary" disabled={loading}><EditIcon /></IconButton>
                                        <IconButton onClick={() => handleOpenConfirmDialog(variant)} color="error" disabled={loading}><DeleteIcon /></IconButton>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </TableContainer>

            {/* Pagination */}
            {totalCount > 0 && (
                <TablePagination
                    rowsPerPageOptions={[5, 10, 25, 50]}
                    component="div"
                    count={totalCount}
                    rowsPerPage={rowsPerPage}
                    page={page}
                    onPageChange={handleChangePage}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                    disabled={loading}
                />
            )}

            {/* Create/Edit Variant Dialog */}
            <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
                <DialogTitle>{editingVariant ? 'Chỉnh sửa Biến thể' : 'Tạo Biến thể Mới'}</DialogTitle>
                <DialogContent>
                    {error && openDialog && (
                        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                            {error}
                        </Alert>
                    )}
                    <TextField autoFocus margin="dense" name="name" label="Tên Biến thể" type="text" fullWidth variant="outlined" value={formData.name ?? ''} onChange={handleInputChange} required disabled={loading} />
                    <TextField margin="dense" name="variantType" label="Loại Biến thể (ví dụ: Màu sắc, Kích thước)" type="text" fullWidth variant="outlined" value={formData.variantType ?? ''} onChange={handleInputChange} disabled={!!editingVariant || loading} />
                    <TextField margin="dense" name="price" label="Phụ Phí" type="number" fullWidth variant="outlined" value={formData.price ?? 0} onChange={handleInputChange} required InputProps={{ inputProps: { min: 0, step: 'any' } }} disabled={loading} />
                    <TextField margin="dense" name="stockQuantity" label="Số lượng tồn kho" type="number" fullWidth variant="outlined" value={formData.stockQuantity ?? 0} onChange={handleInputChange} required InputProps={{ inputProps: { min: 0, step: 1 } }} disabled={loading} />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseDialog} disabled={loading} color="inherit">Hủy</Button>
                    <Button onClick={handleSaveVariant} variant="contained" disabled={loading}>
                        {loading && openDialog ? <CircularProgress size={24} color="inherit" /> : (editingVariant ? 'Lưu Thay đổi' : 'Tạo Biến thể')}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Delete Confirmation Dialog */}
            <Dialog open={openConfirmDialog} onClose={handleCloseConfirmDialog}>
                <DialogTitle>Xác nhận Xóa Biến thể</DialogTitle>
                <DialogContent>
                    <Typography>
                        Bảnh có chắc muốn xóa biến thể "{variantToDelete?.name ?? 'này'}" không? Hành động này không thể hoàn tác.
                    </Typography>

                     {error && openConfirmDialog && (
                        <Alert severity="error" sx={{ mt: 2 }} onClose={() => setError(null)}>
                            {error}
                        </Alert>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseConfirmDialog} disabled={loading} color="inherit">Hủy</Button>
                    <Button onClick={handleDeleteConfirm} color="error" variant="contained" autoFocus disabled={loading}>
                        {loading && openConfirmDialog ? <CircularProgress size={24} color="inherit" /> : 'Xóa'}
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default ProductVariantManagementPage;

