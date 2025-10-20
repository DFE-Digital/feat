export const apiConfig = {
    // Endpoint paths only (baseURL comes from Playwright config)
    endpoints: {
        search: '/api/Search',
    },

    // Logical defaults for API payloads
    pagination: {
        defaultPage: 1,
        defaultPageSize: 10,
        minPageSize: 1,
        maxPageSize: 100,
    },

    // Shared default body parameters
    defaults: {
        sessionId: null,
        location: null,
        radius: 1,
        includeOnlineCourses: true,
        orderBy: 'Relevance',
    },
};
