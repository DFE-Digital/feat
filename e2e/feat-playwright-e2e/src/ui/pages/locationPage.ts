import { Page, Locator, expect } from '@playwright/test';

export class LocationPage {
    constructor(private page: Page) {}

    backLink = (): Locator =>
        this.page.locator('a.govuk-back-link');

    heading = (): Locator =>
        this.page.locator('h1.govuk-heading-l', { hasText: 'Where do you want to study?' });

    introParagraph = (): Locator =>
        this.page.getByText('You can search anywhere in England.');

    locationInput = (): Locator =>
        this.page.locator('#Location');

    // Autocomplete (appear when implemented)
    suggestionsMenu = (): Locator =>
        this.page.locator('[role="listbox"], .autocomplete__menu, .govuk-listbox, .ui-autocomplete');

    suggestionItem = (text: string): Locator =>
        this.page.locator('[role="option"], .autocomplete__option, li', { hasText: new RegExp(text, 'i') });

    noResultsMessage = (): Locator =>
        this.page.getByText(/No locations found/i);

    distanceLegend = (): Locator =>
        this.page.locator('legend.govuk-fieldset__legend', { hasText: /How far would you be happy to travel/i });

    distanceRadio = (label: string): Locator =>
        this.page.getByRole('radio', { name: new RegExp(`^${label}$`, 'i') });

    continueButton = (): Locator =>
        this.page.getByRole('button', { name: /^Continue$/i });

    // Error handling
    errorSummary = (): Locator =>
        this.page.locator('.govuk-error-summary')
            .or(this.page.getByRole('heading', { name: /There is a problem/i }).locator('..'));

    errorSummaryHeading = (): Locator =>
        this.page.getByRole('heading', { name: /There is a problem/i });

    fieldError = (text: string): Locator =>
        this.page.locator('.govuk-error-message', { hasText: new RegExp(text, 'i') });

    locationFormGroupError = (): Locator =>
        this.page.locator('#Location').locator('xpath=ancestor::*[contains(@class,"govuk-form-group--error")]');

    async goto() {
        await this.page.goto('/location', { waitUntil: 'networkidle' });
        await expect(this.heading()).toBeVisible();
    }
}
