import { test, expect } from '@playwright/test';
import { ApiHelper } from '../helpers/apiHelper';
import { ResponseValidator } from '../helpers/responseValidator';
import { TestDataFactory } from '../fixtures/testDataFactory';
import { searchResponseSchema } from '../schemas/searchResponseSchema';
import { apiConfig } from '../config/apiConfig';

test.beforeEach(async ({}, testInfo) => {
    console.log(`Starting test: ${testInfo.title}`);
});

test.afterEach(async ({}, testInfo) => {
    console.log(`Test completed: ${testInfo.title} - ${testInfo.status}\n`);
});

//Happy Path Tests
test.describe('Search API - Happy Path', () => {

    test('should return valid search results for "car" query', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.validSearch();

        const response = await api.post(apiConfig.endpoints.search, payload);

        // Validate response success
        ResponseValidator.expectSuccess(response);

        // Validate schema structure
        await ResponseValidator.expectJsonSchema(response, searchResponseSchema);

        // Validate key fields
        const body = await response.json();
        expect(body.page).toBe(1);
        expect(body.pageSize).toBe(apiConfig.pagination.defaultPageSize);
        expect(Array.isArray(body.courses)).toBeTruthy();
        expect(body.courses.length).toBeGreaterThan(0);

        const firstCourse = body.courses[0];
        expect(firstCourse.courseName).toBeTruthy();
        expect(firstCourse.providerName).toBeTruthy();
        expect(firstCourse.id).toBeTruthy();
    });

    test('should include valid facets array', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.validSearch();

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectSuccess(response);

        const body = await response.json();
        expect(Array.isArray(body.facets)).toBeTruthy();
        expect(body.facets.length).toBeGreaterThan(0);

        const deliveryFacet = body.facets.find((f: any) => f.name === 'DELIVERY_MODE');
        expect(deliveryFacet).toBeTruthy();
        expect(Object.keys(deliveryFacet.values).length).toBeGreaterThan(0);
    });
});

//Edge case tests
test.describe('Search API - Edge Cases', () => {

    test('should handle empty query string gracefully', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.emptyQuery();

        const response = await api.post(apiConfig.endpoints.search, payload);

        // API could return 200 (all results) or 400 (invalid)
        ResponseValidator.expectStatusRange(response, 200, 400);
    });

    test('should handle large page size', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.largePageSize();

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectStatusRange(response, 200, 400);
    });

    test('should handle invalid page number', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.invalidPageNumber();

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectStatusRange(response, 200, 400);
    });

    test('should support location-based search', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.searchWithLocation();

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectSuccess(response);

        const body = await response.json();
        expect(body.courses.length).toBeGreaterThan(0);
    });
});

// Negative Tests
test.describe('Search API - Negative Tests', () => {

    test('should handle missing required query field', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = TestDataFactory.missingQuery();

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectStatusRange(response, 400, 422);
    });

    test('should handle completely invalid payload', async ({ request }) => {
        const api = new ApiHelper(request);
        const payload = { invalid: 'body' };

        const response = await api.post(apiConfig.endpoints.search, payload);
        ResponseValidator.expectStatusRange(response, 400, 422);
    });
});
