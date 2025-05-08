// src/hooks/useProductVariantManagement.ts
import {
    useState,
    useEffect,
    useCallback,
    ChangeEvent,
} from 'react';
import { SelectChangeEvent } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import {
    Configuration,
    ProductVariantsApi,
    ProductVariantDetailsDto,
    ProductVariantDetailsDtoPaginationList,
    CreateVariantCommand,
    UpdateVariantCommand,
} from '../api'; 
import { authService } from '../auth/AuthConfig';

interface UseProductVariantManagementProps {
    productId: string | null;
}


const apiConfig = new Configuration({ basePath: import.meta.env.VITE_BASE_PATH_API });
const productVariantsApi = new ProductVariantsApi(apiConfig);

export const useProductVariantManagement = ({ productId }: UseProductVariantManagementProps) => {
    const navigate = useNavigate();

    // --- State ---
    const [variants, setVariants] = useState<ProductVariantDetailsDto[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [openDialog, setOpenDialog] = useState<boolean>(false);
    const [editingVariant, setEditingVariant] = useState<ProductVariantDetailsDto | null>(null);
    const [formData, setFormData] = useState<Partial<CreateVariantCommand & UpdateVariantCommand>>({});
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [totalCount, setTotalCount] = useState(0);
    const [openConfirmDialog, setOpenConfirmDialog] = useState(false);
    const [variantToDelete, setVariantToDelete] = useState<ProductVariantDetailsDto | null>(null);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [sortBy, setSortBy] = useState<string>('name'); 
    const [minPrice, setMinPrice] = useState<string>(""); 
    const [maxPrice, setMaxPrice] = useState<string>(""); 
    const [appliedMinPrice, setAppliedMinPrice] = useState<number | undefined>();
    const [appliedMaxPrice, setAppliedMaxPrice] = useState<number | undefined>();

    // --- Authentication & Error Parsing ---
    const getAuthHeaders = useCallback(async (): Promise<Record<string, string> | null> => {
        try {
            const token = await authService.getAccessToken();
            if (!token) {
                console.error('Authentication token not found.');
                navigate('/login');
                return null;
            }
            return { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' };
        } catch (authError) {
            console.error('Error getting access token:', authError);
            setError('Failed to get authentication token.');
            return null;
        }
    }, [navigate]);

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

    // --- Fetch Variants Logic ---
    const fetchVariantsInternal = useCallback(async (
        pId: string,
        currentPage: number,
        currentSize: number,
        currentSearch: string,
        currentSort: string,
        currentMinPrice?: number,
        currentMaxPrice?: number
    ) => {
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setLoading(false);
            setVariants([]);
            setTotalCount(0);
            setError("Authentication failed. Cannot fetch variants.");
            return;
        }
        try {
            const response: ProductVariantDetailsDtoPaginationList =
                await productVariantsApi.apiV1ProductVariantsGet(
                    {
                        productId: pId,
                        pageNumber: currentPage + 1,
                        pageSize: currentSize,
                        searchTerm: currentSearch || undefined,
                        sortBy: currentSort || undefined,
                        minPrice: currentMinPrice,
                        maxPrice: currentMaxPrice,
                    },
                    { headers }
                );
            setVariants(response.items ?? []);
            setTotalCount(response.totalCount ?? 0);
        } catch (err: unknown) {
            const errorMessage = await parseApiError(err);
            setError(`Could not load variants: ${errorMessage}`);
            setVariants([]);
            setTotalCount(0);
        } finally {
            setLoading(false);
        }
    }, [getAuthHeaders, parseApiError]);

    // --- Effect for Initial Load & Dependency Changes ---
    useEffect(() => {
        if (productId) {
            fetchVariantsInternal(productId, page, rowsPerPage, searchTerm, sortBy, appliedMinPrice, appliedMaxPrice);
        } else {
            setVariants([]);
            setTotalCount(0);
        }
    }, [productId, page, rowsPerPage, searchTerm, sortBy, appliedMinPrice, appliedMaxPrice, fetchVariantsInternal]);


    // --- Search, Sort & Filter Handlers ---
    const handleSearchChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
        setPage(0);
    }, []);

    const handleSortChange = useCallback((event: SelectChangeEvent<string>) => {
        setSortBy(event.target.value);
        setPage(0); 
    }, []);

    const handleMinPriceChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        setMinPrice(event.target.value);
    }, []);

    const handleMaxPriceChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        setMaxPrice(event.target.value);
    }, []);

    const handleApplyFilters = useCallback(() => {
        const minP = minPrice ? parseFloat(minPrice) : undefined;
        const maxP = maxPrice ? parseFloat(maxPrice) : undefined;

        if ((minPrice && Number.isNaN(minP)) || (maxPrice && Number.isNaN(maxP))) {
            setError("Giá trị min/max price không hợp lệ.");
            return;
        }
        setAppliedMinPrice(minP);
        setAppliedMaxPrice(maxP);
        setPage(0);
    }, [minPrice, maxPrice]);

    const handleClearFilters = useCallback(() => {
        setMinPrice('');
        setMaxPrice('');
        setAppliedMinPrice(undefined);
        setAppliedMaxPrice(undefined);
        setPage(0);
    }, []);

    // --- Pagination Handlers ---
    const handleChangePage = useCallback((
        _event: React.MouseEvent<HTMLButtonElement> | null,
        newPage: number
    ) => {
        setPage(newPage);
    }, []);

    const handleChangeRowsPerPage = useCallback((
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0); 
    }, []);

    // --- Dialog Handlers ---
    const handleOpenDialog = useCallback((variant: ProductVariantDetailsDto | null = null): void => {
        setEditingVariant(variant);
        setFormData(
            variant
                ? { name: variant.name, price: variant.price, stockQuantity: variant.stockQuantity, variantType: variant.variantType }
                : { price: 0, stockQuantity: 0 }
        );
        setError(null); 
        setOpenDialog(true);
    }, []);

    const handleCloseDialog = useCallback((): void => {
        setOpenDialog(false);
        setEditingVariant(null);
        setFormData({});
        setError(null); 
    }, []);

    const handleOpenConfirmDialog = useCallback((variant: ProductVariantDetailsDto): void => {
        setVariantToDelete(variant);
        setOpenConfirmDialog(true);
    }, []);

    const handleCloseConfirmDialog = useCallback((): void => {
        setOpenConfirmDialog(false);
        setVariantToDelete(null);
    }, []);

    // --- Form Input Handler ---
    const handleInputChange = useCallback((
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string>
    ): void => {
        const { name, value } = event.target;
        const type = (event.target as HTMLInputElement).type;
        setFormData((prevData) => ({
            ...prevData,
            [name as string]: type === 'number' ? (parseFloat(value) || 0) : value,
        }));
    }, []);

    // --- Save Variant ---
    const handleSaveVariant = useCallback(async (): Promise<void> => {
        if (!productId) {
            setError("Product ID is missing. Cannot save variant.");
            return;
        }
        setLoading(true);

        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot save variant.");
            setLoading(false);
            return;
        }

        if (!formData.name || formData.price === undefined || formData.price < 0 || formData.stockQuantity === undefined || formData.stockQuantity < 0) {
            setError('Please fill in all required fields (Name, valid Price, valid Stock).');
            setLoading(false);
            return;
        }

        try {
            if (editingVariant?.id) {
                const updateCommand: UpdateVariantCommand = {
                    name: formData.name,
                    price: formData.price,
                    stockQuantity: formData.stockQuantity,
                };
                await productVariantsApi.apiV1ProductVariantsVariantIdPut(
                    { variantId: editingVariant.id, updateVariantCommand: updateCommand },
                    { headers } 
                );
            } else {
                const createCommand: CreateVariantCommand = {
                    productId: productId,
                    name: formData.name!,
                    price: formData.price!,
                    stockQuantity: formData.stockQuantity!,
                    variantType: formData.variantType,
                };
                await productVariantsApi.apiV1ProductVariantsPost(
                    { createVariantCommand: createCommand },
                    { headers }
                );
            }
            handleCloseDialog();

            fetchVariantsInternal(productId, page, rowsPerPage, searchTerm, sortBy, appliedMinPrice, appliedMaxPrice);
            setPage(0);
        } catch (err: unknown) {
            const errorMessage = await parseApiError(err);
            setError(`Save failed: ${errorMessage}`);
        } finally {
            setLoading(false);
        }
    }, [productId, formData, editingVariant, getAuthHeaders, parseApiError, handleCloseDialog, fetchVariantsInternal, page, rowsPerPage, searchTerm, sortBy, appliedMinPrice, appliedMaxPrice]);

    // --- Delete Variant ---
    const handleDeleteConfirm = useCallback(async (): Promise<void> => {
        if (!variantToDelete?.id || !productId) return;

        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot delete variant.");
            setLoading(false);
            handleCloseConfirmDialog();
            return;
        }

        try {
            await productVariantsApi.apiV1ProductVariantsVariantIdDelete(
                { variantId: variantToDelete.id },
                { headers }
            );
            handleCloseConfirmDialog();
            const newTotalCount = totalCount - 1;
            const newTotalPages = Math.ceil(newTotalCount / rowsPerPage);
            const currentPageToGo = page >= newTotalPages ? Math.max(0, newTotalPages - 1) : page;

            if (page === currentPageToGo) {
                fetchVariantsInternal(productId, currentPageToGo, rowsPerPage, searchTerm, sortBy, appliedMinPrice, appliedMaxPrice);
            } else {
                setPage(currentPageToGo); 
            }

        } catch (err: unknown) {
            const errorMessage = await parseApiError(err);
            setError(`Delete failed: ${errorMessage}`);
            handleCloseConfirmDialog();
        } finally {
            setLoading(false);
        }
    }, [
        variantToDelete,
        productId,
        getAuthHeaders,
        parseApiError,
        handleCloseConfirmDialog,
        totalCount,
        rowsPerPage,
        page,
        searchTerm,
        sortBy,
        appliedMinPrice,
        appliedMaxPrice,
        fetchVariantsInternal
    ]);

    // --- Return values ---
    return {
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
        minPrice, 
        maxPrice, 
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
    };
};