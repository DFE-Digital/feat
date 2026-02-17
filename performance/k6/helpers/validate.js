import { check } from 'k6';

export function validateSearchResponse(res) {
    let body = null;
    try { body = res.json(); } catch (_) {}

    check(res, {
        'Search: status 200': (r) => r.status === 200,
        'Search: JSON body': () => body !== null,
    });

    if (!body) return { body: null };

    check(body, {
        'Search: has page': (b) => typeof b.page === 'number',
        'Search: has pageSize': (b) => typeof b.pageSize === 'number',
        'Search: has totalCount': (b) => typeof b.totalCount === 'number',
        'Search: courses is array': (b) => Array.isArray(b.courses),
        'Search: facets is array': (b) => Array.isArray(b.facets),
    });

    // Light sanity checks on first course
    if (Array.isArray(body.courses) && body.courses.length > 0) {
        const c = body.courses[0];
        check(c, {
            'Search course: has instanceId': (x) => typeof x.instanceId === 'string' && x.instanceId.length > 0,
            'Search course: has title': (x) => typeof x.title === 'string',
        });
    }

    return { body };
}

export function validateCourseDetailsResponse(res) {
    let body = null;
    try { body = res.json(); } catch (_) {}

    check(res, {
        'Details: status 200': (r) => r.status === 200,
        'Details: JSON body': () => body !== null,
    });

    if (!body) return;

    check(body, {
        'Details: has id': (b) => typeof b.id === 'string' && b.id.length > 0,
        'Details: has entryType': (b) => typeof b.entryType === 'string' || b.entryType === null,
        'Details: has courseType': (b) => typeof b.courseType === 'string' || b.courseType === null,
        'Details: has deliveryMode': (b) => typeof b.deliveryMode === 'string' || b.deliveryMode === null,
    });
}

export function validateAutocompleteResponse(res) {
    let body = null;
    try { body = res.json(); } catch (_) {}

    check(res, {
        'Autocomplete: status 200': (r) => r.status === 200,
        'Autocomplete: JSON body': () => body !== null,
    });

    if (!body) return;

    check(body, {
        'Autocomplete: returns array': (b) => Array.isArray(b),
    });

    if (Array.isArray(body) && body.length > 0) {
        const first = body[0];
        check(first, {
            'Autocomplete: has name': (x) => typeof x.name === 'string',
            'Autocomplete: has latitude': (x) => typeof x.latitude === 'number',
            'Autocomplete: has longitude': (x) => typeof x.longitude === 'number',
        });
    }
}
