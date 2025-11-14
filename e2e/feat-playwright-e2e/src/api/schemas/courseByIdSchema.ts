export const courseByIdSchema = {
    type: 'object',
    required: ['id'],
    additionalProperties: true,
    properties: {
        id: { type: 'string' },

        title: { type: ['string', 'null'] },
        type: { type: ['string', 'null'] },
        level: { type: ['string', 'null'] },
        entryRequirements: { type: ['string', 'null'] },
        description: { type: ['string', 'null'] },
        deliveryMode: { type: ['string', 'null'] },
        duration: { type: ['string', 'null'] },
        hoursType: { type: ['string', 'null'] },

        employerName: { type: ['string', 'null'] },
        employerDescription: { type: ['string', 'null'] },

        employerAddresses: {
            type: 'array',
            items: {
                type: 'object',
                additionalProperties: true,
                properties: {
                    address1: { type: ['string', 'null'] },
                    address2: { type: ['string', 'null'] },
                    address3: { type: ['string', 'null'] },
                    address4: { type: ['string', 'null'] },
                    town: { type: ['string', 'null'] },
                    county: { type: ['string', 'null'] },
                    postcode: { type: ['string', 'null'] },
                    geoLocation: {
                        type: ['object', 'null'],
                        additionalProperties: true,
                        properties: {
                            latitude: { type: ['number', 'null'] },
                            longitude: { type: ['number', 'null'] },
                        },
                    },
                },
            },
        },

        providerName: { type: ['string', 'null'] },
        providerAddresses: {
            type: 'array',
            items: {
                type: 'object',
                additionalProperties: true,
                properties: {
                    address1: { type: ['string', 'null'] },
                    address2: { type: ['string', 'null'] },
                    address3: { type: ['string', 'null'] },
                    address4: { type: ['string', 'null'] },
                    town: { type: ['string', 'null'] },
                    county: { type: ['string', 'null'] },
                    postcode: { type: ['string', 'null'] },
                    geoLocation: {
                        type: ['object', 'null'],
                        additionalProperties: true,
                        properties: {
                            latitude: { type: ['number', 'null'] },
                            longitude: { type: ['number', 'null'] },
                        },
                    },
                },
            },
        },

        providerUrl: { type: ['string', 'null'] },

        startDates: {
            type: 'array',
            items: { type: ['string', 'null'] },
        },

        costs: {
            type: 'array',
            items: { type: ['number', 'string', 'null'] },
        },

        wage: { type: ['number', 'string', 'null'] },
        tuitionFee: { type: ['number', 'string', 'null'] },
        positionAvailable: { type: ['string', 'null'] },
        trainingProvider: { type: ['string', 'null'] },
        awardingOrganisation: { type: ['string', 'null'] },
        university: { type: ['string', 'null'] },
        campusName: { type: ['string', 'null'] },
        courseUrl: { type: ['string', 'null'] },
    },
} as const;
