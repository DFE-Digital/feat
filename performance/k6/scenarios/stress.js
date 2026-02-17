export const options = {
    scenarios: {
        stress: {
            executor: 'ramping-arrival-rate',
            startRate: 6,
            timeUnit: '1s',
            preAllocatedVUs: 50,
            maxVUs: 300,
            stages: [
                { target: 6, duration: '1m' },
                { target: 12, duration: '1m' },
                { target: 0, duration: '30s' }
            ]
        }
    },
    thresholds: {
        feat_search_error_rate: ['rate<0.05'],
        feat_details_error_rate: ['rate<0.05'],
        feat_autocomplete_error_rate: ['rate<0.05']
    }
};
