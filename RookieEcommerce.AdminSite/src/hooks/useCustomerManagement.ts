import { useState, useEffect, useCallback, ChangeEvent } from 'react';
import { SelectChangeEvent } from '@mui/material';
import { Configuration, CustomerDetailsDto, CustomerDetailsDtoPaginationList, CustomersApi } from '../api';
import { authService } from '../auth/AuthConfig';

const apiConfig = new Configuration({ basePath: import.meta.env.VITE_BASE_PATH_API });
const customersApi = new CustomersApi(apiConfig);

export const useCustomerManagement = () => {
    const [customers, setCustomers] = useState<CustomerDetailsDto[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [totalCount, setTotalCount] = useState(0);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [sortBy, setSortBy] = useState<string>('name');

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

    useEffect(() => {
        const executeFetch = async () => {
            setLoading(true);
            setError(null);
            const headers = await getAuthHeaders();
            if (!headers) {
                setLoading(false);
                setError("Authentication failed. Cannot fetch customers.");
                setCustomers([]);
                setTotalCount(0);
                return;
            }

            try {
                const response: CustomerDetailsDtoPaginationList =
                    await customersApi.apiV1CustomersGet({
                        pageNumber: page + 1,
                        pageSize: rowsPerPage,
                        searchTerm: searchTerm || undefined,
                        sortBy: sortBy || undefined,
                    }
                        , { headers }
                    );
                setCustomers(response.items ?? []);
                setTotalCount(response.totalCount ?? 0);
            } catch (err: unknown) {
                const errorMessage = await parseApiError(err);
                setError(`Could not load customers: ${errorMessage}`);
                setCustomers([]);
                setTotalCount(0);
            } finally {
                setLoading(false);
            }
        };

        executeFetch();
    }, [page, rowsPerPage, searchTerm, sortBy, getAuthHeaders, parseApiError]);


    const handleSearchChange = (event: ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
    };

    const handleSearchSubmit = () => {
        setPage(0);
    };

    const handleSortChange = (event: SelectChangeEvent<string>) => {
        const newSortBy = event.target.value;
        setSortBy(newSortBy);
        setPage(0);
    };

    const handleChangePage = (
        _event: React.MouseEvent<HTMLButtonElement> | null,
        newPage: number
    ): void => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ): void => {
        const newSize = parseInt(event.target.value, 10);
        setRowsPerPage(newSize);
        setPage(0);
    };

    return {
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
    };
};