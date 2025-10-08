# Decision - 003 - Caching

## Context and Problem Statement

The site needs to be performant and cost-efficient when undertaking the following:

* Scaling-out to support spikes in demand
* Cost-efficient when handling things such as text embedding

We needed to ensure:

* Infrastructure costs are as low as possible
* Infrastructure is simple and easy to manage
* We follow DfE best practices
* If the site scales, any caching and evictions apply to all scaled instances

## Considered Options

* No Caching
* In-Memory Caching
* FusionCache with an Azure Cache for Redis Backplane
* Microsoft Hybrid Cache with an Azure Cache for Redis Distributed Cache
* Microsoft Hybrid Cache with a Microsoft SQL Server Distributed Cache

### Evaluation

|     Criteria     | Comment                                                                                                                          | No Caching | In-Memory | FusionCache | Hybrid/Redis | Hybrid/SQL |
|:----------------:|:---------------------------------------------------------------------------------------------------------------------------------|:----------:|:---------:|:-----------:|:------------:|:----------:|
|     Adoption     | The implementation should we widely used and have excellent documentation                                                        |     1      |     4     |      4      |      5       |     5      |
| Development Cost | The implementation should not require any additional development cost, tooling, or understanding of the backing cache            |     1      |     3     |      4      |      4       |     4      |
|   Performance    | The caching should work with as little latency as possible across various environments                                           |     1      |     3     |      4      |      4       |     4      |
|     Scaling      | The caching should survive an application restart, deployment, or scaling automatically                                          |     1      |     1     |      4      |      4       |     4      |
|  Clearing Cache  | The caching should allow quick and easy clearing, plus support additional things like tagging for easier related content removal |     1      |     4     |      4      |      4       |     4      |
| Deployment Cost  | The caching should use existing technologies where available which wouldn't incur additional costs                               |     5      |     5     |      3      |      3       |     5      |
|    **Total**     |                                                                                                                                  |   **10**   |  **22**   |   **23**    |    **24**    |   **26**   |

## Decision Outcome

Based on the analysis above, we have chosen Microsoft's Hybrid Search to use an in-memory L1 cache and a SQL Server L2 cache.

Since making a similar choice within another project last year, Microsoft's Hybrid Cache has become generally available and is the recommended Microsoft choice within .NET 9.

Documentation has massively improved and support for SQL Server means that we can rely on the SQL Server for our secondary cache, rather than an additional instance of Azure Cache for Redis, which could potentially reduce costings.


### Considerations on selected technology

During development, the cache can be deployed using L1 cache only, which allows application restarts to flush the cache, but still allow for testing of caching.

Switching over to Redis, if required, is easy enough, but would increase some costs if SQL Server ends up having problems with performance - we will be able to look into this further as part of our performance testing
