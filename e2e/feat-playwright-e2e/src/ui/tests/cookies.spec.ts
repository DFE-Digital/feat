import { test, expect } from '@playwright/test';
import { CookieBanner } from '../pages/cookieBanner';

test.describe('FEAT – Cookies', () => {
    test('Cookie banner is shown on first visit', async ({ page, baseURL }) => {
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });

        const banner = new CookieBanner(page);
        await banner.expectVisible();
    });

    test('Accept analytics cookies hides banner, persists, and sets consent cookies', async ({
                                                                                                 page,
                                                                                                 baseURL,
                                                                                             }) => {
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });

        const banner = new CookieBanner(page);
        await banner.accept();

        // Reload -> banner should stay hidden
        await page.reload({ waitUntil: 'networkidle' });
        await banner.expectHidden();

        const cookies = await page.context().cookies();

        const analytics = cookies.find(c => c.name === 'AnalyticsCookie');
        const marketing = cookies.find(c => c.name === 'MarketingCookie');

        expect(analytics).toBeTruthy();
        expect(analytics!.value).toBe('yes');

        expect(marketing).toBeTruthy();
        expect(marketing!.value).toBe('yes');
    });

    test('Reject analytics cookies hides banner, persists, and sets consent cookies', async ({
                                                                                                 page,
                                                                                                 baseURL,
                                                                                             }) => {
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });

        const banner = new CookieBanner(page);
        await banner.reject();

        await page.reload({ waitUntil: 'networkidle' });
        await banner.expectHidden();

        const cookies = await page.context().cookies();

        const analytics = cookies.find(c => c.name === 'AnalyticsCookie');
        const marketing = cookies.find(c => c.name === 'MarketingCookie');

        expect(analytics).toBeTruthy();
        expect(analytics!.value).toBe('no');

        expect(marketing).toBeTruthy();
        expect(marketing!.value).toBe('no');
    });

    test('If cookies are cleared, banner reappears', async ({ page, baseURL }) => {
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });

        const banner = new CookieBanner(page);
        await banner.accept();

        await page.context().clearCookies();

        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });
        await banner.expectVisible();
    });

    test('View cookies link navigates to cookies page', async ({ page, baseURL }) => {
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });

        const banner = new CookieBanner(page);
        await banner.expectVisible();
        await banner.goToCookiesPage();

        await expect(page).toHaveURL(/\/linkpages\/cookies$/i);
        await expect(
            page.getByRole('heading',{ level: 1, name: /cookies on the/i })
        ).toBeVisible();
    });
});
