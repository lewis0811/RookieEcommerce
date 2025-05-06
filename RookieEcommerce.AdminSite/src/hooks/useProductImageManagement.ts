import { useState, useEffect, useCallback, useRef, ChangeEvent } from 'react';
import {
    Configuration,
    CreateProductImageCommand,
    ProductImageDetailsDto,
    ProductImageDetailsDtoPaginationList,
    ProductImagesApi,
    UpdateProductImageCommand,
} from '../api';
import { authService } from '../auth/AuthConfig';
import { BASE_PATH_API } from '../config/api';

// ----- CLOUDINARY CONFIGURATION -----
const CLOUDINARY_CLOUD_NAME = import.meta.env.VITE_REACT_APP_CLOUDINARY_CLOUD_NAME;
const CLOUDINARY_UPLOAD_PRESET = import.meta.env.VITE_REACT_APP_CLOUDINARY_UPLOAD_PRESET;
const CLOUDINARY_UPLOAD_URL = `https://api.cloudinary.com/v1_1/${CLOUDINARY_CLOUD_NAME}/image/upload`;
// --------------------------------------------

const apiConfig = new Configuration({ basePath: BASE_PATH_API });
const productImagesApi = new ProductImagesApi(apiConfig);

interface UseProductImageManagementReturn {
    productImages: ProductImageDetailsDto[];
    loading: boolean;
    error: string | null;
    uploading: boolean;
    editingImageId: string | null;
    editFormData: { altText: string; sortOrder: string };
    imagePreview: string | null;
    fileToUpload: File | null;
    imageToDelete: ProductImageDetailsDto | null;
    openUploadDialog: boolean;
    openConfirmDialog: boolean;
    fetchProductImages: () => Promise<void>;
    handleOpenUploadDialog: () => void;
    handleCloseUploadDialog: () => void;
    handleFileSelect: (event: ChangeEvent<HTMLInputElement>) => void;
    handleSaveImage: () => Promise<void>;
    handleOpenConfirmDialog: (image: ProductImageDetailsDto) => void;
    handleCloseConfirmDialog: () => void;
    handleDeleteImageConfirm: () => Promise<void>;
    handleSetPrimaryImage: (imageId: string) => Promise<void>;
    handleStartEdit: (image: ProductImageDetailsDto) => void;
    handleCancelEdit: () => void;
    handleEditFormChange: (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
    handleSaveChanges: () => Promise<void>;
    setError: React.Dispatch<React.SetStateAction<string | null>>;
}

export const useProductImageManagement = (productId: string | null): UseProductImageManagementReturn => {
    // --- State for image ---
    const [productImages, setProductImages] = useState<ProductImageDetailsDto[]>([]);
    const [imageToDelete, setImageToDelete] = useState<ProductImageDetailsDto | null>(null);

    // --- State for common use ---
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [openUploadDialog, setOpenUploadDialog] = useState<boolean>(false);
    const [openConfirmDialog, setOpenConfirmDialog] = useState<boolean>(false);

    // --- State cho việc upload ---
    const [fileToUpload, setFileToUpload] = useState<File | null>(null);
    const [imagePreview, setImagePreview] = useState<string | null>(null);
    const [uploading, setUploading] = useState<boolean>(false);

    // --- State for update ---
    const [editingImageId, setEditingImageId] = useState<string | null>(null);
    const [editFormData, setEditFormData] = useState<{ altText: string; sortOrder: string }>({ altText: '', sortOrder: '0' });
    const editingImageIsPrimary = useRef<boolean>(false);


    // --- Get Auth Header ---
    const getAuthHeaders = useCallback(async (): Promise<Record<string, string> | null> => {
        try {
            const token = await authService.getAccessToken();
            if (!token) {
                console.error('Authentication token not found.');
                return null;
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
    }, []);

    // --- Api Error Handle ---
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

    // --- Fetch Product Images ---
    const fetchProductImages = useCallback(async () => {
        if (!productId) {
            setError("Product ID is missing.");
            setProductImages([]);
            return;
        }
        console.log(`Workspaceing images for product: ${productId}`);
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setLoading(false);
            setError("Authentication failed. Cannot fetch images.");
             setProductImages([]);
            return;
        }

        try {
            const response: ProductImageDetailsDtoPaginationList =
                await productImagesApi.apiV1ProductImagesGet(
                    { productId: productId, pageSize: 100, },
                    { headers }
                );
            setProductImages(response.items ?? []);
            console.log('Fetched images:', response.items);
        } catch (err: unknown) {
            console.error('Failed to fetch product images:', err);
            const errorMessage = await parseApiError(err);
            setError(`Could not load images: ${errorMessage}`);
            setProductImages([]);
        } finally {
            setLoading(false);
        }
    
    }, [productId, getAuthHeaders, parseApiError]);
    
    useEffect(() => {
        fetchProductImages();
    }, [fetchProductImages]);

    // --- Dialog Upload Handlers ---
    const handleOpenUploadDialog = useCallback((): void => {
        setFileToUpload(null);
        setImagePreview(null);
        setError(null);
        setOpenUploadDialog(true);
    }, []);

    const handleCloseUploadDialog = useCallback((): void => {
        setOpenUploadDialog(false);
    }, []);

    // --- File Select Handlers ---
    const handleFileSelect = useCallback((event: ChangeEvent<HTMLInputElement>): void => {
        const file = event.target.files?.[0];
        if (file) {
            setFileToUpload(file);
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result as string);
            };
            reader.readAsDataURL(file);
        } else {
            setFileToUpload(null);
            setImagePreview(null);
        }
    }, []); 

    // --- Upload Image ---
    const uploadImageToCloudinary = useCallback(async (file: File): Promise<string> => {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET!);

        console.log('Uploading to Cloudinary...');
        const response = await fetch(CLOUDINARY_UPLOAD_URL, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Cloudinary upload failed:', errorData);
            throw new Error(errorData.error?.message || 'Cloudinary upload failed');
        }

        const data = await response.json();
        console.log('Cloudinary upload successful:', data);
        return data.secure_url;
    }, []);

    // --- Handle save image 
    const handleSaveImage = useCallback(async (): Promise<void> => {
        if (!fileToUpload || !productId) {
            setError('Please select an image file.');
            return;
        }
        if (!CLOUDINARY_CLOUD_NAME || !CLOUDINARY_UPLOAD_PRESET) {
            setError('Cloudinary configuration is missing.');
            return;
        }

        setUploading(true);
        setError(null);

        try {
            const imageUrl = await uploadImageToCloudinary(fileToUpload);
            const headers = await getAuthHeaders();
            if (!headers) {
                throw new Error("Authentication failed. Cannot save image.");
            }

            const createCommand: CreateProductImageCommand = {
                productId: productId,
                imageUrl: imageUrl,
                altText: fileToUpload.name,
            };

            console.log('Sending create command to API:', createCommand);
            await productImagesApi.apiV1ProductImagesPost(
                { createProductImageCommand: createCommand },
                { headers }
            );

            console.log('API create successful');
            handleCloseUploadDialog();
            await fetchProductImages();

        } catch (err: unknown) {
            console.error('Failed to save product image:', err);
            if (err instanceof Error && err.message.includes('Cloudinary')) {
                 setError(`Save failed: ${err.message}`);
            } else {
                const errorMessage = await parseApiError(err);
                setError(`Save failed: ${errorMessage}`);
            }
        } finally {
            setUploading(false);
        }
    }, [productId, fileToUpload, getAuthHeaders, parseApiError, uploadImageToCloudinary, handleCloseUploadDialog, fetchProductImages]);

    // --- Handle Dialog Delete ---
    const handleOpenConfirmDialog = useCallback((image: ProductImageDetailsDto): void => {
        setImageToDelete(image);
        setError(null);
        setOpenConfirmDialog(true);
    }, []);

    const handleCloseConfirmDialog = useCallback((): void => {
        setOpenConfirmDialog(false);
        setImageToDelete(null);
    }, []);

    // --- Handle Confirm Delete ---
    const handleDeleteImageConfirm = useCallback(async (): Promise<void> => {
        if (!imageToDelete?.id) return;

        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setError("Authentication failed. Cannot delete image.");
            setLoading(false);
            handleCloseConfirmDialog();
            return;
        }

        try {
            await productImagesApi.apiV1ProductImagesImageIdDelete(
                { imageId: imageToDelete.id },
                { headers }
            );
            handleCloseConfirmDialog();
            await fetchProductImages();
        } catch (err: unknown) {
            console.error('Failed to delete product image:', err);
            const errorMessage = await parseApiError(err);
            setError(`Delete failed: ${errorMessage}`);
            handleCloseConfirmDialog();
        } finally {
            setLoading(false);
        }
    }, [imageToDelete, getAuthHeaders, parseApiError, handleCloseConfirmDialog, fetchProductImages]);

    // --- Handle Set Primary Button ---
    const handleSetPrimaryImage = useCallback(async (imageId: string): Promise<void> => {
        setLoading(true);
        setError(null);
        // Get existing image data to avoid overwriting other fields accidentally
        const currentImage = productImages.find(img => img.id === imageId);
        if (!currentImage) {
             setError("Image not found to set as primary.");
             setLoading(false);
             return;
        }
        const headers = await getAuthHeaders();
        if (!headers) {
            setLoading(false);
            setError("Authentication failed. Cannot fetch images.");
             setProductImages([]);
            return;
        }

        const updateCommand: UpdateProductImageCommand = {
             isPrimary: true,
             altText: currentImage.altText,
             sortOrder: currentImage.sortOrder
        };

        try {
            await productImagesApi.apiV1ProductImagesImageIdPut({
                imageId: imageId,
                updateProductImageCommand: updateCommand
            }, {headers});
            await fetchProductImages();
        } catch (err: unknown) {
            console.error(`Failed to set image ${imageId} as primary:`, err);
            const errorMessage = await parseApiError(err);
            setError(`Failed to update primary status: ${errorMessage}`);
        } finally {
            setLoading(false);
        }

    }, [productImages, parseApiError, fetchProductImages, getAuthHeaders]);

    // --- Edit Handlers ---
    const handleStartEdit = useCallback((image: ProductImageDetailsDto): void => {
        setEditingImageId(image.id!);
        setEditFormData({
            altText: image.altText || '',
            sortOrder: (image.sortOrder ?? 0).toString(),
        });
        editingImageIsPrimary.current = image.isPrimary ?? false;
        setError(null);
    }, []);

    const handleCancelEdit = useCallback((): void => {
        setEditingImageId(null);
        setError(null);
    }, []);

    const handleEditFormChange = useCallback((event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>): void => {
        const { name, value } = event.target;
        setEditFormData(prev => ({
            ...prev,
            [name]: value,
        }));
    }, []);

    const handleSaveChanges = useCallback(async (): Promise<void> => {
        if (!editingImageId) return;

        console.log(`Saving changes for image ${editingImageId}`);
        setLoading(true);
        setError(null);
        const headers = await getAuthHeaders();
        if (!headers) {
            setLoading(false);
            setError("Authentication failed. Cannot fetch images.");
             setProductImages([]);
            return;
        }

        const sortOrderNum = parseInt(editFormData.sortOrder, 10);
        const updateCommand: UpdateProductImageCommand = {
            altText: editFormData.altText,
            sortOrder: isNaN(sortOrderNum) ? 0 : sortOrderNum,
            isPrimary: editingImageIsPrimary.current,
        };

        
        try {
            await productImagesApi.apiV1ProductImagesImageIdPut({
                imageId: editingImageId,
                updateProductImageCommand: updateCommand
            }, {headers});
            console.log(`Image ${editingImageId} updated successfully.`);
            setEditingImageId(null); 
            await fetchProductImages();
        } catch (err: unknown) {
            console.error(`Failed to save changes for image ${editingImageId}:`, err);
            const errorMessage = await parseApiError(err);
            setError(`Save failed: ${errorMessage}`);
        } finally {
            setLoading(false);
        }
    }, [editingImageId, editFormData, parseApiError, fetchProductImages, getAuthHeaders]);

    return {
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
        fetchProductImages,
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
    };
};