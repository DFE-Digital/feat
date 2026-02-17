export function getEnv(name) {
    const value = __ENV[name];
    if (!value) {
        throw new Error(`Missing required env var: ${name}`);
    }
    return value;
}

export function getBaseUrl() {
    return getEnv('BASE_URL');
}
