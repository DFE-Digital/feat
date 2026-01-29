import { APIRequestContext, APIResponse } from '@playwright/test';
import { apiConfig } from '../config/apiConfig';

export interface RequestOptions {
    headers?: Record<string, string>;
    timeout?: number;
}

export class ApiHelper {
    constructor(private request: APIRequestContext) {}

    private resolveUrl(endpoint: string): string {
        if (endpoint.startsWith('http://') || endpoint.startsWith('https://')) {
            return endpoint;
        }
        const base = apiConfig.baseUrl.replace(/\/$/, '');
        const path = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
        return `${base}${path}`;
    }
    
    // Executes a POST request to the given endpoint
    async post(endpoint: string, body: any, options: RequestOptions = {}): Promise<APIResponse> {
        const url = this.resolveUrl(endpoint);

        const headers = {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            ...(options.headers ?? {}),
        };
        
        const res = await this.request.post(url, {
            data: body,
            headers,
            timeout: options.timeout,
        });
        return res;
    }

    // Executes a GET request to the given endpoint
    async get(endpoint: string, options: RequestOptions = {}): Promise<APIResponse> {
        const url = this.resolveUrl(endpoint);

        const headers = {
            Accept: 'application/json',
            ...(options.headers ?? {}),
        };
        const res = await this.request.get(url, {
            headers,
            timeout: options.timeout,
        });
        return res;
    }
}
