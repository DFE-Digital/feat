import { getBaseUrl } from './env.js';

export function searchEndpoint() {
    // FEAT endpoint (case-sensitive)
    return `${getBaseUrl()}/api/Search`;
}

export function courseDetailsEndpoint(instanceId) {
    return `${getBaseUrl()}/api/Courses/${instanceId}`;
}

export function autocompleteLocationsEndpoint(query) {
    return `${getBaseUrl()}/api/AutocompleteLocations?query=${encodeURIComponent(query)}`;
}
