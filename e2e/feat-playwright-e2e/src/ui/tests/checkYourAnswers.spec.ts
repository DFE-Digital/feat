import { test, expect, Page } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';
import { LocationPage } from '../pages/locationPage';
import { InterestsPage } from '../pages/interestsPage';
import { QualificationLevelPage } from '../pages/qualificationLevelPage';
import { CheckYourAnswersPage } from '../pages/checkYourAnswersPage';

type QualOption = 'skills' | 'level1or2' | 'level3' | 'level4to8';

type JourneyResult = {
    selectedLocation: string;
    distanceLabel: string;
    interestsEntered: string[];
    qualificationSelected: QualOption[];
    ageWasShown: boolean;
};

function escapeRegex(str: string) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

//helper class
async function goToCheckYourAnswers(
    page: Page,
    opts?: {
        locationType?: string; // what we type into the autocomplete
        distanceLabel?: string;
        interests?: { i1?: string; i2?: string; i3?: string };
        qualificationSelections?: QualOption[];
        selectAgeLabel?: '16 or 17' | '18' | '19' | '20 - 24' | '25 or older';
    }
): Promise<JourneyResult> {
    const locationType = opts?.locationType ?? 'Leeds'; // what we type (may select something else)
    const distanceLabel = opts?.distanceLabel ?? 'Up to 10 miles';
    const qualificationSelections = opts?.qualificationSelections ?? ['level3'];

    // Start
    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    await index.startNow().click();

    // Location
    const loc = new LocationPage(page);
    await expect(page).toHaveURL(/\/location/i);

    // IMPORTANT: capture what the autocomplete actually selected
    const selectedLocation = await loc.enterLocationAndSelectFirst(locationType);

    const distance = loc.distanceRadio(distanceLabel);
    await distance.check({ force: true });
    await expect(distance).toBeChecked();

    await loc.continueButton().click();
    await expect(page).toHaveURL(/\/interests$/i);

    // Interests
    const interests = new InterestsPage(page);
    const interestsEntered: string[] = [];

    if (opts?.interests?.i1) {
        await interests.interestInput(1).fill(opts.interests.i1);
        interestsEntered.push(opts.interests.i1);
    }
    if (opts?.interests?.i2) {
        await interests.interestInput(2).fill(opts.interests.i2);
        interestsEntered.push(opts.interests.i2);
    }
    if (opts?.interests?.i3) {
        await interests.interestInput(3).fill(opts.interests.i3);
        interestsEntered.push(opts.interests.i3);
    }

    await interests.continueButton().click();
    await expect(page).toHaveURL(/qualification[-]?level/i);

    // Qualification
    const qual = new QualificationLevelPage(page);
    await expect(qual.heading()).toBeVisible();
    await qual.selectOptionsAndContinue(qualificationSelections);

    // Age may or may not appear
    let ageWasShown = false;

    if (/\/age/i.test(page.url())) {
        ageWasShown = true;

        if (opts?.selectAgeLabel) {
            await page
                .getByRole('radio', {
                    name: new RegExp(`^${escapeRegex(opts.selectAgeLabel)}$`, 'i'),
                })
                .check();
        }

        await page.getByRole('button', { name: /^continue$/i }).click();
    }

    // Check your answers
    await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    const cya = new CheckYourAnswersPage(page);
    await expect(cya.heading()).toBeVisible();

    return {
        selectedLocation,
        distanceLabel,
        interestsEntered,
        qualificationSelected: qualificationSelections,
        ageWasShown,
    };
}

test.describe('FEAT – Check your answers page', () => {
    test('AC1: shows the answers entered (including Age when Age step is shown)', async ({ page }) => {
        // Go through a path that DOES show the Age page
        const result = await goToCheckYourAnswers(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level1or2'], // this triggers Age
            selectAgeLabel: '18',
        });

        const cya = new CheckYourAnswersPage(page);

        // Page heading
        await expect(cya.heading()).toBeVisible();

        // Core answers
        await expect(cya.summaryRowByLabel('Location')).toBeVisible();
        await expect(cya.summaryRowByLabel('Distance')).toBeVisible();
        await expect(cya.summaryRowByLabel('Qualification level')).toBeVisible();

        // Age row should exist on this path
        expect(result.ageWasShown).toBe(true);
        await expect(cya.summaryRowByLabel('Age')).toBeVisible();

        await expect(cya.summaryValue('Age')).toHaveText(/^\s*18\s*$/i);

        // Optional: Interests (only if entered)
        await expect(cya.summaryRowByLabel('Interests')).toBeVisible();
    });


    test('AC3: Age row is NOT shown when Age step is skipped by journey rules', async ({ page }) => {
        const result = await goToCheckYourAnswers(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'], // should skip Age
        });

        const cya = new CheckYourAnswersPage(page);

        // On this path Age step wasn’t shown, so Age row should not exist
        expect(result.ageWasShown).toBe(false);
        await expect(cya.summaryRowByLabel('Age')).toHaveCount(0);
    });

    test('AC2/AC6/AC7: Change link (Location) goes back and returns to CYA after Continue', async ({ page }) => {
        const result = await goToCheckYourAnswers(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const cya = new CheckYourAnswersPage(page);

        // Click Change for Location
        await cya.changeLinkFor('Location').click();
        await expect(page).toHaveURL(/\/location/i);

        await page.getByRole('button', { name: /^continue$/i }).click();

        await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
        await expect(cya.summaryValue('Location')).toContainText(result.selectedLocation);
    });
});
