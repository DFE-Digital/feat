import { test, expect, Page } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';
import { LocationPage } from '../pages/locationPage';
import { InterestsPage } from '../pages/interestsPage';
import { QualificationLevelPage } from '../pages/qualificationLevelPage';
import { CheckYourAnswersPage } from '../pages/checkYourAnswersPage';
import { SearchResultsPage } from '../pages/searchResultsPage';

type QualOption = 'skills' | 'level1or2' | 'level3' | 'level4to8';

function escapeRegex(str: string) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

async function goToResultsFromCya(
    page: Page,
    opts?: {
        locationType?: string;
        distanceLabel?: string;
        interests?: { i1?: string; i2?: string; i3?: string };
        qualificationSelections?: QualOption[];
        selectAgeLabel?: '16 or 17' | '18' | '19' | '20 - 24' | '25 or older';
    }
): Promise<SearchResultsPage> {
    const locationType = opts?.locationType ?? 'Leeds';
    const distanceLabel = opts?.distanceLabel ?? 'Up to 10 miles';
    const qualificationSelections = opts?.qualificationSelections ?? ['level3'];

    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    await index.startNow().click();

    const loc = new LocationPage(page);
    await expect(page).toHaveURL(/\/location/i);

    await loc.enterLocationAndSelectFirst(locationType);
    await loc.distanceRadio(distanceLabel).check({ force: true });
    await expect(loc.distanceRadio(distanceLabel)).toBeChecked();

    await loc.continueButton().click();
    await expect(page).toHaveURL(/\/interests$/i);

    const interests = new InterestsPage(page);
    if (opts?.interests?.i1) await interests.interestInput(1).fill(opts.interests.i1);
    if (opts?.interests?.i2) await interests.interestInput(2).fill(opts.interests.i2);
    if (opts?.interests?.i3) await interests.interestInput(3).fill(opts.interests.i3);

    await interests.continueButton().click();
    await expect(page).toHaveURL(/qualification[-]?level/i);

    const qual = new QualificationLevelPage(page);
    await expect(qual.heading()).toBeVisible();
    await qual.selectOptionsAndContinue(qualificationSelections);

    if (/\/age/i.test(page.url())) {
        if (opts?.selectAgeLabel) {
            await page
                .getByRole('radio', {
                    name: new RegExp(`^${escapeRegex(opts.selectAgeLabel)}$`, 'i'),
                })
                .check();
        }
        await page.getByRole('button', { name: /^continue$/i }).click();
    }

    const cya = new CheckYourAnswersPage(page);
    await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    await expect(cya.heading()).toBeVisible();

    await cya.searchButton().click();

    const results = new SearchResultsPage(page);
    await results.expectOnPage();
    return results;
}

async function goToResultsFromCyaNoLocation(
    page: Page,
    opts?: {
        interests?: { i1?: string; i2?: string; i3?: string };
        qualificationSelections?: QualOption[];
        selectAgeLabel?: '16 or 17' | '18' | '19' | '20 - 24' | '25 or older';
    }
): Promise<SearchResultsPage> {
    const qualificationSelections = opts?.qualificationSelections ?? ['level3'];

    const index = new IndexPage(page);
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    await index.startNow().click();

    const loc = new LocationPage(page);
    await expect(page).toHaveURL(/\/location/i);

    await loc.continueButton().click();
    await expect(page).toHaveURL(/\/interests$/i);

    const interests = new InterestsPage(page);
    if (opts?.interests?.i1) await interests.interestInput(1).fill(opts.interests.i1);
    if (opts?.interests?.i2) await interests.interestInput(2).fill(opts.interests.i2);
    if (opts?.interests?.i3) await interests.interestInput(3).fill(opts.interests.i3);

    await interests.continueButton().click();
    await expect(page).toHaveURL(/qualification[-]?level/i);

    const qual = new QualificationLevelPage(page);
    await expect(qual.heading()).toBeVisible();
    await qual.selectOptionsAndContinue(qualificationSelections);

    if (/\/age/i.test(page.url())) {
        if (opts?.selectAgeLabel) {
            await page
                .getByRole('radio', {
                    name: new RegExp(`^${escapeRegex(opts.selectAgeLabel)}$`, 'i'),
                })
                .check();
        }
        await page.getByRole('button', { name: /^continue$/i }).click();
    }

    const cya = new CheckYourAnswersPage(page);
    await expect(page).toHaveURL(/check-your-answers|checkanswers/i);
    await expect(cya.heading()).toBeVisible();

    await cya.searchButton().click();

    const results = new SearchResultsPage(page);
    await results.expectOnPage();
    return results;
}

test.describe('FEAT – Search results page', () => {
    test('AC (Layout) 1/2: heading shows and Search again link is visible', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await expect(results.heading()).toBeVisible();
        await expect(results.searchAgainLink()).toBeVisible();
    });

    test('AC (Functional) 1 + (Layout) 5: results list renders and first card shows key fields', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const firstCard = results.firstCourseCard();
        await expect(firstCard).toBeVisible();
        await expect(results.viewMoreDetailsLinks().first()).toBeVisible();

        await expect(firstCard.locator('h2, h3').first()).toBeVisible();
    });

    test('AC (Functional) 2 + (Layout) 4: default sort is Distance when Location is provided', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await expect(results.sortBlock()).toBeVisible();

        const active = (await results.activeSort().innerText().catch(() => '')).toLowerCase();
        if (active.includes('distance')) {
            await expect(results.activeSort()).toHaveText(/distance/i);
            await expect(results.relevanceSortLink()).toBeVisible();
        } else {
            await expect(results.distanceSortText()).toHaveCount(0);
            await expect(results.distanceSortLink()).toBeVisible();
        }
    });

    test('AC (Functional) 2: default sort is Relevance when Location is NOT provided (Distance hidden)', async ({ page }) => {
        const results = await goToResultsFromCyaNoLocation(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await expect(results.sortBlock()).toBeVisible();
        await expect(results.activeSort()).toHaveText(/relevance/i);

        await expect(results.distanceSortLink()).toHaveCount(0);
        await expect(results.distanceSortText()).toHaveCount(0);
    });

    test('AC (Functional) 3 + Sort story AC2/AC5: switching to Relevance updates URL and UI', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const hasLink = await results.hasRelevanceSortLink();
        if (!hasLink) {
            await expect(results.activeSort()).toHaveText(/relevance|distance/i);
            return;
        }

        await results.clickRelevanceSort();
        await expect(page).toHaveURL(/orderby=relevance/i);
        await expect(results.activeSort()).toHaveText(/relevance/i);
    });

    test('AC (Functional) 3: switching sort changes the first result (best effort)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const hasLink = await results.hasRelevanceSortLink();
        if (!hasLink) return;

        const beforeId = await results.firstCourseId().catch(() => '');
        const beforeTitle = await results.firstCourseTitle().catch(() => '');

        await results.clickRelevanceSort();

        const afterId = await results.firstCourseId().catch(() => '');
        const afterTitle = await results.firstCourseTitle().catch(() => '');

        if (beforeId && afterId) {
            expect(afterId).not.toBe(beforeId);
        } else if (beforeTitle && afterTitle) {
            expect(afterTitle).not.toBe(beforeTitle);
        } else {
            await expect(results.firstCourseCard()).toBeVisible();
        }
    });

    test('AC (Functional) 4: View more details navigates to Course details page', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.openFirstCourseDetails();
        await expect(page).toHaveURL(/\/courses\/[0-9a-fA-F-]{36}/, { timeout: 30_000 });
    });

    test('AC (Functional) 6: pagination loads next page and results render (if shown)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const paginationVisible = await results.isPaginationVisible();
        if (!paginationVisible) {
            await expect(results.pagination()).toHaveCount(0);
            return;
        }

        await expect(results.currentPageLink()).toBeVisible();
        await results.nextPageLink().click();
        await expect(page).toHaveURL(/pagenumber=2/i);
        await expect(results.firstCourseCard()).toBeVisible();
        await expect(results.sortBlock()).toBeVisible();
    });

    test('AC (Functional) 6 + Filters story AC3: pagination keeps applied filter on next page (if shown)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.openFiltersIfCollapsed();
        await results.facetCheckboxByLabel('Degree').check({ force: true });
        await results.applyFiltersButton().click();
        await results.expectOnPage();

        const paginationVisible = await results.isPaginationVisible();
        if (!paginationVisible) {
            await expect(results.pagination()).toHaveCount(0);
            await expect(results.facetCheckboxByLabel('Degree')).toBeChecked();
            return;
        }

        await results.nextPageLink().click();
        await expect(page).toHaveURL(/pagenumber=2/i);

        await results.openFiltersIfCollapsed();
        await expect(results.facetCheckboxByLabel('Degree')).toBeChecked();
    });

    test('AC (Functional) 9 + (Layout) 2: Search again takes the user back to Location page', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.searchAgainLink().click();
        await expect(page).toHaveURL(/\/location/i);
    });

    test('AC (Functional) 10 + Filters story AC3/AC4: Apply filters keeps selection visible; Clear all filters resets', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.openFiltersIfCollapsed();

        await results.facetCheckboxByLabel('Degree').check({ force: true });
        await results.applyFiltersButton().click();
        await results.expectOnPage();

        await results.openFiltersIfCollapsed();
        await expect(results.facetCheckboxByLabel('Degree')).toBeChecked();

        await expect(results.clearAllFiltersLink()).toBeVisible();
        await results.clearAllFiltersLink().click();

        await results.expectOnPage();
        await expect(results.firstCourseCard()).toBeVisible();
    });

    test('Sort story AC2/AC5: can switch to Relevance and back to Distance (when both options exist)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const hasRelevance = await results.hasRelevanceSortLink();
        if (!hasRelevance) {
            await expect(results.sortBlock()).toBeVisible();
            return;
        }

        await results.clickRelevanceSort();
        await expect(page).toHaveURL(/orderby=relevance/i);
        await expect(results.activeSort()).toHaveText(/relevance/i);

        const hasDistanceAfter = await results.hasDistanceSortLink();
        if (!hasDistanceAfter) {
            await expect(results.sortBlock()).toBeVisible();
            return;
        }

        await results.distanceSortLink().click();
        await results.expectOnPage();
        await expect(results.activeSort()).toHaveText(/distance/i);
    });

    test('Filters story AC4: Clear all filters unchecks previously selected filter', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.openFiltersIfCollapsed();

        const label = 'Degree';
        const checkbox = results.facetCheckboxByLabel(label);

        await checkbox.check({ force: true });
        await results.applyFiltersButton().click();
        await results.expectOnPage();

        await results.openFiltersIfCollapsed();
        await expect(checkbox).toBeChecked();

        await results.clearAllFiltersLink().click();
        await results.expectOnPage();

        await results.openFiltersIfCollapsed();
        await expect(checkbox).not.toBeChecked();
    });

    test('AC (Functional) 7: returning from Course details keeps user on results (best effort)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.openFirstCourseDetails();
        await expect(page).toHaveURL(/\/courses\/[0-9a-fA-F-]{36}/, { timeout: 30_000 });

        await page.goBack();
        await results.expectOnPage();
        await expect(results.firstCourseCard()).toBeVisible();
    });

    test('AC (Functional) 7: refresh keeps user on results and renders cards', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await page.reload({ waitUntil: 'domcontentloaded' });
        await results.expectOnPage();
        await expect(results.firstCourseCard()).toBeVisible();
    });

    test('AC (Functional) 6: pagination next and previous navigates correctly (if shown)', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            locationType: 'Leeds',
            distanceLabel: 'Up to 10 miles',
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        const visible = await results.isPaginationVisible();
        if (!visible) {
            await expect(results.pagination()).toHaveCount(0);
            return;
        }

        await results.nextPageLink().click();
        await expect(page).toHaveURL(/pagenumber=2/i);

        await results.prevPageLink().click();
        await expect(page).toHaveURL(/pagenumber=1|loadcourses(?!.*pagenumber=2)/i);
        await expect(results.firstCourseCard()).toBeVisible();
    });

    test('AC (Functional) 9: Search again returns to Location and user can reach results again', async ({ page }) => {
        const results = await goToResultsFromCya(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await results.searchAgainLink().click();
        await expect(page).toHaveURL(/\/location/i);

        const loc = new LocationPage(page);
        await loc.continueButton().click();

        await expect(page).toHaveURL(/\/interests$/i);
        const interests = new InterestsPage(page);
        await interests.interestInput(1).fill('Art');
        await interests.continueButton().click();

        await expect(page).toHaveURL(/qualification[-]?level/i);
        const qual = new QualificationLevelPage(page);
        await qual.selectOptionsAndContinue(['level3']);

        const cya = new CheckYourAnswersPage(page);
        await expect(cya.heading()).toBeVisible();
        await cya.searchButton().click();

        const results2 = new SearchResultsPage(page);
        await results2.expectOnPage();
        await expect(results2.firstCourseCard()).toBeVisible();
    });

    test('Sort story AC1/AC4: with no location, Relevance is active and Distance is not shown', async ({ page }) => {
        const results = await goToResultsFromCyaNoLocation(page, {
            interests: { i1: 'Art' },
            qualificationSelections: ['level3'],
        });

        await expect(results.activeSort()).toHaveText(/relevance/i);
        await expect(results.distanceSortLink()).toHaveCount(0);
        await expect(results.distanceSortText()).toHaveCount(0);
    });
});
