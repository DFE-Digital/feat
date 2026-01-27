import { test, expect } from '@playwright/test';
import {
    fetchAutocompleteLocations,
    expectAutocompleteSchema,
} from '../helpers/autocompleteClient';

test.describe('API – AutocompleteLocations', () => {

    test('200 + schema: returns array of locations', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, 'MK4');
        expect(res.status()).toBe(200);

        const json = await res.json();
        await expectAutocompleteSchema(json);
    });

    test('Trigger rule: query < 3 chars returns empty array (or no results)', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, 'MK');
        expect(res.status()).toBe(400);

        // assert it returns *some* error payload
        const body = await res.text();
        expect(body.length).toBeGreaterThan(0);
    });

    test('England-only: English postcode query returns results', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, 'MK4');
        expect(res.status()).toBe(200);

        const json = await res.json();
        await expectAutocompleteSchema(json);

        expect(json.length).toBeGreaterThan(0);
    });

    test('England-only: non-England location should return no results (EH1)', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, 'EH1');
        expect(res.status()).toBe(200);

        const json = await res.json();
        expect(Array.isArray(json)).toBeTruthy();
        
        expect(json.length).toBe(0);
    });

    test('No results: random query returns empty array', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, 'zzzzzzzz');
        expect(res.status()).toBe(200);

        const json = await res.json();
        expect(Array.isArray(json)).toBeTruthy();
        expect(json.length).toBe(0);
    });

    test('Normalisation: query is case-insensitive and trims whitespace', async ({ request }) => {
        const variants = ['mk4', ' MK4 ', 'mK4'];

        for (const q of variants) {
            const res = await fetchAutocompleteLocations(request, q);
            expect(res.status(), `Expected 200 for query "${q}"`).toBe(200);

            const json = await res.json();
            await expectAutocompleteSchema(json);
            expect(json.length, `Expected results for query "${q}"`).toBeGreaterThan(0);
        }
    });
    
    // Invalid input / validation tests
    const invalidInputs = [
        null,
        '12345',
        'MK1!!!',
        '!!@@'
    ];

    invalidInputs.forEach((input) => {
        test(`Invalid input "${input}" returns empty list`, async ({ request }) => {
            const res = await fetchAutocompleteLocations(request, input as string | null);

            // Expect 200 OK
            expect(res.status()).toBe(200);

            // Expect empty array
            const json = await res.json();
            expect(Array.isArray(json)).toBeTruthy();
            expect(json.length).toBe(0);
        });
    });

    test('Empty string query returns validation error', async ({ request }) => {
        const res = await fetchAutocompleteLocations(request, '');
        expect(res.status()).toBe(400);

        const body = await res.json();

        // Check it has expected structure
        expect(body).toHaveProperty('errors.query');
        expect(Array.isArray(body.errors.query)).toBeTruthy();
        expect(body.errors.query[0]).toBe('The query field is required.');

        // Optional: check status and title
        expect(body).toHaveProperty('status', 400);
        expect(body).toHaveProperty('title', 'One or more validation errors occurred.');
    });

});
