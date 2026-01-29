import { Page, Locator, expect } from '@playwright/test';

export class SearchResultsPage {
    constructor(private page: Page) {}

    urlPath = /\/loadcourses/i;

    heading = (): Locator =>
        this.page.locator('h1.govuk-heading-l', { hasText: 'Your search results' });

    searchAgainLink = (): Locator =>
        this.page.getByRole('link', { name: /^search again$/i });

    sortBlock = (): Locator =>
        this.page.locator('p.govuk-body', { hasText: 'Sort courses by:' });

    activeSort = (): Locator =>
        this.sortBlock().locator('[aria-current="true"]');

    relevanceSortLink = (): Locator =>
        this.sortBlock().getByRole('link', { name: /^relevance$/i });

    distanceSortLink = (): Locator =>
        this.sortBlock().getByRole('link', { name: /^distance$/i });

    distanceSortText = (): Locator =>
        this.sortBlock().locator('span', { hasText: /^distance$/i });

    viewMoreDetailsLinks = (): Locator =>
        this.page.getByRole('link', { name: /view more details/i });

    courseCards = (): Locator =>
        this.page.locator('.govuk-summary-card').filter({
            has: this.page.getByRole('link', { name: /view more details/i }),
        });

    firstCourseCard = (): Locator =>
        this.courseCards().first();

    pagination = (): Locator =>
        this.page.locator('.govuk-pagination');

    nextPageLink = (): Locator =>
        this.page.getByRole('link', { name: /next/i });

    prevPageLink = (): Locator =>
        this.page.getByRole('link', { name: /previous/i });

    currentPageLink = (): Locator =>
        this.page.locator('.govuk-pagination__link[aria-current="page"]');

    filterToggleBtn = (): Locator =>
        this.page.locator('#filterToggleBtn');

    filterPanel = (): Locator =>
        this.page.locator('#filter-panel');

    applyFiltersButton = (): Locator =>
        this.page.getByRole('button', { name: /^apply filters$/i });

    clearAllFiltersLink = (): Locator =>
        this.page.getByRole('link', { name: /clear all filters/i });

    facetCheckboxByLabel = (label: string): Locator =>
        this.page.getByRole('checkbox', { name: new RegExp(`^${escapeRegex(label)}$`, 'i') });

    async expectOnPage() {
        await expect(this.page).toHaveURL(this.urlPath);
        await expect(this.heading()).toBeVisible();
    }

    async openFiltersIfCollapsed() {
        const btn = this.filterToggleBtn();
        if (!(await btn.isVisible().catch(() => false))) return;

        const expanded = await btn.getAttribute('aria-expanded').catch(() => null);
        if (expanded === 'false') {
            await btn.click();
        }
        await expect(this.filterPanel()).toBeVisible();
    }

    async clickRelevanceSort() {
        if ((await this.relevanceSortLink().count().catch(() => 0)) === 0) return;
        await this.relevanceSortLink().click();
        await this.expectOnPage();
    }

    async openFirstCourseDetails() {
        const link = this.viewMoreDetailsLinks().first();
        await expect(link).toBeVisible({ timeout: 30_000 });
        await link.click({ timeout: 30_000 });
    }

    async firstCourseId(): Promise<string> {
        const href = await this.viewMoreDetailsLinks().first().getAttribute('href');
        if (!href) return '';
        const match = href.match(/\/courses\/([0-9a-fA-F-]{36})/);
        return match?.[1] ?? '';
    }

    async firstCourseTitle(): Promise<string> {
        const card = this.firstCourseCard();
        const heading = card.locator('h2, h3').first();
        const text = await heading.innerText().catch(() => '');
        return text.trim();
    }

    async hasDistanceSortLink(): Promise<boolean> {
        const count = await this.distanceSortLink().count().catch(() => 0);
        return count > 0;
    }

    async hasRelevanceSortLink(): Promise<boolean> {
        const count = await this.relevanceSortLink().count().catch(() => 0);
        return count > 0;
    }

    async isPaginationVisible(timeoutMs: number = 10_000): Promise<boolean> {
        await Promise.race([
            this.courseCards().first().waitFor({ state: 'visible', timeout: timeoutMs }).catch(() => {}),
            this.page.getByText(/no results/i).first().waitFor({ state: 'visible', timeout: timeoutMs }).catch(() => {}),
            this.page.waitForTimeout(500),
        ]);

        return await this.pagination().isVisible().catch(() => false);
    }
}

function escapeRegex(str: string) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}
