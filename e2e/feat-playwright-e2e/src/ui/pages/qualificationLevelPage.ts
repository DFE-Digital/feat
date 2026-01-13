import {Locator, Page, expect } from '@playwright/test';

export class QualificationLevelPage {
    constructor(private page: Page) {}

    backLink = () => this.page.getByRole('link', { name: /^back$/i });

    heading = () =>
        this.page.getByRole('heading', {
            name: /what qualification level/i,
        });

    // Checkboxes
    skillsCheckbox = () =>
        this.page.getByRole('checkbox', {
            name: /get skills and experience/i,
        });

    level1or2Checkbox = () =>
        this.page.getByRole('checkbox', {
            name: /level 1 or 2/i,
        });

    level3Checkbox = () =>
        this.page.getByRole('checkbox', {
            name: /^level 3\b/i,
        });

    level4to8Checkbox = () =>
        this.page.getByRole('checkbox', {
            name: /level 4 to 8/i,
        });

    // Guidance link
    guidanceLink = () =>
        this.page.getByRole('link', {
            name: /what qualification levels mean/i,
        });

    // Continue button
    continueButton = () =>
        this.page.getByRole('button', {
            name: /^continue$/i,
        });

    // Errors

    errorSummary = (): Locator => this.page.locator('.govuk-error-summary').first();
    
    errorSummaryHeading = (): Locator =>
        this.errorSummary().getByRole('heading', { name: /there is a problem/i });

    errorMessageLink = (): Locator =>
        this.errorSummary().getByRole('link', { name: /select .*qualification level/i });
    
    async goto() {
        await this.page.goto('/qualificationlevel', { waitUntil: 'domcontentloaded' });
        await expect(this.page).toHaveURL(/qualification[-]?level/i);
    }

    async selectOptionsAndContinue(
        options: Array<'skills' | 'level1or2' | 'level3' | 'level4to8'>,
    ) {
        const map = {
            skills: this.skillsCheckbox,
            level1or2: this.level1or2Checkbox,
            level3: this.level3Checkbox,
            level4to8: this.level4to8Checkbox,
        } as const;

        for (const key of options) {
            await map[key]().check();
        }

        await this.continueButton().click();
    }
}
