import {
    Middleware,
    RequestContext,
    FetchParams,
    ErrorContext,
    ResponseError
} from './runtime';
import { authService, oidcConfig } from '../auth/AuthConfig';

export const authMiddleware: Middleware = {
    pre: async (context: RequestContext): Promise<FetchParams | void> => {
        const token = await authService.getAccessToken();
               const authority = oidcConfig.authority;

        if (token && !context.url.startsWith(authority)) {
            const headers: Record<string, string> = { ...context.init.headers } as Record<string, string>;
            headers['Authorization'] = `Bearer ${token}`;
            headers['Content-Type'] = 'application/json';
            console.log(`Attaching token to ${context.url}`);
            return {
                url: context.url,
                init: { ...context.init, headers }
            };
        }
        
        console.log(`Not attaching token to ${context.url}`); 
    },
    onError: async (context: ErrorContext): Promise<Response | void> => {
        if (context.error instanceof ResponseError && context.error.response.status === 401) {
             console.error('API returned 401 Unauthorized. Token might be invalid or expired.');
             try {
                 const user = await authService.renewToken();
                 if (user?.access_token) {
                     console.log('Retrying API request after silent renew.');

                     const headers: Record<string, string> = { ...context.init.headers } as Record<string, string>;
                     headers['Authorization'] = `Bearer ${user.access_token}`;
                     headers['Content-Type'] = 'application/json';
                     
                     const fetchApi = context.fetch || fetch;
                     const retryResponse = await fetchApi(context.url, { ...context.init, headers });

                     
                     if (retryResponse.ok) {
                         return retryResponse;
                     } else {
                          console.error(`Retry failed with status: ${retryResponse.status}`);
                          await authService.logout();
                          throw new Error(`API request failed after retry with status ${retryResponse.status}`);
                     }

                 } else {
                      await authService.logout();
                      throw new Error('Silent renew failed, user logged out.');
                 }
             } catch (renewError) {
                  console.error('Error retrying after 401:', renewError);
                  await authService.logout();
                  throw renewError;
             }
        } else if (context.error instanceof Error) {
             console.error('API request failed:', context.error.message);
        } else {
             console.error('An unknown API error occurred:', context.error);
        }
    }
};