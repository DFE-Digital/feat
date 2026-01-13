import { test, expect, Page } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';
import { LocationPage } from '../pages/locationPage';
import { InterestsPage } from '../pages/interestsPage';
import { QualificationLevelPage } from '../pages/qualificationLevelPage';

async function goToQualificationLevel(page: Page) {
    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    await index.startNow().click();

    const loc = new LocationPage(page);
    await expect(page).toHaveURL(/\/location/i);

    // select location properly (accessibleAutocomplete)
    await loc.enterLocationAndSelectFirst('Leeds');

    // choose a distance that takes you to Interests (adjust if your journey differs)
    const tenMiles = loc.distanceRadio('Up to 10 miles');
    await tenMiles.check({ force: true });
    await expect(tenMiles).toBeChecked();

    await loc.continueButton().click();
    await expect(page).toHaveURL(/\/interests$/i);

    const interests = new InterestsPage(page);

    // optional path: leave blank and continue to qualification level
    await interests.continueButton().click();
    await expect(page).toHaveURL(/qualification[-]?level/i);

    const qual = new QualificationLevelPage(page);
    await expect(qual.heading()).toBeVisible();
}

test.describe('FEAT – Qualification level page', () => {
    test.beforeEach(async ({ page }) => {
        await goToQualificationLevel(page)
    });

    test('AC1: shows all qualification level options and guidance link', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        await expect(qual.heading()).toBeVisible();

        await expect(qual.skillsCheckbox()).toBeVisible();
        await expect(qual.level1or2Checkbox()).toBeVisible();
        await expect(qual.level3Checkbox()).toBeVisible();
        await expect(qual.level4to8Checkbox()).toBeVisible();

        await expect(qual.guidanceLink()).toBeVisible();
        await expect(qual.guidanceLink()).toHaveText(/what qualification levels mean/i);
    });

    test('AC1: options can be selected and deselected independently', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        // Select/deselect a single option
        await qual.level3Checkbox().check();
        await expect(qual.level3Checkbox()).toBeChecked();

        await qual.level3Checkbox().uncheck();
        await expect(qual.level3Checkbox()).not.toBeChecked();

        // Select multiple options at once
        await qual.level1or2Checkbox().check();
        await qual.level4to8Checkbox().check();

        await expect(qual.level1or2Checkbox()).toBeChecked();
        await expect(qual.level4to8Checkbox()).toBeChecked();
        await expect(qual.level3Checkbox()).not.toBeChecked();
    });

    test('AC2/AC7: shows error when no option is selected', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        await qual.continueButton().click();

        await expect(qual.errorSummaryHeading()).toBeVisible();
        await expect(qual.errorMessageLink()).toBeVisible();

        await expect(page).toHaveURL(/qualificationlevel/i);
    });

    test('AC3: Get skills and experience only -> About You page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        await qual.selectOptionsAndContinue(['skills']);

        await expect(page).toHaveURL(/age/i);
        
    });

    test('AC3: Level 1 or 2 only -> About You page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        await qual.selectOptionsAndContinue(['level1or2']);

        await expect(page).toHaveURL(/age/i);
    });

    test('AC3: Level 3 only -> Check your answers page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['level3']);
        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    });

    test('AC3: Level 4 to 8 only -> Check your answers page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['level4to8']);
        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    });

    test('AC3: Level 1 or 2 + Level 3 -> About You page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['level1or2', 'level3']);
        await expect(page).toHaveURL(/age/i);
    });

    test('AC3: Level 3 + Level 4 to 8 -> Check your answers page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['level3', 'level4to8']);
        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    });

    test('AC3: Get skills + any other option -> About You page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['skills', 'level3']);
        await expect(page).toHaveURL(/age/i);
    });

    test('AC3: all options selected -> About You page', async ({ page }) => {
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue([
            'skills',
            'level1or2',
            'level3',
            'level4to8',
        ]);
        await expect(page).toHaveURL(/age/i);
    });

    test('AC6: guidance link does not disappear selections (basic check)', async ({ page }) => {
        const qual = new QualificationLevelPage(page);

        await qual.level3Checkbox().check();
        await expect(qual.level3Checkbox()).toBeChecked();

        await expect(qual.guidanceLink()).toBeVisible();

        const href = await qual.guidanceLink().getAttribute('href');
        expect(href).toBeTruthy();

        // We’re not following the external link here – just asserting it exists
        await expect(qual.level3Checkbox()).toBeChecked();
    });
});
