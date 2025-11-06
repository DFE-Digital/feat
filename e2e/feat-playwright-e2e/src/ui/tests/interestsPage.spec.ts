import { test, expect, Page } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';
import { LocationPage } from '../pages/locationPage';
import { InterestsPage } from '../pages/interestsPage';

// Helper flows to reach Interests in the two states
async function goToInterestsOptional(page: Page) {
    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'networkidle' });
    await index.startNow().click();

    const loc = new LocationPage(page);
    await loc.locationInput().fill('Leeds');
    await loc.distanceRadio('Up to 10 miles').check(); // ≤ 30 miles => optional
    await loc.continueButton().click();

    await expect(page).toHaveURL(/\/interests$/i);
}

async function goToInterestsMandatory(page: Page) {
    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'networkidle' });
    await index.startNow().click();

    const loc = new LocationPage(page);
    // No location or > 30 miles => mandatory
    await loc.distanceRadio('Over 30 miles').check();
    await loc.continueButton().click();

    await expect(page).toHaveURL(/\/interests$/i);
}

test.describe('FEAT – 3.0 Interests', () => {
    test('AC1/AC2: heading, guidance, examples, info line are visible', async ({ page }) => {
        await goToInterestsOptional(page);

        const interests = new InterestsPage(page);
        await expect(interests.heading()).toBeVisible();
        await expect(interests.guidancePara()).toBeVisible();
        await expect(interests.examplesList()).toBeVisible();
        await expect(interests.examplesList()).toContainText(/maths/i);
        await expect(interests.examplesList()).toContainText(/bricklaying/i);
        await expect(interests.examplesList()).toContainText(/working with animals/i);
        await expect(interests.infoLine()).toBeVisible();
    });

    test('AC3/AC4: three inputs accept free text up to 100 chars', async ({ page }) => {
        await goToInterestsOptional(page);

        const interests = new InterestsPage(page);
        const long = 'x'.repeat(120);

        await interests.interestInput(1).fill('maths');
        await interests.interestInput(2).fill('computer science');
        await interests.interestInput(3).fill(long);

        const v1 = await interests.interestInput(1).inputValue();
        const v2 = await interests.interestInput(2).inputValue();
        const v3 = await interests.interestInput(3).inputValue();

        expect(v1.length).toBeLessThanOrEqual(100);
        expect(v2.length).toBeLessThanOrEqual(100);
        expect(v3.length).toBeLessThanOrEqual(100);
    });

    test('AC6/AC8 (optional): can continue with all fields blank', async ({ page }) => {
        await goToInterestsOptional(page);

        const interests = new InterestsPage(page);
        // Optional state should show "(optional)" on Interest 1
        await expect(interests.interestLabel(1)).toContainText('(optional)');

        await interests.continueButton().click();
        await expect(page).toHaveURL(/\/qualificationlevel$/i);
    });

    test('AC5/AC8 (mandatory): blank submit shows errors and stays', async ({ page }) => {
        await goToInterestsMandatory(page);

        const interests = new InterestsPage(page);
        await expect(interests.interestLabel(1)).not.toContainText('(optional)');

        await interests.continueButton().click();

        await expect(interests.errorSummary()).toBeVisible();
        await expect(interests.errorSummary()).toContainText(/Please enter an interest/i);
        await expect(interests.interest1InlineError()).toBeVisible();
        await expect(page).toHaveURL(/\/interests$/i);

        // Enter a value then proceed
        await interests.interestInput(1).fill('maths');
        await interests.continueButton().click();
        await expect(page).toHaveURL(/(level|what-qualification-level|check-your-answers)/i);
    });

    test('AC7: entered interests persist when returning', async ({ page }) => {
        await goToInterestsOptional(page);

        const interests = new InterestsPage(page);
        await interests.interestInput(1).fill('maths');
        await interests.interestInput(2).fill('design');
        await interests.continueButton().click();

        await page.goBack();
        await expect(interests.interestInput(1)).toHaveValue(/maths/i);
        await expect(interests.interestInput(2)).toHaveValue(/design/i);
    });

    test('AC6: valid entries proceed to next step and store', async ({ page }) => {
        await goToInterestsMandatory(page);

        const interests = new InterestsPage(page);
        await interests.interestInput(1).fill('bricklaying');
        await interests.continueButton().click();

        await expect(page).toHaveURL(/(level|what-qualification-level|check-your-answers)/i);
    });
});
