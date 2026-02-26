# FEAT APIs – Performance Test Report

---

## 1. Overview

This round of performance testing focused on the FEAT backend APIs that power the core search journey:

- `POST /api/Search`
  - Initial search
  - Filtered search
  - Pagination
- `GET /api/AutocompleteLocations`
- `GET /api/Courses/{instanceId}` (course details)

UI rendering performance was **not** tested as part of this exercise.

The purpose of this testing was to understand how these backend APIs behave under different traffic levels for private beta, specifically to evaluate:

- Response time behaviour (p95 latency)
- Error rates
- Stability under sustained load
- Failure behaviour under stress
- System limits and scaling boundaries

The goal is to understand operating limits, failure modes, and whether the system is ready for private beta.

---

## 2. Test Environment & Configuration

Performance testing was executed in the **Test** environment, using infrastructure representative of production search configuration.

### Configuration

| Configuration Item        | Value                                      |
|--------------------------|--------------------------------------------|
| Environment              | Test                                       |
| Azure AI Search replicas | 1                                          |
| Auto-scaling             | Not enabled                                |
| API rate limiting        | Not configured                             |
| AI Search auto-scale     | Not supported                              |
| Tool                     | k6                                         |
| Executors                | constant-arrival-rate, ramping-arrival-rate |
| Thresholds defined       | p95 latency + error rate                   |

These constraints are important when interpreting the results.

All stress behaviour must be understood in light of the following:

- Only 1 Azure AI Search replica
- No API rate limiting
- No auto-scaling
- No back-pressure mechanism

---

## 3. Load Model Executed

Load levels were derived from documented private beta volumetric assumptions and extended deliberately to include stress scenarios.

### Private Beta Modelling Assumptions

- 250 invited users
- 30–50% active
- 5–6 searches per session
- Expected sustained load: **~0.04–0.08 RPS**

### Testing Scenarios

- Baseline (quiet usage)
- Expected private beta peak
- Controlled ramp – 3 RPS sustained
- Stress scenario – 6–12 RPS

---

## 3.1 Baseline (Quiet Usage)

Very low sustained RPS to simulate steady-state behaviour.

### Result Summary

| Metric                   | Value   |
|--------------------------|---------|
| Search Initial p95      | ~700ms  |
| Search Filtered p95     | ~135ms  |
| Pagination p95          | ~200ms  |
| Autocomplete p95        | ~50ms   |
| Course Details p95      | ~200ms  |
| Error Rate              | 0%      |
| HTTP Failures           | 0       |

**Assessment**

Stable. No meaningful error rate. Response times consistent.

---

## 3.2 Expected Private Beta Peak

Modelled using volumetric assumptions (250 users, 30–50% active, 5–6 searches per session).

### Result Summary

| Metric                   | Value   |
|--------------------------|---------|
| Search Initial p95      | ~680ms  |
| Filtered p95            | ~143ms  |
| Pagination p95          | ~699ms  |
| Autocomplete p95        | ~55ms   |
| Course Details p95      | ~58ms   |
| Error Rate              | 0%      |
| Failed Checks           | 0       |

**Assessment**

APIs behaved within acceptable limits.  
No significant error rate observed.  
Response times remained within expected bounds.

At this level, the system appears stable for controlled private beta usage.

---

## 3.3 Controlled Ramp Test – 3 RPS Sustained

A controlled ramping scenario was executed to validate behaviour at a moderate sustained load above expected beta traffic but below stress levels.

### Profile

- Ramp to 2 RPS (3 min warm-up)
- Sustain 3 RPS (5 min)
- Ramp down (2 min)
- Total duration: 10 minutes

### Results

- ~3.4 HTTP requests/sec sustained
- 1199 iterations completed
- 2058 HTTP requests executed
- 0% HTTP failures
- 0% API error rate
- 1 dropped iteration across full run

### Performance (p95)

| Endpoint         | p95 Latency |
|------------------|-------------|
| Initial Search   | 881ms       |
| Filtered Search  | 357ms       |
| Pagination       | 914ms       |
| Course Details   | 42ms        |
| Autocomplete     | 72ms        |

All performance thresholds were met comfortably.

**Assessment**

At 3 RPS sustained load, the FEAT APIs remained stable and within acceptable latency bounds, even with:

- A single Azure AI Search replica  
- No rate limiting configured  

This confirms stable behaviour above expected private beta traffic levels.

---

## 3.4 Stress Scenario – 6–12 RPS

This scenario intentionally exceeded expected private beta traffic.

### Purpose

- Identify scaling limits
- Observe degradation behaviour
- Understand failure mode

### Observed Results

| Metric                     | Observed Behaviour |
|----------------------------|-------------------|
| Search API Error Rate      | ~57%              |
| HTTP 500 Responses         | Multiple          |
| Timeouts                   | Observed          |
| Dropped Iterations         | Recorded          |
| Search p95 Latency         | > 50 seconds      |

Autocomplete and Course Details remained stable under the same window:

| Endpoint        | Behaviour                  |
|----------------|----------------------------|
| Autocomplete   | Stable, near 0% errors     |
| Course Details | Stable, near 0% errors     |

The bottleneck under stress was isolated to the **Search endpoint**.

---

## 4. Degradation Behaviour

Under load beyond capacity (6–12 RPS):

- The system did not degrade gradually.
- It returned HTTP 500 responses.
- Response times increased significantly before failure.
- Some requests timed out.
- Dropped iterations occurred.

There is currently:

- No API rate limiting
- No back-pressure mechanism
- No graceful degradation strategy
- No auto-scaling

Failure under stress is abrupt rather than controlled.

---

## 5. Infrastructure Context

During testing:

- Azure AI Search configured with 1 replica
- No auto-scaling
- No API rate limiting

Technical confirmation indicates that under stress, Azure Search limits were reached.

This aligns with the observed 57% failure rate and extreme latency during the 6–12 RPS scenario.

---

## 6. Private Beta Readiness Assessment

Based on volumetric assumptions:

- Expected sustained private beta load: **~0.04–0.08 RPS**

Validated stable load:

- **3 RPS sustained** (≈ 30–70x expected beta traffic)

### Conclusion

- Under realistic private beta traffic levels, the system behaves stably.
- Under moderate sustained load (3 RPS), the system remains stable.
- Under significantly higher stress (6–12 RPS), the system fails due to infrastructure limits.

The failure scenario is outside expected private beta traffic but is important for future planning.

There is no performance blocker for private beta, given controlled access.

---

## 7. Risks Identified

| Risk                         | Impact                        |
|------------------------------|--------------------------------|
| Single Azure AI Search replica | Low scaling ceiling         |
| No rate limiting             | No protection under surge     |
| No graceful degradation      | Abrupt 500 responses          |
| No auto-scaling              | Manual intervention required  |

For private beta, these risks are manageable due to controlled access.  
For public beta or growth scenarios, they must be addressed.

---

## 8. Recommendations

Before public beta:

- Increase Azure AI Search replicas (minimum 2; scale higher during peak)
- Introduce API rate limiting (via FauAPI as planned)
- Define graceful degradation strategy
- Re-run stress testing after scaling changes
- Monitor p95 latency and error rate during private beta

---
