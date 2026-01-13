export const searchResponseSchema = {
    type: 'object',
    required: ['page', 'pageSize', 'totalCount', 'courses', 'facets'],
    properties: {
        page: { type: 'integer', minimum: 1 },
        pageSize: { type: 'integer', minimum: 1 },
        totalCount: { type: 'integer', minimum: 0 },

        courses: {
            type: 'array',
            items: {
                type: 'object',
                required: ['id', 'instanceId', 'title', 'provider', 'location', 'score'],
                properties: {
                    id: { type: 'string' },                 // uuid (string)
                    instanceId: { type: 'string' },         // uuid (string)
                    title: { type: ['string', 'null'] },
                    provider: { type: ['string', 'null'] },
                    courseType: { type: ['string', 'null'] },
                    requirements: { type: ['string', 'null'] },
                    overview: { type: ['string', 'null'] },
                    locationName: { type: ['string', 'null'] },
                    distance: { type: ['number', 'null'] },
                    score: { type: ['number', 'null'] },
                    location: {
                        type: 'object',
                        required: ['latitude', 'longitude'],
                        properties: {
                            latitude: { type: 'number' },
                            longitude: { type: 'number' },
                        },
                    },
                },
                additionalProperties: true,
            },
        },

        facets: {
            type: 'array',
            items: {
                type: 'object',
                required: ['name', 'values'],
                properties: {
                    name: { type: 'string' },
                    values: {
                        type: 'object',
                        additionalProperties: { type: 'integer' },
                    },
                },
            },
        },
    },
};
