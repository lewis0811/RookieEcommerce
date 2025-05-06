import * as React from 'react';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Alert, Button } from '@mui/material';
import ProductImageManagementPage from './ProductImageManagement';
import ProductVariantManagementPage from './ProductVariantManagement';

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

const CustomTabPanel = (props: TabPanelProps) => {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
}

const a11yProps = (index: number) => {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    };
}

export const ProductDetailManagementPage = () => {
    const [searchParams] = useSearchParams();
    const productId = searchParams.get('productId');
    const [value, setValue] = React.useState(0);
    const navigate = useNavigate();

    const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };


    // Handle case where productId is missing entirely
    if (!productId) {
        return (
            <Box sx={{ width: '100%', p: 3 }}>
                <Button variant="outlined" onClick={() => navigate(-1)} sx={{ mb: 2 }}>
                    Quay lại Danh sách Sản phẩm
                </Button>
                <Alert severity="error">
                    Không tìm thấy ID sản phẩm trong địa chỉ URL. Vui lòng kiểm tra lại.
                </Alert>
            </Box>
        );
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Button variant="contained" color="primary" onClick={() => navigate(-1)}>
                Quay lại
            </Button>
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={value} onChange={handleChange} aria-label="basic tabs example">
                    <Tab label="Variants" {...a11yProps(0)} />
                    <Tab label="Images" {...a11yProps(1)} />
                </Tabs>
            </Box>
            <CustomTabPanel value={value} index={0}>
                <ProductVariantManagementPage productId={productId} />
            </CustomTabPanel>
            <CustomTabPanel value={value} index={1}>
                <ProductImageManagementPage productId={productId} />
            </CustomTabPanel>
        </Box>
    );
}
