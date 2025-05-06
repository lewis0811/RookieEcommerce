import React, {
} from 'react';
import {
  Alert,
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardMedia,
  Chip,
  CircularProgress,
  Container,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  IconButton,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate'; // Icon thêm ảnh
import DeleteIcon from '@mui/icons-material/Delete';
import StarBorderIcon from '@mui/icons-material/StarBorder';
import EditIcon from '@mui/icons-material/Edit';
import CancelIcon from '@mui/icons-material/Cancel';
import SaveIcon from '@mui/icons-material/Save';
import { useProductImageManagement } from '../../hooks/useProductImageManagement';

interface ProductImageManagementProps {
  productId: string | null;
}

const ProductImageManagementPage: React.FC<ProductImageManagementProps> = ({ productId }) => {

  const {
    productImages,
    loading,
    error,
    uploading,
    editingImageId,
    editFormData,
    imagePreview,
    fileToUpload,
    imageToDelete,
    openUploadDialog,
    openConfirmDialog,
    // fetchProductImages, // Only needed if you want a manual refresh button
    handleOpenUploadDialog,
    handleCloseUploadDialog,
    handleFileSelect,
    handleSaveImage,
    handleOpenConfirmDialog,
    handleCloseConfirmDialog,
    handleDeleteImageConfirm,
    handleSetPrimaryImage,
    handleStartEdit,
    handleCancelEdit,
    handleEditFormChange,
    handleSaveChanges,
    setError
} = useProductImageManagement(productId);

  // --- JSX ---
  return (
    <Container maxWidth="lg" sx={{ textAlign: 'center' }}>
      <Typography variant="h4" component="h1" gutterBottom sx={{ textAlign: 'center', fontWeight: 'bold', color: 'primary.main', mb: 3 }}>
        Danh sách Images
      </Typography>
      <Typography variant='body1'>
        Bạn chỉ có thể add tối đa 3 ảnh
      </Typography>

      {/* --- Add Image Button --- */}
      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<AddPhotoAlternateIcon />}
          onClick={handleOpenUploadDialog}
          disabled={loading || uploading || !productId} // Disable nếu đang tải hoặc chưa có productId
        >
          Thêm ảnh mới
        </Button>
      </Box>

      {/* --- Loading & Error --- */}
      {loading && <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}><CircularProgress /></Box>}
      {error && !openUploadDialog && !openConfirmDialog && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}
      {!productId && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          Không tìm thấy ID Sản phẩm trong đường dẫn.
        </Alert>
      )}


      {/* --- Grid Show Image --- */}
      <Grid container spacing={3}>
        {productImages.length > 0 ? (
          productImages.map((image) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={image.id}>
              {/* --- 4. Conditional Rendering: Edit Mode vs Display Mode --- */}
              {editingImageId === image.id ? (
                // --- Edit Mode ---
                <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', border: '2px solid', borderColor: 'primary.main' }}>
                  <CardMedia component="img" height="150" image={image.imageUrl || "/placeholder.png"} alt={`Editing ${image.altText}`} sx={{ objectFit: 'contain' }} />
                  <CardContent sx={{ flexGrow: 1, p: 2 }}>

                    {error && editingImageId === image.id && (
                      <Alert severity="error" sx={{ mb: 1 }} onClose={() => setError(null)}> {error} </Alert>
                    )}

                    <TextField
                      label="Alt Text"
                      name="altText"
                      value={editFormData.altText}
                      onChange={handleEditFormChange}
                      fullWidth
                      margin="normal"
                      size="small"
                      disabled={loading}
                    />
                    <TextField
                      label="Thứ tự (Sort Order)"
                      name="sortOrder"
                      type="number"
                      value={editFormData.sortOrder}
                      onChange={handleEditFormChange}
                      fullWidth
                      margin="normal"
                      size="small"
                      disabled={loading}
                      InputProps={{ inputProps: { min: 0 } }}
                    />
                  </CardContent>
                  <CardActions sx={{ justifyContent: 'center', pb: 2 }}>
                    <Tooltip title="Hủy bỏ thay đổi">
                      <span>
                        <IconButton onClick={handleCancelEdit} disabled={loading} color="warning" size="small">
                          <CancelIcon />
                        </IconButton>
                      </span>
                    </Tooltip>
                    <Tooltip title="Lưu thay đổi">
                      <span>
                        <IconButton onClick={handleSaveChanges} disabled={loading} color="success" size="small">
                          {loading ? <CircularProgress size={20} color="inherit" /> : <SaveIcon />}
                        </IconButton>
                      </span>
                    </Tooltip>
                  </CardActions>
                </Card>
              ) : (
                // --- Display Mode ---
                <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', position: 'relative' }}>
                  <CardMedia
                    component="img"
                    height="300"
                    image={image.imageUrl || "/placeholder.png"}
                    alt={image.altText || `Product Image ${image.id}`}
                    sx={{ objectFit: 'cover' }}
                    onError={(e) => { (e.target as HTMLImageElement).onerror = null; (e.target as HTMLImageElement).src = "/placeholder.png"; }}
                  />
                    {image.isPrimary && (<Chip label="Ảnh Chính" color="success" size="small" sx={{ position: 'absolute', bottom: 12, right: 10, fontWeight: 'bold', boxShadow: 1 }} />)}
                  {/* Show Alt Text & Order */}
                  <CardContent sx={{ pt: 1, pb: 0 }}>
                    <Typography variant="caption" display="block" gutterBottom noWrap title={image.altText || '(No Alt Text)'}>
                      Alt: {image.altText || '(None)'}
                    </Typography>
                    <Typography variant="caption" display="block">
                      Order: {image.sortOrder ?? 'N/A'}
                    </Typography>
                  </CardContent>
                  <CardActions sx={{ justifyContent: 'center', marginTop: 'auto' }}>
                    {/* Edit */}
                    <Tooltip title="Chỉnh sửa Alt Text / Thứ tự">
                      <span>
                        <IconButton onClick={() => handleStartEdit(image)} disabled={loading || uploading || !!editingImageId} size="small" color="info">
                          <EditIcon />
                        </IconButton>
                      </span>
                    </Tooltip>
                    {/* Set Primary */}
                    {!image.isPrimary && (
                      <Tooltip title="Đặt làm ảnh chính">
                        <span>
                          <IconButton onClick={() => handleSetPrimaryImage(image.id!)} disabled={loading || uploading || !!editingImageId} color="primary" size="small">
                            <StarBorderIcon />
                          </IconButton>
                        </span>
                      </Tooltip>
                    )}
                    {/* Delete */}
                    <Tooltip title="Xóa ảnh">
                      <span>
                        <IconButton onClick={() => handleOpenConfirmDialog(image)} disabled={loading || uploading || !!editingImageId} color="error" size="small">
                          <DeleteIcon />
                        </IconButton>
                      </span>
                    </Tooltip>
                  </CardActions>
                </Card>
              )}
              {/* -------------------------------------------------------- */}
            </Grid>
          ))
        ) : (
          !loading && productId && (
            <Grid size={{ xs: 12 }} >
              <Typography sx={{ textAlign: 'center', mt: 5, fontStyle: 'italic' }}> Sản phẩm này chưa có ảnh nào.</Typography>
            </Grid>
          )
        )}
      </Grid>

      {/* --- Dialog Upload Image --- */}
      <Dialog open={openUploadDialog} onClose={handleCloseUploadDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Thêm Ảnh Mới</DialogTitle>
        <DialogContent>
          <Button
            variant="outlined"
            component="label"
            fullWidth
            sx={{ mt: 1, mb: 2 }}
            disabled={uploading}
          >
            Chọn ảnh từ máy tính
            <input
              type="file"
              hidden
              accept="image/*"
              onChange={handleFileSelect}
            />
          </Button>


          {/* Hiển thị tên file đã chọn */}
          {fileToUpload && <Typography variant="body2" sx={{ mb: 1 }}>Đã chọn: {fileToUpload.name}</Typography>}


          {/* Xem trước ảnh */}
          {imagePreview && (
            <Box sx={{ textAlign: 'center', mb: 2, maxHeight: 300, overflow: 'hidden' }}>
              <img src={imagePreview} alt="Preview" style={{ maxWidth: '100%', maxHeight: '280px', objectFit: 'contain' }} />
            </Box>
          )}

          {/* Hiển thị lỗi trong dialog */}
          {error && openUploadDialog && (
            <Alert severity="error" sx={{ mt: 2 }} onClose={() => setError(null)}>
              {error}
            </Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseUploadDialog} disabled={uploading} color="inherit">Hủy</Button>
          <Button
            onClick={handleSaveImage}
            variant="contained"
            disabled={!fileToUpload || uploading} // Disable nếu chưa chọn file hoặc đang upload
          >
            {uploading ? <CircularProgress size={24} color="inherit" /> : 'Tải lên & Lưu'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* --- Dialog Confirm Delete --- */}
      <Dialog open={openConfirmDialog} onClose={handleCloseConfirmDialog}>
        <DialogTitle>Xác nhận Xóa Ảnh</DialogTitle>
        <DialogContent>
          <Typography>
            Bảnh có chắc muốn xóa ảnh này không? Hành động này không thể hoàn tác.
          </Typography>

          {imageToDelete?.imageUrl && (
            <Box sx={{ textAlign: 'center', mt: 2 }}>
              <img src={imageToDelete.imageUrl} alt="Image to delete" style={{ maxWidth: '100px', maxHeight: '100px' }} />
            </Box>
          )}

        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseConfirmDialog} disabled={loading} color="inherit">Hủy</Button>
          <Button
            onClick={handleDeleteImageConfirm}
            color="error"
            variant="contained"
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} color="inherit" /> : 'Xóa'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default ProductImageManagementPage;
