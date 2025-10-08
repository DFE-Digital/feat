# FEAT Project – QA Test Plan

## Document History

| Version | Date       | Author               | Reviewer(s)                                   | Approver                | Description of Changes          |
|---------|------------|--------------------|-----------------------------------------------|-------------------------|--------------------------------|
| 1.0     | 11/09/2025 | Varsha Krishnamurthy | 1. Stuart Duncan<br>2. Clare Arolker<br>3. Anita Holcroft (DfE) | Initial draft of the test plan |

---

## 1. Introduction

The **Find Education and Training (FEAT)** platform consolidates all government-funded education and training opportunities into a single, unified search service. It integrates datasets from multiple sources (NCS, Apprenticeships, and Discover Uni), enriches them with AI embeddings, and exposes them through a user-facing website and API.

### Technology Stack

- **Azure Infrastructure:** Front Door, Blob, Bastion
- **.NET 9 Services:** API + ingestion pipeline
- **Azure AI Search:** Vector embeddings enabling hybrid semantic + keyword search
- **Azure Cache:** Microsoft Hybrid Cache — in-memory L1 cache + SQL Server L2 cache for performance and resilience
- **UI Layer:** DfE / Gov.UK Design System

### Platform Aims

- Empower young people and adults to explore post-16 education and training options.
- Support inclusivity for users with barriers (language, accessibility needs, dyslexia).
- Enable intermediaries (parents, teachers, advisers) to discover accurate opportunities.
- Ensure transparency on the role of AI in search results.
- Comply with accessibility standards (Gov.UK and WCAG 2.2 AA).

### Document Purpose

This test plan defines the **quality assurance strategy** for FEAT. It covers scope, objectives, test approach, environments, risks, deliverables, defect management, and best practices to ensure FEAT is delivered as a reliable, performant, secure, and user-centric service.

---

## 2. Testing Objectives

The primary objective of testing is to validate that the FEAT platform meets **functional, non-functional, and compliance requirements** while delivering a high-quality user experience.

### Core Objectives

#### Functional Validation

- Confirm correctness of website, API, and ingestion pipeline features.
- Ensure seamless integration between components (API ↔ DB ↔ Search ↔ Cache).

#### Data Quality & Relevance

- Validate accuracy, completeness, deduplication, and consistency of datasets.
- Test AI embeddings for semantic relevance, typo tolerance, and edge cases.

#### Non-Functional Qualities

- Verify performance, scalability, and resilience under peak load.
- Confirm security posture against OWASP Top 10 vulnerabilities.
- Validate disaster recovery readiness (RTO/RPO, failover, backups).

#### Compliance & Accessibility

- Ensure compliance with Gov.UK design standards.
- Validate WCAG 2.2 AA accessibility, including screen readers and keyboard-only navigation.
- Confirm AI disclosure and transparency requirements.

#### Operational Readiness

- For private beta, focus on ensuring FEAT is stable, testable, and observable.
- Broader service transition processes apply later at public beta once integrated into wider Skills journey (e.g., NCS or Skills for Careers).

---

## 3. Testing Scope

Defines boundaries of testing: what is **in scope** and **out of scope**.

### 3.1 In Scope

#### Functional Testing

- **Website:** search, navigation, filters, AI disclosure, accessibility features
- **API:** query handling, endpoints, caching, and error handling
- **Ingestion Pipeline:** data ingestion, transformation, and embeddings generation

#### Integration Testing

- End-to-end data flow: Ingestion → Database → Search Index → API → Website
- Cache synchronisation and expiry validation
- API contract validation (including consumer-driven contract testing)

#### Data Quality & Relevance Testing

- Schema validation (PK/FK integrity, mandatory fields)
- Deduplication and consistency across datasets
- Semantic similarity and AI embeddings validation
- Validation of data completeness and edge cases

#### Non-Functional Testing

- **Performance & Scalability:** Load, stress, endurance testing for API and frontend
- **Accessibility:** Automated + manual WCAG 2.2 AA compliance
- **Security:** OWASP Top 10, penetration testing, input sanitisation
- **Disaster Recovery (DR):** Failover testing, backup/restore validation

#### Cross-Platform & Device Testing

- Browser coverage (Chromium, Edge, Safari, Firefox)
- Mobile-first validation across iOS and Android devices

### 3.2 Out of Scope

- Third-party provider systems (e.g., NCS, Apprenticeship Service, HESA) infrastructure – outside DfE control
- AI model training or fine-tuning – focus is on integration & relevance validation, not model development

---

## 4. Test Approach

A **multi-layered strategy** combining functional, non-functional, and compliance testing. Automated testing where feasible, complemented by exploratory/manual QA for usability and accessibility.

### Testing Methodologies

#### Manual Testing

- Exploratory, smoke, sanity, and accessibility
- Testing of Data model and Ingestion service
- Usability checks aligned with GovUK Design System

#### Automated Testing

- UI, API, regression, and E2E flows
- CI/CD integrated, triggered on deployments

### Tools

- **Functional/Automation:** Playwright + .NET, Postman, K6
- **Performance:** JMeter, Azure Load Testing or K6
- **Accessibility:** Lighthouse, Pa11y, axe-core, WAVE
- **Security:** OWASP ZAP, SonarQube
- **CI/CD:** GitHub Actions

### Testing Phases

1. **Unit Testing (Dev-led)** – Core functions, APIs, ingestion pipeline; run in CI pipeline
2. **Integration Testing (Dev/QA)** – Validate API contracts, cache sync, ingestion→DB→Search flows; Pact tests
3. **Data Quality & Relevance Testing (QA-led)** – Schema validation, deduplication, AI embeddings relevance, edge cases
4. **End-to-End (QA-led)** – Simulate real journeys, browser/device coverage
5. **Performance & Scalability (QA-led)** – Simulate peak traffic, stress ingestion pipeline, resilience checks
6. **Accessibility (QA-led)** – Automated + manual testing with assistive tech
7. **Security (Dev/QA)** – Input sanitisation, auth/role checks, OWASP Top 10
8. **Disaster Recovery (Dev/QA)** – Validate RTO/RPO, test cache/DB failover, Blob recovery

---

## 5. Test Environments

### 5.1 Environment Setup

| Environment | Purpose                  | Characteristics |
|-------------|--------------------------|----------------|
| Dev         | Development              |                |
| Test        | Functional + integration | Stable builds, anonymised datasets, mocks for 3rd party APIs |
| Staging     | E2E + performance + security | Mirrors production, realistic data, AI embeddings applied |
| Production  | Live service             | Limited to smoke tests + monitoring |

### 5.2 Test Data

- **Synthetic Test Data:** Generated sample courses, apprenticeships, and providers
- **Realistic Production-like Data:** Anonymised datasets from NCS / Discover Uni
- **Negative Test Data:** Corrupted, missing, malformed records to test ingestion resilience
- **AI Edge Cases:** Queries with synonyms, ambiguous words, misspellings

---

## 6. Entry & Exit Criteria

### Entry Criteria

- Approved requirements/user stories
- Stable build deployed to Testing/Staging
- Test data available in required format

### Exit Criteria

- 100%+ test case execution completed
- No open Priority 1 and 2, Severity 1 or 2 defects
- Performance benchmarks met (response time < 2s for 95% queries – TBC)
- Accessibility compliance confirmed

---

## 7. Risks & Mitigations

| Risk                       | Impact                       | Mitigation                                    |
|-----------------------------|------------------------------|-----------------------------------------------|
| Incomplete/dirty source data | Poor search results          | Data validation & cleansing rules, anomaly detection tests |
| AI model returns irrelevant results | User frustration, lack of trust | Relevance tests, human-in-loop review for edge cases |
| Cache misconfiguration      | Stale/incorrect results      | Automated cache expiry tests                  |
| Infrastructure scaling issues | Site downtime under load    | Load & stress testing in PERF environment    |
| Security vulnerabilities    | Data breach                  | Azure Security Center monitoring             |

---

## 8. Test Deliverables

- Test Strategy & Detailed Test Plan
- Automated Test Suites: UI (Playwright with .NET), API (Postman, Playwright/K6)
- Manual Test cases covering Data Quality
- Test Execution Reports
- Accessibility audit reports
- Security & penetration test reports
- Performance benchmark reports
- Defect Reports (Jira)
- Final Test Closure Report

---

## 9. Governance & Reporting

- **Test Management Tool:** Jira, Git
- **Defect Tracking:** Jira with agreed severity/priority
- **Reporting Cadence:** Daily progress in standups, weekly QA dashboard, Go/No-Go quality gate before release

---

## 10. Defect Management, Source of Truth, and QA Best Practices

### 10.1 Defect Lifecycle

| Stage                  | Description                                                                 | Responsibility      |
|------------------------|-----------------------------------------------------------------------------|-------------------|
| New / Logged           | Reported in tool with steps, env, severity, screenshots/logs                | QA Tester         |
| Triaged / Confirmed    | QA Lead/Product Owner reviews, prioritizes, filters duplicates              | QA Lead/PO        |
| Assigned               | Assigned to responsible developer/team                                       | Development Team  |
| In Progress            | Developer investigates/fixes, QA may clarify                                | Developer         |
| Fixed / Ready for Retest | Fix deployed to test/staging, QA prepares for retest                        | Developer / QA    |
| Retest                 | QA validates fix and regression                                              | QA Tester         |
| Closed                 | Defect passes retest, meets acceptance criteria                              | QA Lead           |
| Reopened               | If defect persists, reopened and reassigned                                   | QA Tester         |

**Severity Levels**

- **S1 (Critical):** Blocking production or core functionality
- **S2 (High):** Major feature broken, workaround exists
- **S3 (Medium):** Minor feature or UI issue
- **S4 (Low / Cosmetic):** Minor visual/textual issues

### 10.2 Source of Truth

- **UI:** Approved prototype
- **Functional:** User stories and acceptance criteria
- **Test Cases & Automation Scripts:** GitHub version-controlled repo
- **Defects / Issues:** Tracked in Jira, linked to stories/test cases
- **Test Execution Reports & Metrics:** Centralized QA repository
- **Communication & Reporting:** Weekly dashboard and standups

### 10.3 QA Best Practices

- Traceability: Link test cases to requirements; defects to test cases
- Shift-Left Testing: Test early to detect defects sooner
- Automation First: Regression, critical workflows, API tests
- Exploratory Testing: Complement automation for usability, accessibility, edge cases
- Peer Reviews: Review test cases, scripts, defect logs
- Consistent Defect Logging: Include env, reproduction steps, screenshots, logs, test data
- Continuous Improvement: Lessons learned after each release
- Metrics & Reporting: Track defect density, age, automation coverage, performance benchmarks, accessibility scores
- Environment Hygiene: Refresh and stabilize test environments to mirror production
- Compliance Checks: GDPR, accessibility, AI transparency, security included in QA processes  

