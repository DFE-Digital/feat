# Decision - 002 - Relational Database

## Context and Problem Statement

The site needs a relational database to store rich information about courses and vacancies, but the information does not need to be used as criteria within a search.

We needed to ensure:

* Infrastructure costs are as low as possible
* Infrastructure is simple and easy to manage
* We follow DfE best practices
* The relational database chosen can be supported by the current, and future teams

## Considered Options

* Microsoft SQL Server 2022
* Microsoft SQL Server 2025 Preview
* Azure Flexible PostgreSQL

### Evaluation

|  Criteria   | Comment                                                                                                                                                                                                                                                                                                                                               | MSSQL 2022 | MSSQL 2025 | PostgreSQL |
|:-----------:|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:----------:|:----------:|:----------:|
|    Cost     | SQL Server costs more than PostgreSQL due to the additional licensing costs, however we may be able to make some mitigations on the deployment model to help reduce the costs                                                                                                                                                                         |     3      |     3      |     4      |
| Performance | Performance is on-par across the two solutions                                                                                                                                                                                                                                                                                                        |     4      |     4      |     4      |
|  Features   | SQL 2025 has additional features to support vectors and functionality that AI might be able to leverage, but these features are still in preview mode and not generally available, therefore these shouldn't be considered for a production-ready system yet - this may change after Microsoft Ignite 2025                                            |     4      |     3      |     3      |
| Maintenance | After speaking with Luke Slowen, Richard Boland, and the potential future owners of the service, it has been deemed that DfE would be better suited to attempting to standardise which relational database to use department-wide. The database of choice is Microsoft SQL Server, which means that PostgreSQL should be the exception, not the rule. |     4      |     4      |     1      |
|  **Total**  |                                                                                                                                                                                                                                                                                                                                                       |   **15**   |   **14**   |   **12**   |

## Decision Outcome

Based on the analysis above, we have chosen Microsoft SQL Server 2022 to be the relational database for the site.

### Considerations on selected technology

Pros and cons had been taken to the DfE Architectural Group, along with discussions with Microsoft directly as part of our Tech Team times, however the new features that might become available haven't get moved into General Availability, which is why SQL 2022 has been chosen over SQL 2025 Preview
