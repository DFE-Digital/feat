import { Page } from '@playwright/test';
import { CookieBanner } from '../pages/cookieBanner';

export async function acceptCookiesIfVisible(page: Page) {
    const banner = new CookieBanner(page);

    try {
        if (await banner.container().isVisible({ timeout: 1000 })) {
            await banner.accept();
        }
    } catch {
        // Intentionally ignore:
        // - banner not present
        // - banner already accepted
        // - race conditions on fast loads
    }
}
