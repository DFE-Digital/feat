import { Page, Locator, expect } from '@playwright/test';

export class IndexPage {
    constructor(private page: Page) {}

    phaseBanner = (): Locator =>
        this.page.locator('.govuk-phase-banner__text');

    feedbackLink = (): Locator =>
        this.page.getByRole('link', { name: /give your feedback/i });

    serviceNameLink = (): Locator =>
        this.page.locator('a.govuk-service-navigation__link', { hasText: 'Find education and training' });
    
    mainHeading = (): Locator =>
        this.page.getByRole('heading', { name: 'Find education and training opportunities' });

    introParagraph = (): Locator =>
        this.page.locator('main p').first();

    startNow = (): Locator =>
        this.page.getByRole('button', { name: /^start now$/i });

    async goto() {
        await this.page.goto('/', { waitUntil: 'networkidle' });
        await expect(this.page).toHaveURL(/\/$/);
        await expect(this.mainHeading()).toBeVisible();
    }
}
