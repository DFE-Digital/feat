import { APIRequestContext, APIResponse } from '@playwright/test';

export interface RequestOptions {
    headers?: Record<string, string>;
    timeout?: number;
}

export class ApiHelper {
    constructor(private request: APIRequestContext) {}

    //Executes a POST request to the given endpoint 
    async post(endpoint: string, body: any, options: RequestOptions = {}): Promise<APIResponse> {
        return await this.request.post(endpoint, {
            data: body,
            headers: options.headers,
            timeout: options.timeout,
        });
    }
    
    //Executes a GET request to the given endpoint
    async get(endpoint: string, options: RequestOptions = {}): Promise<APIResponse> {
        return await this.request.get(endpoint, {
            headers: options.headers,
            timeout: options.timeout,
        });
    }
}
