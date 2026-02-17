import { SharedArray } from 'k6/data';

const payloads = new SharedArray('featSearchPayloads', () => {
    return JSON.parse(open('../data/searchPayloads.json'));
});

function deepClone(obj) {
    return JSON.parse(JSON.stringify(obj));
}

function pickBasePayload() {
    const index = Math.floor(Math.random() * payloads.length);
    return payloads[index];
}

function stableSessionId() {
    // Useful for correlation while still being simple
    return `k6-vu-${__VU}`;
}

/**
 * Builds FEAT Search request bodies for:
 * - initial search
 * - filtered search
 * - pagination (same endpoint, page increments)
 */
export function buildSearchRequest(journeyType) {
    const base = pickBasePayload();
    const request = deepClone(base.request);

    // Ensure correct types / defaults
    request.sessionId = request.sessionId ?? stableSessionId();
    request.page = request.page ?? 1;
    request.pageSize = request.pageSize ?? 10;
    request.radius = request.radius ?? 10;
    request.orderBy = request.orderBy ?? 'Relevance';
    request.query = Array.isArray(request.query) && request.query.length ? request.query : [''];

    if (journeyType === 'paginate') {
        request.page = Math.random() < 0.7 ? 2 : 3;
    }

    if (journeyType === 'filtered') {
        request.page = 1;

        // Apply realistic defaults if null
        request.courseType = request.courseType ?? ['EssentialSkills'];
        request.qualificationLevel = request.qualificationLevel ?? ['Entry'];
        request.learningMethod = request.learningMethod ?? ['Online'];
        request.courseHours = request.courseHours ?? ['FullTime'];
        request.studyTime = request.studyTime ?? ['Evening'];

        // If location exists, distance ordering is plausible
        if (request.location) request.orderBy = Math.random() < 0.5 ? 'Distance' : 'Relevance';
    }

    return { name: base.name, request };
}

export function pickAutocompleteQuery() {
    // Swagger: min length 3
    const options = ['lon', 'man', 'bir', 'cro', 'bex', 'lea', 'se9'];
    return options[Math.floor(Math.random() * options.length)];
}
