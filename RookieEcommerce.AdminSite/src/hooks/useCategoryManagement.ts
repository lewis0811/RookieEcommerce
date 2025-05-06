// src/hooks/useCategoryManagement.ts
import { useState, useEffect, useCallback } from 'react';
import { SelectChangeEvent } from '@mui/material/Select';
import {
    CategoriesApi,
    Category,
    CategoryDetailsDto,
    Configuration,
    CreateCategoryCommand,
    UpdateCategoryCommand,
} from '../api';

const apiConfig = new Configuration({ basePath: import.meta.env.VITE_BASE_PATH_API });
const categoriesApi = new CategoriesApi(apiConfig);

export interface CategoryFormDataState {
    name: string;
    description: string;
    parentCategoryId: string | null;
}

export const useCategoryManagement = (initialPageSize: number = 10) => {
    const [categories, setCategories] = useState<CategoryDetailsDto[]>([]);
    const [parentCategories, setParentCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(initialPageSize);
    const [totalCount, setTotalCount] = useState(0);

    const [searchTerm, setSearchTerm] = useState<string>('');
    const [sortBy, setSortBy] = useState<string>('name');

    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const [editingCategory, setEditingCategory] = useState<CategoryDetailsDto | null>(null);
    const [formData, setFormData] = useState<CategoryFormDataState>({
        name: '',
        description: '',
        parentCategoryId: null,
    });

    const fetchCategories = useCallback(async (showLoading = true) => {
        if (showLoading) setLoading(true);
        
        try {
            const params = {
                pageNumber: page + 1,
                pageSize: rowsPerPage,
                searchTerm: searchTerm || undefined,
                sortBy: sortBy || undefined,
                isIncludeItems: true,
            };
            const response = await categoriesApi.apiV1CategoriesPaginationGet(params);
            setCategories(response.items ?? []);
            setTotalCount(response.totalCount ?? 0);
          
            if (response.totalCount !== null && response.totalCount !== undefined && (page + 1) > (response.totalPages ?? 0) && page > 0) {
                setPage(Math.max(0, (response.totalPages ?? 1) - 1));
            }

        } catch (err: unknown) {
            console.error("Failed to fetch categories:", err);
            const errorMessage = err instanceof Error ? err.message : 'Lỗi không xác định';
            setError(`Không thể tải danh mục: ${errorMessage}`);
            setCategories([]);
            setTotalCount(0);
        } finally {
            if (showLoading) setLoading(false);
        }
    }, [page, rowsPerPage, searchTerm, sortBy]);

    const fetchParentCategories = useCallback(async () => {
        try {
            const response = await categoriesApi.apiV1CategoriesGet({ pageSize: 1000 });
            setParentCategories(response ?? []);
        } catch (err) {
            console.error("Failed to fetch parent categories:", err);
        }
    }, []);

    useEffect(() => {
        fetchCategories();
    }, [fetchCategories]);

    useEffect(() => {
        fetchParentCategories();
    }, [fetchParentCategories]);

    // Debounce search
    useEffect(() => {
        const timer = setTimeout(() => {
            setPage(0);
        }, 500);
        return () => clearTimeout(timer);
    }, [searchTerm]);


    const handleCreate = () => {
        setEditingCategory(null);
        setFormData({ name: '', description: '', parentCategoryId: null });
        setError(null);
        setIsModalOpen(true);
    };

    const handleEdit = (category: CategoryDetailsDto) => {
        setEditingCategory(category);
        setFormData({
            name: category.name ?? '',
            description: category.description ?? '',
            parentCategoryId: category.parentCategoryId ?? null,
        });
        setError(null);
        setIsModalOpen(true);
    };

    const handleDelete = async (categoryId: string) => {
        setLoading(true);
        setError(null);
        try {
            await categoriesApi.apiV1CategoriesCategoryIdDelete({ categoryId });
            if (categories.length === 1 && page > 0) {
                setPage(prevPage => prevPage - 1);
            } else {
                fetchCategories(false);
            }
        } catch (err: unknown) {
            console.error("Failed to delete category:", err);
            const errorMessage = err instanceof Error ? err.message : 'Lỗi không xác định';
            setError(`Xóa thất bại: ${errorMessage}`);
            setLoading(false);
        }
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
    };

    const handleFormChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string | null>
    ) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value === '' ? null : value }));
    };

    const handleFormSubmit = async () => {
        setLoading(true);
        setError(null);

        const commandData = {
            name: formData.name || undefined,
            description: formData.description || undefined,
            parentCategoryId: formData.parentCategoryId || undefined,
        };

        try {
            if (editingCategory?.id) {
                const updateCommand: UpdateCategoryCommand = {
                    name: commandData.name,
                    description: commandData.description,
                    // parentCategoryId không cho sửa ở đây theo logic cũ
                };
                await categoriesApi.apiV1CategoriesCategoryIdPut({
                    categoryId: editingCategory.id,
                    updateCategoryCommand: updateCommand,
                });
            } else {
                const createCommand: CreateCategoryCommand = commandData;
                await categoriesApi.apiV1CategoriesPost({ createCategoryCommand: createCommand });
            }
            handleModalClose();
            setPage(0); 
            fetchParentCategories();
        } catch (err: unknown) {
            console.error("Failed to save category:", err);
            let errMsg = 'Lỗi không xác định khi lưu danh mục.';
            if (err instanceof Error) {
                errMsg = err.message;
            }
            setError(errMsg);
            throw err;
        } finally {
            setLoading(false);
        }
    };

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(e.target.value);
    };

    const handleSortChange = (event: SelectChangeEvent<string>) => {
        const newSortBy = event.target.value;
        setSortBy(newSortBy);
        setPage(0);
    };

    const handleChangePage = (
        _event: React.MouseEvent<HTMLButtonElement> | null,
        newPage: number
    ) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (
        event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    return {
        // States
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
        // Handlers
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
    };
};