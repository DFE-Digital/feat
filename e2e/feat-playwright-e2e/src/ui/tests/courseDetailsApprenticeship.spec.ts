import { test, expect } from '@playwright/test';
import { CourseDetailsPage } from '../pages/courseDetailsPage';
import { getCourseId } from '../helpers/courseResolver';

test.describe('FEAT – Course details (Apprenticeship)', () => {
    test('Apprenticeship: renders sections + key fields + links', async ({ page }) => {
        const courseId = getCourseId('apprenticeship');
        await page.goto(`/courses/${courseId}`, { waitUntil: 'domcontentloaded' });

        const details = new CourseDetailsPage(page);
        await details.expectOnPage();

        await expect(details.cardValueForKey('About the course', 'Qualification type')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Level')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Wage')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Positions available')).toBeVisible({ timeout: 10_000 });

        await expect(details.cardValueForKey('Location', 'Employer')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Location', 'Employer address')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Location', 'Training provider')).toBeVisible({ timeout: 10_000 });

        await expect(details.cardValueForKey('Course details', 'Delivery method')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Start date')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Duration')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Hours')).toBeVisible({ timeout: 10_000 });

        await expect(details.goToButton()).toHaveText(/go to apprenticeship opportunity/i);

        await expect(
            details.relatedLink('Explore jobs and careers you could do after this course')
        ).toBeVisible({ timeout: 10_000 });

        await expect(
            details.relatedLink('More information about Apprenticeships')
        ).toBeVisible({ timeout: 10_000 });
    });
});
