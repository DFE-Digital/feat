import { defineConfig, devices } from '@playwright/test';
import * as dotenv from 'dotenv';
dotenv.config();

export default defineConfig({
    testDir: './src',
    testMatch: '**/*.spec.ts',
    timeout: 30_000,
    expect: { timeout: 10_000 },
    fullyParallel: true,
    retries: 1,
    workers: 4,
    reporter: [['list'], ['html', { open: 'never' }]],
    use: {
        trace: 'on-first-retry',
        screenshot: 'only-on-failure',
        video: 'retain-on-failure',
        //baseURL: process.env.FEAT_BASE_URL || 'http://localhost:3000',
        ignoreHTTPSErrors: true,
        headless: true,
    },
    projects: [
        // UI Project
        { name: 'Chromium', use: { browserName: 'chromium' } },
        {
            name: 'Mobile Chrome',
            use: { browserName: 'chromium', ...devices['Pixel 8'], isMobile: true },
        },
        {
            name: 'Mobile Safari',
            use: { browserName: 'webkit', ...devices['iPhone 16'] },
        },

        // API Project
        {
            name: 'API',
            testDir: './src/api/tests',   // API test files live here
            use: {
                // No browser needed
                baseURL: process.env.FEAT_API_BASE_URL || 'https://s213d01-app-api.azurewebsites.net',
                extraHTTPHeaders: {
                    'Content-Type': 'application/json',
                    Accept: '*/*',
                    //Authorization: `Bearer ${process.env.FEAT_API_TOKEN || ''}`,
                },
            },
        },
    ],
});
