import { Page, Locator, expect } from '@playwright/test';

export class CookieBanner {
    constructor(private page: Page) {}

    heading = (): Locator =>
        this.page.getByRole('heading', {
            name: 'Cookies on Find Education and Training',
        });

    // Banner wrapper
    container = (): Locator =>
        this.page.locator('.govuk-cookie-banner__message.govuk-width-container');

    acceptButton = (): Locator =>
        this.container().getByRole('button', { name: 'Accept analytics cookies' });

    rejectButton = (): Locator =>
        this.container().getByRole('button', { name: 'Reject analytics cookies' });

    viewCookiesLink = (): Locator =>
        this.container().getByRole('link', { name: 'View cookies' });

    async expectVisible() {
        await expect(this.heading()).toBeVisible();
        await expect(this.container()).toBeVisible();
        await expect(this.acceptButton()).toBeVisible();
        await expect(this.rejectButton()).toBeVisible();
        await expect(this.viewCookiesLink()).toBeVisible();
    }

    async expectHidden() {
        await expect(this.container()).toBeHidden();
        await expect(this.heading()).toBeHidden();
    }

    async accept() {
        await this.expectVisible();
        await this.acceptButton().click();
        await this.expectHidden();
    }

    async reject() {
        await this.expectVisible();
        await this.rejectButton().click();
        await this.expectHidden();
    }

    async goToCookiesPage() {
        await this.viewCookiesLink().click();
        await expect(this.page).toHaveURL(/\/linkpages\/cookies$/i);
    }
}
