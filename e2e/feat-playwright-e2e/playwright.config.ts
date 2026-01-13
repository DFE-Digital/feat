import { defineConfig, devices } from '@playwright/test';
import * as dotenv from 'dotenv';
import * as path from 'path';

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
        //clean context
        {
            name: 'Cookies (clean) – Desktop',
            testDir: './src/ui/tests',
            testMatch: '**/cookies.spec.ts',
            use: {
                ...devices['Desktop Chrome'],
                baseURL:
                    process.env.FEAT_UI_BASE_URL ||
                    'https://s265d01-app-web.azurewebsites.net/',
            },
        },
        // UI Project
        {
            name: 'Desktop Chromium',
            testDir: './src/ui/tests',
            testIgnore: '**/cookies.spec.ts',
            use: {
                ...devices['Desktop Chrome'],
                baseURL:
                    process.env.FEAT_UI_BASE_URL ||
                    'https://s265d01-app-web.azurewebsites.net/',
                storageState: path.resolve('src/ui/.auth/storageState.accepted.json'),
            },
        },
        {
            name: 'Mobile Chrome',
            testDir: './src/ui/tests',
            testIgnore: '**/cookies.spec.ts',
            use: {
                ...devices['Pixel 8'],
                isMobile: true,
                baseURL:
                    process.env.FEAT_UI_BASE_URL ||
                    'https://s265d01-app-web.azurewebsites.net/',
                storageState: path.resolve('src/ui/.auth/storageState.accepted.json'),
            },
        },
        {
            name: 'Mobile Safari',
            testDir: './src/ui/tests',
            testIgnore: '**/cookies.spec.ts',
            use: {
                ...devices['iPhone 16'],
                baseURL:
                    process.env.FEAT_UI_BASE_URL ||
                    'https://s265d01-app-web.azurewebsites.net/',
                storageState: path.resolve('src/ui/.auth/storageState.accepted.json'),
            },
        },

        // API Project
        {
            name: 'API',
            testDir: './src/api/tests',   // API test files live here
            use: {
                // No browser needed
                baseURL: process.env.FEAT_API_BASE_URL || 'https://s265d01-app-api.azurewebsites.net',
                extraHTTPHeaders: {
                    'Content-Type': 'application/json',
                    Accept: '*/*',
                    //Authorization: `Bearer ${process.env.FEAT_API_TOKEN || ''}`,
                },
            },
        },
    ],
});
