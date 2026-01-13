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
        await loc.locationInput().fill('@@@'); // invalid
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(/Select how far you would be able to travel/i);
        await expect(loc.fieldError('Select how far you would be able to travel')).toBeVisible();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC3: autocomplete – no dropdown until 3 alpha chars; appears at 3+', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Lo');
        await expect(loc.suggestionsMenu()).toBeHidden();

        await loc.locationInput().type('n'); // now 3 chars 'Lon'
        await expect(loc.suggestionsMenu()).not.toHaveClass(/autocomplete__menu--hidden/i, {
            timeout: 10_000,
        });
    });

    test('AC3: autocomplete supports keyboard selection', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.locationInput().fill('Lon');
        await expect(loc.suggestionsMenu()).not.toHaveClass(/autocomplete__menu--hidden/i, {
            timeout: 10_000,
        });
        
        // wait for options
        await expect(loc.suggestionOptions().first()).toBeVisible({ timeout: 10_000 });

        await page.keyboard.press('ArrowDown');
        await page.keyboard.press('Enter');

        // listbox hidden again after selection
        await expect(loc.suggestionsMenu()).toHaveClass(/autocomplete__menu--hidden/i, {
            timeout: 10_000,
        });        
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

        await loc.enterLocationAndSelectFirst('M13 9PL'); // valid format postcode
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(/Select how far you would be able to travel/i);
        await expect(loc.fieldError('Select how far you would be able to travel')).toBeVisible();
    });

    test('AC4: if distance selected but no location, show location required error', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.distanceRadio('Up to 10 miles').check();
        await loc.continueButton().click();

        await expect(loc.errorSummary()).toBeVisible();
        await expect(loc.errorSummary()).toContainText(
            /Enter a town, city or postcode to use the distance filter/i
        );

        await expect(
            loc.fieldError('Enter a town, city or postcode to use the distance filter')
        ).toBeVisible();

        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC6/AC8: valid location + distance proceeds to next step', async ({ page }) => {
        const loc = new LocationPage(page);
        
        // Assert we now have a "real" value (full postcode format)
        const selected = await loc.enterLocationAndSelectFirst('MK4');
        expect(selected).toMatch(/^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$/i);

        await loc.distanceRadio('Up to 5 miles').check();
        await loc.continueButton().click();

        await expect(page).toHaveURL(/\/interests$/i);
    });

    test('AC7: returning to page restores location and distance', async ({ page }) => {
        const loc = new LocationPage(page);
        
        // Capture the exact selected value (this is what must persist)
        const selected = await loc.enterLocationAndSelectFirst('MK4');
        await loc.distanceRadio('Up to 15 miles').check();
        await loc.continueButton().click();

        await expect(page).toHaveURL(/\/interests$/i);

        await page.goBack();

        // Assert the exact same selected location is restored
        await expect(loc.locationInput()).toHaveValue(selected);
        await expect(loc.distanceRadio('Up to 15 miles')).toBeChecked();
    });


    test('AC9:  Over 30 miles selected -> proceeds (navigation-level check)', async ({ page }) => {
        const loc = new LocationPage(page);

        await loc.enterLocationAndSelectFirst('MK4');
        await loc.distanceRadio('Over 30 miles').check();
        await loc.continueButton().click();
        await expect(page).toHaveURL(/\/interests$/i);
    });
});
