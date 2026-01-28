import { chromium } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';
import { acceptCookiesIfVisible } from './src/ui/helpers/cookies';

export default async function globalSetup() {
    const baseURL =
        process.env.FEAT_UI_BASE_URL || 'https://s265d01-app-web.azurewebsites.net/';

    const authDir = path.resolve('src/ui/.auth');
    const storagePath = path.join(authDir, 'storageState.accepted.json');

    fs.mkdirSync(authDir, { recursive: true });

    const browser = await chromium.launch();
    const context = await browser.newContext();
    const page = await context.newPage();

    await page.goto(baseURL, { waitUntil: 'domcontentloaded' });
    
    await acceptCookiesIfVisible(page);

    await context.storageState({ path: storagePath });
    await browser.close();

    console.log(`Saved cookie storage state to ${storagePath}`);
}
