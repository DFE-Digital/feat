export const options = {
    scenarios: {
        peak: {
            executor: 'constant-arrival-rate',
            rate: 1,
            timeUnit: '12.5s',
            duration: '15m',
            preAllocatedVUs: 10,
            maxVUs: 50
        }
    },
    thresholds: {
        feat_search_initial_duration_ms: ['p(95)<=2000'],
        feat_search_filtered_duration_ms: ['p(95)<=2500'],
        feat_search_paginate_duration_ms: ['p(95)<=1500'],
        feat_details_duration_ms: ['p(95)<=1500'],
        feat_autocomplete_duration_ms: ['p(95)<=1000'],

        feat_search_error_rate: ['rate<0.01'],
        feat_details_error_rate: ['rate<0.01'],
        feat_autocomplete_error_rate: ['rate<0.01']
    }
};
