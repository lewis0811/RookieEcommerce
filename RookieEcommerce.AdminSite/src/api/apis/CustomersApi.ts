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
  CustomerDetailsDto,
  CustomerDetailsDtoPaginationList,
  ProblemDetails,
} from '../models/index';
import {
    CustomerDetailsDtoFromJSON,
    CustomerDetailsDtoToJSON,
    CustomerDetailsDtoPaginationListFromJSON,
    CustomerDetailsDtoPaginationListToJSON,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
} from '../models/index';

export interface ApiV1CustomersGetRequest {
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    sortBy?: string;
    includeProperties?: string;
}

export interface ApiV1CustomersIdGetRequest {
    id: string;
}

/**
 * 
 */
export class CustomersApi extends runtime.BaseAPI {

    /**
     */
    async apiV1CustomersGetRaw(requestParameters: ApiV1CustomersGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CustomerDetailsDtoPaginationList>> {
        const queryParameters: any = {};

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
            path: `/api/v1/customers`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CustomerDetailsDtoPaginationListFromJSON(jsonValue));
    }

    /**
     */
    async apiV1CustomersGet(requestParameters: ApiV1CustomersGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CustomerDetailsDtoPaginationList> {
        const response = await this.apiV1CustomersGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiV1CustomersIdGetRaw(requestParameters: ApiV1CustomersIdGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CustomerDetailsDto>> {
        if (requestParameters['id'] == null) {
            throw new runtime.RequiredError(
                'id',
                'Required parameter "id" was null or undefined when calling apiV1CustomersIdGet().'
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
            path: `/api/v1/customers/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters['id']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CustomerDetailsDtoFromJSON(jsonValue));
    }

    /**
     */
    async apiV1CustomersIdGet(requestParameters: ApiV1CustomersIdGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CustomerDetailsDto> {
        const response = await this.apiV1CustomersIdGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
