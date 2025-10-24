// src/api/helpers/responseValidator.ts
import { APIResponse, expect } from '@playwright/test';
import Ajv, { ValidateFunction } from 'ajv';

// AJV: JSON Schema validator
const ajv = new Ajv({ allErrors: true, verbose: true });

//ResponseValidator:Provides simple, reusable validation methods for API responses (status codes + JSON schema).
 
export class ResponseValidator {

    //Validate that response has expected exact status 
    static expectStatus(response: APIResponse, expectedStatus: number, message?: string) {
        const actualStatus = response.status();
        expect(
            actualStatus,
            message || `Expected status ${expectedStatus}, got ${actualStatus}`
        ).toBe(expectedStatus);
    }

    //Validate that status is within an acceptable range 
    static expectStatusRange(response: APIResponse, min: number, max: number) {
        const status = response.status();
        expect(
            status,
            `Expected status between ${min}-${max}, got ${status}`
        ).toBeGreaterThanOrEqual(min);
        expect(
            status,
            `Expected status between ${min}-${max}, got ${status}`
        ).toBeLessThanOrEqual(max);
    }

    //Validate that response indicates success (2xx) 
    static expectSuccess(response: APIResponse) {
        expect(
            response.ok(),
            `Expected successful response (2xx), got ${response.status()}`
        ).toBeTruthy();
    }

    //Validate that response matches the provided JSON schema 
    static async expectJsonSchema(response: APIResponse, schema: object) {
        const body = await response.json();
        const validate: ValidateFunction = ajv.compile(schema);
        const isValid = validate(body);

        if (!isValid) {
            console.error('Schema validation errors:', JSON.stringify(validate.errors, null, 2));
            console.error('Response body:', JSON.stringify(body, null, 2));
        }

        expect(isValid, `Schema validation failed: ${JSON.stringify(validate.errors)}`).toBeTruthy();
    }
}
