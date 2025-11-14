// src/api/tests/courseById.spec.ts
import { test, expect } from '@playwright/test';
import { apiConfig } from '../config/apiConfig';
import { ApiHelper } from '../helpers/apiHelper';
import { ResponseValidator } from '../helpers/responseValidator';
import { courseByIdSchema } from '../schemas/courseByIdSchema';

const UUID_REGEX =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

const defaultSampleId = '8b4600a3-d862-4333-c8f5-08de214cd1df';
const sampleCourseId = process.env.FEAT_SAMPLE_COURSE_ID || defaultSampleId;

// Simple valid UUID v4 generator for "unknown" IDs
function randomUUIDv4(): string {
    const hex = [...Array(256).keys()].map(i => (i + 0x100).toString(16).slice(1));
    const rnd = new Uint8Array(16);
    for (let i = 0; i < 16; i++) rnd[i] = Math.floor(Math.random() * 256);
    rnd[6] = (rnd[6] & 0x0f) | 0x40;
    rnd[8] = (rnd[8] & 0x3f) | 0x80;
    const b = rnd;
    return `${hex[b[0]]}${hex[b[1]]}${hex[b[2]]}${hex[b[3]]}-${hex[b[4]]}${hex[b[5]]}-${hex[b[6]]}${hex[b[7]]}-${hex[b[8]]}${hex[b[9]]}-${hex[b[10]]}${hex[b[11]]}${hex[b[12]]}${hex[b[13]]}${hex[b[14]]}${hex[b[15]]}`;
}

test.describe('GET /api/Courses/{courseId}', () => {
    test.beforeEach(async ({}, info) => {
        console.log(`Starting test: ${info.title}`);
    });

    test.afterEach(async ({}, info) => {
        console.log(`Finished: ${info.title} → ${info.status}\n`);
    });
    
    // Happy Path
    test('happy path: valid UUID returns course details', async ({ request }) => {
        expect(sampleCourseId).toMatch(UUID_REGEX);

        const api = new ApiHelper(request);
        const response = await api.get(apiConfig.endpoints.courseById(sampleCourseId));

        ResponseValidator.expectSuccess(response);
        await ResponseValidator.expectJsonSchema(response, courseByIdSchema);

        const body = await response.json();
        expect(body && typeof body === 'object').toBe(true);

        expect(body.id).toBe(sampleCourseId);
        expect(['string', 'object']).toContain(typeof body.title);
        expect(['string', 'object']).toContain(typeof body.type);
        expect(['string', 'object']).toContain(typeof body.level);
        expect(['string', 'object']).toContain(typeof body.description);

        // Employer Addresses
        expect(Array.isArray(body.employerAddresses)).toBe(true);

        if (body.employerAddresses.length > 0) {
            const addr = body.employerAddresses[0];
            expect(['string', 'object']).toContain(typeof addr.address1);
            expect(['string', 'object']).toContain(typeof addr.postcode);

            if (addr.geoLocation) {
                expect(typeof addr.geoLocation.latitude).toBe('number');
                expect(typeof addr.geoLocation.longitude).toBe('number');
            }
        }

        expect(['string', 'object']).toContain(typeof body.providerName);
        expect(Array.isArray(body.providerAddresses)).toBe(true);

        // Date arrays
        expect(Array.isArray(body.startDates)).toBe(true);
        if (body.startDates[0]) expect(typeof body.startDates[0]).toBe('string');

        // Misc fields
        expect(['string', 'object']).toContain(typeof body.courseUrl);
        expect(['string', 'object']).toContain(typeof body.positionAvailable);

        const contentType = response.headers()['content-type']?.toLowerCase() || '';
        expect(contentType).toContain('application/json');
    });

    // Valid but unknown UUID
    test('valid but unknown UUID returns 404 OR safe 200', async ({ request }) => {
        const api = new ApiHelper(request);
        const unknownId = randomUUIDv4();

        const response = await api.get(apiConfig.endpoints.courseById(unknownId));
        const status = response.status();

        expect([200, 404]).toContain(status);
        if (status === 200) expect(typeof (await response.json())).toBe('object');
    });

    // Invalid UUID formats
    test('invalid UUID returns 400 or 404', async ({ request }) => {
        const api = new ApiHelper(request);
        const invalidIds = [
            'not-a-uuid',
            '123',
            '../etc/passwd',
            'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaaa',
            '00000000-0000-0000-0000-00000000000Z',
            ' ',
            'x'.repeat(300),
            '123e4567-e89b-92d3-a456-426614174000', // wrong version
            '123e4567-e89b-42d3-c456-426614174000', // wrong variant
        ];

        for (const id of invalidIds) {
            const res = await api.get(apiConfig.endpoints.courseById(id));
            expect([400, 404]).toContain(res.status());
        }
    });

    // All-zero GUID
    test('all-zero GUID returns 400 or 404', async ({ request }) => {
        const api = new ApiHelper(request);
        const zero = '00000000-0000-0000-0000-000000000000';
        const res = await api.get(apiConfig.endpoints.courseById(zero));
        expect([400, 404]).toContain(res.status());
    });

    // POST not allowed
    test('POST to /api/Courses returns 404 or 405', async ({ request }) => {
        const api = new ApiHelper(request);
        const res = await api.post(apiConfig.endpoints.coursesBase, { junk: true });
        expect([404, 405]).toContain(res.status());
    });
});
