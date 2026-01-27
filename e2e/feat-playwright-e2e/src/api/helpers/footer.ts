import { expect, Page } from '@playwright/test';

export class Footer {
    constructor(private page: Page) {}

    async expectVisible() {
        await expect(this.page.locator('footer')).toBeVisible();
    }

    link(name: string) {
        return this.page.getByRole('link', { name });
    }

    async expectLinkHref(name: string, hrefPattern: RegExp) {
        await expect(this.link(name)).toHaveAttribute('href', hrefPattern);
    }
}
