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
 * @interface UpdateProductRatingCommand
 */
export interface UpdateProductRatingCommand {
    /**
     * 
     * @type {number}
     * @memberof UpdateProductRatingCommand
     */
    ratingValue?: number | null;
    /**
     * 
     * @type {string}
     * @memberof UpdateProductRatingCommand
     */
    comment?: string | null;
}

/**
 * Check if a given object implements the UpdateProductRatingCommand interface.
 */
export function instanceOfUpdateProductRatingCommand(value: object): value is UpdateProductRatingCommand {
    return true;
}

export function UpdateProductRatingCommandFromJSON(json: any): UpdateProductRatingCommand {
    return UpdateProductRatingCommandFromJSONTyped(json, false);
}

export function UpdateProductRatingCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): UpdateProductRatingCommand {
    if (json == null) {
        return json;
    }
    return {
        
        'ratingValue': json['ratingValue'] == null ? undefined : json['ratingValue'],
        'comment': json['comment'] == null ? undefined : json['comment'],
    };
}

export function UpdateProductRatingCommandToJSON(json: any): UpdateProductRatingCommand {
    return UpdateProductRatingCommandToJSONTyped(json, false);
}

export function UpdateProductRatingCommandToJSONTyped(value?: UpdateProductRatingCommand | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'ratingValue': value['ratingValue'],
        'comment': value['comment'],
    };
}

