# Data Ingestion

As part of the work undertaken during the Private Beta, we had to work with a number of different data sources, some of which came in different formats to the ones we expected.

This included:

- ZIP files containing CSV/XML data dumps
- Database Table Exports in CSV format
- Additional manually uploaded and filtered datasets including:
  - [ONS postcode database](https://geoportal.statistics.gov.uk/datasets/3be72478d8454b59bb86ba97b4ee325b) - filtered to England-Only postcodes
  - [ONS built-up areas database](https://geoportal.statistics.gov.uk/maps/ons::built-up-areas-web-map-1/aboutONS) - filtered to England-Only
  - [Approved Qualifications List](https://www.qualifications.education.gov.uk/Home/Downloads)
  - [Learning Aim Data](https://submit-learner-data.service.gov.uk/find-a-learning-aim/DownloadData)

Some of this data is uploaded directly to Azure Blob Storage.

Other parts of the data are downloaded directly into Blob Storage, or streamed from APIs.

## Ingestion Methods

This data is then stored into staging tables and combined within a .NET application, which has handlers for each of the three data types:

- FAC ([Find A Course](https://nationalcareers.service.gov.uk/find-a-course) / Publish to the Course Directory) - ingested via flat files
- FAA ([Find an Apprenticeship](https://www.gov.uk/apply-apprenticeship)) - ingested via API
- DU ([Discover Uni](https://www.hesa.ac.uk/support/tools-and-downloads/unistats))

Each of these handlers inherits from a base service, offering the following functionality:

- Validate - check the data files and/or connectivity
- Ingest - load the data into the staging tables
- Sync - update the core and additional tables
- Index - create/update the AI search index and generate text embeddings

There is an additional GeoLocation Handler which imports the following data:

- ONS Postcodes (including latitude and longitude) for postcode lookups
- ONS Built Up Areas (including latitude and longitude) for location name lookups and the autocomplete

## Issues

- The data ingestion can be slow due to the volume of data being processed
- Many of the ingestions require manually downloading, filtering, and uploading data into Blob Storage
- Ingestions based around directly downloaded data are subject to file format changes
- Text embeddings are manually generated and uploaded, which takes time

## Recommendations

Based on what we have learned, and the problems we have faced, we are making the following recommendations:

### Ingestion Methods

- Utilise an ETL/ELT tool such as Microsoft Fabric or Azure Data Factory to handle downloads and data transformation. Tools like this are much better equipped to handle large volumes of data.
- Request an exemption to deploy an instance of Azure OpenAI to be used for text-embedding generation
- Update the core AISearchEntries table in SQL Server and set this as a data-source for AI Search to pull data from directly
  - Leverage SQL Server Change Tracking to determine which rows have changed
  - Leverage the OpenAI instance to have Azure AI Search generate the text-embeddings on-the-fly
- Update the FAC/PttCD system to support a new endpoint to stream all live courses including incremental updates so that this can be ingested directly
