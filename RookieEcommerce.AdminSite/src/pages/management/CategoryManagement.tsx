import React from 'react';
import {
    Container, Box, Typography, TextField, Button, CircularProgress, Alert,
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper,
    IconButton, Dialog, DialogActions, DialogContent, DialogTitle,
    Select, MenuItem, FormControl, InputLabel, FormHelperText, TablePagination
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { useCategoryManagement } from '../../hooks/useCategoryManagement';
import { CategoryDetailsDto } from '../../api';

const DEFAULT_PAGE_SIZE_COMPONENT = 10;

const CategoryManagementPage: React.FC = () => {
    const {
        categories,
        parentCategories,
        loading,
        error,
        page,
        rowsPerPage,
        totalCount,
        searchTerm,
        sortBy,
        isModalOpen,
        editingCategory,
        formData,
        setError,
        handleCreate,
        handleEdit,
        handleDelete,
        handleModalClose,
        handleFormChange,
        handleFormSubmit,
        handleSearchChange,
        handleSortChange,
        handleChangePage,
        handleChangeRowsPerPage,
    } = useCategoryManagement(DEFAULT_PAGE_SIZE_COMPONENT);

    // Handler dialog delete confirm
    const handleDeleteConfirmed = (categoryId: string) => {
        if (window.confirm(`Bạn có chắc muốn xóa danh mục này không? (${categoryId})`)) {
            handleDelete(categoryId);
        }
    };

    // Handler submit form to call prevent default
    const handleSubmitForm = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await handleFormSubmit();
        } catch (submitError) {
            console.info("Submit failed (handled by hook):", submitError);
        }
    };

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom sx={{ textAlign: 'center', fontWeight: 'bold', color: 'primary.main', mb: 3 }}>
                Danh sách Category
            </Typography>

            {/* Search, sort, add button */}
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2, flexWrap: 'wrap', gap: 2 }}>
                <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
                    <TextField
                        label="Tìm kiếm theo tên"
                        variant="outlined"
                        size="small"
                        value={searchTerm}
                        onChange={handleSearchChange}
                        sx={{ minWidth: '200px' }}
                    />
                    <FormControl size="small" sx={{ minWidth: 220 }} disabled={loading && !isModalOpen}>
                        <InputLabel>Sắp xếp theo</InputLabel>
                        <Select
                            value={sortBy}
                            label="Sắp xếp theo"
                            onChange={handleSortChange}
                        >
                            <MenuItem value="name">Tên (A-Z)</MenuItem>
                            <MenuItem value="name desc">Tên (Z-A)</MenuItem>
                            <MenuItem value="createdDate">Ngày tạo (Cũ nhất)</MenuItem>
                            <MenuItem value="createdDate desc">Ngày tạo (Mới nhất)</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={handleCreate}
                    disabled={loading && !isModalOpen}
                >
                    Thêm Category
                </Button>
            </Box>

            {loading && categories.length === 0 && !isModalOpen && (
                <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}><CircularProgress /></Box>
            )}
            {error && !isModalOpen && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>
            )}

            {/* Category table */}
            <TableContainer component={Paper} sx={{ boxShadow: '0 4px 8px rgba(0,0,0,0.1)' }}>
                <Table stickyHeader sx={{ minWidth: 650 }} aria-label="category table">
                    <TableHead >
                        <TableRow>
                            <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Tên</TableCell>
                            <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText', width: 600 }}>Mô tả</TableCell>
                            <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Ngày tạo</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Hành động</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>

                        {!loading && categories.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={5} align="center">
                                    {searchTerm ? `Không tìm thấy danh mục nào với từ khóa "${searchTerm}".` : "Không có danh mục nào."}
                                </TableCell>
                            </TableRow>
                        )}

                        {categories.length > 0 && categories.map((cat: CategoryDetailsDto) => (
                            <TableRow
                                hover
                                key={cat.id}
                                sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                            >
                                <TableCell sx={{ typography: 'body2', whiteSpace: 'nowrap' }}>{cat.name}</TableCell>
                                <TableCell sx={{ typography: 'body2', whiteSpace: 'normal', overflow: 'hidden', textOverflow: 'ellipsis' }}>{cat.description}</TableCell>
                                <TableCell sx={{ typography: 'body2', whiteSpace: 'nowrap' }}>{cat.createdDate ? new Date(cat.createdDate).toLocaleString() : 'N/A'}</TableCell>
                                <TableCell align="right">
                                    <IconButton onClick={() => handleEdit(cat)} color="primary" disabled={loading} size="small">
                                        <EditIcon fontSize="small" />
                                    </IconButton>
                                    <IconButton onClick={() => handleDeleteConfirmed(cat.id!)} color="error" disabled={loading} size="small">
                                        <DeleteIcon fontSize="small" />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}

                        {loading && categories.length === 0 && !isModalOpen &&
                            Array.from(new Array(rowsPerPage)).map((_, index) => (
                                <TableRow key={`skeleton-${index}`}>
                                    <TableCell><Box sx={{ width: '100%', height: 20, backgroundColor: 'grey.200', borderRadius: 1 }} /></TableCell>
                                    <TableCell><Box sx={{ width: '100%', height: 20, backgroundColor: 'grey.200', borderRadius: 1 }} /></TableCell>
                                    <TableCell><Box sx={{ width: '100%', height: 20, backgroundColor: 'grey.200', borderRadius: 1 }} /></TableCell>
                                    <TableCell><Box sx={{ width: '100%', height: 20, backgroundColor: 'grey.200', borderRadius: 1 }} /></TableCell>
                                    <TableCell align="right"><Box sx={{ display: 'flex', justifyContent: 'flex-end' }}><CircularProgress size={20} sx={{ mr: 1 }} /><CircularProgress size={20} /></Box></TableCell>
                                </TableRow>
                            ))
                        }
                    </TableBody>
                </Table>
            </TableContainer>

            {/* --- Pagination --- */}
            {totalCount > 0 && (!loading || categories.length > 0) && (
                <TablePagination
                    component="div"
                    count={totalCount}
                    page={page}
                    rowsPerPage={rowsPerPage}
                    onPageChange={handleChangePage}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                    rowsPerPageOptions={[5, 10, DEFAULT_PAGE_SIZE_COMPONENT, 25, 50, 100].filter((v, i, a) => a.indexOf(v) === i).sort((a, b) => a - b)} // Loại bỏ trùng lặp và sắp xếp
                    labelRowsPerPage="Số dòng mỗi trang:"
                    labelDisplayedRows={({ from, to, count }) =>
                        `${from}–${to} của ${count !== -1 ? count : `hơn ${to}`}`
                    }
                    SelectProps={{
                        inputProps: { 'aria-label': 'rows per page' },
                        native: true,
                        disabled: loading,
                    }}
                    backIconButtonProps={{
                        disabled: loading || page === 0,
                    }}
                    nextIconButtonProps={{
                        disabled: loading || page >= Math.ceil(totalCount / rowsPerPage) - 1,
                    }}
                    sx={{ mt: 2, borderTop: '1px solid rgba(224, 224, 224, 1)' }}
                />
            )}

            {/* Add/Update Modal */}
            <Dialog open={isModalOpen} onClose={handleModalClose} maxWidth="sm" fullWidth>
                <DialogTitle>{editingCategory ? 'Sửa Danh mục' : 'Thêm Danh mục mới'}</DialogTitle>
                <Box component="form" onSubmit={handleSubmitForm} noValidate>
                    <DialogContent>

                        {error && isModalOpen && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="name"
                            label="Tên Danh mục"
                            name="name"
                            value={formData.name}
                            onChange={handleFormChange}
                            autoFocus
                            disabled={loading}
                        />
                        <FormControl fullWidth margin="normal" disabled={!!editingCategory?.id || loading}>
                            <InputLabel id="parent-category-label">Tắt vì chưa implement logic ở customer site</InputLabel>
                            <Select
                                labelId="parent-category-label"
                                id="parentCategoryId"
                                name="parentCategoryId"
                                value={formData.parentCategoryId ?? ''}
                                label="Danh mục cha"
                                onChange={handleFormChange}
                                disabled
                            >
                                <MenuItem value="">
                                    <em>-- Không có (Danh mục gốc) --</em>
                                </MenuItem>
                                {parentCategories
                                    .filter(pCat => pCat.id !== editingCategory?.id)
                                    .map(pCat => (
                                        <MenuItem key={pCat.id} value={pCat.id}>
                                            {pCat.name}
                                        </MenuItem>
                                    ))}
                            </Select>
                            {!!editingCategory?.id && <FormHelperText>Không thể thay đổi danh mục cha khi sửa.</FormHelperText>}
                        </FormControl>
                        <TextField
                            margin="normal"
                            fullWidth
                            id="description"
                            label="Mô tả"
                            name="description"
                            multiline
                            rows={3}
                            value={formData.description}
                            onChange={handleFormChange}
                            disabled={loading}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleModalClose} color="inherit" disabled={loading}>Hủy</Button>
                        <Button
                            type="submit"
                            variant="contained"
                            disabled={loading || !formData.name}
                        >
                            {loading ? <CircularProgress size={24} /> : (editingCategory ? 'Cập nhật' : 'Tạo mới')}
                        </Button>
                    </DialogActions>
                </Box>
            </Dialog>
        </Container>
    );
};

export default CategoryManagementPage;