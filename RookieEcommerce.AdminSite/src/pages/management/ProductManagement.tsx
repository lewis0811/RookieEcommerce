import React, {
  useState,
  useEffect,
  useCallback,
  ChangeEvent,
  useRef,
} from 'react';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Container, // Changed Paper to Container for overall structure
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl, // Added
  InputLabel, // Added
  IconButton,
  MenuItem, // Added
  Paper,
  Select, // Added
  SelectChangeEvent, // Added
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination, // Kept TablePagination as it's suitable here
  TableRow,
  TextField,
  Typography,
  Grid,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import InfoIcon from '@mui/icons-material/Info';
import {
  Configuration,
  CreateProductCommand,
  ProductDetailsDto,
  ProductDetailsDtoPaginationList,
  ProductsApi,
  UpdateProductCommand,
  CategoriesApi, // Added to fetch categories for dropdown
  Category, // Added Category type
} from '../../api';
import { authService } from '../../auth/AuthConfig';
import { BASE_PATH_API } from '../../config/api';
import { useNavigate } from 'react-router-dom';
import $ from 'jquery';

const apiConfig = new Configuration({ basePath: BASE_PATH_API });
const productsApi = new ProductsApi(apiConfig);
const categoriesApi = new CategoriesApi(apiConfig);

const ProductManagement: React.FC = () => {
  const [products, setProducts] = useState<ProductDetailsDto[]>([]);
  const [categories, setCategories] = useState<Category[]>([]); // State for categories dropdown
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const [editingProduct, setEditingProduct] = useState<ProductDetailsDto | null>(
    null
  );
  const [formData, setFormData] = useState<
    Partial<CreateProductCommand & UpdateProductCommand>
  >({});
  const [page, setPage] = useState(0); // Keep TablePagination's 0-based index
  const [rowsPerPage, setRowsPerPage] = useState(10); // Default to 10 like Category example
  const [totalCount, setTotalCount] = useState(0);
  const [openConfirmDialog, setOpenConfirmDialog] = useState(false);
  const [productToDelete, setProductToDelete] =
    useState<ProductDetailsDto | null>(null);

  // --- Navigation ---~
  const navigate = useNavigate();

  // --- Summernote Rich text editor ---
  const editorRef = useRef<HTMLDivElement>(null);

  // --- Effect to initialize and destroy Summernote ---
  useEffect(() => {
    const initializeEditor = () => {
      if (editorRef.current) {
        console.log('Attempting to initialize Summernote...');
        // Initialize Summernote on the ref element
        $(editorRef.current).summernote({
          placeholder: 'Nhập chi tiết sản phẩm ở đây...',
          height: 300, 
          minHeight: 150,
          maxHeight: 500,
          focus: false,
          callbacks: {
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            onChange: function (contents, _$editable) {
              setFormData(prev => ({ ...prev, details: contents }));
            }
          }
        });
        console.log('Summernote initialized.');
      } else {
        console.warn('editorRef.current is null, cannot initialize Summernote.');
      }
    };

    // Function to destroy the editor
    const destroyEditor = () => {
      if (editorRef.current && $(editorRef.current).hasClass('note-editor')) {
        console.log('Destroying Summernote...');
        $(editorRef.current).summernote('destroy');
        console.log('Summernote destroyed.');
      }
    };

    // --- Logic based on dialog state ---
    if (openDialog) {
      // IMPORTANT: Initialize AFTER the dialog is open and element is likely rendered
      // Use a small timeout to ensure the DOM element is definitely ready
      const timer = setTimeout(() => {
        initializeEditor();
      }, 100);

      return () => clearTimeout(timer);

    } else {
      destroyEditor();
    }

    // --- Cleanup on component unmount ---
    return () => {
      destroyEditor();
    };

  }, [openDialog]);

  const getEditorContent = () => {
    if (editorRef.current) {
      console.log("in");
      const editorInstance = editorRef.current || null;
      const content = ($(editorInstance) as JQuery).summernote();
      console.log(content); // Lấy nội dung HTML

      return content;
    }
    return '';
  };

  // --- Search & Sort State ---
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [sortBy, setSortBy] = useState<string>('name'); // Default sort by name


  const getAuthHeaders = useCallback(async (): Promise<Record<string, string> | null> => {
    try {
      const token = await authService.getAccessToken();
      if (!token) {
        // setError('Authentication token not found. Please login.'); // Consider redirecting instead
        console.error('Authentication token not found.');
        return null; // Indicate failure
      }
      return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      };
    } catch (authError) {
      console.error('Error getting access token:', authError);
      setError('Failed to get authentication token.');
      return null;
    }
  }, []); // Removed setError dependency to avoid potential loops

  const parseApiError = async (err: unknown): Promise<string> => {
    if (err instanceof Error) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const response = (err as any).response as Response | undefined;
      if (response) {
        try {
          // Try parsing as ProblemDetails first
          const errorBody = await response.json();
          if (errorBody && (errorBody.title || errorBody.detail || errorBody.errors)) {
            // Handle validation errors specifically if present
            if (errorBody.errors && typeof errorBody.errors === 'object') {
              const validationErrors = Object.entries(errorBody.errors)
                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                .map(([field, messages]: [string, any]) => `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`)
                .join('; ');
              return `${errorBody.title || 'Validation Failed'}: ${validationErrors}`;
            }
            return errorBody.title || errorBody.detail || response.statusText || 'An API error occurred.';
          }
          // Fallback if not ProblemDetails or parsing fails
          return response.statusText || err.message || `API Error: ${response.status}`;

        } catch (jsonError) {
          console.error("Failed to parse error response:", jsonError);
          // If JSON parsing fails, return status text or generic message
          return response.statusText || err.message || `API request failed with status ${response.status}`;
        }
      }
      return err.message || 'An unknown error occurred.';
    }
    return 'An unexpected error occurred.';
  };

  // --- Fetch Products (Modified for Search/Sort) ---
  const fetchProducts = useCallback(async (
    currentPage = page,
    currentSize = rowsPerPage,
    currentSearch = searchTerm,
    currentSort = sortBy
  ) => {
    console.log(`Workspaceing: Page ${currentPage + 1}, Size ${currentSize}, Search '${currentSearch}', Sort '${currentSort}'`);
    setLoading(true);
    setError(null);
    const headers = await getAuthHeaders();
    if (!headers) {
      setLoading(false);
      setError("Authentication failed. Cannot fetch products.");
      setProducts([]); // Clear products if auth fails
      setTotalCount(0);
      return;
    }

    try {
      const response: ProductDetailsDtoPaginationList =
        await productsApi.apiV1ProductsGet(
          {
            pageNumber: currentPage + 1,
            pageSize: currentSize,
            searchTerm: currentSearch || undefined,
            sortBy: currentSort || undefined,
            isIncludeItems: true,
          },
          // { headers }
        );
      setProducts(response.items ?? []);
      setTotalCount(response.totalCount ?? 0);
      console.log('Fetched products:', response.items);
      getEditorContent();
    } catch (err: unknown) {
      console.error('Failed to fetch products:', err);
      const errorMessage = await parseApiError(err);
      setError(`Could not load products: ${errorMessage}`);
      setProducts([]); 
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  }, [page, rowsPerPage, searchTerm, sortBy, getAuthHeaders]); 

  // --- Fetch Categories for Dropdown ---
  const fetchCategories = useCallback(async () => {
    const headers = await getAuthHeaders();
    if (!headers) return; // Don't fetch if not authenticated
    try {
      const response = await categoriesApi.apiV1CategoriesGet(
        { pageSize: 1000 },
        { headers }
      );
      setCategories(response ?? []);
    } catch (err) {
      console.error('Failed to fetch categories:', err);
      setError(prev => prev ? `${prev}\nCould not load categories.` : 'Could not load categories.');
    }
  }, [getAuthHeaders]);

  // --- Initial Data Load ---
  useEffect(() => {
    fetchProducts();
    fetchCategories(); // Fetch categories on initial load
  }, [fetchProducts, fetchCategories]);

  // --- Search Handler ---
  const handleSearchChange = (event: ChangeEvent<HTMLInputElement>) => {
    const newSearchTerm = event.target.value;
    setSearchTerm(newSearchTerm);
  };

  // --- Sort Handler ---
  const handleSortChange = (event: SelectChangeEvent<string>) => {
    const newSortBy = event.target.value;
    setSortBy(newSortBy);
    setPage(0); // Reset to first page on sort change
    fetchProducts(0, rowsPerPage, searchTerm, newSortBy);
  };

  // --- Pagination Handlers ---
  const handleChangePage = (
    _event: React.MouseEvent<HTMLButtonElement> | null,
    newPage: number
  ): void => {
    setPage(newPage);
    // Fetch products for the new page with current search/sort
    fetchProducts(newPage, rowsPerPage, searchTerm, sortBy);
  };

  const handleChangeRowsPerPage = (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void => {
    const newSize = parseInt(event.target.value, 10);
    setRowsPerPage(newSize);
    setPage(0); // Reset to first page when changing size
    fetchProducts(0, newSize, searchTerm, sortBy);
  };

  // --- Dialog Handlers (Minor adjustments for Category) ---
  const handleOpenDialog = (product: ProductDetailsDto | null = null): void => {
    setEditingProduct(product);
    setFormData(
      product
        ? {
          name: product.name,
          description: product.description,
          price: product.price,
          details: product.details,
          categoryId: product.category?.id,
        }
        : { price: 0 }
    );
    setError(null); 
    setOpenDialog(true);
  };

  const handleCloseDialog = (): void => {
    setOpenDialog(false);
    setEditingProduct(null);
    setFormData({});
    setError(null); // Clear errors when closing dialog normally
  };

  const handleOpenConfirmDialog = (product: ProductDetailsDto): void => {
    setProductToDelete(product);
    setOpenConfirmDialog(true);
  };

  const handleCloseConfirmDialog = (): void => {
    setOpenConfirmDialog(false);
    setProductToDelete(null);
  };

  // --- Form Input Handler (Includes Select) ---
  const handleInputChange = (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string>
  ): void => {
    const { name, value } = event.target;
    const type = (event.target as HTMLInputElement).type;

    setFormData((prevData) => ({
      ...prevData,
      [name as string]: type === 'number' ? parseFloat(value) || 0 : value,
    }));
  };

  // --- Save Product ---
  const handleSaveProduct = async (): Promise<void> => {
    setLoading(true);
    setError(null);
    const headers = await getAuthHeaders();
    if (!headers) {
      setError("Authentication failed. Cannot save product.");
      setLoading(false);
      return;
    }

    // Basic Frontend Validation (Example)
    if (!formData.name || !formData.categoryId || formData.price === undefined || formData.price < 0) {
      setError('Please fill in all required fields (Name, Category, valid Price).');
      setLoading(false);
      return;
    }


    try {
      if (editingProduct?.id) {
        const updateCommand: UpdateProductCommand = {
          name: formData.name,
          description: formData.description,
          price: formData.price,
          details: formData.details,
          // categoryId is NOT updated here assuming API doesn't allow it or business rule
        };
        await productsApi.apiV1ProductsProductIdPut(
          {
            productId: editingProduct.id,
            updateProductCommand: updateCommand,
          },
          { headers }
        );
      } else {
        if (!formData.categoryId) {
          throw new Error("Category is required to create a product.");
        }
        const createCommand: CreateProductCommand = {
          name: formData.name!, 
          description: formData.description,
          price: formData.price!, 
          details: formData.details,
          categoryId: formData.categoryId!, 
        };
        await productsApi.apiV1ProductsPost(
          { createProductCommand: createCommand },
          { headers }
        );
      }
      handleCloseDialog();
      // Fetch the first page to see the new/updated item easily, or fetch current page
      setPage(0);
      fetchProducts(0, rowsPerPage, '', sortBy);
    } catch (err: unknown) {
      console.error('Failed to save product:', err);
      const errorMessage = await parseApiError(err);
      setError(`Save failed: ${errorMessage}`);
    } finally {
      setLoading(false);
    }
  };

  // --- Delete Product ---
  const handleDeleteConfirm = async (): Promise<void> => {
    if (!productToDelete?.id) return;
    setLoading(true);
    setError(null);
    const headers = await getAuthHeaders();
    if (!headers) {
      setError("Authentication failed. Cannot delete product.");
      setLoading(false);
      handleCloseConfirmDialog();
      return;
    }

    try {
      await productsApi.apiV1ProductsProductIdDelete(
        { productId: productToDelete.id },
        { headers }
      );
      handleCloseConfirmDialog();
      // After delete, fetch current page or previous page if it becomes empty
      const newTotalCount = totalCount - 1;
      const newTotalPages = Math.ceil(newTotalCount / rowsPerPage);
      const currentPageToGo = page >= newTotalPages ? Math.max(0, newTotalPages - 1) : page;
      setPage(currentPageToGo);
      fetchProducts(currentPageToGo, rowsPerPage, searchTerm, sortBy);
    } catch (err: unknown) {
      console.error('Failed to delete product:', err);
      const errorMessage = await parseApiError(err);
      setError(`Delete failed: ${errorMessage}`);
      handleCloseConfirmDialog();
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom sx={{ textAlign: 'center', fontWeight: 'bold', color: 'primary.main', mb: 3 }}>
        Danh sách Product
      </Typography>

      {/* --- Toolbar: Search, Sort, Add Button --- */}
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 2,
          flexWrap: 'wrap',
          gap: 2,
        }}
      >
        {/* --- Left side: Search and Sort --- */}
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          <TextField
            label="Search by name"
            variant="outlined"
            size="small"
            value={searchTerm}
            onChange={handleSearchChange}
            sx={{ minWidth: '200px' }}
          />
          <FormControl size="small" sx={{ minWidth: 220 }}>
            <InputLabel>Sort by</InputLabel>
            <Select
              value={sortBy}
              label="Sort by"
              onChange={handleSortChange}
            >
              <MenuItem value="name">Name (A-Z)</MenuItem>
              <MenuItem value="name desc">Name (Z-A)</MenuItem>
              <MenuItem value="price">Price (Low-High)</MenuItem>
              <MenuItem value="price desc">Price (High-Low)</MenuItem>
              {/* Add other sort options if API supports, e.g., createdDate */}
            </Select>
          </FormControl>
        </Box>

        {/* --- Right side: Add Button --- */}
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
          disabled={loading} // Disable button while loading list
        >
          Create Product
        </Button>
      </Box>

      {/* Global Loading Indicator and Error */}
      {loading && <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}><CircularProgress /></Box>}

      {/* Display error on main page only if not related to dialogs */}
      {error && !openDialog && !openConfirmDialog && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {/* --- Products Table --- */}
      <TableContainer component={Paper}>
        <Table stickyHeader aria-label="product table">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Name</TableCell>
              <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Description</TableCell>
              <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }} align="right">Price</TableCell>
              <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }}>Category</TableCell>
              <TableCell sx={{ fontWeight: 'bold', backgroundColor: 'primary.light', color: 'primary.contrastText' }} align="center">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading && !products.length ? (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <CircularProgress />
                </TableCell>
              </TableRow>
            ) : (
              products.length > 0 ? products.map((product) => (
                <TableRow hover role="checkbox" tabIndex={-1} key={product.id}>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>{product.name}</TableCell>
                  <TableCell>{product.description}</TableCell>
                  <TableCell align="right">
                    {product.price?.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }) ?? 'N/A'}
                  </TableCell>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>{product.category?.name ?? 'N/A'}</TableCell>
                  <TableCell align="center">
                    <IconButton
                      onClick={() => handleOpenDialog(product)}
                      color="primary"
                      aria-label={`edit ${product.name}`}
                      disabled={loading} 
                    >
                      <EditIcon />
                    </IconButton>
                    <IconButton
                      onClick={() => handleOpenConfirmDialog(product)}
                      color="error"
                      aria-label={`delete ${product.name}`}
                      disabled={loading} 
                    >
                      <DeleteIcon />
                    </IconButton>
                    <IconButton
                      onClick={() => navigate(`./details?productId=${product.id}`)}
                      color="secondary"
                      aria-label={`details`}
                      disabled={loading}
                    >
                      <InfoIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              )) : (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    {searchTerm ? `No products found matching "${searchTerm}".` : "No products available."}
                  </TableCell>
                </TableRow>
              )
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* --- Pagination --- */}
      {totalCount > 0 && (
        <TablePagination
          rowsPerPageOptions={[5, 10, 25, 50]} // Standard options
          component="div"
          count={totalCount}
          rowsPerPage={rowsPerPage}
          page={page} // 0-based index
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          // Show loading state on pagination as well
          SelectProps={{
            disabled: loading,
          }}
          backIconButtonProps={{
            disabled: loading || page === 0,
          }}
          nextIconButtonProps={{
            disabled: loading || page >= Math.ceil(totalCount / rowsPerPage) - 1,
          }}
        />
      )}

      {/* --- Create/Edit Dialog --- */}
      <Dialog
        open={openDialog}
        onClose={handleCloseDialog} // Allow closing by clicking outside
        aria-labelledby="product-dialog-title"
        maxWidth="lg"
        fullWidth
      >
        <DialogTitle id="product-dialog-title" component="h2">
          {editingProduct ? 'Edit Product' : 'Create New Product'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2}>
            <Grid size={{ xs: 6, md: 8 }}>
              <div>
                <div ref={editorRef}
                  dangerouslySetInnerHTML={{ __html: formData.details! }}
                >
                </div>
                <button onClick={getEditorContent}>Lấy Nội dung</button>
              </div>
            </Grid>
            <Grid size={{ xs: 6, md: 4 }}>
              <TextField
                autoFocus
                margin="dense"
                name="name"
                label="Product Name"
                type="text"
                fullWidth
                variant="outlined"
                value={formData.name ?? ''}
                onChange={handleInputChange}
                required
                disabled={loading} // Disable fields during save
              />
              {/* Category Select Dropdown */}
              <FormControl fullWidth margin="dense" required disabled={!!editingProduct || loading}>
                <InputLabel id="category-select-label">Category</InputLabel>
                <Select
                  labelId="category-select-label"
                  name="categoryId"
                  value={formData.categoryId ?? ''}
                  label="Category"
                  onChange={handleInputChange}
                >
                  <MenuItem value="" disabled>
                    <em>Select a category...</em>
                  </MenuItem>
                  {categories.map((cat) => (
                    <MenuItem key={cat.id} value={cat.id}>
                      {cat.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <TextField
                margin="dense"
                name="description"
                label="Description"
                type="text"
                fullWidth
                multiline
                rows={3} // Reduced rows
                variant="outlined"
                value={formData.description ?? ''}
                onChange={handleInputChange}
                disabled={loading}
              />
              <TextField
                margin="dense"
                name="price"
                label="Price"
                type="number"
                fullWidth
                variant="outlined"
                value={formData.price ?? 0}
                onChange={handleInputChange}
                required
                InputProps={{ inputProps: { min: 0, step: '0.01' } }}
                disabled={loading}
              />
            </Grid>

          </Grid>
          {/* Error specific to the dialog */}
          {error && openDialog && (
            <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
              {error}
            </Alert>
          )}

          {/* <TextField
            margin="dense"
            name="details"
            label="Details (Optional)" // Mark as optional if it is
            type="text"
            fullWidth
            multiline
            rows={3} // Reduced rows
            variant="outlined"
            value={formData.details ?? ''}
            onChange={handleInputChange}
            disabled={loading}
          /> */}

        </DialogContent>
        <DialogActions>
          {/* Show Loading indicator inside the button */}
          <Button onClick={handleCloseDialog} disabled={loading} color="inherit">Cancel</Button>
          <Button onClick={handleSaveProduct} variant="contained" disabled={loading}>
            {loading ? (
              <CircularProgress size={24} color="inherit" />
            ) : editingProduct ? (
              'Save Changes'
            ) : (
              'Create Product'
            )}
          </Button>
        </DialogActions>
      </Dialog>

      {/* --- Delete Confirmation Dialog (Using MUI Dialog instead of window.confirm) --- */}
      <Dialog
        open={openConfirmDialog}
        onClose={handleCloseConfirmDialog}
        aria-labelledby="alert-dialog-confirm-delete-title"
        aria-describedby="alert-dialog-confirm-delete-description"
      >
        <DialogTitle id="alert-dialog-confirm-delete-title">
          Confirm Deletion
        </DialogTitle>
        <DialogContent>
          <Typography id="alert-dialog-confirm-delete-description">
            Bảnh có chắc muốn xóa sản phẩm "
            {productToDelete?.name ?? 'này'}" không? Hành động này không thể hoàn tác.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseConfirmDialog} disabled={loading} color="inherit">Hủy</Button>
          <Button
            onClick={handleDeleteConfirm}
            color="error"
            variant="contained" // Make delete more prominent
            autoFocus
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} color="inherit" /> : 'Xóa'}
          </Button>
        </DialogActions>
      </Dialog>

    </Container>
  );
};

export default ProductManagement;