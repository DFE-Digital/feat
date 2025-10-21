import { apiConfig } from '../config/apiConfig';

export interface SearchRequestBody {
    query: string;
    sessionId: string | null;
    page: number;
    pageSize: number;
    location: string | null;
    radius: number;
    includeOnlineCourses: boolean;
    orderBy: string;
}

/**
 * TestDataFactory:
 * A builder class for creating request payloads for the /api/Search endpoint.
 * Uses sensible defaults from apiConfig.defaults.
 */
export class TestDataFactory {
    private requestBody: SearchRequestBody;

    constructor() {
        const { sessionId, location, radius, includeOnlineCourses, orderBy } = apiConfig.defaults;

        this.requestBody = {
            query: 'digital',
            sessionId,
            page: apiConfig.pagination.defaultPage,
            pageSize: apiConfig.pagination.defaultPageSize,
            location,
            radius,
            includeOnlineCourses,
            orderBy,
        };
    }

    withQuery(query: string): this {
        this.requestBody.query = query;
        return this;
    }

    withPage(page: number): this {
        this.requestBody.page = page;
        return this;
    }

    withPageSize(size: number): this {
        this.requestBody.pageSize = size;
        return this;
    }

    withLocation(location: string | null): this {
        this.requestBody.location = location;
        return this;
    }

    withRadius(radius: number): this {
        this.requestBody.radius = radius;
        return this;
    }

    includeOnlineCourses(include: boolean): this {
        this.requestBody.includeOnlineCourses = include;
        return this;
    }

    orderBy(order: 'Relevance' | 'Distance' | 'Date'): this {
        this.requestBody.orderBy = order;
        return this;
    }

    build(): SearchRequestBody {
        return { ...this.requestBody };
    }

    // ---------- Predefined Scenarios ----------

    //Valid POST payload (happy path)
    static validSearch(): SearchRequestBody {
        return new TestDataFactory().withQuery('car').build();
    }

    //Missing query field 
    static missingQuery(): SearchRequestBody {
        const data = new TestDataFactory().build();
        delete (data as any).query;
        return data;
    }

    //Empty query string 
    static emptyQuery(): SearchRequestBody {
        return new TestDataFactory().withQuery('').build();
    }

    //Page size larger than allowed limit 
    static largePageSize(): SearchRequestBody {
        return new TestDataFactory()
            .withQuery('car')
            .withPageSize(apiConfig.pagination.maxPageSize + 50)
            .build();
    }

    //Invalid page number (negative)
    static invalidPageNumber(): SearchRequestBody {
        return new TestDataFactory().withQuery('car').withPage(-1).build();
    }

    //Search filtered by location and radius 
    static searchWithLocation(): SearchRequestBody {
        return new TestDataFactory()
            .withQuery('design')
            .withLocation('Leeds')
            .withRadius(25)
            .orderBy('Distance')
            .build();
    }
}
