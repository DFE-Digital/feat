import { test, expect } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';

test.describe('FEAT – 1.0 Index (Direct search entry)', () => {
    test.beforeEach(async ({ page }) => {
        const index = new IndexPage(page);
        await index.goto();
    });

    test('AC1: phase banner text and feedback link are visible', async ({ page }) => {
        const index = new IndexPage(page);

        await expect(index.phaseBanner()).toBeVisible();
        await expect(index.phaseBanner()).toContainText('This is a new service. Help us improve it and');
        await expect(index.feedbackLink()).toBeVisible();
        await expect(index.feedbackLink()).toContainText(/give your feedback/i);
        // We don't assert target/new tab here since the snippet shows href="#" (placeholder)
    });

    test('AC2/AC3: heading and intro paragraph are visible', async ({ page }) => {
        const index = new IndexPage(page);

        await expect(index.mainHeading()).toBeVisible();
        await expect(index.mainHeading()).toHaveText('Find education and training opportunities');

        await expect(index.introParagraph()).toBeVisible();
        await expect(index.introParagraph()).toContainText(
            /Whether you're already in education or training, or coming back after a break/i
        );
    });

    test('Layout: service name link is visible in header', async ({ page }) => {
        const index = new IndexPage(page);
        await expect(index.serviceNameLink()).toBeVisible();
        await expect(index.serviceNameLink()).toHaveText(/Find education and training/i);
    });

    test('AC4: Start now navigates to Location step', async ({ page }) => {
        const index = new IndexPage(page);
        await expect(index.startNow()).toBeVisible();
        await index.startNow().click();
        await expect(page).toHaveURL(/\/location$/i);
    });
});
