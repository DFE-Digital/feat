import { test, expect } from '@playwright/test';
import { apiConfig } from '../config/apiConfig';
import { ApiHelper } from '../helpers/apiHelper';
import { ResponseValidator } from '../helpers/responseValidator';
import { courseByIdSchema } from '../schemas/courseByIdSchema';

// UUID format validation 
const UUID_REGEX =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

// InstanceId taken from a real UI course details page:https://s265d01-app-web.azurewebsites.net/courses/{instanceId}
const defaultSampleInstanceId = '9a8e01d2-c58e-4e60-e305-08de252620ac';

// Allow override via env for flexibility across environments
const sampleInstanceId =
    process.env.FEAT_SAMPLE_COURSE_ID || defaultSampleInstanceId;

// Simple UUID v4 generator for testing "valid but unknown" IDs
function randomUUIDv4(): string {
    const hex = [...Array(256).keys()].map(i => (i + 0x100).toString(16).slice(1));
    const rnd = new Uint8Array(16);
    for (let i = 0; i < 16; i++) rnd[i] = Math.floor(Math.random() * 256);
    rnd[6] = (rnd[6] & 0x0f) | 0x40; // version 4
    rnd[8] = (rnd[8] & 0x3f) | 0x80; // variant
    const b = rnd;
    return `${hex[b[0]]}${hex[b[1]]}${hex[b[2]]}${hex[b[3]]}-${hex[b[4]]}${hex[b[5]]}-${hex[b[6]]}${hex[b[7]]}-${hex[b[8]]}${hex[b[9]]}-${hex[b[10]]}${hex[b[11]]}${hex[b[12]]}${hex[b[13]]}${hex[b[14]]}${hex[b[15]]}`;
}

test.describe('GET /api/Courses/{instanceId}', () => {
    test.beforeEach(async ({}, info) => {
        console.log(`Starting test: ${info.title}`);
    });

    test.afterEach(async ({}, info) => {
        console.log(`Finished: ${info.title} → ${info.status}\n`);
    });

    //Happy path-Valid instanceId, 200 response, Matches schema, Key fields present and correctly typed
    test('happy path: valid UUID returns course details', async ({ request }) => {
        // smoke test to avoid false failures due to bad test data
        expect(sampleInstanceId).toMatch(UUID_REGEX);

        const api = new ApiHelper(request);
        const response = await api.get(
            apiConfig.endpoints.courseById(sampleInstanceId)
        );

        // Assert successful response (2xx)
        await ResponseValidator.expectSuccess(response);

        // Contract-level validation against agreed schema
        await ResponseValidator.expectJsonSchema(response, courseByIdSchema);

        const body = await response.json();

        // Basic structure check
        expect(body && typeof body === 'object').toBe(true);

        // ID should match the requested instanceId
        expect(body.id).toBe(sampleInstanceId);

        // Nullable string fields (null OR string as per schema)
        expect(body.title === null || typeof body.title === 'string').toBe(true);
        expect(body.entryType === null || typeof body.entryType === 'string').toBe(true);
        expect(body.level === null || typeof body.level === 'string').toBe(true);
        expect(body.description === null || typeof body.description === 'string').toBe(true);

        // Employer addresses array should always be present
        expect(Array.isArray(body.employerAddresses)).toBe(true);

        // If an address exists, validate its structure
        if (body.employerAddresses.length > 0) {
            const addr = body.employerAddresses[0];

            expect(addr.address1 === null || typeof addr.address1 === 'string').toBe(true);
            expect(addr.postcode === null || typeof addr.postcode === 'string').toBe(true);

            // Geo location is optional, but must be numeric if present
            if (addr.geoLocation) {
                expect(typeof addr.geoLocation.latitude).toBe('number');
                expect(typeof addr.geoLocation.longitude).toBe('number');
            }
        }

        // Provider information
        expect(body.providerName === null || typeof body.providerName === 'string').toBe(true);
        expect(Array.isArray(body.providerAddresses)).toBe(true);

        // Start dates should always be an array
        expect(Array.isArray(body.startDates)).toBe(true);
        if (body.startDates[0]) {
            expect(typeof body.startDates[0]).toBe('string');
        }

        // Misc optional fields
        expect(body.courseUrl === null || typeof body.courseUrl === 'string').toBe(true);

        // positionsAvailable can be null, number, or string 
        expect(
            body.positionsAvailable === null ||
            typeof body.positionsAvailable === 'number' ||
            typeof body.positionsAvailable === 'string'
        ).toBe(true);

        // API sometimes returns JSON with text/plain content-type
        const contentType = response.headers()['content-type']?.toLowerCase() || '';
        expect(contentType).toMatch(/application\/json|text\/plain/);
    });
    
     //Valid UUID that does not exist- API may return 404 and Some environments may return a safe 200 with an empty object
    test('valid but unknown UUID returns 404 OR safe 200', async ({ request }) => {
        const api = new ApiHelper(request);
        const unknownId = randomUUIDv4();

        const response = await api.get(
            apiConfig.endpoints.courseById(unknownId)
        );

        const status = response.status();
        expect([200, 404]).toContain(status);
        
        if (status === 200) {
            expect(typeof (await response.json())).toBe('object');
        }
    });
    
     //Invalid UUID formats should not be accepted
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

    //All-zero GUID should be rejected
    test('all-zero GUID returns 400 or 404', async ({ request }) => {
        const api = new ApiHelper(request);
        const zero = '00000000-0000-0000-0000-000000000000';

        const res = await api.get(apiConfig.endpoints.courseById(zero));
        expect([400, 404]).toContain(res.status());
    });

    //POST is not supported on this endpoint
    test('POST to /api/Courses returns 404 or 405', async ({ request }) => {
        const api = new ApiHelper(request);
        const res = await api.post(apiConfig.endpoints.coursesBase, { junk: true });

        expect([404, 405]).toContain(res.status());
    });
});
