import {
    Configuration,
    ConfigurationParameters,
    BaseAPI
} from './runtime';
import { BASE_PATH_API } from '../config/api';
import { authMiddleware } from './authMiddleware';

export function createApiClient<T extends BaseAPI>(
    ApiClass: new (config?: Configuration) => T
): T {
    
    const configParams: ConfigurationParameters = {
        basePath: BASE_PATH_API,
        middleware: [authMiddleware],
    };

    const configuration = new Configuration(configParams);

    const apiClient = new ApiClass(configuration);
    return apiClient;
}
