# Decision - 001 - External Entry Point

## Context and Problem Statement

The site needs to ensure it is protected against threats on the internet, but that it is also performant and scalable.

We needed to ensure:

* Infrastructure costs are as low as possible
* Infrastructure is simple and easy to manage
* The site must support custom DfE subdomains and SSL
* The site can be scaled out and load balanced accordingly

## Considered Options

* Azure Front Door
* Azure Application Gateway
* Shared DfE Baracuda WAF

### Evaluation

|  Criteria   | Comment                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | Front Door | Application Gateway | Baracuda WAF |
|:-----------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:----------:|:-------------------:|:------------:|
|    Cost     | Front Door Premium is approximately 330 USD per month, Front Door Standard is 35 USD per month, and Application Gateway has a wide range of costs, but usually starting at around 330 USD per month, the Baracuda WAF is already managed by DfE and so incurs no extra cost                                                                                                                                                                                                 |     4      |          3          |      5       |
| Performance | Performance is on-par across the three solutions                                                                                                                                                                                                                                                                                                                                                                                                                            |     4      |          4          |      4       |
|  Security   | Front Door Premium supports additional managed rules, allowing for restrictions to be bypassed for friendly search engine bots. It also supports managed rulesets which adhere to WASP security guidelines. Application Gateway can do this, but at additional cost and usually involving a combination of Application Gateway and Front Door. The Baracuda WAF is already managed by DfE and should already be designed to adhere to the current WASP security guidelines. |     4      |          2          |      4       |
| Maintenance | Front Door allows for custom domains and SSL certificate generation. Application Gateway requires SSL certificates to be deployed and managed separately, increasing maintenance costs and effort, plus the risk of missing a renewal. There are already processes in place to support handling domain renewals etc for the Baracuda firewall within DfE                                                                                                                    |     4      |          1          |      5       |
|  **Total**  |                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |   **16**   |       **10**        |     **18**   |

## Decision Outcome

Based on the analysis above, we have chosen the DfE Baracuda WAF to protect the site.

### Considerations on selected technology

Pros and cons had been taken to the DfE Architectural Group monthly meetings and, while Application Gateway does have some features that give it a bonus over Front Door, none of those features are being used within this service.

In this situation, however, as we have no custom rules that need defining, the Baracuda WAF will work sufficiently for us.
