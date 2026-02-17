import http from 'k6/http';
import { sleep } from 'k6';
import { Trend, Rate } from 'k6/metrics';

import { buildHeaders } from '../helpers/headers.js';
import {
    searchEndpoint,
    courseDetailsEndpoint,
    autocompleteLocationsEndpoint
} from '../helpers/endpoints.js';

import { buildSearchRequest, pickAutocompleteQuery } from '../helpers/requestBuilder.js';
import {
    validateSearchResponse,
    validateCourseDetailsResponse,
    validateAutocompleteResponse
} from '../helpers/validate.js';

import { options as baseline } from '../scenarios/baseline.js';
import { options as peak } from '../scenarios/peak.js';
import { options as spike } from '../scenarios/spike.js';
import { options as stress } from '../scenarios/stress.js';

//Custom metrics (per endpoint + journey)
export const feat_search_duration_ms = new Trend('feat_search_duration_ms', true);
export const feat_search_initial_duration_ms = new Trend('feat_search_initial_duration_ms', true);
export const feat_search_filtered_duration_ms = new Trend('feat_search_filtered_duration_ms', true);
export const feat_search_paginate_duration_ms = new Trend('feat_search_paginate_duration_ms', true);

export const feat_details_duration_ms = new Trend('feat_details_duration_ms', true);
export const feat_autocomplete_duration_ms = new Trend('feat_autocomplete_duration_ms', true);

export const feat_search_error_rate = new Rate('feat_search_error_rate');
export const feat_details_error_rate = new Rate('feat_details_error_rate');
export const feat_autocomplete_error_rate = new Rate('feat_autocomplete_error_rate');

const scenario = (__ENV.SCENARIO || 'baseline').toLowerCase();

export const options =
    scenario === 'peak' ? peak :
        scenario === 'spike' ? spike :
            scenario === 'stress' ? stress :
                baseline;

export default function () {
    const headers = buildHeaders();

    // 55% initial, 25% filtered, 20% paginate
    const r = Math.random();
    const journeyType = r < 0.55 ? 'initial' : (r < 0.80 ? 'filtered' : 'paginate');

    //Search
    const { name, request } = buildSearchRequest(journeyType);

    const searchRes = http.post(
        searchEndpoint(),
        JSON.stringify(request),
        {
            headers,
            tags: { endpoint: 'Search', journey: journeyType, payload: name }
        }
    );
    if (searchRes.status !== 200) {
        console.error(
            `Search failed: status=${searchRes.status} journey=${journeyType} payload=${name} body=${searchRes.body}`
        );
    }

    // Metrics for Search
    feat_search_duration_ms.add(searchRes.timings.duration);
    feat_search_error_rate.add(searchRes.status !== 200);

    if (journeyType === 'initial') feat_search_initial_duration_ms.add(searchRes.timings.duration);
    if (journeyType === 'filtered') feat_search_filtered_duration_ms.add(searchRes.timings.duration);
    if (journeyType === 'paginate') feat_search_paginate_duration_ms.add(searchRes.timings.duration);

    const { body } = validateSearchResponse(searchRes);

    //Course details ~40%
    if (body && Array.isArray(body.courses) && body.courses.length > 0 && Math.random() < 0.40) {
        const picked = body.courses[Math.floor(Math.random() * body.courses.length)];
        const instanceId = picked.instanceId;

        if (instanceId) {
            const detailsRes = http.get(
                courseDetailsEndpoint(instanceId),
                { headers, tags: { endpoint: 'Courses', journey: 'details' } }
            );

            feat_details_duration_ms.add(detailsRes.timings.duration);
            feat_details_error_rate.add(detailsRes.status !== 200);

            validateCourseDetailsResponse(detailsRes);
        }
    }

    // Autocomplete ~50%
    if (Math.random() < 0.50) {
        const q = pickAutocompleteQuery();
        const autoRes = http.get(
            autocompleteLocationsEndpoint(q),
            { headers, tags: { endpoint: 'AutocompleteLocations', journey: 'autocomplete' } }
        );

        feat_autocomplete_duration_ms.add(autoRes.timings.duration);
        feat_autocomplete_error_rate.add(autoRes.status !== 200);

        validateAutocompleteResponse(autoRes);
    }

    sleep(1);
}
