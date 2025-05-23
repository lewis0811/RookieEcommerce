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
/**
 * 
 * @export
 * @interface ProductImageDetailsDto
 */
export interface ProductImageDetailsDto {
    /**
     * 
     * @type {string}
     * @memberof ProductImageDetailsDto
     */
    id?: string;
    /**
     * 
     * @type {Date}
     * @memberof ProductImageDetailsDto
     */
    createdDate?: Date;
    /**
     * 
     * @type {Date}
     * @memberof ProductImageDetailsDto
     */
    modifiedDate?: Date | null;
    /**
     * 
     * @type {string}
     * @memberof ProductImageDetailsDto
     */
    imageUrl?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ProductImageDetailsDto
     */
    altText?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ProductImageDetailsDto
     */
    sortOrder?: number;
    /**
     * 
     * @type {boolean}
     * @memberof ProductImageDetailsDto
     */
    isPrimary?: boolean;
    /**
     * 
     * @type {string}
     * @memberof ProductImageDetailsDto
     */
    productId?: string;
}

/**
 * Check if a given object implements the ProductImageDetailsDto interface.
 */
export function instanceOfProductImageDetailsDto(value: object): value is ProductImageDetailsDto {
    return true;
}

export function ProductImageDetailsDtoFromJSON(json: any): ProductImageDetailsDto {
    return ProductImageDetailsDtoFromJSONTyped(json, false);
}

export function ProductImageDetailsDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): ProductImageDetailsDto {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'createdDate': json['createdDate'] == null ? undefined : (new Date(json['createdDate'])),
        'modifiedDate': json['modifiedDate'] == null ? undefined : (new Date(json['modifiedDate'])),
        'imageUrl': json['imageUrl'] == null ? undefined : json['imageUrl'],
        'altText': json['altText'] == null ? undefined : json['altText'],
        'sortOrder': json['sortOrder'] == null ? undefined : json['sortOrder'],
        'isPrimary': json['isPrimary'] == null ? undefined : json['isPrimary'],
        'productId': json['productId'] == null ? undefined : json['productId'],
    };
}

export function ProductImageDetailsDtoToJSON(json: any): ProductImageDetailsDto {
    return ProductImageDetailsDtoToJSONTyped(json, false);
}

export function ProductImageDetailsDtoToJSONTyped(value?: ProductImageDetailsDto | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'createdDate': value['createdDate'] == null ? undefined : ((value['createdDate']).toISOString()),
        'modifiedDate': value['modifiedDate'] == null ? undefined : ((value['modifiedDate'] as any).toISOString()),
        'imageUrl': value['imageUrl'],
        'altText': value['altText'],
        'sortOrder': value['sortOrder'],
        'isPrimary': value['isPrimary'],
        'productId': value['productId'],
    };
}

