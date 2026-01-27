import { Page, Locator, expect } from '@playwright/test';

export class LocationPage {
    constructor(private page: Page) {}

    backLink = (): Locator => this.page.locator('a.govuk-back-link');

    heading = (): Locator =>
        this.page.locator('h1.govuk-heading-l', { hasText: 'Where do you want to study?' });

    locationInput = (): Locator => this.page.locator('#Location');

    // Autocomplete
    suggestionsMenu = (): Locator => this.page.locator('#Location__listbox');

    // Option items (filter out transient/non-option messages)
    suggestionOptions = (): Locator =>
        this.page.locator('#Location__listbox [role="option"]').filter({
            hasNotText: /No locations found|Searching, please wait/i,
        });

    noResultsMessage = (): Locator =>
        this.page.locator('#Location__listbox').getByText(/No locations found/i);

    statusA = (): Locator => this.page.locator('#Location__status--A');

    distanceRadio = (label: string): Locator =>
        this.page.getByRole('radio', { name: new RegExp(`^${escapeRegex(label)}$`, 'i') });

    continueButton = (): Locator => this.page.getByRole('button', { name: /^Continue$/i });

    // Error handling
    errorSummary = (): Locator => this.page.locator('.govuk-error-summary');

    fieldError = (text: string): Locator =>
        this.page.locator('.govuk-error-message', { hasText: new RegExp(escapeRegex(text), 'i') });

    async goto() {
        await this.page.goto('/location', { waitUntil: 'domcontentloaded' });
        await expect(this.heading()).toBeVisible();
        await expect(this.locationInput()).toBeVisible();
        await expect(this.locationInput()).toBeEnabled();
    }

    async enterLocationAndSelectFirst(value: string): Promise<string> {
        const typed = value.trim();
        if (typed.length < 3) throw new Error('Location autocomplete requires at least 3 characters');

        const input = this.locationInput();
        const maxAttempts = 3;

        for (let attempt = 1; attempt <= maxAttempts; attempt++) {
            // Reset any previous state
            await input.click();
            await input.fill('');
            await this.page.keyboard.press('Escape').catch(() => {}); // close stale menus if any

            await input.type(typed, { delay: 60 });

            const expanded = await expect
                .poll(async () => (await input.getAttribute('aria-expanded')) ?? 'false', {
                    timeout: 15_000,
                })
                .toBe('true')
                .then(() => true)
                .catch(() => false);

            if (!expanded) {
                if (attempt === maxAttempts) {
                    throw new Error(`Autocomplete never expanded for "${typed}" (attempt ${attempt})`);
                }
                await this.page.waitForTimeout(300);
                continue;
            }

            // Wait until we either have options OR a no-results outcome
            const ready = await expect
                .poll(async () => {
                    const noResults = await this.noResultsMessage().isVisible().catch(() => false);
                    const optionCount = await this.suggestionOptions().count().catch(() => 0);
                    return noResults ? 'no-results' : optionCount > 0 ? 'options' : 'waiting';
                }, { timeout: 25_000 })
                .not.toBe('waiting')
                .then(async () => {
                    // return the final state to decide next
                    const noResults = await this.noResultsMessage().isVisible().catch(() => false);
                    return noResults ? 'no-results' : 'options';
                })
                .catch(() => 'waiting');

            if (ready === 'no-results') {
                throw new Error(`No locations found for "${typed}"`);
            }

            if (ready !== 'options') {
                if (attempt === maxAttempts) {
                    throw new Error(`Autocomplete did not produce options for "${typed}"`);
                }
                await this.page.waitForTimeout(300);
                continue;
            }

            // Optional: wait for status text to say results available
            await this.statusA()
                .waitFor({ state: 'visible', timeout: 5_000 })
                .catch(() => {});
            await this.page.keyboard.press('ArrowDown');
            await this.page.keyboard.press('Enter');

            // Confirm value is set and menu collapses
            await expect(input).not.toHaveValue('', { timeout: 10_000 });
            await expect
                .poll(async () => (await input.getAttribute('aria-expanded')) ?? 'false', {
                    timeout: 10_000,
                })
                .toBe('false');

            return (await input.inputValue()).trim();
        }

        throw new Error(`Failed to select location for "${typed}" after ${maxAttempts} attempts`);
    }

    async selectDistance(label: string) {
        const radio = this.distanceRadio(label);

        await expect(this.page.locator('input[type="radio"][name="Distance"]').first()).toBeVisible({
            timeout: 10_000,
        });

        await radio.scrollIntoViewIfNeeded();
        await radio.setChecked(true, { force: true });
        await expect(radio).toBeChecked();
    }
}

function escapeRegex(str: string) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}
