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
 * @interface CreateProductRatingCommand
 */
export interface CreateProductRatingCommand {
    /**
     * 
     * @type {string}
     * @memberof CreateProductRatingCommand
     */
    productId?: string;
    /**
     * 
     * @type {string}
     * @memberof CreateProductRatingCommand
     */
    customerId?: string;
    /**
     * 
     * @type {number}
     * @memberof CreateProductRatingCommand
     */
    ratingValue?: number;
    /**
     * 
     * @type {string}
     * @memberof CreateProductRatingCommand
     */
    comment?: string | null;
}

/**
 * Check if a given object implements the CreateProductRatingCommand interface.
 */
export function instanceOfCreateProductRatingCommand(value: object): value is CreateProductRatingCommand {
    return true;
}

export function CreateProductRatingCommandFromJSON(json: any): CreateProductRatingCommand {
    return CreateProductRatingCommandFromJSONTyped(json, false);
}

export function CreateProductRatingCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): CreateProductRatingCommand {
    if (json == null) {
        return json;
    }
    return {
        
        'productId': json['productId'] == null ? undefined : json['productId'],
        'customerId': json['customerId'] == null ? undefined : json['customerId'],
        'ratingValue': json['ratingValue'] == null ? undefined : json['ratingValue'],
        'comment': json['comment'] == null ? undefined : json['comment'],
    };
}

export function CreateProductRatingCommandToJSON(json: any): CreateProductRatingCommand {
    return CreateProductRatingCommandToJSONTyped(json, false);
}

export function CreateProductRatingCommandToJSONTyped(value?: CreateProductRatingCommand | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'productId': value['productId'],
        'customerId': value['customerId'],
        'ratingValue': value['ratingValue'],
        'comment': value['comment'],
    };
}

