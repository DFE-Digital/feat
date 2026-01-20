import { test, expect } from '@playwright/test';
import { CourseDetailsPage } from '../pages/courseDetailsPage';
import { getCourseId } from '../helpers/courseResolver';

test.describe('FEAT – Course details (NCS)', () => {
    test('NCS: renders sections + key fields + links', async ({ page }) => {
        const courseId = getCourseId('ncs');
        await page.goto(`/courses/${courseId}`, { waitUntil: 'domcontentloaded' });

        const details = new CourseDetailsPage(page);
        await details.expectOnPage();
        
        await expect(details.cardValueForKey('About the course', 'Level')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Entry requirements')).toBeVisible({ timeout: 10_000 });

        await details.expectAnyKeyExists('About the course', ['Cost', 'Course overview']);

        await expect(details.cardValueForKey('Location', 'Provider')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Location', 'Provider location')).toBeVisible({ timeout: 10_000 });

        await expect(details.cardValueForKey('Course details', 'Delivery method')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Duration')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Hours')).toBeVisible({ timeout: 10_000 });

        await expect(details.goToButton()).toHaveText(/go to course/i);

        await expect(
            details.relatedLink('Explore jobs and careers you could do after this course')
        ).toBeVisible({ timeout: 10_000 });

        await expect(
            details.relatedLink('More information about training options')
        ).toBeVisible({ timeout: 10_000 });
    });
});
