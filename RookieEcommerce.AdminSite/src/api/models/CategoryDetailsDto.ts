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
import type { SubCategoriesDto } from './SubCategoriesDto';
import {
    SubCategoriesDtoFromJSON,
    SubCategoriesDtoFromJSONTyped,
    SubCategoriesDtoToJSON,
    SubCategoriesDtoToJSONTyped,
} from './SubCategoriesDto';
import type { ProductsInCategoryDto } from './ProductsInCategoryDto';
import {
    ProductsInCategoryDtoFromJSON,
    ProductsInCategoryDtoFromJSONTyped,
    ProductsInCategoryDtoToJSON,
    ProductsInCategoryDtoToJSONTyped,
} from './ProductsInCategoryDto';

/**
 * 
 * @export
 * @interface CategoryDetailsDto
 */
export interface CategoryDetailsDto {
    /**
     * 
     * @type {string}
     * @memberof CategoryDetailsDto
     */
    id?: string;
    /**
     * 
     * @type {Date}
     * @memberof CategoryDetailsDto
     */
    createdDate?: Date;
    /**
     * 
     * @type {Date}
     * @memberof CategoryDetailsDto
     */
    modifiedDate?: Date | null;
    /**
     * 
     * @type {string}
     * @memberof CategoryDetailsDto
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof CategoryDetailsDto
     */
    description?: string | null;
    /**
     * 
     * @type {string}
     * @memberof CategoryDetailsDto
     */
    parentCategoryId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof CategoryDetailsDto
     */
    parentCategoryName?: string | null;
    /**
     * 
     * @type {Array<SubCategoriesDto>}
     * @memberof CategoryDetailsDto
     */
    subCategories?: Array<SubCategoriesDto> | null;
    /**
     * 
     * @type {Array<ProductsInCategoryDto>}
     * @memberof CategoryDetailsDto
     */
    products?: Array<ProductsInCategoryDto> | null;
}

/**
 * Check if a given object implements the CategoryDetailsDto interface.
 */
export function instanceOfCategoryDetailsDto(value: object): value is CategoryDetailsDto {
    return true;
}

export function CategoryDetailsDtoFromJSON(json: any): CategoryDetailsDto {
    return CategoryDetailsDtoFromJSONTyped(json, false);
}

export function CategoryDetailsDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): CategoryDetailsDto {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'createdDate': json['createdDate'] == null ? undefined : (new Date(json['createdDate'])),
        'modifiedDate': json['modifiedDate'] == null ? undefined : (new Date(json['modifiedDate'])),
        'name': json['name'] == null ? undefined : json['name'],
        'description': json['description'] == null ? undefined : json['description'],
        'parentCategoryId': json['parentCategoryId'] == null ? undefined : json['parentCategoryId'],
        'parentCategoryName': json['parentCategoryName'] == null ? undefined : json['parentCategoryName'],
        'subCategories': json['subCategories'] == null ? undefined : ((json['subCategories'] as Array<any>).map(SubCategoriesDtoFromJSON)),
        'products': json['products'] == null ? undefined : ((json['products'] as Array<any>).map(ProductsInCategoryDtoFromJSON)),
    };
}

export function CategoryDetailsDtoToJSON(json: any): CategoryDetailsDto {
    return CategoryDetailsDtoToJSONTyped(json, false);
}

export function CategoryDetailsDtoToJSONTyped(value?: CategoryDetailsDto | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'createdDate': value['createdDate'] == null ? undefined : ((value['createdDate']).toISOString()),
        'modifiedDate': value['modifiedDate'] == null ? undefined : ((value['modifiedDate'] as any).toISOString()),
        'name': value['name'],
        'description': value['description'],
        'parentCategoryId': value['parentCategoryId'],
        'parentCategoryName': value['parentCategoryName'],
        'subCategories': value['subCategories'] == null ? undefined : ((value['subCategories'] as Array<any>).map(SubCategoriesDtoToJSON)),
        'products': value['products'] == null ? undefined : ((value['products'] as Array<any>).map(ProductsInCategoryDtoToJSON)),
    };
}

