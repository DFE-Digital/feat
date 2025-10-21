// src/api/schemas/searchResponseSchema.ts

/**
 * JSON Schema for validating /api/Search responses.
 * Used by ResponseValidator to ensure the structure is consistent.
 */
export const searchResponseSchema = {
    type: 'object',
    required: ['page', 'pageSize', 'courses', 'facets'],

    properties: {
        page: { type: 'integer', minimum: 1 },
        pageSize: { type: 'integer', minimum: 1 },
        totalCount: { type: ['integer', 'null'] },

        courses: {
            type: 'array',
            items: {
                type: 'object',
                required: ['providerName', 'id'],
                properties: {
                    score: { type: ['number', 'null'] },
                    courseName: { type: ['string', 'null'] },
                    whoThisCourseIsFor: { type: ['string', 'null'] },
                    providerName: { type: 'string' },
                    startDate: { type: ['string', 'null'] },
                    entryRequirements: { type: ['string', 'null'] },
                    deliveryMode: { type: ['string', 'null'] },
                    duration: { type: ['string', 'null'] },
                    cost: { type: ['string', 'null'] },
                    source: { type: ['string', 'null'] },
                    postcodeEmpty: { type: ['boolean', 'null'] },
                    learningAIMTitle: { type: ['string', 'null'] },
                    ssaT1: { type: ['string', 'null'] },
                    ssaT2: { type: ['string', 'null'] },
                    skillsForLifeDescription: { type: ['string', 'null'] },
                    id: { type: 'string' },
                },
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
