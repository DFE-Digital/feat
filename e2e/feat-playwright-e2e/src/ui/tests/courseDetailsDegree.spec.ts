import { test, expect } from '@playwright/test';
import { CourseDetailsPage } from '../pages/courseDetailsPage';
import { getCourseId } from '../helpers/courseResolver';

test.describe('FEAT – Course details (Degree)', () => {
    test('Degree: renders sections + key fields + links', async ({ page }) => {
        const courseId = getCourseId('degree');
        await page.goto(`/courses/${courseId}`, { waitUntil: 'domcontentloaded' });

        const details = new CourseDetailsPage(page);
        await details.expectOnPage();

        console.log('Degree URL:', page.url());
        console.log('Degree title:', await page.title());
        await expect(details.cardValueForKey('About the course', 'Qualification type')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Level')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Entry requirements')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Tuition fee')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('About the course', 'Awarding organisation')).toBeVisible({ timeout: 10_000 });

        await expect(details.cardValueForKey('Location', 'University')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Location', 'Campus address')).toBeVisible({ timeout: 10_000 });

        await expect(details.cardValueForKey('Course details', 'Delivery method')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Duration')).toBeVisible({ timeout: 10_000 });
        await expect(details.cardValueForKey('Course details', 'Hours')).toBeVisible({ timeout: 10_000 });

        await expect(details.goToButton()).toHaveText(/go to course/i);

        await expect(
            details.relatedLink('Explore jobs and careers you could do after this course')
        ).toBeVisible({ timeout: 10_000 });

        await expect(
            details.relatedLink('More information about Degrees')
        ).toBeVisible({ timeout: 10_000 });
    });
});
