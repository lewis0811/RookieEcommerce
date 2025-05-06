import React, { useState, useEffect, useCallback, useRef, ChangeEvent } from 'react';
import { SelectChangeEvent } from '@mui/material';
import {
    ProductDetailsDto, CreateProductCommand, UpdateProductCommand, Category,
    ProductsApi, CategoriesApi, Configuration, ProductDetailsDtoPaginationList
} from '../api';
import { authService } from '../auth/AuthConfig';
import $ from 'jquery';

// --- API Instances ---
const apiConfig = new Configuration({ basePath: import.meta.env.VITE_BASE_PATH_API });
const productsApi = new ProductsApi(apiConfig);
const categoriesApi = new CategoriesApi(apiConfig);

interface UseProductManagementReturn {
    products: ProductDetailsDto[];
    categories: Category[];
    loading: boolean;
    error: string | null;
    openDialog: boolean;
    editingProduct: ProductDetailsDto | null;
    formData: Partial<CreateProductCommand & UpdateProductCommand>;
    page: number;
    rowsPerPage: number;
    totalCount: number;
    openConfirmDialog: boolean;
    productToDelete: ProductDetailsDto | null;
    searchTerm: string;
    sortBy: string;
    editorRef: React.RefObject<HTMLDivElement | null>;
    setError: React.Dispatch<React.SetStateAction<string | null>>;
    setFormData: React.Dispatch<React.SetStateAction<Partial<CreateProductCommand & UpdateProductCommand>>>;
    fetchProducts: (currentPage?: number, currentSize?: number, currentSearch?: string, currentSort?: string) => Promise<void>;
    fetchCategories: () => Promise<void>;
    handleOpenDialog: (product?: ProductDetailsDto | null) => void;
    handleCloseDialog: () => void;
    handleOpenConfirmDialog: (product: ProductDetailsDto) => void;
    handleCloseConfirmDialog: () => void;
    handleInputChange: (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string>) => void;
    handleSaveProduct: () => Promise<void>;
    handleDeleteConfirm: () => Promise<void>;
    handleChangePage: (_event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => void;
    handleChangeRowsPerPage: (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
    handleSearchChange: (event: ChangeEvent<HTMLInputElement>) => void;
    handleSortChange: (event: SelectChangeEvent<string>) => void;
}


export const useProductManagement = (): UseProductManagementReturn => {
    // --- States ---
    const [products, setProducts] = useState<ProductDetailsDto[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [openDialog, setOpenDialog] = useState<boolean>(false);
    const [editingProduct, setEditingProduct] = useState<ProductDetailsDto | null>(null);
    const [formData, setFormData] = useState<Partial<CreateProductCommand & UpdateProductCommand>>({});
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [totalCount, setTotalCount] = useState(0);
    const [openConfirmDialog, setOpenConfirmDialog] = useState(false);
    const [productToDelete, setProductToDelete] = useState<ProductDetailsDto | null>(null);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [sortBy, setSortBy] = useState<string>('name');
    const editorRef = useRef<HTMLDivElement>(null);

    // --- Auth & Error Parsing ---
    const getAuthHeaders = useCallback(async (): Promise<Record<string, string> | null> => {
        try {
            const token = await authService.getAccessToken();
            if (!token) {
                console.error('Authentication token not found.');
                return null;
            }
            return { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' };
        } catch (authError) {
            console.error('Error getting access token:', authError);
            return null;
        }
    }, []);

    const parseApiError = useCallback(async (err: unknown): Promise<string> => {
        if (err instanceof Error) {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            const response = (err as any).response as Response | undefined;
            if (response) {
                try {
                    const errorBody = await response.json();
                    if (errorBody && (errorBody.title || errorBody.detail || errorBody.errors)) {
                        if (errorBody.errors && typeof errorBody.errors === 'object') {
                            const validationErrors = Object.entries(errorBody.errors)
                                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                                .map(([field, messages]: [string, any]) => `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`)
                                .join('; ');
                            return `${errorBody.title || 'Validation Failed'}: ${validationErrors}`;
                        }
                        return errorBody.title || errorBody.detail || response.statusText || 'An API error occurred.';
                    }
                    return response.statusText || err.message || `API Error: ${response.status}`;
                } catch (jsonError) {
                    console.error("Failed to parse error response:", jsonError);
                    return response.statusText || err.message || `API request failed with status ${response.status}`;
                }
            }
            return err.message || 'An unknown error occurred.';
        }
        return 'An unexpected error occurred.';
    }, []);

    // --- Summernote Logic ---
    const getEditorContent = useCallback(() => {
        if (editorRef.current && $(editorRef.current).hasClass('note-editor')) {
            return $(editorRef.current).summernote('code');
        }
        return '';
    }, []);

    useEffect(() => {
        const initializeEditor = () => {
            if (editorRef.current) {

                setTimeout(() => {
                    if (editorRef.current && !$(editorRef.current).hasClass('note-editor')) {
                        $(editorRef.current).summernote({
                            placeholder: 'Nhập chi tiết sản phẩm ở đây...',
                            height: 300,
                            minHeight: 150,
                            maxHeight: 500,
                            focus: false,
                            callbacks: {
                                onChange: function (contents) {
                                    setFormData((prev: Partial<CreateProductCommand & UpdateProductCommand>) =>
                                        ({ ...prev, details: contents }));
                                }
                            }
                        });

                        if (formData.details) {
                            $(editorRef.current).summernote('code', formData.details);
                        } else {
                            $(editorRef.current).summernote('code', '');
                        }
                    } else if (editorRef.current && $(editorRef.current).hasClass('note-editor')) {
                        if (formData.details) {
                            $(editorRef.current).summernote('code', formData.details);
                        } else {
                            $(editorRef.current).summernote('code', '');
                        }
                    }
                }, 200);
            }
        };

        const destroyEditor = () => {
            if (editorRef.current && $(editorRef.current).hasClass('note-editor')) {
                try {
                    $(editorRef.current).summernote('destroy');
                } catch (e) {
                    console.warn("Error destroying Summernote:", e);
                }
            }
        };

        if (openDialog) {
            initializeEditor();
        } else {
            destroyEditor();
        }

        return () => {
            destroyEditor();
        };
    }, [openDialog, formData.details]);


    // --- Data Fetching ---
    const fetchProducts = useCallback(async (
        currentPage = page,
        currentSize = rowsPerPage,
        currentSearch = searchTerm,
        currentSort = sortBy
    ) => {
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot fetch products.");
            setProducts([]);
            setTotalCount(0);
            setLoading(false);
            return;
        }
        try {
            const response: ProductDetailsDtoPaginationList = await productsApi.apiV1ProductsGet(
                {
                    pageNumber: currentPage + 1,
                    pageSize: currentSize,
                    searchTerm: currentSearch || undefined,
                    sortBy: currentSort || undefined,
                    isIncludeItems: true,
                },
                { headers }
            );
            setProducts(response.items ?? []);
            setTotalCount(response.totalCount ?? 0);
        } catch (err) {
            const errorMessage = await parseApiError(err);
            setError(`Could not load products: ${errorMessage}`);
            setProducts([]);
            setTotalCount(0);
        } finally {
            setLoading(false);
        }
    }, [page, rowsPerPage, searchTerm, sortBy, getAuthHeaders, parseApiError]);

    const fetchCategories = useCallback(async () => {
        const headers = await getAuthHeaders();
        if (!headers) {
            return;
        }
        try {
            const response = await categoriesApi.apiV1CategoriesGet(
                { pageSize: 1000 },
                { headers }
            );
            setCategories(response ?? []);
        } catch (err) {
            const errorMessage = await parseApiError(err);
            setError(prev => prev ? `${prev}\nCould not load categories: ${errorMessage}` : `Could not load categories: ${errorMessage}`);
        } finally {
            setLoading(false);
        }
    }, [getAuthHeaders, parseApiError]);

    // --- Initial Load ---
    useEffect(() => {
        fetchProducts();
        fetchCategories();
    }, [fetchProducts, fetchCategories]);


    // --- Event Handlers ---
    const handleOpenDialog = useCallback((product: ProductDetailsDto | null = null) => {
        setEditingProduct(product);
        const initialDetails = product?.details || '';
        setFormData(
            product
                ? {
                    name: product.name,
                    description: product.description,
                    price: product.price,
                    details: initialDetails,
                    categoryId: product.category?.id,
                }
                : { price: 0, details: '' }
        );
        setError(null);
        setOpenDialog(true);
    }, []);

    const handleCloseDialog = useCallback(() => {
        setOpenDialog(false);
        setEditingProduct(null);
        setFormData({});
        setError(null);
    }, []);


    const handleInputChange = useCallback((
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string>
    ) => {
        const target = event.target;
        const name = target.name;
        const value = target.value;
        const type = 'type' in target ? (target as HTMLInputElement).type : 'text';

        setFormData((prevData: Partial<CreateProductCommand | UpdateProductCommand>) => ({
            ...prevData,
            [name as string]: type === 'number' ? parseFloat(value) || 0 : value,
        }));
    }, []); 

    const handleSaveProduct = useCallback(async () => {
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot save product.");
            setLoading(false);
            return;
        }

        const currentEditorDetails = getEditorContent();

        const detailsToSave = typeof currentEditorDetails === 'string' ? currentEditorDetails : formData.details;

        const dataToSave = { ...formData, details: detailsToSave };

        if (!dataToSave.name || !dataToSave.categoryId || dataToSave.price === undefined || dataToSave.price < 0) {
            setError('Please fill in all required fields (Name, Category, valid Price).');
            setLoading(false);
            return;
        }

        try {
            if (editingProduct?.id) {
                const updateCommand: UpdateProductCommand = {
                    name: dataToSave.name,
                    description: dataToSave.description,
                    price: dataToSave.price,
                    details: dataToSave.details,
                };
                await productsApi.apiV1ProductsProductIdPut(
                    { productId: editingProduct.id, updateProductCommand: updateCommand },
                    { headers }
                );
            } else {
                if (!dataToSave.categoryId) {
                    throw new Error("Category is required to create a product.");
                }
                const createCommand: CreateProductCommand = {
                    name: dataToSave.name!,
                    description: dataToSave.description,
                    price: dataToSave.price!,
                    details: dataToSave.details,
                    categoryId: dataToSave.categoryId!,
                };
                await productsApi.apiV1ProductsPost(
                    { createProductCommand: createCommand },
                    { headers }
                );
            }
            handleCloseDialog();
            setPage(0);
            fetchProducts(0, rowsPerPage, '', sortBy);
        } catch (err) {
            const errorMessage = await parseApiError(err);
            setError(`Save failed: ${errorMessage}`);
        } finally {
            setLoading(false);
        }
    }, [formData, editingProduct, getAuthHeaders, parseApiError, handleCloseDialog, fetchProducts, getEditorContent, rowsPerPage, sortBy]); //Thêm dependencies

    const handleOpenConfirmDialog = useCallback((product: ProductDetailsDto) => {
        setProductToDelete(product);
        setOpenConfirmDialog(true);
    }, []);

    const handleCloseConfirmDialog = useCallback(() => {
        setOpenConfirmDialog(false);
        setProductToDelete(null); 
        setError(null);
    }, []);

    const handleDeleteConfirm = useCallback(async () => {
        if (!productToDelete?.id) return;
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot delete product.");
            setLoading(false);
            return;
        }

        try {
            await productsApi.apiV1ProductsProductIdDelete(
                { productId: productToDelete.id },
                { headers }
            );
            handleCloseConfirmDialog();

            const newTotalCount = totalCount - 1;
            const newTotalPages = Math.ceil(newTotalCount / rowsPerPage);
            const currentPageToGo = page >= newTotalPages ? Math.max(0, newTotalPages - 1) : page;
            setPage(currentPageToGo);
            fetchProducts(currentPageToGo, rowsPerPage, searchTerm, sortBy);
        } catch (err) {
            const errorMessage = await parseApiError(err);
            setError(`Delete failed: ${errorMessage}`);
        } finally {
            setLoading(false);
        }
    }, [productToDelete, getAuthHeaders, parseApiError, handleCloseConfirmDialog, totalCount, page, rowsPerPage, searchTerm, sortBy, fetchProducts]);


    const handleSearchChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
        const newSearchTerm = event.target.value;
        setSearchTerm(newSearchTerm);
        setPage(0); 
        fetchProducts(0, rowsPerPage, newSearchTerm, sortBy);
    }, [rowsPerPage, sortBy, fetchProducts]);

    const handleSortChange = useCallback((event: SelectChangeEvent<string>) => {
        const newSortBy = event.target.value;
        setSortBy(newSortBy);
        setPage(0);
        fetchProducts(0, rowsPerPage, searchTerm, newSortBy);
    }, [rowsPerPage, searchTerm, fetchProducts]);

    const handleChangePage = useCallback((
        _event: React.MouseEvent<HTMLButtonElement> | null,
        newPage: number
    ) => {
        setPage(newPage);
    }, []);

    useEffect(() => {
    }, [page, rowsPerPage, searchTerm, sortBy, fetchProducts]);


    const handleChangeRowsPerPage = useCallback((
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        const newSize = parseInt(event.target.value, 10);
        setRowsPerPage(newSize);
        setPage(0);
    }, []);


    // --- Return ---
    return {
        products,
        categories,
        loading,
        error,
        setError, 
        openDialog,
        editingProduct,
        formData,
        setFormData,
        page,
        rowsPerPage,
        totalCount,
        openConfirmDialog,
        productToDelete,
        searchTerm,
        sortBy,
        editorRef,
        fetchProducts, 
        fetchCategories,
        handleOpenDialog,
        handleCloseDialog,
        handleOpenConfirmDialog,
        handleCloseConfirmDialog,
        handleInputChange,
        handleSaveProduct,
        handleDeleteConfirm,
        handleChangePage,
        handleChangeRowsPerPage,
        handleSearchChange,
        handleSortChange,
    };
};