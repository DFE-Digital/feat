import { Page, Locator, expect } from '@playwright/test';

function escapeRegex(str: string) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

export class CourseDetailsPage {
    constructor(private page: Page) {}

    urlPath = /\/courses\/[0-9a-fA-F-]{36}/;

    backLink = (): Locator =>
        this.page.locator('a.govuk-back-link', { hasText: /back/i });
    
    courseTitle = (titleText: string): Locator =>
        this.page.locator(`h2.govuk-summary-card__title`, { hasText: titleText });


    cardByTitle = (title: string): Locator =>
        this.page.locator('.govuk-summary-card').filter({
            has: this.page.locator('.govuk-summary-card__title', {
                hasText: new RegExp(`^${escapeRegex(title)}$`, 'i'),
            }),
        }).first();

    cardKey = (cardTitle: string, key: string): Locator =>
        this.cardByTitle(cardTitle)
            .locator('.govuk-summary-list__key')
            .filter({ hasText: new RegExp(`^${escapeRegex(key)}$`, 'i') })
            .first();

    cardValueForKey = (cardTitle: string, key: string): Locator =>
        this.cardByTitle(cardTitle)
            .locator('.govuk-summary-list__row', {
                has: this.page.locator('.govuk-summary-list__key', {
                    hasText: new RegExp(`^${escapeRegex(key)}$`, 'i'),
                }),
            })
            .locator('.govuk-summary-list__value')
            .first();

    goToButton = (): Locator =>
        this.page.getByRole('button', { name: /go to/i });

    relatedLinksHeading = (): Locator =>
        this.page.getByRole('heading', { name: /^related links$/i });

    relatedLink = (name: string): Locator =>
        this.page.getByRole('link', { name: new RegExp(escapeRegex(name), 'i') });

    async expectOnPage() {
        await expect(this.page).toHaveURL(this.urlPath, { timeout: 15_000 });
        await expect(this.courseTitle('About the course')).toBeVisible({ timeout: 15000 });
        await expect(this.courseTitle('Location')).toBeVisible({ timeout: 15000 });
        await expect(this.courseTitle('Course details')).toBeVisible({ timeout: 15000 });
        await expect(this.goToButton()).toBeVisible({ timeout: 15_000 });
        await expect(this.relatedLinksHeading()).toBeVisible({ timeout: 15_000 });
    }

    async expectAnyKeyExists(cardTitle: string, keys: string[]) {
        for (const key of keys) {
            const count = await this.cardKey(cardTitle, key).count().catch(() => 0);
            if (count > 0) return;
        }
        throw new Error(`None of the expected keys were found in "${cardTitle}": ${keys.join(', ')}`);
    }
}
