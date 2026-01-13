import { Page, Locator, expect } from '@playwright/test';

export class LocationPage {
    constructor(private page: Page) {}

    backLink = (): Locator => this.page.locator('a.govuk-back-link');

    heading = (): Locator =>
        this.page.locator('h1.govuk-heading-l', { hasText: 'Where do you want to study?' });

    introParagraph = (): Locator =>
        this.page.getByText('You can search anywhere in England.');

    locationInput = (): Locator => this.page.locator('#Location');

    // Autocomplete
    suggestionsMenu = (): Locator => this.page.locator('#Location__listbox');
    
    suggestionOptions = (): Locator =>
        this.page.locator(
            '#Location__listbox [role="option"]' +
            ':not(:has-text("No results found"))' +
            ':not(:has-text("Searching, please wait"))'
        );

    noResultsMessage = (): Locator =>
        this.page.getByText(/No locations found/i);

    distanceLegend = (): Locator =>
        this.page.locator('legend.govuk-fieldset__legend', {
            hasText: /How far would you be happy to travel/i,
        });

    distanceRadio = (label: string): Locator =>
        this.page.getByRole('radio', { name: new RegExp(`^${label}$`, 'i') });

    continueButton = (): Locator =>
        this.page.getByRole('button', { name: /^Continue$/i });

    // Error handling
    errorSummary = (): Locator => this.page.locator('.govuk-error-summary');

    errorSummaryHeading = (): Locator =>
        this.page.getByRole('heading', { name: /There is a problem/i });

    fieldError = (text: string): Locator =>
        this.page.locator('.govuk-error-message', { hasText: new RegExp(text, 'i') });

    locationFormGroupError = (): Locator =>
        this.page
            .locator('#Location')
            .locator('xpath=ancestor::*[contains(@class,"govuk-form-group--error")]');

    async goto() {
        await this.page.goto('/location', { waitUntil: 'networkidle' });
        await expect(this.heading()).toBeVisible();
    }

    async enterLocationAndSelectFirst(value: string) {
        const typed = value.trim();
        if (typed.length < 3) {
            throw new Error('Location autocomplete requires at least 3 characters');
        }

        const input = this.locationInput();
        await input.click();
        await input.fill('');

        await input.pressSequentially(typed, { delay: 50 });

        await expect(this.suggestionsMenu()).toBeAttached({ timeout: 10_000 });
        await expect(this.suggestionsMenu()).not.toHaveClass(/autocomplete__menu--hidden/i, {
            timeout: 10_000,
        });

        // either no-results is visible OR we have real options
        await expect
            .poll(
                async () => {
                    const noResults = await this.noResultsMessage().isVisible().catch(() => false);
                    const optionCount = await this.suggestionOptions().count();
                    return noResults || optionCount > 0;
                },
                { timeout: 20_000 }
            )
            .toBe(true);

        if (await this.noResultsMessage().isVisible().catch(() => false)) {
            throw new Error(`No locations found for "${typed}"`);
        }

        const firstRealOption = this.suggestionOptions().first();

        // click once it exists
        await firstRealOption.scrollIntoViewIfNeeded();
        const firstText = (await firstRealOption.innerText()).trim();

        await firstRealOption.click({ force: true });

        await expect(input).toHaveValue(firstText, { timeout: 10_000 });
        await expect(this.suggestionsMenu()).toHaveClass(/autocomplete__menu--hidden/i, {
            timeout: 10_000,
        });

        return firstText;
    }



}
