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

|     Criteria     | Comment                                                                                                                                                                                                                                 | No Caching | In-Memory | FusionCache | Hybrid/Redis | Hybrid/SQL |
|:----------------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:----------:|:---------:|:-----------:|:------------:|:----------:|
|     Adoption     | The implementation should we widely used and have excellent documentation                                                                                                                                                               |     1      |     4     |      4      |      4       |     4      |
| Development Cost | The implementation should not require any additional development cost, tooling, or understanding of the backing cache                                                                                                                   |     1      |     3     |      4      |      4       |     4      |
|   Performance    | The caching should work with as little latency as possible across various environments                                                                                                                                                  |     1      |     3     |      4      |      4       |     4      |
|     Scaling      | The caching should survive an application restart, deployment, or scaling automatically                                                                                                                                                 |     1      |     1     |      4      |      4       |     4      |
|  Clearing Cache  | The caching should allow quick and easy clearing, plus support additional things like tagging for easier related content removal, plus clearing the cache on one node of a load balanced system MUST clear the cache on all other nodes |     1      |     2     |      5      |      2       |     2      |
| Deployment Cost  | The caching should use existing technologies where available which wouldn't incur additional costs                                                                                                                                      |     5      |     5     |      3      |      3       |     4      |
|    **Total**     |                                                                                                                                                                                                                                         |   **10**   |  **20**   |   **24**    |    **21**    |   **22**   |

## Decision Outcome

Based on the analysis above, we have chose to use FusionCache as this has great documentation and was, in fact, ahead of Microsoft in making itself HybridCache compatible.

FusionCache allows us to support the following features:

* L1 cache in-memory
* L2 cache using IDistributedCache (in this case, Redis)
* Ability to use the Redis instance as a backplane to enable communication between nodes
* Tagging support
* Clearing the cache using the * tag

While HybridCache is now in mainstream support, it still does not support using the L2 layer as a backplane and notifying other nodes that the cache has been cleared, therefore leaving stale in-memory cache entries in all other nodes.

### Considerations on selected technology

During development, the cache can be deployed using L1 cache only, which allows application restarts to flush the cache, but still allow for testing of caching.