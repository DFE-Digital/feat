import { test, expect, Page } from '@playwright/test';
import { LocationPage } from '../pages/locationPage';
import { IndexPage } from '../pages/indexPage';

async function gotoHomeWithRetry(page: Page, baseURL?: string) {
    const url = baseURL || '/';

    page.setDefaultTimeout(60_000);
    page.setDefaultNavigationTimeout(60_000);

    const attempts = 3;
    let lastError: unknown;

    for (let i = 1; i <= attempts; i++) {
        try {
            await page.goto(url, { waitUntil: 'load', timeout: 60_000 });
            await page.waitForLoadState('domcontentloaded');
            return;
        } catch (e) {
            lastError = e;
            if (i === attempts) break;
            await page.waitForTimeout(800);
        }
    }

    throw lastError;
}

test.describe('FEAT – 2.0 Location', () => {
    test.beforeEach(async ({ page, baseURL }) => {
        const index = new IndexPage(page);

        await page.context().clearCookies();
        await page.goto('/', { waitUntil: 'domcontentloaded' });
        await page.evaluate(() => {
            localStorage.clear();
            sessionStorage.clear();
        });

        await gotoHomeWithRetry(page, baseURL);

        await expect(index.startNow()).toBeVisible({ timeout: 30_000 });
        await index.startNow().click();

        await expect(page).toHaveURL(/\/location/i, { timeout: 30_000 });

        await expect(page.locator('#Location')).toBeVisible({ timeout: 30_000 });
        await expect(page.locator('#Location')).toBeEnabled({ timeout: 30_000 });
    });

    test('AC1: page loads with heading and back link', async ({ page }) => {
        const loc = new LocationPage(page);
        await expect(loc.heading()).toBeVisible();
        await expect(loc.backLink()).toBeVisible();
    });

    test('AC1: optional – continue with both fields empty goes to next step', async ({ page }) => {
        const loc = new LocationPage(page);
        await loc.continueButton().click();
        await expect(page).toHaveURL(/(what-are-you-interested-in|interests)/i);
    });

    test('AC2: invalid location shows inline + summary errors (only if interacted)', async ({ page }) => {
        const loc = new LocationPage(page);

        await expect(page.locator('#Location')).toBeVisible({ timeout: 30_000 });
        await expect(page.locator('#Location')).toBeEnabled({ timeout: 30_000 });

        await loc.locationInput().fill('@@@');
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(/Select how far you would be able to travel/i);
        await expect(loc.fieldError('Select how far you would be able to travel')).toBeVisible();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC3: autocomplete – no dropdown until 3 alpha chars; appears at 3+', async ({ page }) => {
        const loc = new LocationPage(page);
        const input = loc.locationInput();

        await input.fill('Lo');

        await expect
            .poll(async () => (await input.getAttribute('aria-expanded')) ?? 'false', {
                timeout: 5_000,
            })
            .toBe('false');

        await input.type('n', { delay: 60 });

        await expect
            .poll(async () => (await input.getAttribute('aria-expanded')) ?? 'false', {
                timeout: 15_000,
            })
            .toBe('true');

        await expect
            .poll(async () => (await loc.suggestionOptions().count().catch(() => 0)) > 0, {
                timeout: 25_000,
            })
            .toBe(true);
    });

    test('AC3: autocomplete supports keyboard selection', async ({ page }) => {
        const loc = new LocationPage(page);

        const selected = await loc.enterLocationAndSelectFirst('Lon');
        expect(selected.length).toBeGreaterThan(0);

        await expect(loc.locationInput()).not.toHaveValue('');
    });

    test('AC4/AC5: distance radios present and selectable in order', async ({ page }) => {
        const loc = new LocationPage(page);

        const options = [
            'Up to 2 miles',
            'Up to 5 miles',
            'Up to 10 miles',
            'Up to 15 miles',
            'Up to 30 miles',
            'Over 30 miles',
        ];

        for (const opt of options) {
            await expect(loc.distanceRadio(opt)).toBeVisible();
        }

        await loc.selectDistance('Up to 10 miles');
    });

    test('AC4: if distance selected but no location, show location required error', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.selectDistance('Up to 10 miles');
        await loc.expectDistanceChecked('Up to 10 miles');

        const outcome = await loc.clickContinueAndWaitForErrorOrNext(20_000);

        // If we navigated, it means distance was not treated as set OR validation didn’t fire.
        expect(outcome, 'Expected validation error summary to appear, but page navigated / nothing happened').toBe('error');

        await expect(loc.errorSummary()).toContainText(/Enter a town, city or postcode to use the distance filter/i);
        await expect(loc.fieldError('Enter a town, city or postcode to use the distance filter')).toBeVisible();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC4: if location entered but no distance, show distance error', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.enterLocationAndSelectFirst('Leeds');
        await loc.expectLocationCommitted();

        const outcome = await loc.clickContinueAndWaitForErrorOrNext(20_000);
        expect(outcome, 'Expected validation error summary to appear, but page navigated / nothing happened').toBe('error');

        await expect(loc.errorSummary()).toContainText(/Select how far you would be able to travel/i);
        await expect(loc.fieldError('Select how far you would be able to travel')).toBeVisible();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC6/AC8: valid location + distance proceeds to next step', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.enterLocationAndSelectFirst('Leeds');
        await loc.selectDistance('Up to 5 miles');

        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);
    });

    test('AC7: returning to page restores location and distance', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.enterLocationAndSelectFirst('Leeds');
        await loc.selectDistance('Up to 15 miles');

        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);

        await page.goBack();

        await expect(loc.locationInput()).not.toHaveValue('');
        await expect(
            page
                .getByRole('group', { name: /How far are you able to travel/i })
                .getByRole('radio', { name: /^Up to 15 miles$/i })
        ).toBeChecked();
    });

    test('AC9: Over 30 miles selected -> proceeds (navigation-level check)', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.enterLocationAndSelectFirst('Leeds');
        await loc.selectDistance('Over 30 miles');

        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);
    });
});

