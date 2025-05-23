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
 * @interface CreateCartItemCommand
 */
export interface CreateCartItemCommand {
    /**
     * 
     * @type {string}
     * @memberof CreateCartItemCommand
     */
    productId?: string;
    /**
     * 
     * @type {string}
     * @memberof CreateCartItemCommand
     */
    productVariantId?: string | null;
    /**
     * 
     * @type {number}
     * @memberof CreateCartItemCommand
     */
    quantity?: number;
}

/**
 * Check if a given object implements the CreateCartItemCommand interface.
 */
export function instanceOfCreateCartItemCommand(value: object): value is CreateCartItemCommand {
    return true;
}

export function CreateCartItemCommandFromJSON(json: any): CreateCartItemCommand {
    return CreateCartItemCommandFromJSONTyped(json, false);
}

export function CreateCartItemCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): CreateCartItemCommand {
    if (json == null) {
        return json;
    }
    return {
        
        'productId': json['productId'] == null ? undefined : json['productId'],
        'productVariantId': json['productVariantId'] == null ? undefined : json['productVariantId'],
        'quantity': json['quantity'] == null ? undefined : json['quantity'],
    };
}

export function CreateCartItemCommandToJSON(json: any): CreateCartItemCommand {
    return CreateCartItemCommandToJSONTyped(json, false);
}

export function CreateCartItemCommandToJSONTyped(value?: CreateCartItemCommand | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'productId': value['productId'],
        'productVariantId': value['productVariantId'],
        'quantity': value['quantity'],
    };
}

