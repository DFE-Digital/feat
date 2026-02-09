# Decision - 005 - Azure AI Search

## Context and Problem Statement

We needed a database capable of storing and retrieving information for semantic searching.

The system used had to have the following functionality:

* Full-Text Searching
* Vector Storage
* Vector Retrieval and Search
* Facets
* Geolocation Filtering
* Geolocation Distance Calculation

## Considered Options

* Microsoft AI Search
* Microsoft SQL Server 2025
* Elasticsearch

### Evaluation

|    Criteria    | Comment                                                                                        | AI Search | SQL 2025 | Elasticsearch |
|:--------------:|:-----------------------------------------------------------------------------------------------|:---------:|:--------:|:-------------:|
| Text Searching | The technology must suport full-text,fuzzy, and wildcard searches on multiple fields           |     5     |    5     |       5       |
| Vector Storage | The technology must be able to support varying vector sizes and configurations                 |     5     |    3     |       5       |
| Vector Search  | The technology must support searching, measuring, and adapting the algorithm to return results |     4     |    2     |       4       |
|     Facets     | The search should be able to return facets of information and filter on them efficiently       |     5     |    -     |       5       |
|  Geolocation   | The technology must allow distance calculation and filtering on distance                       |     5     |    5     |       5       |
|   Adoption     | The technology should be widely used, with good documentation and support materials            |     5     |    5     |       5       |
|    Support     | The technology should be easily supported within the DfE estate                                |     5     |    3     |       2       |
|   **Total**    |                                                                                                |  **34**   |  **23**  |    **31**     |


## Decision Outcome

We found the following:

- Azure AI Search was already in-use within DfE, even though the vector-based and semantic search parts were not in use
  - The DfE AI Centre of Excellence were able to support us with AI policy exemptions
  - Microsoft were able to offer us additional support
- Even though SQL Server 2025 now supported Vectors, it was discounted because:
  - It was not get available in General Availability at the time the decision was made (it went GA in November 2025)
  - It did not support faceted searching, so additional functionality would have been needed
  - There was no form of semantic re-ranking available
- Elasticsearch did everyghing we needed, but was not in use within DfE and we would have needed to procure licensing to use Elasticsearch Cloud, which we did not have a great amount of time to do

For the reasons above, we decided to move forward with Azure AI Search