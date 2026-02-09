# Decision - 006 - Replacing Text Embedding Algorithm

## Context and Problem Statement

The text embedding used in Alpha and early Beta was `text-embedding-ada-002`

As models are only officially supported for 12 months, we made the decision to move to a more recently model.

We were also limited by the models made available to us by the DfE Centre of Excellence OpenAPI endpoint

## Considered Options

* `text-embedding-3-large`
* `text-embedding-3-small`
* `text-embedding-3-large` with MRL compression

### Evaluation

|        Criteria        | Comment                                                                         | ADA-002 | Large  | Small  | Compressed |
|:----------------------:|:--------------------------------------------------------------------------------|:-------:|:------:|:------:|:----------:|
|          Size          | The storage size for the vector? (Smaller is better)                            |    3    |   2    |   4    |     5      |
| Generation Performance | How performant is generating the vector?                                        |    3    |   2    |   3    |     1      |
| Retrieval Performance  | How performant are searches using this vector type?                             |    4    |   4    |   4    |     2      |
|        Accuracy        | How relevant are the search results returned when using this vector type?       |    3    |   5    |   1    |     2      |
|         Support        | How likely is the embedding model to remain in support for a further 12 months? |    1    |   5    |   5    |     5      |
|       **Total**        |                                                                                 | **14**  | **18** | **17** |   **15**   |


## Decision Outcome

We found the following:

- Whist the small embedding model was performant, relevance and accuracy was such an important factor, that we chose text-embedding-3-large
- The compression of the vectors increased the performance overhead, and also dropped the retrieval scores.

For the reasons above, we decided to move forward with `text-embedding-3-large`
