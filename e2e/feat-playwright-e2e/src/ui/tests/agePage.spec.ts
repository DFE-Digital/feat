import { test, expect, Page } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';
import { LocationPage } from '../pages/locationPage';
import { InterestsPage } from '../pages/interestsPage';
import { QualificationLevelPage } from '../pages/qualificationLevelPage';
import { AgePage } from '../pages/agePage';

/**
 * Helper flow:
 * Index -> Location -> Interests -> Qualification level -> Age (only for allowed qualification combos)
 */
async function goToAgeWithQualificationSelections(
    page: Page,
    options: Array<'skills' | 'level1or2' | 'level3' | 'level4to8'>
) {
    const index = new IndexPage(page);

    // 1) Start the journey
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    await index.startNow().click();

    // 2) Location step
    const loc = new LocationPage(page);
    await expect(page).toHaveURL(/\/location/i);

    // Pick a location + distance that keeps Interests optional 
    await loc.enterLocationAndSelectFirst('Leeds');
    await loc.distanceRadio('Up to 10 miles').check();
    await expect(loc.distanceRadio('Up to 10 miles')).toBeChecked();

    await loc.continueButton().click();
    await expect(page).toHaveURL(/\/interests$/i);

    // 3) Interests step (optional path -> can continue blank)
    const interests = new InterestsPage(page);
    await expect(interests.heading()).toBeVisible();
    await interests.continueButton().click();

    // 4) Qualification level step
    const qual = new QualificationLevelPage(page);
    await expect(page).toHaveURL(/qualification[-]?level/i);
    await expect(qual.heading()).toBeVisible();

    // Select requested qualification options
    await qual.selectOptionsAndContinue(options);
}

test.describe('FEAT – 5.0 Age', () => {
    test('AC1 + Layout: page loads, has copy, link, radios in order, and Continue button', async ({ page }) => {
        await goToAgeWithQualificationSelections(page, ['skills']); // allowed path -> Age page

        const age = new AgePage(page);
        await expect(page).toHaveURL(/\/age$/i);

        // Frame
        await expect(age.backLink()).toBeVisible();
        await expect(age.heading()).toBeVisible();

        // Intro copy
        await expect(age.introPara1()).toBeVisible();
        await expect(age.introPara2()).toBeVisible();
        await expect(age.introPara3()).toBeVisible();

        // Guidance link
        await expect(age.trainingOptionsLink()).toBeVisible();
        await expect(age.trainingOptionsLink()).toHaveAttribute('href', '/trainingoptionspages/trainingoptions');

        // Question block
        await expect(age.questionLegend()).toBeVisible();

        // Options in the required order 
        const options = ['16 or 17', '18', '19', '20 - 24', '25 or older'] as const;
        for (const opt of options) {
            await expect(age.ageOption(opt)).toBeVisible();
        }

        // Button styling basic check
        await expect(age.continueButton()).toBeVisible();
        await expect(age.continueButton()).toHaveClass(/govuk-button/);
    });

    test('AC1/AC7: optional question – continue without selection goes to Check your answers (no error)', async ({ page }) => {
        await goToAgeWithQualificationSelections(page, ['skills']);

        const age = new AgePage(page);
        await expect(page).toHaveURL(/\/age$/i);

        // No selection, just continue
        await age.continueButton().click();
        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    });

    test('AC2: single selection – selecting one radio unselects others', async ({ page }) => {
        await goToAgeWithQualificationSelections(page, ['skills']);

        const age = new AgePage(page);
        await expect(page).toHaveURL(/\/age$/i);

        await age.ageOption('18').check();
        await expect(age.ageOption('18')).toBeChecked();

        // Pick a different option -> first should uncheck
        await age.ageOption('20 - 24').check();
        await expect(age.ageOption('20 - 24')).toBeChecked();
        await expect(age.ageOption('18')).not.toBeChecked();
    });

    test('AC3 + AC4: selecting an age saves to session and persists when returning (Back)', async ({ page }) => {
        await goToAgeWithQualificationSelections(page, ['skills']);

        const age = new AgePage(page);
        await expect(page).toHaveURL(/\/age$/i);

        await age.selectAge('25 or older');
        await age.continueButton().click();

        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);

        // Return (Back) and verify selection is restored
        await page.goBack();
        await expect(page).toHaveURL(/\/age$/i);
        await expect(age.ageOption('25 or older')).toBeChecked();
    });

    test('AC6: training options link opens in same tab and does not lose the current selection', async ({ page }) => {
        await goToAgeWithQualificationSelections(page, ['skills']);

        const age = new AgePage(page);
        await expect(page).toHaveURL(/\/age$/i);

        // Make a selection first
        await age.selectAge('19');

        // Click the guidance link (same tab)
        await expect(age.trainingOptionsLink()).toHaveAttribute('href', '/trainingoptionspages/trainingoptions');
        await age.trainingOptionsLink().click();
        await expect(page).toHaveURL(/\/trainingoptionspages\/trainingoptions/i);

        // Go back and confirm selection is still there
        await page.goBack();
        await expect(page).toHaveURL(/\/age$/i);
        await expect(age.ageOption('19')).toBeChecked();
    });
});
