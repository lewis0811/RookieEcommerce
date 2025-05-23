/* tslint:disable */
/* eslint-disable */
/**
 * NashLux API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import type {
  Category,
  CategoryCreateDto,
  CategoryDetailsDto,
  CategoryDetailsDtoPaginationList,
  CreateCategoryCommand,
  ProblemDetails,
  UpdateCategoryCommand,
} from '../models/index';
import {
    CategoryFromJSON,
    CategoryToJSON,
    CategoryCreateDtoFromJSON,
    CategoryCreateDtoToJSON,
    CategoryDetailsDtoFromJSON,
    CategoryDetailsDtoToJSON,
    CategoryDetailsDtoPaginationListFromJSON,
    CategoryDetailsDtoPaginationListToJSON,
    CreateCategoryCommandFromJSON,
    CreateCategoryCommandToJSON,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    UpdateCategoryCommandFromJSON,
    UpdateCategoryCommandToJSON,
} from '../models/index';

export interface ApiV1CategoriesCategoryIdDeleteRequest {
    categoryId: string;
}

export interface ApiV1CategoriesCategoryIdGetRequest {
    categoryId: string;
    isIncludeItems?: boolean;
}

export interface ApiV1CategoriesCategoryIdPutRequest {
    categoryId: string;
    updateCategoryCommand?: UpdateCategoryCommand;
}

export interface ApiV1CategoriesGetRequest {
    parentCategoryId?: string;
    isIncludeItems?: boolean;
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    sortBy?: string;
    includeProperties?: string;
}

export interface ApiV1CategoriesPaginationGetRequest {
    parentCategoryId?: string;
    isIncludeItems?: boolean;
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    sortBy?: string;
    includeProperties?: string;
}

export interface ApiV1CategoriesPostRequest {
    createCategoryCommand?: CreateCategoryCommand;
}

/**
 * 
 */
export class CategoriesApi extends runtime.BaseAPI {

    /**
     */
    async apiV1CategoriesCategoryIdDeleteRaw(requestParameters: ApiV1CategoriesCategoryIdDeleteRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<void>> {
        if (requestParameters['categoryId'] == null) {
            throw new runtime.RequiredError(
                'categoryId',
                'Required parameter "categoryId" was null or undefined when calling apiV1CategoriesCategoryIdDelete().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories/{category-id}`.replace(`{${"category-id"}}`, encodeURIComponent(String(requestParameters['categoryId']))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiV1CategoriesCategoryIdDelete(requestParameters: ApiV1CategoriesCategoryIdDeleteRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<void> {
        await this.apiV1CategoriesCategoryIdDeleteRaw(requestParameters, initOverrides);
    }

    /**
     */
    async apiV1CategoriesCategoryIdGetRaw(requestParameters: ApiV1CategoriesCategoryIdGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CategoryDetailsDto>> {
        if (requestParameters['categoryId'] == null) {
            throw new runtime.RequiredError(
                'categoryId',
                'Required parameter "categoryId" was null or undefined when calling apiV1CategoriesCategoryIdGet().'
            );
        }

        const queryParameters: any = {};

        if (requestParameters['isIncludeItems'] != null) {
            queryParameters['isIncludeItems'] = requestParameters['isIncludeItems'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories/{category-id}`.replace(`{${"category-id"}}`, encodeURIComponent(String(requestParameters['categoryId']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CategoryDetailsDtoFromJSON(jsonValue));
    }

    /**
     */
    async apiV1CategoriesCategoryIdGet(requestParameters: ApiV1CategoriesCategoryIdGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CategoryDetailsDto> {
        const response = await this.apiV1CategoriesCategoryIdGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiV1CategoriesCategoryIdPutRaw(requestParameters: ApiV1CategoriesCategoryIdPutRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<void>> {
        if (requestParameters['categoryId'] == null) {
            throw new runtime.RequiredError(
                'categoryId',
                'Required parameter "categoryId" was null or undefined when calling apiV1CategoriesCategoryIdPut().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories/{category-id}`.replace(`{${"category-id"}}`, encodeURIComponent(String(requestParameters['categoryId']))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: UpdateCategoryCommandToJSON(requestParameters['updateCategoryCommand']),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiV1CategoriesCategoryIdPut(requestParameters: ApiV1CategoriesCategoryIdPutRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<void> {
        await this.apiV1CategoriesCategoryIdPutRaw(requestParameters, initOverrides);
    }

    /**
     */
    async apiV1CategoriesGetRaw(requestParameters: ApiV1CategoriesGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Array<Category>>> {
        const queryParameters: any = {};

        if (requestParameters['parentCategoryId'] != null) {
            queryParameters['ParentCategoryId'] = requestParameters['parentCategoryId'];
        }

        if (requestParameters['isIncludeItems'] != null) {
            queryParameters['IsIncludeItems'] = requestParameters['isIncludeItems'];
        }

        if (requestParameters['pageNumber'] != null) {
            queryParameters['PageNumber'] = requestParameters['pageNumber'];
        }

        if (requestParameters['pageSize'] != null) {
            queryParameters['PageSize'] = requestParameters['pageSize'];
        }

        if (requestParameters['searchTerm'] != null) {
            queryParameters['SearchTerm'] = requestParameters['searchTerm'];
        }

        if (requestParameters['sortBy'] != null) {
            queryParameters['SortBy'] = requestParameters['sortBy'];
        }

        if (requestParameters['includeProperties'] != null) {
            queryParameters['IncludeProperties'] = requestParameters['includeProperties'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(CategoryFromJSON));
    }

    /**
     */
    async apiV1CategoriesGet(requestParameters: ApiV1CategoriesGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Array<Category>> {
        const response = await this.apiV1CategoriesGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiV1CategoriesPaginationGetRaw(requestParameters: ApiV1CategoriesPaginationGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CategoryDetailsDtoPaginationList>> {
        const queryParameters: any = {};

        if (requestParameters['parentCategoryId'] != null) {
            queryParameters['ParentCategoryId'] = requestParameters['parentCategoryId'];
        }

        if (requestParameters['isIncludeItems'] != null) {
            queryParameters['IsIncludeItems'] = requestParameters['isIncludeItems'];
        }

        if (requestParameters['pageNumber'] != null) {
            queryParameters['PageNumber'] = requestParameters['pageNumber'];
        }

        if (requestParameters['pageSize'] != null) {
            queryParameters['PageSize'] = requestParameters['pageSize'];
        }

        if (requestParameters['searchTerm'] != null) {
            queryParameters['SearchTerm'] = requestParameters['searchTerm'];
        }

        if (requestParameters['sortBy'] != null) {
            queryParameters['SortBy'] = requestParameters['sortBy'];
        }

        if (requestParameters['includeProperties'] != null) {
            queryParameters['IncludeProperties'] = requestParameters['includeProperties'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories/pagination`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CategoryDetailsDtoPaginationListFromJSON(jsonValue));
    }

    /**
     */
    async apiV1CategoriesPaginationGet(requestParameters: ApiV1CategoriesPaginationGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CategoryDetailsDtoPaginationList> {
        const response = await this.apiV1CategoriesPaginationGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiV1CategoriesPostRaw(requestParameters: ApiV1CategoriesPostRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CategoryCreateDto>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/v1/categories`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: CreateCategoryCommandToJSON(requestParameters['createCategoryCommand']),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CategoryCreateDtoFromJSON(jsonValue));
    }

    /**
     */
    async apiV1CategoriesPost(requestParameters: ApiV1CategoriesPostRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CategoryCreateDto> {
        const response = await this.apiV1CategoriesPostRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
