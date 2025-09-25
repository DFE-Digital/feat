# FEAT Playwright E2E Testing Framework

This repository contains an End-to-End (E2E) automation framework for the **FEAT (Find Education and Training)** platform, built with **Playwright** and **TypeScript**.

The framework supports:
- Cross-browser testing (Chromium, Firefox, WebKit)
- Mobile emulation (Pixel 8, iPhone 16)
- Both UI and API tests

It is designed for scalability, maintainability, and ease of use.

---

## Features
- Page Object Model (POM) for maintainable test design
- Parallel test execution for faster feedback
- Tracing, screenshots, and video recording for debugging
- Configurable retries and timeouts
- Detailed HTML reports
- Cross-browser support (Chromium, Firefox, WebKit)
- Mobile device testing
- API testing support with configurable base URLs and headers

---

## Project Structure
```
feat-playwright-e2e/
│
├── src/
│   ├── pages/                # Page Object Models
│   │   └── landing.page.ts
│   ├── tests/                # UI Test files
│   │   └── landingPage.spec.ts
│   ├── api/                  # API Test files
│   │   └── searchApi.spec.ts
│   ├── helpers/              # Utility functions
│   │   └── utilities.ts
│   ├── tasks/                # Reusable task-based functions
│       └── performAction.ts
│
├── playwright.config.ts      # Playwright configuration
├── tsconfig.json             # TypeScript configuration
├── package.json              # Node.js dependencies
├── .eslintrc.json            # ESLint configuration
├── .prettierrc               # Prettier configuration
├── .gitignore                # Files to ignore
├── .env                      # Environment variables
```

---

## Getting Started

### Prerequisites
- [Node.js (LTS version)](https://nodejs.org/en/)
- [Yarn](https://yarnpkg.com/) package manager

### Installation
```bash
# Clone the repository
git clone <repository-url>
cd feat-playwright-e2e

# Install dependencies
yarn install

# Install Playwright browsers
npx playwright install
```

---

## Running Tests

### Run All Tests
```bash
yarn playwright test
```

### Run a Specific Test
```bash
yarn playwright test src/tests/landingPage.spec.ts
```

### Run Tests in a Specific Browser
```bash
yarn playwright test --project=Chromium
```

### Run API Tests Only
```bash
npx playwright test --project=API
```

### Run in Debug Mode
```bash
yarn playwright test --debug
```

---

## Reporting and Debugging

### View Test Report
After running tests, generate and open the HTML report:
```bash
yarn playwright show-report
```

### Screenshots and Videos
- Screenshots: Captured automatically on test failures
- Videos: Retained on test failures (configurable in `playwright.config.ts`)

---

## Configuration

Main settings in `playwright.config.ts` include:
- `testDir`: Directories for UI (`src/tests`) and API (`src/api`) tests
- `timeout`: Global test timeout
- `retries`: Number of retries on failure
- `projects`: Browser, device, and API configurations
- `use`: Default settings (trace, screenshot, video, baseURL, headers)

---

## Contribution Guidelines
1. Fork the repository
2. Create a new branch for your feature/bug fix
3. Commit changes with clear messages
4. Submit a pull request for review

---

## License
This project is licensed under the **MIT License**.  
See the [LICENSE](./LICENSE) file for details.

---

## Support
For questions or issues:
- Open an issue on the repository
- Or contact the maintainer  
