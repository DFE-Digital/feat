export interface RequestOptions {
    headers?: Record<string, string>;
    timeout?: number;
}

export type SimpleResponse = {
    url: string;
    status(): number;
    ok(): boolean;
    headers(): Record<string, string>;
    text(): Promise<string>;
    json(): Promise<any>;
};

export class ApiHelperFetch {
    constructor(private baseURL: string) {}

    async post(endpoint: string, body: any, options: RequestOptions = {}): Promise<SimpleResponse> {
        const url = this.resolveUrl(endpoint);

        const controller = new AbortController();
        const timeoutMs = options.timeout ?? 30_000;
        const t = setTimeout(() => controller.abort(), timeoutMs);

        const headers: Record<string, string> = {
            "Content-Type": "application/json",
            Accept: "application/json",
            ...(options.headers ?? {}),
        };

        try {
            const res = await fetch(url, {
                method: "POST",
                headers,
                body: JSON.stringify(body),
                signal: controller.signal,
            });

            const raw = await res.text();

            const headersObj: Record<string, string> = {};
            res.headers.forEach((v, k) => (headersObj[k] = v));

            return {
                url,
                status: () => res.status,
                ok: () => res.ok,
                headers: () => headersObj,
                text: async () => raw,
                json: async () => (raw ? JSON.parse(raw) : {}),
            };
        } finally {
            clearTimeout(t);
        }
    }

    private resolveUrl(endpoint: string): string {
        const base = this.baseURL.replace(/\/+$/, "");
        const path = endpoint.startsWith("/") ? endpoint : `/${endpoint}`;
        return `${base}${path}`;
    }
}
