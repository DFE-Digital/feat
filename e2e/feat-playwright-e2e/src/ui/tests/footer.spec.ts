import { test } from '@playwright/test';
import { Footer } from '../pages/Footer';

test.describe('Footer', () => {
    test('renders expected links and destinations', async ({ page }) => {
        await page.goto('/');

        const footer = new Footer(page);
        await footer.checkLinks();
    });
});
