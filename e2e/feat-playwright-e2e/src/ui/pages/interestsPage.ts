import { Page, Locator, expect } from '@playwright/test';

export class InterestsPage {
    constructor(private page: Page) {}

    // Navigation / headings
    backLink = (): Locator =>
        this.page.getByRole('link', { name: /^Back$/i });

    heading = (): Locator =>
        this.page.getByRole('heading', {
            name: /What courses, subjects, jobs, or careers are you interested in\?/i,
        });

    // Guidance & examples
    guidancePara = (): Locator =>
        this.page.getByText(
            /Be as specific or broad as you like\. You can try searching for academic subjects, practical skills, or just start from an idea of what you'd like to do\./i
        );

    examplesList = (): Locator =>
        this.page.locator('ul, .govuk-list').filter({
            hasText: /maths|bricklaying|working with animals/i,
        });

    infoLine = (): Locator =>
        this.page.getByText(/If you skip this question, the search results will be everything in your chosen location/i);

    // Inputs (label-bound)
    interestLabel = (n: 1 | 2 | 3): Locator =>
        this.page.locator(`label[for="UserInterest${n}"]`);

    interestInput = (n: 1 | 2 | 3): Locator =>
        this.page.locator(`#UserInterest${n}`);

    // Continue
    continueButton = (): Locator =>
        this.page.getByRole('button', { name: /^Continue$/i });

    // Errors
    errorSummary = (): Locator =>
        this.page.locator('.govuk-error-summary').first();
            //.or(this.page.getByRole('heading', { name: /There is a problem/i }).locator('..'));

    errorSummaryHeading = (): Locator =>
        this.errorSummary().getByRole('heading', { name: /There is a problem/i });

    interest1InlineError = (): Locator =>
        this.page.locator('.govuk-error-message', { hasText: /Enter an interest/i });

    errorSummaryLink() {
        return this.page.locator('.govuk-error-summary a');
    }
    async goto() {
        await this.page.goto('/interests', { waitUntil: 'domcontentloaded' });
        await expect(this.heading()).toBeVisible();
    }
}
