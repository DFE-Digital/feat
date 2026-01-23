import { Page, expect, Locator } from '@playwright/test';

export class Footer {
    private footer: Locator;

    // List all footer links here
    private links = [
        { name: 'Accessibility', url: 'https://accessibility-statements.education.gov.uk/s/56' },
        { name: 'Privacy', url: '/linkpages/privacy' },
        { name: 'Cookies', url: '/linkpages/cookies' },
        { name: 'Contact information', url: 'https://www.skillsforcareers.education.gov.uk/pages/help/contact-information' },
        { name: 'Scotland', url: 'https://www.myworldofwork.co.uk/' },
        { name: 'Wales', url: 'https://careerswales.gov.wales/' },
        { name: 'Northern Ireland', url: 'https://www.nidirect.gov.uk/campaigns/careers' },
    ];

    constructor(private page: Page) {
        this.footer = page.locator('footer');
    }

    async expectVisible() {
        await expect(this.footer).toBeVisible();
    }

    link(name: string) {
        return this.footer.getByRole('link', { name });
    }

    // Click and verify links, only for internal URLs
    async checkLinks() {
        await this.expectVisible();

        for (const link of this.links) {
            const locator = this.link(link.name);
            await expect(locator).toBeVisible();
            await expect(locator).toHaveAttribute('href', link.url);

            // Navigate only for internal links
            if (link.url.startsWith('/')) {
                await locator.click();
                await expect(this.page).toHaveURL(link.url);
                await this.page.goBack();
            }
        }
    }
}
