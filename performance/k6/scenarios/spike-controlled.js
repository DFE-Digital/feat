export const options = {
    scenarios: {
        ramp_to_three_rps: {
            executor: 'ramping-arrival-rate',
            startRate: 1,
            timeUnit: '1s',
            preAllocatedVUs: 10,
            maxVUs: 60,
            stages: [
                { target: 2, duration: '3m' },  // warm up
                { target: 3, duration: '5m' },  // sustain 3 RPS
                { target: 0, duration: '2m' }   // ramp down
            ]
        }
    }
};