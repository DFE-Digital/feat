import { Page, Locator } from '@playwright/test';

export class CheckYourAnswersPage {
    constructor(private page: Page) {}

    heading = (): Locator =>
        this.page.getByRole('heading', { name: /check your answers/i });

    summaryList = (): Locator => this.page.locator('.govuk-summary-list');

    rows = (): Locator => this.page.locator('.govuk-summary-list__row');

    summaryRowByLabel = (label: string): Locator =>
        this.rows().filter({
            has: this.page.locator('.govuk-summary-list__key', { hasText: label }),
        });

    summaryValue = (label: string): Locator =>
        this.summaryRowByLabel(label).locator('.govuk-summary-list__value');

    // “Change” link next to a row
    changeLinkFor = (label: string): Locator =>
        this.summaryRowByLabel(label).getByRole('link', { name: /change/i });

    aiDisclosureLink = (): Locator =>
        this.page.getByRole('link', { name: /how we use ai/i });

    searchButton = (): Locator =>
        this.page.getByRole('button', { name: /^search$/i });

    backLink = (): Locator =>
        this.page.getByRole('link', { name: /^back$/i });
}
