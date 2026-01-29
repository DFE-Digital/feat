function requireEnv(name: string): string {
    const v = process.env[name];
    if (!v) throw new Error(`Missing required env var: ${name}`);
    return v;
}

const apiBaseUrl = requireEnv("FEAT_API_BASE_URL").replace(/\/+$/, "");

export const apiConfig = {
    baseUrl: apiBaseUrl,
    endpoints: {
        search: "/api/Search",
        autocompleteLocations: "/api/AutocompleteLocations",

        coursesBase: '/api/Courses',
        courseById: (instanceId: string) => `/api/Courses/${encodeURIComponent(instanceId)}`,
    },
    pagination: {
        defaultPage: 1,
        defaultPageSize: 10,
        maxPageSize: 100,
    },
    defaults: {
        sessionId: null as string | null,
        location: null as string | null,
        radius: 1,
        includeOnlineCourses: true,
        orderBy: "Relevance" as "Relevance" | "Distance" | "Date",
    },
};
