# Volumetrics

## Introduction

It is important to understand the Volumetrics of the Find Education and Training site to ensure that the website can handle the
expected traffic without degradation in performance, without incurring unnecessary Azure costs.

This is a new site, so no historical data is available. However, we can make estimates based on data from other
similar sites such as the Find A Course and Find an Apprenticeship.

## Projected Volumetrics

The assumptions will be based on xxx users per month.

We can make reasonable assumptions that the site will be operating during UK day time hours, with increased usage around exam results times in August and January. 
Therefore, we can make an assumption of around yy users per hour at peak times.

Assuming a user session lasts roughly xx minutes: xx * yy = zz concurrent requests.

TODO: Sort volumetrics


| Test Type | Concurrent Users | Requests per second |
|-----------|------------------|---------------------|
| Normal    | xx               | ~x requests/sec     |
| Spike     | yy               | ~y requests/sec     |
| Stress    | zzz              | ~zz requests/sec    |
