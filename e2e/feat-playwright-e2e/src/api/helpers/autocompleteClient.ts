import { APIRequestContext, expect } from '@playwright/test';

export type AutocompleteLocation = {
    name: string;
    latitude: number;
    longitude: number;
};

export async function fetchAutocompleteLocations(
    request: APIRequestContext,
    query: string,
) {
    const res = await request.get(`/api/AutocompleteLocations`, {
        params: { query },
    });

    return res;
}

export async function expectAutocompleteSchema(json: any) {
    expect(Array.isArray(json)).toBeTruthy();

    for (const item of json) {
        expect(item).toHaveProperty('name');
        expect(typeof item.name).toBe('string');

        expect(item).toHaveProperty('latitude');
        expect(typeof item.latitude).toBe('number');

        expect(item).toHaveProperty('longitude');
        expect(typeof item.longitude).toBe('number');
    }
}
