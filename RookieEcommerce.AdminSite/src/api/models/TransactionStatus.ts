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
import type { TransactionStatusCode } from './TransactionStatusCode';
import {
    TransactionStatusCodeFromJSON,
    TransactionStatusCodeFromJSONTyped,
    TransactionStatusCodeToJSON,
    TransactionStatusCodeToJSONTyped,
} from './TransactionStatusCode';

/**
 * 
 * @export
 * @interface TransactionStatus
 */
export interface TransactionStatus {
    /**
     * 
     * @type {TransactionStatusCode}
     * @memberof TransactionStatus
     */
    code?: TransactionStatusCode;
    /**
     * 
     * @type {string}
     * @memberof TransactionStatus
     */
    description?: string | null;
}



/**
 * Check if a given object implements the TransactionStatus interface.
 */
export function instanceOfTransactionStatus(value: object): value is TransactionStatus {
    return true;
}

export function TransactionStatusFromJSON(json: any): TransactionStatus {
    return TransactionStatusFromJSONTyped(json, false);
}

export function TransactionStatusFromJSONTyped(json: any, ignoreDiscriminator: boolean): TransactionStatus {
    if (json == null) {
        return json;
    }
    return {
        
        'code': json['code'] == null ? undefined : TransactionStatusCodeFromJSON(json['code']),
        'description': json['description'] == null ? undefined : json['description'],
    };
}

export function TransactionStatusToJSON(json: any): TransactionStatus {
    return TransactionStatusToJSONTyped(json, false);
}

export function TransactionStatusToJSONTyped(value?: TransactionStatus | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'code': TransactionStatusCodeToJSON(value['code']),
        'description': value['description'],
    };
}

