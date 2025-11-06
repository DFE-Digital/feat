import { test, expect } from '@playwright/test';
import { LocationPage } from '../pages/locationPage';
import { IndexPage } from '../pages/indexPage';

test.describe('FEAT – 2.0 Location', () => {
    test.beforeEach(async ({ page, baseURL }) => {
        const index = new IndexPage(page);
        await page.goto(baseURL || '/', { waitUntil: 'networkidle' });
        await index.startNow().click();
        await expect(page).toHaveURL(/\/location/i);
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

        await page.goto('/location', { waitUntil: 'networkidle' });
        await loc.locationInput().fill('@@@'); // clearly invalid
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(/Please enter a valid location/i);
        await expect(loc.fieldError('Please enter a valid location.')).toBeVisible();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC3: autocomplete – no dropdown until 3 alpha chars; appears at 3+', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Lo');
        await expect(loc.suggestionsMenu()).toBeHidden();

        await loc.locationInput().type('n'); // now 3 chars
        await expect(loc.suggestionsMenu()).toBeVisible();

        // if backend returns nothing, expect “No locations found”
        // await expect(page.getByText(/No locations found/i)).toBeVisible();
    });

    test('AC3: autocomplete supports keyboard selection', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Lon');
        await expect(loc.suggestionsMenu()).toBeVisible();

        await page.keyboard.press('ArrowDown');
        await page.keyboard.press('Enter');

        await expect(loc.suggestionsMenu()).toBeHidden();
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

        await loc.distanceRadio('Up to 10 miles').check();
        await expect(loc.distanceRadio('Up to 10 miles')).toBeChecked();
    });

    test('AC4: if location entered but no distance, show distance error', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('M13 9PL'); // valid format postcode
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(/Please select how far you would be happy to travel/i);
        await expect(loc.fieldError('Please select how far you would be happy to travel.')).toBeVisible();
    });

    test('AC6/AC8: valid location + distance proceeds to next step', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('M13 9PL');
        await loc.distanceRadio('Up to 5 miles').check();
        await loc.continueButton().click();

        await expect(page).toHaveURL(/\/interests$/i);
    });

    test('AC7: returning to page restores location and distance', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Leeds');
        await loc.distanceRadio('Up to 15 miles').check();
        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);

        await page.goBack();
        await expect(loc.locationInput()).toHaveValue(/Leeds/i);
        await expect(loc.distanceRadio('Up to 15 miles')).toBeChecked();
    });

    test('AC9: conditional subject requirement flagging (navigation-level check)', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Over 30 test');
        await loc.distanceRadio('Over 30 miles').check();
        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);

    });
});
