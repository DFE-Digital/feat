# Decision - 004 - Replacing Azure Cache for Redis

## Context and Problem Statement

Azure Cache for Redis is being retured on September 30th 2028, as specified in the [Azure Cache for Redis Retirement FAQ](https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/retirement-faq)

* Creation of new instances will be blocked from April 1st 2026
* Existing customers will be blocked from creating instances from October 1st 2026
* Instances will be disabled from October 1st 2028

## Considered Options

* Do nothing
* Put in a migration plan for 2026
* Upgrade now

## Decision Outcome

To avoid having to redeploy and migrate at a later date, we have taken the decision to move straight to Azure Managed Redis.

As we were due to use a relatively lightweight cache with a fairly low overhead, we will be able to stay with the lower-tiered Balanced instances, therefore keeping costs low.

In order to make these instances as cost effective as possible, it is deemed that we will be use the following:

* Development: B0 One-Node (Non-High Availability)
* Staging: B0 Two-Node (High Availability)
* Production: B1 Two-Node (High Availability)

If monitoring shows that the cache is getting filled quickly, we can look to scale up to B3 or B5 to increase the memory headroom.

