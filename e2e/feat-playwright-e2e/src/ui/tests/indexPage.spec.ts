import { test, expect } from '@playwright/test';
import { IndexPage } from '../pages/indexPage';

test.describe('FEAT – 1.0 Index (Direct search entry)', () => {
    test.beforeEach(async ({ page }) => {
        const index = new IndexPage(page);
        await index.goto();
    });

    test('AC1/AC2: phase banner text and feedback link are visible; feedback link same tab', async ({ page }) => {
        const index = new IndexPage(page);

        await expect(index.phaseBanner()).toBeVisible();
        await expect(index.phaseBanner()).toContainText('This is a new service. Help us improve it and');
        await expect(index.feedbackLink()).toBeVisible();
        await expect(index.feedbackLink()).toContainText(/give your feedback/i);
        await expect(index.feedbackLink()).toHaveClass(/govuk-link/);

        // opens in new tab
        await expect(index.feedbackLink()).toHaveAttribute('target', '_blank');

        // doesn't assert the exact URL, just that it's an external link
        const href = await index.feedbackLink().getAttribute('href');
        expect(href, 'Feedback link should have an href').toBeTruthy();
        expect(href!, 'Feedback link should be external').toMatch(/^https?:\/\//);

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

    test('Layout AC3: service name link is visible in header', async ({ page }) => {
        const index = new IndexPage(page);
        await expect(index.serviceNameLink()).toBeVisible();
        await expect(index.serviceNameLink()).toHaveText(/Find education and training/i);
        await expect(index.serviceNameLink()).toHaveClass(/govuk-service-navigation__link/);
    });

    test('AC4 + Layout AC6: Start now visible, styled and navigates to Location step', async ({ page }) => {
        const index = new IndexPage(page);
        await expect(index.startNow()).toBeVisible();
        await expect(index.startNow()).toHaveClass(/govuk-button/);
        await index.startNow().click();
        await expect(page).toHaveURL(/\/location$/i);
    });
});
