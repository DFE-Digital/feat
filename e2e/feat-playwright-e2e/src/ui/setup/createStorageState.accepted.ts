import { chromium } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

async function createStorageStateAccepted() {
    const baseURL =
        process.env.FEAT_UI_BASE_URL || 'https://s265d01-app-web.azurewebsites.net/';

    const outDir = path.resolve('src/ui/.auth');
    const outFile = path.join(outDir, 'storageState.accepted.json');

    if (!fs.existsSync(outDir)) {
        fs.mkdirSync(outDir, { recursive: true });
    }

    const browser = await chromium.launch();
    const context = await browser.newContext();
    const page = await context.newPage();

    await page.goto(baseURL, { waitUntil: 'networkidle' });

    // Accept cookies ONCE (banner is expected to appear in a fresh context)
    await page.getByRole('button', { name: /accept analytics cookies/i }).click();

    // Save cookies + localStorage into storageState file
    await context.storageState({ path: outFile });

    await browser.close();

    console.log(`Saved accepted cookie storageState to: ${outFile}`);
}

createStorageStateAccepted().catch((err) => {
    console.error('Failed to create storageState:', err);
    process.exit(1);
});
