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

import { mapValues } from '../runtime';
import type { ProductImageDetailsDto } from './ProductImageDetailsDto';
import {
    ProductImageDetailsDtoFromJSON,
    ProductImageDetailsDtoFromJSONTyped,
    ProductImageDetailsDtoToJSON,
    ProductImageDetailsDtoToJSONTyped,
} from './ProductImageDetailsDto';
import type { CategoryDetailsDto } from './CategoryDetailsDto';
import {
    CategoryDetailsDtoFromJSON,
    CategoryDetailsDtoFromJSONTyped,
    CategoryDetailsDtoToJSON,
    CategoryDetailsDtoToJSONTyped,
} from './CategoryDetailsDto';
import type { ProductVariantDetailsDto } from './ProductVariantDetailsDto';
import {
    ProductVariantDetailsDtoFromJSON,
    ProductVariantDetailsDtoFromJSONTyped,
    ProductVariantDetailsDtoToJSON,
    ProductVariantDetailsDtoToJSONTyped,
} from './ProductVariantDetailsDto';

/**
 * 
 * @export
 * @interface ProductDetailsDto
 */
export interface ProductDetailsDto {
    /**
     * 
     * @type {string}
     * @memberof ProductDetailsDto
     */
    id?: string;
    /**
     * 
     * @type {Date}
     * @memberof ProductDetailsDto
     */
    createdDate?: Date;
    /**
     * 
     * @type {Date}
     * @memberof ProductDetailsDto
     */
    modifiedDate?: Date | null;
    /**
     * 
     * @type {string}
     * @memberof ProductDetailsDto
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ProductDetailsDto
     */
    description?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ProductDetailsDto
     */
    price?: number;
    /**
     * 
     * @type {string}
     * @memberof ProductDetailsDto
     */
    details?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ProductDetailsDto
     */
    totalQuantity?: number;
    /**
     * 
     * @type {string}
     * @memberof ProductDetailsDto
     */
    sku?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ProductDetailsDto
     */
    totalSell?: number;
    /**
     * 
     * @type {CategoryDetailsDto}
     * @memberof ProductDetailsDto
     */
    category?: CategoryDetailsDto;
    /**
     * 
     * @type {Array<ProductImageDetailsDto>}
     * @memberof ProductDetailsDto
     */
    images?: Array<ProductImageDetailsDto> | null;
    /**
     * 
     * @type {Array<ProductVariantDetailsDto>}
     * @memberof ProductDetailsDto
     */
    variants?: Array<ProductVariantDetailsDto> | null;
}

/**
 * Check if a given object implements the ProductDetailsDto interface.
 */
export function instanceOfProductDetailsDto(value: object): value is ProductDetailsDto {
    return true;
}

export function ProductDetailsDtoFromJSON(json: any): ProductDetailsDto {
    return ProductDetailsDtoFromJSONTyped(json, false);
}

export function ProductDetailsDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): ProductDetailsDto {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'createdDate': json['createdDate'] == null ? undefined : (new Date(json['createdDate'])),
        'modifiedDate': json['modifiedDate'] == null ? undefined : (new Date(json['modifiedDate'])),
        'name': json['name'] == null ? undefined : json['name'],
        'description': json['description'] == null ? undefined : json['description'],
        'price': json['price'] == null ? undefined : json['price'],
        'details': json['details'] == null ? undefined : json['details'],
        'totalQuantity': json['totalQuantity'] == null ? undefined : json['totalQuantity'],
        'sku': json['sku'] == null ? undefined : json['sku'],
        'totalSell': json['totalSell'] == null ? undefined : json['totalSell'],
        'category': json['category'] == null ? undefined : CategoryDetailsDtoFromJSON(json['category']),
        'images': json['images'] == null ? undefined : ((json['images'] as Array<any>).map(ProductImageDetailsDtoFromJSON)),
        'variants': json['variants'] == null ? undefined : ((json['variants'] as Array<any>).map(ProductVariantDetailsDtoFromJSON)),
    };
}

export function ProductDetailsDtoToJSON(json: any): ProductDetailsDto {
    return ProductDetailsDtoToJSONTyped(json, false);
}

export function ProductDetailsDtoToJSONTyped(value?: ProductDetailsDto | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'createdDate': value['createdDate'] == null ? undefined : ((value['createdDate']).toISOString()),
        'modifiedDate': value['modifiedDate'] == null ? undefined : ((value['modifiedDate'] as any).toISOString()),
        'name': value['name'],
        'description': value['description'],
        'price': value['price'],
        'details': value['details'],
        'totalQuantity': value['totalQuantity'],
        'sku': value['sku'],
        'totalSell': value['totalSell'],
        'category': CategoryDetailsDtoToJSON(value['category']),
        'images': value['images'] == null ? undefined : ((value['images'] as Array<any>).map(ProductImageDetailsDtoToJSON)),
        'variants': value['variants'] == null ? undefined : ((value['variants'] as Array<any>).map(ProductVariantDetailsDtoToJSON)),
    };
}

