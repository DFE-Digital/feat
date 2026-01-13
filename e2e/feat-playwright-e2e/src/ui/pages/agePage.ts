import { Page, Locator, expect } from '@playwright/test';

export class AgePage {
    constructor(private page: Page) {}

    // Frame
    backLink = (): Locator => this.page.locator('a.govuk-back-link');

    heading = (): Locator =>
        this.page.getByRole('heading', { name: /^Your age$/i });

    // Intro 
    introPara1 = (): Locator =>
        this.page.getByText(/This is optional but telling us means you will see more relevant results/i);

    introPara2 = (): Locator =>
        this.page.getByText(/Not all education and training courses are available to everyone/i);

    introPara3 = (): Locator =>
        this.page.getByText(/This is often linked to age/i);

    // “Discover the different training options” link
    trainingOptionsLink = (): Locator =>
        this.page.getByRole('link', { name: /Discover the different training options/i });

    // Question block
    questionLegend = (): Locator =>
        this.page.locator('legend.govuk-fieldset__legend', { hasText: /How old are you\?\s*\(optional\)/i });

    // Radios (single-select)
    ageOption = (label: '16 or 17' | '18' | '19' | '20 - 24' | '25 or older'): Locator =>
        this.page.getByRole('radio', { name: new RegExp(`^${label}$`, 'i') });

    // Primary action
    continueButton = (): Locator =>
        this.page.getByRole('button', { name: /^Continue$/i });
    
    async selectAge(label: '16 or 17' | '18' | '19' | '20 - 24' | '25 or older') {
        const radio = this.ageOption(label);
        await radio.check();
        await expect(radio).toBeChecked();
    }

    async goto() {
        await this.page.goto('/age', { waitUntil: 'domcontentloaded' });
        await expect(this.heading()).toBeVisible();
    }
}
