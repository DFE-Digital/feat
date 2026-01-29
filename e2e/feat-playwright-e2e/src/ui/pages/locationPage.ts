import { Page, Locator, expect } from '@playwright/test';

export class LocationPage {
    constructor(private page: Page) {}

    backLink = (): Locator => this.page.locator('a.govuk-back-link');

    heading = (): Locator =>
        this.page.locator('h1.govuk-heading-l', { hasText: 'Where do you want to study?' });

    locationInput = (): Locator => this.page.locator('#Location');

    // Autocomplete
    suggestionsMenu = (): Locator => this.page.locator('#Location__listbox');

    suggestionOptions = (): Locator =>
        this.page.locator('#Location__listbox [role="option"]').filter({
            hasNotText: /No locations found|Searching, please wait/i,
        });

    noResultsMessage = (): Locator =>
        this.page.locator('#Location__listbox').getByText(/No locations found/i);

    statusA = (): Locator => this.page.locator('#Location__status--A');

    continueButton = (): Locator => this.page.getByRole('button', { name: /^Continue$/i });

    // Error handling
    errorSummary = (): Locator => this.page.locator('.govuk-error-summary');

    fieldError = (text: string): Locator =>
        this.page.locator('.govuk-error-message', {
            hasText: new RegExp(this.escapeRegex(text), 'i'),
        });

    // Distance radios (scoped to the group for stability)
    private distanceGroup = (): Locator =>
        this.page.getByRole('group', { name: /How far are you able to travel/i });

    distanceRadio = (label: string): Locator =>
        this.distanceGroup().getByRole('radio', {
            name: new RegExp(`^${this.escapeRegex(label)}$`, 'i'),
        });

    async goto() {
        await this.page.goto('/location', { waitUntil: 'domcontentloaded' });
        await expect(this.heading()).toBeVisible();
        await expect(this.locationInput()).toBeVisible();
        await expect(this.locationInput()).toBeEnabled();
    }

   
    async enterLocationAndSelectFirst(value: string): Promise<string> {
        const typed = (value ?? '').trim();
        if (typed.length < 3) throw new Error('Location autocomplete requires at least 3 characters');

        const input = this.locationInput();
        const maxAttempts = 3;

        for (let attempt = 1; attempt <= maxAttempts; attempt++) {
            await input.click();
            await input.fill('');
            await this.page.keyboard.press('Escape').catch(() => {});
            await input.pressSequentially(typed, { delay: 60 });

            const menuVisible = await this.suggestionsMenu()
                .waitFor({ state: 'visible', timeout: 15_000 })
                .then(() => true)
                .catch(() => false);

            if (!menuVisible) {
                if (attempt === maxAttempts) {
                    throw new Error(`Autocomplete listbox never became visible for "${typed}" (attempt ${attempt})`);
                }
                await this.page.waitForTimeout(300);
                continue;
            }

            const ready = await expect
                .poll(
                    async () => {
                        const noResults = await this.noResultsMessage().isVisible().catch(() => false);
                        const optionCount = await this.suggestionOptions().count().catch(() => 0);
                        return noResults ? 'no-results' : optionCount > 0 ? 'options' : 'waiting';
                    },
                    { timeout: 25_000 },
                )
                .not.toBe('waiting')
                .then(async () => {
                    const noResults = await this.noResultsMessage().isVisible().catch(() => false);
                    return noResults ? 'no-results' : 'options';
                })
                .catch(() => 'waiting');

            if (ready === 'no-results') throw new Error(`No locations found for "${typed}"`);

            if (ready !== 'options') {
                if (attempt === maxAttempts) throw new Error(`Autocomplete did not produce options for "${typed}"`);
                await this.page.waitForTimeout(300);
                continue;
            }

            await this.statusA().waitFor({ state: 'attached', timeout: 2_000 }).catch(() => {});

            await this.page.keyboard.press('ArrowDown');
            await this.page.keyboard.press('Enter');

            // Commit check: non-empty value
            await expect(input).not.toHaveValue('', { timeout: 10_000 });

            // Let the listbox close if it does
            await this.suggestionsMenu().waitFor({ state: 'hidden', timeout: 10_000 }).catch(() => {});

            // Extra stability: continue enabled implies form settled
            await expect(this.continueButton()).toBeEnabled({ timeout: 10_000 });

            return (await input.inputValue()).trim();
        }

        throw new Error(`Failed to select location for "${typed}" after ${maxAttempts} attempts`);
    }

    async selectDistance(label: string) {
        const radio = this.distanceRadio(label);

        for (let attempt = 1; attempt <= 4; attempt++) {
            await expect(radio).toBeVisible({ timeout: 10_000 });
            await expect(radio).toBeEnabled({ timeout: 10_000 });

            const handle = await radio.elementHandle();
            if (handle) await handle.waitForElementState('stable');

            await radio.scrollIntoViewIfNeeded();
            await radio.click();

            // Verify it really stuck
            if (await radio.isChecked().catch(() => false)) return;

            await this.page.waitForTimeout(150);
        }

        throw new Error(`Distance "${label}" would not stay checked after retries`);
    }
    
    async expectLocationCommitted() {
        const value = (await this.locationInput().inputValue()).trim();
        expect(value, 'Location input did not contain a committed value').not.toBe('');
    }

    async expectDistanceChecked(label: string) {
        const radio = this.distanceRadio(label);
        await expect(radio, `Distance "${label}" was not actually checked`).toBeChecked({ timeout: 5_000 });
    }

    async clickContinueAndWaitForErrorOrNext(timeoutMs: number = 15_000) {
        const startUrl = this.page.url();

        await this.continueButton().click();

        const navigated = await this.page
            .waitForURL((url) => url.toString() !== startUrl, { timeout: timeoutMs })
            .then(() => true)
            .catch(() => false);

        return navigated ? ('navigated' as const) : ('error' as const); // 'error' | 'navigated'
    }

    private escapeRegex(value: unknown) {
        const str = String(value ?? '');
        return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }
}
