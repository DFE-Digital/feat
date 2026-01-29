import { test, expect } from "@playwright/test";
import Ajv from "ajv";

import { apiConfig } from "../config/apiConfig";
import { ApiHelperFetch } from "../helpers/apiHelperFetch";
import { TestDataFactory } from "../fixtures/testDataFactory";
import { searchResponseSchema } from "../schemas/searchResponseSchema";

test.describe("Search API", () => {
    const baseURL = apiConfig.baseUrl;

    // AJV v8 safe config
    const ajv = new Ajv({ allErrors: true, strict: false });
    const validate = ajv.compile(searchResponseSchema as any);

    function validateSchema(data: any) {
        const ok = validate(data);
        if (!ok) {
            const message = ajv.errorsText(validate.errors, { separator: "\n" });
            throw new Error(`Schema validation failed:\n${message}`);
        }
    }

    test("should return valid search results for 'car' query", async () => {
        const api = new ApiHelperFetch(baseURL);
        const payload = TestDataFactory.validSearch();

        const res = await api.post(apiConfig.endpoints.search, payload);

        expect(res.ok(), `Expected 2xx, got ${res.status()}`).toBeTruthy();

        const data = await res.json();
        validateSchema(data);

        expect(data.page).toBeGreaterThanOrEqual(1);
        expect(Array.isArray(data.courses)).toBeTruthy();
    });

    test("should include valid facets array", async () => {
        const api = new ApiHelperFetch(baseURL);
        const payload = TestDataFactory.validSearch();

        const res = await api.post(apiConfig.endpoints.search, payload);

        expect(res.ok(), `Expected 2xx, got ${res.status()}`).toBeTruthy();

        const data = await res.json();
        validateSchema(data);

        expect(Array.isArray(data.facets)).toBeTruthy();
        expect(data.facets.length).toBeGreaterThan(0);
    });

    test("edge: empty query should NOT 500", async () => {
        const api = new ApiHelperFetch(apiConfig.baseUrl);

        const payload = new TestDataFactory()
            .withQuery("   ")     // will become [] after trim change
            .build();

        const res = await api.post("/api/Search", payload);

        expect(
            [200, 400, 422].includes(res.status()),
            `Expected 200/400/422 for empty query, got ${res.status()}`
        ).toBeTruthy();
    });

    test("edge: large page size should return 400/422 (or clamp but MUST NOT 500)", async () => {
        const api = new ApiHelperFetch(baseURL);
        const payload = TestDataFactory.largePageSize();

        const res = await api.post(apiConfig.endpoints.search, payload);

        expect(
            [200, 400, 422].includes(res.status()),
            `Expected 200/400/422 for large page size, got ${res.status()}`
        ).toBeTruthy();
    });

    test("edge: invalid page number should return 400/422 (MUST NOT 500)", async () => {
        const api = new ApiHelperFetch(baseURL);
        const payload = TestDataFactory.invalidPageNumber();

        const res = await api.post(apiConfig.endpoints.search, payload);

        expect(
            [400, 422].includes(res.status()),
            `Expected 400/422 for invalid page, got ${res.status()}`
        ).toBeTruthy();
    });

    test("negative: missing required query field should return 400/422 (MUST NOT 500)", async () => {
        const api = new ApiHelperFetch(baseURL);
        const payload = TestDataFactory.missingQuery();

        const res = await api.post(apiConfig.endpoints.search, payload);

        expect(
            [400, 422].includes(res.status()),
            `Expected 400/422 for missing query, got ${res.status()}`
        ).toBeTruthy();
    });

    test("negative: completely invalid payload should return 400/422 (MUST NOT 500)", async () => {
        const api = new ApiHelperFetch(baseURL);

        const res = await api.post(apiConfig.endpoints.search, { invalid: "body" });

        expect(
            [400, 422].includes(res.status()),
            `Expected 400/422 for invalid body, got ${res.status()}`
        ).toBeTruthy();
    });

    test("negative: GET should return 405 (method not allowed)", async () => {
        const base = baseURL.replace(/\/+$/, "");
        const url = `${base}${apiConfig.endpoints.search}`;

        const res = await fetch(url, { method: "GET" });

        expect(res.status, `Expected 405 for GET, got ${res.status}`).toBe(405);
    });
});
