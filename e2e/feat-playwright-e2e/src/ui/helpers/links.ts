import { Locator, expect } from '@playwright/test';

// For links that must NOT open a new tab
export async function expectOpensSameTab(link: Locator) {
    // Either no target attribute or explicitly _self
    await expect(link).toHaveAttribute('target', /(^$)|(^_self$)/);
}
